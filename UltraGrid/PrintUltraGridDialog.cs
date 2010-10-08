using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.Drawing.Printing;
using Infragistics.Win;

namespace KaupischITC.Shared
{
	public partial class PrintUltraGridDialog : Form
	{
		private bool preventRefreshFlag = true;
		private UltraGridPrintDocument ultraPrintDocument = new UltraGridPrintDocument();

		public PrintUltraGridDialog(UltraGrid ultraGrid,string title,string description)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.ultraPrintPreviewControl.Settings.Appearance.BackColor = SystemColors.Control;

			this.ultraPrintDocument.Grid = ultraGrid;
			this.ultraPrintPreviewControl.Document = this.ultraPrintDocument;

			this.ultraPrintDocument.Header.TextCenter = title;
			this.ultraPrintDocument.Header.TextLeft = "\r\n\r\n"+description;
		}

		private void PrintUltraGridDialog_Shown(object sender,EventArgs e)
		{
			radioButtonPortrait.Checked = true;

			numericUpDownTop.Value = numericUpDownBottom.Value = numericUpDownLeft.Value = numericUpDownRight.Value = 2;

			//Auslesen der Systemdrucker und Hinzufügen zur ComboBox
			foreach (string drucker in PrinterSettings.InstalledPrinters)
				this.comboBoxPrinter.Items.Add(drucker);
			this.comboBoxPrinter.SelectedItem = new PrinterSettings().PrinterName;

			//Laden der Kopf- und Fußzeile
			radioButtonColumnAutoFitPages.Checked = true;
			numericUpDownAutoFitPageCount.Value = 1;

			ultraPrintDocument.PageBody.BorderStyle = UIElementBorderStyle.None;
			ultraPrintDocument.Page.BorderStyle = UIElementBorderStyle.None;

			ultraPrintDocument.PrintColorStyle = ColorRenderMode.Monochrome;
			// ultraPrintDocument.Header.TextCenter = ultraPrintDocument.DocumentName;
			ultraPrintDocument.Header.Margins.Bottom = 10;
			
			ultraPrintDocument.Footer.TextLeft = "[Date Printed] [Time Printed]";
			ultraPrintDocument.Footer.TextRight = "Seite <#> von <##>";

			this.preventRefreshFlag = false;
			this.RefreshPreview();
		}


		private void buttonCancel_Click(object sender,EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void buttonPrint_Click(object sender,EventArgs e)
		{
			this.RefreshPrintDocumentSettings();
			this.ultraPrintDocument.Print();
		}

		public void RefreshPrintDocumentSettings()
		{
			this.ultraPrintDocument.PrinterSettings.PrinterName = (string)this.comboBoxPrinter.SelectedItem;
			this.ultraPrintDocument.DefaultPageSettings.PaperSize = (PaperSize)this.comboBoxPapersize.SelectedItem;
			this.ultraPrintDocument.DefaultPageSettings.Margins.Top = CmToPrintInch(numericUpDownTop.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Bottom = CmToPrintInch(numericUpDownBottom.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Left = CmToPrintInch(numericUpDownLeft.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Right = CmToPrintInch(numericUpDownRight.Value);
			this.ultraPrintDocument.DefaultPageSettings.Landscape = radioButtonLandscape.Checked;
			this.ultraPrintDocument.FitWidthToPages = (this.radioButtonColumnDefaultSize.Checked) ? 0 : (int)this.numericUpDownAutoFitPageCount.Value;
		}




		private void comboBoxPrinter_SelectedIndexChanged(object sender,EventArgs e)
		{
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrinterSettings.PrinterName = comboBoxPrinter.SelectedItem.ToString();
			List<PaperSize> paperSizes = printDocument.PrinterSettings.PaperSizes.OfType<PaperSize>().ToList();
			this.comboBoxPapersize.DataSource =  paperSizes;
			this.comboBoxPapersize.DisplayMember = "PaperName";
			this.comboBoxPapersize.SelectedItem = paperSizes.FirstOrDefault(ps => ps.PaperName==printDocument.DefaultPageSettings.PaperSize.PaperName);

			this.RefreshPreview();
		}


		private decimal PrintInchToCm(int inch)
		{
			return (decimal)(inch / 39.37);
		}

		private int CmToPrintInch(decimal cm)
		{
			return (int)((float)cm / 0.0254);
		}


		private void numericUpDownColumn_ValueChanged(object sender,EventArgs e)
		{
			radioButtonColumnAutoFitPages.Checked = true;
			this.RefreshPreview();
		}


		private void RefreshPreview()
		{
			if (!this.preventRefreshFlag)
			{
				this.RefreshPrintDocumentSettings();
				this.ultraPrintPreviewControl.GeneratePreview(true);
			}
		}

		private void comboBoxPapersize_SelectedIndexChanged(object sender,EventArgs e)
		{
			this.RefreshPreview();
		}

		private void numericUpDownTop_ValueChanged(object sender,EventArgs e)
		{
			this.RefreshPreview();
		}

		private void radioButtonLandscape_CheckedChanged(object sender,EventArgs e)
		{
			if (((RadioButton)sender).Checked)
				this.RefreshPreview();
		}
	}
}
