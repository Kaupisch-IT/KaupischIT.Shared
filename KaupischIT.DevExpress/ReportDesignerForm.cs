using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using KaupischIT.DevExpressControls.Properties;

namespace KaupischIT.DevExpressControls
{
	/// <summary>
	/// Stellt ein Fenster zum Entwerfen eines Berichtes dar
	/// </summary>
	public partial class ReportDesignerForm : Form
	{
		/// <summary>
		/// Gibt den zu bearbeitenden Bericht zurück oder legt diesen fest
		/// </summary>
		public XtraReport Report
		{
			get => this.reportDesigner.Report;
			set => this.reportDesigner.OpenReport(value);
		}


		/// <summary>
		/// Erstellt ein neues Fenster zum Entwerfen eines Berichtes
		/// </summary>
		public ReportDesignerForm()
		{
			this.InitializeComponent();

			Rectangle workingArea = Screen.FromControl(this).WorkingArea;
			this.Size = new Size(Math.Min(workingArea.Width,Settings.Default.ReportDesignerFormSize.Width),Math.Min(workingArea.Height,Settings.Default.ReportDesignerFormSize.Height));
			this.FormClosing += delegate
			{
				Settings.Default.ReportDesignerFormSize = this.Size;
				Settings.Default.Save();
			};
		}


		/// <summary>
		/// Beim Schließen des Fensters den Bericht wieder freigeben
		/// </summary>
		private void ReportDesignerForm_FormClosed(object sender,FormClosedEventArgs e) => this.reportDesigner.CloseReport();
	}
}
