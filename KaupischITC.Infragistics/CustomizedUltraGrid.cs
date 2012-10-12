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
using KaupischITC.Charting;
using KaupischITC.Extensions;
using KaupischITC.InfragisticsControls.ValueAppearances;
using KaupischITC.Shared;

namespace KaupischITC.InfragisticsControls
{
	public partial class CustomizedUltraGrid : UltraGrid,IUIElementCreationFilter,IUIElementDrawFilter
	{
		private static ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
		private Timer timer = new Timer();
		private List<ExpandedRow> expandedRowsState;

		private readonly ToolStripMenuItem summaryToolStripMenuItem;
		private readonly ToolStripMenuItem formatToolStripMenuItem;
		private readonly ToolStripMenuItem fontToolStripMenuItem;
		private readonly ToolStripMenuItem visualizationToolStripMenuItem;
		private readonly UrlProtocolHandler urlProtocolHandler = new UrlProtocolHandler();

		public ContextMenuStrip ColumnContextMenuStrip { get; private set; }
		public ContextMenuStrip RowContextMenuStrip { get; private set; }
		public UltraGridRow ContextUltraGridRow { get; private set; }
		public UltraGridCell ContextUltraGridCell { get; private set; }
		public HeaderUIElement ContextHeaderUIElement { get; private set; }
		public UltraGridColumn ContextUltraGridColumn { get; private set; }
		public SummaryFooterUIElement ContextSummaryFooterUIElement { get; private set; }
		public Image SortIndicatorImageAscending { get; set; }
		public Image SortIndicatorImageDescending { get; set; }

		public class UltraGridColumnEventArgs : EventArgs { public UltraGridColumn Column { get; set; } }
		public event EventHandler<UltraGridColumnEventArgs> ColumnFormatChanged;
		public event EventHandler<UltraGridColumnEventArgs> ColumnCaptionChanged;

		private Dictionary<SummaryType,string[]> availableSummaries = new Dictionary<SummaryType,string[]>()
		{
			{ SummaryType.Sum, new [] { "Summe","Σ" }},
			{ SummaryType.Average, new [] {"Durchschnitt","Ø" }},
			{ SummaryType.Count,new [] { "Anzahl","#" }},
			{ SummaryType.Minimum, new [] {"Minimum","Min" }},
			{ SummaryType.Maximum, new [] {"Maximum","Max" }}
		};


		private Dictionary<string,string> availableFormats = new Dictionary<string,string>()
		{
			{ "N0","Zahl (mit Tausendertrennzeichen)" },
			{ "C", "Währung" },
			{ "P", "Prozent" },
			{ "dd.MM.yyyy", "Datum" },
			{ "HH:mm", "Uhrzeit" },
			{ "dd.MM.yyyy HH:mm", "Datum und Uhrzeit" },
			{ "0.## Std", "Stunden" }
		};


		public bool AllowRowFiltering
		{
			get { return (this.DisplayLayout.Override.AllowRowFiltering==DefaultableBoolean.True); }
			set
			{
				this.DisplayLayout.Override.AllowRowFiltering = (value) ? DefaultableBoolean.True : DefaultableBoolean.False;
				if (!value)
					foreach (UltraGridBand band in this.DisplayLayout.Bands)
						band.ColumnFilters.ClearAllFilters();
			}
		}


		public CustomizedUltraGrid()
		{
			this.CreationFilter = this;
			this.DrawFilter = this;

			this.Font = SystemFonts.MessageBoxFont;
			this.InitializeComponent();

			this.RowContextMenuStrip = new ContextMenuStrip();
			this.ColumnContextMenuStrip = new ContextMenuStrip();

			ToolStripTextBox toolStripTextBoxCaption = new ToolStripTextBox("HeaderCaption");
			toolStripTextBoxCaption.TextChanged += delegate
			{
				if (this.ContextUltraGridColumn!=null)
					this.ContextUltraGridColumn.Header.Caption = toolStripTextBoxCaption.Text;

				if (this.ColumnCaptionChanged!=null && this.ContextUltraGridColumn!=null)
					this.ColumnCaptionChanged(this,new UltraGridColumnEventArgs { Column = this.ContextUltraGridColumn });
			};
			this.ColumnContextMenuStrip.Items.Add(toolStripTextBoxCaption);

			this.formatToolStripMenuItem = (ToolStripMenuItem)this.ColumnContextMenuStrip.Items.Add("Werte formatieren als");
			foreach (string formatString in availableFormats.Keys)
				this.formatToolStripMenuItem.DropDownItems.Add(availableFormats[formatString],null,FormatMenuItem_Click).Tag = formatString;

			ToolStripTextBox toolStripTextBox = new ToolStripTextBox("Custom");
			toolStripTextBox.TextChanged += delegate
			{
				toolStripTextBox.Tag = toolStripTextBox.Text;
				this.FormatMenuItem_Click(toolStripTextBox,EventArgs.Empty);
			};
			toolStripTextBox.Click += delegate { this.FormatMenuItem_Click(toolStripTextBox,EventArgs.Empty); };
			this.formatToolStripMenuItem.DropDownItems.Add(toolStripTextBox);

			this.summaryToolStripMenuItem = (ToolStripMenuItem)this.ColumnContextMenuStrip.Items.Add("Zusammenfassung hinzufügen");
			foreach (SummaryType summaryType in availableSummaries.Keys)
				this.summaryToolStripMenuItem.DropDownItems.Add(availableSummaries[summaryType][0],null,SummaryMenuItem_Click).Tag = summaryType;

			this.fontToolStripMenuItem = (ToolStripMenuItem)this.ColumnContextMenuStrip.Items.Add("Text formatieren");
			this.fontToolStripMenuItem.DropDownItems.Add("Fett",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Bold = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Kursiv",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Italic =((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Unterstrichen",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Underline = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });

			this.fontToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
			this.fontToolStripMenuItem.DropDownItems.Add("negative Werte rot",null,delegate(object sender,EventArgs e) { ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).HighlightNegativeValues = !((ToolStripMenuItem)sender).Checked; });
			this.fontToolStripMenuItem.DropDownItems.Add("Tendenzpfeile",null,delegate(object sender,EventArgs e) { ((ValueAppearance)this.ContextUltraGridColumn.ValueBasedAppearance).ShowTrendIndicators = !((ToolStripMenuItem)sender).Checked; });

			this.visualizationToolStripMenuItem = (ToolStripMenuItem)this.ColumnContextMenuStrip.Items.Add("Visualisierung");
			this.visualizationToolStripMenuItem.DropDownItems.Add("Kreisdiagramm anzeigen",null,delegate { this.ShowChartForm(new PieChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Balkendiagramm anzeigen",null,delegate { this.ShowChartForm(new BarChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Flächendiagramm anzeigen",null,delegate { this.ShowChartForm(new TreeMapForm()); });

			this.RowContextMenuStrip.Items.Add("Erweitern",null,delegate { this.ContextUltraGridRow.Expanded = true; });
			this.RowContextMenuStrip.Items.Add("Reduzieren",null,delegate { this.ContextUltraGridRow.Expanded = false; });
			this.RowContextMenuStrip.Items.Add("-");
			this.RowContextMenuStrip.Items.Add("Alles erweitern",null,delegate { this.ContextUltraGridRow.ParentCollection.ExpandAll(false); });
			this.RowContextMenuStrip.Items.Add("Alles reduzieren",null,delegate { this.ContextUltraGridRow.ParentCollection.CollapseAll(false); });

			this.ColumnContextMenuStrip.Opening += delegate
			{
				foreach (ToolStripItem item in this.ColumnContextMenuStrip.Items.Cast<ToolStripItem>().Where(tsi => tsi.Tag==this.urlProtocolHandler).ToList())
					this.ColumnContextMenuStrip.Items.Remove(item);

				List<UrlProtocolHandler.ConcreteRoute> validRoutes = this.urlProtocolHandler.GetValidRoutes(this.ContextUltraGridCell).ToList();
				if (validRoutes.Any())
				{
					this.ColumnContextMenuStrip.Items.Add("-").Tag = this.urlProtocolHandler;
					foreach (UrlProtocolHandler.ConcreteRoute route in validRoutes)
						this.ColumnContextMenuStrip.Items.Add(route.Name,null,delegate { route.Invoke(); }).Tag = this.urlProtocolHandler;
				}
			};


			ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
			this.SortIndicatorImageAscending = (Image)resources.GetObject("Up");
			this.SortIndicatorImageDescending = (Image)resources.GetObject("Down");

			this.OnInitializeLayout(new InitializeLayoutEventArgs(this.DisplayLayout));

			Infragistics.Win.UltraWinGrid.Resources.Customizer.SetCustomizedString("ColumnChooserButtonToolTip","Spalten auswählen");
		}


		private void SummaryMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			SummaryType summaryType = (SummaryType)menuItem.Tag;

			if (!menuItem.Checked)
				this.AddColumnSummary(this.ContextUltraGridColumn,summaryType);
			else
				this.RemoveColumnSummary(this.ContextUltraGridColumn,summaryType);
		}
		public void AddColumnSummary(UltraGridColumn column,SummaryType summaryType)
		{
			SummarySettings summarySettings =column.Band.Summaries.Add(summaryType,column);
			summarySettings.DisplayFormat = this.GetColumnSummaryFormat(column.Format,summaryType);
			summarySettings.Appearance.TextHAlign = HAlign.Right;
		}
		public void RemoveColumnSummary(UltraGridColumn column,SummaryType summaryType)
		{
			foreach (SummarySettings summarySettings in column.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SummaryType==summaryType && ss.SourceColumn==column))
				this.ContextUltraGridColumn.Band.Summaries.Remove(summarySettings);
		}



		private void FormatMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripItem menuItem = (ToolStripItem)sender;
			string format = (string)menuItem.Tag;

			this.SetColumnFormat(this.ContextUltraGridColumn,format);
		}
		public void SetColumnFormat(UltraGridColumn column,string format)
		{
			column.Format = format;

			foreach (SummarySettings summarySettings in column.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SourceColumn==column))
				summarySettings.DisplayFormat = this.GetColumnSummaryFormat(format,summarySettings.SummaryType);

			if (this.ColumnFormatChanged!=null)
				this.ColumnFormatChanged(this,new UltraGridColumnEventArgs { Column = column });
		}


		private string GetColumnSummaryFormat(string cellFormat,SummaryType summaryType)
		{
			return this.availableSummaries[summaryType][1]+": " + ((summaryType!=SummaryType.Count) ? "{0:"+cellFormat+"}" : "{0}");
		}


		private void ShowChartForm(ChartForm chartForm)
		{
			var elements = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Where(ugr => !ugr.Hidden && !ugr.IsFilteredOut).Select(ugr => ugr.ListObject).ToList();
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


		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button==MouseButtons.Right)
			{
				Point mousePoint = new Point(e.X,e.Y);
				UIElement element = this.DisplayLayout.UIElement.ElementFromPoint(mousePoint);
				if (element!=null)
				{
					this.ContextUltraGridRow = (UltraGridRow)element.GetContext(typeof(UltraGridRow));				// geklickte Zeile
					this.ContextUltraGridCell = (UltraGridCell)element.GetContext(typeof(UltraGridCell));			// geklickte Zelle
					this.ContextHeaderUIElement = (HeaderUIElement)element.GetAncestor(typeof(HeaderUIElement));	// geklickter Spaltenkopf
					this.ContextUltraGridColumn = (UltraGridColumn)element.GetContext(typeof(UltraGridColumn));		// geklickte Spalte
					this.ContextSummaryFooterUIElement = (SummaryFooterUIElement)element.GetAncestor(typeof(SummaryFooterUIElement)); // geklickter Spaltenfuß

					// Kontextmenü für Spalten
					if (this.ContextUltraGridColumn!=null)
					{
						this.ColumnContextMenuStrip.Items["HeaderCaption"].Text = (this.ContextUltraGridColumn!=null) ? this.ContextUltraGridColumn.Header.Caption : null;

						// "Zusammenfassungen"
						if (this.ContextUltraGridColumn!=null)
							foreach (ToolStripMenuItem menuItem in this.summaryToolStripMenuItem.DropDownItems)
								menuItem.Checked = this.ContextUltraGridColumn.Band.Summaries.Cast<SummarySettings>().Any(ss => ss.SourceColumn==this.ContextUltraGridColumn && ss.SummaryType==(SummaryType)menuItem.Tag);

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

						this.ColumnContextMenuStrip.Show(this,mousePoint);
					}
					// Kontextmenü für Zeilen
					else if (this.ContextUltraGridRow!=null)
					{
						if (this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Any(row => row.IsExpandable))
						{
							this.RowContextMenuStrip.Items[0].Visible = this.ContextUltraGridRow.IsExpandable && !this.ContextUltraGridRow.IsExpanded;
							this.RowContextMenuStrip.Items[1].Visible = this.ContextUltraGridRow.IsExpandable && this.ContextUltraGridRow.IsExpanded;
							this.RowContextMenuStrip.Items[2].Visible = this.ContextUltraGridRow.IsExpandable;
							this.RowContextMenuStrip.Show(this,mousePoint);
						}
					}
				}
			}
			base.OnMouseUp(e);
		}



		public void AutoResizeColumns()
		{
			foreach (UltraGridBand band in this.DisplayLayout.Bands)
			{
				Dictionary<UltraGridColumn,int> oldMaxWidths = new Dictionary<UltraGridColumn,int>(band.Columns.Count);
				foreach (UltraGridColumn column in band.Columns)
				{
					oldMaxWidths[column] = column.MaxWidth;
					column.MaxWidth = 300;
				}

				band.PerformAutoResizeColumns(false,PerformAutoSizeType.VisibleRows,AutoResizeColumnWidthOptions.IncludeCells|AutoResizeColumnWidthOptions.IncludeHeader|AutoResizeColumnWidthOptions.IncludeSummaryRows);

				foreach (UltraGridColumn column in oldMaxWidths.Keys)
					column.MaxWidth = oldMaxWidths[column];
			}
		}


		public void AutoResizeRows()
		{
			this.DisplayLayout.Override.CellMultiLine = DefaultableBoolean.True;
			foreach (RowScrollRegion rowScrollRegion in this.DisplayLayout.RowScrollRegions)
				foreach (VisibleRow visibleRow in rowScrollRegion.VisibleRows)
					visibleRow.Row.PerformAutoSize();
		}


		protected override void OnInitializeLayout(InitializeLayoutEventArgs e)
		{
			using (new WaitCursorChanger(this))
			{
				this.DisplayLayout.MaxBandDepth = 5; // TODO
				this.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

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

				this.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.False;
				this.DisplayLayout.Override.FilterOperatorDefaultValue = FilterOperatorDefaultValue.Contains;
				this.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
				this.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand;

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
				this.DisplayLayout.Override.SummaryValueAppearance.BackColor = summaryBackColor;
				this.DisplayLayout.Override.SummaryValueAppearance.BorderColor = borderColor;
				this.DisplayLayout.Override.RowAppearance.BorderColor = borderColor;

				this.SuspendRowSynchronization();

				foreach (UltraGridBand ultraGridBand in e.Layout.Bands)
				{
					ultraGridBand.HeaderVisible = (ultraGridBand.Index!=0 && ultraGridBand.Key!="Elements");
					ultraGridBand.Header.Appearance.TextHAlign = HAlign.Left;
					ultraGridBand.RowLayoutStyle = RowLayoutStyle.None;

					foreach (UltraGridColumn ultraGridColumn in ultraGridBand.Columns)
					{
						ultraGridColumn.CellActivation = Activation.ActivateOnly;
						ultraGridColumn.HiddenWhenGroupBy = DefaultableBoolean.True;
						if (ultraGridColumn.ValueBasedAppearance==null)
							ultraGridColumn.ValueBasedAppearance = new ValueAppearance { HighlightNegativeValues = ultraGridColumn.DataType.IsNumeric() };

						if (ultraGridColumn.DataType.IsNumeric())
							ultraGridColumn.CellAppearance.TextHAlign = HAlign.Right;

						if (ultraGridColumn.Format==null)
						{
							if (ultraGridColumn.Key.EndsWith("Preis",StringComparison.InvariantCultureIgnoreCase))
								this.SetColumnFormat(ultraGridColumn,"C");
							else if (Regex.IsMatch(ultraGridColumn.Key,"[^A-Z]P$"))
								this.SetColumnFormat(ultraGridColumn,"P");
						}

						string[] hiddenPostfixes = { "ID","Id","Key" };
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


		protected override void OnInitializeRow(InitializeRowEventArgs e)
		{
			this.SetWaitCursorWithAutoReset();
			base.OnInitializeRow(e);
		}


		protected override void OnInitializeGroupByRow(InitializeGroupByRowEventArgs e)
		{
			object value = e.Row.Value;
			if (value is bool)
				value = ((bool)value) ? "Ja" : "Nein";

			e.Row.Description = e.Row.Column.Header.Caption+": "+value+" ("+e.Row.Rows.Count+" "+((e.Row.Rows.Count==1)?"Element":"Elemente")+")";
			base.OnInitializeGroupByRow(e);
		}


		protected override void OnBeforeColumnChooserDisplayed(BeforeColumnChooserDisplayedEventArgs e)
		{
			e.Cancel = true;

			ColumnChooser columnChooser = new ColumnChooser(e.Dialog.ColumnChooserControl.CurrentBand) { BorderStyle = BorderStyle.None };
			ToolStripControlHost toolStripControlHost = new ToolStripControlHost(columnChooser) { Margin = new Padding(3) };
			ToolStripDropDown toolStripDropDown = new ToolStripDropDown();
			toolStripDropDown.Items.Add(toolStripControlHost);

			UIElement uiElement = this.DisplayLayout.UIElement.ElementFromPoint(this.DisplayLayout.UIElement.CurrentMousePosition);
			toolStripDropDown.Show(this,uiElement.Rect.X-1,uiElement.Rect.Bottom+1);
		}


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

		protected override void OnAfterRowExpanded(RowEventArgs e)
		{
			foreach (UltraGridChildBand childBands in e.Row.ChildBands)
			{
				string count = childBands.Rows.Count.ToString();
				string text = new string('8',count.Length) + new string('.',count.Length/3);

				var la = TextRenderer.MeasureText(text,this.Font);

				int preferredWidth = la.Width + 15;
				if (childBands.Band.Override.RowSelectorWidth<preferredWidth)
					childBands.Band.Override.RowSelectorWidth = preferredWidth;
			}

			base.OnAfterRowExpanded(e);
		}


		protected override void OnInitializePrint(CancelablePrintEventArgs e)
		{
			e.PrintLayout.BorderStyle = UIElementBorderStyle.None;
			e.DefaultLogicalPageLayoutInfo.ClippingOverride = ClippingOverride.No;
			e.DefaultLogicalPageLayoutInfo.ColumnClipMode = ColumnClipMode.RepeatClippedColumns;
		}


		protected override void OnClickCellButton(CellEventArgs e)
		{
			// TODO
			base.OnClickCellButton(e);
		}


		protected override void OnBeforeRowExpanded(CancelableRowEventArgs e)
		{
			this.SetWaitCursorWithAutoReset();
			base.OnBeforeRowExpanded(e);
		}


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


		public void AfterCreateChildElements(UIElement parent)
		{
			if (parent is CellUIElement)
			{
				UltraGridColumn ultraGridColumn = parent.GetContext(typeof(UltraGridColumn)) as UltraGridColumn;
				if (ultraGridColumn!=null && ultraGridColumn.Style==Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton)
				{
					UltraGridCell ultraGridCell = parent.GetContext(typeof(UltraGridCell)) as UltraGridCell;
					if ((ultraGridCell.Style==Infragistics.Win.UltraWinGrid.ColumnStyle.Default || ultraGridCell.Style==ultraGridColumn.Style) && ultraGridCell.Value==null)
						ultraGridCell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Edit;
				}
			}
		}

		public bool BeforeCreateChildElements(UIElement parent)
		{
			return false;
		}




		public bool DrawElement(DrawPhase drawPhase,ref UIElementDrawParams drawParams)
		{
			if (drawParams.Element is HeaderUIElement || drawParams.Element is RowSelectorUIElement)
			{
				HeaderUIElement headerUIElement = drawParams.Element as HeaderUIElement;
				bool isHighlight = (headerUIElement!=null) && this.RectangleToScreen(drawParams.InvalidRect).Contains(Control.MousePosition);

				Color color1 = (isHighlight) ? Color.FromArgb(254,254,254) : Color.FromArgb(248,248,248);
				Color color2 = (isHighlight) ? Color.FromArgb(248,248,248) : Color.FromArgb(242,242,242);

				Rectangle rectangle = (headerUIElement!=null)
					? new Rectangle(x: drawParams.Element.Rect.X-1,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width,height: drawParams.Element.Rect.Height-((headerUIElement.Header.Column!=null)?1:0))
					: new Rectangle(x: drawParams.Element.Rect.X,y: drawParams.Element.Rect.Y-1,width: drawParams.Element.Rect.Width-1,height: drawParams.Element.Rect.Height);

				using (LinearGradientBrush brush = new LinearGradientBrush(rectangle,color1,color2,LinearGradientMode.Vertical))
					drawParams.Graphics.FillRectangle(brush,rectangle);
				using (Pen pen = new Pen(Color.FromArgb(202,203,211)))
					drawParams.Graphics.DrawRectangle(pen,rectangle);
			}

			if (drawParams.Element is SortIndicatorUIElement)
			{
				UltraGridColumn column = drawParams.Element.GetContext(typeof(UltraGridColumn)) as UltraGridColumn;
				if (column!=null)
				{
					Rectangle rectangle = new Rectangle(x: drawParams.Element.Rect.X,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width-1,height: drawParams.Element.Rect.Height-1);
					Image image = (column.SortIndicator==SortIndicator.Ascending) ? this.SortIndicatorImageAscending : this.SortIndicatorImageDescending;

					Point point = new Point((rectangle.Left+rectangle.Right-image.Width)/2,(rectangle.Top+rectangle.Bottom)/2);
					if (column.Band.SortedColumns.Count>1)
						point.Offset(0,-image.Height);

					drawParams.Graphics.DrawImageUnscaled(image,point);

					List<UltraGridColumn> sortedColumns = column.Band.SortedColumns.Cast<UltraGridColumn>().Where(col => !col.IsGroupByColumn).ToList();
					if (sortedColumns.Count>1)
					{
						int index = sortedColumns.IndexOf(column)+1;

						using (Font font = new Font("Verdana",6.25f,FontStyle.Regular))
						using (StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Far })
							drawParams.Graphics.DrawString(index.ToString(),font,Brushes.Gray,rectangle,stringFormat);
					}
				}
			}

			return true;
		}

		public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
		{
			if (drawParams.Element is HeaderUIElement)
				return DrawPhase.AfterDrawTheme;
			if (drawParams.Element is SortIndicatorUIElement)
				return DrawPhase.BeforeDrawElement;
			if (drawParams.Element is RowSelectorUIElement)
				return DrawPhase.BeforeDrawBorders;

			return DrawPhase.None;
		}


		public void SaveExpandedState()
		{
			this.expandedRowsState = this.GetExpandedRows(this.Rows).ToList();
		}

		private IEnumerable<ExpandedRow> GetExpandedRows(RowsCollection rowsCollection)
		{
			foreach (UltraGridGroupByRow groupByRow in rowsCollection.OfType<UltraGridGroupByRow>())
				if (groupByRow.IsExpanded)
					yield return new ExpandedRow
					{
						ColumnKey = groupByRow.Column.Key,
						Value = groupByRow.ValueAsDisplayText,
						ChildRows = this.GetExpandedRows(groupByRow.Rows).ToList()
					};
		}

		public void RestoreExpandedState()
		{
			if (this.expandedRowsState!=null)
				this.SetExpandedRows(this.expandedRowsState,this.Rows);
		}

		private void SetExpandedRows(IEnumerable<ExpandedRow> expandedRows,RowsCollection rowsCollection)
		{
			foreach (UltraGridGroupByRow groupByRow in rowsCollection.OfType<UltraGridGroupByRow>())
			{
				ExpandedRow expandedRow = expandedRows.SingleOrDefault(er => er.ColumnKey==groupByRow.Column.Key && er.Value==groupByRow.ValueAsDisplayText);
				if (expandedRow!=null)
				{
					groupByRow.Expanded = true;
					this.SetExpandedRows(expandedRow.ChildRows,groupByRow.Rows);
				}
			}
		}

		[DebuggerDisplay("Name")]
		private class ExpandedRow
		{
			public string ColumnKey { get; set; }
			public string Value { get; set; }
			public List<ExpandedRow> ChildRows { get; set; }
		}
	}
}
