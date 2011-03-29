using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using KaupischITC.Extensions;

namespace KaupischITC.Charting
{
	/// <summary>
	/// Stellt Funktionen bereit, um Kreisdiagramme zu erstellen
	/// </summary>
	public class PieChart
	{
		/// <summary>
		/// Stellt Informationen über ein Kuchenstück des Kreisdiagramms/"Kuchendiagramm" bereit
		/// </summary>
		public class PieSlice
		{
			/// <summary> Ruft die Beschriftung ab oder legt diese fest. </summary>
			public string Text { get; set; }

			/// <summary> Ruft den Winkel, bei dem das Kuchenstück anfängt, ab oder legt diesen fest. </summary>
			public float StartAngle { get; set; }

			/// <summary> Ruft den Winkel, bei dem das Kuchenstück aufhört, ab oder legt diesen fest. </summary>
			public float EndAngle { get; set; }

			/// <summary> Ruft den Winkel, der vom Kuchenstück aufgespannt wird, ab. </summary>
			public float SweepAngle { get { return (this.EndAngle-this.StartAngle+360)%360; } }

			/// <summary> Ruft die Farbe des Kuchenstücks ab oder legt diese fest. </summary>
			public Color Color { get; set; }

			/// <summary> Ruft die Verschiebung des Kuchenstücks aus der Mittelpunkt des Kreisdiagramms ab oder legt diese fest. </summary>
			public int Offset { get; set; }
		}
		

		/// <summary> Ruft die Breite des Kreisdiagramms ab oder legt diese fest. </summary>
		public int EllipseWidth { get; set; }

		/// <summary> Ruft die Höhe des Kreisdiagramms ab oder legt diese fest. </summary>
		public int EllipseHeight { get; set; }

		/// <summary> Ruft die Höhe das Kuchenstücke ("Kuchenstücke") ab oder legt diese fest. </summary>
		public int PieHeight { get; set; }

		/// <summary> Ruft die Durchlässigkeit der Farben der Flächen des Kreisdiagramms ab oder legt diese fest- </summary>
		public byte Opacity { get; set; }

		/// <summary> Ruft die Schriftart für die Anzeige von Text ab oder legt diese fest. </summary>
		public Font LegendFont { get; set; }

		/// <summary> Ruft die Entfernung der Beschriftung vom Rand eines Kuchenstücks ab oder legt diese fest. </summary>
		public int LegendDistance { get; set; }

		/// <summary> Ruft die Breite der gesamten Zeichenfläche ab. </summary>
		public int Width { get; private set; }

		/// <summary> Ruft die Höhe der gesamten Zeichenfläche ab. </summary>
		public int Height { get; private set; }
		

		/// <summary>
		/// Erzeugt ein neues PieChart-Objekt
		/// </summary>
		public PieChart()
		{
			this.LegendFont = new Font("Verdana",7.8f,FontStyle.Bold);
			this.LegendDistance = 20;
			this.EllipseWidth = 200;
			this.EllipseHeight = 100;
			this.Opacity = 130;
			this.PieHeight = 15;
		}
		

		/// <summary>
		/// Zeichnet ein neues Kreisdiagramm
		/// </summary>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen.</param>
		/// <returns>Bitmap-Objekt, auf das das Kreisdiagramm gezeichnet wurde.</returns>
		public Bitmap Draw(IList<PieSlice> pieSlices)
		{
			// (Henne-Ei-Problem: Um die Bitmap-Größe zu bestimmen, braucht man ein Graphics-Objekt, für ein Graphics-Objekt braucht man ein Bitmap)
			Bitmap bitmap = new Bitmap(1,1);
			Graphics graphics = Graphics.FromImage(bitmap);

			if (pieSlices.Count==0)
				return bitmap;
			// Höhe und Breite des Bitmaps anhand der Texte der Kuchenstücke, der Offsets und der Entfernung der Beschriftungen ermitteln
			int maxWidthOffset = pieSlices.Max(ps => ps.Offset) + this.LegendDistance + pieSlices.Max(ps => (int)graphics.MeasureString(ps.Text,this.LegendFont).Width);
			int maxHeightOffset = pieSlices.Max(ps => ps.Offset) + this.LegendDistance + pieSlices.Max(ps => (int)graphics.MeasureString(ps.Text,this.LegendFont).Height);
			this.Width = this.EllipseWidth + 2*maxWidthOffset + 1;
			this.Height = this.EllipseHeight + 2*maxHeightOffset + this.PieHeight + 1;

			// (so, jetzt das richtige Bild erstellen)
			graphics.Dispose();
			bitmap.Dispose();
			bitmap = new Bitmap(this.Width,this.Height);
			using (graphics = Graphics.FromImage(bitmap))
			{
				// weißer Hintergrund und AntiAlias
				graphics.FillRectangle(Brushes.White,0,0,this.Width,this.Height);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;

				// Kuchenstücke zeichnen
				graphics.TranslateTransform(maxWidthOffset,maxHeightOffset+this.PieHeight); // Koordinatensystem so schieben, dass die Beschriftungen links und oben ranpassen
				if (pieSlices.Count==1 && pieSlices[0].StartAngle==pieSlices[0].EndAngle)
					this.DrawLonelySlice(graphics,pieSlices[0]);		// 100%-Stück
				else
				{
					this.DrawBottoms(graphics,pieSlices);				// Kreisausschnitte unten
					this.DrawBackgroundSurfaces(graphics,pieSlices);	// hintere Seitenflächen
					this.DrawCuttingEdges(graphics,pieSlices);			// Schnittflächen
					this.DrawForegroundSurfaces(graphics,pieSlices);	// vordere Seitenflächen
					this.DrawTops(graphics,pieSlices);					// Kreisausschnitte oben
				}

				// Beschriftungen hinzufügen
				this.DrawTexts(graphics,pieSlices);
			}

			return bitmap;
		}
		

		/// <summary>
		/// Zeichnet ein einzelnes 100%-Stück
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlice">Kuchenstück, das gezeichnet werden soll</param>
		private void DrawLonelySlice(Graphics graphics,PieSlice pieSlice)
		{
			using (Pen pen = new Pen(pieSlice.Color.ChangeBrightness(0.8f)))
			using (Brush brush = new SolidBrush(Color.FromArgb(this.Opacity,pieSlice.Color)))
			{
				PointF sliceFocus = this.GetSliceFocus(pieSlice);

				// Fläche und Kontur unten
				graphics.FillEllipse(brush,sliceFocus.X,sliceFocus.Y,this.EllipseWidth,this.EllipseHeight);
				graphics.DrawEllipse(pen,sliceFocus.X,sliceFocus.Y,this.EllipseWidth,this.EllipseHeight);

				// Seitenflächen hinten und vorn
				this.DrawSurfaces(graphics,new PieSlice[] { pieSlice },ps => 180,ps => 360);
				this.DrawSurfaces(graphics,new PieSlice[] { pieSlice },ps => 0,ps => 180);

				// Fläche und Kontur oben
				graphics.FillEllipse(brush,sliceFocus.X,sliceFocus.Y-this.PieHeight,this.EllipseWidth,this.EllipseHeight);
				graphics.DrawEllipse(pen,sliceFocus.X,sliceFocus.Y-this.PieHeight,this.EllipseWidth,this.EllipseHeight);
			}
		}


		/// <summary>
		/// Zeichnet die Bodenflächen aller Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawBottoms(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			this.DrawPlanes(graphics,pieSlices,0);
		}

		/// <summary>
		/// Zeichnet die Oberseiten aller Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawTops(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			this.DrawPlanes(graphics,pieSlices,this.PieHeight);
		}


		/// <summary>
		/// Zeichnet die Kreisausschnittflächen der Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		/// <param name="altitude">relative Höhe der Flächen</param>
		private void DrawPlanes(Graphics graphics,IEnumerable<PieSlice> pieSlices,int altitude)
		{
			foreach (PieSlice pieSlice in pieSlices)
				using (Pen pen = new Pen(pieSlice.Color.ChangeBrightness(0.8f)))
				using (Brush brush = new SolidBrush(Color.FromArgb(this.Opacity,pieSlice.Color)))
				{
					// Mittelpunkt der Ellipse und transformierte Start-/Endwinkel ermitteln
					float startAngleT = TransformAngle(pieSlice.StartAngle);
					float sweepAngleT = (TransformAngle(pieSlice.EndAngle)-startAngleT+360)%360;
					PointF sliceFocus = this.GetSliceFocus(pieSlice);

					// Kreisausschnittfläche und Kontur zeichnen
					graphics.FillPie(brush,sliceFocus.X,sliceFocus.Y-altitude,this.EllipseWidth,this.EllipseHeight,startAngleT,sweepAngleT);
					graphics.DrawPie(pen,sliceFocus.X,sliceFocus.Y-altitude,this.EllipseWidth,this.EllipseHeight,startAngleT,sweepAngleT);
				}
		}
		

		/// <summary>
		/// Zeichnet alle Seitenflächen der Kuchenstücke, die im Hintergrund liegen
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawBackgroundSurfaces(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			// Seitenflächen ermitteln, die im Hintergrund liegen, und von hinten nach vorn sortieren
			IEnumerable<PieSlice> slicesWithBackground = pieSlices.Where(p => p.StartAngle>180 || p.EndAngle>180 || p.StartAngle>p.EndAngle);
			slicesWithBackground = slicesWithBackground.OrderBy(ps => Math.Abs(ps.StartAngle+ps.SweepAngle/2-270));

			// entsprechende Seitenflächen zeichnen (fangen frühestens bei 180° an und hören spätestens bei 360° auf)
			Func<PieSlice,float> start = (pieSlice) => Math.Max(180,pieSlice.StartAngle);
			Func<PieSlice,float> end = (pieSlice) => (pieSlice.EndAngle>180) ? pieSlice.EndAngle : 0;
			this.DrawSurfaces(graphics,slicesWithBackground,start,end);
		}

		/// <summary>
		/// Zeichnet alle Seitenflächen der Kuchenstücke, die im Vordergrund liegen
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawForegroundSurfaces(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			// Seitenflächen ermitteln, die im Vordergrund liegen, und von hinten nach vorn sortieren
			IEnumerable<PieSlice> slicesWithForeground = pieSlices.Where(p => p.StartAngle<180 || p.EndAngle<180 || p.StartAngle>p.EndAngle);
			slicesWithForeground = slicesWithForeground.OrderByDescending(ps => Math.Abs(ps.StartAngle+ps.SweepAngle/2-90));

			// entsprechende Seitenflächen zeichnen (fangen frühestens bei 0° an und hören spätestens bei 180° auf)
			Func<PieSlice,float> start = (pieSlice) => (pieSlice.StartAngle<180) ? pieSlice.StartAngle : 0;
			Func<PieSlice,float> end = (pieSlice) => Math.Min(180,pieSlice.EndAngle);
			this.DrawSurfaces(graphics,slicesWithForeground,start,end);
		}


		/// <summary>
		/// Zeichnet die Seitenflächen der Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		/// <param name="getStartAngle">Delegat zur Bestimmung des Startwinkels eines Kreisausschnitts.</param>
		/// <param name="getEndAngle">Delegat zur Bestimmung des Endwinkels eines Kreisausschnitts.</param>
		private void DrawSurfaces(Graphics graphics,IEnumerable<PieSlice> pieSlices,Func<PieSlice,float> getStartAngle,Func<PieSlice,float> getEndAngle)
		{
			foreach (PieSlice pieSlice in pieSlices)
				using (Pen pen = new Pen(pieSlice.Color.ChangeBrightness(0.8f)))
				{
					// Brush für die Seitenfläche erstellen
					ColorBlend colorBlend = new ColorBlend();
					Color color = Color.FromArgb(this.Opacity,pieSlice.Color);
					colorBlend.Colors = new Color[]
                    {
                        color.ChangeBrightness(0.5f),
                        color.ChangeBrightness(0.8f),
                        color,
                        color.ChangeBrightness(0.8f),
                        color.ChangeBrightness(0.5f)
                    };
					colorBlend.Positions = new float[] { 0f,0.15f,0.5f,0.85f,1.0f };
					using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Point(0,0),new Point(this.EllipseWidth,0),Color.Blue,Color.White))
					{
						linearGradientBrush.InterpolationColors = colorBlend;

						// Mittelpunkt und transformierte Start-/Endwinkel bestimmen
						PointF sliceFocus = this.GetSliceFocus(pieSlice);
						float startAngleT = TransformAngle(getStartAngle(pieSlice));
						float sweepAngleT = (TransformAngle(getEndAngle(pieSlice))-startAngleT+360)%360;

						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							// Path für die Seitenfläche erstellen
							graphicsPath.AddArc(sliceFocus.X,sliceFocus.Y,this.EllipseWidth,this.EllipseHeight,startAngleT,sweepAngleT);
							graphicsPath.AddArc(sliceFocus.X,sliceFocus.Y-this.PieHeight,this.EllipseWidth,this.EllipseHeight,startAngleT+sweepAngleT,-sweepAngleT);
							graphicsPath.CloseFigure();

							// Seitenfläche und Kontur zeichnen
							graphics.FillPath(linearGradientBrush,graphicsPath);
							graphics.DrawPath(pen,graphicsPath);
						}
					}
				}
		}

		
		/// <summary>
		/// Zeichnet die Schnittflächen der Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawCuttingEdges(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			// Auflistung der ganzen Seitenflächen der Kuchenstücke erstellen (ein Kuchenstück hat ja zwei Seitenflächen)
			var cuttingEdges = pieSlices.Select(ps => new { Angle = ps.StartAngle,PieSlice = ps }).Concat(pieSlices.Select(ps => new { Angle = ps.EndAngle,PieSlice = ps }));

			// Seitenflächen ermitteln, die auf der linken Seite des Kreisdiagramms liegen, von hinten nach vorn sortieren und zeichnen
			var leftCuttingEdges = cuttingEdges.Where(ce => ce.Angle>90 && ce.Angle<270).OrderByDescending(ce => ce.Angle).ThenByDescending(ce => ce.PieSlice.StartAngle);
			foreach (var cuttingEdge in leftCuttingEdges)
				this.DrawCuttingEdge(graphics,cuttingEdge.Angle,cuttingEdge.PieSlice);

			// Seitenflächen ermitteln, die auf der rechten Seite des Kreisdiagramms liegen, von hinten nach vorn sortieren und zeichnen
			var rightCuttingEdges = cuttingEdges.Where(ce => !(ce.Angle>90 && ce.Angle<270)).OrderBy(ce => (ce.Angle>90) ? ce.Angle : ce.Angle+360).ThenBy(ce => (ce.PieSlice.StartAngle>90) ? ce.PieSlice.StartAngle : ce.PieSlice.StartAngle+360);
			foreach (var cuttingEdge in rightCuttingEdges)
				this.DrawCuttingEdge(graphics,cuttingEdge.Angle,cuttingEdge.PieSlice);
		}

		/// <summary>
		/// Zeichnet eine Seitenfläche
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="angle">Winkel der Seitenfläche</param>
		/// <param name="pieSlice">Kuchenstück, zu dem die Seitenfläche gehört</param>
		private void DrawCuttingEdge(Graphics graphics,float angle,PieSlice pieSlice)
		{
			// Mittelpunkt bestimmen und Koordinatensystem so verschieben, dass eine Seite der Seitenfläche im Koordinatenursprung beginnt
			PointF sliceFocus = this.GetSliceFocus(pieSlice);
			PointF newPointOfOrigin = new PointF(sliceFocus.X+this.EllipseWidth/2,sliceFocus.Y+this.EllipseHeight/2);
			graphics.TranslateTransform(newPointOfOrigin.X,newPointOfOrigin.Y);
			{
				using (Pen pen = new Pen(pieSlice.Color.ChangeBrightness(0.8f)))
				using (Brush brush = new SolidBrush(Color.FromArgb(this.Opacity,pieSlice.Color)))
				{
					// Winkel und Radius/Länge der Seitenfläche bestimmen
					float angleT = TransformAngle(angle);
					float radius = this.GetEllipseRadius(angleT);

					// Koordinaten auf dem Außenkreis berechnen
					float x = (float)Math.Cos(Radian(angleT)) * radius;
					float y = (float)Math.Sin(Radian(angleT)) * radius;

					// Umriss der Seitenfläche erstellen, Fläche und Kontur zeichnen
					PointF[] points = new PointF[]
                    {
                        new PointF(0,0),
                        new PointF(0,-this.PieHeight),
                        new PointF(x,y-this.PieHeight),
                        new PointF(x,y)
                    };
					graphics.FillPolygon(brush,points);
					graphics.DrawPolygon(pen,points);
				}
			}
			// Koordinatensystem da hin schieben, wo es vorher war
			graphics.TranslateTransform(-newPointOfOrigin.X,-newPointOfOrigin.Y);
		}

		
		/// <summary>
		/// Zeichnet die Beschriftung der Kuchenstücke
		/// </summary>
		/// <param name="graphics">Zeichnungsoberfläche, auf die gezeichnet werden soll</param>
		/// <param name="pieSlices">Kuchenstücke, die gezeichnet werden sollen</param>
		private void DrawTexts(Graphics graphics,IEnumerable<PieSlice> pieSlices)
		{
			foreach (PieSlice pieSlice in pieSlices)
				if (!String.IsNullOrEmpty(pieSlice.Text))
					using (Pen pen = new Pen(pieSlice.Color.ChangeBrightness(0.5f)))
					using (Brush brush = new SolidBrush(pieSlice.Color.ChangeBrightness(0.5f)))
					{
						// Mittelpunkt und Winkel der Winkelhalbierenden des Kuchenstücks bestimmen                        
						PointF sliceFocus = this.GetSliceFocus(pieSlice);
						float bisectorAngleT = TransformAngle(pieSlice.StartAngle+pieSlice.SweepAngle/2);

						// (Vorberechnungen)
						float radius = this.GetEllipseRadius(bisectorAngleT);
						float sin = (float)Math.Sin(Radian(bisectorAngleT));
						float cos = (float)Math.Cos(Radian(bisectorAngleT));

						// die hinteren Kuchenstücke bekommen den Strich an die Oberkante, die vorderen an die Unterkante
						int altitude = (bisectorAngleT<178 && bisectorAngleT>2) ? 0 : this.PieHeight;

						// Koordinaten für den Strich bestimmen und diesen Zeichnen
						float x1 = cos*radius + this.EllipseWidth/2+sliceFocus.X;
						float y1 = sin*radius + this.EllipseHeight/2+sliceFocus.Y - altitude;
						float x2 = cos*(radius+this.LegendDistance) + this.EllipseWidth/2+sliceFocus.X;
						float y2 = sin*(radius+this.LegendDistance) + this.EllipseHeight/2+sliceFocus.Y - altitude;
						graphics.DrawLine(pen,x1,y1,x2,y2);

						// Schrift auf der linken Seite rechtsbündig und auf der rechten Seite linksbündig zeichnen
						using (StringFormat stringFormat = new StringFormat())
						{
							stringFormat.Alignment = (bisectorAngleT<90 || bisectorAngleT>270) ? StringAlignment.Near : StringAlignment.Far;
							stringFormat.LineAlignment = StringAlignment.Center;
							graphics.DrawString(pieSlice.Text,this.LegendFont,brush,new PointF(x2,y2),stringFormat);
						}
					}
		}

		
		/// <summary>
		/// Transformiert einen Winkel in einem Kreis so, dass die Flächenverhältnisse in der perspektivischen Projektion (Ellipse) gewahrt bleiben
		/// </summary>
		/// <param name="angle">Winkel aus dem Kreis</param>
		/// <returns>Winkel für eine Ellipse, die dem perspektivisch projizierten Kreis entspricht</returns>
		private float TransformAngle(float angle)
		{
			double x = this.EllipseWidth * Math.Cos(Radian(angle));
			double y = this.EllipseHeight * Math.Sin(Radian(angle));
			return (float)(Math.Atan2(y,x)*180/Math.PI + 360)%360;
		}
		

		/// <summary>
		/// Berechnet den Winkel im Bogenmaß
		/// </summary>
		/// <param name="degree">der Winkel im Gradmaß</param>
		/// <returns>den Winkel im Bogenmaß</returns>
		private static float Radian(float degree)
		{
			return (float)(degree*Math.PI/180f);
		}
		

		/// <summary>
		/// Ermittelt den Mittelpunkt eines Kreisausschnitts (also den Mittelpunkt des eigentlichen Kreises)
		/// </summary>
		/// <param name="pieSlice">der Kreisausschnitt, dessen Mittelpunkt bestimmt werden soll</param>
		/// <returns>den Mittelpunkt des Kreisausschnitts</returns>
		private PointF GetSliceFocus(PieSlice pieSlice)
		{
			// Winkelhalbierende bestimmen...
			float startAngleT = TransformAngle(pieSlice.StartAngle);
			float sweepAngleT = (TransformAngle(pieSlice.EndAngle)-startAngleT+360)%360;
			float bisectorAngleT = startAngleT + sweepAngleT/2;

			// ... und entsprechend des Offsets dort entlang verschieben
			float x = (float)Math.Cos(Radian(bisectorAngleT))*pieSlice.Offset;
			float y = (float)Math.Sin(Radian(bisectorAngleT))*pieSlice.Offset;
			return new PointF(x,y);
		}
		

		/// <summary>
		/// Bestimmt den "Radius" einer Ellipse an einem bestimmten Winkel
		/// </summary>
		/// <param name="angle">der Winkel zur Bestimmung des "Radius"</param>
		/// <returns>"Radius" einer Ellipse</returns>
		private float GetEllipseRadius(float angle)
		{
			float a = this.EllipseWidth/2;
			float b = this.EllipseHeight/2;
			return (float)(b / Math.Sqrt(1-(1-(b*b)/(a*a))*Math.Pow(Math.Cos(Radian(angle)),2)));
		}
	}
}
