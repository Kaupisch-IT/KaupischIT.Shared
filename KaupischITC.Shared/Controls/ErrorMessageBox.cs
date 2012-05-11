using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Zeigt ein Meldungsfeld an, das Text und Details über eine Ausnahme enthält.
	/// </summary>
	public partial class ErrorMessageBox : Form
	{
		/// <summary>
		/// Gibt die Ausnahme zurück, deren Details angezeigt werden.
		/// </summary>
		public Exception Exception { get; private set; }


		internal ErrorMessageBox(string message,string caption,Exception exception)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			Bitmap bitmap = SystemIcons.Error.ToBitmap();
			this.pictureBoxIcon.Image = bitmap;
			this.pictureBoxIcon.Size = bitmap.Size;

			this.Text = caption;
			this.labelMessage.Text = message.Trim();
			this.Exception = exception;

			StringBuilder stringBuilder = new StringBuilder();
			while (exception!=null)
			{
				stringBuilder.AppendLine(exception.ToString());
				stringBuilder.AppendLine();

				exception = exception.InnerException;
			}
			this.textBoxDetails.Text = stringBuilder.ToString().Trim();
		}


		private void ButtonOk_Click(object sender,EventArgs e)
		{
			this.Close();
		}


		private void LinkLabelDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
		{
			Size oldSize = this.Size;

			this.SuspendLayout();

			this.textBoxDetails.Visible = true;
			this.linkLabelDetails.Visible = false;
			this.textBoxDetails.MinimumSize = new Size(480,150);
			this.labelMessage.MinimumSize = new Size(450,this.labelMessage.MinimumSize.Height);
			this.buttonOk.Select();

			this.ResumeLayout();

			this.Location = new Point(Math.Max(1,this.Left - (this.Width-oldSize.Width)/2),Math.Max(1,this.Top - (this.Height-oldSize.Height)/2));
		}


		private void ErrorMessageBox_KeyDown(object sender,KeyEventArgs e)
		{
			if (e.Control && e.KeyCode==Keys.C)
			{
				StringBuilder result = new StringBuilder();
				result.AppendLine(this.labelMessage.Text);
				result.AppendLine();
				result.AppendLine(this.textBoxDetails.Text);

				Clipboard.SetText(result.ToString());
			}
		}

		private void TextBoxDetails_KeyDown(object sender,KeyEventArgs e)
		{
			if (e.Control && e.KeyCode==Keys.A)
				this.textBoxDetails.SelectAll();
		}


#if true // Windows API Code Pack
		protected override void OnShown(EventArgs e)
		{
			if (TaskbarManager.IsPlatformSupported)
			{
				TaskbarManager.Instance.SetProgressValue(1,1);
				TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
			}
			base.OnShown(e);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (TaskbarManager.IsPlatformSupported)
				TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
			base.OnClosing(e);
		}
#endif


		/// <summary>
		/// Zeigt ein Meldungsfeld mit dem Meldungstext und ggf. Detailinformationen der angegebenen Ausnahme an.
		/// </summary>
		/// <param name="exception">die Ausnahme, deren Meldung und Details ausgegeben werden sollen</param>
		/// <returns>das angezeigte Meldungsfeld</returns>
		public static ErrorMessageBox Show(Exception exception)
		{
			return ErrorMessageBox.Show(exception.Message,exception);
		}


		/// <summary>
		/// Zeigt ein Meldungsfeld mit dem Meldungstext und ggf. Detailinformationen der angegebenen Ausnahme an.
		/// </summary>
		/// <param name="message">der im Meldungsfeld anzuzeigende Text</param>
		/// <param name="exception">die Ausnahme, deren Details ausgegeben werden sollen</param>
		/// <returns>das angezeigte Meldungsfeld</returns>
		public static ErrorMessageBox Show(string message,Exception exception)
		{
			return ErrorMessageBox.Show(message,Application.ProductName,exception);
		}


		/// <summary>
		/// Zeigt ein Meldungsfeld mit dem Meldungstext und ggf. Detailinformationen der angegebenen Ausnahme an.
		/// </summary>
		/// <param name="message">der im Meldungsfeld anzuzeigende Text</param>
		/// <param name="caption">der in der Titelleiste des Meldungsfelds anzuzeigende Text</param>
		/// <param name="exception">die Ausnahme, deren Details ausgegeben werden sollen</param>
		/// <returns>das angezeigte Meldungsfeld</returns>
		public static ErrorMessageBox Show(string message,string caption,Exception exception)
		{
			ErrorMessageBox errorMessageBox = new ErrorMessageBox(message,caption,exception);

			Form activeForm = Form.ActiveForm;
			if (activeForm!=null)
			{
				errorMessageBox.StartPosition = FormStartPosition.CenterParent;
				errorMessageBox.ShowInTaskbar = false;
				activeForm.Invoke((MethodInvoker)delegate { errorMessageBox.ShowDialog(activeForm); });
			}
			else
				errorMessageBox.ShowDialog();
			
			return errorMessageBox;
		}
	}
}
