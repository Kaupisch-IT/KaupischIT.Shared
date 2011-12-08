using System;
using System.Diagnostics;
using System.Reflection;

namespace KaupischITC.Extensions
{
	public static class MemberInfoExtensions
	{
		[DebuggerStepThrough]
		public static Type GetMemberType(this MemberInfo memberInfo)
		{
			if (memberInfo is PropertyInfo)
				return ((PropertyInfo)memberInfo).PropertyType;
			else if (memberInfo is MethodInfo)
				return ((MethodInfo)memberInfo).ReturnType;
			else if (memberInfo is FieldInfo)
				return ((FieldInfo)memberInfo).FieldType;
			else
				throw new ArgumentException(memberInfo.ToString());
		}
	}
}
