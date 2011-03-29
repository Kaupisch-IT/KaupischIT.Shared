using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	public class SeparatorLabel : UserControl
	{
		private readonly Label label;

		public SeparatorLabel()
		{
			this.label = new System.Windows.Forms.Label() { AutoSize = true };
			this.label.SizeChanged += new System.EventHandler(this.Label_SizeChanged);
			this.Controls.Add(this.label);

			this.AutoScaleMode = AutoScaleMode.None;
			this.TabStop = false;
			this.DoLayout();
		}


		[Browsable(true)]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text
		{
			get { return this.label.Text; }
			set { this.label.Text = value; }
		}

		private void Label_SizeChanged(object sender,EventArgs e)
		{
			this.DoLayout();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			using (Pen pen = new Pen(SystemColors.ControlDark))
				e.Graphics.DrawLine(pen,this.label.Width + 2,this.Height/2 + 1,this.Width,this.Height/2 + 1);

			using (Pen pen = new Pen(SystemColors.ControlLightLight))
				e.Graphics.DrawLine(pen,this.label.Width + 2,this.Height/2 + 2,this.Width,this.Height/2 + 2);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			this.DoLayout();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.DoLayout();
		}

		private void DoLayout()
		{
			this.Height = this.label.Height;
		}
	}
}
