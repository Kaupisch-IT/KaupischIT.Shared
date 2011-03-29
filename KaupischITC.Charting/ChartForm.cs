﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KaupischITC.Charting;
using KaupischITC.Extensions;
using KaupischITC.Shared;

namespace KaupischITC.Charting
{
	public partial class ChartForm : Form
	{
		protected bool SortItems { get; set; }

		public IEnumerable<object> Elements { get; set; }

		public IEnumerable<object> EmphasizedElements { get; set; }

		public Type DisplayedType
		{
			get { return this.propertyBrowserDisplay.DisplayedType; }
			set
			{
				this.propertyBrowserDisplay.DisplayedType = value;
				this.propertyBrowserValue.DisplayedType = value;
			}
		}

		public string DisplayMember
		{
			get { return this.propertyBrowserDisplay.SelectedProperty; }
			set { this.propertyBrowserDisplay.SelectedProperty = value; }
		}

		public string ValueMember
		{
			get { return this.propertyBrowserValue.SelectedProperty; }
			set { this.propertyBrowserValue.SelectedProperty = value; }
		}

		public Func<string,string> GetFormatString { get; set; }

		public ChartForm()
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.GetFormatString = (name) => null;
			this.propertyBrowserValue.TypeFilter = (type) => type.IsNumeric();
			this.propertyBrowserDisplay.SelectedValueChanged += delegate { this.PrintPieChart(); };
			this.propertyBrowserValue.SelectedValueChanged += delegate { this.PrintPieChart(); };
			this.numericUpDownPercentageThreshold.ValueChanged += delegate { this.PrintPieChart(); };
		}

		private void PieForm_Load(object sender,EventArgs e)
		{
			this.PrintPieChart();
		}

		private void PrintPieChart()
		{
			using (new WaitCursorChanger(this))
				if (!String.IsNullOrEmpty(this.propertyBrowserValue.SelectedProperty))
				{
					Func<object,object> getDisplay = (item) => this.propertyBrowserDisplay.GetSelectedPropertyValue(item);
					Func<object,double?> getValue = (item) =>
					{
						object rawValue = this.propertyBrowserValue.GetSelectedPropertyValue(item);
						double result;
						return (rawValue!=null && Double.TryParse(rawValue.ToString(),out result)) ? result : (double?)null;
					};

					Func<object,string,string> getFormattedText = (value,formatString) => (value is IFormattable) ? ((IFormattable)value).ToString(formatString,null).Trim() : (value??"").ToString().Trim();
					Func<object,string> getDisplayText = (value) => getFormattedText(value,this.GetFormatString(this.propertyBrowserDisplay.SelectedProperty));
					Func<object,string> getValueText = (value) => getFormattedText(value,this.GetFormatString(this.propertyBrowserValue.SelectedProperty));

					var values = this.Elements
						.Select(item => new { DisplayText = getDisplayText(getDisplay(item)),Value = getValue(item),IsEmphasized = (this.EmphasizedElements!=null && this.EmphasizedElements.Contains(item)) })
						.Where(item => item.Value!=null)
						.GroupBy(item => item.DisplayText)
						.Select(group => new { DisplayText = group.Key,Value = (double)group.Sum(item => item.Value),IsEmphasized = group.Any(item => item.IsEmphasized) })
						.ToList();
					
					Bitmap bitmap = this.DrawPercent(
						values,
						item => item.Value,
						(item) => item.DisplayText,
						(value) => getValueText(value),
						(items,percentage) => items.Count()+" sonstige",
						(float)this.numericUpDownPercentageThreshold.Value,
						values.Where(item => item.IsEmphasized).ToList());

					this.pictureBoxPie.Image = bitmap;
					this.pictureBoxPie.Size = bitmap.Size;
				}
		}

		
		/// <summary>
		/// Erstellt ein Kreisdiagramm für die übergebenen Elemente, die mit dem jeweiligen prozentualen Anteil beschriftet sind
		/// </summary>
		/// <typeparam name="T">Der Typ der Elemente, die visualisiert werden sollen.</typeparam>
		/// <param name="pieChart">Komponente zum Zeichnen von Kreisdiagrammen</param>
		/// <param name="elements">Auflistung mit den Elementen, die als Kuchenstücke visualisiert werden sollen.</param>
		/// <param name="getValue">Delegat zur Ermittlung der Größe, anhand der der prozentuale Anteil bestimmt wird. </param>
		/// <param name="getText">Delegat zur Ermittlung der Beschriftung eines Elements. (Der prozentuale Anteil dieses Elements wird mit übergeben.) </param>
		/// <param name="getMergedText">Delegate zur Ermitllung der Beschriftung der zusammengefassten Elemente. (Der prozentuale Anteil dieser Elemente wird mit übergeben.) </param>
		/// <param name="percentageThreshold">Prozent-Schwellwert, ab dem ein Element unter "sonstiges" verbucht wird.</param>
		/// <param name="emphasizedElements">Auflistung der Elemente, die durch weiteres Herausschieben aus der Mitte des Kreisdiagramms hervorgehoben werden sollen.</param>
		/// <returns>Bitmap-Objekt, auf das das Kreisdiagramm gezeichnet wurde.</returns>
		private Bitmap DrawPercent<T>(IEnumerable<T> elements,Func<T,double> getValue,Func<T,string> getDisplayText,Func<double,string> getValueText,Func<IEnumerable<T>,double,string> getMergedText,float percentageThreshold,IEnumerable<T> emphasizedElements)
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

			if (this.SortItems)
				slices = slices.OrderByDescending(item => item.Percent);

			slices = slices
				.Concat(slicesToMerge.Where(item => slicesToMerge.Count()>1).Take(1)
				.Select(item => new ChartItem
				{
					DisplayText = getMergedText(slicesToMerge.Select(i => i.Key),slicesToMerge.Sum(i => i.Percent)),
					ValueText = "Σ:"+getValueText(slicesToMerge.Sum(i => getValue(i.Key))), // ∑Σ
					Percent = slicesToMerge.Sum(i => i.Percent),
					IsEmphasized = emphasizedElements.Contains(item.Key) 
				}));

			return this.DrawChart(slices);
		}


		protected virtual Bitmap DrawChart(IEnumerable<ChartItem> slices)
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
				
		private void buttonToClipboard_Click(object sender,EventArgs e)
		{
			Clipboard.SetImage(this.pictureBoxPie.Image);
		}

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