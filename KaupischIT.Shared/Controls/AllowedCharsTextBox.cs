using System;
using System.Text;
using System.Windows.Forms;

namespace KaupischIT.Shared.Controls
{
	/// <summary>
	/// Stellt ein Textfeld-Steuerelement dar, in dem nur bestimmte Zeichen als Eingabe zugelassen werden
	/// </summary>
	public class AllowedCharsTextBox : TextBox
	{
		private Func<char,bool> isValidChar;
		private string lastValidText = "";
		private int lastValidSelectionStart = 0;
		private int lastValidSelectionLength = 0;
		private bool isValidating = false;

		public Func<char,bool> IsValidChar
		{
			get { return this.isValidChar; }
			set
			{
				this.isValidChar = value;
				this.Text = this.Text;
			}
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				StringBuilder onlyValid = new StringBuilder();
				if (value!=null)
					foreach (char ch in value)
						if (this.IsValidChar!=null && this.IsValidChar(ch))
							onlyValid.Append(ch);

				base.Text = onlyValid.ToString();
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (this.isValidChar!=null && !this.IsValidChar(e.KeyChar) && !Char.IsControl(e.KeyChar))
				e.Handled = true;

			base.OnKeyPress(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if (this.isValidating)
				return;

			try
			{
				this.isValidating = true;

				foreach (char ch in base.Text)
					if (this.IsValidChar!=null && !this.IsValidChar(ch))
					{
						base.Text = this.lastValidText;
						this.SelectionStart = this.lastValidSelectionStart;
						this.SelectionLength = this.lastValidSelectionLength;
						return;
					}

				this.lastValidText = base.Text;
				this.lastValidSelectionStart = this.SelectionStart;
				this.lastValidSelectionLength = this.SelectionLength;
				base.OnTextChanged(e);
			}
			finally
			{
				this.isValidating = false;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			this.lastValidSelectionStart = this.SelectionStart;
			this.lastValidSelectionLength = this.SelectionLength;
			base.OnClick(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (base.Text == this.lastValidText)
			{
				this.lastValidSelectionStart = this.SelectionStart;
				this.lastValidSelectionLength = this.SelectionLength;
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (base.Text == this.lastValidText)
			{
				this.lastValidSelectionStart = this.SelectionStart;
				this.lastValidSelectionLength = this.SelectionLength;
			}
			base.OnKeyUp(e);
		}
	}
}
