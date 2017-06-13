using System;
using System.Diagnostics;
using System.Globalization;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Object-Klasse bereit
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Konvertiert ein Objekt in den angegebenen Zieltypen
		/// </summary>
		/// <typeparam name="T">der Typ, in den das übergebene Objekt konvertiert werden soll</typeparam>
		/// <param name="value">das Objekt, das konvertiert werden soll</param>
		/// <param name="formatProvider">ein FormatProvider-Schnittstelle, die kulturabhängige Formatierungsinformationen liefert</param>
		/// <returns>das in den Zieltypen konvertierte Objekt</returns>
		public static T ConvertTo<T>(this object value,IFormatProvider formatProvider)
		{
			return (T)ObjectExtensions.ConvertTo(value,typeof(T),formatProvider);
		}


		/// <summary>
		/// Konvertiert ein Objekt in den angegebenen Zieltypen
		/// </summary>
		/// <param name="value">das Objekt, das konvertiert werden soll</param>
		/// <param name="targetType">der Typ, in den das übergebene Objekt konvertiert werden soll</param>
		/// <param name="formatProvider">ein FormatProvider-Schnittstelle, die kulturabhängige Formatierungsinformationen liefert</param>
		/// <returns>das in den Zieltypen konvertierte Objekt</returns>
		public static object ConvertTo(this object value,Type targetType,IFormatProvider formatProvider)
		{
			if (targetType.IsNullable())
			{
				if (value==null || (value is string && String.IsNullOrEmpty((string)value)))
					return null;
				else
					targetType = Nullable.GetUnderlyingType(targetType);
			}

			if (targetType.IsEnum)
				return Enum.Parse(targetType,value.ToString());
			
			return Convert.ChangeType(value,targetType,formatProvider);
		}


		/// <summary>
		/// Versucht ein Objekt in den angegebenen Zieltypen zu konvertieren
		/// </summary>
		/// <param name="value">das Objekt, das konvertiert werden soll</param>
		/// <param name="targetType">der Typ, in den das übergebene Objekt konvertiert werden soll</param>
		/// <param name="formatProvider">ein FormatProvider-Schnittstelle, die kulturabhängige Formatierungsinformationen liefert</param>
		/// <param name="result">das in den Zieltypen konvertierte Objekt</param>
		/// <returns>true, falls das Objekt erfolgreich konvertiert werden konnte; andernfalls false</returns>
		[DebuggerStepThrough]
		public static bool TryConvertTo(this object value,Type targetType,IFormatProvider formatProvider,out object result)
		{
			try
			{
				result = ObjectExtensions.ConvertTo(value,targetType,formatProvider);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}
	}
}
