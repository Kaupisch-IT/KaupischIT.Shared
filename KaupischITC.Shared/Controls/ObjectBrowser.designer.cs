﻿
namespace KaupischITC.Shared
{
	partial class ObjectBrowser
	{
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectBrowser));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
			this.imageList.Images.SetKeyName(0,"Method");
			this.imageList.Images.SetKeyName(1,"NavigationProperty");
			this.imageList.Images.SetKeyName(2,"Class");
			this.imageList.Images.SetKeyName(3,"ComplexType");
			this.imageList.Images.SetKeyName(4,"EntitySet");
			this.imageList.Images.SetKeyName(5,"Property");
			this.imageList.Images.SetKeyName(6,"Property");
			this.imageList.Images.SetKeyName(7,"QuestionMark");
			this.imageList.Images.SetKeyName(8,"EntityType");
			// 
			// ObjectBrowser
			// 
			this.LineColor = System.Drawing.Color.Black;
			this.ResumeLayout(false);

		}
	}
}
