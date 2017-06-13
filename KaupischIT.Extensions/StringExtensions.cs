using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt einen Satz von static-Methoden als Erweiterung für den string-Typen bereit
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Kürzt ggf. die übergebene Zeichenfolge auf die angegebene Länge
		/// </summary>
		/// <param name="value">die Zeichenfolge, die ggf. gekürzt werden soll</param>
		/// <param name="maxLength">die maximale Länge der Zeichenfolge</param>
		/// <returns>die gekürzte Zeichenfolge, falls die Länge die angegebene Maximallänge überschreitet; andernfalls die unveränderte Zeichenfolge</returns>
		public static string WithMaxLength(this string value,int maxLength)
		{
			return (value!=null && value.Length>maxLength) ? value.Substring(0,maxLength) : value;
		}



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



		/// <summary>
		/// Ersetzt ein oder mehrere Formatelemente in einer angegebenen Zeichenfolge durch die Zeichenfolgendarstellung der Eigenschaften eines angegebenen Objekts.
		/// </summary>
		/// <param name="format">eine kombinierte Formatzeichenfolge</param>
		/// <param name="value">das Objekt mit den zu formatierenden Eigenschaften</param>
		/// <param name="provider">ein Objekt, das kulturspezifische Formatierungsinformationen bereitstellt</param>
		/// <returns>eine Kopie von format, in der die Formatelemente durch die Zeichenfolgendarstellung der entsprechenden Eigenschaften aus source ersetzt wurden</returns>
		/// <remarks>http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables.aspx</remarks>
		public static string FormatWith(this string format,object value,IFormatProvider provider = null)
		{
			Func<string,object> matchEvaluator = (propertyName) =>
			{
				if (propertyName=="0")
					return value;
				else
					try { return DataBinder.Eval(value,propertyName); }
					catch (HttpException e) { throw new FormatException(null,e); }
			};

			return StringExtensions.FormatWith(format,matchEvaluator,provider);
		}


		/// <summary>
		/// Ersetzt ein oder mehrere Formatelemente in einer angegebenen Zeichenfolge durch die Zeichenfolgendarstellung der Eigenschaften eines angegebenen Objekts.
		/// </summary>
		/// <param name="format">eine kombinierte Formatzeichenfolge</param>
		/// <param name="matchEvaluator">ein Delegat, der ein Format als Eingabe akzeptiert und das entsprechende zu formatierende Objekt zurück gibt</param>
		/// <param name="provider">ein Objekt, das kulturspezifische Formatierungsinformationen bereitstellt</param>
		/// <returns>eine Kopie von format, in der die Formatelemente durch die Zeichenfolgendarstellung der entsprechenden Eigenschaften aus source ersetzt wurden</returns>
		/// <remarks>http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables.aspx</remarks>
		public static string FormatWith(this string format,Func<string,object> matchEvaluator,IFormatProvider provider = null)
		{
			if (format == null)
				throw new ArgumentNullException("format");

			List<object> values = new List<object>();

			Regex regex = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
			string rewrittenFormat = regex.Replace(format,
				delegate(Match match)
				{
					Group startGroup = match.Groups["start"];
					Group propertyGroup = match.Groups["property"];
					Group formatGroup = match.Groups["format"];
					Group endGroup = match.Groups["end"];

					values.Add(matchEvaluator(propertyGroup.Value));

					int openingsCount = startGroup.Captures.Count;
					int closingsCount = endGroup.Captures.Count;

					if (openingsCount>closingsCount || openingsCount%2==0)
						return match.Value;
					else
						return new string('{',openingsCount) + (values.Count-1) + formatGroup.Value + new string('}',closingsCount);
				});

			return string.Format(provider,rewrittenFormat,values.ToArray());
		}
	}
}
