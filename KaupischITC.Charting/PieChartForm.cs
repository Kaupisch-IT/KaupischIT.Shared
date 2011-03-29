﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt ein Kreisdiagramm dar
	/// </summary>
	public class PieChartForm : ChartForm
	{
		/// <summary>
		/// Erstellt ein neues Fenster zum Darstellen eines Kreisdiagramms
		/// </summary>
		public PieChartForm()
		{
			this.Text = "Kreisdiagramm";
			this.SortItems = true;
			this.SupportsNegativeValues = false;
		}


		/// <summary>
		/// Zeichnet das Kreisdiagramm
		/// </summary>
		/// <param name="items">die Elemente, die gezeichnet werden sollen-</param>
		/// <returns>ein Bild, das das gezeichnete Diagramm enthält</returns>
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
			List<PieChart.Slice> pieSlices = slices.Select((item,index) => new PieChart.Slice
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
