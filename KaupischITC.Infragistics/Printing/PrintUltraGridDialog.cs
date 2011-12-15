using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls.Printing
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

			this.ultraPrintDocument.DocumentName = title;
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
			this.radioButtonColumnAutoFitPages.Checked = true;
			this.numericUpDownAutoFitPageCount.Value = 1;
			this.ultraPrintDocument.PageBody.BorderStyle = UIElementBorderStyle.None;
			this.ultraPrintDocument.Page.BorderStyle = UIElementBorderStyle.None;
			this.ultraPrintDocument.Header.Margins.Bottom = 10;
			this.ultraPrintDocument.Footer.TextLeft = "[Date Printed] [Time Printed]";
			this.ultraPrintDocument.Footer.TextRight = "Seite <#> von <##>";

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
			this.ultraPrintDocument.PrinterSettings.Duplex = (this.checkBoxDuplex.Checked) ? Duplex.Default : Duplex.Simplex;
			this.ultraPrintDocument.PrinterSettings.Copies = (short)this.numericUpDownCopies.Value;
			this.ultraPrintDocument.PrintColorStyle = (ColorRenderMode)comboBoxColorStyle.SelectedValue;
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
			this.checkBoxDuplex.Visible = printDocument.PrinterSettings.CanDuplex;
			if (!this.checkBoxDuplex.Visible)
				this.checkBoxDuplex.Checked = false;

			var colorStyles = new[]
			{
				new ColorStyle { ColorRenderMode = ColorRenderMode.GrayScale, Name = "Graustufen" },
				new ColorStyle { ColorRenderMode = ColorRenderMode.Monochrome,Name = "Schwarzweiß" }
			};
			if (printDocument.PrinterSettings.SupportsColor)
				colorStyles = new[] { new ColorStyle { ColorRenderMode = ColorRenderMode.Color,Name = "Farbdruck" } }.Concat(colorStyles).ToArray();

			string selectedText = this.comboBoxColorStyle.Text;
			this.comboBoxColorStyle.DataSource = colorStyles;
			this.comboBoxColorStyle.DisplayMember = "Name";
			this.comboBoxColorStyle.ValueMember = "ColorRenderMode";
			this.comboBoxColorStyle.Text = selectedText;

			this.RefreshPreview();
		}


		[Obfuscation(Feature = "renaming",ApplyToMembers=true,Exclude=true)]
		private class ColorStyle
		{
			public string Name { get; set; }
			public ColorRenderMode ColorRenderMode { get; set; }
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

		private void radioButton_CheckedChanged(object sender,EventArgs e)
		{
			if (((RadioButton)sender).Checked)
				this.RefreshPreview();
		}

		private void comboBoxColorStyle_SelectionChangeCommitted(object sender,EventArgs e)
		{
			this.RefreshPreview();
		}
	}
}
