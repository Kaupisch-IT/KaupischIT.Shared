using System.Xml.Serialization;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	[XmlRoot("grid")]
	public class GridLayout
	{
		[XmlArray("bands"),XmlArrayItem("band")]
		public BandLayout[] Bands { get; set; }
	}
}
