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

					if (value!=null)
						foreach (string nodeName in value)
							if (!String.IsNullOrEmpty(nodeName))
							{
								TreeNode treeNode = this.GetNode(this.Nodes[0],nodeName);
								treeNode.Checked = true;
								this.SelectedNode = treeNode; // TODO
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

			char separator = '.';
			if (nodeName.Contains(separator))
			{
				string childNodeName = (parentTreeNode.Name+"."+nodeName.Substring(0,nodeName.IndexOf(separator))).Trim('.');
				TreeNode childNode = parentTreeNode.Nodes[childNodeName];
				if (childNode==null)
					childNode = parentTreeNode.Nodes.Add(childNodeName,childNodeName,"QuestionMark","QuestionMark");
				else
				{
					this.OnBeforeExpand(new TreeViewCancelEventArgs(childNode,false,TreeViewAction.Expand));
					childNode.Expand();
				}

				return this.GetNode(childNode,nodeName.Substring(nodeName.IndexOf(separator)+1));
			}
			else
				return parentTreeNode.Nodes[(parentTreeNode.Name+"."+nodeName).Trim('.')] ?? parentTreeNode.Nodes.Add(nodeName,nodeName,"QuestionMark","QuestionMark");
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
			IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance|BindingFlags.Public);
			if (!type.IsCompilerGenerated())
				properties = properties.OrderBy(pi => pi.Name);
			foreach (PropertyInfo propertyInfo in properties)
				if (propertyInfo.DeclaringType!=typeof(object))
					if (!propertyInfo.Name.EndsWith("ID") && !propertyInfo.Name.EndsWith("Id") && !propertyInfo.GetCustomAttributes(true).OfType<BrowsableAttribute>().Any(ba => !ba.Browsable))
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
	}
}
