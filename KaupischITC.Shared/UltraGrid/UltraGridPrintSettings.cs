using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaupischITC.Shared
{
	public class UltraGridPrintSettings
	{
		public int MarginTop { get; set; }
		public int MarginBottom { get; set; }
		public int MarginLeft { get; set; }
		public int MarginRight { get; set; }
		public bool PageOrientationIsLandscape { get; set; }
		public bool AdaptColumns { get; set; }
		public int PaperSizeWidth { get; set; }
		public int PaperSizeHeight { get; set; }
		public int AdaptColumnsToPages { get; set; }
		public string Printer { get; set; }
		public string HeaderText { get; set; }
		public string HeaderAlign { get; set; }
		public string FooterText { get; set; }
		public string FooterAlign { get; set; }

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
