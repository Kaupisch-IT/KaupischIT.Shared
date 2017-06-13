using System.Diagnostics;
using System.Security.Principal;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die NTAccount-Klasse bereit
	/// </summary>
	public static class NTAccountExtensions
	{
		/// <summary>
		/// Übersetzt das NTAccount-Objekt in ein SecurityIdentifier-Objekt
		/// </summary>
		/// <param name="ntAccount">das NTAccount-Objekt, das in ein SecurityIdentifier-Objekt umgewandelt werden soll</param>
		/// <returns>das in ein SecurityIdentifier-Objekt umgewandelte NTAccount-Objekt</returns>
		[DebuggerStepThrough]
		public static SecurityIdentifier ToSecurityIdentifier(this NTAccount ntAccount)
		{
			try { return ((SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier))); }
			catch { return null; }
		}
	}
}
