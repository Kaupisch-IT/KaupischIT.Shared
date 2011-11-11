using System;
using Infragistics.Win;

namespace KaupischITC.InfragisticsControls.ValueAppearances
{
	public class IconValueAppearance : IValueAppearance
	{
		private static readonly Appearance negativeAppearance = new Appearance() { Image = "down" };
		private static readonly Appearance positiveAppearance = new Appearance() { Image = "up" };
		private static readonly Appearance neutralAppearance = new Appearance() { Image = "right" };

		public event EventHandler PropertyChanged { add { } remove { } }

		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			double value;
			if (Double.TryParse(Convert.ToString(dataValue),out value))
			{
				double threshold = 0.05;

				if (value < -threshold)
					IconValueAppearance.negativeAppearance.MergeData(ref appData,ref flags);
				else if (value > threshold)
					IconValueAppearance.positiveAppearance.MergeData(ref appData,ref flags);
				else
					IconValueAppearance.neutralAppearance.MergeData(ref appData,ref flags);
			}
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
