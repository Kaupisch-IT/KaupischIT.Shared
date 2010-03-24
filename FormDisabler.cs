using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	public class FormDisabler : IDisposable
	{
		private readonly Control control;	// das Control für das der Cursor geändert werden soll 
		private readonly bool rawState;
		private readonly List<Control> changedControls = new List<Control>();


		public FormDisabler(Control control)
		{
			this.control = getRootControl(control);
			this.rawState = this.control.Enabled;

			foreach (Control childControl in this.control.Controls)
				if (childControl.Enabled)
				{
					this.changedControls.Add(childControl);
					childControl.Enabled = false;
				}
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
			foreach (Control childControl in this.changedControls)
				childControl.Enabled = this.rawState;
		}
	}
}
