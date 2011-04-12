using System;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt einen Satz von static-Methoden als Erweiterung für den string-Typen bereit
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Versucht, die angegebene Zeichenkette in den angegeben Typen zu konvertieren
		/// </summary>
		/// <typeparam name="T">der Typ, in den die Zeichenkette konvertiert werden soll</typeparam>
		/// <param name="value">die Zeichenkette, die konvertiert werden soll</param>
		/// <returns>ein Exemplar des Zieltypen, falls die Zeichenkette konvertiert werden konnte, andernfalls den Standardwert des Zieltypen</returns>
		public static Nullable<T> AsNullable<T>(this string value) where T : struct
		{
			if (typeof(T)==typeof(int))
			{
				int result;
				return (int.TryParse(value,out result)) ? (T)(object)result : (Nullable<T>)null;
			}
			else
				try { return (Nullable<T>)Convert.ChangeType(value,typeof(T)); }
				catch { return null; }
		}



		/// <summary>
		/// Gibt null zurück, falls die Zeichenkette null oder leer ist
		/// </summary>
		/// <param name="value">die Zeichenkette</param>
		/// <returns>null, falls die Zeichenkette null oder leer ist, andernfalls die Zeichenkette selbst</returns>
		public static string AsNullIfEmpty(this string value)
		{
			return (String.IsNullOrEmpty(value)) ? null : value;
		}
	}
}
