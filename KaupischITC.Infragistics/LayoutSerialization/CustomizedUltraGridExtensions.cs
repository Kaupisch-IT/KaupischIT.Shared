using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischITC.InfragisticsControls.ValueAppearances;
using System;

namespace KaupischITC.InfragisticsControls.LayoutSerialization
{
	public static class CustomizedUltraGridExtensions
	{
		public static GridLayout GetLayout(this UltraGrid ultraGrid)
		{
			return new GridLayout
			{
				Bands = ultraGrid.DisplayLayout.Bands.Cast<UltraGridBand>().Select(band => new BandLayout
				{
					Key = band.GetKey(),
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
						Sorting = column.SortIndicator,
						SortIndex = band.SortedColumns.IndexOf(column),

					}).ToArray(),

					Summaries = band.Summaries.Cast<SummarySettings>().Select(summary => new ColumnSummary
					{
						ColumnKey = summary.SourceColumn.Key,
						SummaryType = summary.SummaryType
					}).ToArray(),

					Groups = band.SortedColumns.Cast<UltraGridColumn>().Where(col => col.IsGroupByColumn).Select(group => new Grouping
					{
						ColumnKey = group.Key
					}).ToArray(),

				}).ToArray()
			};
		}


		public static void RestoreLayout(this CustomizedUltraGrid ultraGrid,GridLayout gridLayout)
		{
			if (gridLayout.Bands!=null)
				foreach (BandLayout bandLayout in gridLayout.Bands)
				{
					UltraGridBand band = ultraGrid.FindBand(bandLayout.Key);
					if (band!=null)
					{
						band.Hidden = bandLayout.Hidden;
						band.Header.Caption = bandLayout.Caption;

						if (bandLayout.Columns!=null)
						{
							foreach (ColumnLayout columnLayout in bandLayout.Columns.Where(c => c.Sorting!=SortIndicator.None).OrderBy(c => c.SortIndex))
								if (band.Columns.Exists(columnLayout.Key))
								{
									UltraGridColumn column = band.Columns[columnLayout.Key];
									band.SortedColumns.Add(column,descending: false,groupBy: false);
									column.SortIndicator = columnLayout.Sorting;
								}

							foreach (ColumnLayout columnLayout in bandLayout.Columns.OrderBy(c => c.Position))
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
						}

						if (bandLayout.Summaries!=null)
							foreach (ColumnSummary summary in bandLayout.Summaries)
								if (band.Columns.Exists(summary.ColumnKey))
								{
									UltraGridColumn column = band.Columns[summary.ColumnKey];
									ultraGrid.AddColumnSummary(column,summary.SummaryType);
								}

						if (bandLayout.Groups!=null)
							foreach (Grouping grouping in bandLayout.Groups)
								if (band.Columns.Exists(grouping.ColumnKey))
									band.SortedColumns.Add(grouping.ColumnKey,false,true);
					}
				}
		}

		private static string GetKey(this UltraGridBand ultraGridBand)
		{
			string result = "";
			while (ultraGridBand!=null)
			{
				result = (!String.IsNullOrEmpty(result)) ? ultraGridBand.Key+"."+result : ultraGridBand.Key;
				ultraGridBand = ultraGridBand.ParentBand;
			}
			return result;
		}

		private static UltraGridBand FindBand(this CustomizedUltraGrid ultraGrid,string key)
		{
			UltraGridBand result = null;
			foreach (string subKey in key.Split('.'))
				if (ultraGrid.DisplayLayout.Bands.Exists(subKey))
					result = (result==null) ? ultraGrid.DisplayLayout.Bands[subKey] : ultraGrid.DisplayLayout.Bands.Cast<UltraGridBand>().SingleOrDefault(ugb => ugb.Key==subKey && ugb.ParentBand==result);

			return result;
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
