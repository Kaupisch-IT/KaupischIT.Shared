using System;
using System.Reflection;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Assembly-Klasse bereit
	/// </summary>
	public static class AssemblyExtensions
	{
		/// <summary>
		/// Ermittelt die Build-Zeit, an der die angegebene Assembly erstellt wurde (Sommerzeit wird nicht berücksichtigt)
		/// </summary>
		/// <param name="assembly">die Assembly, deren Build-Zeit ermittelt werden soll</param>
		/// <returns>Datum und Uhrzeit, wann die angegebene Assembly vom Compiler erstellt wurde (Sommerzeit wird nicht berücksichtigt)</returns>
		public static DateTime GetBuildDateTime(this Assembly assembly)
		{
			// die Build-Zeit kann aus den standardmäßigen Build- und Revisionsnummern in der Assembly-Version ermittelt werden
			Version version = assembly.GetName().Version;
			return new DateTime(2000,1,1).AddDays(version.Build) // "Build"-Wert entspricht der Anzahl der Tage seit dem 01.01.2000
				.AddSeconds(version.Revision*2);                 // "Revision"-Wert entspricht der Hälfte der Anzahl der Sekunden seit Mitternacht
		}
	}
}
