using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace KaupischITC.Shared
{
	[DebuggerDisplay("DisplayedType = {DisplayedType}, SelectedProperty = {SelectedProperty}")]
	public class PropertyBrowser : ComboBoxControlHost
	{
		private ObjectBrowser objectBrowser = new ObjectBrowser();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedProperty
		{
			get { return (this.objectBrowser.SelectedNode!=null) ? this.objectBrowser.SelectedNode.Name.TrimStart('.') : null; }
			set
			{
				if (!this.DesignMode)
				{
					this.objectBrowser.CheckedNodes = new[] { value };
					this.DataSource = new object[] { this.SelectedProperty ?? String.Empty };
				}
			}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type DisplayedType
		{
			get { return this.objectBrowser.DisplayedType; }
			set
			{
				if (this.DisplayedType!=value)
				{
					string selectedProperty = this.SelectedProperty;
					this.objectBrowser.DisplayedType = value;
					this.SelectedProperty = selectedProperty;
				}
			}
		}

		
		public Func<Type,bool> TypeFilter
		{
			get { return this.objectBrowser.TypeFilter; }
			set { this.objectBrowser.TypeFilter = value; }
		}


		public PropertyBrowser()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			this.DropDownHeight *= 2;

			this.objectBrowser.ResolveEnumerable = false;
			this.objectBrowser.RemoveEmptyTypes = true;
			this.objectBrowser.HideSelection = false;
			this.objectBrowser.FullRowSelect = true;
			this.objectBrowser.BorderStyle = BorderStyle.None;
			this.objectBrowser.CheckBoxes = false;
			this.objectBrowser.VisibleChanged += new EventHandler(objectBrowser_VisibleChanged);
			this.objectBrowser.AfterSelect += new TreeViewEventHandler(objectBrowser_AfterSelect);
			this.SelectedValueChanged += new EventHandler(PropertyBrowser_SelectedValueChanged);

			this.HostedControl = objectBrowser;
		}


		private void PropertyBrowser_SelectedValueChanged(object sender,EventArgs e)
		{
			this.SelectedProperty = this.Text;
		}


		private void objectBrowser_AfterSelect(object sender,TreeViewEventArgs e)
		{
			if (!this.DesignMode)
			{
				this.HideDropDown();
				this.DataSource = new[] { this.SelectedProperty };
			}
		}


		private void objectBrowser_VisibleChanged(object sender,EventArgs e)
		{
			if (!this.objectBrowser.Visible)
				this.objectBrowser.Focus();
		}


		public object GetSelectedPropertyValue(object value)
		{
			if (value==null)
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
