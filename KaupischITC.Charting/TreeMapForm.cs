using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
{
	public class TreeMapForm : ChartForm
	{
		public TreeMapForm()
		{
			this.Text = "Flächendiagramm";
			this.SortItems = true;
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

			TreeMap paneChart = new TreeMap();
			return paneChart.Draw(bars);
		}
	}
}
