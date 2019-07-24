using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischIT.Extensions;

namespace KaupischIT.InfragisticsControls.Printing
{
	/// <summary>
	/// Stellt ein Druckvorschau-Dialogfenster für ein UltraGrid dar
	/// </summary>
	public partial class PrintUltraGridDialog : Form
	{
		private bool preventRefreshFlag = true;
		private int pageNumber;
		private readonly UltraGridPrintDocument ultraPrintDocument = new UltraGridPrintDocument();


		/// <summary>
		/// Erstellt ein neues Druckvorschau-Dialogfenster für ein UltraGrid
		/// </summary>
		/// <param name="ultraGrid">das UltraGrid, das gedruckt werden soll</param>
		/// <param name="title">der Titel des Druckdokuments</param>
		/// <param name="description">die Beschreibung des Druckdokuments</param>
		public PrintUltraGridDialog(UltraGrid ultraGrid,string title,string description)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.ultraPrintPreviewControl.Settings.Appearance.BackColor = SystemColors.Control;

			this.ultraPrintDocument.Grid = ultraGrid;
			this.ultraPrintPreviewControl.Document = this.ultraPrintDocument;

			this.ultraPrintDocument.DocumentName = title;
			this.ultraPrintDocument.Header.TextCenter = title;
			this.ultraPrintDocument.Header.TextLeft = "\r\n\r\n"+description;
		}


		/// <summary>
		/// Sobald das Druckvorschau-Dialogfenster angezeigt wird
		/// </summary>
		private void PrintUltraGridDialog_Shown(object sender,EventArgs e)
		{
			this.radioButtonPortrait.Checked = true;

			this.numericUpDownTop.Value = this.numericUpDownBottom.Value = this.numericUpDownLeft.Value = this.numericUpDownRight.Value = 2;

			//Auslesen der Systemdrucker und Hinzufügen zur ComboBox
			foreach (string drucker in PrinterSettings.InstalledPrinters)
				this.comboBoxPrinter.Items.Add(drucker);
			this.comboBoxPrinter.SelectedItem = new PrinterSettings().PrinterName;

			//Laden der Kopf- und Fußzeile
			this.radioButtonColumnAutoFitPages.Checked = true;
			this.numericUpDownAutoFitPageCount.Value = 1;
			this.ultraPrintDocument.PageBody.BorderStyle = UIElementBorderStyle.None;
			this.ultraPrintDocument.Page.BorderStyle = UIElementBorderStyle.None;
			this.ultraPrintDocument.Header.Margins.Bottom = 10;
			this.ultraPrintDocument.Footer.TextLeft = "[Date Printed] [Time Printed]";
			this.ultraPrintDocument.BeginPrint += delegate { this.pageNumber = 0; };
			this.ultraPrintDocument.PagePrinting += delegate
			{
				this.ultraPrintDocument.Footer.TextRight = (this.ultraPrintDocument.PrinterSettings.PrintRange==PrintRange.AllPages) ? "Seite <#> von <##>" : "Seite "+(this.ultraPrintDocument.PrinterSettings.FromPage+this.pageNumber);
				this.pageNumber++;
			};

			this.preventRefreshFlag = false;
			this.RefreshPreview();
		}


		/// <summary>
		/// Beim Klicken auf den "Abbrechen"-Button
		/// </summary>
		private void ButtonCancel_Click(object sender,EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		/// <summary>
		/// Beim Klicken auf den "Drucken"-Button
		/// </summary>
		private void ButtonPrint_Click(object sender,EventArgs e)
		{
			this.RefreshPrintDocumentSettings();

			if (String.IsNullOrEmpty(this.textBoxPages.Text))
			{
				this.ultraPrintDocument.PrinterSettings.PrintRange = PrintRange.AllPages;
				this.ultraPrintDocument.Print();
			}
			else
			{
				string pageRange = ","+Regex.Replace(this.textBoxPages.Text,@"\s+","").Trim(',')+",";
				if (!Regex.IsMatch(pageRange,@"^,(((?<single>\d+)|(?<range>\d+-\d+)),)+$"))
					MessageBox.Show("Bitte prüfen Sie die Angabe des Druckbereichs.\r\n"+this.toolTipPageRange.GetToolTip(this.textBoxPages),"Fehler beim Druckbereich",MessageBoxButtons.OK,MessageBoxIcon.Error);
				else
				{
					foreach (Match ma in Regex.Matches(pageRange,@",(?<page>\d{1,6})(?=,)"))
					{
						int page = Int32.Parse(ma.Groups["page"].Value);
						this.ultraPrintDocument.PrinterSettings.PrintRange = PrintRange.SomePages;
						this.ultraPrintDocument.PrinterSettings.FromPage = this.ultraPrintDocument.PrinterSettings.ToPage = page;
						this.ultraPrintDocument.Print();
					}
					foreach (Match ma in Regex.Matches(pageRange,@",(?<from>\d{1,6})-(?<to>\d{1,6})(?=,)"))
					{
						int from = Int32.Parse(ma.Groups["from"].Value);
						int to = Int32.Parse(ma.Groups["to"].Value);
						this.ultraPrintDocument.PrinterSettings.PrintRange = PrintRange.SomePages;
						this.ultraPrintDocument.PrinterSettings.FromPage = from;
						this.ultraPrintDocument.PrinterSettings.ToPage = to;
						this.ultraPrintDocument.Print();
					}
				}
			}

			this.ultraPrintDocument.PrinterSettings.PrintRange = PrintRange.AllPages;
		}


		/// <summary>
		/// Aktualisiert die Druckdokument-Einstellungen anhand der im Druckvorschaudialog vorgenommenen Einstellungen
		/// </summary>
		public void RefreshPrintDocumentSettings()
		{
			this.ultraPrintDocument.PrinterSettings.PrinterName = (string)this.comboBoxPrinter.SelectedItem;
			this.ultraPrintDocument.PrinterSettings.Duplex = (this.checkBoxDuplex.Checked) ? Duplex.Default : Duplex.Simplex;
			this.ultraPrintDocument.PrinterSettings.Copies = (short)this.numericUpDownCopies.Value;
			this.ultraPrintDocument.PrintColorStyle = (ColorRenderMode)this.comboBoxColorStyle.SelectedValue;
			this.ultraPrintDocument.DefaultPageSettings.PaperSize = (PaperSize)this.comboBoxPapersize.SelectedItem;
			this.ultraPrintDocument.DefaultPageSettings.Margins.Top = CmToPrintInch(this.numericUpDownTop.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Bottom = CmToPrintInch(this.numericUpDownBottom.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Left = CmToPrintInch(this.numericUpDownLeft.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Right = CmToPrintInch(this.numericUpDownRight.Value);
			this.ultraPrintDocument.DefaultPageSettings.Landscape = this.radioButtonLandscape.Checked;
			this.ultraPrintDocument.FitWidthToPages = (this.radioButtonColumnDefaultSize.Checked) ? 0 : (int)this.numericUpDownAutoFitPageCount.Value;
		}


		/// <summary>
		/// Wenn der ausgewählte Drucker geändert wird, die auswählbaren Drucker- und Seiteneinstellungen aktualisieren
		/// </summary>
		private void ComboBoxPrinter_SelectedIndexChanged(object sender,EventArgs e)
		{
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrinterSettings.PrinterName = this.comboBoxPrinter.SelectedItem.ToString();
			List<PaperSize> paperSizes = printDocument.PrinterSettings.PaperSizes.OfType<PaperSize>().ToList();
			this.comboBoxPapersize.DataSource =  paperSizes;
			this.comboBoxPapersize.DisplayMember = "PaperName";
			this.comboBoxPapersize.SelectedItem = paperSizes.FirstOrDefault(ps => ps.PaperName==printDocument.DefaultPageSettings.PaperSize.PaperName);
			this.checkBoxDuplex.Visible = printDocument.PrinterSettings.CanDuplex;
			if (!this.checkBoxDuplex.Visible)
				this.checkBoxDuplex.Checked = false;

			ColorStyleViewModel[] colorStyles = new[]
			{
				new ColorStyleViewModel { ColorRenderMode = ColorRenderMode.GrayScale, Name = "Graustufen" },
				new ColorStyleViewModel { ColorRenderMode = ColorRenderMode.Monochrome,Name = "Schwarzweiß" }
			};
			if (printDocument.PrinterSettings.SupportsColor)
				colorStyles = new[] { new ColorStyleViewModel { ColorRenderMode = ColorRenderMode.Color,Name = "Farbdruck" } }.Concat(colorStyles).ToArray();

			string selectedText = this.comboBoxColorStyle.Text;
			this.comboBoxColorStyle.DataSource = colorStyles;
			this.comboBoxColorStyle.DisplayMember = GetMemberName.Of<ColorStyleViewModel>(cs => cs.Name);
			this.comboBoxColorStyle.ValueMember = GetMemberName.Of<ColorStyleViewModel>(cs => cs.ColorRenderMode);
			this.comboBoxColorStyle.Text = selectedText;

			this.RefreshPreview();
		}


		/// <summary> Stellt Dialog-Informationen über Farbeinstellungen des Druckers bereit </summary>
		private class ColorStyleViewModel
		{
			public string Name { get; set; }
			public ColorRenderMode ColorRenderMode { get; set; }
		}

		/// <summary>
		/// Wandelt Drucker-Zoll in Zentimeter um
		/// </summary>
		private decimal PrintInchToCm(int inch) => (decimal)(inch / 39.37);

		/// <summary>
		/// Wandelt Zentimeter in Drucker-Zoll 
		/// </summary>
		private int CmToPrintInch(decimal cm) => (int)((float)cm / 0.0254);


		/// <summary>
		/// Aktualisiert die Druckvorschau entsprechend der vorgenommenen Einstellungen
		/// </summary>
		private void RefreshPreview()
		{
			if (!this.preventRefreshFlag)
			{
				this.RefreshPrintDocumentSettings();
				this.ultraPrintPreviewControl.GeneratePreview(recreate: true);
			}
		}
		private void NumericUpDownColumn_ValueChanged(object sender,EventArgs e)
		{
			this.radioButtonColumnAutoFitPages.Checked = true;
			this.RefreshPreview();
		}
		private void ComboBoxPapersize_SelectedIndexChanged(object sender,EventArgs e) => this.RefreshPreview();
		private void NumericUpDownTop_ValueChanged(object sender,EventArgs e) => this.RefreshPreview();
		private void RadioButton_CheckedChanged(object sender,EventArgs e)
		{
			if (((RadioButton)sender).Checked)
				this.RefreshPreview();
		}
		private void ComboBoxColorStyle_SelectionChangeCommitted(object sender,EventArgs e) => this.RefreshPreview();
	}
}
