using System.Reflection;
using System.Runtime.CompilerServices;

namespace KaupischIT.Extensions
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
		public static bool IsCompilerGenerated(this ICustomAttributeProvider customAttributeProvider) => customAttributeProvider.IsDefined(typeof(CompilerGeneratedAttribute),false);


		/// <summary>
		/// Ermittelt, ob der angegebene ICustomAttributeProvider das ExtensionAttribute besitzt
		/// </summary>
		/// <param name="customAttributeProvider">der ICustomAttributeProvider</param>
		/// <returns>true, wenn der ICustomAttributeProvider ein ExtensionAttribute-Attribut besitzt; andernfalls false</returns>
		public static bool IsExtension(this ICustomAttributeProvider customAttributeProvider) => customAttributeProvider.IsDefined(typeof(ExtensionAttribute),false);
	}
}
