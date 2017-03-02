using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNet.Method.Forms;
using LcmsNet.Method;

namespace LcmsNet.Simulator
{
    public partial class formChartPopoutWindow : Form
    {
        private UserControl control;
        private TabControl parentControl;

        public formChartPopoutWindow(UserControl cntrl, TabControl parentCntrl)
        {
            InitializeComponent();
            control = cntrl;
            parentControl = parentCntrl;
            panelControl.Controls.Add(control);
            cntrl.Refresh();
        }

        public void btnTack_OnClick(object sender, EventArgs e)
        {
            panelControl.Controls.Remove(control);
            var page = new TabPage();
            page.Text = this.Text;
            page.Controls.Add(control);
            parentControl.Controls.Add(page);
            this.Close();
        }

        public void OnClose()
        {
            control = null;
            parentControl = null;
        }
    }
}