using System;
using LcmsNetDataClasses.Devices;

namespace DemoPluginLibrary
{
    public partial class DemoPALAdvancedControl : controlBaseDeviceControl, IDeviceControl
    {
        DemoPAL m_PALdevice;

        public DemoPALAdvancedControl()
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
                return m_PALdevice;
            }
            set
            {
                m_PALdevice = value as DemoPAL;
                SetBaseDevice(value);
            }
        }

        private void btnRunMethod_Click(object sender, EventArgs e)
        {
            // use a defaulted sampledata object since there's no sample associated with a user clicking "run"
            m_PALdevice.RunMethod(Convert.ToDouble(numTimeout.Value), new LcmsNetDataClasses.classSampleData(), comboMethod.SelectedItem.ToString());
        }


    }
}
