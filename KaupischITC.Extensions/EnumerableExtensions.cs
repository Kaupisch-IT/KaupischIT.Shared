using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die IEnumerable-Schnittstelle bereit
	/// </summary>
	public static class EnumerableExtensions
	{
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


		/// <summary>
		/// Korreliert die Elemente einer Sequenz anhand der Gleichheit der Schlüssel. Schlüssel werden mithilfe des Standardgleichheitsvergleichs verglichen.
		/// </summary>
		/// <typeparam name="TSource">Der Typ der Elemente der beiden Sequenzen.</typeparam>
		/// <typeparam name="TKey">Der Typ der von den Schlüsselauswahlfunktionen zurückgegebenen Schlüssel.</typeparam>
		/// <typeparam name="TResult"> Der Typ der Ergebniselemente.</typeparam>
		/// <param name="source">Die Sequenz, die mit sich selbst verknüpft werden soll</param>
		/// <param name="outerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der ersten Sequenz.</param>
		/// <param name="innerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der zweiten Sequenz.</param>
		/// <param name="resultSelector">Eine Funktion zum Erstellen eines Ergebniselements anhand eines Element aus der ersten Sequenz und einer Auflistung von übereinstimmenden Elementen aus der zweiten Sequenz.</param>
		/// <returns>Ein System.Collections.Generic.IEnumerable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IEnumerable<TResult> SelfJoin<TSource,TKey,TResult>(this IEnumerable<TSource> source,Func<TSource,TKey> outerKeySelector,Func<TSource,TKey> innerKeySelector,Func<TSource,TSource,TResult> resultSelector)
		{
			return source.Join(source,outerKeySelector,innerKeySelector,resultSelector);
		}


		/// <summary>
		/// Korreliert die Elemente einer Sequenz anhand der Gleichheit der Schlüssel. Dabei werden alle Tupel der ersten Sequenz in die Ergebnisrelation aufgenommen und jene Attribute eines Tupels mit Nullwerten aufgefüllt, die keinen Join-Partner aus der zweiten Sequenz gefunden haben. Schlüssel werden mithilfe des Standardgleichheitsvergleichs verglichen.
		/// </summary>
		/// <typeparam name="TSource">Der Typ der Elemente der beiden Sequenzen.</typeparam>
		/// <typeparam name="TKey">Der Typ der von den Schlüsselauswahlfunktionen zurückgegebenen Schlüssel.</typeparam>
		/// <typeparam name="TResult"> Der Typ der Ergebniselemente.</typeparam>
		/// <param name="source">Die Sequenz, die mit sich selbst verknüpft werden soll</param>
		/// <param name="outerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der ersten Sequenz.</param>
		/// <param name="innerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der zweiten Sequenz.</param>
		/// <param name="resultSelector">Eine Funktion zum Erstellen eines Ergebniselements anhand eines Element aus der ersten Sequenz und einer Auflistung von übereinstimmenden Elementen aus der zweiten Sequenz.</param>
		/// <returns>Ein System.Collections.Generic.IEnumerable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IEnumerable<TResult> SelfLeftOuterJoin<TSource,TKey,TResult>(this IEnumerable<TSource> source,Func<TSource,TKey> outerKeySelector,Func<TSource,TKey> innerKeySelector,Func<TSource,TSource,TResult> resultSelector)
		{
			return source.LeftOuterJoin(source,outerKeySelector,innerKeySelector,resultSelector);
		}


		/// <summary>
		/// Korreliert die Elemente von zwei Sequenzen anhand der Gleichheit der Schlüssel. Dabei werden alle Tupel der ersten Sequenz in die Ergebnisrelation aufgenommen und jene Attribute eines Tupels mit Nullwerten aufgefüllt, die keinen Join-Partner aus der zweiten Sequenz gefunden haben. Schlüssel werden mithilfe des Standardgleichheitsvergleichs verglichen.
		/// </summary>
		/// <typeparam name="TOuter">Der Typ der Elemente der ersten Sequenz.</typeparam>
		/// <typeparam name="TInner">Der Typ der Elemente der zweiten Sequenz.</typeparam>
		/// <typeparam name="TKey">Der Typ der von den Schlüsselauswahlfunktionen zurückgegebenen Schlüssel.</typeparam>
		/// <typeparam name="TResult"> Der Typ der Ergebniselemente.</typeparam>
		/// <param name="outer">Die erste zu verknüpfende Sequenz</param>
		/// <param name="inner">Die Sequenz, die mit der ersten Sequenz verknüpft werden soll.</param>
		/// <param name="outerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der ersten Sequenz.</param>
		/// <param name="innerKeySelector"> Eine Funktion zum Extrahieren des Joinschlüssels aus jedem Element der zweiten Sequenz.</param>
		/// <param name="resultSelector">Eine Funktion zum Erstellen eines Ergebniselements anhand eines Element aus der ersten Sequenz und einer Auflistung von übereinstimmenden Elementen aus der zweiten Sequenz.</param>
		/// <returns>Ein System.Collections.Generic.IEnumerable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IEnumerable<TResult> LeftOuterJoin<TOuter,TInner,TKey,TResult>(this IEnumerable<TOuter> outer,IEnumerable<TInner> inner,Func<TOuter,TKey> outerKeySelector,Func<TInner,TKey> innerKeySelector,Func<TOuter,TInner,TResult> resultSelector)
		{
			return outer
				.GroupJoin(inner,outerKeySelector,innerKeySelector,(outerItem,innerGroup) => innerGroup.DefaultIfEmpty().Select(innerGroupItem => resultSelector(outerItem,innerGroupItem)))
				.SelectMany(value => value);
		}



		/// <summary>
		/// Gruppiert eine Sequenz nach der angegebenen Datumsauswahlfunktion und fügt leere Gruppen für nicht vorhandene Datumswerte im gesamten Bereich der Sequenz ein.
		/// Die leeren Gruppen werden so gewählt, dass stets der gesamte Monat befüllt wird.
		/// </summary>
		/// <typeparam name="TElement">Der Typ der Elemente der Eingabesequenz</typeparam>
		/// <param name="source">Die Eingabesequenz</param>
		/// <param name="dateSelector">Die Funktion zum Extrahieren des Datums aus jedem Element</param>
		/// <returns>Eine Sequenz mit den gruppierten Elementen der Eingabesequenz</returns>
		public static IEnumerable<IGrouping<DateTime,TElement>> FullDateGroupBy<TElement>(this IEnumerable<TElement> source,Func<TElement,DateTime> dateSelector)
		{
			if (!(source is IList))	// für Performance (sonst wird source jeweils bei Min,Max und GroupJoin evaluiert)
				source = source.ToList();

			// Monatsanfang des kleinsten Datums der Eingabesequenz finden
			DateTime minDate = source.Min(item => dateSelector(item).Date);
			minDate = minDate.AddDays(1-minDate.Day);

			// Monatsende des größten Datums der Eingabesequenz finden
			DateTime maxDate = source.Max(item => dateSelector(item).Date);
			maxDate = maxDate.AddDays(1-maxDate.Day).AddMonths(1).AddDays(-1);

			// Datumsgruppierung durchführen
			return EnumerableExtensions.FullDateGroupBy(source,dateSelector,minDate,maxDate);
		}


		/// <summary>
		/// Gruppiert eine Sequenz nach der angegebenen Datumsauswahlfunktion und fügt leere Gruppen für nicht vorhandene Datumswerte im gesamten Bereich der Sequenz ein.
		/// </summary>
		/// <typeparam name="TElement">Der Typ der Elemente der Eingabesequenz</typeparam>
		/// <param name="source">Die Eingabesequenz</param>
		/// <param name="dateSelector">Die Funktion zum Extrahieren des Datums aus jedem Element</param>
		/// <param name="minDate">Das Datum, mit dem die Ausgabesequenz begonnen wird</param>
		/// <param name="maxDate">Das Datum, mit dem die Ausgabesequenz beendet wird</param>
		/// <returns></returns>
		public static IEnumerable<IGrouping<DateTime,TElement>> FullDateGroupBy<TElement>(this IEnumerable<TElement> source,Func<TElement,DateTime> dateSelector,DateTime minDate,DateTime maxDate)
		{
			return Enumerable.Range(0,(maxDate-minDate).Days+1).Select(i => minDate.AddDays(i))
				.GroupJoin(source,date => date,item => dateSelector(item).Date,(date,group) => new Grouping<DateTime,TElement>(date,group))
				.Cast<IGrouping<DateTime,TElement>>();
		}


		/// <summary>
		/// Stellt eine Auflistung von Objekten dar, die über einen gemeinsamen Schlüssel verfügen
		/// </summary>
		/// <typeparam name="TKey">Der Typ des Schlüssels</typeparam>
		/// <typeparam name="TValue">Der Typ der Werte</typeparam>
		private class Grouping<TKey,TValue> : IGrouping<TKey,TValue>
		{
			public TKey Key { get; set; }
			public IEnumerable<TValue> Values { get; set; }

			public Grouping(TKey key,IEnumerable<TValue> values)
			{
				this.Key = key;
				this.Values = values;
			}

			public IEnumerator<TValue> GetEnumerator() { return this.Values.GetEnumerator(); }
			IEnumerator IEnumerable.GetEnumerator() { return this.Values.GetEnumerator(); ; }
		}
	}
}
