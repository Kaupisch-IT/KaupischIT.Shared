using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

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
			this.objectBrowser.VisibleChanged += objectBrowser_VisibleChanged;
			this.objectBrowser.AfterSelect += objectBrowser_AfterSelect;
			this.SelectedValueChanged += PropertyBrowser_SelectedValueChanged;

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
				this.objectBrowser.Focus(); // TODO: Warum? Wegen Focus und so!?
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
