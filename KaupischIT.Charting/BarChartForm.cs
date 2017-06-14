using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischIT.Charting
{
	/// <summary>
	/// Stellt ein Balkendiagramm dar
	/// </summary>
	public class BarChartForm : ChartForm
	{
		/// <summary>
		/// Erstellt ein neues Fenster zum Darstellen eines Kreisdiagramms
		/// </summary>
		public BarChartForm()
		{
			this.Text = "Balkendiagramm";
			this.SortItems = false;
			this.SupportsNegativeValues = true;
		}


		/// <summary>
		/// Zeichnet das Balkendiagramm
		/// </summary>
		/// <param name="items">die Elemente, die gezeichnet werden sollen-</param>
		/// <returns>ein Bild, das das gezeichnete Diagramm enthält</returns>
		protected override Bitmap DrawChart(IEnumerable<ChartItem> slices)
		{
			List<BarChart.Item> bars = slices.Select((item,index) => new BarChart.Item
			{
				DisplayText = item.DisplayText,
				ValueText = item.ValueText,
				Value = item.Percent,
				Color = ChartForm.GenerateColor(index,slices.Count())
			}).ToList();

			BarChart barChart = new BarChart();
			return barChart.Draw(bars);
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// BarChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.ClientSize = new System.Drawing.Size(449, 319);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BarChartForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
