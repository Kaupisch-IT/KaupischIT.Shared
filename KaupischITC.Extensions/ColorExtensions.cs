using System;
using System.Drawing;

namespace KaupischITC.Extensions
{
	public static class ColorExtensions
	{
		/// <summary>
		/// Erstellt eine System.Drawing.Color-Struktur, in dem die Helligkeit der übergebenen Farbe geändert wird
		/// </summary>
		/// <param name="color">Farbe, deren Helligkeit geändert werden soll</param>
		/// <param name="factor">Faktor, der auf die ARGB-Komponenten angewendet wird</param>
		/// <returns>Farbe mit geänderter Helligkeit</returns>
		public static Color ChangeBrightness(this Color color,float factor)
		{
			return Color.FromArgb(
				Math.Min(255,(int)(color.A*(2-factor))),
				Math.Min(255,(int)(color.R*factor)),
				Math.Min(255,(int)(color.G*factor)),
				Math.Min(255,(int)(color.B*factor)));
		}



		/// <summary>
		/// Erstellt eine System.Drawing.Color-Struktur aus den drei HSB-Komponenten (Farbton, Sättigung, Helligkeit)
		/// </summary>
		/// <param name="Hue">Farbton (0..360)</param>
		/// <param name="Saturation">Sättigung (0..1)</param>
		/// <param name="Brightness">Helligkeit (0..1)</param>
		/// <returns>eine System.Drawing.Color-Struktur</returns>
		public static Color GetColorFromHSB(float hue,float saturation,float brightness)
		{
			// Parameter prüfen
			if (hue<0 || hue>360)
				throw new ArgumentException("hue 0..360");
			if (saturation<0 || saturation>1)
				throw new ArgumentException("saturation 0..1");
			if (brightness<0 || brightness>1)
				throw new ArgumentException("brightness 0..1");

			if (saturation==0)
			{
				// achromatische Farbe
				byte rgb = (byte)(brightness*255);
				return Color.FromArgb(rgb,rgb,rgb);
			}
			else
			{
				float fHexHue = (6.0f/360.0f) * hue;
				float fHexSector = (float)Math.Floor((double)fHexHue);
				float fHexSectorPos = fHexHue - fHexSector;

				float fBrightness = brightness*255.0f;
				float fSaturation = saturation;

				byte bWashOut = (byte)(0.5f + fBrightness*(1.0f-fSaturation));
				byte bHueModifierOddSector = (byte)(0.5f + fBrightness * (1.0f - fSaturation*fHexSectorPos));
				byte bHueModifierEvenSector = (byte)(0.5f + fBrightness * (1.0f - fSaturation*(1.0f-fHexSectorPos)));

				// RGB-Farben abhängig vom Sektor erzeugen
				switch ((int)fHexSector)
				{
					case 0:
						return Color.FromArgb((byte)(brightness*255),bHueModifierEvenSector,bWashOut);
					case 1:
						return Color.FromArgb(bHueModifierOddSector,(byte)(brightness*255),bWashOut);
					case 2:
						return Color.FromArgb(bWashOut,(byte)(brightness*255),bHueModifierEvenSector);
					case 3:
						return Color.FromArgb(bWashOut,bHueModifierOddSector,(int)(brightness*255));
					case 4:
						return Color.FromArgb(bHueModifierEvenSector,bWashOut,(byte)(brightness*255));
					case 5:
						return Color.FromArgb((byte)(brightness*255),bWashOut,bHueModifierOddSector);
					default:
						return Color.FromArgb(0,0,0);
				}
			}
		}
	}
}
