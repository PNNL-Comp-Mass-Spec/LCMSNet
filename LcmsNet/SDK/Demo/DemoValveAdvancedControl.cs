using System;
using LcmsNetDataClasses.Devices;

namespace DemoPluginLibrary
{
    public partial class DemoValveAdvancedControl : controlBaseDeviceControl, IDeviceControl
    {

        DemoValve m_valve;

        public DemoValveAdvancedControl()
        {
            InitializeComponent();
        }

        private void btnStateA_Click(object sender, EventArgs e)
        {
            m_valve.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionA);
        }

        private void btnStateB_Click(object sender, EventArgs e)
        {
            m_valve.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionB);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtState.Text = ((FluidicsSDK.Base.TwoPositionState)m_valve.GetPosition()).ToString();
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
                return m_valve;
            }
            set
            {
                m_valve = value as DemoValve;
                SetBaseDevice(value);
            }
        }
    }
}
