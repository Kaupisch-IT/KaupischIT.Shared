using System.Diagnostics;
using System.Xml.Serialization;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Layout-Informationen über Gruppierungen bereit
	/// </summary>
	[DebuggerDisplay("{ColumnKey}")]
	public class Grouping
	{
		/// <summary>
		/// Gibt den Schlüssel der zugehörigen Spalte zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("columnKey")]
		public string ColumnKey { get; set; }
	}
}
