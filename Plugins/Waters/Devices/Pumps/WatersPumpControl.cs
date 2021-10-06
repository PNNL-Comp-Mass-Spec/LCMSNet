using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using System.Collections.Generic;
using System;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices.Pumps;

namespace Waters.Devices.Pumps
{
    public partial class WatersPumpControl : controlBaseDeviceControl, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private WatersPump mobj_pump;

        public WatersPumpControl()
        {
            InitializeComponent();
        }
        
        

        public void RegisterDevice(IDevice device)
        {
            mobj_pump = device as WatersPump;
            SetBaseDevice(mobj_pump);

            if (mobj_pump != null)
            {
                mobj_pump.InstrumentsFound  += new EventHandler<WatersEventArgs>(mobj_pump_InstrumentsFound);
                mobj_pump.MethodsFound      += new EventHandler<WatersEventArgs>(mobj_pump_MethodsFound);
                mobj_pump.MonitoringDataReceived += new EventHandler<PumpDataEventArgs>(mobj_pump_MonitoringDataReceived);

                mobj_pump.PumpStatus += new EventHandler<WatersEventArgs>(mobj_pump_PumpStatus);

                mtext_computerName.Text      = mobj_pump.MachineName;
                mtext_systemName.Text        = mobj_pump.SystemName;
            }
        }

        void mobj_pump_PumpStatus(object sender, WatersEventArgs e)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new EventHandler<WatersEventArgs>(mobj_pump_PumpStatus), new object[] { sender, e });
            }
            else
            {
                m_statusLabel.Text = e.Status;
            }
        }

        /// <summary>
        /// Handles the event when data is received from the pumps.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pressure"></param>
        /// <param name="flowrate"></param>
        /// <param name="percentB"></param>
        void mobj_pump_MonitoringDataReceived(object sender, PumpDataEventArgs args)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new EventHandler<PumpDataEventArgs>(mcontrol_pumpDisplay.DisplayMonitoringData), new object[] { sender, args });
            }
            else
            {
                mcontrol_pumpDisplay.DisplayMonitoringData(sender, args);
            }
        }

        void mobj_pump_InstrumentsFound(object sender, WatersEventArgs e)
        {
            mlist_instruments.Items.Clear();
            foreach (string method in e.Data)
            {
                mlist_instruments.Items.Add(method);
            }
            
        }

        void mobj_pump_MethodsFound(object sender, WatersEventArgs e)
        {
            mcomboBox_methods.Items.Clear();
            foreach (string method in e.Data)
            {
                mcomboBox_methods.Items.Add(method);
            }
            if (mcomboBox_methods.Items.Count > 0)
            {
                mcomboBox_methods.SelectedIndex = 0;
            }
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
                return mobj_pump;
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

        private void button1_Click(object sender, System.EventArgs e)
        {
            mobj_pump.ShowConsole();
        }        

        private void tabPage2_Click(object sender, System.EventArgs e)
        {

        }

        private void m_buttonTestMethod_Click_1(object sender, System.EventArgs e)
        {
            string methodName = "";
            double timeout    = Convert.ToDouble(mnum_methodLength.Value);
            if (mcomboBox_methods.Text != null)
            {
                classApplicationLogger.LogMessage(0,
                    string.Format("Manually starting Waters Method {0} for {1} mins.", methodName, timeout));
                methodName = mcomboBox_methods.Text as string;
                mobj_pump.StartMethod(timeout, methodName);
            }
            else
            {

                classApplicationLogger.LogMessage(0,
                    string.Format("Cannot start a method when no method is selected.", methodName, timeout));
            }
        }

        private void mbutton_getListOfMethods_Click(object sender, System.EventArgs e)
        {
            List<string> methods = mobj_pump.GetMethodList();
        }

        private void mbutton_stopMethod_Click(object sender, EventArgs e)
        {
            mobj_pump.StopMethod();
        }

        private void mbutton_setInstrument_Click(object sender, EventArgs e)
        {
            if (mlist_instruments.SelectedItem != null)
            {
                string item = mlist_instruments.SelectedItem as string;
                mobj_pump.Instrument = item;
            }
            else
            {
                classApplicationLogger.LogError(0, "You have to select a nano BSM pump first.");
            }
        }

        private void mbutton_scanInstruments_Click(object sender, EventArgs e)
        {
            classApplicationLogger.LogMessage(0, "Scanning network for Waters pump list.");
            List<string> instruments = mobj_pump.GetInstrumentList();
            classApplicationLogger.LogMessage(0, string.Format("Found {0} Water's Nano BSM pumps.", instruments.Count));
        }

        private void mbutton_setComputerName_Click(object sender, EventArgs e)
        {
            mobj_pump.MachineName = mtext_computerName.Text;
        }

        private void mbutton_setSystemName_Click(object sender, EventArgs e)
        {
            mobj_pump.SystemName = mtext_systemName.Text;
        }
    }
}
