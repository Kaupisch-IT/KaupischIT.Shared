using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using KaupischIT.Extensions;

namespace KaupischIT.Shared
{
	public partial class ObjectBrowser : TreeView
	{
		private bool isUpdating = false;
		private ImageList imageList;
		private IContainer components;

		public Func<Type,bool> TypeFilter { get; set; }


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
		public string SelectedProperty
		{
			get => this.SelectedNode?.Name.TrimStart('.');
			set
			{
				if (!this.DesignMode)
					this.SelectedNode = this.Nodes.Cast<TreeNode>().SelectMany(node => node.Flatten(tn => tn.Nodes.Cast<TreeNode>())).FirstOrDefault(tn => tn.Name==value);
			}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type DisplayedType
		{
			get => this.displayedType;
			set
			{
				if (!this.DesignMode && !this.AreEquivalentTypes(this.displayedType,value))
					using (new WaitCursorChanger(this))
					{
						this.BeginUpdate();
						this.isUpdating = true;

						string selectedProperty = this.SelectedProperty;
						this.Nodes.Clear();

						if (value!=null)
						{
							TreeNode treeNode = new TreeNode();
							treeNode.Text = (!value.IsCompilerGenerated()) ? value.Name : "(generierter Typ)";
							treeNode.ImageKey = treeNode.SelectedImageKey = "ComplexType";
							this.Nodes.Add(treeNode);
							this.AddMemberNodes(treeNode.Nodes,value,"",false);
							treeNode.Expand();
						}
						else
							this.Nodes.Add("","(unbekannter Typ)","QuestionMark");

						this.SelectedProperty = selectedProperty;
						this.EndUpdate();
						this.isUpdating = false;
						this.displayedType = value;
					}
			}
		}
		private Type displayedType;


		private bool AreEquivalentTypes(Type first,Type second)
		{
			if (first==null || second==null)
				return (first==second);

			if (first.ImplementsInterface(typeof(IEnumerable<>)) && second.ImplementsInterface(typeof(IEnumerable<>)))
			{
				Type getElementType(Type type) => type.GetInterfaces().Concat(new[] { type }).First(itype => itype.IsGenericType && itype.GetGenericTypeDefinition()==typeof(IEnumerable<>)).GetGenericArguments()[0];
				return this.AreEquivalentTypes(getElementType(first),getElementType(second));
			}

			if (first!=second && (first!=null && first.IsCompilerGenerated()) && (second!=null && second.IsCompilerGenerated()))
			{
				PropertyInfo[] firstProperties = first.GetProperties();
				PropertyInfo[] secondProperties = second.GetProperties();

				if (!firstProperties.All(pi1 => secondProperties.Any(pi2 => pi1.Name==pi2.Name && this.AreEquivalentTypes(pi1.PropertyType,pi2.PropertyType))))
					return false;
				else if (!secondProperties.All(pi2 => firstProperties.Any(pi1 => pi2.Name==pi1.Name && this.AreEquivalentTypes(pi2.PropertyType,pi1.PropertyType))))
					return false;
				else
					return true;
			}
			else
				return (first==second);
		}

		private void AddMemberNodes(TreeNodeCollection targetTreeNodeCollection,Type type,string parentKey,bool deferredLoading)
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
								bool isEnumerable = (isNavigation && propertyInfo.PropertyType.GetInterface("IEnumerable")!=null);
								if (!isEnumerable)
								{
									TreeNode treeNode = new TreeNode();
									treeNode.Name = (parentKey+"."+propertyInfo.Name).Trim('.');
									treeNode.Text = propertyInfo.Name;
									treeNode.ImageKey = treeNode.SelectedImageKey = (!isNavigation) ? "Property" : "EntityType";

									if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType!=typeof(string))
										this.AddMemberNodes(treeNode,propertyInfo.PropertyType,deferredLoading);

									if (!isNavigation || treeNode.Nodes.Count>0)
										targetTreeNodeCollection.Add(treeNode);
								}
							}
		}


		private void AddMemberNodes(TreeNode parentTreeNode,Type type,bool deferredLoading)
		{
			if (!deferredLoading)
				this.AddMemberNodes(parentTreeNode.Nodes,type,parentTreeNode.Name,true);
			else
			{
				void treeViewCancelEventHandler(object sender,TreeViewCancelEventArgs e)
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

						this.AddMemberNodes(parentTreeNode.Nodes,type,parentTreeNode.Name,true);
						this.BeforeExpand -= treeViewCancelEventHandler;
						if (!this.isUpdating)
							this.EndUpdate();
					}
				}

				this.BeforeExpand += treeViewCancelEventHandler;
			}
		}


		public PropertyInfo GetSelectedPropertyInfo(Type type)
		{
			PropertyInfo result = null;
			foreach (string propertyName in this.SelectedProperty.Split('.'))
			{
				result = type.GetProperty(propertyName);
				if (result==null)
					return null;
				else
					type = result.PropertyType;
			}
			return result;
		}


		public object GetSelectedPropertyValue(object value)
		{
			if (value==null || String.IsNullOrEmpty(this.SelectedProperty))
				return null;
			else
			{
				foreach (string propertyName in this.SelectedProperty.Split('.'))
				{
					PropertyInfo propertyInfo = value.GetType().GetProperty(propertyName);
					value = propertyInfo.GetValue(value,null);
				}
				return value;
			}
		}
	}
}
