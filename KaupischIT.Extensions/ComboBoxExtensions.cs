using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischIT.Extensions
{
	public static class ComboBoxExtensions
	{
		/// <summary>
		/// Aktiviert die Möglichkeit, Text aufzunehmen, wenn dieser über das Eingabefeld gezogen wird
		/// </summary>
		/// <param name="comboBox">das Eingabefeld</param>
		public static T EnableTextDropComboBox<T>(this T comboBox,Func<string,string> modifyDropText = null) where T : ComboBox
		{
			comboBox.AllowDrop = true;
			if (modifyDropText==null)
				modifyDropText = t => t;

			comboBox.DragEnter += delegate (object sender,DragEventArgs e)
			{
				if (comboBox.DropDownStyle==ComboBoxStyle.DropDownList)
					e.Effect = DragDropEffects.None;
				else
				{
					string text = modifyDropText((string)e.Data.GetData(typeof(string)));
					if (!String.IsNullOrEmpty(text))
						e.Effect = DragDropEffects.Move;
				}
			};

			comboBox.DragOver += delegate (object sender,DragEventArgs e)
			{
				if (comboBox.DropDownStyle!=ComboBoxStyle.DropDownList)
				{
					string text = modifyDropText((string)e.Data.GetData(typeof(string)));
					if (!String.IsNullOrEmpty(text))
					{
						comboBox.SelectionStart = comboBox.GetCaretIndexFromPoint(new Point(e.X,e.Y));
						comboBox.SelectionLength = 0;

						if (!comboBox.Focused)
							comboBox.Focus();
					}
				}
			};

			comboBox.DragDrop += delegate (object sender,DragEventArgs e)
			{
				if (comboBox.DropDownStyle!=ComboBoxStyle.DropDownList)
				{
					string text = modifyDropText((string)e.Data.GetData(typeof(string)));
					if (!String.IsNullOrEmpty(text))
						comboBox.SelectedText = text;
				}
			};

			return comboBox;
		}

		/// <summary>
		/// Ermittelt die Position des Carets für den angegebenen Punkt
		/// </summary>
		/// <param name="comboBox">das Eingabefeld, dessen Caret-Position ermittelt werden soll</param>
		/// <param name="point">der Punkt, für den die Caret-Position ermittelt werden soll</param>
		/// <returns></returns>
		public static int GetCaretIndexFromPoint(this ComboBox comboBox,Point point)
		{
			Point actualPoint = comboBox.PointToClient(point);
			for (int i=0;i<comboBox.Text.Length;i++)
				// nicht ganz tolle Lösung (funktioniert komisch, wenn mehr Text in der ComboBox steht, als sie breit ist - aber besser als nix...
				if (TextRenderer.MeasureText(comboBox.Text.Substring(0,i),comboBox.Font).Width>actualPoint.X)
					return i;

			return comboBox.Text.Length;
		}
	}
}
