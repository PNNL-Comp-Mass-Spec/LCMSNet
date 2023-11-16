using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;

namespace LcmsNetPlugins.ZaberStage
{
    public abstract class StageBase : IDevice, IDisposable
    {
        /// <summary>
        /// The valve's name
        /// </summary>
        private string deviceName;

        private string portName = "COM1";

        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private DeviceStatus status;

        protected Zaber.Motion.Ascii.Connection connection;
        private bool connected;
        protected internal readonly IReadOnlyList<StageControl> StagesUsed;

        protected const int MinTimeBetweenCommandsMs = 100; // milliseconds

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
        /// <param name="stageNames">Internal reference names for stages used</param>
        /// <param name="defaultName">default device name</param>
        protected StageBase(IReadOnlyList<string> stageNames, string defaultName)
        {
            StagesUsed = stageNames.Select(x => new StageControl(x)).ToArray();

            deviceName = defaultName;
            StatusPollDelay = 1;

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
                OnDeviceSaveRequired();
            }
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        //public bool Emulation { get; set; }
        public bool Emulation // TODO
        {
            get => false;
            set { }
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
            get => deviceName;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref deviceName, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Serial port name
        /// </summary>
        [DeviceSavedSetting("PortName")]
        public string PortName
        {
            get => portName;
            set
            {
                portName = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// The delay when polling for system status in seconds
        /// </summary>
        [DeviceSavedSetting("StatusPollDelay")]
        public int StatusPollDelay { get; set; }

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
            catch (UnauthorizedAccessException)
            {
                if (Error != null)
                {
                }

                throw new Exception();
            }
            catch (Exception e)
            {
            }
            return true;
        }

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                //Fill in fake ID, version
                return true;
            }

            //If the serial port is not open, open it
            if (!connected  || !connection.IsConnected)
            {
                try
                {
                    connection = StageConnectionManager.Instance.OpenConnection(this);
                    connected = true;
                }
                catch (UnauthorizedAccessException ex)
                {
                    errorMessage = "Could not access the COM Port.  " + ex.Message;
                    return false;
                }
            }

            var devices = connection.DetectDevices();
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
                    if (stage.SerialNumber == device.SerialNumber)
                    {
                        if (!StageConnectionManager.Instance.IsStageUsed(device.SerialNumber))
                        {
                            serialsFound.Add(device.SerialNumber);
                            stage.DeviceRef = device;
                            stage.StageConnectionError = false;
                        }
                        else
                        {
                            stage.StageConnectionError = true;
                        }
                    }
                }
            }

            StageConnectionManager.Instance.AddUsedStages(serialsFound);

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
                        for (var i = 1; i <= device.AxisCount; i++)
                        {
                            var axis = device.GetAxis(i);
                            if (!axis.IsHomed())
                            {
                                tasks.Add(axis.HomeAsync());
                            }
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

        protected string GetHardwarePosition()
        {
            if (Emulation)
            {
                return "";
            }

            // TODO: Read issues exist with old 2-position valves!
            var error = ReadCommand("CP", out var hwPosition, 200);
            //if (error != ValveErrors.Success)
            //{
            //    switch (error)
            //    {
            //        case ValveErrors.UnauthorizedAccess:
            //            throw new ValveExceptionUnauthorizedAccess();
            //        case ValveErrors.TimeoutDuringWrite:
            //            throw new ValveExceptionWriteTimeout();
            //        case ValveErrors.TimeoutDuringRead:
            //            throw new ValveExceptionReadTimeout();
            //    }
            //}

            //This should look like
            //  Position is "B" (2-position)
            //  Position is = 1 (MultiPosition)
            // Universal actuator:
            //  \0CP01 (no hardware ID) or 5CP01 (hardware ID 5) (multiposition)
            //  2-position unknown/not tested
            var result = "";
            var cpPos = hwPosition.IndexOf("CP", StringComparison.OrdinalIgnoreCase);
            if (cpPos >= 0 && cpPos < 2)
            {
                // Universal actuator
                result = hwPosition.Substring(cpPos + 2).Trim('\r', '\n');
            }
            else
            {
                var data = hwPosition.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = data.Length - 1; i >= 0; i--)
                {
                    var x = data[i];
                    x = x.Replace(" ", "").ToLower();
                    var loc = x.IndexOf("positionis", StringComparison.OrdinalIgnoreCase);
                    if (loc >= 0)
                    {
                        // Grab the actual position from the string
                        result = x.Substring(loc + "positionis".Length).Trim('=', '"', '\'');
                        break;
                    }
                }
            }

            return result;
        }

        //protected ValveErrors SetHardwarePosition(string position, int rotationDelayTimeMs = 200)
        //{
        //    if (Emulation)
        //    {
        //        return ValveErrors.Success;
        //    }
        //
        //    // 'GO' by itself, on universal actuator, will change position (2-position) or advance to the next position (multiposition)
        //    var sendError = SendCommand("GO" + position);
        //    if (sendError == ValveErrors.UnauthorizedAccess || sendError == ValveErrors.TimeoutDuringWrite)
        //    {
        //        return sendError;
        //    }
        //
        //    //Wait rotationDelayTimeMs for valve to actually switch before proceeding
        //    //System.Threading.Thread.Sleep(rotationDelayTimeMs);
        //    //TODO: BLL test this instead using the abort event.
        //    if (AbortEvent == null)
        //        AbortEvent = new System.Threading.ManualResetEvent(false);
        //
        //    var waited = System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { AbortEvent }, rotationDelayTimeMs);
        //    if (waited)
        //        return ValveErrors.BadArgument;
        //
        //    GetPosition();
        //
        //    return ValveErrors.Success;
        //}

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <param name="noPrefix">If true, the SoftwareID is not pre-pended to the command</param>
        /// <returns></returns>
        //protected ValveErrors SendCommand(string command, bool noPrefix = false)
        //{
        //    if (Emulation)
        //    {
        //        return ValveErrors.Success;
        //    }
        //
        //    var id = noPrefix ? "" : SoftwareID.ToString();
        //
        //    //If the serial port is not open, open it
        //    if (!Port.IsOpen)
        //    {
        //        try
        //        {
        //            Port.Open();
        //        }
        //        catch (UnauthorizedAccessException)
        //        {
        //            return ValveErrors.UnauthorizedAccess;
        //        }
        //    }
        //
        //    try
        //    {
        //        Port.WriteLine(id + command);
        //    }
        //    catch (TimeoutException)
        //    {
        //        //ApplicationLogger.LogError(0, "Could not send command.  Write timeout.");
        //        return ValveErrors.TimeoutDuringWrite;
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        //ApplicationLogger.LogError(0, "Could not send command.  Could not access serial port.");
        //        return ValveErrors.UnauthorizedAccess;
        //    }
        //
        //    return ValveErrors.Success;
        //}

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        protected string ReadCommand(string command, out string returnData, int readDelayMs = 0)
        {
            returnData = "";
            if (Emulation)
            {
                return "ValveErrors.Success";
            }

            //If the serial port is not open, open it
            //if (!Port.IsOpen)
            //{
            //    try
            //    {
            //        Port.Open();
            //    }
            //    catch (UnauthorizedAccessException)
            //    {
            //        return "ValveErrors.UnauthorizedAccess";
            //    }
            //}
            //
            //try
            //{
            //    Port.DiscardInBuffer();
            //    Port.WriteLine(SoftwareID + command);
            //    if (readDelayMs > 0)
            //    {
            //        System.Threading.Thread.Sleep(readDelayMs);
            //    }
            //}
            //catch (TimeoutException)
            //{
            //    return "ValveErrors.TimeoutDuringWrite";
            //}
            //catch (UnauthorizedAccessException)
            //{
            //    return "ValveErrors.UnauthorizedAccess";
            //}
            //
            //try
            //{
            //    //Read in whatever is waiting in the buffer
            //    returnData = Port.ReadExisting();
            //    if (!string.IsNullOrEmpty(returnData))
            //    {
            //        // Valve may return string containing \r\n or \n\r; make all instances be \n
            //        returnData = returnData.Replace("\r", "\n").Replace("\n\n", "\n").Trim('\n');
            //    }
            //}
            //catch (TimeoutException)
            //{
            //    return "ValveErrors.TimeoutDuringRead";
            //}
            //catch (UnauthorizedAccessException)
            //{
            //    return "ValveErrors.UnauthorizedAccess";
            //}

            if (returnData.IndexOf("Bad command", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return "ValveErrors.BadCommand";
            }

            return "ValveErrors.Success";
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return deviceName;
        }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// For reporting status changes within a method operation. Primary usage is by MultiPosition valves, with advanced methods.
        /// </summary>
        protected void SendStatusMessage(StatusReportType statusType, string message)
        {
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, statusType.ToString(), this, message));
        }

        protected enum StatusReportType
        {
            PositionChanged,
            CycleCount
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
