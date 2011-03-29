using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die ICustomAttributeProvider-Klasse bereit
	/// </summary>
	public static class CustomAttributeProviderExtensions
	{
		/// <summary>
		/// Ermittelt, ob der angegebene ICustomAttributeProvider das CompilerGeneratedAttribute besitzt
		/// </summary>
		/// <param name="customAttributeProvider">der ICustomAttributeProvider</param>
		/// <returns>true, wenn der ICustomAttributeProvider ein CompilerGenerated-Attribut besitzt; andernfalls false</returns>
		public static bool IsCompilerGenerated(this ICustomAttributeProvider customAttributeProvider)
		{
			return customAttributeProvider.GetCustomAttributes(typeof(CompilerGeneratedAttribute),false).Any();
		}
	}
}
