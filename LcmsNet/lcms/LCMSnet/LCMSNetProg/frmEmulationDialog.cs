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
    public partial class frmEmulationDialog : Form
    {
        public frmEmulationDialog()
        {
            InitializeComponent();
            string warningMessage = "Emulation mode is not enabled!" + Environment.NewLine +
                                    "Running a simulation now will result in inaccurate simulation" +
                                    Environment.NewLine + "and may result in operation of actual hardware." +
                                    Environment.NewLine + "Do you wish to enable emulation mode?";
            lblMessage.Text = warningMessage;
            this.Icon = SystemIcons.Warning;
        }
    }
}