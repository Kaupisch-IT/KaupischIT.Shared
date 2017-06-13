namespace KaupischIT.DevExpressControls
{
	partial class ReportDesignerForm
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
			this.reportDesigner = new KaupischIT.DevExpressControls.ReportDesigner();
			this.SuspendLayout();
			// 
			// reportDesigner
			// 
			this.reportDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reportDesigner.Location = new System.Drawing.Point(0, 0);
			this.reportDesigner.Name = "reportDesigner";
			this.reportDesigner.Size = new System.Drawing.Size(952, 645);
			this.reportDesigner.TabIndex = 0;
			// 
			// ReportDesignerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(952, 645);
			this.Controls.Add(this.reportDesigner);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReportDesignerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Berichts-Editor";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReportDesignerForm_FormClosed);
			this.ResumeLayout(false);

		}

		#endregion

		private ReportDesigner reportDesigner;
	}
}