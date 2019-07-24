using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KaupischIT.Shared
{
	public class ConsoleForWindowsApplication : IDisposable
	{
		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int pid);

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
