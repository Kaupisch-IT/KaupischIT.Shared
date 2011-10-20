using System.Drawing;
using System.Linq;

namespace KaupischITC.Extensions
{
	public static class GraphicsExtensions
	{
		/// <summary>
		/// Gibt den Bereich an, der den angegeben Text umschließt, wenn dieser mit dem angegebenen Gerätekontext und mit der angegebenen Schriftart erstellt wird. 
		/// </summary>
		/// <param name="graphics">Der Gerätekontext, in dem der Text bemessen werden soll.</param>
		/// <param name="text">Der zu bemessende Text.</param>
		/// <param name="font">Der Font, der auf den bemessenen Text angewendet werden soll.</param>
		/// <returns>Den Bereich, der die Zeichenfolge umschließt. </returns>
		public static RectangleF MeasureText(this Graphics graphics,string text,Font font,int maxWidth=0)
		{
			CharacterRange[] characterRange = { new CharacterRange(0,text.Length) };
			StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
			stringFormat.SetMeasurableCharacterRanges(characterRange);
			Region[] regions = graphics.MeasureCharacterRanges(text,font,new Rectangle(0,0,maxWidth,0),stringFormat);
			return regions.First().GetBounds(graphics);
		}
	}
}
