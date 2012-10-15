using System.Drawing;

namespace KaupischITC.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Font-Klasse bereit
	/// </summary>
	public static class FontExtensions
	{
		/// <summary>
		/// Erstellt aus der angegebenen Schriftart eine Quelltexteditor-Schriftart
		/// </summary>
		/// <param name="font">die Schriftart, für die eine Quelltexteditor-Schriftart erstellt werden soll</param>
		/// <returns>die entsprechende Quelltexteditor-Schriftart</returns>
		public static Font MakeCodeFontFamily(this Font font)
		{
			Font result = new Font("Consolas",font.SizeInPoints,font.Style);
			if (result.Name=="Consolas")
				return result;
			else
			{
				result.Dispose();
				return new Font("Courier New",font.SizeInPoints,font.Style);
			}
		}
	}
}
