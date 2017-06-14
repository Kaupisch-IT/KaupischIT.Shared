using System;
using System.Windows.Forms;

namespace KaupischIT.Shared
{
	/// <summary>
	/// Eine Textbox, die schnelle Eingaben berücksichtigt und das TextChanged-Ereignis erst auslöst, wenn nach der letzten Eingabe eine bestimmte Zeit verstrichen ist
	/// </summary>
	public class DelayTextBox : TextBox
	{
		private Timer timer = new Timer();  // Timer, der nach einer bestimmten Zeit das OnTextChanged-Ereignis auslöst


		/// <summary>
		/// Die Zeitspanne (in ms), nach einer Texteingabe ohne erneute Eingabe vergehen muss, bis das OnTextChanged-Ereignis ausgelöst wird
		/// </summary>
		public int TextChangedDelay
		{
			get => this.timer.Interval;
			set => this.timer.Interval = value;
		}


		/// <summary>
		/// Erstellt eine neue Textbox, die schnelle Eingaben berücksichtigt und das OnTextChanged-Ereignis erst auslöst, wenn nach der letzten Eingabe eine bestimmte Zeit verstrichen ist
		/// </summary>
		public DelayTextBox() => this.timer.Tick += this.OnTimerTick;

		/// <summary>
		/// Timerablauf, der das OnTextChanged-Event auslöst
		/// </summary>
		private void OnTimerTick(object sender,EventArgs e)
		{
			this.timer.Enabled = false;
			base.OnTextChanged(new EventArgs());
		}


		/// <summary>
		/// OnTextChanged abfangen und Timer (neu)starten
		/// </summary>
		protected override void OnTextChanged(EventArgs e)
		{
			if (this.Focused)
			{
				this.timer.Enabled = false;
				this.timer.Enabled = true;
			}
			else
				base.OnTextChanged(e);
		}


		/// <summary>
		/// Vor dem Leave ggf. ein ausstehendes TextChanged auslösen
		/// </summary>
		protected override void OnLeave(EventArgs e)
		{
			if (this.timer.Enabled)
				this.OnTimerTick(this,e);
			base.OnLeave(e);
		}
	}
}
