using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischITC.InfragisticsControls.ValueAppearances;

namespace KaupischITC.InfragisticsControls
{
	[XmlRoot("grid")]
	public class GridLayout
	{
		[XmlArray("bands"),XmlArrayItem("band")]
		public BandLayout[] Bands { get; set; }
	}

	[DebuggerDisplay("{Key}")]
	public class BandLayout
	{
		[XmlAttribute("key")]
		public string Key { get; set; }
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		[XmlAttribute("hidden")]
		public bool Hidden { get; set; }
		[XmlArray("columns"),XmlArrayItem("column")]
		public ColumnLayout[] Columns { get; set; }
		[XmlArray("summaries"),XmlArrayItem("summary")]
		public ColumnSummary[] Summaries { get; set; }
	}

	[DebuggerDisplay("{Key}")]
	public class ColumnLayout
	{
		[XmlAttribute("key")]
		public string Key { get; set; }
		[XmlAttribute("caption")]
		public string Caption { get; set; }
		[XmlAttribute("hidden")]
		public bool Hidden { get; set; }
		[XmlAttribute("width")]
		public int Width { get; set; }
		[XmlAttribute("position")]
		public int Position { get; set; }
		[XmlAttribute("format")]
		public string Format { get; set; }
		[XmlAttribute("bold")]
		public bool IsBold { get; set; }
		[XmlAttribute("italic")]
		public bool IsItalic { get; set; }
		[XmlAttribute("underline")]
		public bool IsUnderlined { get; set; }
		[XmlAttribute("highlightNegative")]
		public bool HighlightNegativeValues { get; set; }
		[XmlAttribute("showTrend")]
		public bool ShowTrend { get; set; }
	}

	public class ColumnSummary
	{
		[XmlAttribute("columnKey")]
		public string ColumnKey { get; set; }
		[XmlAttribute("type")]
		public SummaryType SummaryType { get; set; }
	}


	public static class CustomizedUltraGridExtensions
	{
		public static GridLayout GetLayout(this UltraGrid ultraGrid)
		{
			return new GridLayout
			{
				Bands = ultraGrid.DisplayLayout.Bands.Cast<UltraGridBand>().Select(band => new BandLayout
				{
					Key = band.Key,
					Hidden = band.Hidden,
					Caption = band.Header.Caption,

					Columns = band.Columns.Cast<UltraGridColumn>().Select(column => new ColumnLayout
					{
						Key = column.Key,
						Caption = column.Header.Caption,
						Hidden = column.Hidden,
						Width = column.Width,
						Position = column.Header.VisiblePosition,
						Format = column.Format,
						IsBold = column.CellAppearance.FontData.Bold==DefaultableBoolean.True,
						IsItalic = column.CellAppearance.FontData.Italic==DefaultableBoolean.True,
						IsUnderlined = column.CellAppearance.FontData.Underline==DefaultableBoolean.True,
						HighlightNegativeValues = ((ValueAppearance)column.ValueBasedAppearance).HighlightNegativeValues,
						ShowTrend = ((ValueAppearance)column.ValueBasedAppearance).ShowTrendIndicators,

					}).ToArray(),

					Summaries = band.Summaries.Cast<SummarySettings>().Select(summary => new ColumnSummary
					{
						ColumnKey = summary.SourceColumn.Key,
						SummaryType = summary.SummaryType
					}).ToArray()

				}).ToArray()
			};
		}

		public static void RestoreLayout(this CustomizedUltraGrid ultraGrid,GridLayout gridLayout)
		{
			if (gridLayout.Bands!=null)
				foreach (BandLayout bandLayout in gridLayout.Bands)
					if (ultraGrid.DisplayLayout.Bands.Exists(bandLayout.Key))
					{
						UltraGridBand band = ultraGrid.DisplayLayout.Bands[bandLayout.Key];
						band.Hidden = bandLayout.Hidden;
						band.Header.Caption = bandLayout.Caption;

						if (bandLayout.Columns!=null)
							foreach (ColumnLayout columnLayout in bandLayout.Columns)
								if (band.Columns.Exists(columnLayout.Key))
								{
									UltraGridColumn column = band.Columns[columnLayout.Key];

									column.Header.VisiblePosition = columnLayout.Position;
									column.Header.Caption = columnLayout.Caption;
									ultraGrid.SetColumnFormat(column,columnLayout.Format);
									column.Hidden = columnLayout.Hidden;
									column.Width = columnLayout.Width;

									column.CellAppearance.FontData.Bold = (columnLayout.IsBold) ? DefaultableBoolean.True : DefaultableBoolean.False;
									column.CellAppearance.FontData.Italic = (columnLayout.IsItalic) ? DefaultableBoolean.True : DefaultableBoolean.False;
									column.CellAppearance.FontData.Underline = (columnLayout.IsUnderlined) ? DefaultableBoolean.True : DefaultableBoolean.False;

									ValueAppearance valueAppearance = (ValueAppearance)column.ValueBasedAppearance;
									valueAppearance.HighlightNegativeValues = columnLayout.HighlightNegativeValues;
									valueAppearance.ShowTrendIndicators = columnLayout.ShowTrend;
								}

						if (bandLayout.Summaries!=null)
							foreach (ColumnSummary summary in bandLayout.Summaries)
								if (band.Columns.Exists(summary.ColumnKey))
								{
									UltraGridColumn column = band.Columns[summary.ColumnKey];
									ultraGrid.AddColumnSummary(column,summary.SummaryType);
								}
					}
		}


		public static void SaveLayoutToXml(this UltraGrid ultraGrid,XmlWriter xmlWriter)
		{
			GridLayout gridLayout = ultraGrid.GetLayout();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridLayout));
			xmlSerializer.Serialize(xmlWriter,gridLayout);
		}

		public static void RestoreLayoutFromXml(this CustomizedUltraGrid ultraGrid,XmlReader xmlReader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridLayout));
			GridLayout gridLayout = (GridLayout)xmlSerializer.Deserialize(xmlReader);
			ultraGrid.RestoreLayout(gridLayout);
		}
	}
}
