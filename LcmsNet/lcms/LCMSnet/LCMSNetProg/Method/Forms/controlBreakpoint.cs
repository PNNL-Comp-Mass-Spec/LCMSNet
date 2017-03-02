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
        private bool m_set;

        public controlBreakpoint()
        {
            InitializeComponent();
            m_set = false;
            this.Click += new EventHandler(controlBreakpoint_Click);
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSet
        {
            get { return m_set; }
            set
            {
                if (m_set == value)
                    return;

                m_set = value;
                if (Changed != null)
                {
                    Changed(this, new BreakpointArgs(m_set));
                }
                if (m_set)
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
            IsSet = (m_set == false);
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