using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet
{
	public partial class WindowManager : Form
	{
		public WindowManager()
		{
			InitializeComponent();
			this.Dock = DockStyle.Top;
			this.TopMost = true;
			this.SendToBack();
		}
	}
}
