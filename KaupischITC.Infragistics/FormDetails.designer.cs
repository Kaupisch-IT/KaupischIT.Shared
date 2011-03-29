namespace KaupischITC.InfragisticsControls
{
	partial class FormDetails
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
			Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("",-1);
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			this.customizedUltraGrid = new CustomizedUltraGrid();
			((System.ComponentModel.ISupportInitialize)(this.customizedUltraGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// customizedUltraGrid
			// 
			this.customizedUltraGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
			this.customizedUltraGrid.DisplayLayout.GroupByBox.Hidden = true;
			this.customizedUltraGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
			this.customizedUltraGrid.DisplayLayout.MaxBandDepth = 5;
			this.customizedUltraGrid.DisplayLayout.MaxColScrollRegions = 1;
			this.customizedUltraGrid.DisplayLayout.MaxRowScrollRegions = 1;
			this.customizedUltraGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
			this.customizedUltraGrid.DisplayLayout.Override.CellAppearance = appearance1;
			this.customizedUltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
			this.customizedUltraGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
			this.customizedUltraGrid.DisplayLayout.Override.RowSelectorNumberStyle = Infragistics.Win.UltraWinGrid.RowSelectorNumberStyle.RowIndex;
			this.customizedUltraGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Free;
			this.customizedUltraGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
			this.customizedUltraGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
			this.customizedUltraGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
			this.customizedUltraGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.customizedUltraGrid.Location = new System.Drawing.Point(0,0);
			this.customizedUltraGrid.Name = "customizedUltraGrid";
			this.customizedUltraGrid.Size = new System.Drawing.Size(719,355);
			this.customizedUltraGrid.TabIndex = 0;
			// 
			// FormDetails
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.ClientSize = new System.Drawing.Size(719,355);
			this.Controls.Add(this.customizedUltraGrid);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FormDetails";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FormDetails";
			((System.ComponentModel.ISupportInitialize)(this.customizedUltraGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private CustomizedUltraGrid customizedUltraGrid;
	}
}