using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using KaupischITC.Extensions;
using KaupischITC.Shared;

namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt ein Diagramm dar
	/// </summary>
	public partial class ChartForm : Form
	{
		/// <summary>
		/// Gibt an, ob die Elemente sortiert werden sollen.
		/// </summary>
		protected bool SortItems { get; set; }

		/// <summary>
		/// Gibt an, ob negative Werte dargestellt werden können.
		/// </summary>
		protected bool SupportsNegativeValues { get; set; }

		/// <summary>
		/// Gibt die darzustellenden Elemente zurück oder legt diese fest.
		/// </summary>
		public IEnumerable<object> Elements { get; set; }

		/// <summary>
		/// Gibt die hervorzuhebenden Elemente zurück oder legt diese fest.
		/// </summary>
		public IEnumerable<object> EmphasizedElements { get; set; }

		/// <summary>
		/// Gibt den Typ der Element zurück oder legt diesen fest.
		/// </summary>
		public Type DisplayedType
		{
			get { return this.propertyBrowserDisplay.DisplayedType; }
			set
			{
				this.propertyBrowserDisplay.DisplayedType = value;
				this.propertyBrowserValue.DisplayedType = value;
			}
		}

		/// <summary>
		/// Gibt ab, woher die Werte für den Beschreibungstext genommen werden.
		/// </summary>
		public string DisplayMember
		{
			get { return this.propertyBrowserDisplay.SelectedProperty; }
			set { this.propertyBrowserDisplay.SelectedProperty = value; }
		}

		/// <summary>
		/// Gibt an, woher die Werte für die Werte genommen werden.
		/// </summary>
		public string ValueMember
		{
			get { return this.propertyBrowserValue.SelectedProperty; }
			set { this.propertyBrowserValue.SelectedProperty = value; }
		}

		/// <summary>
		/// Gibt die Formatierungszeichenfolge für die Werte zurück.
		/// </summary>
		public Func<string,string> GetFormatString { get; set; }



		/// <summary>
		/// Erstellt ein neues Fenster zum Anzeigen eines Diagramms
		/// </summary>
		public ChartForm()
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.GetFormatString = (name) => null;
			this.propertyBrowserValue.TypeFilter = (type) => type.IsNumeric();
			this.propertyBrowserDisplay.SelectedValueChanged += delegate { this.PrintChart(); };
			this.propertyBrowserValue.SelectedValueChanged += delegate { this.PrintChart(); };
			this.numericUpDownPercentageThreshold.ValueChanged += delegate { this.PrintChart(); };
		}

		/// <summary>
		/// Initiales Zeichnen der Diagramms beim Laden
		/// </summary>
		private void PieForm_Load(object sender,EventArgs e)
		{
			this.PrintChart();
		}

		/// <summary>
		/// Erstellt das Diagramm uns zeigt es an
		/// </summary>
		private void PrintChart()
		{
			using (new WaitCursorChanger(this))
				if (this.Elements!=null)
				{
					// Delegaten zum Ermitteln des Anzeigetexts und des Wertes
					Func<object,object> getDisplay = (item) => this.propertyBrowserDisplay.GetSelectedPropertyValue(item);
					Func<object,double?> getValue = (item) =>
					{
						double result = 1;
						if (String.IsNullOrEmpty(this.propertyBrowserValue.SelectedProperty))
							return result;
						else
						{
							object rawValue = this.propertyBrowserValue.GetSelectedPropertyValue(item);
							return (rawValue!=null && Double.TryParse(rawValue.ToString(),out result)) ? result : (double?)null;
						}
					};

					// Delegaten zum Ermitteln der Anzeigetexte
					Func<object,string,string> getFormattedText = (value,formatString) => (value is IFormattable) ? ((IFormattable)value).ToString(formatString,null).Trim() : (value??"").ToString().Trim();
					Func<object,string> getDisplayText = (value) => getFormattedText(value,this.GetFormatString(this.propertyBrowserDisplay.SelectedProperty));
					Func<object,string> getValueText = (value) => (String.IsNullOrEmpty(this.propertyBrowserValue.SelectedProperty)) ? "#:"+value : getFormattedText(value,this.GetFormatString(this.propertyBrowserValue.SelectedProperty));

					// Elemente entsprechend des Beschreibungstextes gruppieren und die Werte zusammenfassen
					var values = this.Elements
						.Select(item => new
						{
							DisplayText = getDisplayText(getDisplay(item)),
							Value = getValue(item),
							IsEmphasized = (this.EmphasizedElements!=null && this.EmphasizedElements.Contains(item))
						})
						.Where(item => item.Value!=null)
						.GroupBy(item => item.DisplayText)
						.Select(group => new
						{
							DisplayText = group.Key,
							Value = (double)group.Sum(item => item.Value),
							IsEmphasized = group.Any(item => item.IsEmphasized)
						})
						.ToList();


					// aggregierte Elemente erstellen
					IEnumerable<ChartItem> chartItems = this.CreateChartItems(
						elements: values,
						getValue: item => item.Value,
						getDisplayText: (item) => item.DisplayText,
						getValueText: (value) => getValueText(value),
						getMergedText: (items,percentage) => items.Count()+" sonstige",
						percentageThreshold: (float)this.numericUpDownPercentageThreshold.Value,
						emphasizedElements: values.Where(item => item.IsEmphasized).ToList());

					// die Diagrammelemente zeichnen
					Bitmap bitmap = (this.SupportsNegativeValues || chartItems.All(v => v.Percent>=0)) ? this.DrawChart(chartItems) : this.CreateErrorImage("Die Darstellung von negativen Werten wird vom aktuellen Diagrammtyp nicht unterstützt.");
					this.pictureBoxPie.Image = bitmap;
					this.pictureBoxPie.Size = bitmap.Size;
				}
		}

		
		/// <summary>
		/// Erzeugt ein Bild mit einer Fehlermeldung 
		/// </summary>
		/// <param name="errorText">der Text der Fehlermeldung</param>
		/// <returns>ein Bild, dass die angegebene Fehlermeldung enthält</returns>
		private Bitmap CreateErrorImage(string errorText)
		{
			Bitmap bitmap = new Bitmap(500,200);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.FillRectangle(Brushes.White,0,0,bitmap.Width,bitmap.Height);
				using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Center })
					graphics.DrawString(errorText,SystemFonts.MessageBoxFont,Brushes.Red,new Rectangle(0,0,bitmap.Width,bitmap.Height),stringFormat);
			}
			return bitmap;
		}


		/// <summary>
		/// Erstellt für ein Diagramm die aggregierten Anzeigeelemente
		/// </summary>
		/// <typeparam name="T">Der Typ der Elemente, die visualisiert werden sollen.</typeparam>
		/// <param name="elements">Auflistung mit den Elementen, die aggregiert/visualisiert werden sollen.</param>
		/// <param name="getValue">Delegat zur Ermittlung der Größe, anhand der der prozentuale Anteil bestimmt wird. </param>
		/// <param name="getDisplayText">Delegat zur Ermittlung der Beschriftung des Namens eines Elements. </param>
		/// <param name="getValueText">Delegat zur Ermittlung der Beschriftung des Wertes eines Elements. </param>
		/// <param name="getMergedText">Delegat zur Ermittlung der Beschriftung der zusammengefassten Elemente. </param>
		/// <param name="percentageThreshold">Prozent-Schwellwert, ab dem ein Element unter "sonstiges" verbucht wird.</param>
		/// <param name="emphasizedElements">Auflistung der Elemente, hervorgehoben werden sollen.</param>
		/// <returns>Eine Auflistung der darzustellenden Diagrammelemente.</returns>
		private IEnumerable<ChartItem> CreateChartItems<T>(IEnumerable<T> elements,Func<T,double> getValue,Func<T,string> getDisplayText,Func<double,string> getValueText,Func<IEnumerable<T>,double,string> getMergedText,float percentageThreshold,IEnumerable<T> emphasizedElements)
		{
			// prozentuale Anteile ermitteln
			double sum = elements.Sum(element => getValue(element));
			var items = elements.Select(element => new { Key = element,Percent = getValue(element)*100/sum });

			// wenn es mehrere Kuchenstücke gibt, deren prozentualer Anteil unter dem Schwellwert liegen, diese zu einem einzigen Stück zusammenfassen
			var slicesToMerge = items.Where(item => item.Percent<percentageThreshold && !emphasizedElements.Contains(item.Key)).ToList();
			var slices =
				items.Where(item => slicesToMerge.Count()<=1 || !slicesToMerge.Contains(item))
				.Select(item => new ChartItem
				{
					DisplayText = getDisplayText(item.Key),
					ValueText = getValueText(getValue(item.Key)),
					Percent = item.Percent,
					IsEmphasized = emphasizedElements.Contains(item.Key)
				});
			// wenn gewünscht, sortieren
			if (this.SortItems)
				slices = slices.OrderByDescending(item => item.Percent);

			// die Elemente für "sonstige" zusammenfassen und als separates Element anhängen
			slices = slices
				.Concat(slicesToMerge.Where(item => slicesToMerge.Count()>1).Take(1)
				.Select(item => new ChartItem
				{
					DisplayText = getMergedText(slicesToMerge.Select(i => i.Key),slicesToMerge.Sum(i => i.Percent)),
					ValueText = "Σ:"+getValueText(slicesToMerge.Sum(i => getValue(i.Key))), // ∑Σ
					Percent = slicesToMerge.Sum(i => i.Percent),
					IsEmphasized = emphasizedElements.Contains(item.Key)
				}));

			return slices;
		}


		/// <summary>
		/// Zeichnet das darzustellende Diagramm
		/// </summary>
		/// <param name="items">die Elemente, die gezeichnet werden sollen-</param>
		/// <returns>ein Bild, das das gezeichnete Diagramm enthält</returns>
		protected virtual Bitmap DrawChart(IEnumerable<ChartItem> items)
		{
			throw new NotImplementedException("Diese Methode muss überschrieben werden.");
		}


		/// <summary>
		/// Generiert eine Farbe
		/// </summary>
		/// <param name="index">Index der Farbe</param>
		/// <param name="count">Anzahl der Farben, die generiert werden sollen/können.</param>
		/// <returns>generierte Farbe</returns>
		protected static Color GenerateColor(int index,int count)
		{
			// einmal quer durch die Farbtöne im HSB-Farbraum
			float angle = (index/(float)count) * 360;
			return ColorExtensions.GetColorFromHSB(angle,1,1);
		}


		/// <summary>
		/// Das aktuelle Bild in die Zwischenablage einfügen
		/// </summary>
		private void buttonToClipboard_Click(object sender,EventArgs e)
		{
			Clipboard.SetImage(this.pictureBoxPie.Image);
		}
		/// <summary>
		/// Button für die Zwischenablage nur anzeigen, wenn sich die Maus über dem Diagrammbild befindet
		/// </summary>
		private void pictureBoxPie_MouseEnter(object sender,EventArgs e)
		{
			this.buttonToClipboard.Location = new Point(this.pictureBoxPie.Location.X+this.pictureBoxPie.Width-this.buttonToClipboard.Width-5,this.pictureBoxPie.Location.Y+5);
			this.buttonToClipboard.Visible = true;
		}
		private void pictureBoxPie_MouseLeave(object sender,EventArgs e)
		{
			if (!this.pictureBoxPie.Bounds.Contains(this.PointToClient(Control.MousePosition)))
				this.buttonToClipboard.Visible = false;
		}
	}
}
