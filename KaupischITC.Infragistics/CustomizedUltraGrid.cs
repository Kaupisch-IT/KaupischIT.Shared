using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischITC.Charting;
using KaupischITC.Extensions;
using KaupischITC.Shared;

namespace KaupischITC.InfragisticsControls
{
	public partial class CustomizedUltraGrid : UltraGrid,IUIElementCreationFilter,IUIElementDrawFilter
	{
		private static ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
		private Timer timer = new Timer();

		private readonly ToolStripMenuItem summaryToolStripMenuItem;
		private readonly ToolStripMenuItem formatToolStripMenuItem;
		private readonly ToolStripMenuItem fontToolStripMenuItem;
		private readonly ToolStripMenuItem visualizationToolStripMenuItem;

		public UltraGridRow ContextUltraGridRow { get; private set; }
		public UltraGridCell ContextUltraGridCell { get; private set; }
		public HeaderUIElement ContextHeaderUIElement { get; private set; }
		public UltraGridColumn ContextUltraGridColumn { get; private set; }
		public SummaryFooterUIElement ContextSummaryFooterUIElement { get; private set; }
		public Image SortIndicatorImageAscending { get; set; }
		public Image SortIndicatorImageDescending { get; set; }


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

			this.InitializeComponent();

			this.ContextMenuStrip = new ContextMenuStrip();

			ToolStripTextBox toolStripTextBoxCaption = new ToolStripTextBox("HeaderCaption");
			toolStripTextBoxCaption.TextChanged += delegate
			{
				if (this.ContextUltraGridColumn!=null)
					this.ContextUltraGridColumn.Header.Caption = toolStripTextBoxCaption.Text;
			};
			this.ContextMenuStrip.Items.Add(toolStripTextBoxCaption);

			this.formatToolStripMenuItem = (ToolStripMenuItem)this.ContextMenuStrip.Items.Add("Werte formatieren als");
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

			this.summaryToolStripMenuItem = (ToolStripMenuItem)this.ContextMenuStrip.Items.Add("Zusammenfassung hinzufügen");
			foreach (SummaryType summaryType in availableSummaries.Keys)
				this.summaryToolStripMenuItem.DropDownItems.Add(availableSummaries[summaryType][0],null,SummaryMenuItem_Click).Tag = summaryType;

			this.fontToolStripMenuItem = (ToolStripMenuItem)this.ContextMenuStrip.Items.Add("Text formatieren");
			this.fontToolStripMenuItem.DropDownItems.Add("Fett",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Bold = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Kursiv",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Italic =((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add("Unterstrichen",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.CellAppearance.FontData.Underline = ((ToolStripMenuItem)sender).Checked ? DefaultableBoolean.False : DefaultableBoolean.True; });
			this.fontToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
			this.fontToolStripMenuItem.DropDownItems.Add("negative Werte rot",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.ValueBasedAppearance = ((ToolStripMenuItem)sender).Checked ? null : new NegativeValueAppearance(); });
			this.fontToolStripMenuItem.DropDownItems.Add("Tendenzpfeile",null,delegate(object sender,EventArgs e) { this.ContextUltraGridColumn.ValueBasedAppearance = ((ToolStripMenuItem)sender).Checked ? null : new IconValueAppearance(); });

			this.visualizationToolStripMenuItem = (ToolStripMenuItem)this.ContextMenuStrip.Items.Add("Visualisierung");
			this.visualizationToolStripMenuItem.DropDownItems.Add("Kreisdiagramm anzeigen",null,delegate { this.ShowChartForm(new PieChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Balkendiagramm anzeigen",null,delegate { this.ShowChartForm(new BarChartForm()); });
			this.visualizationToolStripMenuItem.DropDownItems.Add("Flächendiagramm anzeigen",null,delegate { this.ShowChartForm(new TreeMapForm()); });

			ComponentResourceManager resources = new ComponentResourceManager(typeof(CustomizedUltraGrid));
			this.SortIndicatorImageAscending = (Image)resources.GetObject("Up");
			this.SortIndicatorImageDescending = (Image)resources.GetObject("Down");
		}

		private void SummaryMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			SummaryType summaryType = (SummaryType)menuItem.Tag;

			if (!menuItem.Checked)
			{
				SummarySettings summarySettings = this.ContextUltraGridColumn.Band.Summaries.Add(summaryType,this.ContextUltraGridColumn);
				summarySettings.DisplayFormat = this.GetSummaryFormat(summaryType);
				summarySettings.Appearance.TextHAlign = HAlign.Right;
			}
			else
				foreach (SummarySettings summarySettings in this.ContextUltraGridColumn.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SummaryType==summaryType && ss.SourceColumn==this.ContextUltraGridColumn))
					this.ContextUltraGridColumn.Band.Summaries.Remove(summarySettings);
		}


		private void FormatMenuItem_Click(object sender,EventArgs e)
		{
			ToolStripItem menuItem = (ToolStripItem)sender;
			string format = (string)menuItem.Tag;

			this.ContextUltraGridColumn.Format = format;

			foreach (SummarySettings summarySettings in this.ContextUltraGridColumn.Band.Summaries.Cast<SummarySettings>().Where(ss => ss.SourceColumn==this.ContextUltraGridColumn))
				summarySettings.DisplayFormat = this.GetSummaryFormat(summarySettings.SummaryType);
		}


		private string GetSummaryFormat(SummaryType summaryType)
		{
			return this.availableSummaries[summaryType][1]+": " + ((summaryType!=SummaryType.Count) ? "{0:"+this.ContextUltraGridColumn.Format+"}" : "{0}");
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

					this.ContextMenuStrip.Items["HeaderCaption"].Visible = this.ContextUltraGridColumn!=null;
					this.ContextMenuStrip.Items["HeaderCaption"].Text = (this.ContextUltraGridColumn!=null) ? this.ContextUltraGridColumn.Header.Caption : null;

					// "Zusammenfassungen"
					this.summaryToolStripMenuItem.Enabled = this.ContextUltraGridColumn!=null;
					if (this.ContextUltraGridColumn!=null)
						foreach (ToolStripMenuItem menuItem in this.summaryToolStripMenuItem.DropDownItems)
							menuItem.Checked = this.ContextUltraGridColumn.Band.Summaries.Cast<SummarySettings>().Any(ss => ss.SourceColumn==this.ContextUltraGridColumn && ss.SummaryType==(SummaryType)menuItem.Tag);

					// "Formatieren als"
					this.formatToolStripMenuItem.Enabled = this.ContextUltraGridColumn!=null;
					if (this.ContextUltraGridColumn!=null)
					{
						foreach (ToolStripMenuItem menuItem in this.formatToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
							menuItem.Checked = (this.ContextUltraGridColumn.Format==(string)menuItem.Tag);
						this.formatToolStripMenuItem.DropDownItems["Custom"].Text = this.ContextUltraGridColumn.Format;
					}

					// "Text formatieren"
					this.fontToolStripMenuItem.Enabled = this.ContextUltraGridColumn!=null;
					if (this.ContextUltraGridColumn!=null)
					{
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[0]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Bold==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[1]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Italic==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[2]).Checked = (this.ContextUltraGridColumn.CellAppearance.FontData.Underline==DefaultableBoolean.True);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[4]).Checked = (this.ContextUltraGridColumn.ValueBasedAppearance is NegativeValueAppearance);
						((ToolStripMenuItem)this.fontToolStripMenuItem.DropDownItems[5]).Checked = (this.ContextUltraGridColumn.ValueBasedAppearance is IconValueAppearance);
					}

					// "Kreisdiagramm anzeigen"
					this.visualizationToolStripMenuItem.Enabled = (this.ContextUltraGridRow!=null && this.ContextUltraGridColumn!=null);

					this.ContextMenuStrip.Show(this,mousePoint);
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
				this.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
				this.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
				this.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.RowIndex;
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

						if (ultraGridColumn.DataType.IsNumeric())
							ultraGridColumn.CellAppearance.TextHAlign = HAlign.Right;

						if (ultraGridColumn.Format==null)
						{
							if (ultraGridColumn.Key.EndsWith("Preis",StringComparison.InvariantCultureIgnoreCase))
								ultraGridColumn.Format = "C";
							else if (Regex.IsMatch(ultraGridColumn.Key,"[^A-Z]P$"))
								ultraGridColumn.Format = "P";
						}

						string[] hiddenPostfixes = { "ID","Id","Key" };
						if (hiddenPostfixes.Any(pf => ultraGridColumn.Key.EndsWith(pf)) || ultraGridColumn.PropertyDescriptor.Attributes.OfType<BrowsableAttribute>().Any(ba => !ba.Browsable))
							ultraGridColumn.Hidden = true;
						else if (ultraGridColumn.DataType!=typeof(string) && !ultraGridColumn.DataType.IsValueType)
						{
							ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
							ultraGridColumn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
							ultraGridColumn.CellButtonAppearance.Image = "searchGlyph";
						}
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
			e.Dialog.Text = "Spaltenauswahl";
			e.Dialog.ColumnChooserControl.MultipleBandSupport = MultipleBandSupport.DisplayColumnsFromAllBands;
			e.Dialog.ColumnChooserControl.Style = ColumnChooserStyle.AllColumnsAndChildBandsWithCheckBoxes;
			base.OnBeforeColumnChooserDisplayed(e);
		}


		protected override void OnBeforeSortChange(BeforeSortChangeEventArgs e)
		{
			// wenn Sortierung von aufsteigend nach absteigend geändert wird...
			foreach (UltraGridColumn column in e.SortedColumns)
				if (e.Band.SortedColumns.Exists(column.Key))
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


		protected override void OnInitializePrint(CancelablePrintEventArgs e)
		{
			e.PrintLayout.BorderStyle = UIElementBorderStyle.None;
			e.DefaultLogicalPageLayoutInfo.ClippingOverride = ClippingOverride.No;
			e.DefaultLogicalPageLayoutInfo.ColumnClipMode = ColumnClipMode.RepeatClippedColumns;
		}


		protected override void OnClickCellButton(CellEventArgs e)
		{
			using (new WaitCursorChanger(this))
			using (FormDetails formDetails = new FormDetails(e.Cell.Value))
			{
				formDetails.ShowDialog(this);
				base.OnClickCellButton(e);
			}
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


		[Serializable]
		private class IconValueAppearance : IValueAppearance
		{
			private static readonly Infragistics.Win.Appearance negativeAppearance = new Infragistics.Win.Appearance() { Image = "down" };
			private static readonly Infragistics.Win.Appearance positiveAppearance = new Infragistics.Win.Appearance() { Image = "up" };
			private static readonly Infragistics.Win.Appearance neutralAppearance = new Infragistics.Win.Appearance() { Image = "right" };

			public event EventHandler PropertyChanged;

			public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
			{
				double value;
				if (Double.TryParse(Convert.ToString(dataValue),out value))
				{
					double threshold = 0.05;

					if (value < -threshold)
						IconValueAppearance.negativeAppearance.MergeData(ref appData,ref flags);
					else if (value > threshold)
						IconValueAppearance.positiveAppearance.MergeData(ref appData,ref flags);
					else
						IconValueAppearance.neutralAppearance.MergeData(ref appData,ref flags);
				}
			}

			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}

		[Serializable]
		private class NegativeValueAppearance : IValueAppearance
		{
			private static readonly Infragistics.Win.Appearance negativeAppearance = new Infragistics.Win.Appearance() { ForeColor = Color.Red };

			public event EventHandler PropertyChanged;

			public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
			{
				double value;
				if (Double.TryParse(Convert.ToString(dataValue),out value))
					if (value<0)
						NegativeValueAppearance.negativeAppearance.MergeData(ref appData,ref flags);
			}

			public object Clone()
			{
				return this.MemberwiseClone();
			}
		}



		public bool DrawElement(DrawPhase drawPhase,ref UIElementDrawParams drawParams)
		{
			if (drawParams.Element is HeaderUIElement || drawParams.Element is RowSelectorUIElement)
			{
				bool isHighlight = (drawParams.Element is HeaderUIElement) && this.RectangleToScreen(drawParams.InvalidRect).Contains(Control.MousePosition);

				Color color1 = (isHighlight) ? Color.FromArgb(254,254,254) : Color.FromArgb(248,248,248);
				Color color2 = (isHighlight) ? Color.FromArgb(248,248,248) : Color.FromArgb(242,242,242);

				Rectangle rectangle = (drawParams.Element is HeaderUIElement)
					? new Rectangle(x: drawParams.Element.Rect.X-1,y: drawParams.Element.Rect.Y,width: drawParams.Element.Rect.Width,height: drawParams.Element.Rect.Height-1)
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

					if (column.Band.SortedColumns.Count>1)
					{
						int index = column.Band.SortedColumns.IndexOf(column)+1;

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
	}
}
