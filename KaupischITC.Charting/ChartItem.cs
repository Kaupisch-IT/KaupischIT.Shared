
namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt Informationen über eine Diagrammelement bereit.
	/// </summary>
	public class ChartItem
	{
		/// <summary> Gibt den Beschriftungstext zurück oder legt diesen fest. </summary>
		public string DisplayText { get; set; }

		/// <summary> Gibt den Text des Wertes zurück oder legt diesen fest. </summary>
		public string ValueText { get; set; }

		/// <summary> Gibt den prozentualen Anteil des Wertes am gesamten Wertebereich zurück oder legt diesen fest. </summary>
		public double Percent { get; set; }

		/// <summary> Gibt an, ob das Diagrammelement hervorgehoben werden soll. </summary>
		public bool IsEmphasized { get; set; }
	}
}
