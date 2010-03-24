using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Bietet durch Implementation von IDisposable eine kompakte Möglichkeit, die UseWaitCursor-Eigenschaft eines Fensters für eine bestimmte Zeit ändern und danach wieder automatisch zurückzusetzen.
	/// </summary>
	public class WaitCursorChanger : IDisposable
	{
		private static readonly object lockingObject = new object();								// Lock-Objekt für threadübergreifende Zugriffe
		private static readonly Dictionary<Control,int> nestingMap = new Dictionary<Control,int>();	// Verzeichnis für die Verschachtelungstiefen 
		private readonly Control control;															// das Control für das der Cursor geändert werden soll 


		/// <summary>
		/// Erstellt ein neues WaitCursorChanger-Objekt und setzt die UseWaitCursor-Eigenschaft des angegbeben Controls auf true.
		/// Sobald alle erstellten WaitCursor-Objekte zu einem Control freigegeben wurden, wird die UseWaitCursor-Eigenschaft wieder auf den Ursprungswert gesetzt
		/// </summary>
		/// <param name="control"></param>
		public WaitCursorChanger(Control control)
		{
			this.control = getRootControl(control);
			lock (WaitCursorChanger.lockingObject)
				if (!WaitCursorChanger.nestingMap.ContainsKey(this.control))
					WaitCursorChanger.invoke(control,delegate
					{
						this.control.UseWaitCursor = true;
						this.control.Cursor = Cursors.WaitCursor;
						WaitCursorChanger.nestingMap.Add(this.control,1);
					});
				else
					WaitCursorChanger.nestingMap[this.control]++;
		}



		/// <summary>
		/// Freigeben des Objektes
		/// </summary>
		public void Dispose()
		{
			lock (WaitCursorChanger.lockingObject)
				if (--WaitCursorChanger.nestingMap[this.control]==0)
					WaitCursorChanger.invoke(this.control,delegate
					{
						control.UseWaitCursor = false;
						this.control.Cursor = Cursors.Default;
						WaitCursorChanger.nestingMap.Remove(this.control);
					});
		}



		/// <summary>
		/// Ermittelt das Root-Control
		/// </summary>
		/// <param name="control">Control, dessen Root-Control ermittelt werden soll</param>
		/// <returns>das Root-Control des übergebenen Controls</returns>
		private Control getRootControl(Control control)
		{
			return (control.Parent==null) ? control : getRootControl(control.Parent);
		}



		/// <summary>
		/// Hilfsmethode zum Delegieren einer Aktion in den GUI-Thread
		/// </summary>
		/// <param name="control">ein Control (das der GUI-Thread erstellt hat)</param>
		/// <param name="action">die Aktion, die ausgeführt werden soll</param>
		private static void invoke(Control control,Action action)
		{
			if (!control.IsDisposed)
				if (control.InvokeRequired)
					control.Invoke((MethodInvoker)delegate { action(); });
				else
					action();
		}
	}
}