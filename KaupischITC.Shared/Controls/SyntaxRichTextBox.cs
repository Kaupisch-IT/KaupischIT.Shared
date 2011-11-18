using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace KaupischITC.Shared.Controls
{
	/// <summary>
	/// Stellt eine Textbox mit Syntaxhervorhebung dar
	/// </summary>
	public class SyntaxRichTextBox : RichTextBox
	{
		private bool isProcessing = false;	// gibt ab, ob die Syntaxhervorhebung gerade durchgeführt wird
		private string highlightedWord;		// gibt das momentan (durch eine Hintergrundfarbe) hervorgehobene Wort an
		private Timer timerCurrentWord = new Timer { Interval = 1000 };	// der Timer für die verzögerte Hervorhebung des momentanen Wortes 


		/// <summary>
		/// Enthält Informationen einer Hervorhebungsregel
		/// </summary>
		public class Rule
		{
			/// <summary> Gibt den Namen der Regel zurück oder legt diesen fest. </summary>
			public string Name { get; set; }

			/// <summary> Gibt den regulären Ausdruck für den hervorzuhebenden Textbereich zurück oder legt diesen fest. </summary>
			public Regex Regex { get; set; }

			/// <summary> Gibt die für die Hervorhebung zu verwendende Vordergrundfarbe zurück oder legt diese fest. </summary>
			public Color ForeColor { get; set; }
		}

		/// <summary>
		/// Gibt eine Auflistung der Hervorhebungsregeln zurück.
		/// </summary>
		public IList<Rule> Rules { get; private set; }


		/// <summary>
		/// Erstellt ein neues Steuerelement zum Hervorheben von Text
		/// </summary>
		public SyntaxRichTextBox()
		{
			this.Rules = new List<Rule>();
			this.InitializeRules();
			this.timerCurrentWord.Tick += this.OnTimerCurrentWordTick;
		}


		/// <summary>
		/// Initialisiert die Hervorhebungsregeln mit Standardwerten
		/// </summary>
		protected virtual void InitializeRules()
		{
			this
				.AddStyle("Numbers",Color.Magenta,@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfFmM]?\b|\b0x[a-fA-F\d]+\b")
				.AddStyle("Strings",Color.Brown,@"""[^""\\\r\n]*(?:\\.[^\""\\\r\n]*)*""")
				.AddStyle("Chars",Color.Brown,@"'\\?.'")
				.AddStyle("InitializedProperties",Color.FromArgb(43,145,175),@"\s[\w_][\w\d_]*(?=\s*= )")
				.AddStyle("InitializedProperties",Color.FromArgb(43,145,175),@"(?<=(^|,)[^,=]*\b([a-zA-Z_]\w*\.)+)([a-zA-Z_]\w*)(?=\s*(,|$))")
				.AddStyle("Comments",Color.Green,@"(//.*$)|(/\*.*?\*/)")
				.AddStyle("Keywords",Color.Blue,@"\b(as|base|bool|break|byte|char|decimal|default|delegate|double|false|float|int|is|long|new|null|object|ref|return|sbyte|short|string|this|throw|true|typeof|uint|ulong|ushort)\b");

		}


		/// <summary>
		/// Fügt dem Regelsatz eine neue Regel zur Syntaxhervorhebung hinzu
		/// </summary>
		/// <param name="name">der Name der Regel</param>
		/// <param name="foreColor">die zu verwendende Vordergrundfarbe</param>
		/// <param name="pattern">das Muster des regulären Ausdrucks für die hervorzuhebenden Textstellen</param>
		/// <returns>die SyntaxRichTextBox, der diese Regel hinzugefügt wurde </returns>
		public SyntaxRichTextBox AddStyle(string name,Color foreColor,string pattern)
		{
			this.Rules.Add(new Rule
			{
				ForeColor = foreColor,
				Regex = new Regex(pattern,RegexOptions.Compiled|RegexOptions.Multiline)
			});
			return this;
		}


		/// <summary>
		/// Verarbeitet Windows-Meldungen
		/// </summary>
		/// <param name="m">die zu verarbeitende Windows-Meldung</param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg==0x00f && this.isProcessing)
				// Flackern während der Syntaxhervorhebung unterdrücken
				m.Result = IntPtr.Zero;
			else
				base.WndProc(ref m);
		}


		/// <summary>
		/// Beim Verarbeiten des Textes die Syntaxhervorhebung für die aktuelle Zeile ausführen
		/// </summary>
		protected override void OnTextChanged(EventArgs e)
		{
			if (!this.isProcessing)
			{
				// (NICHT mit den GetIrgendwasLine-Methoden arbeiten, da diese auf den wirklich dargestellten Zeilen basieren - Achtung bei WordWrap)
				int lineStart = this.SelectionStart;
				while (lineStart>0 && this.Text[lineStart-1]!='\n')
					lineStart--;
				int lineEnd = this.SelectionStart;
				while (lineEnd<this.Text.Length && this.Text[lineEnd]!='\n')
					lineEnd++;

				this.ProcessText(lineStart,lineEnd);
			}
		}


		/// <summary>
		///  Ruft den aktuellen Text in der SyntaxTextBox ab oder legt diesen fest
		/// </summary>
		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				this.ProcessText();
			}
		}


		/// <summary>
		/// Führt die Syntaxhervorhebung für den gesamten Text durch 
		/// </summary>
		public void ProcessText()
		{
			this.ProcessText(0,this.Text.Length);
		}


		/// <summary>
		/// Führt die Syntaxhervorhebung für den angegebenen Textbereich aus
		/// </summary>
		/// <param name="startIndex">der Zeichenindex, an dem mit der Syntaxhervorhebung begonnen werden soll</param>
		/// <param name="endIndex">der Zeichenindex, bis zu dem die Syntaxhervorhebung durchgeführt werden soll</param>
		protected virtual void ProcessText(int startIndex,int endIndex)
		{
			this.isProcessing = true;
			using (new RichTextBoxSelectionKeeper(this))
			{
				// alle angewendeten Vordergrundfarben im gewünschten Textbereich entfernen
				this.SelectionStart = startIndex;
				this.SelectionLength = endIndex-startIndex;
				this.SelectionColor = this.ForeColor;

				// alle Regeln auf den gewünschten Textbereich anwenden
				string text = this.Text.Substring(startIndex,endIndex-startIndex);
				foreach (Rule style in this.Rules)
					foreach (Match match in style.Regex.Matches(text))
					{
						this.SelectionStart = startIndex + match.Index;
						this.SelectionLength = match.Length;
						this.SelectionColor = style.ForeColor;
					}
			}
			this.isProcessing = false;
		}


		/// <summary>
		/// Beim Drücken der Enter-Taste die nächste Zeile automatisch einrücken
		/// </summary>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (e.KeyChar==(char)Keys.Enter)
			{
				// den Text der vorhergehenden Zeile ermitteln
				int lineStart = this.SelectionStart-1;
				while (lineStart>0 && this.Text[lineStart-1]!='\n')
					lineStart--;
				string prevLineText = this.Text.Substring(lineStart,this.SelectionStart-lineStart);

				if (!String.IsNullOrEmpty(prevLineText))
				{
					// die führenden Leerzeichen/Tabs der letzten Zeile auch für die aktuelle Zeile übernehmen
					Match match = Regex.Match(prevLineText,@"^(?<space>[ \t]*)(?<bracket>\{?)");
					this.SelectedText = match.Groups["space"].Value;
					// ggf. nach einer öffnenden geschweiften Klammer eine zusätzliche Ebene einrücken
					if (match.Groups["bracket"].Value=="{")
						this.SelectedText = "   ";
				}
			}

			base.OnKeyPress(e);
		}


		/// <summary>
		/// Beim Tastendruck [Strg+V] (Einfügen) die Syntaxhervorhebung für den gesamten Text durchführen
		/// </summary>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.KeyData==(Keys.Control|Keys.V))
				this.ProcessText();
		}



		/// <summary>
		/// Hebt das aktuelle Wort verzögert hervor (I)
		/// </summary>
		protected override void OnSelectionChanged(EventArgs e)
		{
			if (!this.isProcessing && this.SelectionLength==0)
			{
				this.timerCurrentWord.Enabled = false;

				// Wortanfang suchen
				int lineStart = this.SelectionStart;
				while (lineStart>0 && SyntaxRichTextBox.nameCharRegex.IsMatch(this.Text[lineStart-1].ToString()))
					lineStart--;
				// Wortende suchen
				int lineEnd = this.SelectionStart;
				while (lineEnd<this.Text.Length && SyntaxRichTextBox.nameCharRegex.IsMatch(this.Text[lineEnd].ToString()))
					lineEnd++;

				// (nur das aktuelle Wort selektieren, wenn der Cursor mitten drin steht)
				string currentWord = (lineStart!=this.SelectionStart && lineEnd!=this.SelectionStart) ? this.Text.Substring(lineStart,lineEnd-lineStart) : "";
				if (this.highlightedWord!=currentWord)
				{
					this.highlightedWord = currentWord;

					this.isProcessing = true;
					using (new RichTextBoxSelectionKeeper(this))
					{
						// die Hintergrundfarbe des gesamten Textes zurücksetzen
						this.SelectionStart = 0;
						this.SelectionLength = this.Text.Length;
						this.SelectionBackColor = this.BackColor;

						// Timer für die verzögerte Worthervorhebung aktivieren
						this.timerCurrentWord.Enabled = true;
					}
					this.isProcessing = false;
				}
			}

			base.OnSelectionChanged(e);
		}
		private static readonly Regex nameCharRegex = new Regex(@"\w",RegexOptions.Compiled);


		/// <summary>
		/// Hebt das aktuelle Wort verzögert hervor (II)
		/// </summary>
		protected virtual void OnTimerCurrentWordTick(object sender,EventArgs e)
		{
			this.timerCurrentWord.Enabled = false;
			if (!this.isProcessing)
			{
				this.isProcessing = true;
				using (new RichTextBoxSelectionKeeper(this))
				{
					this.SelectionStart = 0;
					this.SelectionLength = this.Text.Length;
					this.SelectionBackColor = this.BackColor;

					// alle Vorkommen des aktuellen Wortes (als ganzes Wort) hervorheben
					foreach (Match match in Regex.Matches(this.Text,@"\b"+Regex.Escape(this.highlightedWord)+@"\b"))
					{
						this.SelectionStart = match.Index;
						this.SelectionLength = match.Length;
						this.SelectionBackColor = Color.FromArgb(255,242,128);
					}
				}
				this.isProcessing = false;
			}
		}
	}
}
