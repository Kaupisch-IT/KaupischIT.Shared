using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using KaupischITC.Extensions;

namespace KaupischITC.Shared
{
	public partial class ObjectBrowser : TreeView
	{
		private bool isUpdating = false;
		private ImageList imageList;
		private IContainer components;

		public bool ResolveEnumerable { get; set; }
		public bool RemoveEmptyTypes { get; set; }

		public Func<Type,bool> TypeFilter { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<string> CheckedNodes
		{
			get
			{
				return this.Nodes.Cast<TreeNode>()
					.SelectMany(node => node.Flatten(tn => tn.Nodes.Cast<TreeNode>()))
					.Where(node => node.Checked)
					.Select(node => (!String.IsNullOrEmpty(node.Name)) ? node.Name : ((this.displayedType.IsCompilerGenerated()) ? "#Element" : "#"+this.Nodes[0].Text));
			}
			set
			{
				if (!this.DesignMode)
				{
					this.BeginUpdate();
					this.isUpdating = true;

					foreach (TreeNode treeNode in this.Nodes.OfType<TreeNode>().SelectMany(node => node.Flatten(tn => tn.Nodes.Cast<TreeNode>())).Where(tn => tn.Checked))
						treeNode.Checked = false;

					if (value!=null && value.Any())
						foreach (string nodeName in value)
							if (!String.IsNullOrEmpty(nodeName))
							{
								TreeNode treeNode = this.GetNode(this.Nodes[0],nodeName);
								treeNode.Checked = true;
								this.SelectedNode = treeNode;
							}

					this.EndUpdate();
					this.isUpdating = false;
				}
			}
		}

		private TreeNode GetNode(TreeNode parentTreeNode,string nodeName)
		{
			if (nodeName.StartsWith("#"))
				return parentTreeNode;

			if (nodeName.Contains('.'))
			{
				string childNodeName = nodeName.Substring(0,nodeName.IndexOf('.')).Trim('.');
				string childNodeKey = (parentTreeNode.Name+"."+childNodeName).Trim('.');

				TreeNode childNode = parentTreeNode.Nodes[childNodeKey];
				if (childNode==null)
					childNode = parentTreeNode.Nodes.Add(childNodeKey,childNodeName.Trim('+'),"QuestionMark","QuestionMark");
				else
				{
					this.OnBeforeExpand(new TreeViewCancelEventArgs(childNode,false,TreeViewAction.Expand));
					childNode.Expand();
				}

				return this.GetNode(childNode,nodeName.Substring(nodeName.IndexOf('.')+1));
			}
			else
			{
				string nodeKey = (parentTreeNode.Name+'.'+nodeName).Trim('.');
				return parentTreeNode.Nodes[nodeKey] ?? parentTreeNode.Nodes.Add(nodeKey,nodeName.Trim('+'),"QuestionMark","QuestionMark");
			}
		}


		public ObjectBrowser()
		{
			this.InitializeComponent();
			this.Font = SystemFonts.MessageBoxFont;

			this.ImageList = this.imageList;
			this.CheckBoxes = true;
			this.ShowRootLines = false;
			this.displayedType = this.GetType(); // HACK
			this.DisplayedType = null;

			this.DrawMode = TreeViewDrawMode.OwnerDrawText;
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type DisplayedType
		{
			get { return this.displayedType; }
			set
			{
				if (!this.DesignMode && this.displayedType!=value)
					using (new WaitCursorChanger(this))
					{
						this.BeginUpdate();
						this.isUpdating = true;

						List<string> oldCheckedNodes = this.CheckedNodes.ToList();
						this.Nodes.Clear();

						if (value!=null)
						{
							TreeNode treeNode = new TreeNode();
							treeNode.Text = (!value.IsCompilerGenerated()) ? value.Name : "(generierter Typ)";
							treeNode.ImageKey = treeNode.SelectedImageKey = "ComplexType";
							this.Nodes.Add(treeNode);
							this.AddMemberNodes(treeNode.Nodes,value,"",this.CheckedNodes,false);
							treeNode.Expand();
						}
						else
							this.Nodes.Add("","(unbekannter Typ)","QuestionMark");

						this.CheckedNodes = oldCheckedNodes;

						this.EndUpdate();
						this.isUpdating = false;
						this.displayedType = value;
					}
			}
		}
		private Type displayedType;



		private void AddMemberNodes(TreeNodeCollection targetTreeNodeCollection,Type type,string parentKey,IEnumerable<string> checkedNodeNames,bool deferredLoading)
		{
			string[] hiddenPostfixes = { "ID","Id","Key" };

			IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance|BindingFlags.Public);
			if (!type.IsCompilerGenerated())
				properties = properties.OrderBy(pi => pi.Name);
			foreach (PropertyInfo propertyInfo in properties)
				if (propertyInfo.DeclaringType!=typeof(object))
					if (!hiddenPostfixes.Any(pf => propertyInfo.Name.EndsWith(pf)) && !propertyInfo.GetCustomAttributes(true).OfType<BrowsableAttribute>().Any(ba => !ba.Browsable))
						if (!propertyInfo.DeclaringType.FullName.StartsWith("System"))
							if (this.TypeFilter==null || this.TypeFilter(propertyInfo.PropertyType))
							{
								bool isNavigation = (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType!=typeof(string));
								bool isEnumerable = (propertyInfo.PropertyType.GetInterface("IEnumerable")!=null);

								TreeNode treeNode = new TreeNode();
								treeNode.Name = (parentKey+"."+propertyInfo.Name).Trim('.');
								treeNode.Text = propertyInfo.Name;
								treeNode.ImageKey = treeNode.SelectedImageKey = (!isNavigation) ? "Property" : ((isEnumerable) ? "EntitySet" : "EntityType");
								treeNode.Checked = checkedNodeNames.Contains(treeNode.Name);

								if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType!=typeof(string))
									this.AddMemberNodes(treeNode,propertyInfo.PropertyType,checkedNodeNames,deferredLoading);

								if (!this.RemoveEmptyTypes || (!isNavigation || treeNode.Nodes.Count>0))
									targetTreeNodeCollection.Add(treeNode);

								if (treeNode.Checked)
									treeNode.Parent.Expand();
							}
		}


		private void AddMemberNodes(TreeNode parentTreeNode,Type type,IEnumerable<string> checkedNodeNames,bool deferredLoading)
		{
			if (this.ResolveEnumerable)
			{
				Type enumerableType = type.GetInterfaces().Concat(new[] { type })
					.Where(itype => itype.IsGenericType && itype.GetGenericTypeDefinition()==typeof(IEnumerable<>))
					.Select(itype => itype.GetGenericArguments()[0])
					.FirstOrDefault();
				if (enumerableType!=null)
				{
					type = enumerableType;
					parentTreeNode.Name += "+";
				}
			}

			if (!deferredLoading)
				this.AddMemberNodes(parentTreeNode.Nodes,type,parentTreeNode.Name,checkedNodeNames,true);
			else
			{
				TreeViewCancelEventHandler treeViewCancelEventHandler = null;
				treeViewCancelEventHandler = delegate(object sender,TreeViewCancelEventArgs e)
				{
					if (e.Node==parentTreeNode.Parent)
					{
						if (!this.isUpdating)
						{
							WaitCursorChanger waitCursorChanger = new WaitCursorChanger(this);
							this.BeginUpdate();
							this.isUpdating = true;
							TreeViewEventHandler handler = null;
							handler = delegate
							{
								this.EndUpdate();
								this.isUpdating = false;
								waitCursorChanger.Dispose();
								this.AfterExpand -= handler;
							};
							this.AfterExpand += handler;
						}

						this.AddMemberNodes(parentTreeNode.Nodes,type,parentTreeNode.Name,checkedNodeNames,true);
						this.BeforeExpand -= treeViewCancelEventHandler;
						if (!this.isUpdating)
							this.EndUpdate();
					}
				};
				this.BeforeExpand += treeViewCancelEventHandler;
			}
		}


		// TODO
		public int HeightResolved
		{
			get { return this.ResolveHeight(this.Nodes) + this.Margin.Top+this.Margin.Bottom; }
		}


		// TODO
		private int ResolveHeight(TreeNodeCollection treeNodeCollection)
		{
			int result = 0;
			foreach (TreeNode treeNode in treeNodeCollection)
			{
				result += this.ItemHeight;
				if (treeNode.IsExpanded)
					result += this.ResolveHeight(treeNode.Nodes);
			}
			return result;
		}


		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			Font regularFont = e.Node.NodeFont ?? this.Font;
			
			FontStyle fontStyle = (e.Node.Checked) ? FontStyle.Bold : FontStyle.Regular;
			using (Font font = new Font(regularFont,fontStyle))
			{
				Color color = (e.State.HasFlag(TreeNodeStates.Focused)) ? SystemColors.HighlightText : SystemColors.ControlText;
				TextRenderer.DrawText(e.Graphics,e.Node.Text,font,e.Bounds.Location,color,TextFormatFlags.GlyphOverhangPadding);
			}

			if (!e.Node.Checked && !e.Node.IsExpanded)
			{
				int checkedChildsCount = e.Node.Flatten(n => n.Nodes.Cast<TreeNode>()).Count(n => n.Checked);
				if (checkedChildsCount>0)
				{
					Size size = TextRenderer.MeasureText(e.Node.Text,regularFont);
					using (Font font = new Font(regularFont,FontStyle.Bold))
						TextRenderer.DrawText(e.Graphics,"["+checkedChildsCount+"]",font,new Point(e.Bounds.X+e.Bounds.Width,e.Bounds.Y),SystemColors.ControlText,TextFormatFlags.GlyphOverhangPadding);
				}
			}
		}
	}
}
