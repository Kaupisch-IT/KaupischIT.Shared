using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data.Browsing.Design;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

namespace KaupischITC.DevExpressControls
{
	/// <summary>
	/// Stellt ein Benutzersteuerelement zum Entwerfen eines Berichtes dar
	/// </summary>
	public partial class ReportDesigner : UserControl,ICommandHandler
	{
		/// <summary>
		/// Statischer Konstruktor
		/// </summary>
		static ReportDesigner()
		{
			UserLookAndFeel.Default.UseDefaultLookAndFeel = false;
			UserLookAndFeel.Default.SkinName = "Seven Classic";

			ColumnImageProvider.Instance = new CustomColumnImageProvider();
		}


		/// <summary>
		/// Gibt den aktutellen Bericht zurück
		/// </summary>
		public XtraReport Report
		{
			get { return this.xrDesignPanel.Report; }
		}


		/// <summary>
		/// Erstellt ein neues Benutzersteuerelement zum Entwerfen eines Berichtes
		/// </summary>
		public ReportDesigner()
		{
			InitializeComponent();
			this.xrDesignPanel.AddCommandHandler(this);
		}


		/// <summary>
		/// Öffnet den angegebenen Bericht zum Bearbeiten
		/// </summary>
		/// <param name="report">der Bericht, der geöffnet werden soll</param>
		public void OpenReport(XtraReport report)
		{
			this.xrDesignPanel.OpenReport(report);
		}

		/// <summary>
		/// Schließt den aktuell geöffneten Bericht
		/// </summary>
		public void CloseReport()
		{
			this.xrDesignPanel.CloseReport();
		}


		/// <summary>
		/// Die Standard-Verarbeitung von bestimmten UI-Ereignissen/-Kommandos verhindern
		/// </summary>
		public bool CanHandleCommand(ReportCommand command,ref bool useNextHandler)
		{
			useNextHandler = !(command==ReportCommand.SaveFile || command==ReportCommand.SaveFileAs || command==ReportCommand.Closing || command==ReportCommand.NewReport);
			return !useNextHandler;
		}
		public void HandleCommand(ReportCommand command,object[] args) { }


		/// <summary>
		/// Stellt eine benutzerdefinierte Bilderquelle für die Feldauswahl bereit
		/// </summary>
		private class CustomColumnImageProvider : ColumnImageProvider
		{
			public override ImageCollection CreateImageCollection()
			{
				ImageCollection result = base.CreateImageCollection();

				ComponentResourceManager resources = new ComponentResourceManager(typeof(ReportDesigner));
				ImageList imageList = new ImageList();
				imageList.ImageStream = (ImageListStreamer)resources.GetObject("imageList.ImageStream");

				for (int i=0;i<imageList.Images.Count;i++)
					result.Images[i] = imageList.Images[i];

				return result;
			}
		}
	}
}
