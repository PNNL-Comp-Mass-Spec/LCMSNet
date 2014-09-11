using System;
using System.Collections.Generic;
using System.IO.Ports;
using AmpsBoxSdk.Devices;
using Finch.Data;
using LcmsNetDataClasses.Devices;

namespace AmpsBox
{

    [classDeviceControlAttribute(typeof(AmpsBoxControl),
                                 typeof(AmpsBoxGlyph),
                                 "Amps Box",
                                 "Voltage Controllers")]
    public class AmpsBoxDevicePlugin: IDevice
    {
        public event EventHandler<AmpsInitializationArgs> CapabilitiesLearned;

        /// <summary>
        /// Software interface to the hardware.
        /// </summary>
        private AmpsBoxSdk.Devices.AmpsBox  m_device;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AmpsBoxDevicePlugin()
        {
            Name        = "Amps Box";
            Version     = "version 1";
            Status      = enumDeviceStatus.NotInitialized;
            ErrorType   = enumDeviceErrorStatus.NoError;
            Emulation   = false;

            m_device = new AmpsBoxSdk.Devices.AmpsBox();
            DeviceData  = new AmpsBoxDeviceData();
        }
        /// <summary>
        /// Gets or sets the device data.
        /// </summary>
        public AmpsBoxDeviceData DeviceData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        [classPersistenceAttribute("BaudRate")]
        public int BaudRate
        {
            get
            {
                return m_device.Port.BaudRate;
            }
            set
            {
                m_device.Port.BaudRate = value;
            }
        }
        /// <summary>
        /// Gets or sets the COM port ID.
        /// </summary>
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get 
            {
                return m_device.Port.PortName;
            }
            set
            {
                m_device.Port.PortName = value;
            }        
        }

        /// <summary>
        /// Gets the serial port object reference.
        /// </summary>
        public SerialPort Port
        {
            get
            {
                return m_device.Port;
            }
        }

        #region IDevice Members

        public string Name
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public enumDeviceStatus Status
        {
            get;
            set;
        }

        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }

        public bool Initialize(ref string errorMessage)
        {
            Open();
            int hvCount = 0;
            int rfCount = 0;
            try
            {
                hvCount = m_device.GetHvChannelCount();
                rfCount = m_device.GetRfChannelCount();

                DeviceData.NumberHvChannels = hvCount;
                DeviceData.NumberRfChannels = rfCount;
            }
            catch (Exception ex)
            {
                if (Error != null)
                {
                    this.Error(this, new classDeviceErrorEventArgs("The device could not be initialized.", 
                                                                    ex,
                                                                    enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                                                    this));
                }
                this.Status = enumDeviceStatus.NotInitialized;
                return false;
            }

            if (CapabilitiesLearned != null)
            {
                AmpsInitializationArgs args = new AmpsInitializationArgs();
                args.HvCount = hvCount;
                args.RfCount = rfCount;

                try
                {
                    CapabilitiesLearned(this, args);
                }
                catch
                {
                    // Ignore any exceptions thrown by somethign not required to 
                    // initialize the device for now.
                }
            }

            return true;
        }

        public bool Shutdown()
        {
            if (m_device.Port.IsOpen)
                m_device.Close();
            return true;
        }

        public void RegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            
        }

        public void UnRegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            
        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
           
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }

        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Component; }
        }

        public bool Emulation
        {
            get
            {
                if (m_device != null)
                    return m_device.Emulated;
                return false;
            }
            set
            {
                if (m_device != null)
                    m_device.Emulated = value;
            }
        }
        #endregion

        public void Start()
        {
            
        }

        #region IFinchComponent Members
        /// <summary>
        /// Gets data for Finch
        /// </summary>
        /// <returns></returns>
        public Finch.Data.FinchComponentData GetData()
        {
            FinchComponentData component    = new FinchComponentData();
            component.Status                = Status.ToString();
            component.Name                  = Name;
            component.Type                  = "AMPS";
            component.LastUpdate            = DateTime.Now;
             
            FinchScalarSignal port          = new FinchScalarSignal();            
            port.Name                       = "Port";
            port.Type                       = FinchDataType.String;
            port.Units                      = "";
            port.Value                      = this.PortName.ToString();

            FinchScalarSignal baud          = new FinchScalarSignal();            
            baud.Name                       = "BaudRate";
            baud.Type                       = FinchDataType.String;
            baud.Units                      = "";
            baud.Value                      = this.BaudRate.ToString();

            component.Signals.Add(port);
            component.Signals.Add(baud);
            return component;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return m_device.GetVersion();
        }
        public void Open()
        {
            m_device.Open();
        }
        public void Close()
        {
            m_device.Close();
        }
        public void SaveParameters()
        {
            m_device.SaveParameters();
        }
        public void SetRfFrequency(int channel, int frequency)
        {
            m_device.SetRfFrequency(channel, frequency);
        }
        public void SetHvOutput(int channel, int voltage)
        {
            m_device.SetHvOutput(channel, voltage);
        }
        public int GetHvOutput(int channel)
        {
            return m_device.GetHvOutput(channel);
        }
        public int GetOutputVoltage(int channel)
        {
            return m_device.GetOutputVoltage(channel);            
        }
        public int GetDriveLevel(int channel)
        {
            return m_device.GetDriveLevel(channel);                        
        }
        public int GetRfFrequency(int channel)
        {
            return m_device.GetRfFrequency(channel);            
        }
        public void SetRfDriveLevel(int channel, int voltage)
        {
            m_device.SetRfDriveLevel(channel, voltage);
        }
        public void SetRfOutputVoltage(int channel, int voltage)
        {
            m_device.SetRfOutputVoltage(channel, voltage);
        }
    }
}
