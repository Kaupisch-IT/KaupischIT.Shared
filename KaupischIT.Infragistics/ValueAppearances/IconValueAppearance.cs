using System;
using Infragistics.Win;

namespace KaupischIT.InfragisticsControls.ValueAppearances
{
	/// <summary>
	/// Stellt ein wertebasierendes Aussehen bereit, dass Tendenzpfeile anzeigt
	/// </summary>
	public class IconValueAppearance : IValueAppearance
	{
		// verschiedene Aussehen für negative, positive und neutrale Werte
		private static readonly Appearance negativeAppearance = new Appearance() { Image = "down" };
		private static readonly Appearance positiveAppearance = new Appearance() { Image = "up" };
		private static readonly Appearance neutralAppearance = new Appearance() { Image = "right" };

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


		/// <summary>
		/// Erzeugt eine Kopie des Objektes
		/// </summary>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
