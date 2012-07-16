using System;
using System.Globalization;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die DateTime-Klasse bereit
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Gibt die Woche des Jahres zurück, in die das Datum in der angegebenen System.DateTime fällt.
		/// </summary>
		/// <param name="dateTime">Die zu lesende System.DateTime</param>
		/// <returns>Eine positive ganze Zahl, die die Woche des Jahres darstellt, in die das im time-Parameter angegebene Datum fällt.</returns>
		public static int GetWeekOfYear(this DateTime dateTime)
		{
			DateTimeFormatInfo dateFormatInfo = DateTimeFormatInfo.CurrentInfo;
			return dateFormatInfo.Calendar.GetWeekOfYear(dateTime,dateFormatInfo.CalendarWeekRule,dateFormatInfo.FirstDayOfWeek);
		}
	}
}
