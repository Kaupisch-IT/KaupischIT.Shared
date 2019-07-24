using System;
using System.Collections.Generic;
using System.Linq;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für generische Typen bereit
	/// </summary>
	public static class ObjectGenericExtensions
	{
		public static TResult IfNotNull<T, TResult>(this T value,Func<T,TResult> func) => (value!=null) ? func(value) : default;


		/// <summary>
		/// Reduziert eine hierarchische Struktur in eine flache Struktur
		/// </summary>
		/// <typeparam name="T">der Typ der Elemente der Sequenz</typeparam>
		/// <param name="value">die hierarchische Struktur, die in eine Flache Struktur umgewandelt werden soll</param>
		/// <param name="childSelector">eine Funktion zum Ermitteln der untergeordneten Elemente</param>
		/// <returns>die in eine flache Struktur umgewandelten Elemente</returns>
		public static IEnumerable<T> Flatten<T>(this T value,Func<T,IEnumerable<T>> childSelector)
		{
			yield return value;
			foreach (T flattendChild in childSelector(value).SelectMany(child => child.Flatten(childSelector)))
				yield return flattendChild;
		}
	}
}
