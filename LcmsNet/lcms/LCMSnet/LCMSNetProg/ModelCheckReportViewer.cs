using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses;

namespace LcmsNet
{
    public partial class ModelCheckReportViewer : UserControl
    {
        IModelCheckController controller;

        public ModelCheckReportViewer()
        {
            InitializeComponent();
        }

        public ModelCheckReportViewer(IModelCheckController cntrlr)
        {
            InitializeComponent();
            controller = cntrlr;
            controller.ModelStatusChangeEvent += StatusChangeHandler;
        }

        private void StatusChangeHandler(object sender, ModelStatusChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ModelStatusChangeEventArgs>(StatusChangeHandler), new object[2] { sender, e});
            }
            else
            {
                foreach (ModelStatus status in e.StatusList)
                {
                    var report = new ModelCheckReport(status);
                    this.panelMessages.Controls.Add(report);
                    report.Dock = DockStyle.Top;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            panelMessages.Controls.Clear();
            ((Control)sender).Refresh(); // this causes the control to redraw, which removes the autoscroll bars, if present.
        }
        
    }
}
