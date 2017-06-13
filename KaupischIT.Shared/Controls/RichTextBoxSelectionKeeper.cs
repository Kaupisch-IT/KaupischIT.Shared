using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischIT.Shared.Controls
{
	/// <summary>
	/// Sorgt dafür, die die momentane Texteinstellung einer RichTextBox erhalten bleibt
	/// </summary>
	public class RichTextBoxSelectionKeeper : IDisposable
	{
		public RichTextBox RichTextBox { get; private set; }

		public int SelectionStart { get; private set; }
		public int SelectionLength { get; private set; }
		public Color SelectionColor { get; private set; }
		public Color SelectionBackColor { get; private set; }


		/// <summary>
		/// Speichert den Zustand der Texteinstellungen der übergebenen RichTextBox
		/// </summary>
		/// <param name="richTextBox">die RichTextBox, deren Einstellungen gespeichert werden sollen</param>
		public RichTextBoxSelectionKeeper(RichTextBox richTextBox)
		{
			this.RichTextBox = richTextBox;
			this.SelectionStart = richTextBox.SelectionStart;
			this.SelectionLength = richTextBox.SelectionLength;
			this.SelectionColor = richTextBox.SelectionColor;
			this.SelectionBackColor = richTextBox.SelectionBackColor;
		}


		/// <summary>
		/// Stellt die anfangs gespeicherten Einstellungen wieder her
		/// </summary>
		public void Dispose()
		{
			this.RichTextBox.SelectionStart = this.SelectionStart;
			this.RichTextBox.SelectionLength = this.SelectionLength;
			this.RichTextBox.SelectionColor = this.SelectionColor;
			this.RichTextBox.SelectionBackColor = this.SelectionBackColor;
		}
	}
}
