using System;
using Infragistics.Win;

namespace KaupischITC.InfragisticsControls.ValueAppearances
{
	public class ValueAppearance : IValueAppearance
	{
		private IconValueAppearance iconValueAppearance = new IconValueAppearance();
		private NegativeValueAppearance negativeValueAppearance = new NegativeValueAppearance();

		public event EventHandler PropertyChanged;

		public bool HighlightNegativeValues
		{
			get { return this.highlightNegativeValues; }
			set { this.ChangePropertyAndNotify(() => this.highlightNegativeValues = value); }
		}
		private bool highlightNegativeValues;


		public bool ShowTrendIndicators
		{
			get { return this.showTrendIndicators; }
			set { this.ChangePropertyAndNotify(() => this.showTrendIndicators = value); }
		}
		private bool showTrendIndicators;



		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			if (this.HighlightNegativeValues)
				this.negativeValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);

			if (this.ShowTrendIndicators)
				this.iconValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);
		}


		private void ChangePropertyAndNotify(Action setter)
		{
			setter();
			if (this.PropertyChanged!=null)
				this.PropertyChanged(this,EventArgs.Empty);
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
