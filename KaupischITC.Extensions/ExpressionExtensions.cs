using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Expression-Klasse bereit
	/// </summary>
	public static class ExpressionExtensions
	{
		private static MethodInfo stringConcatMethodInfo = typeof(string).GetMethod("Concat",new[] { typeof(string),typeof(string) });	// für den Plus-Operator für für Zeichenketten


		/// <summary>
		/// Erzeugt eine Addition von zwei Ausdrucksbäumen
		/// </summary>
		/// <typeparam name="T">der Typ der Summanden</typeparam>
		/// <typeparam name="TValue">der Typ der Summe</typeparam>
		/// <param name="expr1">der erste Summand</param>
		/// <param name="expr2">der zweite Summand</param>
		/// <returns>einen Ausdrucksbaum, der eine Addition der beiden übergebenen Ausdrucksbäume darstellt</returns>
		public static Expression<Func<T,TValue>> Add<T,TValue>(this Expression<Func<T,TValue>> expr1,Expression<Func<T,TValue>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2,expr1.Parameters.Cast<Expression>());
			MethodInfo methodInfo = (typeof(TValue)==typeof(string)) ? stringConcatMethodInfo : null;

			return Expression.Lambda<Func<T,TValue>>(Expression.Add(expr1.Body,invokedExpr,methodInfo),expr1.Parameters);
		}


		/// <summary>
		/// Erzeugt eine Subtraktion von zwei Ausdrucksbäumen
		/// </summary>
		/// <typeparam name="T">der Typ des Minuenden/Subtrahenden</typeparam>
		/// <typeparam name="TValue">der Typ der Differenz</typeparam>
		/// <param name="expr1">der Minuend</param>
		/// <param name="expr2">der Subtrahend</param>
		/// <returns>einen Ausdrucksbaum, der eine Subtraktion der beiden übergebenen Ausdrucksbäume darstellt</returns>
		public static Expression<Func<T,TValue>> Subtract<T,TValue>(this Expression<Func<T,TValue>> expr1,Expression<Func<T,TValue>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2,expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T,TValue>>(Expression.Subtract(expr1.Body,invokedExpr),expr1.Parameters);
		}


		/// <summary>
		/// Erzeugt eine Multiplikation von zwei Ausdrucksbäumen
		/// </summary>
		/// <typeparam name="T">der Typ der Faktoren</typeparam>
		/// <typeparam name="TValue">der Typ der Produkts</typeparam>
		/// <param name="expr1">der erste Faktor</param>
		/// <param name="expr2">der zweite Faktor</param>
		/// <returns>einen Ausdrucksbaum, der eine Multiplikation der beiden übergebenen Ausdrucksbäume darstellt</returns>
		public static Expression<Func<T,TValue>> Multiply<T,TValue>(this Expression<Func<T,TValue>> expr1,Expression<Func<T,TValue>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2,expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T,TValue>>(Expression.Multiply(expr1.Body,invokedExpr),expr1.Parameters);
		}


		/// <summary>
		/// Erzeugt eine Division von zwei Ausdrucksbäumen
		/// </summary>
		/// <typeparam name="T">der Typ des Dividenden/Divisors</typeparam>
		/// <typeparam name="TValue">der Typ der Quotienten</typeparam>
		/// <param name="expr1">der Dividend</param>
		/// <param name="expr2">der Divisor</param>
		/// <returns>einen Ausdrucksbaum, der eine Division der beiden übergebenen Ausdrucksbäume darstellt</returns>
		public static Expression<Func<T,TValue>> Divide<T,TValue>(this Expression<Func<T,TValue>> expr1,Expression<Func<T,TValue>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2,expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T,TValue>>(Expression.Divide(expr1.Body,invokedExpr),expr1.Parameters);
		}


		/// <summary>
		/// Ermittelt den Namen des aufgerufenen Members, das als Expression übergeben wurde
		/// </summary>
		/// <param name="expression">die Expression des Memberaufrufs</param>
		/// <returns>den Namen des aufgerufenen Members</returns>
		public static string GetMemberName(this LambdaExpression expression)
		{
			MemberExpression memberExpression = expression.Body as MemberExpression;
			if (memberExpression!=null)
				return memberExpression.Member.Name;
			else
				throw new ArgumentException("Der angegebene Ausdruck entspricht keinem Memberzugriff.");
		}
	}

	
	/// <summary>
	/// Stellt Methoden bereit, um typisiert den Namen einen Members zu ermitteln
	/// </summary>
	public static class GetMemberName
	{
		/// <summary>
		/// Ermittelt den Namen des angegebenen Members
		/// </summary>
		/// <typeparam name="T">der Typ des Objekts, dessen Member aufgerufen wird</typeparam>
		/// <param name="expression">die Expression des Memberaufrufs</param>
		/// <returns>den Namen des aufgerufenen Members</returns>
		public static string Of<T>(Expression<Func<T,object>> expression)
		{
			return ExpressionExtensions.GetMemberName((LambdaExpression)expression);
		}


		/// <summary>
		/// Ermittelt den Namen des angegebenen Members
		/// </summary>
		/// <param name="expression">die Expression des Memberaufrufs</param>
		/// <returns>den Namen des aufgerufenen Members</returns>
		public static string Of(Expression<Func<object>> expression)
		{
			return ExpressionExtensions.GetMemberName((LambdaExpression)expression);
		}
	}
}
