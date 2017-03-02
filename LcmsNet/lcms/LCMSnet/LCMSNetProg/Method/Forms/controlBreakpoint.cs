using System;
using System.Windows.Forms;
using LcmsNet.Properties;

namespace LcmsNet.Method.Forms
{
    public partial class controlBreakpoint : UserControl
    {
        private bool m_set;

        public controlBreakpoint()
        {
            InitializeComponent();
            m_set = false;
            Click += controlBreakpoint_Click;
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
                Changed?.Invoke(this, new BreakpointArgs(m_set));
                if (m_set)
                {
                    BackgroundImage = Resources.breakpoint;
                }
                else
                {
                    BackgroundImage = Resources.breakpointDisabled;
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