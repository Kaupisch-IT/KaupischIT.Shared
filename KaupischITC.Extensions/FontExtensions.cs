using System.Drawing;

namespace KaupischITC.Extensions
{
	public static class FontExtensions
	{
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
