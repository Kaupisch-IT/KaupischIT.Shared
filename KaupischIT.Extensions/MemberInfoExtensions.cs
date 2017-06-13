using System;
using System.Reflection;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die MemberInfo-Klasse bereit
	/// </summary>
	public static class MemberInfoExtensions
	{
		/// <summary>
		/// Ermittelt den (Rückgabe-)Typ eines Members
		/// </summary>
		/// <param name="memberInfo">das MemberInfo, dessen (Rückgabe-)Typ ermittelt werden soll</param>
		/// <returns>der (Rückgabe-)Typ des übergebenen Members</returns>
		public static Type GetMemberType(this MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
				return ((PropertyInfo)memberInfo).PropertyType;
			else if (memberInfo is MethodInfo)
				return ((MethodInfo)memberInfo).ReturnType;
			else if (memberInfo is FieldInfo)
				return ((FieldInfo)memberInfo).FieldType;
			
			throw new ArgumentException(memberInfo.ToString());
		}
	}
}
