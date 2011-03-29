using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KaupischITC.Shared
{
	/// <summary>
	/// Stellt ein ComboBox-Steuerelement dar, das andere Steuerelemente als DropDown anbieten kann
	/// </summary>
	public class ComboBoxControlHost : ComboBox
	{
		private ToolStripControlHost toolStripControlHost;						// kann Controls hosten
		private ToolStripDropDown toolStripDropDown = new ToolStripDropDown();	// DropDown, das den ToolStripControlHost enthalten kann

		// WindowMessages
		private const int WM_USER = 0x0400;
		private const int WM_REFLECT = WM_USER + 0x1C00;
		private const int WM_COMMAND = 0x0111;
		private const int CBN_DROPDOWN = 7;
		private const int WM_CLICK = 513;



		/// <summary>
		/// Gibt das Steuerelement, dass beim DropDown angezeigt wird, zurück oder legt dieses fest
		/// </summary>
		public Control HostedControl
		{
			get { return (toolStripControlHost!=null) ? this.toolStripControlHost.Control : null; }
			set
			{
				this.toolStripDropDown.Items.Clear();
				if (value!=null)
				{
					this.toolStripControlHost = new ToolStripControlHost(value);
					this.toolStripDropDown.Items.Add(this.toolStripControlHost);
				}
			}
		}



		/// <summary>
		/// Erstellt ein neues ComboBoxControlHost-Steuerelement
		/// </summary>
		public ComboBoxControlHost()
		{
			this.toolStripDropDown.Opened += delegate
			{
				if (this.HostedControl!=null)
					this.HostedControl.Focus();
			};
		}



		/// <summary>
		/// Zeigt den DropDown an
		/// </summary>
		public void ShowDropDown()
		{
			this.toolStripControlHost.Control.Width = this.DropDownWidth;
			this.toolStripControlHost.Control.Height = this.DropDownHeight;
			this.toolStripDropDown.Show(this,0,this.Height);
		}


		public void HideDropDown()
		{
			this.toolStripDropDown.Hide();
		}


		/// <summary>
		/// Retrieves the high-order word from the given 32-bit value. 
		/// </summary>
		/// <param name="value">Specifies the value to be converted. </param>
		/// <returns>The high-order word of the specified value. </returns>
		private static int HIWORD(int value)
		{
			return (value >> 16) & 0xffff;
		}


		/// <summary>
		/// Verarbeitet Windows-Meldungen
		/// </summary>
		/// <param name="message">Die zu verarbeitende Windows-Message</param>
		protected override void WndProc(ref Message message)
		{
			if ((message.Msg==(WM_REFLECT + WM_COMMAND) && HIWORD((int)message.WParam)==CBN_DROPDOWN) || message.Msg == WM_CLICK)
				this.ShowDropDown();
			else
				base.WndProc(ref message);
		}


		/// <summary>
		/// Gibt nicht mehr verwendete Ressourcen frei
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				this.toolStripDropDown.Dispose();
			base.Dispose(disposing);
		}
	}
}
