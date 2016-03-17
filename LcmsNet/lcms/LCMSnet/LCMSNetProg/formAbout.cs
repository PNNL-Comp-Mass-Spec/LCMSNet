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
    public partial class formAbout : Form
    {
        public formAbout()
        {
            InitializeComponent();
            mlabel_version.Text += Application.ProductVersion;
        }
    }
}