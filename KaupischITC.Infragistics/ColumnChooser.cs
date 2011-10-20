using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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

			foreach (UltraGridColumn column in this.currentBand.Columns.OfType<UltraGridColumn>().OrderBy(c => c.Header.Caption))
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
			Color backColor = (column.IsChaptered) ? Color.FromArgb(240,241,242) : e.BackColor;
			Color foreColor = (this.GetItemChecked(e.Index)) ? e.ForeColor : SystemColors.InactiveCaptionText;

			base.OnDrawItem(new DrawItemEventArgs(e.Graphics,e.Font,e.Bounds,e.Index,e.State,foreColor,e.BackColor));
			if (column.IsChaptered)
			{
				Font font = new Font(e.Font,FontStyle.Bold);
				Rectangle rect = e.Bounds;

				Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics,CheckBoxState.CheckedNormal);
				rect.Offset(glyphSize.Width+4,0);
				
				using (Brush brush = new SolidBrush(e.BackColor))
					e.Graphics.FillRectangle(brush,rect);
				using (Brush brush = new SolidBrush(e.ForeColor))
					e.Graphics.DrawString(column.Header.Caption,font,brush,rect);
			}
		}

	}
}