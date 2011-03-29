using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using KaupischITC.Extensions;

namespace KaupischITC.Charting
{
	public class TreeMap
	{
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
		public TreeMap()
		{
			this.Opacity = 80;
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
		public Bitmap Draw(List<BarChart.Bar> bars)
		{
			// Bild und Zeichenfläche für das Balkendiagramm erstellen
			Bitmap bitmap = new Bitmap(this.Width+2,this.Width+2);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				// weißer Hintergrund (sonst sieht die Schrift blöd aus)
				graphics.FillRectangle(Brushes.White,0,0,bitmap.Width,bitmap.Height);

				//bars = bars.OrderByDescending(b => b.Value).ToList();
				Rectangle rectangle = new Rectangle(0,0,bitmap.Width,bitmap.Height);

				this.DrawPanes(bars,graphics,rectangle,false);
			}
			return bitmap;
		}


		private void DrawPanes(List<BarChart.Bar> bars,Graphics graphics,Rectangle rectangle,bool horizontal)
		{
			if (bars.Count==1)
			{
				BarChart.Bar bar = bars.Single();
				using (Pen pen = new Pen(bar.Color.ChangeBrightness(0.5f)))
				using (Brush foreBrush = new SolidBrush(bar.Color.ChangeBrightness(0.4f)))
				using (Brush backBrush = new SolidBrush(Color.FromArgb(this.Opacity,bar.Color)))
				{
					rectangle.Inflate(-1,-1);
					
					//graphics.FillRectangle(backBrush,rectangle);
					Color backColor = Color.FromArgb(this.Opacity,bar.Color);
					using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle,backColor.ChangeBrightness(0.6f),backColor,270f))
						graphics.FillRectangle(linearGradientBrush,rectangle);

					graphics.DrawRectangle(pen,rectangle);

					using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Center })
					{
						string text = bar.DisplayText+"\r\n"+bar.ValueText;
						if (rectangle.Height>rectangle.Width && graphics.MeasureString(text,this.LegendFont).Width>rectangle.Width)
							stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;

						graphics.DrawString(text,this.LegendFont,foreBrush,rectangle,stringFormat);
						//	graphics.DrawString(bar.DisplayText+"\r\n"+bar.ValueText,this.LegendFont,foreBrush,new Point(rectangle.X+rectangle.Width/2,rectangle.Y+rectangle.Height/2),stringFormat);
					}
				}
			}
			else if (bars.Any())
			{
				double sum = bars.Sum(b => b.Value);
				var leftBars = new List<BarChart.Bar>();
				var rightBars = new List<BarChart.Bar>();

				double current = 0;
				foreach (BarChart.Bar bar in bars)
					if ((current+=bar.Value)<sum/2 || !leftBars.Any())
						leftBars.Add(bar);
					else
						rightBars.Add(bar);

				double factor = leftBars.Sum(b => b.Value) / sum;

				//if (horizontal)
				if (rectangle.Width>rectangle.Height)
				{
					int leftWidth = (int)(rectangle.Width*factor);
					this.DrawPanes(leftBars,graphics,new Rectangle(rectangle.X,rectangle.Y,leftWidth,rectangle.Height),!horizontal);
					this.DrawPanes(rightBars,graphics,new Rectangle(rectangle.X+leftWidth,rectangle.Y,rectangle.Width-leftWidth,rectangle.Height),!horizontal);
				}
				else
				{
					int leftHeight = (int)(rectangle.Height*factor);
					this.DrawPanes(leftBars,graphics,new Rectangle(rectangle.X,rectangle.Y,rectangle.Width,leftHeight),!horizontal);
					this.DrawPanes(rightBars,graphics,new Rectangle(rectangle.X,rectangle.Y+leftHeight,rectangle.Width,rectangle.Height-leftHeight),!horizontal);
				}
			}
		}
	}
}
