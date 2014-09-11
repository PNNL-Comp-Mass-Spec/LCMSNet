using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace DemoPluginLibrary
{
    public partial class DemoValve2AdvancedControl : controlBaseDeviceControl, IDeviceControl
    {
        DemoValve2 m_device;

        public DemoValve2AdvancedControl()
        {
            InitializeComponent();
            foreach(var value in Enum.GetValues(typeof(FluidicsSDK.Base.EightPositionState)))
            {
                comboPosition.Items.Add(value);
            }
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
                return m_device as IDevice;
            }
            set
            {
                m_device = value as DemoValve2;
                SetBaseDevice(value);
            }
        }

        public void btnSetPosition_Click(object sender, EventArgs e)
        {
            m_device.SetPosition((FluidicsSDK.Base.EightPositionState)Enum.Parse(typeof(FluidicsSDK.Base.EightPositionState), comboPosition.SelectedItem.ToString()));
        }

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            txtState.Text = ((FluidicsSDK.Base.EightPositionState)m_device.GetPosition()).ToString();
        }
    }
}
