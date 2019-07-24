using System;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace KaupischIT.Shared
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


		/// <summary>
		/// Erstellt ein neues Meldungsfenster für Details einer Ausnahme
		/// </summary>
		/// <param name="message">die Meldung, die angezeigt werden soll</param>
		/// <param name="caption">der Fenstertitel</param>
		/// <param name="exception">die Ausnahme, deren Details angezeigt werden sollen</param>
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

			this.textBoxDetails.Text = this.GetCompleteExceptionMessage(exception).Trim();
		}


		/// <summary>
		/// Ermittelt die vollständige Fehlermeldung der Ausnahme inklusive der InnerExceptions
		/// </summary>
		private string GetCompleteExceptionMessage(Exception exception)
		{
			StringBuilder result = new StringBuilder();
			while (exception!=null)
			{
				result.AppendLine(exception.ToString());
				result.AppendLine();

				if (exception is ReflectionTypeLoadException typeLoadException)
					foreach (Exception loaderException in typeLoadException.LoaderExceptions)
						result.AppendLine(this.GetCompleteExceptionMessage(loaderException));

				exception = exception.InnerException;
			}
			return result.ToString();
		}


		/// <summary>
		/// Klick auf "OK"
		/// </summary>
		private void ButtonOk_Click(object sender,EventArgs e) => this.Close();

		/// <summary>
		/// Klick auf "Details anzeigen"
		/// </summary>
		private void LinkLabelDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
		{
			Size oldSize = this.Size;

			this.SuspendLayout();
			{
				this.textBoxDetails.Visible = true;
				this.linkLabelDetails.Visible = false;
				this.MinimumSize = new Size(this.MaximumSize.Width,this.MinimumSize.Height);
				this.buttonOk.Select();
			}
			this.ResumeLayout();

			this.Location = new Point(Math.Max(1,this.Left - (this.Width-oldSize.Width)/2),Math.Max(1,this.Top - (this.Height-oldSize.Height)/2));
		}

		/// <summary>
		/// Beim Drücken von [Strg+C] den Inhalt der MessageBox in die Zwischenablage kopieren
		/// </summary>
		private void ErrorMessageBox_KeyDown(object sender,KeyEventArgs e)
		{
			if (e.Control && e.KeyCode==Keys.C)
			{
				StringBuilder result = new StringBuilder();
				result.AppendLine(this.labelMessage.Text);
				result.AppendLine();
				result.AppendLine(this.textBoxDetails.Text);

				try
				{ Clipboard.SetText(result.ToString()); }
				catch { }
			}
		}

		/// <summary>
		/// [Strg+A] für die TextBox mit den Ausnahme-Details
		/// </summary>
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
		public static ErrorMessageBox Show(Exception exception) => ErrorMessageBox.Show(exception.Message,exception);


		/// <summary>
		/// Zeigt ein Meldungsfeld mit dem Meldungstext und ggf. Detailinformationen der angegebenen Ausnahme an.
		/// </summary>
		/// <param name="message">der im Meldungsfeld anzuzeigende Text</param>
		/// <param name="exception">die Ausnahme, deren Details ausgegeben werden sollen</param>
		/// <returns>das angezeigte Meldungsfeld</returns>
		public static ErrorMessageBox Show(string message,Exception exception) => ErrorMessageBox.Show(message,Application.ProductName,exception);


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
