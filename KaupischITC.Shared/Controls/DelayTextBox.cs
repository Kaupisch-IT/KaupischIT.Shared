﻿using System;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Eine Textbox, die schnelle Eingaben berücksichtigt und das TextChanged-Ereignis erst auslöst, wenn nach der letzten Eingabe eine bestimmte Zeit verstrichen ist
	/// </summary>
	public class DelayTextBox : TextBox
	{
		private Timer timer = new Timer();	// Timer, der nach einer bestimmten Zeit das OnTextChanged-Ereignis auslöst


		/// <summary>
		/// Die Zeitspanne (in ms), nach einer Texteingabe ohne erneute Eingabe vergehen muss, bis das OnTextChanged-Ereignis ausgelöst wird
		/// </summary>
		public int TextChangedDelay
		{
			get { return this.timer.Interval; }
			set { this.timer.Interval = value; }
		}


		/// <summary>
		/// Erstellt eine neue Textbox, die schnelle Eingaben berücksichtigt und das OnTextChanged-Ereignis erst auslöst, wenn nach der letzten Eingabe eine bestimmte Zeit verstrichen ist
		/// </summary>
		public DelayTextBox()
		{
			this.timer.Tick += new EventHandler(timer_Tick);
		}



		/// <summary>
		/// Timerablauf, der das OnTextChanged-Event auslöst
		/// </summary>
		private void timer_Tick(object sender,EventArgs e)
		{
			this.timer.Enabled = false;
			base.OnTextChanged(new EventArgs());
		}



		/// <summary>
		/// OnTextChanged abfangen und Timer (neu)starten
		/// </summary>
		protected override void OnTextChanged(EventArgs e)
		{
			this.timer.Enabled = false;
			this.timer.Enabled = true;
		}
	}
}