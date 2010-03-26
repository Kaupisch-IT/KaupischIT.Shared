using System.Windows.Forms;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Stellt Hilfsmethoden für die Control-Klasse bereit
	/// </summary>
	public static class ControlHelper
	{
		/// <summary>
		/// Hilfsmethode zum Delegieren einer Aktion in den GUI-Thread
		/// </summary>
		/// <param name="control">ein Control (das der GUI-Thread erstellt hat)</param>
		/// <param name="action">die Aktion, die ausgeführt werden soll</param>
		public static void Invoke(Control control,MethodInvoker action)
		{
			if (!control.IsDisposed)
				if (control.InvokeRequired)
					control.Invoke((MethodInvoker)delegate { action(); });
				else
					action();
		}
	}
}
