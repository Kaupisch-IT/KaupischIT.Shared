using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace KaupischIT.InfragisticsControls
{
	/// <summary>
	/// Stellt ein Fenster dar, in dem Unterschiede zwischen verschiedenen Zeilen farblich hervorgehoben werden
	/// </summary>
	public partial class FormDiff : Form
	{
		/// <summary>
		/// Erstellt ein neues Fenster zur Darstellung von Unterschieden zwischen Zeilen
		/// </summary>
		/// <param name="values">die Werte, die als Zeilen verwendet werden soll</param>
		public FormDiff(IEnumerable values)
		{
			this.Font = SystemFonts.MessageBoxFont;
			this.InitializeComponent();

			this.customizedUltraGrid.DisplayLayout.MaxBandDepth = 1;
			this.customizedUltraGrid.DataSource = values;

			// Aussehen für die hervorgehobenen Zellen
			Infragistics.Win.Appearance appearance = this.customizedUltraGrid.DisplayLayout.Appearances.Add("Positive");
			appearance.BackColor = Color.Yellow;

			UltraGridBand ultraGridBand = this.customizedUltraGrid.DisplayLayout.Bands[0];
			for (int i = 0;i<this.customizedUltraGrid.Rows.Count-1;i++)
			{
				UltraGridRow previousRow = this.customizedUltraGrid.Rows[i];
				UltraGridRow currentRow = this.customizedUltraGrid.Rows[i+1];

				// Unterschiede zwischen den Zellwerten aufeinander folgender Zeilen ermitteln und ggf. hervorheben
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
