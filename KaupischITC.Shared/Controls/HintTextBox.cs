using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Stellt ein Textbox-Steuerelement dar, dass einen Hinweistext und ein Bild darstellt, wenn der eingegebene Text leer ist und das Textfeld nicht den Fokus hat
	/// </summary>
	public class HintTextBox : TextBox
	{
		/// <summary>
		/// Gibt das anzuzeigende Bild zurück oder legt dieses fest.
		/// </summary>
		public Image EmptyImage { get; set; }

		/// <summary>
		/// Gibt den anzuzeigenden Leertext zurück oder legt diesen fest
		/// </summary>
		public string EmptyText
		{
			get { return this.emptyText; }
			set
			{
				this.emptyText = value;
				this.Invalidate();
			}
		}
		private string emptyText;


		/// <summary>
		/// Das die OnPaint-Methode nicht aufgerufen wird, selbst nach dem passenden Zeitpunkt im Zeichenprozess suchen
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg==15) // WM_PAINT
				this.CustomPaint();
		}

		/// <summary>
		/// Ggf Leertext und Bild in die Textbox zeichnen
		/// </summary>
		private void CustomPaint()
		{
			if (!this.Focused && String.IsNullOrEmpty(this.Text))
			{
				Rectangle bounds = this.ClientRectangle;
				using (BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(),bounds))
				using (Graphics graphics = bufferedGraphics.Graphics)
				{
					graphics.FillRectangle(SystemBrushes.Window,bounds); // Hintergrund

					// Leertext
					if (!String.IsNullOrEmpty(this.EmptyText))
					{
						TextFormatFlags textFormatFlags = TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.PathEllipsis;
						TextRenderer.DrawText(graphics,this.EmptyText,this.Font,new Rectangle(bounds.X,bounds.X,bounds.Width-((this.EmptyImage!=null)?this.EmptyImage.Width:0),bounds.Height),SystemColors.GrayText,textFormatFlags);
					}

					// Bild
					if (this.EmptyImage!=null)
						graphics.DrawImage(this.EmptyImage,bounds.Width-this.EmptyImage.Width,(bounds.Height-this.EmptyImage.Height)/2);

					bufferedGraphics.Render();
				}
			}
		}
	}
}
