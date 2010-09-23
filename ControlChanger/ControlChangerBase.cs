using System;
using System.Collections.Generic;
using System.Windows.Forms;

[assembly: KaupischITC.Shared.Subversion("$Id$")]
namespace KaupischITC.Shared
{
	/// <summary>
	/// Bietet durch Implementation von IDisposable eine kompakte Möglichkeit, Eigenschaften eines Controls für eine bestimmte Zeit ändern und danach wieder automatisch zurückzusetzen.
	/// </summary>
	[Subversion("$Id$")]
	public abstract class ControlChangerBase : IDisposable
	{
		private static readonly object lockingObject = new object();	// Lock-Objekt für threadübergreifende Zugriffe
		private static readonly Dictionary<Type,Dictionary<Control,int>> nestingMap = new Dictionary<Type,Dictionary<Control,int>>();	// Verzeichnis für die Verschachtelungstiefen 
		private readonly Control baseControl;							// das Basiscontrol des übergebenen Controls
		private volatile bool isDisposed = false;						// gibt an, schon disposed wurde


		/// <summary>
		/// Erstellt ein neues ControlChanger-Objekt
		/// </summary>
		/// <param name="control">das betroffene Control</param>
		public ControlChangerBase(Control control)
		{
			this.baseControl = getRootControl(control);
			Type type = this.GetType();
			lock (ControlChangerBase.lockingObject)
			{
				if (!ControlChangerBase.nestingMap.ContainsKey(type))
					ControlChangerBase.nestingMap.Add(type,new Dictionary<Control,int>());

				if (!ControlChangerBase.nestingMap[type].ContainsKey(this.baseControl))
					ControlHelper.Invoke(control,delegate
					{
						this.EnableChanger(this.baseControl);
						ControlChangerBase.nestingMap[type].Add(this.baseControl,1);
					});
				else
					ControlChangerBase.nestingMap[type][this.baseControl]++;
			}
		}


		/// <summary>
		/// Wird aufgerufen, wenn das Objekt erstellt wird und noch keins dieses Typs auf das Basiscontrol angewendet wurde
		/// </summary>
		/// <param name="baseControl">das Basiscontrol</param>
		protected abstract void EnableChanger(Control baseControl);


		/// <summary>
		/// Freigeben des Objektes
		/// </summary>
		public void Dispose()
		{
			Type type = this.GetType();
			lock (ControlChangerBase.lockingObject)
				if (!this.isDisposed)
					if (--ControlChangerBase.nestingMap[type][this.baseControl]==0)
						ControlHelper.Invoke(this.baseControl,delegate
						{
							this.DisableChanger(baseControl);
							ControlChangerBase.nestingMap[type].Remove(this.baseControl);
							this.isDisposed = true;
						});
		}


		/// <summary>
		/// Wir aufgerufen, wenn kein weiterer Changer dieses Typs auf das Basiscontrol angewendet ist und der Changer Objekt (das erste Mal) verworfen wird
		/// </summary>
		/// <param name="baseControl">das Basiscontrol</param>
		protected abstract void DisableChanger(Control baseControl);


		/// <summary>
		/// Ermittelt das Root-Control
		/// </summary>
		/// <param name="control">Control, dessen Root-Control ermittelt werden soll</param>
		/// <returns>das Root-Control des übergebenen Controls</returns>
		private Control getRootControl(Control control)
		{
			return (control.Parent==null) ? control : getRootControl(control.Parent);
		}
	}
}