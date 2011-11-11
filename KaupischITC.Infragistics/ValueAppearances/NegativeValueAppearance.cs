using System;
using System.Drawing;
using Infragistics.Win;

namespace KaupischITC.InfragisticsControls.ValueAppearances
{
	public class NegativeValueAppearance : IValueAppearance
	{
		private static readonly Appearance negativeAppearance = new Appearance() { ForeColor = Color.Red };

		public event EventHandler PropertyChanged { add { } remove { } }

		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			double value;
			if (Double.TryParse(Convert.ToString(dataValue),out value))
				if (value<0)
					NegativeValueAppearance.negativeAppearance.MergeData(ref appData,ref flags);
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
