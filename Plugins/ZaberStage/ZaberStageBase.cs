using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    public abstract class ZaberStageBase<T> : IDevice, IZaberStageGroup, IDisposable where T : StageBase
    {
        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private DeviceStatus status;

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stage"></param>
        protected ZaberStageBase(T stage)
        {
            StageDevice = stage;
            StageDevice.PropertyChanged += OnPropertyChanged;
            StageDevice.StageConfigUpdated += (s, e) => { OnDeviceSaveRequired(); };
            StageDevice.ZaberStatusReport += OnStatusReport;
            StageDevice.ZaberMessage += OnZaberMessage;
            StageDevice.ZaberError += OnZaberError;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StageBase.Name):
                    OnDeviceSaveRequired();
                    this.RaisePropertyChanged(nameof(Name));
                    break;
                case nameof(StageBase.PortName):
                    OnDeviceSaveRequired();
                    this.RaisePropertyChanged(nameof(PortName));
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            StageDevice.Dispose();
        }

        public T StageDevice { get; }

        public StageBase StageBase => StageDevice;

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation // TODO
        {
            get => StageDevice.Emulation;
            set => StageDevice.Emulation = value;
        }

        /// <summary>
        /// Gets or sets the status of the device
        /// </summary>
        public DeviceStatus Status
        {
            get
            {
                if (Emulation)
                {
                    return DeviceStatus.Initialized;
                }

                return status;
            }
            set
            {
                if (value != status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status Changed", this));
                status = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's name
        /// </summary>
        public string Name
        {
            get => StageDevice.Name;
            set => StageDevice.Name = value;
        }

        /// <summary>
        /// Serial port name
        /// </summary>
        [DeviceSavedSetting("PortName")]
        public string PortName
        {
            get => StageDevice.PortName;
            set => StageDevice.PortName = value;
        }

        /// <summary>
        /// Indicates that a save in the Fluidics designer is required
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        protected virtual void SendError(DeviceErrorEventArgs args)
        {
            Error?.Invoke(this, args);
        }

        /// <summary>
        /// Disconnects from the valve.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Shutdown()
        {
            return StageDevice.Shutdown();
        }

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Initialize(ref string errorMessage)
        {
            return StageDevice.Initialize(out errorMessage);
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// For reporting status changes within a method operation. Primary usage is by MultiPosition valves, with advanced methods.
        /// </summary>
        protected void OnStatusReport(object sender, ZaberStatusReportEventArgs args)
        {
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, args.StatusType.ToString(), this, args.Message));
        }

        private void OnZaberMessage(object sender, ZaberMessageEventArgs args)
        {
            ApplicationLogger.LogMessage(LogLevel.Warning, args.Message);
        }

        private void OnZaberError(object sender, ZaberErrorEventArgs args)
        {
            ApplicationLogger.LogError(1, args.Message, args.Exception);
        }

        public List<string> GetStatusNotificationList()
        {
            var statusOptions = new List<string>() { "Status Changed" };
            statusOptions.AddRange(Enum.GetNames(typeof(StatusReportType)));
            return statusOptions;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Valve Position" };
        }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Component;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
