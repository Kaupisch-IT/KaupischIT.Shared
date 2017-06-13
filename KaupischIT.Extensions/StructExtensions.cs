using System;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für Werttypen bereit
	/// </summary>
	public static class StructExtensions
	{
		/// <summary>
		/// Wandelt den angegebenen Werttypen in ein Nullable um 
		/// </summary>
		/// <typeparam name="T">der zugrunde liegende Werttyp</typeparam>
		/// <param name="value">der Wert</param>
		/// <returns>den übergebenen Typen als Nullable</returns>
		public static Nullable<T> AsNullable<T>(T value) where T : struct
		{
			return new Nullable<T>(value);
		}
	}
}
