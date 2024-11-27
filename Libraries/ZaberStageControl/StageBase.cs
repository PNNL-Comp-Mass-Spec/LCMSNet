using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ZaberStageControl
{
    public abstract class StageBase : INotifyPropertyChangedExtZaber, IDisposable
    {
        /// <summary>
        /// The valve's name
        /// </summary>
        private string name;

        private string portName = "COM1";

        protected Zaber.Motion.Ascii.Connection Connection;
        private bool connected;

        public IReadOnlyList<StageControl> StagesUsed { get; }

        protected const int MinTimeBetweenCommandsMs = 100; // milliseconds

        public event EventHandler StageConfigUpdated;

        /// <summary>
        /// For reporting status changes within a method operation.
        /// </summary>
        public event EventHandler<ZaberStatusReportEventArgs> ZaberStatusReport;

        /// <summary>
        /// Used to report status and informational messages
        /// </summary>
        public event EventHandler<ZaberMessageEventArgs> ZaberMessage;

        /// <summary>
        /// Used for reporting configuration and other errors
        /// </summary>
        public event EventHandler<ZaberErrorEventArgs> ZaberError;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stageNames">Internal reference names for stages used</param>
        /// <param name="defaultName">default device name</param>
        protected StageBase(IReadOnlyList<string> stageNames, string defaultName)
        {
            StagesUsed = stageNames.Select(x => new StageControl(x)).ToArray();

            name = defaultName;

            foreach (var stage in StagesUsed)
            {
                stage.PropertyChanged += Stage_PropertyChanged;
            }
        }

        private void Stage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(StageControl.StageDisplayName)) ||
                e.PropertyName.Equals(nameof(StageControl.SerialNumber)) ||
                e.PropertyName.Equals(nameof(StageControl.IsInverted)))
            {
                StageConfigUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        //public bool Emulation { get; set; }
        public bool Emulation { get => false;
            set { }
        }

        /// <summary>
        /// Gets or sets the device's name
        /// </summary>
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName
        {
            get => portName;
            set => this.RaiseAndSetIfChanged(ref portName, value);
        }

        /// <summary>
        /// Disconnects from the valve.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Shutdown()
        {
            if (Emulation)
            {
                return Emulation;
            }

            try
            {
                StageConnectionManager.Instance.CloseConnection(this);
                connected = false;

                StageConnectionManager.Instance.RemoveUsedStages(StagesUsed);
            }
            catch (Exception ex)
            {
                LogError("Error closing Zaber stage connection", ex);
            }

            return true;
        }

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Initialize(out string errorMessage)
        {
            errorMessage = "";

            if (Emulation)
            {
                //Fill in fake ID, version
                return true;
            }

            //If the serial port is not open, open it
            if (!connected)
            {
                try
                {
                    Connection = StageConnectionManager.Instance.OpenConnection(this);
                    connected = true;
                }
                catch (UnauthorizedAccessException ex)
                {
                    errorMessage = "Could not access the COM Port.  " + ex.Message;
                    return false;
                }
            }

            var devices = Connection.DetectDevices();
            StageConnectionManager.Instance.ReadStagesForConnection(PortName, devices);
            var serialsFound = new List<long>(StagesUsed.Count);

            // reset attached stages
            StageConnectionManager.Instance.RemoveUsedStages(StagesUsed);

            foreach (var stage in StagesUsed)
            {
                if (stage.SerialNumber <= 0)
                {
                    continue;
                }

                foreach (var device in devices)
                {
                    for (var i = 1; i <= device.AxisCount; i++)
                    {
                        var axis = device.GetAxis(i);
                        var serialNumber = device.IsIntegrated
                            ? device.SerialNumber
                            : axis.Identity.PeripheralSerialNumber;
                        if (stage.SerialNumber == serialNumber)
                        {
                            if (!StageConnectionManager.Instance.IsStageUsed(serialNumber))
                            {
                                serialsFound.Add(serialNumber);
                                stage.DeviceRef = device;
                                stage.AxisNumber = i;
                                stage.StageConnectionError = false;
                            }
                            else
                            {
                                stage.StageConnectionError = true;
                            }
                        }
                    }
                }
            }

            StageConnectionManager.Instance.AddUsedStages(serialsFound);

            if (!HomeStages(false, out errorMessage))
            {
                return false;
            }

            System.Threading.Thread.Sleep(MinTimeBetweenCommandsMs);

            try
            {
                //var position = GetPosition();
                //if (position == -1)
                //{
                //    errorMessage = "The valve position is unknown.  Make sure it is plugged in.";
                //    SendError(new DeviceErrorEventArgs(errorMessage, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Valve Position"));
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                errorMessage = "Could not get the valve position. " + ex.Message;
                return false;
            }
            return true;
        }

        public bool HomeStages(bool force, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                var homeGroups = StagesUsed.Where(x => x.DeviceRef != null).GroupBy(x => x.InitOrder).OrderBy(x => x.Key);
                foreach (var group in homeGroups)
                {
                    var items = group.ToArray();
                    var tasks = new List<Task>();
                    foreach (var stage in items)
                    {
                        var device = stage.DeviceRef;
                        var axis = device.GetAxis(stage.AxisNumber);
                        if (!axis.IsHomed() || force)
                        {
                            tasks.Add(axis.HomeAsync());
                        }
                    }

                    if (tasks.Count > 0)
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                }
                //foreach (var stage in StagesUsed.Where(x => x.DeviceRef != null).OrderBy(x => x.InitOrder))
                //{
                //    var device = stage.DeviceRef;
                //    for (var i = 1; i <= device.AxisCount; i++)
                //    {
                //        var axis = device.GetAxis(i);
                //        if (!axis.IsHomed())
                //        {
                //            axis.Home();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                errorMessage = "Could not home the axes in use. " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// For reporting status changes within a method operation.
        /// </summary>
        protected void SendStatusMessage(StatusReportType statusType, string message)
        {
            ZaberStatusReport?.Invoke(this, new ZaberStatusReportEventArgs(statusType, message));
        }

        protected void LogMessage(string message)
        {
            ZaberMessage?.Invoke(this, new ZaberMessageEventArgs(message));
        }

        protected void LogError(string message, Exception ex = null)
        {
            ZaberError?.Invoke(this, new ZaberErrorEventArgs(message, ex));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
