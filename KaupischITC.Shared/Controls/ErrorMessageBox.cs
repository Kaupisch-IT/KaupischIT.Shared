using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	public partial class ErrorMessageBox : Form
	{
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


		private void buttonOk_Click(object sender,EventArgs e)
		{
			this.Close();
		}


		private void linkLabelDetails_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
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


		private void ErrorMessageBox_KeyUp(object sender,KeyEventArgs e)
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

		public static ErrorMessageBox Show(string message,Exception exception)
		{
			return ErrorMessageBox.Show(message,Application.ProductName,exception);
		}

		public static ErrorMessageBox Show(string message,string caption,Exception exception)
		{
			ErrorMessageBox errorMessageBox = new ErrorMessageBox(message,caption,exception);

			Form activeForm = Form.ActiveForm;
			if (activeForm!=null)
				errorMessageBox.StartPosition = FormStartPosition.CenterParent;

			errorMessageBox.ShowDialog(activeForm);
			return errorMessageBox;
		}
	}
}
