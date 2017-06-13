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
		public ControlChangerBase(Control control,bool useRootControl)
		{
			this.baseControl = (useRootControl) ? ControlChangerBase.GetRootControl(control) : control;
			Type type = this.GetType();
			lock (ControlChangerBase.lockingObject)
			{
				if (!ControlChangerBase.nestingMap.ContainsKey(type))
					ControlChangerBase.nestingMap.Add(type,new Dictionary<Control,int>());

				if (!ControlChangerBase.nestingMap[type].ContainsKey(this.baseControl))
					ControlChangerBase.nestingMap[type].Add(this.baseControl,0);

				ControlChangerBase.nestingMap[type][this.baseControl]++;
				if (ControlChangerBase.nestingMap[type][this.baseControl]==1)
					ControlHelper.InvokeAsync(control,delegate
					{
						this.EnableChanger(this.baseControl);
					});
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
			{
				ControlChangerBase.nestingMap[type][this.baseControl]--;

				if (ControlChangerBase.nestingMap[type][this.baseControl]==0 && !this.isDisposed)
					ControlHelper.InvokeAsync(this.baseControl,delegate
					{
						try { this.DisableChanger(baseControl); }
						catch (ObjectDisposedException)
						{
						}
						this.isDisposed = true;
					});

				if (ControlChangerBase.nestingMap[type][this.baseControl]<0)
					throw new Exception("CursorChanger-Index ist negativ");
			}
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
		private static Control GetRootControl(Control control)
		{
			return (control.Parent==null) ? control : GetRootControl(control.Parent);
		}
	}
}