using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using KaupischIT.Charting;
using KaupischIT.Extensions;
using KaupischIT.InfragisticsControls.ValueAppearances;
using KaupischIT.Shared;

namespace KaupischIT.InfragisticsControls
{
	/// <summary>
	/// Stellt ein angepasstes UltraGrid-Steuerelement dar
	/// </summary>
	public partial class CustomizedUltraGrid : UltraGrid, IUIElementDrawFilter
	{
		private static ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
		private Timer timer = new Timer();                      // Timer für den WaitCursor
		private List<ExpandedGroupByRow> expandedRowsState;     // enthält ggf. die gespeicherten ausgeklappten Gruppierungszeilen

		private ToolStripMenuItem summaryToolStripMenuItem;     // Kontextmenüeintrag für Zusammenfassungen
		private ToolStripMenuItem formatToolStripMenuItem;      // Kontextmenüeintrag für Wertformatierungen
		private ToolStripMenuItem fontToolStripMenuItem;        // Kontextmenüeintrag für Schriftformatierungen
		private ToolStripMenuItem visualizationToolStripMenuItem; // Kontextmenüeintrag für Visualisierungen
		private UrlProtocolHandler urlProtocolHandler = new UrlProtocolHandler(); // der UrlProtocolHandler für benutzerdefinierte Kontextmenüeinträge

		// alle verfügbaren Zusammenfassungen mit Beschreibungstext und Symbol
		private Dictionary<SummaryType,string[]> availableSummaries = new Dictionary<SummaryType,string[]>()
		{
			{ SummaryType.Sum, new [] { "Summe","Σ" }},
			{ SummaryType.Average, new [] {"Durchschnitt","Ø" }},
			{ SummaryType.Count,new [] { "Anzahl","#" }},
			{ SummaryType.Minimum, new [] {"Minimum","Min" }},
			{ SummaryType.Maximum, new [] {"Maximum","Max" }}
		};

		// alle verfügbaren vordefinierten Wertformatierungen mit Beschreibungstext
		private Dictionary<string,string> availableFormats = new Dictionary<string,string>()
		{
			{ "","keine Formatierung" },
			{ "N0","Zahl (mit Tausendertrennzeichen)" },
			{ "C", "Währung" },
			{ "P", "Prozent" },
			{ "0.##\\%", "Prozent 2" },
			{ "dd.MM.yyyy", "Datum" },
			{ "HH:mm", "Uhrzeit" },
			{ "dd.MM.yyyy HH:mm", "Datum und Uhrzeit" },
			{ "0.## Std", "Stunden" }
		};


		/// <summary> Gibt das Kontextmenü, dass beim Klicken in ein Zelle, die zu einer Spalte gehört, angezeigt wird, zurück </summary>
		public ContextMenuStrip ColumnCellContextMenuStrip { get; private set; }

		/// <summary> Gibt das Kontextmenü, das beim Klicken auf den Zeilenselektor angezeigt wird, zurück </summary>
		public ContextMenuStrip RowSelectorContextMenuStrip { get; private set; }

		/// <summary> Gibt die Zeile zurück, auf die in diesem Kontext geklickt wurde </summary>
		public UltraGridRow ContextUltraGridRow { get; private set; }

		/// <summary> Gibt die Zelle zurück, auf die in diesem Kontext geklickt wurde </summary>
		public UltraGridCell ContextUltraGridCell { get; private set; }

		/// <summary> Gibt den Spaltenkopf zurück, auf den in diesem Kontext geklickt wurde </summary>
		public HeaderUIElement ContextHeaderUIElement { get; private set; }

		/// <summary> Gibt die Spalte zurück, auf die in diesem Kontext geklickt wurde </summary>
		public UltraGridColumn ContextUltraGridColumn { get; private set; }

		/// <summary> Gibt die Zusammenfassung zurück, auf die in diesem Kontext geklickt wurde </summary>
		public SummaryFooterUIElement ContextSummaryFooterUIElement { get; private set; }

		/// <summary> Gibt die Grafik für aufsteigend sortierte Spalten zurück oder legt diese fest </summary>
		public Image SortIndicatorImageAscending { get; set; }

		/// <summary> Gibt die Grafik für absteigend sortierte Spalten zurück oder legt diese fest </summary>
		public Image SortIndicatorImageDescending { get; set; }


		/// <summary> Stellt Informationen über Ereignisdaten bereit, bei denen eine Spalte betroffen ist </summary>
		public class UltraGridColumnEventArgs : EventArgs { public UltraGridColumn Column { get; set; } }

		/// <summary> Wird ausgelöst, wenn das Werteformat einer Spalte geändert wird </summary>
		public event EventHandler<UltraGridColumnEventArgs> ColumnFormatChanged;

		/// <summary> Wird ausgelöst, wenn die Beschriftung einer Spalte geändert wird </summary>
		public event EventHandler<UltraGridColumnEventArgs> ColumnCaptionChanged;



		/// <summary>
		/// Gibt an, ob der Zeilenfilter aktiviert ist, oder legt dieses fest
		/// </summary>
		public bool AllowRowFiltering
		{
			get => (this.DisplayLayout.Override.AllowRowFiltering==DefaultableBoolean.True);
			set
			{
				this.DisplayLayout.Override.AllowRowFiltering = (value) ? DefaultableBoolean.True : DefaultableBoolean.False;
				if (!value)
					foreach (UltraGridBand band in this.DisplayLayout.Bands)
						band.ColumnFilters.ClearAllFilters();
			}
		}



		/// <summary>
		/// Erstellt ein neues angepasstes UltraGrid-Steuerelement
		/// </summary>
		public CustomizedUltraGrid()
		{
			this.DrawFilter = this;
			this.Font = SystemFonts.MessageBoxFont;

			this.InitializeComponent();

			// Kontextmenüs initialisieren
			this.InitializeColumContextMenuStrip();
			this.InitializeRowContextMenuStrip();

			// Ressourcen initialisieren
			ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
			this.SortIndicatorImageAscending = (Image)resources.GetObject("Up");
			this.SortIndicatorImageDescending = (Image)resources.GetObject("Down");
			Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("ColumnChooserButtonToolTip","Spalten auswählen");
			Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("FilterClearButtonToolTip_FilterCell","Klicken Sie hier, um den Filterwert für die Spalte '{0}' zu entfernen");
			Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("FilterClearButtonToolTip_RowSelector","Klicken Sie hier, um alle Filterwerte zu entfernen");

			// Layout initialisieren
			this.OnInitializeLayout(new InitializeLayoutEventArgs(this.DisplayLayout));
		}


		/// <summary>
		/// Initialisierung des Layouts des Grids
		/// </summary>
		protected override void OnInitializeLayout(InitializeLayoutEventArgs e)
		{
			using (new WaitCursorChanger(this))
			{
				this.DisplayLayout.MaxBandDepth = 5; // bei zyklischen Datenstrukturen hängt das Grid gern mal beim Laden. 5 scheint relativ in Ordnung zu sein: Viel tiefer wird (eigentlich) nie aufgeklappt; schon bei 7 kann es sein, dass das Grid hängen bleibt
				this.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

				// diverse Einstellungen für Aussehen & Verhalten
				this.DisplayLayout.GroupByBox.Prompt = "Ziehen Sie einen Spaltenkopf hierher, um nach diesem zu gruppieren.";
				this.DisplayLayout.Override.CellClickAction = CellClickAction.EditAndSelectText;
				this.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
				this.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
				this.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
				this.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
				this.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.RowIndex;
				this.DisplayLayout.Override.RowSelectorAppearance.TextTrimming = TextTrimming.None;
				this.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
				this.DisplayLayout.Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
				this.DisplayLayout.Override.HeaderAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
				this.DisplayLayout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
				this.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
				this.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
				this.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinBand;
				this.DisplayLayout.Override.CellMultiLine = DefaultableBoolean.False;
				this.SyncWithCurrencyManager = false;
				this.DisplayLayout.Override.AllowMultiCellOperations = AllowMultiCellOperation.CopyWithHeaders;
				this.DisplayLayout.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
				this.DisplayLayout.Override.RowSelectorHeaderAppearance.Image = this.imageList.Images["columnDetail"];

				this.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
				this.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
				this.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;
				this.DisplayLayout.Override.FilterOperandStyle = FilterOperandStyle.Edit;
				this.DisplayLayout.Override.FilterOperatorLocation = FilterOperatorLocation.Hidden;
				this.DisplayLayout.Override.FilterRowAppearance.Image = this.imageList.Images["FilterTextbox"];
				this.DisplayLayout.Override.FilterClearButtonAppearance.Image = this.imageList.Images["DeleteFilter"];

				// Anpassung diverser Farben und Zeichenstile zur Vereinheitlichung des Aussehens mit den DevExpress-Steuerelementen
				Color borderColor = Color.FromArgb(202,203,211);
				Color selectedBackColor = Color.FromArgb(226,234,253);
				Color selectedForeColor = SystemColors.ControlText;
				Color groupByBoxBackColor = Color.FromArgb(235,236,239);
				Color groupByBoxForeColor = Color.FromArgb(192,192,202);
				Color headerBackColor1 = Color.FromArgb(248,248,248);
				Color headerBackColor2 = Color.FromArgb(242,242,242);
				Color summaryBackColor = Color.FromArgb(242,246,251);
				this.DisplayLayout.DefaultSelectedBackColor = selectedBackColor;
				this.DisplayLayout.DefaultSelectedForeColor = selectedForeColor;
				this.DisplayLayout.GroupByBox.Style = GroupByBoxStyle.Compact;
				this.DisplayLayout.GroupByBox.Appearance.BackColor = groupByBoxBackColor;
				this.DisplayLayout.GroupByBox.PromptAppearance.BackColor = groupByBoxBackColor;
				this.DisplayLayout.GroupByBox.PromptAppearance.ForeColor = groupByBoxForeColor;
				this.DisplayLayout.GroupByBox.PromptAppearance.FontData.Name = this.Font.Name;
				this.DisplayLayout.GroupByBox.PromptAppearance.FontData.SizeInPoints = this.Font.SizeInPoints;
				this.DisplayLayout.GroupByBox.ButtonBorderStyle = UIElementBorderStyle.Solid;
				this.DisplayLayout.Override.HeaderAppearance.BorderColor = borderColor;
				this.DisplayLayout.Override.HeaderAppearance.BackColor = headerBackColor1;
				this.DisplayLayout.Override.HeaderAppearance.BackColor2 = headerBackColor2;
				this.DisplayLayout.Override.HeaderAppearance.BackGradientStyle = GradientStyle.Vertical;
				this.DisplayLayout.Override.BorderStyleSummaryFooter = UIElementBorderStyle.None;
				this.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.GroupByRowsFooter|SummaryDisplayAreas.InGroupByRows|SummaryDisplayAreas.TopFixed;
				this.DisplayLayout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
				this.DisplayLayout.Override.SummaryValueAppearance.BackColor = summaryBackColor;
				this.DisplayLayout.Override.SummaryValueAppearance.BorderColor = borderColor;
				this.DisplayLayout.Override.RowAppearance.BorderColor = borderColor;

				this.SuspendRowSynchronization();

				// Anpassungen für Bänder
				foreach (UltraGridBand ultraGridBand in e.Layout.Bands)
				{
					ultraGridBand.HeaderVisible = (ultraGridBand.Index!=0 && ultraGridBand.Key!="Elements");
					ultraGridBand.Header.Appearance.TextHAlign = HAlign.Left;
					ultraGridBand.RowLayoutStyle = RowLayoutStyle.None;

					// Anpassungen für Spalten
					foreach (UltraGridColumn ultraGridColumn in ultraGridBand.Columns)
					{
						ultraGridColumn.CellActivation = Activation.ActivateOnly;
						ultraGridColumn.HiddenWhenGroupBy = DefaultableBoolean.True;
						if (ultraGridColumn.ValueBasedAppearance==null)
							ultraGridColumn.ValueBasedAppearance = new ValueAppearance { HighlightNegativeValues = ultraGridColumn.DataType.IsNumeric() };

						if (ultraGridColumn.Format==null) // voreingestellte Wertformatierungen bei bestimmten Spaltennamen
						{
							if (ultraGridColumn.Key.EndsWith("Preis",StringComparison.InvariantCultureIgnoreCase))
								this.SetColumnFormat(ultraGridColumn,"C");
							else if (Regex.IsMatch(ultraGridColumn.Key,"[^A-Z]P$"))
								this.SetColumnFormat(ultraGridColumn,"P");
						}

						if (ultraGridColumn.DataType.IsNumeric())
						{
							ultraGridColumn.CellAppearance.TextHAlign = HAlign.Right;// numerische Werte rechtsbündig
							if (ultraGridColumn.Format==null)
								this.SetColumnFormat(ultraGridColumn,(ultraGridColumn.DataType==typeof(int) || ultraGridColumn.DataType==typeof(int?)) ? "" : "#,0.###");
						}

						// bestimmte Spalten nicht anzeigen: Spezielle Postfixes und komplexe Datentypen
						string[] hiddenPostfixes = { "ID","Id","Key","_" };
						if (hiddenPostfixes.Any(pf => ultraGridColumn.Key.EndsWith(pf)) || ultraGridColumn.PropertyDescriptor.Attributes.OfType<BrowsableAttribute>().Any(ba => !ba.Browsable))
							ultraGridColumn.Hidden = true;
						else if (ultraGridColumn.DataType!=typeof(string) && !ultraGridColumn.DataType.IsValueType)
							ultraGridColumn.Hidden = true;
						else
							ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
					}
				}
			}

			base.OnInitializeLayout(e);
		}


		/// <summary>
		/// Initialisiert das Kontextmenü, dass beim Klicken in eine Zelle, die zu einer Spalte gehört, angezeigt wird
		/// </summary>
		private void InitializeColumContextMenuStrip()
		{
			this.ColumnCellContextMenuStrip = new ContextMenuStrip();

			// Spaltenbezeichnung
			ToolStripTextBox toolStripTextBoxCaption = new ToolStripTextBox("HeaderCaption");
			toolStripTextBoxCaption.TextChanged += delegate
			{
				if (this.ContextUltraGridColumn!=null)
					this.ContextUltraGridColumn.Header.Caption = toolStripTextBoxCaption.Text;

				if (this.ColumnCaptionChanged!=null && this.ContextUltraGridColumn!=null)
					this.ColumnCaptionChanged(this,new UltraGridColumnEventArgs { Column = this.ContextUltraGridColumn });
			};
			this.ColumnCellContextMenuStrip.Items.Add(toolStripTextBoxCaption);

			// Werte formatieren als
			this.formatToolStripMenuItem = (ToolStripMenuItem)this.ColumnCellContextMenuStrip.Items.Add("Werte formatieren als");
			foreach (string formatString in this.availableFormats.Keys)
				this.formatToolStripMenuItem.DropDownItems.Add(this.availableFormats[formatString],null,this.FormatMenuItem_Click).Tag = formatString; // vordefinierte Formate
																																					   // eigenes Format
			ToolStripTextBox toolStripTextBox = new ToolStripTextBox("Custom");
			toolStripTextBox.TextChanged += delegate
			{
				toolStripTextBox.Tag = toolStripTextBox.Text;
				this.FormatMenuItem_Click(toolStripTextBox,EventArgs.Empty);
			};
			toolStripTextBox.Click += delegate
			{ this.FormatMenuItem_Click(toolStripTextBox,EventArgs.Empty); };
			this.formatToolStripMenuItem.DropDownItems.Add(toolStripTextBox);

			// Zusammenfassungen
			this.summaryToolStripMenuItem = (ToolStripMenuItem)this.ColumnCellContextMenuStrip.Items.Add("Zusammenfassung hinzufügen");
			foreach (SummaryType summaryType in this.availableSummaries.Keys)
				this.summaryToolStripMenuItem.DropDownItems.Add(this.availableSummaries[summaryType][0],null,this.SummaryMenuItem_Click).Tag = summaryType;

			// Formatierungen
			this.fontToolStripMenuItem = (ToolStripMenuItem)this.ColumnCellContextMenuStrip.Items.Add("Text formatieren");
			this.fontToolStripMenuItem.DropDownItems.Add("Fett",null,delegate (object sender,EventArgs e)
			{ this.ContextUltraGridColumn.CellAppearance.FontData.Bold = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Kursiv",null,delegate (object sender,EventArgs e)
			{ this.ContextUltraGridColumn.CellAppearance.FontData.Italic =((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Unterstrichen",null,delegate (object sender,EventArgs e)
			{ this.ContextUltraGridColumn.CellAppearance.FontData.Underline = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });

			// wertbasierte Formatierungen
			this.fontToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
			this.fontToolStripMenuItem.DropDownItems.Add("negative Werte rot",null,delegate (object sender,EventArgs e)
			{ ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).HighlightNegativeValues = !((ToolStripMenuItem)sender).Checked; });
			this.fontToolStripMenuItem.DropDownItems.Add("Tendenzpfeile",null,delegate (object sender,EventArgs e)
			{ ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).ShowTrendIndicators = !((ToolStripMenuItem)sender).Checked; });

			// Visualisierungen
			this.visualizationToolStripMenuItem = (ToolStripMenuItem)this.ColumnCellContextMenuStrip.Items.Add("Visualisierung");
			this.visualizationToolStripMenuItem.DropDownItems.Add("Kreisdiagramm anzeigen",null,delegate
			{ this.ShowChartForm(new PieChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Balkendiagramm anzeigen",null,delegate
			{ this.ShowChartForm(new BarChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Flächendiagramm anzeigen",null,delegate
			{ this.ShowChartForm(new TreeMapForm()); });
		}

		/// <summary>
		/// Wenn auf einen Kontextmenüeintrag geklickt wird, der einen (Spalten-)Zusammenfassung darstellt
		/// </summary>
		private void SummaryMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			SummaryType summaryType = (SummaryType)menuItem.Tag;

			if (!menuItem.Checked)
				this.AddColumnSummary(this.ContextUltraGridColumn,summaryType);
			else
				this.RemoveColumnSummary(this.ContextUltraGridColumn,summaryType);
		}
		/// <summary> Fügt einer Spalte eine Zusammenfassung hinzu </summary>
		public void AddColumnSummary(UltraGridColumn column,SummaryType summaryType)
		{
			SummarySettings summarySettings = column.Band.Summaries.Add(summaryType,column);
			summarySettings.DisplayFormat = this.GetColumnSummaryFormat(column.Format,summaryType);
			summarySettings.Appearance.TextHAlign = HAlign.Right;
			summarySettings.GroupBySummaryValueAppearance.TextHAlign = HAlign.Right;
		}
		/// <summary> Entfernt die Zusammenfassung einer Spalte </summary>
		public void RemoveColumnSummary(UltraGridColumn column,SummaryType summaryType)
		{
			foreach (SummarySettings summarySettings in column.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SummaryType==summaryType && ss.SourceColumn==column))
				this.ContextUltraGridColumn.Band.Summaries.Remove(summarySettings);
		}

		/// <summary>
		/// Wenn auf einen Kontextmenüeintrag zum Formatieren von Spaltenwerten geklickt wird
		/// </summary>
		private void FormatMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripItem menuItem = (ToolStripItem)sender;
			string format = (string)menuItem.Tag;

			this.SetColumnFormat(this.ContextUltraGridColumn,format);
		}
		/// <summary> Richtet das angegebene Format für die angegebene Spalte und alle zugehörigen Zusammenfassungsspalten ein </summary>
		public void SetColumnFormat(UltraGridColumn column,string format)
		{
			column.Format = format;

			foreach (SummarySettings summarySettings in column.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SourceColumn==column))
				summarySettings.DisplayFormat = this.GetColumnSummaryFormat(format,summarySettings.SummaryType);

			this.ColumnFormatChanged?.Invoke(this,new UltraGridColumnEventArgs { Column = column });
		}

		/// <summary>
		/// Ermittelt das spezifische Format für eine Spalten-Zusammenfassung
		/// </summary>
		private string GetColumnSummaryFormat(string cellFormat,SummaryType summaryType)
		{
			string summarySymbol = this.availableSummaries[summaryType][1];
			return summarySymbol+": " + ((summaryType!=SummaryType.Count) ? "{0:"+cellFormat+"}" : "{0:N0}");
		}


		/// <summary>
		/// Wenn auf einen Kontextmenüeintrag zum Anzeigen einer Visualisierung geklickt wird
		/// </summary>
		private void ShowChartForm(ChartForm chartForm)
		{
			List<object> elements = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Where(ugr => !ugr.Hidden && !ugr.IsFilteredOut).Select(ugr => ugr.ListObject).ToList();
			if (elements.Any())
			{
				chartForm.DisplayedType = elements.First().GetType();
				chartForm.Elements = elements;
				chartForm.EmphasizedElements = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Where(ugr => ugr.Selected).Select(ugr => ugr.ListObject).ToList();
				chartForm.ShowInTaskbar = false;
				chartForm.DisplayMember = this.ContextUltraGridColumn.Band.GetFirstVisibleCol(this.ActiveColScrollRegion,true).Key;
				chartForm.ValueMember = this.ContextUltraGridColumn.Key;
				chartForm.GetFormatString = (columnName) => (this.ContextUltraGridColumn.Band.Columns.Exists(columnName)) ? this.ContextUltraGridColumn.Band.Columns[columnName].Format : null;

				chartForm.Show(this);
			}
		}


		/// <summary>
		/// Initialisiert das Kontextmenü, das angezeigt wird, wenn auf einen Zeilenselektor geklickt wird
		/// </summary>
		private void InitializeRowContextMenuStrip()
		{
			this.RowSelectorContextMenuStrip = new ContextMenuStrip();

			this.RowSelectorContextMenuStrip.Items.Add("Erweitern",null,delegate
			{ this.ContextUltraGridRow.Expanded = true; });
			this.RowSelectorContextMenuStrip.Items.Add("Reduzieren",null,delegate
			{ this.ContextUltraGridRow.Expanded = false; });
			this.RowSelectorContextMenuStrip.Items.Add("-");
			this.RowSelectorContextMenuStrip.Items.Add("Alles erweitern",null,delegate
			{ this.ContextUltraGridRow.ParentCollection.ExpandAll(false); });
			this.RowSelectorContextMenuStrip.Items.Add("Alles reduzieren",null,delegate
			{ this.ContextUltraGridRow.ParentCollection.CollapseAll(false); });
		}


		/// <summary>
		/// Nach einem Rechtsklick in das UltraGrid die Kontext-Objekte aktualisieren und ggf. eine Kontextmenü anzeigen
		/// </summary>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button==MouseButtons.Right)
			{
				Point mousePoint = new Point(e.X,e.Y);
				UIElement element = this.DisplayLayout.UIElement.ElementFromPoint(mousePoint);
				if (element!=null)
				{
					this.ContextUltraGridRow = (UltraGridRow)element.GetContext(typeof(UltraGridRow));              // geklickte Zeile
					this.ContextUltraGridCell = (UltraGridCell)element.GetContext(typeof(UltraGridCell));           // geklickte Zelle
					this.ContextHeaderUIElement = (HeaderUIElement)element.GetAncestor(typeof(HeaderUIElement));    // geklickter Spaltenkopf
					this.ContextUltraGridColumn = (UltraGridColumn)element.GetContext(typeof(UltraGridColumn));     // geklickte Spalte
					this.ContextSummaryFooterUIElement = (SummaryFooterUIElement)element.GetAncestor(typeof(SummaryFooterUIElement)); // geklickter Spaltenfuß

					// Kontextmenü für Spalten
					if (this.ContextUltraGridColumn!=null)
					{
						this.ColumnCellContextMenuStrip.Items["HeaderCaption"].Text = this.ContextUltraGridColumn?.Header.Caption;

						// "Zusammenfassungen"
						if (this.ContextUltraGridColumn!=null)
							foreach (ToolStripMenuItem menuItem in this.summaryToolStripMenuItem.DropDownItems)
							{
								SummaryType summaryType = (SummaryType)menuItem.Tag;
								menuItem.Checked = this.ContextUltraGridColumn.Band.Summaries.Cast<SummarySettings>().Any(ss => ss.SourceColumn==this.ContextUltraGridColumn && ss.SummaryType==summaryType);
								menuItem.Visible = this.ContextUltraGridColumn.DataType.IsNumeric() || summaryType==SummaryType.Count;
							}

						// "Formatieren als"
						if (this.ContextUltraGridColumn!=null)
						{
							foreach (ToolStripMenuItem menuItem in this.formatToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
								menuItem.Checked = (this.ContextUltraGridColumn.Format==(string)menuItem.Tag);
							this.formatToolStripMenuItem.DropDownItems["Custom"].Text = this.ContextUltraGridColumn.Format;
						}

						// "Text formatieren"
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[0]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Bold==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[1]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Italic==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[2]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Underline==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[4]).Checked = ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).HighlightNegativeValues;
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[5]).Checked = ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).ShowTrendIndicators;

						// "Visualisierung anzeigen"
						this.visualizationToolStripMenuItem.Enabled = (this.ContextUltraGridRow!=null);

						// Protokoll-Handler
						this.AddUrlProtocolHandlerItems(this.ColumnCellContextMenuStrip);

						this.ColumnCellContextMenuStrip.Show(this,mousePoint);
					}
					// Kontextmenü für Zeilen
					else if (this.ContextUltraGridRow!=null)
					{
						// "Erweitern"/"Reduzieren"
						bool canExpand = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Any(row => row.IsExpandable);
						this.RowSelectorContextMenuStrip.Items[0].Available = canExpand && this.ContextUltraGridRow.IsExpandable && !this.ContextUltraGridRow.IsExpanded;
						this.RowSelectorContextMenuStrip.Items[1].Available = canExpand && this.ContextUltraGridRow.IsExpandable && this.ContextUltraGridRow.IsExpanded;
						this.RowSelectorContextMenuStrip.Items[2].Available = canExpand && this.ContextUltraGridRow.IsExpandable;
						this.RowSelectorContextMenuStrip.Items[3].Available = canExpand;
						this.RowSelectorContextMenuStrip.Items[4].Available = canExpand;

						// Protokoll-Handler
						this.AddUrlProtocolHandlerItems(this.RowSelectorContextMenuStrip);

						if (this.RowSelectorContextMenuStrip.Items.Count>5)
							this.RowSelectorContextMenuStrip.Items[5].Available = !(this.RowSelectorContextMenuStrip.Items[5] is ToolStripSeparator);

						if (this.RowSelectorContextMenuStrip.Items.Cast<ToolStripItem>().Any(mi => mi.Available))
							this.RowSelectorContextMenuStrip.Show(this,mousePoint);
					}
				}
			}
			base.OnMouseUp(e);
		}


		/// <summary>
		/// Registriert die verfügbaren UrlProtocolHandler in einem Kontextmenü
		/// </summary>
		private void AddUrlProtocolHandlerItems(ContextMenuStrip contextMenuStrip)
		{
			// alle alten Einträge entfernen...
			foreach (ToolStripItem item in contextMenuStrip.Items.Cast<ToolStripItem>().Where(tsi => tsi.Tag==this.urlProtocolHandler).ToList())
				contextMenuStrip.Items.Remove(item);

			// ... und nur Einträge für Protokoll-Handler erzeugen, die ausführbar sind
			if (this.ContextHeaderUIElement==null)
			{
				List<UrlProtocolHandler.ConcreteRoute> validRoutes = this.urlProtocolHandler.GetValidRoutes(this.ContextUltraGridRow).ToList();
				if (validRoutes.Any())
				{
					contextMenuStrip.Items.Add("-").Tag = this.urlProtocolHandler;
					foreach (UrlProtocolHandler.ConcreteRoute r in validRoutes)
					{
						UrlProtocolHandler.ConcreteRoute route = r;
						contextMenuStrip.Items.Add(route.Name,null,delegate
						{ route.Invoke(); }).Tag = this.urlProtocolHandler;
					}
				}
			}
		}



		/// <summary>
		/// Führt eine automatische Breitenanpassung der Spalten aus
		/// </summary>
		public void AutoResizeColumns()
		{
			foreach (UltraGridBand band in this.DisplayLayout.Bands)
			{
				// für die automatische Breitenanpassung eine maximalbreite für die Spalten verwenden (sonst wird es manchmal sehr unübersichtlich) -> dazu die alten Maximalbreiten merken
				Dictionary<UltraGridColumn,int> oldMaxWidths = new Dictionary<UltraGridColumn,int>(band.Columns.Count);
				foreach (UltraGridColumn column in band.Columns)
				{
					oldMaxWidths[column] = column.MaxWidth;
					column.MaxWidth = 300;
				}

				// automatische Spaltenbreitenanpassung durchführen
				band.PerformAutoResizeColumns(false,PerformAutoSizeType.VisibleRows,AutoResizeColumnWidthOptions.IncludeCells|AutoResizeColumnWidthOptions.IncludeHeader|AutoResizeColumnWidthOptions.IncludeSummaryRows);

				// die alten Maximalbreiten wiederherstellen
				foreach (UltraGridColumn column in oldMaxWidths.Keys)
					column.MaxWidth = oldMaxWidths[column];
			}
		}

		/// <summary>
		/// Führt eine automatische Höhenanpassung der Zeilen aus
		/// </summary>
		public void AutoResizeRows()
		{
			this.DisplayLayout.Override.CellMultiLine = DefaultableBoolean.True;
			foreach (RowScrollRegion rowScrollRegion in this.DisplayLayout.RowScrollRegions)
				foreach (VisibleRow visibleRow in rowScrollRegion.VisibleRows)
					visibleRow.Row.PerformAutoSize();
		}


		/// <summary>
		/// Waitcursor anzeigen, während eine Zeile initialisiert wird
		/// </summary>
		protected override void OnInitializeRow(InitializeRowEventArgs e)
		{
			this.SetWaitCursorWithAutoReset();
			base.OnInitializeRow(e);
		}
		/// <summary>
		/// Waitcursor anzeigen, während eine Zeile ausgeklappt wird
		/// </summary>
		protected override void OnBeforeRowExpanded(CancelableRowEventArgs e)
		{
			this.SetWaitCursorWithAutoReset();
			base.OnBeforeRowExpanded(e);
		}
		/// <summary>
		/// Aktiviert den Waitcursor und setzt diesen zurück, wenn die folgende Benutzeraktion beendet wurde
		/// </summary>
		private void SetWaitCursorWithAutoReset()
		{
			if (!this.timer.Enabled)
			{
				Cursor oldCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;

				EventHandler tickEventHandler = null;
				tickEventHandler = delegate
				{
					this.timer.Stop();
					Cursor.Current = oldCursor;
					this.timer.Tick -= tickEventHandler;
				};
				this.timer.Tick += tickEventHandler;
				this.timer.Interval = 1;
				this.timer.Start();
			}
		}


		/// <summary>
		/// Angepassten Text für Gruppierungszeilen anzeigen
		/// </summary>
		protected override void OnInitializeGroupByRow(InitializeGroupByRowEventArgs e)
		{
			object value = e.Row.Value;
			if (value is bool)
				value = ((bool)value) ? "Ja" : "Nein";

			e.Row.Description = e.Row.Column.Header.Caption+": "+value+" ("+e.Row.Rows.Count.ToString("N0")+" "+((e.Row.Rows.Count==1) ? "Element" : "Elemente")+")";
			base.OnInitializeGroupByRow(e);
		}


		/// <summary>
		/// Eigenes Steuerelement für die Spaltenauswahl anzeigen
		/// </summary>
		protected override void OnBeforeColumnChooserDisplayed(BeforeColumnChooserDisplayedEventArgs e)
		{
			ColumnChooser columnChooser = new ColumnChooser(e.Dialog.ColumnChooserControl.CurrentBand) { BorderStyle = BorderStyle.None };

			// ColumnChooser-Control als DropDown anzeigen
			ToolStripControlHost toolStripControlHost = new ToolStripControlHost(columnChooser) { Margin = new Padding(3) };
			ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
			toolStripDropDown.Items.Add(toolStripControlHost);

			UIElement uiElement = this.DisplayLayout.UIElement.ElementFromPoint(this.DisplayLayout.UIElement.CurrentMousePosition);
			toolStripDropDown.Show(this,uiElement.Rect.X-1,uiElement.Rect.Bottom+1);
			e.Cancel = true;
		}


		/// <summary>
		/// Drei-Zustands-Sortierung (aufsteigend/absteigend/keine)
		/// </summary>
		protected override void OnBeforeSortChange(BeforeSortChangeEventArgs e)
		{
			// wenn nicht Shift gedrückt ist (MultiColumnSort)
			if (!Control.ModifierKeys.HasFlag(Keys.Shift))
				foreach (UltraGridColumn column in e.SortedColumns)
					if (e.Band.SortedColumns.Exists(column.Key))
						// wenn Sortierung von aufsteigend nach absteigend geändert wird ...
						if (column.SortIndicator==SortIndicator.Ascending && e.Band.SortedColumns[column.Key].SortIndicator==SortIndicator.Descending)
						{
							// ...dies abbrechen und die Sortierung entfernen
							e.Cancel = true;
							this.BeginInvoke((MethodInvoker)delegate
							{
								e.Band.SortedColumns[column.Key].SortIndicator = SortIndicator.None;
								column.Band.SortedColumns.RefreshSort(true);
							});
						}

			base.OnBeforeSortChange(e);
		}


		/// <summary>
		/// Workaround für Bug bei der Breite des Zeilenselektors, wenn mehr als 999 Zeilen in einem Unter-Band enthalten sind
		/// </summary>
		protected override void OnAfterRowExpanded(RowEventArgs e)
		{
			foreach (UltraGridChildBand childBands in e.Row.ChildBands)
			{
				string count = childBands.Rows.Count.ToString();
				string text = new string('8',count.Length) + new string('.',count.Length/3);

				Size measuredSize = TextRenderer.MeasureText(text,this.Font);

				int preferredWidth = measuredSize.Width + 15;
				if (childBands.Band.Override.RowSelectorWidth<preferredWidth)
					childBands.Band.Override.RowSelectorWidth = preferredWidth;
			}

			base.OnAfterRowExpanded(e);
		}


		/// <summary>
		/// Anpassungen für das Aussehen beim Drucken
		/// </summary>
		protected override void OnInitializePrint(CancelablePrintEventArgs e)
		{
			e.PrintLayout.BorderStyle = UIElementBorderStyle.None;
			e.DefaultLogicalPageLayoutInfo.ClippingOverride = ClippingOverride.No;
			e.DefaultLogicalPageLayoutInfo.ColumnClipMode = ColumnClipMode.RepeatClippedColumns;
		}


		/// <summary>
		/// Exportiert den Inhalt des UltraGrids als Excel-Datei
		/// </summary>
		/// <param name="path">der Pfad der Datei, die erstellt werden soll</param>
		public void ExportToExcel(string path)
		{
			using (UltraGridExcelExporter excelExporter = new UltraGridExcelExporter())
			{
				excelExporter.BandSpacing = BandSpacing.None;
				excelExporter.InitializeColumn += (sender,initializeColumnEventArgs) => initializeColumnEventArgs.ExcelFormatStr = this.GetExcelFormatStr(initializeColumnEventArgs.FrameworkFormatStr,initializeColumnEventArgs.Column.DataType);

				int lastOutlineLevel = -1;
				bool cancelColumnHeader = false;
				excelExporter.HeaderRowExporting += delegate (object sender,HeaderRowExportingEventArgs e) // Header nur über der ersten Zeile eines ausgeklappten Bereichs exportieren 
				{
					if (lastOutlineLevel>=e.CurrentOutlineLevel)
					{
						if (e.HeaderType==HeaderTypes.BandHeader)
						{
							e.Cancel = true;
							cancelColumnHeader = true;
						}
						if (e.HeaderType==HeaderTypes.ColumnHeader)
						{
							e.Cancel = cancelColumnHeader || e.Band.Index==0;
							cancelColumnHeader = false;
						}
					}

					lastOutlineLevel = e.CurrentOutlineLevel;
				};

				excelExporter.Export(this,path);
			}
		}
		/// <summary>
		/// Wandelt .NET-Framework-Formatierungszeichenfolgen in entsprechende Formatierungszeichenfolgen für Excel um
		/// </summary>
		private string GetExcelFormatStr(string frameworkFormatStr,Type dataType)
		{
			if (frameworkFormatStr==null)
				return null;

			if (frameworkFormatStr=="C")
				return @"#,##0.00 €";

			if (frameworkFormatStr=="P")
				return "0%";

			if (frameworkFormatStr=="#,0.###")
				return null;

			Match numericMatch = Regex.Match(frameworkFormatStr,@"^N(?<count>\d{1,3})$");
			if (numericMatch.Success)
			{
				string result = "#,##0";
				int count = Int32.Parse(numericMatch.Groups["count"].Value);
				if (count>0)
					result += "." + new string('0',count);
				return result;
			}

			return frameworkFormatStr;
		}


		/// <summary>
		/// Exportiert den Inhalt des UltraGrids als PDF-Datei
		/// </summary>
		/// <param name="path">der Pfad der Datei, die erstellt werden soll</param>
		public void ExportAsPdf(string path)
		{
			using (Infragistics.Win.UltraWinGrid.DocumentExport.UltraGridDocumentExporter documentExporter = new Infragistics.Win.UltraWinGrid.DocumentExport.UltraGridDocumentExporter())
				documentExporter.Export(this,path,Infragistics.Win.UltraWinGrid.DocumentExport.GridExportFileFormat.PDF);
		}



		/// <summary>
		/// Bestimmte Bereiche selbst zeichnen, um das Aussehen mit den DevExpress-Steuerelementen zu vereinheitlichen
		/// </summary>
		public bool DrawElement(DrawPhase drawPhase,ref UIElementDrawParams drawParams)
		{
			// Aggregat-Zelle
			if (drawParams.Element is SummaryValueUIElement)
			{
				Rectangle rectangle = new Rectangle(x: drawParams.Element.Rect.X,y: drawParams.Element.Rect.Y-1,width: drawParams.Element.Rect.Width-1,height: drawParams.Element.Rect.Height);
				using (Pen pen = new Pen(Color.FromArgb(202,203,211)))
					drawParams.Graphics.DrawRectangle(pen,rectangle);
			}

			// Spaltenkopf & Zeilenselektor
			if (drawParams.Element is HeaderUIElement || drawParams.Element is RowSelectorUIElement)
			{
				HeaderUIElement headerUIElement = drawParams.Element as HeaderUIElement;
				bool isHighlight = (headerUIElement!=null) && this.RectangleToScreen(drawParams.InvalidRect).Contains(Control.MousePosition);

				Color color1 = (isHighlight) ? Color.FromArgb(254,254,254) : Color.FromArgb(248,248,248);
				Color color2 = (isHighlight) ? Color.FromArgb(248,248,248) : Color.FromArgb(242,242,242);

				Rectangle rectangle = (headerUIElement!=null)
					? new Rectangle(x: drawParams.Element.Rect.X-1,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width,height: drawParams.Element.Rect.Height-((headerUIElement.Header.Column!=null) ? 1 : 0))
					: new Rectangle(x: drawParams.Element.Rect.X,y: drawParams.Element.Rect.Y-1,width: drawParams.Element.Rect.Width-1,height: drawParams.Element.Rect.Height);

				using (LinearGradientBrush brush = new LinearGradientBrush(rectangle,color1,color2,LinearGradientMode.Vertical))
					drawParams.Graphics.FillRectangle(brush,rectangle);
				using (Pen pen = new Pen(Color.FromArgb(202,203,211)))
					drawParams.Graphics.DrawRectangle(pen,rectangle);
			}

			// BandHeader 
			if (drawParams.Element is HeaderUIElement headerElement && headerElement.GetContext(typeof(UltraGridColumn))==null)
			{
				UltraGridBand band = headerElement.Header.Band;
				List<UltraGridBand> equalNamedBands = this.DisplayLayout.Bands.OfType<UltraGridBand>().Where(b => b.Key==band.Key).ToList();
				if (equalNamedBands.Count>1) // falls es mehrere Bänder mit dem gleichen Namen gibt, eine Nummerierung anzeigen
				{
					Rectangle rectangle = new Rectangle(x: drawParams.Element.Rect.X-1,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width,height: drawParams.Element.Rect.Height-((headerElement.Header.Column!=null) ? 1 : 0));
					using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Far,LineAlignment = StringAlignment.Center })
						drawParams.Graphics.DrawString("("+(equalNamedBands.IndexOf(band)+1)+")",this.Font,Brushes.Gray,rectangle,stringFormat);
				}
			}

			// Markierung für die Sortierung
			if (drawParams.Element is SortIndicatorUIElement)
				if (drawParams.Element.GetContext(typeof(UltraGridColumn)) is UltraGridColumn column)
				{
					Rectangle rectangle = new Rectangle(x: drawParams.Element.Rect.X,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width-1,height: drawParams.Element.Rect.Height-1);
					Image image = (column.SortIndicator==SortIndicator.Ascending) ? this.SortIndicatorImageAscending : this.SortIndicatorImageDescending;

					Point point = new Point((rectangle.Left+rectangle.Right-image.Width)/2,(rectangle.Top+rectangle.Bottom)/2);
					if (column.Band.SortedColumns.Count>1)
						point.Offset(0,-image.Height);

					// eigenes Bild für aufsteigend/absteigend
					drawParams.Graphics.DrawImage(image,point.X,point.Y,image.Width,image.Height);

					// bei Sortierung mit mehreren Spalten deren Reihenfolge mit anzeigen
					List<UltraGridColumn> sortedColumns = column.Band.SortedColumns.Cast<UltraGridColumn>().Where(col => !col.IsGroupByColumn).ToList();
					if (sortedColumns.Count>1)
					{
						int index = sortedColumns.IndexOf(column)+1;

						using (Font font = new Font("Verdana",6.25f,FontStyle.Regular))
						using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Far })
							drawParams.Graphics.DrawString(index.ToString(),font,Brushes.Gray,rectangle,stringFormat);
					}
				}

			return true;
		}
		/// <summary>
		/// In den regulären Grid-Zeichenablauf eingreifen
		/// </summary>
		public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
		{
			if (drawParams.Element is HeaderUIElement)
				return DrawPhase.AfterDrawTheme;
			if (drawParams.Element is SortIndicatorUIElement)
				return DrawPhase.BeforeDrawElement;
			if (drawParams.Element is RowSelectorUIElement)
				return DrawPhase.BeforeDrawBorders;
			if (drawParams.Element is SummaryValueUIElement)
				return DrawPhase.BeforeDrawBorders;

			return DrawPhase.None;
		}



		/// <summary>
		/// Speichert alle ausgeklappten Gruppierungszeilen
		/// </summary>
		public void SaveExpandedState()
		{
			this.expandedRowsState = this.GetExpandedRows(this.Rows).ToList();
		}
		/// <summary>
		/// Ermittelt eine alle Gruppierungszeilen, die ausgeklappt sind
		/// </summary>
		private IEnumerable<ExpandedGroupByRow> GetExpandedRows(RowsCollection rowsCollection)
		{
			foreach (UltraGridGroupByRow groupByRow in rowsCollection.OfType<UltraGridGroupByRow>())
				if (groupByRow.IsExpanded)
					yield return new ExpandedGroupByRow
					{
						ColumnKey = groupByRow.Column.Key,
						Value = groupByRow.ValueAsDisplayText,
						ChildRows = this.GetExpandedRows(groupByRow.Rows).ToList()
					};
		}

		/// <summary>
		/// Stellt die zuvor gespeicherten ausgeklappten Gruppierungszeilen wieder her
		/// </summary>
		public void RestoreExpandedState()
		{
			if (this.expandedRowsState!=null)
				this.SetExpandedRows(this.expandedRowsState,this.Rows);
		}
		private void SetExpandedRows(IEnumerable<ExpandedGroupByRow> expandedRows,RowsCollection rowsCollection)
		{
			foreach (UltraGridGroupByRow groupByRow in rowsCollection.OfType<UltraGridGroupByRow>())
			{
				ExpandedGroupByRow expandedRow = expandedRows.SingleOrDefault(er => er.ColumnKey==groupByRow.Column.Key && er.Value==groupByRow.ValueAsDisplayText);
				if (expandedRow!=null)
				{
					groupByRow.Expanded = true;
					this.SetExpandedRows(expandedRow.ChildRows,groupByRow.Rows);
				}
			}
		}

		/// <summary>
		/// Stellt Informationen über eine ausgeklappte Gruppierungszeile bereit
		/// </summary>
		[DebuggerDisplay("Name")]
		private class ExpandedGroupByRow
		{
			/// <summary> Gibt den Schlüssel der Spalte zurück, die gruppiert wurde, oder legt diesen fest </summary>
			public string ColumnKey { get; set; }
			/// <summary> Gibt den Wert der Gruppierung zurück oder legt diesen fest </summary>
			public string Value { get; set; }
			/// <summary> Gibt alle ausgeklappten Unter-Gruppierungszeilen zurück oder legt diese fest </summary>
			public List<ExpandedGroupByRow> ChildRows { get; set; }
		}
	}
}
