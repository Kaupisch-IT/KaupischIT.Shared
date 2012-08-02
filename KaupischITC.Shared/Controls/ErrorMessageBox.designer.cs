namespace KaupischITC.Shared
{
	partial class ErrorMessageBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorMessageBox));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.labelMessage = new System.Windows.Forms.Label();
			this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.linkLabelDetails = new System.Windows.Forms.LinkLabel();
			this.textBoxDetails = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.labelMessage, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBoxIcon, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(15, 20, 15, 20);
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(414, 104);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// labelMessage
			// 
			this.labelMessage.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelMessage.AutoSize = true;
			this.labelMessage.Location = new System.Drawing.Point(56, 20);
			this.labelMessage.MaximumSize = new System.Drawing.Size(350, 0);
			this.labelMessage.MinimumSize = new System.Drawing.Size(200, 13);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(338, 64);
			this.labelMessage.TabIndex = 0;
			this.labelMessage.Text = resources.GetString("labelMessage.Text");
			// 
			// pictureBoxIcon
			// 
			this.pictureBoxIcon.Location = new System.Drawing.Point(18, 23);
			this.pictureBoxIcon.Name = "pictureBoxIcon";
			this.pictureBoxIcon.Size = new System.Drawing.Size(32, 32);
			this.pictureBoxIcon.TabIndex = 1;
			this.pictureBoxIcon.TabStop = false;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonOk.AutoSize = true;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOk.Location = new System.Drawing.Point(331, 194);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.buttonOk, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.linkLabelDetails, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.textBoxDetails, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 104);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(414, 224);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// linkLabelDetails
			// 
			this.linkLabelDetails.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.linkLabelDetails.AutoSize = true;
			this.linkLabelDetails.Location = new System.Drawing.Point(8, 199);
			this.linkLabelDetails.Name = "linkLabelDetails";
			this.linkLabelDetails.Size = new System.Drawing.Size(120, 13);
			this.linkLabelDetails.TabIndex = 1;
			this.linkLabelDetails.TabStop = true;
			this.linkLabelDetails.Text = "Was genau ist passiert?";
			this.linkLabelDetails.UseMnemonic = false;
			this.linkLabelDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelDetails_LinkClicked);
			// 
			// textBoxDetails
			// 
			this.tableLayoutPanel2.SetColumnSpan(this.textBoxDetails, 2);
			this.textBoxDetails.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxDetails.Location = new System.Drawing.Point(8, 7);
			this.textBoxDetails.Multiline = true;
			this.textBoxDetails.Name = "textBoxDetails";
			this.textBoxDetails.ReadOnly = true;
			this.textBoxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxDetails.Size = new System.Drawing.Size(398, 181);
			this.textBoxDetails.TabIndex = 2;
			this.textBoxDetails.Visible = false;
			this.textBoxDetails.WordWrap = false;
			this.textBoxDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxDetails_KeyDown);
			// 
			// ErrorMessageBox
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.buttonOk;
			this.ClientSize = new System.Drawing.Size(414, 328);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.tableLayoutPanel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(430, 1000);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(200, 50);
			this.Name = "ErrorMessageBox";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ExceptionMessageBox";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ErrorMessageBox_KeyDown);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.PictureBox pictureBoxIcon;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.LinkLabel linkLabelDetails;
		private System.Windows.Forms.TextBox textBoxDetails;
	}
}