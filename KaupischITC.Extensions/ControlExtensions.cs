using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Control-Klasse bereit
	/// </summary>
	public static class ControlExtensions
	{
		/// <summary>
		/// Ermittelt, ob sich der Mauszeiger momentan im Zeichenbereich des angegebenen Steuerelements befindet
		/// </summary>
		/// <param name="control">das Steuerelement, dessen Zeichenbereich ermittelt werden soll</param>
		/// <returns>true, wenn sich der Mauszeiger im Zeichenbereich des angegebenen Steuerelements befindet; andernfalls false</returns>
		public static bool ContainsMousePosition(this Control control)
		{
			return ControlExtensions.ContainsMousePosition(control,0);
		}


		/// <summary>
		/// Ermittelt, ob sich der Mauszeiger momentan im Zeichenbereich des angegebenen Steuerelements befindet
		/// </summary>
		/// <param name="control">das Steuerelement, dessen Zeichenbereich ermittelt werden soll</param>
		/// <param name="padding">die Breite des Zusatzrandes, der hinzugefügt werden soll</param>
		/// <returns>true, wenn sich der Mauszeiger im Zeichenbereich des angegebenen Steuerelements befindet; andernfalls false</returns>
		public static bool ContainsMousePosition(this Control control,int padding)
		{
			Rectangle rectangle = control.RectangleToScreen(control.ClientRectangle);
			rectangle.Inflate(padding,padding);
			return rectangle.Contains(Control.MousePosition);
		}


		/// <summary>
		/// Sendet dem Fenster die angegebene Meldung zu
		/// </summary>
		/// <param name="control">das Fenster, an das die Meldung geschickt werden soll</param>
		/// <param name="message">die Meldung</param>
		/// <returns>das Ergebnis des Meldungsverarbeitens; sein Wert hängt von der gesendeten Nachricht ab</returns>
		public static IntPtr SendMessage(this Control control,Message message)
		{
			return ControlExtensions.SendMessage(control,message.Msg,message.WParam,message.LParam);
		}

		/// <summary>
		/// Sendet dem Fenster die angegebene Meldung zu
		/// </summary>
		/// <param name="control">das Fenster, an das die Meldung geschickt werden soll</param>
		/// <param name="msg">die ID-Nummer der Meldung</param>
		/// <param name="wParam">das WParam-Feld der Meldung</param>
		/// <param name="lParam">das LParam-Feld der Meldung</param>
		/// <returns>das Ergebnis des Meldungsverarbeitens; sein Wert hängt von der gesendeten Nachricht ab</returns>
		public static IntPtr SendMessage(this Control control,Int32 msg,IntPtr wParam,IntPtr lParam)
		{
			return ControlExtensions.SendMessage(control.Handle,msg,wParam,lParam);
		}

		[DllImport("user32.dll",EntryPoint="SendMessage",CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd,Int32 Msg,IntPtr wParam,IntPtr lParam);



		/// <summary>
		/// Ermittelt die sichtbaren Bildlaufleisten des Steuerelements
		/// </summary>
		/// <param name="control">das Steuerelement, dessen sichtbare Bildlaufleisten ermittelt werden sollen</param>
		/// <returns>die sichtbaren Bildlaufleisten</returns>
		public static ScrollBars GetVisibleScrollbars(this Control control)
		{
			const int GWL_STYLE = -16; // offset of window style value

			// window style constants for scrollbars
			const int WS_VSCROLL = 0x00200000;
			const int WS_HSCROLL = 0x00100000;

			int wndStyle = ControlExtensions.GetWindowLong(control.Handle,GWL_STYLE);
			bool hsVisible = (wndStyle & WS_HSCROLL) != 0;
			bool vsVisible = (wndStyle & WS_VSCROLL) != 0;

			if (hsVisible)
				return vsVisible ? ScrollBars.Both : ScrollBars.Horizontal;
			else
				return vsVisible ? ScrollBars.Vertical : ScrollBars.None;
		}

		[DllImport("user32.dll",EntryPoint="GetWindowLong",SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd,int nIndex);
	}
}
