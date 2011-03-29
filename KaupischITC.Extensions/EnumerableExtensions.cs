using System;
using System.Collections.Generic;
using System.Linq;

namespace KaupischITC.Extensions
{
	public static class EnumerableExtensions
	{
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
	}
}
