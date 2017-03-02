using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet.Method.Forms
{
    public partial class controlBreakpoint : UserControl
    {
        private bool mbool_set;

        public controlBreakpoint()
        {
            InitializeComponent();
            mbool_set = false;
            this.Click += new EventHandler(controlBreakpoint_Click);
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSet
        {
            get { return mbool_set; }
            set
            {
                if (mbool_set == value)
                    return;

                mbool_set = value;
                if (Changed != null)
                {
                    Changed(this, new BreakpointArgs(mbool_set));
                }
                if (mbool_set)
                {
                    BackgroundImage = global::LcmsNet.Properties.Resources.breakpoint;
                }
                else
                {
                    BackgroundImage = global::LcmsNet.Properties.Resources.breakpointDisabled;
                }
            }
        }

        void controlBreakpoint_Click(object sender, EventArgs e)
        {
            IsSet = (mbool_set == false);
        }

        public event EventHandler<BreakpointArgs> Changed;
    }

    public class BreakpointArgs : EventArgs
    {
        public BreakpointArgs(bool set)
        {
            IsSet = set;
        }

        public bool IsSet { get; private set; }
    }
}