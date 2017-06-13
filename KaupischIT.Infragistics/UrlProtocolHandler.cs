using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinGrid;
using KaupischIT.InfragisticsControls.Properties;
using KaupischIT.Shared;

namespace KaupischIT.InfragisticsControls
{
	/// <summary>
	/// Stellt Informationen über eine URL-Route bereit
	/// </summary>
	[DebuggerDisplay("{Name} {Pattern}")]
	public class Route
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("pattern")]
		public string Pattern { get; set; }
	}

	
	/// <summary>
	/// Stellt Funktionen zum Ermitteln und Verarbeiten von URL-Routen bereit
	/// </summary>
	internal class UrlProtocolHandler
	{
		/// <summary>
		/// Gibt eine Auflistung der verfügbaren URL-Routen zurück oder legt diese fest.
		/// </summary>
		public List<Route> Routes { get; set; }		

		/// <summary>
		/// Stellt Informationen über eine konkrete Route bereit
		/// </summary>
		public class ConcreteRoute : Route
		{
			/// <summary> Gibt die konkrete URL zurück oder legt diese fest </summary>
			public string Url { get; set; }

			/// <summary> Ruft die konkrete URL auf </summary>
			public void Invoke()
			{
				try { Process.Start(this.Url); }
				catch (Exception ex) { ErrorMessageBox.Show(ex); }
			}
		}


		/// <summary>
		/// Erstellt eine neue UrlProtocolHandler-Instanz
		/// </summary>
		public UrlProtocolHandler()
		{
			this.Routes = Settings.Default.UrlRoutes.ToList();
		}


		/// <summary>
		/// Gibt eine Auflistung aller konkreten URL-Routen zurück, deren Ausführung im Kontext der übergebenen UltraGridCell möglich sind
		/// </summary>
		/// <param name="ultraGridCell">die Zelle, in deren Kontext konkrete URL-Routen ermittelt werden sollen</param>
		/// <returns>eine Auflistung aller gefundenen konkreten URL-Routen zurück</returns>
		public IEnumerable<ConcreteRoute> GetValidRoutes(UltraGridRow ultraGridRow)
		{
			Func<string,string,bool> equals = (patternPart,name) =>
			{
				string pattern = Regex.Escape(patternPart).Replace(Regex.Escape("*"),".*?");
				return Regex.IsMatch(name,"^"+pattern+"$",RegexOptions.IgnoreCase);
			};

			if (ultraGridRow!=null && ultraGridRow.Cells!=null)
				foreach (Route route in this.Routes)
				{
					bool success = true;
					{
						// Versuchen, die Platzhalter im Pattern durch die Werte aus Spalten entsprechendem Key oder Anzeigenamen zu ersetzen
						ConcreteRoute result = new ConcreteRoute { Name = route.Name,Pattern = route.Pattern,Url = route.Pattern };

						foreach (Match match in Regex.Matches(route.Pattern,@"\{(?<part>[^}]+)\}"))
						{
							string[] values = match.Groups["part"].Value.Split(new[] { '|' },StringSplitOptions.RemoveEmptyEntries);
							UltraGridCell matchedCell = ultraGridRow.Cells.Cast<UltraGridCell>().FirstOrDefault(cell => cell.Value!=null && (values.Any(val => equals(val,cell.Column.Key)) || values.Any(val => equals(val,cell.Column.Header.Caption))));

							if (matchedCell!=null)
								result.Url = result.Url.Replace(match.Value,matchedCell.Value.ToString());
							else
							{
								success = false;
								break;
							}
						}

						if (success)
							yield return result;
					}
				}
		}
	}
}
