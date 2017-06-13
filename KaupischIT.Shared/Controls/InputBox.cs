using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischIT.Shared
{
	/// <summary>
	/// Stellt ein modales Eingabefenster für Text dar
	/// </summary>
	public partial class InputBox : Form
	{
		/// <summary>
		/// Gibt den Beschreibungstext für die geforderte Eingabe zurück oder legt diesen fest.
		/// </summary>
		public string Description
		{
			get { return this.labelMessage.Text; }
			set { this.labelMessage.Text = value; }
		}

		/// <summary>
		/// Gibt den Eingabetext zurück oder legt diesen fest.
		/// </summary>
		public string Value
		{
			get { return this.textBoxValue.Text; }
			set { this.textBoxValue.Text = value; }
		}

		/// <summary>
		/// Gibt die Funktion zurück, die bestimmt, ob OK geklickt werden kann, oder legt diese fest.
		/// </summary>
		public Func<string,bool> CanClickOK { get; set; }

		/// <summary>
		/// Gibt die Funktion zurück, die bestimmt, ob ein Zeichen eine gültige Eingabe ist, oder legt diese fest.
		/// </summary>
		public Func<char,bool> IsValidInputChar
		{
			get { return this.textBoxValue.IsValidChar; }
			set { this.textBoxValue.IsValidChar = value; }
		}


		/// <summary>
		/// Erstellt ein neues Fenster für Benutzereingaben von Text
		/// </summary>
		public InputBox()
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.CanClickOK = (value) => !String.IsNullOrEmpty(value);
			this.IsValidInputChar = _ => true;

			this.Shown += delegate { this.TextBoxValue_TextChanged(this,EventArgs.Empty); };
		}


		/// <summary>
		/// Klick auf "OK"
		/// </summary>
		private void ButtonOK_Click(object sender,EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// Klick auf "Abbrechen"
		/// </summary>
		private void ButtonCancel_Click(object sender,EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Eingabetext validieren, sobald er geändert wird
		/// </summary>
		private void TextBoxValue_TextChanged(object sender,EventArgs e)
		{
			this.buttonOK.Enabled = this.CanClickOK(this.textBoxValue.Text);
		}
	}
}
