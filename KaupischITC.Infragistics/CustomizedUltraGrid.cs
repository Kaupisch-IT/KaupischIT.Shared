﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischITC.Extensions;
using KaupischITC.Shared;
using KaupischITC.Charting;

namespace KaupischITC.InfragisticsControls
{
	public partial class CustomizedUltraGrid : UltraGrid,IUIElementCreationFilter
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
			this.visualizationToolStripMenuItem.DropDownItems.Add("Kreisdiagramm anzeigen",null,delegate { this.ShowPieForm(); });
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


		private void ShowPieForm()
		{
			var elements = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Where(ugr => !ugr.Hidden && !ugr.IsFilteredOut).Select(ugr => ugr.ListObject).ToList();
			if (elements.Any())
			{
				PieChartForm pieForm = new PieChartForm()
				{
					DisplayedType = elements.First().GetType(),
					Elements = elements,
					EmphasizedElements = this.ContextUltraGridRow.ParentCollection.Cast<UltraGridRow>().Where(ugr => ugr.Selected).Select(ugr => ugr.ListObject).ToList(),
					ShowInTaskbar = false,
					DisplayMember = this.ContextUltraGridColumn.Band.GetFirstVisibleCol(this.ActiveColScrollRegion,true).Key,
					ValueMember = this.ContextUltraGridColumn.Key,
					GetFormatString = (columnName) => (this.ContextUltraGridColumn.Band.Columns.Exists(columnName)) ? this.ContextUltraGridColumn.Band.Columns[columnName].Format : null
				};
				pieForm.Show(this);
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

				this.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.RowIndex;
				this.DisplayLayout.Override.ExpansionIndicator = ShowExpansionIndicator.CheckOnDisplay;
				this.DisplayLayout.Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
				this.DisplayLayout.Override.RowSelectorNumberStyle = RowSelectorNumberStyle.RowIndex;
				this.DisplayLayout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
				this.DisplayLayout.Override.AllowColSizing = AllowColSizing.Synchronized;
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

				this.SuspendRowSynchronization();

				foreach (UltraGridBand ultraGridBand in e.Layout.Bands)
				{
					ultraGridBand.HeaderVisible = (ultraGridBand.Index!=0 && ultraGridBand.Key!="Elements");
					ultraGridBand.Header.Appearance.TextHAlign = HAlign.Left;
					ultraGridBand.RowLayoutStyle = RowLayoutStyle.GroupLayout;

					foreach (UltraGridColumn ultraGridColumn in ultraGridBand.Columns)
					{
						ultraGridColumn.CellActivation = Activation.ActivateOnly;
						ultraGridColumn.HiddenWhenGroupBy = DefaultableBoolean.True;

						if (ultraGridColumn.DataType.IsNumeric())
							ultraGridColumn.CellAppearance.TextHAlign = HAlign.Right;

						if (ultraGridColumn.Format==null)
						{
							if (ultraGridColumn.Key.EndsWith("Preis"))
								ultraGridColumn.Format = "C";
							else if (Regex.IsMatch(ultraGridColumn.Key,"[^A-Z]P$"))
								ultraGridColumn.Format = "P";
						}

						if (ultraGridColumn.Key.EndsWith("ID") || ultraGridColumn.Key.EndsWith("Id") || ultraGridColumn.PropertyDescriptor.Attributes.OfType<BrowsableAttribute>().Any(ba => !ba.Browsable))
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


		protected override void OnInitializePrint(CancelablePrintEventArgs e)
		{
			e.PrintLayout.BorderStyle = UIElementBorderStyle.None;
			e.DefaultLogicalPageLayoutInfo.ClippingOverride = ClippingOverride.No;
			e.DefaultLogicalPageLayoutInfo.ColumnClipMode = ColumnClipMode.RepeatClippedColumns;
		}


		protected override void OnClickCellButton(CellEventArgs e)
		{
			using (new WaitCursorChanger(this))
			{
				new FormDetails(e.Cell.Value).ShowDialog(this);
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
	}
}