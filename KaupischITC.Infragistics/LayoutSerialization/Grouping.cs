using System.Diagnostics;
using System.Xml.Serialization;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	[DebuggerDisplay("{ColumnKey}")]
	public class Grouping
	{
		[XmlAttribute("columnKey")]
		public string ColumnKey { get; set; }
	}
}
