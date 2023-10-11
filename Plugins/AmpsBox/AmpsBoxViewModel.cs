using AmpsBoxSdk.Devices;
using LcmsNetSDK.Devices;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using ReactiveUI;

namespace AmpsBox
{
    /// <summary>
    /// GUI control for the PNNL Amps Boxes made by Gordon Anderson and Randy Norheim.
    /// </summary>
    public class AmpsBoxViewModel : BaseDeviceControlViewModelReactive
    {
        /// <summary>
        /// Total number of channels available.
        /// </summary>
        private const int CONST_NUMBER_CHANNELS = 8;
        /// <summary>
        /// Device control
        /// </summary>
        private AmpsBoxDevicePlugin m_device;


        public AmpsBoxViewModel()
        {
            // make sure the interface knows what channel it is on.
            Expandable = false;

            //this.WhenAnyValue(x => x.ChannelNumberRF).Subscribe(x => ChannelNumberRFChanged());
            this.WhenAnyValue(x => x.ChannelHV).Subscribe(x => ChannelHVChanged());

            ClosePortCommand = ReactiveCommand.Create(ClosePort);
            OpenPortCommand = ReactiveCommand.Create(OpenPort);
            SaveParametersCommand = ReactiveCommand.Create(SaveParameters);
            GetVersionCommand = ReactiveCommand.Create(GetVersion);
            ToggleEmulationCommand = ReactiveCommand.Create(ToggleEmulation);
        }

        private int channelNumberRF;
        private int channelNumberRFMax = 16;
        private RF rf;
        private int channelHV;
        private int channelHVMax = 16;
        private SingleElement hv;
        private SerialPort port;
        private string version;
        private string emulateButtonText;
        private bool channelsEnabled = false;

        public int ChannelNumberRF { get => channelNumberRF; set => this.RaiseAndSetIfChanged(ref channelNumberRF, value); }
        public int ChannelNumberRFMax { get => channelNumberRFMax; set => this.RaiseAndSetIfChanged(ref channelNumberRFMax, value); }
        public RF RF { get => rf; set => this.RaiseAndSetIfChanged(ref rf, value); }
        public int ChannelHV { get => channelHV; set => this.RaiseAndSetIfChanged(ref channelHV, value); }
        public int ChannelHVMax { get => channelHVMax; set => this.RaiseAndSetIfChanged(ref channelHVMax, value); }
        public SingleElement HV { get => hv; set => this.RaiseAndSetIfChanged(ref hv, value); }
        public SerialPort Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public string Version { get => version; set => this.RaiseAndSetIfChanged(ref version, value); }
        public string EmulateButtonText { get => emulateButtonText; set => this.RaiseAndSetIfChanged(ref emulateButtonText, value); }
        public bool ChannelsEnabled { get => channelsEnabled; set => this.RaiseAndSetIfChanged(ref channelsEnabled, value); }

        public ReactiveCommand<Unit, Unit> ClosePortCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenPortCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveParametersCommand { get; }
        public ReactiveCommand<Unit, Unit> GetVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleEmulationCommand { get; }

        #region RF Commands
        void m_rfControl_GetRfFrequency(object sender, AmpsBoxCommandEventArgs e)
        {
            try
            {
                UpdateStatusDisplay("");
                int voltage = m_device.GetRfFrequency(e.Data.Channel);
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
                int voltage = m_device.GetDriveLevel(e.Data.Channel);
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
                int voltage = m_device.GetHvOutput(e.Data.Channel);
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
            int channel = ChannelNumberRF;
            e.Data.Channel = channel;

            m_device.SetRfFrequency(channel, e.Data.Setpoint);
        }
        void m_rfControl_SetOutputVoltage(object sender, AmpsBoxCommandEventArgs e)
        {
            UpdateStatusDisplay("");
            int channel = ChannelNumberRF;
            m_device.SetRfOutputVoltage(channel, e.Data.Setpoint);
        }
        void m_rfControl_SetDriveLevel(object sender, AmpsBoxCommandEventArgs e)
        {
            UpdateStatusDisplay("");
            int channel = ChannelNumberRF;
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
            m_device.Error += new EventHandler<DeviceErrorEventArgs>(m_device_Error);

            // Add a list of available serial port names to the combo box.
            Port = m_device.Port;
        }

        void m_device_Error(object sender, DeviceErrorEventArgs e)
        {

            if (m_device.Status == LcmsNetSDK.Devices.DeviceStatus.NotInitialized)
            {
                MessageBox.Show(e.Error + "  Is the device turned on or is the COM port setting correct?");
            }

            UpdateStatusDisplay(e.Error);
        }

        List<RF> m_rfControls = new List<RF>();
        List<SingleElement> m_hvControls = new List<SingleElement>();

        /// <summary>
        /// Handles when the device is initialized so we can update what the device has available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_device_CapabilitiesLearned(object sender, AmpsInitializationArgs e)
        {
            ChannelHVMax = e.HvCount;
            ChannelNumberRFMax = e.RfCount;

            if (Expandable)
            {
                for (int i = 0; i < e.RfCount; i++)
                {
                    var control = new RF();

                    //m_rfPanel.ad
                }
            }
            else
            {
                HV.SetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_hvControl_SetDataCommand);
                HV.GetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_hvControl_GetDataCommand);

                RF.SetDriveLevel += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetDriveLevel);
                RF.SetOutputVoltage += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetOutputVoltage);
                RF.SetRfFrequency += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetRfFrequency);

                RF.GetDriveLevel += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetDriveLevel);
                RF.GetOutputVoltage += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetOutputVoltage);
                RF.GetRfFrequency += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetRfFrequency);

                RF.SetData(m_device.DeviceData.GetRfData(1));
                HV.SetData(m_device.DeviceData.GetHvData(1));

                ChannelsEnabled = true;
            }
        }

        public bool Expandable { get; set; }

        #region IDeviceControl Members

        public override UserControl GetDefaultView()
        {
            return new AmpsBoxView();
        }

        /// <summary>
        /// Interface to the underlying device.
        /// </summary>
        public override IDevice Device
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

        private void ClosePort()
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
        private void GetVersion()
        {
            try
            {

                UpdateStatusDisplay("Retrieving Version.");
                Version = m_device.GetVersion();
                UpdateStatusDisplay("Version Retrieved.");
            }
            catch (Exception ex)
            {
                UpdateStatusDisplay("Could not retrieve the version: " + ex.Message);
            }
        }
        private void SaveParameters()
        {
            try
            {
                UpdateStatusDisplay("Saving parameters.");
                m_device.SaveParameters();
                UpdateStatusDisplay("Parameters Saved.");
            }
            catch (Exception ex)
            {
                UpdateStatusDisplay("Could not save the parameters: " + ex.Message);
            }
        }
        private void OpenPort()
        {
            try
            {
                UpdateStatusDisplay("Opening Port.");
                m_device.Open();
                UpdateStatusDisplay("Port Opened.");
            }
            catch (Exception ex)
            {
                UpdateStatusDisplay("Could not open the port: " + ex.Message);
            }
        }

        private void ChannelHVChanged()
        {
            if (m_device != null)
            {
                int channel = ChannelHV;
                try
                {
                    AmpsBoxChannelData data = m_device.DeviceData.GetHvData(channel);
                    HV.SetData(data);
                }
                catch
                {
                    // add that the channel is not supported if the key is not found?
                }

            }
        }

        private void ToggleEmulation()
        {
            m_device.Emulation = !m_device.Emulation;
            bool isEmulated = m_device.Emulation;
            EmulateButtonText = isEmulated ? "Don't Emulate" : "Emulate Me";
        }
    }
}
