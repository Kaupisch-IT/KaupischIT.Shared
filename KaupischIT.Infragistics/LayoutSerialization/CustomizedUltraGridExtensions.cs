using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using KaupischIT.InfragisticsControls.ValueAppearances;

namespace KaupischIT.InfragisticsControls.LayoutSerialization
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die (Customized)UltraGrid-Klasse bereit
	/// </summary>
	public static class CustomizedUltraGridExtensions
	{
		/// <summary>
		/// Ermittelt die Layout-Informationen eines UltraGrids
		/// </summary>
		/// <param name="ultraGrid">das Grid, dessen Layout-Informationen bereitgestellt werden sollen</param>
		/// <returns>die Layout-Informationen des angegebenen UltraGrids</returns>
		public static GridLayout GetLayout(this UltraGrid ultraGrid)
		{
			return new GridLayout
			{
				// alle Band-Layouts
				Bands = ultraGrid.DisplayLayout.Bands.Cast<UltraGridBand>().Select(band => new BandLayout 
				{
					Key = band.GetKey(),
					Hidden = band.Hidden,
					Caption = band.Header.Caption,

					// alle Spalten-Layouts
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

					// alle Spalten-Zusammenfassungen
					Summaries = band.Summaries.Cast<SummarySettings>().Select(summary => new ColumnSummary // alle 
					{
						ColumnKey = summary.SourceColumn.Key,
						SummaryType = summary.SummaryType
					}).ToArray(),

					// alle Spalten-Gruppierungen
					Groups = band.SortedColumns.Cast<UltraGridColumn>().Where(col => col.IsGroupByColumn).Select(group => new Grouping
					{
						ColumnKey = group.Key
					}).ToArray(),

				}).ToArray()
			};
		}


		/// <summary>
		/// Stellt ein UltraGrid-Layout wieder her
		/// </summary>
		/// <param name="ultraGrid">das CustomizedUltraGrid, dessen Layout wieder her</param>
		/// <param name="gridLayout">das Layout, das wieder hergestellt werden soll</param>
		public static void RestoreLayout(this CustomizedUltraGrid ultraGrid,GridLayout gridLayout)
		{
			if (gridLayout.Bands!=null)
				foreach (BandLayout bandLayout in gridLayout.Bands)
				{
					// alle Band-Layouts
					UltraGridBand band = ultraGrid.FindBand(bandLayout.Key);
					if (band!=null)
					{
						band.Hidden = bandLayout.Hidden;
						band.Header.Caption = bandLayout.Caption;

						if (bandLayout.Columns!=null)
						{
							// alle Spalten-Layouts
							foreach (ColumnLayout columnLayout in bandLayout.Columns.Where(c => c.Sorting!=SortIndicator.None).OrderBy(c => c.SortIndex)) // Reihenfolge beim Setzen der SortedColumns beachten 
								if (band.Columns.Exists(columnLayout.Key))
								{
									UltraGridColumn column = band.Columns[columnLayout.Key];
									band.SortedColumns.Add(column,descending: false,groupBy: false);
									column.SortIndicator = columnLayout.Sorting;
								}
							foreach (ColumnLayout columnLayout in bandLayout.Columns.OrderBy(c => c.Position)) // Reihenfolge beim Setzen der VisiblePosition beachten
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

						// alle Spalten-Zusammenfassungen
						if (bandLayout.Summaries!=null)
							foreach (ColumnSummary summary in bandLayout.Summaries)
								if (band.Columns.Exists(summary.ColumnKey))
								{
									UltraGridColumn column = band.Columns[summary.ColumnKey];
									ultraGrid.AddColumnSummary(column,summary.SummaryType);
								}

						// alle Gruppierungen
						if (bandLayout.Groups!=null)
							foreach (Grouping grouping in bandLayout.Groups)
								if (band.Columns.Exists(grouping.ColumnKey))
									band.SortedColumns.Add(grouping.ColumnKey,false,true);
					}
				}
		}


		/// <summary>
		/// Ermittelt den Schlüssel eines Bandes
		/// </summary>
		/// <param name="ultraGridBand">das Band, dessen Schlüssel ermittelt werden soll</param>
		/// <returns>den Schlüssel des Bandes</returns>
		private static string GetKey(this UltraGridBand ultraGridBand)
		{
			// Es wird nicht die normale Band.Key-Eigenschaft verwendet, da es mehrere verschiedene Bänder mit dem gleichen Wert geben kann.
			// Daher wird der Schlüssel aus der Hierarchie des Bandes ermittelt.

			string result = "";
			while (ultraGridBand!=null)
			{
				result = (!String.IsNullOrEmpty(result)) ? ultraGridBand.Key+"."+result : ultraGridBand.Key;
				ultraGridBand = ultraGridBand.ParentBand;
			}
			return result;
		}

		/// <summary>
		/// Ermittelt ein Band anhand des übergebenen Schlüssels
		/// </summary>
		/// <param name="ultraGrid">das UltraGrid, in dem das Band gesucht werden soll</param>
		/// <param name="key">der Schlüssel des Bandes, das gefunden werden soll</param>
		/// <returns>das Band mit dem angegebenen Schlüssel; falls nicht gefunden, null</returns>
		private static UltraGridBand FindBand(this CustomizedUltraGrid ultraGrid,string key)
		{
			// Es wird nicht die normale Band.Key-Eigenschaft verwendet, da es mehrere verschiedene Bänder mit dem gleichen Wert geben kann.
			// Daher wird das Band anhand des hierarchischen Schlüssels ermittelt

			UltraGridBand result = null;
			foreach (string subKey in key.Split('.'))
				if (ultraGrid.DisplayLayout.Bands.Exists(subKey))
					result = (result==null) ? ultraGrid.DisplayLayout.Bands[subKey] : ultraGrid.DisplayLayout.Bands.Cast<UltraGridBand>().SingleOrDefault(ugb => ugb.Key==subKey && ugb.ParentBand==result);

			return result;
		}

		
		/// <summary>
		/// Speichert die Layout-Informationen des UltraGrids in ein XML-Dokument
		/// </summary>
		/// <param name="ultraGrid">das UltraGrid, dessen Layout-Informationen gespeichert werden sollen</param>
		/// <param name="xmlWriter">der zum Schreiben des XML-Dokuments verwendete XmlWriter</param>
		public static void SaveLayoutToXml(this UltraGrid ultraGrid,XmlWriter xmlWriter)
		{
			GridLayout gridLayout = ultraGrid.GetLayout();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridLayout));
			xmlSerializer.Serialize(xmlWriter,gridLayout);
		}

		/// <summary>
		/// Stellt die Layout-Informationen des UltraGrids aus einem XML-Dokument wieder her
		/// </summary>
		/// <param name="ultraGrid"></param>
		/// <param name="xmlReader">der zum Lesen des XML-Dokuments verwendete XmlReader</param>
		public static void RestoreLayoutFromXml(this CustomizedUltraGrid ultraGrid,XmlReader xmlReader)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridLayout));
			GridLayout gridLayout = (GridLayout)xmlSerializer.Deserialize(xmlReader);
			ultraGrid.RestoreLayout(gridLayout);
		}
	}
}
