using System.Diagnostics;
using System.Security.Principal;

namespace KaupischITC.Extensions
{
	public static class NTAccountExtensions
	{
		[DebuggerStepThrough]
		public static SecurityIdentifier ToSecurityIdentifier(this NTAccount ntAccount)
		{
			try { return ((SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier))); }
			catch { return null; }
		}
	}
}
