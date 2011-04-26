using System.Drawing;
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
	}
}
