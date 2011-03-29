using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
{
	public class BarChartForm : ChartForm
	{
		public BarChartForm()
		{
			this.Text = "Balkendiagramm";
			this.SortItems = false;
		}

		protected override Bitmap DrawChart(IEnumerable<ChartItem> slices)
		{
			List<BarChart.Bar> bars = slices.Select((item,index) => new BarChart.Bar
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
