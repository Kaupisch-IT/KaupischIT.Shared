using System.Windows.Forms;

[assembly: KaupischITC.Shared.Subversion("$Id$")]
namespace KaupischITC.Shared
{
	/// <summary>
	/// Bietet durch Implementation von IDisposable eine kompakte Möglichkeit, die UseWaitCursor-Eigenschaft eines Fensters für eine bestimmte Zeit ändern und danach wieder automatisch zurückzusetzen.
	/// </summary>
	[Subversion("$Id$")]
	public class WaitCursorChanger : ControlChangerBase
	{
		/// <summary>
		/// Erstellt ein neues WaitCursorChanger-Objekt und setzt die UseWaitCursor-Eigenschaft des angegbeben Controls auf true.
		/// Sobald alle erstellten WaitCursor-Objekte zu einem Control freigegeben wurden, wird die UseWaitCursor-Eigenschaft wieder auf den Ursprungswert gesetzt
		/// </summary>
		/// <param name="control"></param>
		public WaitCursorChanger(Control control)
			: this(control,true)
		{ }


		public WaitCursorChanger(Control control,bool useRootControl)
			: base(control,useRootControl)
		{ }


		protected override void EnableChanger(Control baseControl)
		{
			//	Cursor.Current = Cursors.WaitCursor;
			baseControl.UseWaitCursor = true;
			baseControl.Cursor = Cursors.WaitCursor;
		}

		protected override void DisableChanger(Control baseControl)
		{
			baseControl.UseWaitCursor = false;
			baseControl.Cursor = Cursors.Default;
			//			Cursor.Current = Cursor.Current;

			Application.DoEvents();
			Cursor.Position = Cursor.Position;
		}
	}
}