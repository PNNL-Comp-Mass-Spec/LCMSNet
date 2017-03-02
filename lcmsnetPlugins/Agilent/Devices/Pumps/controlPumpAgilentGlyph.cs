using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;

namespace LcmsNet.Devices.Pumps
{
    public partial class controlPumpAgilentGlyph : UserControl, IDeviceGlyph
    {
        private classPumpAgilent mobj_agilentPump;
        formAgilentPumpPurge     mform_purges;

        public controlPumpAgilentGlyph()
        {
            InitializeComponent();
        }

        #region IDeviceGlyph Methods
        public void RegisterDevice(IDevice device)
        {
            mobj_agilentPump    = device as classPumpAgilent;
            mlabel_name.Text    = device.Name;
            mform_purges        = new formAgilentPumpPurge(mobj_agilentPump);
            mobj_agilentPump.MonitoringDataReceived += new EventHandler<PumpDataEventArgs>(mobj_agilentPump_MonitoringDataReceived);
            device.DeviceSaveRequired               += new EventHandler(DeviceSaveRequired);
        }

        void DeviceSaveRequired(object sender, EventArgs e)
        {
            if (mobj_agilentPump != null)
            {
                mlabel_name.Text = mobj_agilentPump.Name;
            }
        }
        public void DeRegisterDevice()
        {
            mobj_agilentPump.MonitoringDataReceived -= mobj_agilentPump_MonitoringDataReceived;
            mobj_agilentPump = null;
        }
        #endregion

        #region Properties
        public UserControl UserControl
        {
            get
            {
                return this;
            }
        }
        public int ZOrder
        {
            get;
            set;
        }
        #endregion
        
        #region Rendering
        private void DisplayMonitoringData(List<DateTime> time, List<double> pressure, List<double> flowrate, List<double> percentB)
        {
            mlabel_flowrate.Text    = string.Format("Flow: {0:0.000} uL/m", flowrate[flowrate.Count - 1]);
            mlabel_composition.Text = string.Format("Comp. B: {0:0.000} %", percentB[percentB.Count - 1]);
            mlabel_pressure.Text    = string.Format("Pressure: {0:0.000} bar", pressure[pressure.Count - 1]);
        }
        private delegate void DelegateDisplayMonitoringData(List<DateTime> time, List<double> pressure, List<double> flowrate, List<double> percentB);

        void mobj_agilentPump_MonitoringDataReceived(object sender, PumpDataEventArgs e)
        {
            mobj_agilentPump_MonitoringDataReceived(e.Pump as classPumpAgilent, e.Time, e.Pressure, e.Flowrate, e.PercentB);
        }
        void mobj_agilentPump_MonitoringDataReceived(classPumpAgilent pump, List<DateTime> time, List<double> pressure, List<double> flowrate, List<double> percentB)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateDisplayMonitoringData(DisplayMonitoringData), new object[] { time, pressure, flowrate, percentB });
            }
            else
            {
                DisplayMonitoringData(time, pressure, flowrate, percentB);
            }            
        }
        #endregion

        
        private void mbutton_purge_Click(object sender, EventArgs e)
        {
            if (mform_purges != null)
            {
                mform_purges.ShowDialog();
            }
        }

        /// <summary>
        /// Gets or sets the status display bar.
        /// </summary>
        public controlDeviceStatusDisplay StatusBar
        {
            get;
            set;
        }
    }
}
