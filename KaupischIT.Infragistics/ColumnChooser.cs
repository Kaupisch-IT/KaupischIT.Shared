using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using KaupischIT.Extensions;

namespace KaupischIT.InfragisticsControls
{
	/// <summary>
	/// Stellt ein Steuerelement zum Ein- und Ausblenden von Spalten eines UltraGrid-Bands bereit
	/// </summary>
	public partial class ColumnChooser : TreeView
	{
		private readonly UltraGridBand currentBand; // das Band, dessen Spalten ein-/ausgeblendet werden sollen


		/// <summary>
		/// Erstellt ein neues Steuerelement zum Ein- und Ausblenden von Spalten eines UltraGrid-Bands
		/// </summary>
		/// <param name="currentBand">das UltraGrid-Band, dessen Spalten ein-/ausgeblendet werden sollen</param>
		public ColumnChooser(UltraGridBand currentBand)
		{
			this.Font = SystemFonts.MessageBoxFont;
			this.Height = 10;

			this.currentBand = currentBand;

			this.ShowRootLines = false;
			this.ShowPlusMinus = false;
			this.ShowLines = false;
			this.CheckBoxes = true;

			// alle Spalten des Bandes zur Auswahl stellen - außer komplexe Datentypen (es sei denn, es sind IEnumerable, die als Unter-Band dargestellt werden)
			foreach (UltraGridColumn column in this.currentBand.Columns.OfType<UltraGridColumn>().OrderBy(c => c.IsChaptered).ThenBy(c => c.Header.Caption))
				if (column.DataType.IsValueType || column.DataType==typeof(string) || column.IsChaptered)
				{
					bool isHidden = (column.IsChaptered) ? this.currentBand.Layout.Bands[column.Key].Hidden : column.Hidden; // (IsChaptered gibt an, dass es ein Unter-Band ist)

					string text = column.Header.Caption;
					if (column.Header.Caption!=column.Key)
						text += " ("+column.Key+")"; // falls geändert, den originalen Namen (wie in Abfrageeditor angezeigt) mit angeben

					TreeNode treeNode = new TreeNode(text) { Checked = !isHidden,Tag = column };
					this.Nodes.Add(treeNode);
				}

			// Ausmaße des Steuerelements an den Inhalt / die vorhandenen TreeNodes anpassen
			foreach (TreeNode treeNode in this.Nodes)
			{
				Rectangle bounds = treeNode.Bounds;
				this.Width = Math.Max(bounds.X + bounds.Width + 20,this.Width);
				this.Height = Math.Max(bounds.Y + bounds.Height,this.Height);
			}

			this.DrawMode = TreeViewDrawMode.OwnerDrawText;
		}


		/// <summary>
		/// TreeNodes selbst zeichnen
		/// </summary>
		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			UltraGridColumn column = (UltraGridColumn)e.Node.Tag;

			FontStyle fontStyle = (this.GetColumnAt(e.Node.Index).IsChaptered) ? FontStyle.Italic : FontStyle.Regular; // Unter-Bänder: Kursiv
			using (Font font = new Font(e.Node.NodeFont ?? this.Font,fontStyle))
			{
				// Es wird nicht der TreeNode.Text direkt gezeichnet, sondern Column.Caption und ggf. Column.Key separat.
				// Der TreeNode.Text besteht aus den beiden Teilen, damit das TreeNode selbst die korrekte Breite bekommt

				Color color = (e.State.HasFlag(TreeNodeStates.Focused)) ? SystemColors.HighlightText : SystemColors.ControlText;
				TextRenderer.DrawText(e.Graphics,column.Header.Caption,font,e.Bounds,color,TextFormatFlags.Left|TextFormatFlags.NoPrefix);

				if (column.Header.Caption!=column.Key)
					TextRenderer.DrawText(e.Graphics,"("+column.Key+")",font,e.Bounds,SystemColors.GrayText,TextFormatFlags.Right|TextFormatFlags.NoPrefix); // der eigentliche Spaltenname grau
			}
		}


		/// <summary>
		/// Ermittelt die Spalte an der angegebenen Position
		/// </summary>
		private UltraGridColumn GetColumnAt(int index) => (UltraGridColumn)this.Nodes[index].Tag;


		/// <summary>
		/// Bei Ändern des Auswahlhakens einer Spalte diese entsprechend im zugehörigen UltraGrid anzeigen bzw. ausblenden
		/// </summary>
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