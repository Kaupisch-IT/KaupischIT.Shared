namespace KaupischIT.InfragisticsControls
{
	partial class FormDiff
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
			Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("", -1);
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiff));
			this.customizedUltraGrid = new KaupischIT.InfragisticsControls.CustomizedUltraGrid();
			((System.ComponentModel.ISupportInitialize)(this.customizedUltraGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// customizedUltraGrid
			// 
			this.customizedUltraGrid.AllowRowFiltering = false;
			this.customizedUltraGrid.Cursor = System.Windows.Forms.Cursors.Default;
			appearance1.TextHAlignAsString = "Left";
			ultraGridBand1.Header.Appearance = appearance1;
			this.customizedUltraGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
			this.customizedUltraGrid.DisplayLayout.GroupByBox.Hidden = true;
			this.customizedUltraGrid.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
			this.customizedUltraGrid.DisplayLayout.MaxBandDepth = 5;
			this.customizedUltraGrid.DisplayLayout.MaxColScrollRegions = 1;
			this.customizedUltraGrid.DisplayLayout.MaxRowScrollRegions = 1;
			this.customizedUltraGrid.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
			this.customizedUltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
			this.customizedUltraGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
			this.customizedUltraGrid.DisplayLayout.Override.RowSelectorNumberStyle = Infragistics.Win.UltraWinGrid.RowSelectorNumberStyle.RowIndex;
			this.customizedUltraGrid.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Free;
			this.customizedUltraGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
			this.customizedUltraGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
			this.customizedUltraGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
			this.customizedUltraGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.customizedUltraGrid.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.customizedUltraGrid.Location = new System.Drawing.Point(0, 0);
			this.customizedUltraGrid.Name = "customizedUltraGrid";
			this.customizedUltraGrid.Size = new System.Drawing.Size(719, 355);
			this.customizedUltraGrid.SortIndicatorImageAscending = ((System.Drawing.Image)(resources.GetObject("customizedUltraGrid.SortIndicatorImageAscending")));
			this.customizedUltraGrid.SortIndicatorImageDescending = ((System.Drawing.Image)(resources.GetObject("customizedUltraGrid.SortIndicatorImageDescending")));
			this.customizedUltraGrid.SyncWithCurrencyManager = false;
			this.customizedUltraGrid.TabIndex = 0;
			// 
			// FormDiff
			// 
			this.ClientSize = new System.Drawing.Size(719, 355);
			this.Controls.Add(this.customizedUltraGrid);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDiff";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FormDiff";
			((System.ComponentModel.ISupportInitialize)(this.customizedUltraGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private CustomizedUltraGrid customizedUltraGrid;
	}
}