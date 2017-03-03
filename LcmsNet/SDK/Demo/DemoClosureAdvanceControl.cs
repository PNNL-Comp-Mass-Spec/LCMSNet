using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetSDK;
using LcmsNetDataClasses.Devices;

namespace DemoPluginLibrary
{
    public partial class DemoClosureAdvanceControl : controlBaseDeviceControl, IDeviceControl
    {

        private DemoClosure m_closure;

        public DemoClosureAdvanceControl()
        {
            InitializeComponent();
        }

        public void RegisterDevice(IDevice device)
        {
            Device = device;
        }

        public void UnRegisterDevice()
        {
            Device = null;
        }

        public IDevice Device
        {
            get
            {
                return m_closure;
            }
            set
            {
                m_closure = value as DemoClosure;
                SetBaseDevice(value);
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            m_closure.Trigger(Convert.ToInt32(numPulseLength.Value), "Port1", Convert.ToDouble(numPulseLength.Value));
        }
    }
}
