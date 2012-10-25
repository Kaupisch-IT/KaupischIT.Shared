using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Layout-Informationen einer Spalte bereit
	/// </summary>
	[DebuggerDisplay("{Key}")]
	public class ColumnLayout
	{
		/// <summary>
		/// Gibt den Schlüssel der Spalte zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("key")]
		public string Key { get; set; }
		
		/// <summary>
		/// Gibt die Beschriftung der Spalte zurück oder legt diese fest
		/// </summary>
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		
		/// <summary>
		/// Gibt an, ob die Spalte sichtbar ist, oder legt dies fest
		/// </summary>
		[XmlAttribute("hidden"),DefaultValue(false)]
		public bool Hidden { get; set; }
		
		/// <summary>
		/// Gibt die Breite der Spalte zurück oder legt diese fest
		/// </summary>
		[XmlAttribute("width")]
		public int Width { get; set; }

		/// <summary>
		/// Gibt die Position der Spalte zurück oder legt diese fest
		/// </summary>
		[XmlAttribute("position")]
		public int Position { get; set; }
		
		/// <summary>
		/// Gibt die Sortierung der Spalte zurück oder legt diese fest
		/// </summary>
		[XmlAttribute("sort"),DefaultValue(SortIndicator.None)]
		public SortIndicator Sorting { get; set; }

		/// <summary>
		/// Gibt den Sortierungs-Index der Spalte zurück oder legt diesen fest
		/// </summary>
		[XmlAttribute("sortIndex"),DefaultValue(-1)]
		public int SortIndex { get; set; }
		
		/// <summary>
		/// Gibt das Format der Spaltenwerte zurück oder legt dieses fest
		/// </summary>
		[XmlAttribute("format")]
		public string Format { get; set; }
		
		/// <summary>
		/// Gibt an, ob eine Spalte fett formatiert wird, oder legt dies fest
		/// </summary>
		[XmlAttribute("bold"),DefaultValue(false)]
		public bool IsBold { get; set; }
		
		/// <summary>
		/// Gibt an, ob eine Spalte kursiv formatiert wird, oder legt dies fest
		/// </summary>
		[XmlAttribute("italic"),DefaultValue(false)]
		public bool IsItalic { get; set; }
		
		/// <summary>
		/// Gibt an, ob eine Spalte unterstrichen formatiert wird, oder legt dies fest
		/// </summary>
		[XmlAttribute("underline"),DefaultValue(false)]
		public bool IsUnderlined { get; set; }
		
		/// <summary>
		/// Gibt an, ob negative Werte in der Spalte hervorgehoben werden, oder legt dies fest
		/// </summary>
		[XmlAttribute("highlightNegative"),DefaultValue(false)]
		public bool HighlightNegativeValues { get; set; }
		
		/// <summary>
		/// Gibt an, ob für Werte in dieser Spalte Tendenzpfeile angezeigt werden, oder legt dies fest
		/// </summary>
		[XmlAttribute("showTrend"),DefaultValue(false)]
		public bool ShowTrend { get; set; }
	}
}
