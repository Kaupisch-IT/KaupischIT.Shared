using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace KaupischIT.Extensions
{
	/// <summary>
	/// Stellt Erweiterungsmethoden für die Image-Klasse bereit
	/// </summary>
	public static class ImageExtensions
	{
		/// <summary>
		/// Wandelt ein Bild ein seine Bytearray-Repräsentation um
		/// </summary>
		/// <param name="image">das Bild, aus dem ein Bytearray erstellt werden soll</param>
		/// <param name="imageFormat">das Format des umgewandelten Bildes</param>
		/// <returns>die Bytearray-Repräsentation des Bildes</returns>
		public static byte[] ToByteArray(this Image image,ImageFormat imageFormat)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				image.Save(memoryStream,imageFormat);
				memoryStream.Flush();
				return memoryStream.ToArray();
			}
		}
	}
}
