using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
{
	public class PieChartForm : ChartForm
	{
		public PieChartForm()
		{
			this.Text = "Kreisdiagramm";
			this.SortItems = true;
		}

		protected override Bitmap DrawChart(IEnumerable<ChartItem> slices)
		{
			PieChart pieChart = new PieChart
			{
				EllipseWidth = 300,
				EllipseHeight = 150,
				PieHeight = 15
			};

			// Kuchenstückobjekte erzeugen
			float startAngle = 20;
			List<PieChart.PieSlice> pieSlices = slices.Select((item,index) => new PieChart.PieSlice
			{
				Offset = (item.IsEmphasized) ? 15 : 3,
				Text = item.DisplayText+"|"+item.ValueText,
				StartAngle = startAngle,
				EndAngle = startAngle = (float)(startAngle+(item.Percent/100)*360)%360,
				Color = ChartForm.GenerateColor(index,slices.Count())
			}).ToList();

			// Kreisdiagramm zeichnen
			return pieChart.Draw(pieSlices);
		}
	}
}
