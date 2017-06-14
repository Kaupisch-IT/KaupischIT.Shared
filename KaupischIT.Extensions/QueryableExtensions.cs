using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die IQueryable-Schnittstelle bereit
	/// </summary>
	public static class QueryableExtensions
	{
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
		/// <returns>Ein System.Linq.IQueryable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IQueryable<TResult> SelfJoin<TSource, TKey, TResult>(this IQueryable<TSource> source,Expression<Func<TSource,TKey>> outerKeySelector,Expression<Func<TSource,TKey>> innerKeySelector,Expression<Func<TSource,TSource,TResult>> resultSelector)
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
		/// <returns>Ein System.Linq.IQueryable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IQueryable<TResult> SelfLeftOuterJoin<TSource, TKey, TResult>(this IQueryable<TSource> source,Expression<Func<TSource,TKey>> outerKeySelector,Expression<Func<TSource,TKey>> innerKeySelector,Expression<Func<TSource,TSource,TResult>> resultSelector)
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
		/// <returns>Ein System.Linq.IQueryable, das Elemente vom Typ TResult enthält, die durch Ausführen eines Group Joins von zwei Sequenzen ermittelt werden.</returns>
		public static IQueryable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,IQueryable<TInner> inner,Expression<Func<TOuter,TKey>> outerKeySelector,Expression<Func<TInner,TKey>> innerKeySelector,Expression<Func<TOuter,TInner,TResult>> resultSelector)
		{
			// Das soll erstellt werden: (outerItem,innerGroup) => innerGroup.DefaultIfEmpty().Select(innerGroupItem => resultSelector(outerItem,innerGroupItem))
			// Dabei muss "innerGroupItem => resultSelector(outerItem,innerGroupItem)" direkt in den Aufruf von resultSelector aufgelöst werden 

			ParameterExpression outerItemParameterExpression = resultSelector.Parameters[0]; // Expression.Parameter(typeof(TOuter),"outerItem");
			ParameterExpression innerGroupParameterExpression = Expression.Parameter(typeof(IEnumerable<TInner>),"innerGroup");
			ParameterExpression innerGroupItemParameterExpression = resultSelector.Parameters[1]; // Expression.Parameter(typeof(TInner),"innerGroupItem");

			MethodInfo enumerableSelectMethodInfo = typeof(Enumerable).GetGenericMethod("Select",typeof(IEnumerable<>),typeof(Func<,>)).MakeGenericMethod(typeof(TInner),typeof(TResult));
			MethodInfo enumerableDefaultIfEmptyMethodInfo = typeof(Enumerable).GetGenericMethod("DefaultIfEmpty",typeof(IEnumerable<>)).MakeGenericMethod(typeof(TInner));

			InvocationExpression callResultSelectorExpression = Expression.Invoke(resultSelector,outerItemParameterExpression,innerGroupItemParameterExpression);
			MethodCallExpression callDefaultIfEmtptyExpression = Expression.Call(null,enumerableDefaultIfEmptyMethodInfo,innerGroupParameterExpression);
			MethodCallExpression callSelectExpression = Expression.Call(null,enumerableSelectMethodInfo,callDefaultIfEmtptyExpression,Expression.Lambda<Func<TInner,TResult>>(callResultSelectorExpression,innerGroupItemParameterExpression));
			Expression<Func<TOuter,IEnumerable<TInner>,IEnumerable<TResult>>> expandedResultSelectionExpression = Expression.Lambda<Func<TOuter,IEnumerable<TInner>,IEnumerable<TResult>>>(callSelectExpression,outerItemParameterExpression,innerGroupParameterExpression);

			return outer
				.GroupJoin(inner,outerKeySelector,innerKeySelector,expandedResultSelectionExpression)
				.SelectMany(value => value);
		}
	}
}
