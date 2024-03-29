﻿using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Type-Klasse bereit
	/// </summary>
	public static class TypeExtensions
	{
		private static readonly Type[] numericTypes = new[] { typeof(byte),typeof(decimal),typeof(double),typeof(short),typeof(int),typeof(long),typeof(sbyte),typeof(float),typeof(ushort),typeof(uint),typeof(ulong) };
		private static readonly Type[] numericNullableTypes = new[] { typeof(byte?),typeof(decimal?),typeof(double?),typeof(short?),typeof(int?),typeof(long?),typeof(sbyte?),typeof(float?),typeof(ushort?),typeof(uint?),typeof(ulong) };


		/// <summary>
		/// Ermittelt, ob ein Typ ein numerischer Typ ist
		/// </summary>
		/// <param name="type">der Typ, der überprüft werden soll.</param>
		/// <returns></returns>
		public static bool IsNumeric(this Type type)
		{
			return TypeExtensions.numericTypes.Contains(type) || TypeExtensions.numericNullableTypes.Contains(type);
		}


		/// <summary>
		/// Sucht die angegebene generische öffentliche Methode, deren Parameter den angegebenen Argumenttypen entsprechen.
		/// </summary>
		/// <param name="type">Das Type-Objekt, dessen Methoden durchsucht werden sollen</param>
		/// <param name="name">Der String, der den Namen der abzurufenden öffentlichen Methode enthält.</param>
		/// <param name="parameterTypes">Ein Array von Type-Objekten, das die Anzahl, die Reihenfolge und den Typ der Parameter der abzurufenden Methode darstellt. – oder – Ein leeres Array von Type-Objekten (bereitgestellt vom EmptyTypes-Feld) zum Abrufen einer Methode, die keine Parameter akzeptiert.</param>
		/// <returns>Ein MethodInfo-Objekt, das die öffentliche Methode darstellt, deren Parameter den angegebenen Argumenttypen entsprechen, sofern gefunden, andernfalls null.</returns>
		public static MethodInfo GetGenericMethod(this Type type,string name,params Type[] parameterTypes)
		{
			return type.GetMethods()
				.Where(method => method.Name == name)
				.Where(method => parameterTypes.SequenceEqual(method.GetParameters().Select(p => (p.ParameterType.IsGenericType) ? p.ParameterType.GetGenericTypeDefinition() : p.ParameterType)))
				.SingleOrDefault();
		}


		/// <summary>
		/// Prüft, ob der angegebene Typ ein generischer Nullable-Typ ist
		/// </summary>
		/// <param name="type">der Typ, der geprüft werden soll</param>
		/// <returns>True, wenn der Typ ein generischer Nullable-Typ ist; andernfalls false</returns>
		public static bool IsNullable(this Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition()==typeof(Nullable<>));
		}


		/// <summary>
		/// Prüft, ob Werte des angegebenen Typs den Wert null annehmen können.
		/// </summary>
		/// <param name="type">der Typ, der geprüft werden soll</param>
		/// <returns>True, wenn Werte des angegebenen Typs den Wert null annehmen können; andernfalls false</returns>
		public static bool IsNullAssignable(this Type type)
		{
			return (!type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition()==typeof(Nullable<>)));
		}


		/// <summary>
		/// Ermittelt, ob der angegebene Typ eine bestimmte Schnittstelle implementiert
		/// </summary>
		/// <param name="type">der Typ, der geprüft werden soll</param>
		/// <param name="interfaceType">der Typ der Schnittstelle</param>
		/// <returns>True, wenn der angegebene Typ die angegebene Schnittstelle implementiert; andernfalls false</returns>
		public static bool ImplementsInterface(this Type type,Type interfaceType)
		{
			if (!interfaceType.IsInterface)
				throw new ArgumentException("Der Typ '"+interfaceType+"' ist keine Schnittstelle.");

			if (interfaceType.IsGenericTypeDefinition)
				return type.GetInterfaces().Concat(new[] { type }).Any(itype => itype.IsGenericType && itype.GetGenericTypeDefinition()==interfaceType);
			else
				return type.GetInterfaces().Concat(new[] { type }).Any(itype => itype==interfaceType);
		}


		/// <summary>
		/// Ermittelt den benutzerfreundlichen Typennamen
		/// </summary>
		/// <param name="type">der Typ, dessen benutzerfreundlicher Name ermittelt werden soll</param>
		/// <returns>den benutzerfreundlichen Typennamen</returns>
		public static string GetPrettyName(this Type type)
		{
			if (type==null)
				return null;

			return Regex.Replace(type.Name,@"`(?<count>\d)$",match =>
			{
				Type[] argumentTypes = type.GetGenericArguments();
				if (type.IsGenericType)
					argumentTypes = type.GetGenericTypeDefinition().GetGenericArguments().Select((genericArgumentType,i) => (argumentTypes[i].IsCompilerGenerated()) ? genericArgumentType : argumentTypes[i]).ToArray();

				return "<"+String.Join(",",Enumerable.Range(0,Convert.ToInt32(match.Groups["count"].Value)).Select(i => argumentTypes[i].GetPrettyName()).ToArray())+">";
			});
		}


		/// <summary>
		/// Ermittelt den benutzerfreundlichen Typennamen inklusive Namespace
		/// </summary>
		/// <param name="type">der Typ, dessen benutzerfreundlicher Name inklusive Namespace ermittelt werden soll</param>
		/// <returns>den benutzerfreundlichen Typennamen inklusive Namespace</returns>
		public static string GetPrettyFullName(this Type type)
		{
			if (type==null)
				return null;

			return Regex.Replace(type.FullName,@"`(?<count>\d)\[.*\]$",match =>
			{
				Type[] argumentTypes = type.GetGenericArguments();
				if (type.IsGenericType)
					argumentTypes = type.GetGenericTypeDefinition().GetGenericArguments().Select((genericArgumentType,i) => (argumentTypes[i].IsCompilerGenerated()) ? genericArgumentType : argumentTypes[i]).ToArray();

				return "<"+String.Join(",",Enumerable.Range(0,Convert.ToInt32(match.Groups["count"].Value)).Select(i => argumentTypes[i].GetPrettyFullName()).ToArray())+">";
			});
		}


		/// <summary>
		/// Gibt den Assembly-qualifizierten Namen eines Typs in Kurzform zurück
		/// </summary>
		/// <param name="type">der Typ, dessen Assembly-qualifizierter Name in Kurzform zurückgegeben werden soll</param>
		/// <returns>den Assembly-qualifizierter Name in Kurzform</returns>
		public static string GetShortAssemblyQualifiedName(this Type type)
		{
			if (type==null)
				return null;

			AssemblyName assemblyName = type.Assembly.GetName();
			if (assemblyName.Name=="mscorlib")
				return type.FullName;
			else
				return type.FullName+", "+type.Assembly.GetName().Name;
		}
	}
}
