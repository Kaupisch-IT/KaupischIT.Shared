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
			return assembly.GetName().GetBuildDateTime();
		}
	}
}
