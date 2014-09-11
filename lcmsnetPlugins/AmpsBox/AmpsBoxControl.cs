using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using AmpsBoxSdk.Devices;

namespace AmpsBox
{
    /// <summary>
    /// GUI control for the PNNL Amps Boxes made by Gordon Anderson and Randy Norheim.
    /// </summary>
    public partial class AmpsBoxControl : controlBaseDeviceControl, IDeviceControl
    {
        /// <summary>
        /// Total number of channels available.
        /// </summary>
        private const int CONST_NUMBER_CHANNELS = 8;
        /// <summary>
        /// Device control
        /// </summary>
        private AmpsBoxDevicePlugin m_device;
        

        public AmpsBoxControl()
        {
            InitializeComponent();
            
            // make sure the interface knows what channel it is on.
            

            Expandable = false;
        }

        #region RF Commands        
        void m_rfControl_GetRfFrequency(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                int voltage     = m_device.GetRfFrequency(e.Data.Channel);
                e.Data.Actual   = voltage;
            }
            catch (InvalidOperationException)
            {
                UpdateStatusDisplay("The port is not open.");
            }
            catch (FormatException)
            {
                UpdateStatusDisplay("The device did not return a valid value.");
            }
            catch (AmpsCommandNotRecognized)
            {
                UpdateStatusDisplay("The AMPS Box did not recognize the command.");
            }
        }
        void m_rfControl_GetOutputVoltage(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                int voltage = m_device.GetOutputVoltage(e.Data.Channel);
                e.Data.Actual = voltage;
            }
            catch (InvalidOperationException)
            {
                UpdateStatusDisplay("The port is not open.");
            }
            catch (FormatException)
            {
                UpdateStatusDisplay("The device did not return a valid value.");
            }
            catch (AmpsCommandNotRecognized)
            {
                UpdateStatusDisplay("The AMPS Box did not recognize the command.");
            }
        }
        void m_rfControl_GetDriveLevel(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                int voltage   = m_device.GetDriveLevel(e.Data.Channel);
                e.Data.Actual = voltage;
            }
            catch (InvalidOperationException)
            {
                UpdateStatusDisplay("The port is not open.");
            }
            catch (FormatException)
            {
                UpdateStatusDisplay("The device did not return a valid value.");
            }
            catch (AmpsCommandNotRecognized)
            {
                UpdateStatusDisplay("The AMPS Box did not recognize the command.");
            }
        }
        #endregion

        #region HV Control
        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_hvControl_GetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                int voltage   = m_device.GetHvOutput(e.Data.Channel);
                e.Data.Actual = voltage;
            }
            catch (InvalidOperationException)
            {
                UpdateStatusDisplay("The port is not open.");
            }
            catch (FormatException)
            {
                UpdateStatusDisplay("The device did not return a valid value.");
            }
            catch (AmpsCommandNotRecognized ex)
            {
                UpdateStatusDisplay("The AMPS Box did not recognize the command.  " + ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_hvControl_SetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                m_device.SetHvOutput(e.Data.Channel, e.Data.Setpoint);
            }
            catch (InvalidOperationException)
            {
                UpdateStatusDisplay("The port is not open.");
            }
        }
        #endregion

        #region RF Control
        void m_rfControl_SetRfFrequency(object sender, AmpsBoxCommandEventArgs e)
        {
            UpdateStatusDisplay("");
            int channel    = Convert.ToInt32(mnum_channelNumberRF.Value);
            e.Data.Channel = channel;

            m_device.SetRfFrequency(channel, e.Data.Setpoint);
        }
        void m_rfControl_SetOutputVoltage(object sender, AmpsBoxCommandEventArgs e)
        {
            UpdateStatusDisplay("");
            int channel = Convert.ToInt32(mnum_channelNumberRF.Value);
            m_device.SetRfOutputVoltage(channel, e.Data.Setpoint);
        }
        void m_rfControl_SetDriveLevel(object sender, AmpsBoxCommandEventArgs e)
        {
            UpdateStatusDisplay("");
            int channel = Convert.ToInt32(mnum_channelNumberRF.Value);
            m_device.SetRfDriveLevel(channel, e.Data.Setpoint);
        }
        #endregion

        /// <summary>
        /// Registers the Amps Box with the control and updates the UI.
        /// </summary>
        /// <param name="device"></param>
        public void RegisterDevice(IDevice device)
        {
            m_device = device as AmpsBoxDevicePlugin;

            if (m_device == null)
                return;

            // Add to the device manager.
            SetBaseDevice(m_device);

            m_device.CapabilitiesLearned += new EventHandler<AmpsInitializationArgs>(m_device_CapabilitiesLearned);
            m_device.Error += new EventHandler<classDeviceErrorEventArgs>(m_device_Error);
   
            // Add a list of available serial port names to the combo box.            
            mpropertyGrid_serialPort.SelectedObject = m_device.Port;                    
        }

        void m_device_Error(object sender, classDeviceErrorEventArgs e)
        {
            
            if (m_device.Status == enumDeviceStatus.NotInitialized)
            {
                MessageBox.Show(e.Error + "  Is the device turned on or is the COM port setting correct?");
            }

            UpdateStatusDisplay(e.Error);
        }

        List<RFControl> m_rfControls            = new List<RFControl>();
        List<SingleElementControl> m_hvControls = new List<SingleElementControl>();

        /// <summary>
        /// Handles when the device is initialized so we can update what the device has available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_device_CapabilitiesLearned(object sender, AmpsInitializationArgs e)
        {
            mnum_channelHV.Maximum          = Convert.ToDecimal(e.HvCount);
            mnum_channelNumberRF.Maximum    = Convert.ToDecimal(e.RfCount);

            if (Expandable)
            {
                for (int i = 0; i < e.RfCount; i++)
                {
                    RFControl control = new RFControl();

                    //m_rfPanel.ad
                }
            }
            else
            {                
                m_hvControl.SetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_hvControl_SetDataCommand);
                m_hvControl.GetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_hvControl_GetDataCommand);

                m_rfControl.SetDriveLevel       += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetDriveLevel);
                m_rfControl.SetOutputVoltage    += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetOutputVoltage);
                m_rfControl.SetRfFrequency      += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetRfFrequency);

                m_rfControl.GetDriveLevel       += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetDriveLevel);
                m_rfControl.GetOutputVoltage    += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetOutputVoltage);
                m_rfControl.GetRfFrequency      += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetRfFrequency);

                m_rfControl.SetData(m_device.DeviceData.GetRfData(1));
                m_hvControl.SetData(m_device.DeviceData.GetHvData(1));

                m_rfControl.Enabled = true;
                m_hvControl.Enabled = true;
            }            
        }

        public bool Expandable { get; set; }

        #region IDeviceControl Members
        /// <summary>
        /// Interface to the underlying device.
        /// </summary>
        public IDevice Device
        {
            get
            {
                return m_device;
            }
            set
            {
                RegisterDevice(value);
            }
        }
        /// <summary>
        /// Determines if the device is in emulation mode.
        /// </summary>
        public bool Emulation
        {
            get
            {
                if (m_device == null)
                    return true;

                return m_device.Emulation;
            }
            set
            {
                if (m_device == null)
                    return;

                m_device.Emulation = value;
            }
        }
        #endregion        
        
        private void mbutton_closePort_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatusDisplay("Closing Port.");
                m_device.Close();
                UpdateStatusDisplay("Port Closed.");
            }
            catch (Exception ex)
            {
                UpdateStatusDisplay("Could not close the port: " + ex.Message);
            }
        }
        private void mbutton_getVersion_Click(object sender, EventArgs e)
        {
            try
            {

                UpdateStatusDisplay("Retrieving Version.");
                string version = m_device.GetVersion();
                mlabel_version.Text = version;
                UpdateStatusDisplay("Version Retrieved.");
            }
            catch (Exception ex)
            {
                UpdateStatusDisplay("Could not retrieve the version: " + ex.Message);
            }
        }
        private void mbutton_saveParameters_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatusDisplay("Saving parameters.");
                m_device.SaveParameters();
                UpdateStatusDisplay("Parameters Saved.");
            }
            catch(Exception ex)
            {
                UpdateStatusDisplay("Could not save the parameters: " + ex.Message);
            }
        }
        private void mbutton_openPort_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatusDisplay("Opening Port.");
                m_device.Open();
                UpdateStatusDisplay("Port Opened.");
            }catch(Exception ex)
            {
                UpdateStatusDisplay("Could not open the port: " + ex.Message);
            }
        }

        private void mnum_channelHV_ValueChanged(object sender, EventArgs e)
        {
            if (m_device != null)
            {
                int channel = Convert.ToInt32(mnum_channelHV.Value);
                try
                {
                    AmpsBoxChannelData data = m_device.DeviceData.GetHvData(channel);
                    m_hvControl.SetData(data);
                }
                catch
                {
                    // add that the channel is not supported if the key is not found?
                }
                
            }
        }

        private void mbuton_emulate_Click(object sender, EventArgs e)
        {
            m_device.Emulation  = m_device.Emulation == false;
            bool isEmulated     = m_device.Emulation;
            mbuton_emulate.Text = isEmulated ? "Don't Emulate" : "Emulate Me";
        }
    }
}
