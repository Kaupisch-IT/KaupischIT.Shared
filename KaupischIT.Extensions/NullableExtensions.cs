using System;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt einen Satz von static-Methoden als Erweiterung für den Nullable-Typen bereit
	/// </summary>
	public static class NullableExtensions
	{
		/// <summary>
		/// Ermittelt, ob der übergebene Wert null oder der Standardwert des zugrundeliegende Typen ist
		/// </summary>
		/// <typeparam name="T">der zugrundeliegende Typ</typeparam>
		/// <param name="value">der Wert, der auf null oder Standardwert geprüft werden soll</param>
		/// <returns>true, falls der angebene Wert null ist oder dem Standardwert des zugrundeliegenden Typen entspricht, andernfalls false</returns>
		public static bool IsNullOrDefault<T>(this Nullable<T> value) where T : struct => !value.HasValue || Object.Equals(value.Value,default(T));
	}
}
