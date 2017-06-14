namespace KaupischIT.Charting
{
	partial class ChartForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartForm));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.propertyBrowserValue = new KaupischIT.Shared.PropertyBrowser();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.propertyBrowserDisplay = new KaupischIT.Shared.PropertyBrowser();
			this.pictureBoxPie = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownPercentageThreshold = new System.Windows.Forms.NumericUpDown();
			this.buttonToClipboard = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPie)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPercentageThreshold)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.propertyBrowserValue, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.propertyBrowserDisplay, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBoxPie, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label3, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.label4, 4, 1);
			this.tableLayoutPanel1.Controls.Add(this.numericUpDownPercentageThreshold, 3, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(386, 280);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// propertyBrowserValue
			// 
			this.propertyBrowserValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyBrowserValue.DropDownHeight = 212;
			this.propertyBrowserValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.propertyBrowserValue.FormattingEnabled = true;
			this.propertyBrowserValue.IntegralHeight = false;
			this.propertyBrowserValue.Location = new System.Drawing.Point(78, 30);
			this.propertyBrowserValue.Name = "propertyBrowserValue";
			this.propertyBrowserValue.Size = new System.Drawing.Size(148, 21);
			this.propertyBrowserValue.TabIndex = 3;
			this.propertyBrowserValue.TypeFilter = null;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Beschriftung:";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Werte:";
			// 
			// propertyBrowserDisplay
			// 
			this.propertyBrowserDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyBrowserDisplay.DropDownHeight = 212;
			this.propertyBrowserDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.propertyBrowserDisplay.FormattingEnabled = true;
			this.propertyBrowserDisplay.IntegralHeight = false;
			this.propertyBrowserDisplay.Location = new System.Drawing.Point(78, 3);
			this.propertyBrowserDisplay.Name = "propertyBrowserDisplay";
			this.propertyBrowserDisplay.Size = new System.Drawing.Size(148, 21);
			this.propertyBrowserDisplay.TabIndex = 2;
			this.propertyBrowserDisplay.TypeFilter = null;
			// 
			// pictureBoxPie
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.pictureBoxPie, 5);
			this.pictureBoxPie.Location = new System.Drawing.Point(3, 57);
			this.pictureBoxPie.Name = "pictureBoxPie";
			this.pictureBoxPie.Size = new System.Drawing.Size(380, 221);
			this.pictureBoxPie.TabIndex = 4;
			this.pictureBoxPie.TabStop = false;
			this.pictureBoxPie.MouseEnter += new System.EventHandler(this.PictureBoxPie_MouseEnter);
			this.pictureBoxPie.MouseLeave += new System.EventHandler(this.PictureBoxPie_MouseLeave);
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(232, 34);
			this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(25, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "(ab ";
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(297, 34);
			this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(86, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "% als \"sonstige\")";
			// 
			// numericUpDownPercentageThreshold
			// 
			this.numericUpDownPercentageThreshold.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.numericUpDownPercentageThreshold.Location = new System.Drawing.Point(257, 30);
			this.numericUpDownPercentageThreshold.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.numericUpDownPercentageThreshold.Name = "numericUpDownPercentageThreshold";
			this.numericUpDownPercentageThreshold.Size = new System.Drawing.Size(40, 20);
			this.numericUpDownPercentageThreshold.TabIndex = 7;
			this.numericUpDownPercentageThreshold.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// buttonToClipboard
			// 
			this.buttonToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonToClipboard.Image = ((System.Drawing.Image)(resources.GetObject("buttonToClipboard.Image")));
			this.buttonToClipboard.Location = new System.Drawing.Point(355, 59);
			this.buttonToClipboard.Name = "buttonToClipboard";
			this.buttonToClipboard.Size = new System.Drawing.Size(26, 27);
			this.buttonToClipboard.TabIndex = 1;
			this.toolTip.SetToolTip(this.buttonToClipboard, "aktuelle Anzeige in die Zwischenablage kopieren");
			this.buttonToClipboard.UseVisualStyleBackColor = true;
			this.buttonToClipboard.Visible = false;
			this.buttonToClipboard.Click += new System.EventHandler(this.ButtonToClipboard_Click);
			// 
			// ChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(386, 280);
			this.Controls.Add(this.buttonToClipboard);
			this.Controls.Add(this.tableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChartForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Diagramm";
			this.Load += new System.EventHandler(this.PieForm_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPie)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownPercentageThreshold)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private KaupischIT.Shared.PropertyBrowser propertyBrowserValue;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private KaupischIT.Shared.PropertyBrowser propertyBrowserDisplay;
		private System.Windows.Forms.PictureBox pictureBoxPie;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownPercentageThreshold;
		private System.Windows.Forms.Button buttonToClipboard;
		private System.Windows.Forms.ToolTip toolTip;

	}
}