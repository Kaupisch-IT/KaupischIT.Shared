using System;
using System.Drawing;
using Infragistics.Win;

namespace KaupischITC.InfragisticsControls.ValueAppearances
{
	/// <summary>
	/// Stellt ein wertebasierendes Aussehen bereit, dass negative Werte rot anzeigt
	/// </summary>
	public class NegativeValueAppearance : IValueAppearance
	{
		// Aussehen für negative Werte
		private static readonly Appearance negativeAppearance = new Appearance() { ForeColor = Color.Red };

		/// <summary>
		/// Wird ausgelöst, wenn sich eine Eigenschaft ändert
		/// </summary>
		public event EventHandler PropertyChanged { add { } remove { } }


		/// <summary>
		/// Ändert das Aussehen wertebasierend
		/// </summary>
		public void ResolveAppearance(ref AppearanceData appData,ref AppearancePropFlags flags,object dataValue,IConditionContextProvider context)
		{
			double value;
			if (Double.TryParse(Convert.ToString(dataValue),out value))
				if (value<0)
					NegativeValueAppearance.negativeAppearance.MergeData(ref appData,ref flags);
		}


		/// <summary>
		/// Erzeugt eine Kopie des Objektes
		/// </summary>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
