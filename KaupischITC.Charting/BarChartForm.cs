using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
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
	}
}
