using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using KaupischITC.Extensions;

namespace KaupischITC.Shared
{
	[DebuggerDisplay("DisplayedType = {DisplayedType}, SelectedProperty = {SelectedProperty}")]
	public class PropertyBrowser : ComboBoxControlHost
	{
		private ObjectBrowser objectBrowser = new ObjectBrowser();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedProperty
		{
			get { return this.objectBrowser.SelectedProperty; }
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
			get { return this.objectBrowser.DisplayedType; }
			set
			{
				if (this.DisplayedType!=value)
					this.objectBrowser.DisplayedType = value;
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
			return this.objectBrowser.GetSelectedPropertyInfo(type);
		}


		public object GetSelectedPropertyValue(object value)
		{
			return this.objectBrowser.GetSelectedPropertyValue(value);
		}
	}
}
