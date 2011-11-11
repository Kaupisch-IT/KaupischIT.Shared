using System;
using Infragistics.Win;

namespace KaupischITC.InfragisticsControls.ValueAppearances
{
	public class ValueAppearance : IValueAppearance
	{
		private IconValueAppearance iconValueAppearance = new IconValueAppearance();
		private NegativeValueAppearance negativeValueAppearance = new NegativeValueAppearance();

		public event EventHandler PropertyChanged { add { } remove { } }

		public bool HighlightNegativeValues { get; set; }
		public bool ShowTrendIndicators { get; set; }


		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			if (this.HighlightNegativeValues)
				this.negativeValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);

			if (this.ShowTrendIndicators)
				this.iconValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
