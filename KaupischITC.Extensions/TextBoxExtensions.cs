using System.Drawing;
using System.Windows.Forms;

namespace KaupischITC.Extensions
{
	public static class TextBoxExtensions
	{
		/// <summary>
		/// Aktiviert die Möglichkeit, Text aufzunehmen, wenn dieser über das Eingabefeld gezogen wird
		/// </summary>
		/// <param name="textBox">das Eingabefeld</param>
		public static T EnableTextDrop<T>(this T textBox) where T : TextBoxBase
		{
			textBox.AllowDrop = true;

			textBox.DragEnter += delegate(object sender,DragEventArgs e)
			{
				if (e.Data.GetDataPresent(typeof(string)))
					e.Effect = DragDropEffects.Copy;
			};

			textBox.DragOver += delegate(object sender,DragEventArgs e)
			{
				if (e.Data.GetDataPresent(typeof(string)))
				{
					textBox.SelectionStart = textBox.GetCaretIndexFromPoint(new Point(e.X,e.Y));
					textBox.SelectionLength = 0;

					if (!textBox.Focused)
						textBox.Focus();
				}
			};

			textBox.DragDrop += delegate(object sender,DragEventArgs e)
			{
				if (e.Data.GetDataPresent(typeof(string)))
					textBox.SelectedText = (string)e.Data.GetData(typeof(string));
			};

			return textBox;
		}

		/// <summary>
		/// Ermittelt die Position des Carets für den angegebenen Punkt
		/// </summary>
		/// <param name="textBox">das Eingabefeld, dessen Caret-Position ermittelt werden soll</param>
		/// <param name="point">der Punkt, für den die Caret-Position ermittelt werden soll</param>
		/// <returns></returns>
		public static int GetCaretIndexFromPoint(this TextBoxBase textBox,Point point)
		{
			Point actualPoint = textBox.PointToClient(point);
			int index = textBox.GetCharIndexFromPosition(actualPoint);
			if (index == textBox.Text.Length - 1)
			{
				Point caretPoint = textBox.GetPositionFromCharIndex(index);
				if (actualPoint.X > caretPoint.X)
					index += 1;
			}
			return index;
		}
	}
}
