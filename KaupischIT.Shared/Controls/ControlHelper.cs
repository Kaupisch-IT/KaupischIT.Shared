using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: KaupischIT.Shared.Subversion("$Id$")]
namespace KaupischIT.Shared
{
	/// <summary>
	/// Stellt Hilfsmethoden für die Control-Klasse bereit
	/// </summary>
	[Subversion("$Id$")]
	public static class ControlHelper
	{
		private const int BCM_FIRST = 0x1600;
		private const int BCM_SETSHIELD = (BCM_FIRST + 0x000C);

		[DllImport("user32")]
		public static extern uint SendMessage(IntPtr hWnd,uint msg,uint wParam,uint lParam);



		/// <summary>
		/// Hilfsmethode zum Delegieren einer Aktion in den GUI-Thread
		/// </summary>
		/// <param name="control">ein Control (das der GUI-Thread erstellt hat)</param>
		/// <param name="action">die Aktion, die ausgeführt werden soll</param>
		public static void Invoke(Control control,MethodInvoker action)
		{
			if (!control.IsDisposed)
				if (control.InvokeRequired)
					try
					{
						control.Invoke((MethodInvoker)delegate
						{
							if (!control.IsDisposed)
								action();
						});
					}
					catch (ObjectDisposedException)
					{ }
				else
					action();
		}


		public static void InvokeAsync(Control control,MethodInvoker action)
		{
			if (!control.IsDisposed)
				if (control.InvokeRequired)
					try
					{
						control.BeginInvoke((MethodInvoker)delegate
						{
							if (!control.IsDisposed)
								action();
						});
					}
					catch (ObjectDisposedException)
					{ }
				else
					action();
		}



		/// <summary>
		/// Fügt einem Button das Admin-Schild hinzu
		/// </summary>
		/// <param name="b">The button</param>
		public static void AddShieldToButton(Button button)
		{
			button.FlatStyle = FlatStyle.System;
			SendMessage(button.Handle,BCM_SETSHIELD,0,0xFFFFFFFF);
		}
	}
}
