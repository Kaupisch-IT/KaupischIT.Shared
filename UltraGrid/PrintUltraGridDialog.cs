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
		private int paperWidth;
		private int paperHeight;
		private bool klickAufruf = true;
		private UltraGridPrintDocument ultraPrintDocument = new UltraGridPrintDocument();

		public PrintUltraGridDialog(UltraGrid ultraGrid)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();
			this.ultraPrintPreviewControl.Settings.Appearance.BackColor = SystemColors.Control;

			this.ultraPrintDocument.Grid = ultraGrid;
			this.ultraPrintPreviewControl.Document = this.ultraPrintDocument;
		}

		private void PrintUltraGridDialog_Shown(object sender,EventArgs e)
		{
			radioButtonPortrait.Checked = true;

			numericUpDownTop.Value = PrintInchToCm(98);
			numericUpDownBottom.Value = PrintInchToCm(98);
			numericUpDownLeft.Value = PrintInchToCm(98);
			numericUpDownRight.Value = PrintInchToCm(98);

			//Auslesen der Systemdrucker und Hinzufügen zur ComboBox
			foreach (string drucker in PrinterSettings.InstalledPrinters)
				this.comboBoxPrinter.Items.Add(drucker);
			this.comboBoxPrinter.SelectedItem = new PrinterSettings().PrinterName;


			//Laden der Seitenbreite und -höhe
			paperWidth = 827;
			paperHeight = 1169;

			comboBoxPapersize.SelectedItem = GetPaperSizeName(paperWidth,paperHeight);

			klickAufruf = false;
			//Hochformat
			if (radioButtonPortrait.Checked)
			{
				this.numericUpDownPaperWidth.Value = PrintInchToCm(paperWidth);
				this.numericUpDownPaperHeight.Value = PrintInchToCm(paperHeight);
			}
			//Querformat
			else
			{
				this.numericUpDownPaperWidth.Value = PrintInchToCm(paperHeight);
				this.numericUpDownPaperHeight.Value = PrintInchToCm(paperWidth);
			}
			klickAufruf = true;

			//Laden der Kopf- und Fußzeile
			radioButtonColumn2.Checked = true;
			numericUpDownColumn.Value = 1;

			ultraPrintDocument.PageBody.BorderStyle = UIElementBorderStyle.None;
			ultraPrintDocument.Page.BorderStyle = UIElementBorderStyle.None;

			//ultraPrintDocument.PrintColorStyle = ColorRenderMode.Monochrome;
			ultraPrintDocument.Header.TextCenter = ultraPrintDocument.DocumentName;

			ultraPrintDocument.Header.TextRight = "[Date Printed]";
			ultraPrintDocument.Footer.TextLeft = "[Date Printed] [Time Printed]";
			ultraPrintDocument.Footer.TextRight = "Seite <#> von <##>";

			this.ultraPrintPreviewControl.GeneratePreview(false);
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

		private void RefreshPrintDocumentSettings()
		{
			this.ultraPrintDocument.PrinterSettings.PrinterName = this.comboBoxPrinter.SelectedItem.ToString();
			PaperSize paperSize = new PaperSize("MySize",paperWidth,paperHeight);
			this.ultraPrintDocument.DefaultPageSettings.PaperSize = paperSize;
			this.ultraPrintDocument.DefaultPageSettings.Margins.Top = CmToPrintInch(numericUpDownTop.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Bottom = CmToPrintInch(numericUpDownBottom.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Left = CmToPrintInch(numericUpDownLeft.Value);
			this.ultraPrintDocument.DefaultPageSettings.Margins.Right = CmToPrintInch(numericUpDownRight.Value);
			this.ultraPrintDocument.DefaultPageSettings.Landscape = radioButtonLandscape.Checked;
		}

		


		private void comboBoxPrinter_SelectedIndexChanged(object sender,EventArgs e)
		{
			//Leeren der ComboBox
			this.comboBoxPapersize.Items.Clear();

			//Abrufen aller Papierformate des gewählten Druckers und hinzufügen zur ComboBox
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrinterSettings.PrinterName = comboBoxPrinter.SelectedItem.ToString();
			foreach (PaperSize papierformat in printDocument.PrinterSettings.PaperSizes)
				this.comboBoxPapersize.Items.Add(papierformat.PaperName);

			//Das benutzerdefinierte Papierformat hinzufügen
			this.comboBoxPapersize.Items.Add("Benutzerdefiniertes Format");

			//Von zuvor ausgewähltem Papierformat Namen ermitteln
			this.comboBoxPapersize.SelectedItem = GetPaperSizeName(this.paperWidth,this.paperHeight);
		}


		private void comboBoxPapersize_SelectedIndexChanged(object sender,EventArgs e)
		{
			if (!klickAufruf) return;

			//Drucker festlegen
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrinterSettings.PrinterName = comboBoxPrinter.SelectedItem.ToString();

			//Da die PrintDocument-Papierformat-Liste nicht über den Namen angesprochen werden kann,
			//muss jedes Format auf den Namen überprüft werden
			foreach (PaperSize papierformat in printDocument.PrinterSettings.PaperSizes)
			{
				//Im Trefferfall wird die Höhe und Breite festgelegt
				if (papierformat.PaperName == this.comboBoxPapersize.SelectedItem.ToString())
				{
					paperWidth = papierformat.Width;
					paperHeight = papierformat.Height;
					klickAufruf = false;
					//Hochformat
					if (radioButtonPortrait.Checked)
					{
						//paperWidth = papierformat.Width;
						//paperHeight = papierformat.Height;
						//klickAufruf = false;
						numericUpDownPaperWidth.Value = PrintInchToCm(papierformat.Width);
						numericUpDownPaperHeight.Value = PrintInchToCm(papierformat.Height);
						//klickAufruf = true;
						//break;
					}

					//Querformat
					else
					{
						//paperWidth = papierformat.Height;
						//paperHeight = papierformat.Width;
						//klickAufruf = false;
						numericUpDownPaperWidth.Value = PrintInchToCm(papierformat.Height);
						numericUpDownPaperHeight.Value = PrintInchToCm(papierformat.Width);
						//klickAufruf = true;
						//break;
					}
					klickAufruf = true;
					break;
				}
			}
		}

		private string GetPaperSizeName(int width,int height)
		{
			//Drucker festlegen
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrinterSettings.PrinterName = comboBoxPrinter.SelectedItem as string ?? comboBoxPrinter.Items[0] as string;

			//Jedes Papierformat wird überprüft, ob es die Höhe und Breite wie das
			//übergebene Format hat. Eine Toleranz von 17/100 Zoll (= 0,42cm) wird einberechnet.
			foreach (PaperSize papierformat in printDocument.PrinterSettings.PaperSizes)
				//Im Erfolgsfall wird der Papierformatname zurückgegeben
				if ((papierformat.Width + 17 >= width & papierformat.Width - 17 <= width) && (papierformat.Height + 17 >= height & papierformat.Height - 17 <= height)) return papierformat.PaperName;

			//Ansonsten das Benutzerdefinierte Papierformat
			return "Benutzerdefiniertes Format";
		}

		private void numericUpDownPaperWidth_ValueChanged(object sender,EventArgs e)
		{
			if (klickAufruf)
			{
				//Hochformat
				if (radioButtonPortrait.Checked)
				{
					klickAufruf = false;
					paperWidth = CmToPrintInch(numericUpDownPaperWidth.Value);
					comboBoxPapersize.SelectedItem = GetPaperSizeName(CmToPrintInch(numericUpDownPaperWidth.Value),CmToPrintInch(numericUpDownPaperHeight.Value));
					klickAufruf = true;
				}

				//Querformat
				else
				{
					klickAufruf = false;
					paperHeight = CmToPrintInch(numericUpDownPaperWidth.Value);
					comboBoxPapersize.SelectedItem = GetPaperSizeName(CmToPrintInch(numericUpDownPaperHeight.Value),CmToPrintInch(numericUpDownPaperWidth.Value));
					klickAufruf = true;
				}
			}
		}

		private void numericUpDownPaperHeight_ValueChanged(object sender,EventArgs e)
		{
			if (klickAufruf)
			{
				//Hochformat
				if (radioButtonPortrait.Checked)
				{
					klickAufruf = false;
					paperHeight = CmToPrintInch(numericUpDownPaperHeight.Value);
					comboBoxPapersize.SelectedItem = GetPaperSizeName(CmToPrintInch(numericUpDownPaperWidth.Value),CmToPrintInch(numericUpDownPaperHeight.Value));
					klickAufruf = true;
				}

				//Querformat
				else
				{
					klickAufruf = false;
					paperWidth = CmToPrintInch(numericUpDownPaperHeight.Value);
					comboBoxPapersize.SelectedItem = GetPaperSizeName(CmToPrintInch(numericUpDownPaperHeight.Value),CmToPrintInch(numericUpDownPaperWidth.Value));
					klickAufruf = true;
				}
			}
		}

		private decimal PrintInchToCm(int inch)
		{
			return (decimal)(inch / 39.37);
		}

		private int CmToPrintInch(decimal cm)
		{
			return (int)((float)cm / 0.0254);
		}

		private void radioButtonPortrait_CheckedChanged(object sender,EventArgs e)
		{
			if (radioButtonPortrait.Checked)
			{
				decimal cache;
				cache = numericUpDownTop.Value;
				numericUpDownTop.Value = numericUpDownLeft.Value;
				numericUpDownLeft.Value = numericUpDownBottom.Value;
				numericUpDownBottom.Value = numericUpDownRight.Value;
				numericUpDownRight.Value = cache;
				cache = numericUpDownPaperHeight.Value;
				numericUpDownPaperHeight.Value = numericUpDownPaperWidth.Value;
				numericUpDownPaperWidth.Value = cache;
			}
		}

		private void radioButtonLandscape_CheckedChanged(object sender,EventArgs e)
		{
			if (radioButtonLandscape.Checked)
			{
				decimal cache;
				cache = numericUpDownLeft.Value;
				numericUpDownLeft.Value = numericUpDownTop.Value;
				numericUpDownTop.Value = numericUpDownRight.Value;
				numericUpDownRight.Value = numericUpDownBottom.Value;
				numericUpDownBottom.Value = cache;
				cache = numericUpDownPaperHeight.Value;
				numericUpDownPaperHeight.Value = numericUpDownPaperWidth.Value;
				numericUpDownPaperWidth.Value = cache;
			}
		}

		private void numericUpDownColumn_ValueChanged(object sender,EventArgs e)
		{
			radioButtonColumn2.Checked = true;
		}


		private void button1_Click(object sender,EventArgs e)
		{
			this.RefreshPrintDocumentSettings();
			 this.ultraPrintPreviewControl.GeneratePreview(true);
		}
	}
}
