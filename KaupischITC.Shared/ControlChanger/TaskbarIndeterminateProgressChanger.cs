using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

[assembly: KaupischITC.Shared.Subversion("$Id$")]
namespace KaupischITC.Shared
{
	/// <summary>
	/// Bietet durch Implementation von IDisposable eine kompakte Möglichkeit, die UseWaitCursor-Eigenschaft eines Fensters für eine bestimmte Zeit ändern und danach wieder automatisch zurückzusetzen.
	/// </summary>
	[Subversion("$Id$")]
	public class TaskbarIndeterminateProgressChanger : ControlChangerBase
	{
		/// <summary>
		/// Erstellt ein neues WaitCursorChanger-Objekt und setzt die UseWaitCursor-Eigenschaft des angegbeben Controls auf true.
		/// Sobald alle erstellten WaitCursor-Objekte zu einem Control freigegeben wurden, wird die UseWaitCursor-Eigenschaft wieder auf den Ursprungswert gesetzt
		/// </summary>
		/// <param name="control"></param>
		public TaskbarIndeterminateProgressChanger(Control control)
			: base(control)
		{}

		
		protected override void EnableChanger(Control baseControl)
		{
			if (TaskbarManager.IsPlatformSupported)
				TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
		}

		protected override void DisableChanger(Control baseControl)
		{
			if (TaskbarManager.IsPlatformSupported)
				TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
		}
	}
}