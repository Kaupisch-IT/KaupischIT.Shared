using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KaupischITC.Extensions
{
	public static class CustomAttributeProviderExtensions
	{
		public static bool IsCompilerGenerated(this ICustomAttributeProvider customAttributeProvider)
		{
			return customAttributeProvider.GetCustomAttributes(typeof(CompilerGeneratedAttribute),false).Any();
		}
	}
}
