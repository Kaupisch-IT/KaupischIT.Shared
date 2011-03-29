using System;
using System.Linq;
using System.Reflection;

namespace KaupischITC.Extensions
{
	public static class TypeExtensions
	{
		private static readonly Type[] numericTypes = new[] { typeof(Byte),typeof(Decimal),typeof(Double),typeof(Int16),typeof(Int32),typeof(Int64),typeof(SByte),typeof(Single),typeof(UInt16),typeof(UInt32),typeof(UInt64) };
		private static readonly Type[] numericNullableTypes = new[] { typeof(Byte?),typeof(Decimal?),typeof(Double?),typeof(Int16?),typeof(Int32?),typeof(Int64?),typeof(SByte?),typeof(Single?),typeof(UInt16?),typeof(UInt32?),typeof(UInt64) };


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
	}
}
