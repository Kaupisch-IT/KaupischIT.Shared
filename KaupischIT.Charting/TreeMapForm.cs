using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischIT.Charting
{
	/// <summary>
	/// Stellt ein Flächendiagramm/eine TreeMap dar
	/// </summary>
	public class TreeMapForm : ChartForm
	{
		/// <summary>
		/// Erstellt ein neues Fenster zum Darstellen eines Kreisdiagramms
		/// </summary>
		public TreeMapForm()
		{
			this.Text = "Flächendiagramm";
			this.SortItems = true;
			this.SupportsNegativeValues = false;
		}


		/// <summary>
		/// Zeichnet das Flächendiagramm
		/// </summary>
		/// <param name="items">die Elemente, die gezeichnet werden sollen-</param>
		/// <returns>ein Bild, das das gezeichnete Diagramm enthält</returns>
		protected override Bitmap DrawChart(IEnumerable<ChartItem> slices)
		{
			List<TreeMap.Item> items = slices.Select((item,index) => new TreeMap.Item
			{
				DisplayText = item.DisplayText,
				ValueText = item.ValueText,
				Value = item.Percent,
				Color = ChartForm.GenerateColor(index,slices.Count())
			}).ToList();

			TreeMap paneChart = new TreeMap();
			return paneChart.Draw(items);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// TreeMapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.ClientSize = new System.Drawing.Size(449, 319);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TreeMapForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
