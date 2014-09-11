using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

//TODO: Events

namespace LcmsNet.Devices.Pumps
{
    public partial class controlPumpAgilent : controlBaseDeviceControl, IDeviceControl
    {
        #region Members

        public classPumpAgilent mobj_pump;

        #endregion

        #region Events

        //Nothing to see here

        #endregion

        #region Constructors

        public controlPumpAgilent()
        {
            InitializeComponent();
            mobj_pump = new classPumpAgilent();
            mobj_pump.Initialize();
            mcomboBox_Mode.DataSource = System.Enum.GetValues(typeof(enumPumpAgilentModes));
            mcomboBox_Mode.SelectedItem = mobj_pump.GetMode();

            mobj_device = mobj_pump;
            classDeviceManager.Manager.AddDevice(mobj_device);
        }

        #endregion

        #region Properties

        public IDevice Device
        {
            get
            {
                return (IDevice)mobj_pump;
            }
        }

        #endregion

        #region Methods

        public override bool RemoveDevice()
        {
            //classDeviceManger...blahblah
            return false;
        }

        private void mbutton_SetFlowRate_Click(object sender, EventArgs e)
        {
            mobj_pump.SetFlowRate(Convert.ToDouble(mtextBox_setFlow.Text));
        }

        private void mbutton_GetFlowRate_Click(object sender, EventArgs e)
        {
            mtextBox_ActualFlowRate.Text = mobj_pump.GetActualFlow().ToString();
        }

        private void mbutton_SetMixerVol_Click(object sender, EventArgs e)
        {
            mobj_pump.SetMixerVolume(Convert.ToDouble(mtextBox_SetMixerVol.Text));
        }

        private void mbutton_GetMixerVol_Click(object sender, EventArgs e)
        {
            mtextBox_GetMixerVol.Text = mobj_pump.GetMixerVolume().ToString();
        }

        private void mbutton_GetPressure_Click(object sender, EventArgs e)
        {
            mtextBox_Pressure.Text = mobj_pump.GetPressure().ToString();
        }

        private void mbutton_SetMode_Click(object sender, EventArgs e)
        {
            mobj_pump.SetMode((enumPumpAgilentModes)mcomboBox_Mode.SelectedValue);
        }

        private void controlPumpAgilent_Load(object sender, EventArgs e)
        {
            mpropertyGrid_SerialSettings.SelectedObject = mobj_pump.Port;
        }

        #endregion
    }
}
