using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace KaupischITC.InfragisticsControls
{
	public partial class ColumnChooser : CheckedListBox
	{
		private readonly UltraGridBand currentBand;

		private class ColumnItem
		{
			public UltraGridColumn Column { get; set; }
			public override string ToString() { return this.Column.Header.Caption; }
		}

		private UltraGridColumn GetColumnAt(int index)
		{
			return ((ColumnItem)this.Items[index]).Column;
		}

		public ColumnChooser(UltraGridBand currentBand)
		{
			this.currentBand = currentBand;

			this.IntegralHeight = false;
			this.CheckOnClick = true;

			foreach (UltraGridColumn column in this.currentBand.Columns.OfType<UltraGridColumn>().OrderBy(c => c.IsChaptered).ThenBy(c => c.Header.Caption))
			{
				bool isHidden = (column.IsChaptered) ? this.currentBand.Layout.Bands[column.Key].Hidden : column.Hidden;
				this.Items.Add(new ColumnItem { Column = column },!isHidden);
			}
		}

		protected override void OnItemCheck(ItemCheckEventArgs e)
		{
			UltraGridColumn column = this.GetColumnAt(e.Index);
			if (column.IsChaptered)
				this.currentBand.Layout.Bands[column.Key].Hidden = (e.NewValue==CheckState.Unchecked);
			else
				this.currentBand.Columns[column.Key].Hidden = (e.NewValue==CheckState.Unchecked);
		}


		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			UltraGridColumn column = ((ColumnItem)this.Items[e.Index]).Column;

			Color foreColor = (this.GetItemChecked(e.Index)) ? e.ForeColor : SystemColors.GrayText;
			Color backColor = (column.IsChaptered) ? Color.FromArgb(240,241,242) : e.BackColor;

			base.OnDrawItem(new DrawItemEventArgs(e.Graphics,e.Font,e.Bounds,e.Index,e.State,foreColor,backColor));
		}
	}
}