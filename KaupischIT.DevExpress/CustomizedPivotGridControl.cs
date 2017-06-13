using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;

namespace KaupischITC.DevExpressControls
{
	/// <summary>
	/// Stellt ein angepasstes PivotGridControl dar
	/// </summary>
	public class CustomizedPivotGridControl : PivotGridControl,IMessageFilter
	{
		private MemoryStream collapsedState; // enthält den gespeicherten Zustand der ausgeklappten Elemente


		/// <summary>
		/// Erstellt ein neues PivotGridControl
		/// </summary>
		public CustomizedPivotGridControl()
		{
			this.LookAndFeel.UseDefaultLookAndFeel = false;		
			this.InitializeLayout();

			// Anpassungen registrieren
			Application.AddMessageFilter(this);
			this.CustomDrawFieldValue += this.OnCustomDrawFieldValue;
			this.PopupMenuShowing += this.OnPopupMenuShowing;
			this.FieldAreaChanged += this.OnFieldAreaChanged;
			this.FieldValueDisplayText += this.OnFieldValueDisplayText;

			// negative Werte rot anzeigen
			PivotGridStyleFormatCondition formatCondition = new PivotGridStyleFormatCondition();
			formatCondition.ApplyToGrandTotalCell = true;
			formatCondition.Condition = FormatConditionEnum.Less;
			formatCondition.Value1 = 0;
			formatCondition.Appearance.ForeColor = Color.Red;
			formatCondition.Appearance.Options.UseForeColor = true;
			this.FormatConditions.Add(formatCondition);

			// Klick in ein Zelle selektiert diese
			this.CellClick += delegate(object sender,PivotCellEventArgs e) { this.Cells.MultiSelection.SetSelection(new Point(e.ColumnIndex,e.RowIndex)); };

			// Diagramm-Einstellungen
			this.OptionsChartDataSource.SelectionOnly = false;
			this.CellSelectionChanged += delegate { this.OptionsChartDataSource.SelectionOnly = this.Cells.MultiSelection.SelectedCells.Count()>1; };
			this.OptionsChartDataSource.AutoTransposeChart = true;
		}
		protected override void Dispose(bool disposing)
		{
			Application.RemoveMessageFilter(this);
			base.Dispose(disposing);
		}


		/// <summary>
		/// Wenn Felder, die keine Standard-Aggregate darstellen, aus dem Feldbereich entfernt werden, das Feld auch komplett entfernen
		/// </summary>
		private void OnFieldAreaChanged(object sender,PivotFieldEventArgs e)
		{
			if (e.Field.Index>0 && e.Field.Area==PivotArea.FilterArea && e.Field.SummaryDisplayType!=PivotSummaryDisplayType.Default)
				this.Fields.RemoveAt(e.Field.Index);

			if (e.Field.Index>0 && e.Field.Area==PivotArea.FilterArea && e.Field.SummaryType!=PivotSummaryType.Sum && this.Fields.Cast<PivotGridField>().Any(pgf => pgf.FieldName==e.Field.FieldName && pgf.SummaryType==PivotSummaryType.Sum))
				this.Fields.RemoveAt(e.Field.Index);
		}


		/// <summary>
		/// Benutzerdefinierte Anzeigetexte in Spaltenköpfen/Zeilenselektoren für Wahrheitswerte und leere Werte
		/// </summary>
		private void OnCustomDrawFieldValue(object sender,PivotCustomDrawFieldValueEventArgs e)
		{
			if (!this.IsAsyncInProgress)
				if (e.Field!=null && e.ValueType==PivotGridValueType.Value)
				{
					if (e.Field.DataType==typeof(bool) || e.Field.DataType==typeof(bool?))
						e.Info.Caption = ((bool?)e.Value==true) ? "Ja" : ((bool?)e.Value==false) ? "Nein" : "n/a";
					if (e.Value==null && String.IsNullOrEmpty(e.Info.Caption))
						e.Info.Caption = "n/a";
				}
		}


		/// <summary>
		/// Mehrzeilige Texte (bzw. mit Zeilenumbruch) verhindern
		/// </summary>
		private void OnFieldValueDisplayText(object sender,PivotFieldDisplayTextEventArgs e)
		{
			e.DisplayText = Regex.Replace(e.DisplayText,@"\s+"," ");
		}


		/// <summary>
		/// Initialisiert die Darstellung des angepassten PivotGridControls
		/// </summary>
		protected virtual void InitializeLayout()
		{
			foreach (AppearanceObject appearance in this.Appearance)
				appearance.Font = SystemFonts.MessageBoxFont;

			this.OptionsFilterPopup.AllowContextMenu = true;
			this.OptionsFilterPopup.GroupFilterMode = PivotGroupFilterMode.Tree;

			this.OptionsBehavior.UseAsyncMode = true;
			this.OptionsCustomization.AllowCustomizationForm = false;
			this.OptionsCustomization.AllowPrefilter = false;
			this.OptionsCustomization.AllowFilterBySummary = false;
			this.OptionsView.RowTotalsLocation = PivotRowTotalsLocation.Tree;
			this.Prefilter.Enabled = false;

			foreach (ScrollBarBase scrollBar in this.Controls.OfType<ScrollBarBase>()) // nicht den hässlichen Scrollbar-Stil aus dem ansonsten schicken Skin verwenden
			{
				scrollBar.LookAndFeel.UseDefaultLookAndFeel = false;
				scrollBar.LookAndFeel.UseWindowsXPTheme = true;
				scrollBar.LookAndFeel.SetWindowsXPStyle();
			}

			// automatisch Aggregatfeld für die Anzahl hinzufügen
			PivotGridField countField = this.Fields.Cast<PivotGridField>().SingleOrDefault(pgf => pgf.Name=="customFieldCount") ?? this.Fields.Add("#",PivotArea.FilterArea);
			countField.Name = "customFieldCount";
			countField.Caption = "#";
			countField.SummaryType = PivotSummaryType.Count;
			countField.UnboundType = UnboundColumnType.Integer;
			countField.UnboundExpression = "1";
			countField.CellFormat.FormatType = FormatType.Numeric;
			countField.CellFormat.FormatString = "N0";
			countField.AllowedAreas = PivotGridAllowedAreas.FilterArea | PivotGridAllowedAreas.DataArea;
		}


		/// <summary>
		/// Nach dem internen Wiederherstellen des Layouts die benutzerdefinierte Layoutanpassung nochmal drüberlaufen lassen
		/// </summary>
		protected override void RestoreLayoutCore(XtraSerializer serializer,object path,OptionsLayoutBase options)
		{
			base.RestoreLayoutCore(serializer,path,options);
			this.InitializeLayout();
		}


		/// <summary>
		/// Einige Steuerelemente haben einige wenige hässliche Elemente aus dem Skin; an diese Steuerelemente kommt man über den MessageFilter ran
		/// </summary>
		public bool PreFilterMessage(ref Message message)
		{
			Control control = Control.FromHandle(message.HWnd);
			// System.Diagnostics.Debug.WriteLineIf(control!=null,control);

			ScrollBarBase scrollBar = control as ScrollBarBase; // hässliche Scrollbalken durch Systemstandard ersetzen
			if (scrollBar!=null && scrollBar.LookAndFeel.UseDefaultLookAndFeel)
			{
				scrollBar.LookAndFeel.UseDefaultLookAndFeel = false;
				scrollBar.LookAndFeel.UseWindowsXPTheme = true;
				scrollBar.LookAndFeel.SetWindowsXPStyle();
			}

			XtraForm xtraForm = control as XtraForm;
			if (xtraForm!=null)
				foreach (ISupportLookAndFeel supportLookAndFeel in xtraForm.Controls.OfType<ISupportLookAndFeel>()) // hässliche Buttons & Checkboxen
					if (supportLookAndFeel.LookAndFeel.UseDefaultLookAndFeel)
					{
						supportLookAndFeel.LookAndFeel.UseDefaultLookAndFeel = false;
						supportLookAndFeel.LookAndFeel.UseWindowsXPTheme = true;
						supportLookAndFeel.LookAndFeel.SetWindowsXPStyle();
					}

			return false;
		}


		/// <summary>
		/// Zeigt die Druckvorschau für den Inhalt des PivotGridControls an
		/// </summary>
		/// <param name="title">der Titel des Druckdokuments</param>
		/// <param name="description">die Beschreibung der Abfrage</param>
		public void ShowPrintPreview(string title,string description)
		{
			PrintingSystem printingSystem = new PrintingSystem();
			printingSystem.Document.Name = title;

			PrintableComponentLink printableComponentLink = new PrintableComponentLink(printingSystem);
			printableComponentLink.Component = this;

			PageHeaderFooter pageHeaderFooter = printableComponentLink.PageHeaderFooter as PageHeaderFooter;
			pageHeaderFooter.Header.Content.AddRange(new[] { " \r\n \r\n "+description,title,"" });
			pageHeaderFooter.Footer.Content.AddRange(new[] { "[Druckdatum] [Druckzeitpunkt]","","Seite [Seite # von #]" });

			this.OptionsPrint.PrintHeadersOnEveryPage = true;
			this.OptionsPrint.PrintFilterHeaders = DefaultBoolean.False;
			this.OptionsPrint.PageSettings.PaperKind = PaperKind.A4;

			printableComponentLink.CreateDocument();
			printableComponentLink.ShowPreviewDialog();
		}


		/// <summary>
		/// Erweiterung der Popup-Menüs
		/// </summary>
		private void OnPopupMenuShowing(object sender,PopupMenuShowingEventArgs e)
		{
			PivotGridField pivotGridField = e.HitInfo.HeaderField;
			if (pivotGridField!=null)
			{
				// PivotSummaryType
				DXSubMenuItem summaryMenuItem = new DXSubMenuItem { Caption = "Zusammenfassung",BeginGroup = true };
				e.Menu.Items.Add(summaryMenuItem);
				var summaryTypes = new Dictionary<string,PivotSummaryType>
				{
					{ "Summe",PivotSummaryType.Sum },
					{ "Durchschnitt",PivotSummaryType.Average },
					{ "Anzahl",PivotSummaryType.Count },
					{ "Minimum",PivotSummaryType.Min },
					{ "Maximum",PivotSummaryType.Max },
				};
				foreach (string key in summaryTypes.Keys)
				{
					PivotSummaryType summaryType = summaryTypes[key];
					DXMenuCheckItem menuItem = new DXMenuCheckItem { Caption = key,Checked = pivotGridField.SummaryType==summaryType };
					menuItem.Click += delegate { this.DuplicatePivotGridField(pivotGridField,summaryType,pivotGridField.SummaryDisplayType); };
					summaryMenuItem.Items.Add(menuItem);
				}

				// PivotSummaryDisplayType
				DXSubMenuItem summaryDisplayMenuItem = new DXSubMenuItem { Caption = "Anzeigen als" };
				e.Menu.Items.Add(summaryDisplayMenuItem);
				var summaryDisplayTypes = new Dictionary<string,PivotSummaryDisplayType>
				{
					{ "Standard",PivotSummaryDisplayType.Default },
					{ "Varianz zum Vorgänger (absolut)",PivotSummaryDisplayType.AbsoluteVariation },
					{ "Varianz zum Vorgänger (prozentual)",PivotSummaryDisplayType.PercentVariation },
					{ "Prozent vom Gesamt (Spalte)",PivotSummaryDisplayType.PercentOfColumn },
					{ "Prozent vom Gesamt (Zeile)",PivotSummaryDisplayType.PercentOfRow },
					{ "Prozent vom Gesamtergebnis (Spalte)",PivotSummaryDisplayType.PercentOfColumnGrandTotal },
					{ "Prozent vom Gesamtergebnis (Zeile)",PivotSummaryDisplayType.PercentOfRowGrandTotal },
				};
				foreach (string key in summaryDisplayTypes.Keys)
				{
					PivotSummaryDisplayType summaryDisplayType = summaryDisplayTypes[key];
					DXMenuCheckItem menuItem = new DXMenuCheckItem { Caption = key,Checked = pivotGridField.SummaryDisplayType==summaryDisplayType };
					menuItem.Click += delegate { this.DuplicatePivotGridField(pivotGridField,pivotGridField.SummaryType,summaryDisplayType); };
					summaryDisplayMenuItem.Items.Add(menuItem);
				}
			}
		}

		/// <summary>
		/// Dupliziert das angegebene Feld
		/// </summary>
		/// <param name="pivotGridField">das Feld, das dupliziert werden soll</param>
		/// <param name="summaryType">der Aggregattyp, der für das duplizierte Feld verwendet werden soll</param>
		/// <param name="summaryDisplayType">die Anzeige des Aggregattyps, der für das duplizierte Feld verwendet werden soll</param>
		private void DuplicatePivotGridField(PivotGridField pivotGridField,PivotSummaryType summaryType,PivotSummaryDisplayType summaryDisplayType)
		{
			PivotGridField duplicatedField = new PivotGridField(pivotGridField.Name+" "+summaryDisplayType+" "+summaryType,pivotGridField.Area);
			duplicatedField.Name = pivotGridField.Name+"-"+summaryDisplayType+"-"+summaryType;
			duplicatedField.Caption = (!String.IsNullOrEmpty(pivotGridField.Caption)) ? pivotGridField.Caption : pivotGridField.FieldName;
			duplicatedField.SummaryType = summaryType;
			duplicatedField.SummaryDisplayType = summaryDisplayType;
			duplicatedField.UnboundType = pivotGridField.UnboundType;
			duplicatedField.UnboundExpression = pivotGridField.UnboundExpression;
			duplicatedField.AllowedAreas = pivotGridField.AllowedAreas;
			duplicatedField.FieldName = pivotGridField.FieldName;
			duplicatedField.CellFormat.FormatType = pivotGridField.CellFormat.FormatType;
			duplicatedField.CellFormat.FormatString = (summaryDisplayType.ToString().StartsWith("Percent")) ? "P" : pivotGridField.CellFormat.FormatString;

			this.Fields.Add(duplicatedField);
		}


		/// <summary>
		/// Speichert den Zustand der ausgeklappten Elemente
		/// </summary>
		public void SaveExpandedState()
		{
			this.collapsedState = new MemoryStream();
			this.SaveCollapsedStateToStream(this.collapsedState);
		}

		/// <summary>
		/// Stellt den Zustand der ausgeklappten Elemente wieder her
		/// </summary>
		public void RestoreExpandedState()
		{
			this.CollapseAll();
			if (this.collapsedState!=null)
			{
				this.collapsedState.Position = 0;
				this.LoadCollapsedStateFromStream(this.collapsedState);
			}
		}
	}
}
