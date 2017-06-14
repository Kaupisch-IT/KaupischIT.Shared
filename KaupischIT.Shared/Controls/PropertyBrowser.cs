using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using KaupischIT.Extensions;

namespace KaupischIT.Shared
{
	[DebuggerDisplay("DisplayedType = {DisplayedType}, SelectedProperty = {SelectedProperty}")]
	public class PropertyBrowser : ComboBoxControlHost
	{
		private ObjectBrowser objectBrowser = new ObjectBrowser();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedProperty
		{
			get => this.objectBrowser.SelectedProperty;
			set
			{
				if (!this.DesignMode)
				{
					this.objectBrowser.SelectedProperty = value;
					this.DataSource = new object[] { this.SelectedProperty ?? String.Empty };
				}
			}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Type DisplayedType
		{
			get => this.objectBrowser.DisplayedType;
			set
			{
				if (this.DisplayedType!=value)
					this.objectBrowser.DisplayedType = value;
			}
		}


		public Func<Type,bool> TypeFilter
		{
			get => this.objectBrowser.TypeFilter;
			set => this.objectBrowser.TypeFilter = value;
		}


		public PropertyBrowser()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
			this.DropDownHeight *= 2;

			this.objectBrowser.HideSelection = false;
			this.objectBrowser.FullRowSelect = true;
			this.objectBrowser.BorderStyle = BorderStyle.None;
			this.objectBrowser.CheckBoxes = false;
			this.objectBrowser.VisibleChanged += this.ObjectBrowser_VisibleChanged;
			this.objectBrowser.AfterSelect += this.ObjectBrowser_AfterSelect;
			this.SelectedValueChanged += this.PropertyBrowser_SelectedValueChanged;

			this.HostedControl = this.objectBrowser;
		}


		private void PropertyBrowser_SelectedValueChanged(object sender,EventArgs e) => this.SelectedProperty = this.Text;


		private void ObjectBrowser_AfterSelect(object sender,TreeViewEventArgs e)
		{
			if (!this.DesignMode)
			{
				this.HideDropDown();
				this.DataSource = new[] { this.SelectedProperty };
			}
		}


		private void ObjectBrowser_VisibleChanged(object sender,EventArgs e)
		{
			if (!this.objectBrowser.Visible)
				this.objectBrowser.Focus(); // TODO: Warum? Wegen Focus und so!?
		}


		public PropertyInfo GetSelectedPropertyInfo(Type type) => this.objectBrowser.GetSelectedPropertyInfo(type);


		public object GetSelectedPropertyValue(object value) => this.objectBrowser.GetSelectedPropertyValue(value);
	}
}
