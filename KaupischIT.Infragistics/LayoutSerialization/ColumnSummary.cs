using System.Diagnostics;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;

namespace KaupischIT.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Layout-Informationen über eine Spalten-Zusammenfassung bereit
	/// </summary>
	[DebuggerDisplay("{ColumnKey} {SummaryType}")]
	public class ColumnSummary
	{
		/// <summary>
		/// Gibt den Schlüssel der zugehörigen Spalte zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("columnKey")]
		public string ColumnKey { get; set; }

		/// <summary>
		/// Gibt den Typ der Zusammenfassung zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("type")]
		public SummaryType SummaryType { get; set; }
	}
}
