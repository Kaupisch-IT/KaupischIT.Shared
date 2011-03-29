using System.Collections.Generic;
using System.Windows.Forms;

[assembly: KaupischITC.Shared.Subversion("$Id$")]
namespace KaupischITC.Shared
{
	/// <summary>
	/// Bietet durch Implementation von IDisposable eine kompakte Möglichkeit, die Enabled-Eigenschaft eines Fensters für eine bestimmte Zeit ändern und danach wieder automatisch zurückzusetzen.
	/// </summary>
	[Subversion("$Id$")]
	public class FormDisabler : ControlChangerBase
	{
		private readonly List<Control> disabledControls = new List<Control>();	// Liste der Controls, die deaktiviert wurden


		/// <summary>
		/// Erstellt ein neues FormDisabler-Objekt und setzt die Enabled-Eigenschaft der aktivierten Kindcontrols des angegbeben Controls auf false.
		/// Sobald alle erstellten FormDisabler-Objekte zu einem Basiscontrol freigegeben wurden, werden die deaktivierten Controls wieder aktiviert.
		/// </summary>
		/// <param name="control"></param>
		public FormDisabler(Control control)
			: base(control)
		{}

		
		/// <summary>
		/// Alle aktierten Kindcontrols des Basiscontrols deaktivieren
		/// </summary>
		/// <param name="baseControl">das Basiscontrol</param>
		protected override void EnableChanger(Control baseControl)
		{
			foreach (Control childControl in baseControl.Controls)
				if (childControl.Enabled)
				{
					this.disabledControls.Add(childControl);
					childControl.Enabled = false;
				}
		}


		/// <summary>
		/// Alle zuvor deaktivierten Controls wieder aktivieren
		/// </summary>
		/// <param name="baseControl"></param>
		protected override void DisableChanger(Control baseControl)
		{
			foreach (Control childControl in this.disabledControls)
				childControl.Enabled = true;
		}
	}
}
