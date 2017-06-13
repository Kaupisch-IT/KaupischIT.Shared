using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Informationen über das Layout ein Band bereit
	/// </summary>
	[DebuggerDisplay("{Key}")]
	public class BandLayout
	{
		/// <summary>
		/// Gibt den Schlüssel des Bands zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("key")]
		public string Key { get; set; }
		
		/// <summary>
		/// Gibt die Beschriftung des Bands zurück oder legt diese fest
		/// </summary>
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		
		/// <summary>
		/// Gibt an, ob das Band sichtbar ist, oder legt dies fest
		/// </summary>
		[XmlAttribute("hidden"),DefaultValue(false)]
		public bool Hidden { get; set; }


		/// <summary>
		/// Gibt die Layout-Informationen über die Spalten des Bandes zurück oder legt diese fest
		/// </summary>
		[XmlArray("columns"),XmlArrayItem("column")]
		public ColumnLayout[] Columns { get; set; }
		
		/// <summary>
		/// Gibt die Layout-Informationen über die Zusammenfassungen der Spalten des Bandes zurück oder legt diese fest
		/// </summary>
		[XmlArray("summaries"),XmlArrayItem("summary")]
		public ColumnSummary[] Summaries { get; set; }
		
		/// <summary>
		/// Gibt die Layout-Informationen über die Gruppierungen des Bandes zurück oder legt diese fest
		/// </summary>
		[XmlArray("groups"),XmlArrayItem("group")]
		public Grouping[] Groups { get; set; }
	}
}
