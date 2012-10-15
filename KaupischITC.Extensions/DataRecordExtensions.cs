using System;
using System.Data;
using System.Diagnostics;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt einen Satz von static-Methoden als Erweiterung für den IDataRecord-Typen bereit
	/// </summary>
	public static class DataRecordExtensions
	{
		/// <summary>
		/// Ermittelt den Wert der angegebenen Spalte
		/// </summary>
		/// <typeparam name="T">der Rückgabetyp, in den der Wert der Spalte gecastet werden soll</typeparam>
		/// <param name="dataRecord">die Datenzeile, die die Werte enhält</param>
		/// <param name="columnName">der Name der Spalte, deren Wert ermittelt werden soll</param>
		/// <returns>der Wert der Spalte</returns>
		[DebuggerStepThrough]
		public static T GetColumnValue<T>(this IDataRecord dataRecord,string columnName)
		{
			object value = dataRecord[columnName];
			return (value is DBNull) ? default(T) : (T)value;
		}
	}
}
