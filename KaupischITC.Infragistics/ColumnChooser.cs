using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using KaupischITC.Extensions;

namespace KaupischITC.InfragisticsControls
{
	public partial class ColumnChooser : TreeView
	{
		private readonly UltraGridBand currentBand;

		public ColumnChooser(UltraGridBand currentBand)
		{
			this.Font = SystemFonts.MessageBoxFont;
			this.Height = 10;

			this.currentBand = currentBand;

			this.ShowRootLines = false;
			this.ShowPlusMinus = false;
			this.ShowLines = false;
			this.CheckBoxes = true;
			
			foreach (UltraGridColumn column in this.currentBand.Columns.OfType<UltraGridColumn>().OrderBy(c => c.Header.Caption))
			{
				bool isHidden = (column.IsChaptered) ? this.currentBand.Layout.Bands[column.Key].Hidden : column.Hidden;
				TreeNode treeNode = new TreeNode(column.Header.Caption) { Checked = !isHidden,Tag = column };
				this.Nodes.Add(treeNode);
			}
						
			foreach (TreeNode treeNode in this.Nodes)
			{
				Rectangle bounds = treeNode.Bounds;
				this.Width = Math.Max(bounds.X + bounds.Width + 20,this.Width);
				this.Height = Math.Max(bounds.Y + bounds.Height,this.Height);
			}

			this.DrawMode = TreeViewDrawMode.OwnerDrawText;
		}
		

		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			FontStyle fontStyle = (this.GetColumnAt(e.Node.Index).IsChaptered) ? FontStyle.Italic : FontStyle.Regular;
			Color color = (e.State.HasFlag(TreeNodeStates.Focused)) ? SystemColors.HighlightText : SystemColors.ControlText;
			using (Font font = new Font(e.Node.NodeFont ?? this.Font,fontStyle))
				TextRenderer.DrawText(e.Graphics,e.Node.Text,font,e.Bounds,color);
		}

		private UltraGridColumn GetColumnAt(int index)
		{
			return (UltraGridColumn)this.Nodes[index].Tag;
		}

		protected override void OnAfterCheck(TreeViewEventArgs e)
		{
			UltraGridColumn column = this.GetColumnAt(e.Node.Index);
			if (column.IsChaptered)
				this.currentBand.Layout.Bands[column.Key].Hidden = !e.Node.Checked;
			else
				this.currentBand.Columns[column.Key].Hidden = !e.Node.Checked;
		}
	}
}