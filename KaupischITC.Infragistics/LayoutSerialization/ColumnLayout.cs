using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	[DebuggerDisplay("{Key}")]
	public class ColumnLayout
	{
		[XmlAttribute("key")]
		public string Key { get; set; }
		
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		
		[XmlAttribute("hidden"),DefaultValue(false)]
		public bool Hidden { get; set; }
		
		[XmlAttribute("width")]
		public int Width { get; set; }
		
		[XmlAttribute("position")]
		public int Position { get; set; }
		
		[XmlAttribute("sort"),DefaultValue(SortIndicator.None)]
		public SortIndicator Sorting { get; set; }
		
		[XmlAttribute("format")]
		public string Format { get; set; }
		
		[XmlAttribute("bold"),DefaultValue(false)]
		public bool IsBold { get; set; }
		
		[XmlAttribute("italic"),DefaultValue(false)]
		public bool IsItalic { get; set; }
		
		[XmlAttribute("underline"),DefaultValue(false)]
		public bool IsUnderlined { get; set; }
		
		[XmlAttribute("highlightNegative"),DefaultValue(false)]
		public bool HighlightNegativeValues { get; set; }
		
		[XmlAttribute("showTrend"),DefaultValue(false)]
		public bool ShowTrend { get; set; }
	}
}
