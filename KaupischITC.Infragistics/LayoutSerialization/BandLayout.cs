using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	[DebuggerDisplay("{Key}")]
	public class BandLayout
	{
		[XmlAttribute("key")]
		public string Key { get; set; }
		
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		
		[XmlAttribute("hidden"),DefaultValue(false)]
		public bool Hidden { get; set; }


		[XmlArray("columns"),XmlArrayItem("column")]
		public ColumnLayout[] Columns { get; set; }
		
		[XmlArray("summaries"),XmlArrayItem("summary")]
		public ColumnSummary[] Summaries { get; set; }
		
		[XmlArray("groups"),XmlArrayItem("group")]
		public Grouping[] Groups { get; set; }
	}
}
