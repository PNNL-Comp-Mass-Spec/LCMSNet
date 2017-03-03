using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace ASUTGen.Devices.Valves
{
    public partial class IDEXValveControl : UserControl, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXValve m_valve;

        public IDEXValveControl()
        {
            InitializeComponent();
        }

        private void mbutton_injectFailure_Click(object sender, EventArgs e)
        {
            m_valve.ChangePosition(100, 100);
        }
        
        public void RegisterDevice(IDevice device)
        {
            m_valve = device as IDEXValve;
        }
        #region IDeviceControl Members

        public event DelegateNameChanged  NameChanged;
        public event DelegateSaveRequired SaveRequired;

        public bool Running
        {
            get;
            set;
        }

        public IDevice Device
        {
            get
            {
                return m_valve;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        public void ShowProps()
        {
            
        }
        #endregion
    }
}
