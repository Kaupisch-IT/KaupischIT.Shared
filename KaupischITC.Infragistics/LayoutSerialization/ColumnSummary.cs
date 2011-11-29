using System.Diagnostics;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	[DebuggerDisplay("{ColumnKey} {SummaryType}")]
	public class ColumnSummary
	{
		[XmlAttribute("columnKey")]
		public string ColumnKey { get; set; }
		
		[XmlAttribute("type")]
		public SummaryType SummaryType { get; set; }
	}
}
