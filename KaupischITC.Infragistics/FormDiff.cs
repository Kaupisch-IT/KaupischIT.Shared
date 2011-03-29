using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls
{
	public partial class FormDiff : Form
	{
		public FormDiff(IEnumerable values)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.customizedUltraGrid.DisplayLayout.MaxBandDepth = 1;
			this.customizedUltraGrid.DataSource = values;

			Infragistics.Win.Appearance appearance = this.customizedUltraGrid.DisplayLayout.Appearances.Add("Positive");
			appearance.BackColor = Color.Yellow;

			UltraGridBand ultraGridBand = this.customizedUltraGrid.DisplayLayout.Bands[0];
			for (int i=0;i<this.customizedUltraGrid.Rows.Count-1;i++)
			{
				UltraGridRow previousRow = this.customizedUltraGrid.Rows[i];
				UltraGridRow currentRow = this.customizedUltraGrid.Rows[i+1];

				foreach (UltraGridColumn ultraGridColumn in ultraGridBand.Columns)
				{
					object previousCellValue = previousRow.GetCellValue(ultraGridColumn);
					object currentCellValue = currentRow.GetCellValue(ultraGridColumn);
					if (!Object.Equals(currentCellValue,previousCellValue))
						currentRow.Cells[ultraGridColumn].Appearance = appearance;
				}
			}
		}
	}
}
