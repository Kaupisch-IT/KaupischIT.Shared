using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Environment-Klasse bereit
	/// </summary>
	public static class EnvironmentExtensions
	{
		/// <summary>
		/// Ruft den Anzeigenamen der Person ab, die derzeit beim Windows-Betriebssystem angemeldet ist.
		/// </summary>
		public static string UserDisplayName
		{
			get { return EnvironmentExtensions.GetUserFullName(Environment.UserDomainName,Environment.UserName); }
		}


		/// <summary>
		/// Ermittelt den Anzeigenamen des angegebenen Benutzers
		/// </summary>
		/// <param name="userDomainName">der Netzwerkdomänenname des Benutzers</param>
		/// <param name="userName">der Benutzername des Benutzers</param>
		/// <returns>den Anzeigenamen des Angegebenen Benutzers</returns>
		public static string GetUserFullName(string userDomainName,string userName)
		{
			// Shortcut für aktuell angemeldeten Benutzer (da die DirectoryEntry-Variante grottenlangsam ist)
			if (String.Equals(userDomainName,Environment.UserDomainName,StringComparison.InvariantCultureIgnoreCase) && String.Equals(userName,Environment.UserName,StringComparison.InvariantCultureIgnoreCase))
				try
				{
					StringBuilder result = new StringBuilder(1024);
					int userNameSize = result.Capacity;
					EnvironmentExtensions.GetUserNameEx(3,result,ref userNameSize);
					return result.ToString();
				}
				catch { }

			// ActiveDirectory fragen
			try { return new DirectoryEntry("WinNT://"+userDomainName+"/"+userName).Properties["fullName"].Value.ToString(); }
			catch { return userDomainName+"\\"+userName; }
		}

		[DllImport("secur32.dll",EntryPoint="GetUserNameEx",CharSet=CharSet.Auto)]
		private static extern int GetUserNameEx(int nameFormat,StringBuilder userName,ref int userNameSize);
	}
}
