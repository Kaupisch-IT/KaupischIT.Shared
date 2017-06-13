using System.Collections.Generic;
using System.Windows.Forms;

namespace KaupischIT.Extensions
{
	public static class TreeViewExtensions
	{
		/// <summary>
		/// Gibt alle Knoten eines TreeViews zurück (inklusiver verschachtelter Kindknoten)
		/// </summary>
		public static IEnumerable<TreeNode> GetAllNodes(this TreeView treeView)
		{
			foreach (TreeNode child in treeView.Nodes)
				foreach (TreeNode result in child.GetAllNodes())
					yield return result;
		}

		/// <summary>
		/// Gibt alle Knoten eines TreeNodes zurück (inklusiver verschachtelter Kindknoten)
		/// </summary>
		public static IEnumerable<TreeNode> GetAllNodes(this TreeNode treeNode)
		{
			yield return treeNode;
			foreach (TreeNode child in treeNode.Nodes)
				foreach (TreeNode result in child.GetAllNodes())
					yield return result;

		}
	}
}
