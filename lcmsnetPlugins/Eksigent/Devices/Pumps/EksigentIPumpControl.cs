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
using EksigentNanoLC;
//using DirectControl;

namespace Eksigent.Devices.Pumps
{
    public partial class EksigentPumpControl : controlBaseDeviceControl, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private EksigentPump mobj_pump;
        /// <summary>
        /// Delegate for updating text on the form from another thread.
        /// </summary>
        private EventHandler<classDeviceStatusEventArgs> m_statusUpdateDelegate;

        public EksigentPumpControl()
        {
            InitializeComponent();

            m_statusUpdateDelegate = new EventHandler<classDeviceStatusEventArgs>(UpdateStatusLabelText);
        }
        
        public void RegisterDevice(IDevice device)
        {
            mobj_pump                   = device as EksigentPump;
            mobj_pump.Error             += new EventHandler<classDeviceErrorEventArgs>(mobj_pump_Error);
            mobj_pump.StatusUpdate      += new EventHandler<classDeviceStatusEventArgs>(mobj_pump_StatusUpdate);
            mobj_pump.RequiresOCXInitialization += new EventHandler(mobj_pump_RequiresOCXInitialization);
            mobj_pump.PumpStatus        += new EventHandler<classDeviceStatusEventArgs>(mobj_pump_PumpStatus);
            mobj_pump.MethodNames       += new DelegateDeviceHasData(mobj_pump_MethodNames);
            mobj_pump.ChannelNumbers    += new EksigentPump.DelegateChannelNumbers(mobj_pump_ChannelNumbers);
            SetBaseDevice(device);
        }

        void mobj_pump_RequiresOCXInitialization(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch(Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not register the Eksigent control software OCX. " + ex.Message, ex);
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
        #endregion
               
        #region Pump Event Handlers
        /// <summary>
        /// Handles the channel numbers.
        /// </summary>
        /// <param name="totalChannels"></param>
        void mobj_pump_ChannelNumbers(int totalChannels)
        {
            int value = Convert.ToInt32(mnum_channels.Value);

            if (value > totalChannels)
            {
                mnum_channels.Value = totalChannels; 
            }

            mnum_channels.Minimum = Math.Min(1, totalChannels);
            mnum_channels.Maximum = totalChannels;
        }
        void UpdateStatusLabelText(object sender, classDeviceStatusEventArgs e)
        {
            mlabel_pumpStatus.Text = e.Message;
        }
        /// <summary>
        /// Handles pump status changes, not just status of the object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mobj_pump_PumpStatus(object sender, classDeviceStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(m_statusUpdateDelegate, new object[] { sender, e });
            }
            else
            {
                UpdateStatusLabelText(sender, e);
            }
        }
        /// <summary>
        /// Updates the list box with the appropiate method names.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void mobj_pump_MethodNames(object sender, List<object> data)
        {
            mcomboBox_methods.BeginUpdate();
            mcomboBox_methods.Items.Clear();
            foreach (object datum in data)
            {
                mcomboBox_methods.Items.Add(datum);
            }
            mcomboBox_methods.EndUpdate();

            if (mcomboBox_methods.Items.Count > 0)
            {
                mcomboBox_methods.SelectedIndex = 0;
            }
        }
        void mobj_pump_StatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            base.UpdateStatusDisplay(e.Message);
        }
        void mobj_pump_Error(object sender, classDeviceErrorEventArgs e)
        {
            base.UpdateStatusDisplay(e.Error);
        }
        #endregion 

        #region Button Event Handlers
        private void mbutton_updateMethods_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            mobj_pump.GetMethods();
        }
        private void mbutton_methodMenu_Click(object sender, EventArgs e)
        {

            UpdateStatusDisplay("");
            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            object methodData = mcomboBox_methods.SelectedItem;
            if (methodData == null)
            {
                UpdateStatusDisplay("Select a method first.");
                return;
            }
            mobj_pump.ShowMethodMenu(Convert.ToInt32(mnum_channels.Value), methodData.ToString());
        }
        private void mbutton_DirectControl_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            try
            {
                mobj_pump.ShowDirectControl(Convert.ToInt32(mnum_channels.Value), this.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_mobilePhase_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            
            try
            {
                mobj_pump.ShowMobilePhaseMenu(Convert.ToInt32(mnum_channels.Value), this.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_instrumentConfig_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            
            try
            {
                mobj_pump.ShowInstrumentConfigMenu();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_advancedSettings_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            
            try
            {
                mobj_pump.ShowAdvancedSettings(this.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_diagnosticsMenu_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }

            try
            {
                mobj_pump.ShowDiagnosticsMenu(Convert.ToInt32(mnum_channels.Value), this.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_mainWindow_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            
            try
            {
                mobj_pump.ShowMainWindow(this.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_AlertsMenu_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            
            try
            {
                mobj_pump.ShowAlertsMenu();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        private void mbutton_start_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");

            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }
            try
            {
                object methodData = mcomboBox_methods.SelectedItem;
                if (methodData == null)
                {
                    UpdateStatusDisplay("Select a method first.");
                    return;
                }
                mobj_pump.StartMethod(0, Convert.ToDouble(mnum_channels.Value), methodData.ToString());
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }

        }
        private void mbutton_stop_Click(object sender, EventArgs e)
        {
            UpdateStatusDisplay("");
            if (mnum_channels.Value < 1)
            {
                UpdateStatusDisplay("Set the channel first other than zero.");
                return;
            }

            try
            {
                mobj_pump.StopMethod(0, Convert.ToInt32(mnum_channels.Value));
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, ex.Message);
            }
        }
        #endregion
    }
}
