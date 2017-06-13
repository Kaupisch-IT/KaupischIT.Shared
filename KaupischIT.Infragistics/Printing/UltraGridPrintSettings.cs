
namespace KaupischITC.InfragisticsControls.Printing
{
	/// <summary>
	/// Stellt Informationen über Druckeinstellungen bereit
	/// </summary>
	public class UltraGridPrintSettings
	{
		/// <summary>
		/// Gibt den oberen Seitenrand zurück oder legt diesen fest
		/// </summary>
		public int MarginTop { get; set; }
		
		/// <summary>
		/// Gibt den unteren Seitenrand zurück oder legt diesen fest
		/// </summary>
		public int MarginBottom { get; set; }

		/// <summary>
		/// Gibt den linken Seitenrand zurück oder legt diesen fest
		/// </summary>
		public int MarginLeft { get; set; }

		/// <summary>
		/// Gibt den rechten Seitenrand zurück oder legt diesen fest
		/// </summary>
		public int MarginRight { get; set; }

		/// <summary>
		/// Gibt an, ob die Seitenausrichtung Querformat ist, oder legt dies fest
		/// </summary>
		public bool PageOrientationIsLandscape { get; set; }

		/// <summary>
		/// Gibt an, ob Spaltenbreiten auf die Seitenbreite angepasst werden, oder legt dies fest
		/// </summary>
		public bool AdaptColumns { get; set; }

		/// <summary>
		/// Gibt an, auf wie viele Seitenbreiten die Spalten angepasst werden sollen, oder legt dies fest
		/// </summary>
		public int AdaptColumnsToPages { get; set; }

		/// <summary>
		/// Gibt die Seitenbreite zurück oder legt diese fest
		/// </summary>
		public int PaperSizeWidth { get; set; }

		/// <summary>
		/// Gibt die Seitenhöhe zurück oder legt diese fest
		/// </summary>
		public int PaperSizeHeight { get; set; }

		/// <summary>
		/// Gibt den Namen des Druckers zurück oder legt diesen fest
		/// </summary>
		public string Printer { get; set; }

		/// <summary>
		/// Gibt den Kopfszeilentext zurück oder legt diesen fest
		/// </summary>
		public string HeaderText { get; set; }

		/// <summary>
		/// Gibt die Ausrichtung der der Kopfzeile zurück oder legt diese fest
		/// </summary>
		public string HeaderAlign { get; set; }

		/// <summary>
		/// Gibt den Fußzeilentext zurück oder legt diesen fest
		/// </summary>
		public string FooterText { get; set; }

		/// <summary>
		/// Gibt die Ausrichtung der Fußzeile zurück oder legt diese fest
		/// </summary>
		public string FooterAlign { get; set; }


		/// <summary>
		/// Erstellt ein neues UltraGridPrintSettings-Objekt
		/// </summary>
		public UltraGridPrintSettings()
		{
			this.MarginTop = 98;
			this.MarginBottom = 79;
			this.MarginLeft = 98;
			this.MarginRight = 98;
			this.PageOrientationIsLandscape = false;
			this.AdaptColumns = false;
			this.PaperSizeWidth = 827;
			this.PaperSizeHeight = 1169;
			this.AdaptColumnsToPages = 1;
			this.Printer = "";
			this.HeaderText = "";
			this.HeaderAlign = "Linksbündig";
			this.FooterText = "";
			this.FooterAlign = "Linksbündig";
		}
	}
}
