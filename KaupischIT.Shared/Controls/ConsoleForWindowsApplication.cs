using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KaupischIT.Shared
{
	/// <summary>
	/// Stellt den Zugriff auf die Console für als Windows-Anwendung erstellte Anwendungen bereit
	/// (sodass z.B. Console.WriteLine benutzt werden kann und auch was auf der Console ausgibt)
	/// </summary>
	public class ConsoleForWindowsApplication : IDisposable
	{
		/// <summary> Allocates a new console for the calling process </summary>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		/// <summary> Attaches the calling process to the console of the specified process. </summary>
		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int pid);

		/// <summary> Detaches the calling process from its console. </summary>
		[DllImport("kernel32.dll",SetLastError = true)]
		private static extern bool FreeConsole();

		public ConsoleForWindowsApplication()
		{
			if (!ConsoleForWindowsApplication.AttachConsole(-1))
				ConsoleForWindowsApplication.AllocConsole();
		}

		public void Dispose()
		{
			ConsoleForWindowsApplication.FreeConsole();
			SendKeys.SendWait("{ENTER}");
		}
	}
}
