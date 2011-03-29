using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KaupischITC.InfragisticsControls
{
	internal partial class FormDetails : Form
	{
		public FormDetails(object value)
		{
			this.Font = SystemFonts.MessageBoxFont;
			InitializeComponent();

			this.Text = value.GetType().Name;
			this.customizedUltraGrid.DisplayLayout.MaxBandDepth = 5;
			this.customizedUltraGrid.DataSource = new List<object> { value };
			//this.customizedUltraGrid.Rows.ExpandAll(false);
		}
	}
}
