using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using KaupischITC.Extensions;

namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt Methoden bereit, um ein Balkendiagramm zu erstellen.
	/// </summary>
	internal class BarChart
	{
		/// <summary>
		/// Stellt Informationen über einen Balken im Balkendiagram bereit
		/// </summary>
		public class Item
		{
			/// <summary> Gibt den Anzeigetext zurück oder legt diesen fest. </summary>
			public string DisplayText { get; set; }

			/// <summary> Gibt den Text für den Wert zurück oder legt diesen fest. </summary>
			public string ValueText { get; set; }

			/// <summary> Gibt den Wert zurück oder legt diesen fest. </summary>
			public double Value { get; set; }

			/// <summary> Gibt die Farbe für den Balken zurück oder legt diese fest </summary>
			public Color Color { get; set; }
		}



		/// <summary> Ruft die Breite des Balkendiagramms ab oder legt diese fest. </summary>
		public int Width { get; set; }

		/// <summary> Ruft die Schriftart für die Anzeige von Text ab oder legt diese fest. </summary>
		public Font LegendFont { get; set; }

		/// <summary> Ruft die Durchlässigkeit der Farben der Flächen der dargestellten Balken ab oder legt diese fest. </summary>
		public byte Opacity { get; set; }

		/// <summary> Ruft die Höhe der einzelnen Zeilen im Balkendiagramm ab oder legt diese fest. </summary>
		public byte LineHeight { get; set; }

		/// <summary> Ruft den Außenabstand eines dargestellten Balkens ab oder legt diesen fest. </summary>
		public byte BarPadding { get; set; }



		/// <summary>
		/// Erstellt ein neues BarChart-Objekt
		/// </summary>
		public BarChart()
		{
			this.Opacity = 130;
			this.LegendFont = new Font("Verdana",7.8f,FontStyle.Bold);
			this.LineHeight = 20;
			this.BarPadding = 5;
			this.Width = 500;
		}


		/// <summary>
		/// Zeichnet ein neues Balkendiagramm
		/// </summary>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen.</param>
		/// <returns>Bitmap-Objekt, auf das das Kreisdiagramm gezeichnet wurde.</returns>
		public Bitmap Draw(List<Item> bars)
		{
			// Bild und Zeichenfläche für das Balkendiagramm erstellen
			Bitmap bitmap = new Bitmap(this.Width,(bars.Count+1)*this.LineHeight);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				// weißer Hintergrund (sonst sieht die Schrift blöd aus)
				graphics.FillRectangle(Brushes.White,0,0,bitmap.Width,bitmap.Height);

				// Maximalwerte ermitteln (dargestellte Werte, Breite der Beschriftungen & Balkenbreite)
				double maxPositiveValue = Math.Max(0,bars.Max(ps => ps.Value));
				double maxNegativeValue = Math.Abs(Math.Min(0,bars.Min(ps => ps.Value)));
				double range = maxPositiveValue + maxNegativeValue;
				int maxDisplayTextWidth = bars.Max(bar => (int)graphics.MeasureString(bar.DisplayText,this.LegendFont).Width);
				int maxValueTextWidth = bars.Max(bar => (int)graphics.MeasureString(bar.ValueText,this.LegendFont).Width);
				int availableBarWidth = bitmap.Width-maxDisplayTextWidth-maxValueTextWidth-2*this.BarPadding;
				int baseLine = (int)(maxDisplayTextWidth + this.BarPadding + (maxNegativeValue/range)*availableBarWidth);

				// gepunktete Null-Linie zeichnen
				using (Pen pen = new Pen(Color.DarkGray) { DashStyle = DashStyle.Dot })
					graphics.DrawLine(pen,baseLine,this.LineHeight,baseLine,bitmap.Height-this.LineHeight);

				// alle Balken zeichnen
				for (int i=0;i<bars.Count;i++)
				{
					Item bar = bars[i];

					using (Pen pen = new Pen(bar.Color.ChangeBrightness(0.5f)))
					using (Brush brush = new SolidBrush(bar.Color.ChangeBrightness(0.5f)))
					{
						// Abstand von oberen Rand und Breite des zu zeichnenden Balkens ermitteln
						int top = i*this.LineHeight+this.LineHeight/2;
						int barWidth = (int)(availableBarWidth * (Math.Abs(bar.Value)/range));

						// den eigentlichen Balken zeichnen
						Rectangle rectangle = new Rectangle
						{
							X = (bar.Value>=0) ? baseLine : baseLine-barWidth,
							Y = top+this.BarPadding,
							Width = Math.Max(1,barWidth),
							Height = this.LineHeight-2*this.BarPadding
						};
						Color backColor = Color.FromArgb(this.Opacity,bar.Color);
						using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle,backColor.ChangeBrightness(0.6f),backColor,0f))
							graphics.FillRectangle(linearGradientBrush,rectangle);	// Hintergrund mit Farbverlauf
						graphics.DrawRectangle(pen,rectangle);						// Rahmen

						// Text für Bezeichnung 
						using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Far,LineAlignment = StringAlignment.Center })
							graphics.DrawString(bar.DisplayText,this.LegendFont,brush,new PointF(maxDisplayTextWidth,top+this.LineHeight/2),stringFormat);
						// Text für Werte
						using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Near,LineAlignment = StringAlignment.Center })
						{
							int left = (bar.Value>=0) ? baseLine+barWidth+2*this.BarPadding : baseLine+this.BarPadding;
							graphics.DrawString(bar.ValueText,this.LegendFont,brush,new PointF(left,top+this.LineHeight/2),stringFormat);
						}
					}
				}
			}
			return bitmap;
		}
	}
}
