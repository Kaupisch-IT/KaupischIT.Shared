using System;
using Infragistics.Win;

namespace KaupischIT.InfragisticsControls.ValueAppearances
{
	/// <summary>
	/// Stellt ein wertebasierendes Aussehen bereit, dass mehrere andere wertebasierendes Aussehen kombiniert
	/// </summary>
	public class ValueAppearance : IValueAppearance
	{
		// kombinierte ValueAppearances
		private readonly IconValueAppearance iconValueAppearance = new IconValueAppearance();
		private readonly NegativeValueAppearance negativeValueAppearance = new NegativeValueAppearance();

		/// <summary>
		/// Wird ausgelöst, wenn eine Eigenschaft geändert wurde
		/// </summary>
		public event EventHandler PropertyChanged;


		/// <summary>
		/// Gibt an, ob negative Werte hervorgehoben werden, oder legt dies fest
		/// </summary>
		public bool HighlightNegativeValues
		{
			get => this.highlightNegativeValues;
			set => this.ChangePropertyAndNotify(() => this.highlightNegativeValues = value);
		}
		private bool highlightNegativeValues;

		/// <summary>
		/// Gibt an, ob Tendenzpfeile angezeigt werden, oder legt dies fest
		/// </summary>
		public bool ShowTrendIndicators
		{
			get => this.showTrendIndicators;
			set => this.ChangePropertyAndNotify(() => this.showTrendIndicators = value);
		}
		private bool showTrendIndicators;


		/// <summary>
		/// Ändert das Aussehen wertebasierend
		/// </summary>
		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			if (this.HighlightNegativeValues)
				this.negativeValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);

			if (this.ShowTrendIndicators)
				this.iconValueAppearance.ResolveAppearance(ref appData,ref flags,dataValue,context);
		}


		/// <summary>
		/// Setzt die angegebene Eigenschaft und löst das PropertyChanged-Ereignis aus
		/// </summary>
		/// <param name="setter"></param>
		private void ChangePropertyAndNotify(Action setter)
		{
			setter();
			this.PropertyChanged?.Invoke(this,EventArgs.Empty);
		}


		/// <summary>
		/// Erzeugt eine Kopie des Objektes
		/// </summary>
		public object Clone() => this.MemberwiseClone();
	}
}
