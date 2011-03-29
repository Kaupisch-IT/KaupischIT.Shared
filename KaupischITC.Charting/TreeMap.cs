using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using KaupischITC.Extensions;

namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt Funktionen bereit, um ein Flächendiagramm zu erstellen.
	/// </summary>
	internal class TreeMap
	{
		/// <summary>
		/// Stellt Informationen über ein Element bereit
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
		public TreeMap()
		{
			this.Opacity = 80;
			this.LegendFont = new Font("Verdana",7.8f,FontStyle.Bold);
			this.LineHeight = 20;
			this.BarPadding = 5;
			this.Width = 500;
		}


		/// <summary>
		/// Zeichnet ein neues Flächendiagramm
		/// </summary>
		/// <param name="items">Flächen, die gezeichnet werden sollen.</param>
		/// <returns>Bitmap-Objekt, auf das das Kreisdiagramm gezeichnet wurde.</returns>
		public Bitmap Draw(List<Item> items)
		{
			if (items.Any(i => i.Value<0))
				throw new ArgumentException("Die Werte dürfen nicht negativ sein.");

			// Bild und Zeichenfläche für das Balkendiagramm erstellen
			Bitmap bitmap = new Bitmap(this.Width+2,this.Width+2);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				// weißer Hintergrund (sonst sieht die Schrift blöd aus)
				graphics.FillRectangle(Brushes.White,0,0,bitmap.Width,bitmap.Height);
				Rectangle rectangle = new Rectangle(0,0,bitmap.Width,bitmap.Height);
				this.DrawPanes(items,graphics,rectangle);
			}
			return bitmap;
		}



		/// <summary>
		/// Erstellt die Flächen für die übergebenen Elemente im angegeben Zeichenbereich
		/// </summary>
		/// <param name="items">die Flächen, die gezeichnet werden sollen</param>
		/// <param name="graphics">das Graphics-Objekt, auf das gezeichnet werden soll</param>
		/// <param name="rectangle">der Bereich, in dem Gezeichnet werden soll</param>
		private void DrawPanes(List<Item> items,Graphics graphics,Rectangle rectangle)
		{
			// einzelnes Element direkt zeichnen
			if (items.Count==1)
			{
				Item treeItem = items.Single();
				using (Pen pen = new Pen(treeItem.Color.ChangeBrightness(0.5f)))
				using (Brush foreBrush = new SolidBrush(treeItem.Color.ChangeBrightness(0.4f)))
				using (Brush backBrush = new SolidBrush(Color.FromArgb(this.Opacity,treeItem.Color)))
				{
					rectangle.Inflate(-1,-1); // 1-Pixel-Abstand von rechts und unten
					
					// Hintergrundfläche zeichnen
					Color backColor = Color.FromArgb(this.Opacity,treeItem.Color);
					using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rectangle,backColor.ChangeBrightness(0.6f),backColor,270f))
						graphics.FillRectangle(linearGradientBrush,rectangle);

					// Rahmen zeichnen
					graphics.DrawRectangle(pen,rectangle);

					// Beschriftung zeichnen
					using (StringFormat stringFormat = new StringFormat { Alignment = StringAlignment.Center,LineAlignment = StringAlignment.Center })
					{
						string text = treeItem.DisplayText+"\r\n"+treeItem.ValueText;
						if (rectangle.Height>rectangle.Width && graphics.MeasureString(text,this.LegendFont).Width>rectangle.Width)
							stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
						graphics.DrawString(text,this.LegendFont,foreBrush,rectangle,stringFormat);
					}
				}
			}
			// Elemente aufteilen, den Zeichenbereich entsprechend in zwei Teile teilen und rekursiv die Elemente zeichnen
			else if (items.Any())
			{
				double sum = items.Sum(b => b.Value);
				List<Item> firstBars = new List<Item>();
				List<Item> secondBars = new List<Item>();

				// die Elemente in zwei möglichst gleich große Teile splitten
				double current = 0;
				foreach (Item item in items)
					if ((current+=item.Value)<sum/2 || !firstBars.Any())
						firstBars.Add(item);
					else
						secondBars.Add(item);

				double factor = firstBars.Sum(b => b.Value) / sum; 

				if (rectangle.Width>rectangle.Height)
				{
					// in zwei Teile nebeneinander aufteilen
					int leftWidth = (int)(rectangle.Width*factor);
					this.DrawPanes(firstBars,graphics,new Rectangle(rectangle.X,rectangle.Y,leftWidth,rectangle.Height));
					this.DrawPanes(secondBars,graphics,new Rectangle(rectangle.X+leftWidth,rectangle.Y,rectangle.Width-leftWidth,rectangle.Height));
				}
				else
				{
					// in zwei Teile untereinander aufteilen
					int leftHeight = (int)(rectangle.Height*factor);
					this.DrawPanes(firstBars,graphics,new Rectangle(rectangle.X,rectangle.Y,rectangle.Width,leftHeight));
					this.DrawPanes(secondBars,graphics,new Rectangle(rectangle.X,rectangle.Y+leftHeight,rectangle.Width,rectangle.Height-leftHeight));
				}
			}
		}
	}
}
