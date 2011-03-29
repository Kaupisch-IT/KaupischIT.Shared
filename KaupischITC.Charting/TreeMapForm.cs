using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KaupischITC.Charting
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
	}
}
