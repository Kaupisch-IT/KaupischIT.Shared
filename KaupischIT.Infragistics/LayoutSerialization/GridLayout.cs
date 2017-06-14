using System.Xml.Serialization;

namespace KaupischIT.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Layout-Information eines UltraGrids bereit
	/// </summary>
	[XmlRoot("grid")]
	public class GridLayout
	{
		/// <summary>
		/// Gibt die Layout-Informationen der Bänder des UltraGrids zurück oder legt diese fest
		/// </summary>
		[XmlArray("bands"), XmlArrayItem("band")]
		public BandLayout[] Bands { get; set; }
	}
}
