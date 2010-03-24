using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	public class FormDisabler : IDisposable
	{
		private static readonly object lockingObject = new object();			// Lock-Objekt für threadübergreifende Zugriffe
		private static readonly Dictionary<Control,int> nestingMap = new Dictionary<Control,int>();	// Verzeichnis für die Verschachtelungstiefen
		private readonly Control control;										// das Control, dessen Kindcontrols deaktiviert werden sollen
		private readonly List<Control> disabledControls = new List<Control>();


		public FormDisabler(Control control)
		{
			this.control = getRootControl(control);

			lock (FormDisabler.lockingObject)
				if (!nestingMap.ContainsKey(this.control))
					FormDisabler.invoke(control,delegate
					{
						foreach (Control childControl in this.control.Controls)
							if (childControl.Enabled)
							{
								this.disabledControls.Add(childControl);
								childControl.Enabled = false;
							}

						FormDisabler.nestingMap.Add(this.control,1);
					});
				else
					nestingMap[this.control]++;
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
		/// Freigeben des Objektes
		/// </summary>
		public void Dispose()
		{
			lock (FormDisabler.lockingObject)
				if (--FormDisabler.nestingMap[this.control]==0)
					FormDisabler.invoke(this.control,delegate
					{
						foreach (Control childControl in this.disabledControls)
							childControl.Enabled = true;

						FormDisabler.nestingMap.Remove(this.control);
					});
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
