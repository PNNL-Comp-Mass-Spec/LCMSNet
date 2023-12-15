using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AxACQUISITIONLib;
using LcmsNetSDK;
using LcmsNetSDK.Logging;
using XCALIBURFILESLib;

namespace XcaliburControl
{
    public class XcaliburCOM : INotifyPropertyChangedExt, IDisposable
    {
        private readonly AxAcquisition acquisitionLib;
        private string templateSldPath = @"C:\Xcalibur\data\Template.sld";
        private string dataFileDirectoryPath = @"C:\Xcalibur\data";
        private string outputSldPath = @"C:\Xcalibur\data\LCMSNet.sld";

        private readonly Action<EventClass, EventName, string> eventReporter;

        public string TemplateSldPath { get => templateSldPath; set => this.RaiseAndSetIfChanged(ref templateSldPath, value); }
        public string DataFileDirectoryPath { get => dataFileDirectoryPath; set => this.RaiseAndSetIfChanged(ref dataFileDirectoryPath, value); }
        public string OutputSldPath { get => outputSldPath; set => this.RaiseAndSetIfChanged(ref outputSldPath, value); }

        public XcaliburCOM(Action<EventClass, EventName, string> eventReporterMethod)
        {
            acquisitionLib = new AxAcquisition();
            acquisitionLib.CreateControl();

            // events with arguments/data
            acquisitionLib.InformationalMessage += AcquisitionInformationalMessage;
            acquisitionLib.WarningMessage += AcquisitionWarningMessage;
            acquisitionLib.ErrorMessage += AcquisitionErrorMessage;
            acquisitionLib.ProgramError += AcquisitionProgramError;

            // events with no data
            acquisitionLib.Acquiring += AcquisitionAcquiring;
            acquisitionLib.DataFileCompleted += AcquisitionDataFileCompleted;
            acquisitionLib.DevicesAreReady += AcquisitionDevicesAreReady;
            acquisitionLib.DiskSpaceWarning += AcquisitionDiskSpaceWarning;
            acquisitionLib.DownloadAbandoned += AcquisitionDownloadAbandoned;
            acquisitionLib.DownloadCompleted += AcquisitionDownloadCompleted;
            acquisitionLib.DownLoadInitiated += AcquisitionDownLoadInitiated;
            acquisitionLib.MethodCheckFail += AcquisitionMethodCheckFail;
            acquisitionLib.MethodCheckOK += AcquisitionMethodCheckOK;
            acquisitionLib.NewData += AcquisitionNewData;
            acquisitionLib.Pause += AcquisitionPause;
            acquisitionLib.QueuedNewSequence += AcquisitionQueuedNewSequence;
            acquisitionLib.Resume += AcquisitionResume;
            acquisitionLib.RunEnded += AcquisitionRunEnded;
            acquisitionLib.SequenceChange += AcquisitionSequenceChange;
            acquisitionLib.SequenceClosed += AcquisitionSequenceClosed;
            acquisitionLib.SequenceOpened += AcquisitionSequenceOpened;
            acquisitionLib.StartCmdSent += AcquisitionStartCmdSent;
            acquisitionLib.StartedProcessing += AcquisitionStartedProcessing;
            acquisitionLib.StateChange += AcquisitionStateChange;
            acquisitionLib.VDStateChange += AcquisitionVDStateChange;
            acquisitionLib.WarningMessage += AcquisitionWarningMessage;

            eventReporter = eventReporterMethod;
        }

        public void Dispose()
        {
            acquisitionLib.Dispose();
        }

        /* Not implemented acquisitionLib calls:
        //ChangeDevicesOperatingMode: parameter is (int mode: '1' (On), '2' (Off), '3' (Standby))
        //ChangeOperatingModeByName: parameters are (int mode: '1' (On), '2' (Off), '3' (Standby), short deviceType)
        //ChangeOperatingModeByType: parameters are (int mode: '1' (On), '2' (Off), '3' (Standby), string deviceName)
        //CheckDiskSpace: parameters are: (string sequencePath, ref long space, int startRowInSequence), returns error code; all paths in sequence file must be valid to not get an error code
        //DeleteSequenceFromQueue: parameters are: (string sequencePath), returns error code
        //DeleteWholeQueue: no parameters, returns error code
        //GetDevicesByType: parameters are: (short typeCode, ref object[] (variant) deviceNames), returns short numDevices, might return '2005', which is a code for an OLE exception in the Acquisition Server
        //GetThisDeviceInfo: parameters are: (ref object[] (variant) deviceInfo, string deviceShortName), returns error code maybe?
        //ShowHomePage: parameters are (bool visibility), no return, not sure on actual functionality.
         */

        /// <summary>
        /// Start a sample in Xcalibur - creates a sequence and starts it
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="xcaliburMethodPath"></param>
        /// <param name="errorMessage"></param>
        /// <returns>false if an error occurred</returns>
        public bool StartSample(string sampleName, string xcaliburMethodPath, out string errorMessage)
        {
            return StartSample(sampleName, xcaliburMethodPath, out errorMessage, "", -1);
        }

        /// <summary>
        /// Start a sample in Xcalibur - creates a sequence and starts it
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="xcaliburMethodPath"></param>
        /// <param name="errorMessage"></param>
        /// <param name="position">Must be non-empty if using a Thermo autosampler</param>
        /// <param name="injectionVolume">Must be greater than zero if using a Thermo autosampler</param>
        /// <returns>false if an error occurred</returns>
        public bool StartSample(string sampleName, string xcaliburMethodPath, out string errorMessage, string position, double injectionVolume)
        {
            errorMessage = string.Empty;
            CreateSequenceForSample(sampleName, xcaliburMethodPath);

            var sequenceName = OutputSldPath;

            var deviceNames = "";
            acquisitionLib.GetDeviceNames(ref deviceNames); // Gets a comma-separated list of device names

            // The device list must not be empty!!!
            if (string.IsNullOrWhiteSpace(deviceNames.Trim()))
            {
                return false;
            }

            // 0-based, '-1' for 'no start' (Xcalibur handles this automatically for 'external autosampler' systems, but if this is '0', it causes an error/sequence pause.
            short startDevice = -1;
            // If position and injection volume are valid (ish), then assume we also need to start the device, rather than relying on a contact closure to start it.
            if (!string.IsNullOrWhiteSpace(position) && injectionVolume > 0)
            {
                // assume the first device is the start device.
                // I'm not sure on what situation there could be where we would have multiple devices configured and be using this software to control it...
                startDevice = 0;
            }

            //SubmitSequence - returns an error code: 0 for success, != 0 for error
            //SubmitSequence2 - returns a status code: >= 0 for success (submission ID), < 0 for failure
            // SubmitSequence2 appears to be friendlier
            var returnCode = acquisitionLib.SubmitSequence2(
                Environment.UserName, // May be limited to 10 characters?
                ref sequenceName,
                1, // First row number to run (0-based)
                1, // Last row number to run (0-based)
                false, // If true, starts this sequence immediately after current sequence; if false, adds to the end of the current queue
                true, // Automatically start the sample when the instrument is ready (this does not bypass contact closures)
                "", // Path of method to run before first sample
                "", // Path of method to run after last sample
                "", // pre-acquisition Program, an .exe or .bat to run
                "", // post-acquisition Program, an .exe or .bat to run
                true, // Specifically refers to running the pre-acquisition Program synchronous or asynchronous
                false, // Specifically refers to running the post-acquisition Program synchronous or asynchronous
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                deviceNames,
                startDevice, // 0-based, '-1' for 'no start' (Xcalibur handles this automatically for 'external autosampler' systems, but if this is '0', it causes an error/sequence pause.)
                1 // 1: ON, 2: OFF, 3: Standby
            );

            if (returnCode >= 0)
            {
                // Return code: >= 0, success (returns a submission ID)
                errorMessage = $"Return code: {returnCode}";
            }
            else
            {
                // Return code: >= 0, error
                string error = "";
                switch (returnCode)
                {
                    case -1:
                        error = "Sequence file error";
                        break;
                    case -2:
                        error = "No rows specified";
                        break;
                    case -3:
                        error = "Sequence file validation error";
                        break;
                    case -4:
                        error = "Error";
                        break;
                    default:
                        error = $"Illegal error: {returnCode}";
                        break;
                }

                errorMessage = error;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a sequence file (<see cref="OutputSldPath"/>) for the provided sample information
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="xcaliburMethodPath"></param>
        public void CreateSequenceForSample(string sampleName, string xcaliburMethodPath)
        {
            CreateSequenceForSample(sampleName, xcaliburMethodPath, "", -1);
        }

        /// <summary>
        /// Create a sequence file (<see cref="OutputSldPath"/>) for the provided sample information; position and injection volume are needed if running a Thermo autosampler
        /// </summary>
        /// <param name="sampleName"></param>
        /// <param name="xcaliburMethodPath"></param>
        /// <param name="position">Must be non-empty if using a Thermo autosampler</param>
        /// <param name="injectionVolume">Must be greater than zero if using a Thermo autosampler</param>
        public void CreateSequenceForSample(string sampleName, string xcaliburMethodPath, string position, double injectionVolume)
        {
            IXSequence sequence = new XSequenceClass();
            if (!string.IsNullOrWhiteSpace(TemplateSldPath) && File.Exists(TemplateSldPath))
            {
                sequence.Open(TemplateSldPath, 0);
            }
            else
            {
                sequence.New();
            }

            var samplesObj = sequence.Samples;
            var samples = (IXSamples)samplesObj;

            for (var i = samples.Count; i > 0; i--)
            {
                samples.Remove(i);
            }

            var sample = (IXSample)samples.Add(-1);
            sample.FileName = sampleName;
            sample.Path = DataFileDirectoryPath;
            sample.InstMeth = Path.ChangeExtension(xcaliburMethodPath, null); // do not include .meth

            if (!string.IsNullOrWhiteSpace(position) && injectionVolume > 0)
            {
                sample.Position = position;
                sample.InjVol = injectionVolume;
            }

            sequence.SaveAs(OutputSldPath, "LCMSOperator", "", 1);
            sequence.Close();
        }

        //GetDeviceNames
        public IReadOnlyList<string> GetDevices()
        {
            var deviceNames = "";
            var errorCode = acquisitionLib.GetDeviceNames(ref deviceNames); // Gets a comma-separated list of device names

            if (errorCode != 0 || string.IsNullOrWhiteSpace(deviceNames))
            {
                return Array.Empty<string>();
            }

            return deviceNames.Split(',');
        }

        //SendAnalysisCommand - parameter is '1' (start), '2' (stop), '3' (pause/resume)
        //SendAnalysisCommand2 - parameter is '1' (start), '2' (stop), '3' (pause), '4' (resume)
        public void StartQueueDefault()
        {
            acquisitionLib.SendAnalysisCommand2(1);
        }

        public void StopQueue()
        {
            acquisitionLib.SendAnalysisCommand2(2);
        }

        public void PauseQueue()
        {
            acquisitionLib.SendAnalysisCommand2(3);
        }

        public void ResumeQueue()
        {
            acquisitionLib.SendAnalysisCommand2(4);
        }

        //GetRunManagerStatus
        public string GetRunStatus()
        {
            return acquisitionLib.GetRunManagerStatus();
        }

        //GetSeqQueuePaused: no parameters, returns bool: true (paused), false (not paused)
        public bool IsQueuePaused()
        {
            return acquisitionLib.GetSeqQueuePaused();
        }

        public string GetDeviceStatus()
        {
            //return $"GetDeviceStatuses: {GetDeviceStatuses()}";
            return $"GetDeviceStatuses: {GetDeviceStatusesString()}\nGetDeviceStatuses2: {GetDeviceStatuses2String()}";
        }

        public string GetDeviceStatusesString()
        {
            var statuses = GetDeviceStatuses();
            if (statuses == null)
            {
                return "Read Error";
            }

            return string.Join("; ", statuses);
        }

        public string GetDeviceStatuses2String()
        {
            var statuses = GetDeviceStatuses2();
            if (statuses == null)
            {
                return "Read Error";
            }

            return string.Join("; ", statuses);
        }

        //GetDeviceStatuses
        public IReadOnlyList<XDeviceStatus> GetDeviceStatuses()
        {
            object wrapper = new System.Runtime.InteropServices.VariantWrapper(new object());
            // GetDeviceStatuses(ref object): object returned is an array of iterators, maybe? Each iterator has 2 values, ElementName and ElementStatus (a status code)
            // GetDeviceStatuses2(ref object): object returned is an array of iterators, maybe? Each iterator has 3 values, ElementName, ElementStatusString, and ElementStatus (a status code)
            var errorCode = acquisitionLib.GetDeviceStatuses(ref wrapper);

            if (errorCode != 0)
            {
                return null;
            }

            var deviceStatuses = (object[,])wrapper;
            if (deviceStatuses == null)
            {
                return new List<XDeviceStatus>() { new XDeviceStatus("No devices found", "error", -1, "No devices" )};
            }

            var statuses = new List<XDeviceStatus>();
            for (var i = 0; i < deviceStatuses.GetLength(1); i++)
            {
                var deviceName = "";
                try
                {
                    deviceName = deviceStatuses[0, i].ToString();
                    var status = Convert.ToInt32(deviceStatuses[1, i]);
                    var statusString = ConvertDeviceStatusCodeToString(status);

                    statuses.Add(new XDeviceStatus(deviceName, status, statusString));
                }
                catch (Exception ex)
                {
                    statuses.Add(new XDeviceStatus(deviceName, "parse failure", -1, ex.Message));
                }
            }

            return statuses;
        }

        //GetDeviceStatuses2
        public IReadOnlyList<XDeviceStatus> GetDeviceStatuses2()
        {
            object wrapper = new System.Runtime.InteropServices.VariantWrapper(new object());
            // GetDeviceStatuses(ref object): object returned is an array of iterators, maybe? Each iterator has 2 values, ElementName and ElementStatus (a status code)
            // GetDeviceStatuses2(ref object): object returned is an array of iterators, maybe? Each iterator has 3 values, ElementName, ElementStatusString, and ElementStatus (a status code)
            var errorCode = acquisitionLib.GetDeviceStatuses2(ref wrapper);

            if (errorCode != 0)
            {
                return null;
            }

            var deviceStatuses = (object[,])wrapper;

            var statuses = new List<XDeviceStatus>();
            for (var i = 0; i < deviceStatuses.GetLength(1); i++)
            {
                var deviceName = "";
                try
                {
                    deviceName = deviceStatuses[0, i].ToString();
                    var str = deviceStatuses[1, i].ToString();
                    var status = Convert.ToInt32(deviceStatuses[2, i]);
                    var statusString = ConvertDeviceStatusCodeToString(status);

                    statuses.Add(new XDeviceStatus(deviceName, str, status, statusString));
                }
                catch (Exception ex)
                {
                    statuses.Add(new XDeviceStatus(deviceName, "parse failure", -1, ex.Message));
                }
            }

            return statuses;
        }

        public string GetDeviceInfoString()
        {
            var statuses = GetDeviceInfo();
            if (statuses == null)
            {
                return "No devices or error";
            }

            return string.Join("; ", statuses);
        }

        //GetDeviceInfoArray
        public IReadOnlyList<DeviceInfo> GetDeviceInfo()
        {
            // short (numDevices) GetDeviceInfoArray(ref object): object returned is an array of iterators, maybe? Each iterator has 15 string values:
            // UI, CFGUI, StatusOCX, VirDev, Description, ShortName, RequiredDevice, HelpFileName, HelpFileLabel,
            // TuneHelpFileName, TuneHelpFileLabel, DirectControlOCX, Version, XcalVersion, Type (device type code)
            // Might return '2005', which is a code for an OLE exception in the Acquisition Server
            object wrapper = new System.Runtime.InteropServices.VariantWrapper(new object());
            var numDevices = acquisitionLib.GetDeviceInfoArray(ref wrapper);

            if (numDevices <= 0)
            {
                return null;
            }

            if (numDevices == 2005)
            {
                return null;
            }

            var deviceInfos = (object[,])wrapper;
            var infos = new List<DeviceInfo>();
            for (var i = 0; i < deviceInfos.GetLength(0); i++)
            {
                try
                {
                    infos.Add(new DeviceInfo(deviceInfos, i));
                }
                catch (Exception ex)
                {
                    infos.Add(new DeviceInfo("Error", "", "", "", "", "", "", "", "", "", "", "", "", "", -1, ex.Message));
                }
            }

            return infos;
        }

        public static string ConvertDeviceStatusCodeToString(int statusCode)
        {
            switch (statusCode)
            {
                case 0: return "Initialising";
                case 1: return "Ready To Download";
                case 2: return "Downloading";
                case 3: return "Preparing For Run";
                case 4: return "Ready For Run";
                case 5: return "Waiting For Contact Closure";
                case 6: return "Running";
                case 7: return "Post Run";
                case 8: return "Error";
                case 9: return "Busy";
                case 10: return "Not Connected";
                case 11: return "Standby";
                case 12: return "Off";
                case 13: return "Server Failed";
                case 14: return "Lamp Warmup";
                case 15: return "Not Ready";
                case 16: return "Direct Control";
                default: return "Illegal device status value";
            }
        }

        internal static string ConvertDeviceTypeToString(int deviceType)
        {
            switch (deviceType)
            {
                case 0: return "MS device";
                case 1: return "LC device";
                case 2: return "GC device";
                case 3: return "AS device";
                case 4: return "Detector device";
                case 5: return "Other device";
                case 6: return "All devices";
                default: return "Illegal device";
            }
        }

        /* Sequence submission event sequence:
         * (submitted)
         * MethodCheckOK
         * QueuedNewSequence
         * SequenceOpened
         * SequenceChange
         * StateChange
         * DownloadInitiated
         * StateChange
         * DownloadCompleted
         * StateChange
         * VDStateChange
         * VDStateChange
         * DevicesAreReady
         * StateChange
         * StartCmdSent
         * StateChange
         * VDStateChange
         * (contact closure)
         * VDStateChange
         * Acquiring
         * StateChange
         * NewData
         * NewData
         * NewData
         * ...
         * (triggered error)
         * VDStateChange
         * NewData
         * Pause
         * DataFileCompleted
         * RunEnded
         * StartedProcessing
         * StateChange
         *
         * (clicking buttons in Xcalibur)
         * Resume
         * Pause
         * SequenceClosed
         * SequenceChange
         */

        private void AcquisitionAcquiring(object sender, EventArgs e)
        {
            // TODO: This is useful, though we also may want to do something more with this.
            //eventReporter(EventClass.Status, EventName.Acquiring, "Acquiring");
            ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC Acquiring");
        }

        private void AcquisitionDataFileCompleted(object sender, EventArgs e)
        {
            // TODO: Only really useful if we need this value for something else; RunEnded is fired after this event.
            //eventReporter(EventClass.Status, EventName.DataFileCompleted, "DataFileCompleted");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC DataFileCompleted");
        }

        private void AcquisitionDevicesAreReady(object sender, EventArgs e)
        {
            // TODO: May not be that useful
            //eventReporter(EventClass.StateChange, EventName.DevicesAreReady, "Devices are ready");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC devices are ready");
        }

        private void AcquisitionDiskSpaceWarning(object sender, EventArgs e)
        {
            // TODO: not generally of concern, because of small raw files.
            //eventReporter(EventClass.Status, EventName.DiskSpaceWarning, "DiskSpaceWarning");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC DiskSpaceWarning");
        }

        private void AcquisitionDownloadAbandoned(object sender, EventArgs e)
        {
            // TODO: Probably should enable the event reporter for this one
            //eventReporter(EventClass.Status, EventName.DownloadAbandoned, "DownloadAbandoned");
            ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC DownloadAbandoned");
        }

        private void AcquisitionDownloadCompleted(object sender, EventArgs e)
        {
            // TODO: May be worth using to report a state?
            //eventReporter(EventClass.Status, EventName.DownloadCompleted, "DownloadCompleted");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC DownloadCompleted");
        }

        private void AcquisitionDownLoadInitiated(object sender, EventArgs e)
        {
            // TODO: Not that useful.
            //eventReporter(EventClass.Status, EventName.DownLoadInitiated, "DownLoadInitiated");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC DownLoadInitiated");
        }

        private void AcquisitionMethodCheckOK(object sender, EventArgs e)
        {
            // Not an error, and not that helpful of a message.
            eventReporter(EventClass.Status, EventName.MethodCheckOK, "Method check OK");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC method check OK");
        }

        private void AcquisitionMethodCheckFail(object sender, EventArgs e)
        {
            eventReporter(EventClass.Error, EventName.MethodCheckFail, "Method check failed");
            ApplicationLogger.LogError(LogLevel.Error, "Xcalibur LC method check failed");
        }

        private void AcquisitionNewData(object sender, EventArgs e)
        {
            //eventReporter(EventClass.Status, EventName.NewData, "NewData");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC NewData");
        }

        private void AcquisitionPause(object sender, EventArgs e)
        {
            eventReporter(EventClass.StateChange, EventName.Pause, "Acquisition paused");
            ApplicationLogger.LogMessage(LogLevel.Warning, "Xcalibur LC acquisition paused");
        }

        private void AcquisitionQueuedNewSequence(object sender, EventArgs e)
        {
            //eventReporter(EventClass.Status, EventName.QueuedNewSequence, "QueuedNewSequence");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC Queued New Sequence");
        }

        private void AcquisitionResume(object sender, EventArgs e)
        {
            eventReporter(EventClass.StateChange, EventName.Resume, "Acquisition resumed");
            ApplicationLogger.LogMessage(LogLevel.Warning, "Xcalibur LC acquisition resumed");
        }

        private void AcquisitionRunEnded(object sender, EventArgs e)
        {
            eventReporter(EventClass.Status, EventName.RunEnded, "Acquisition run completed");
            ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC acquisition run completed");
        }

        private void AcquisitionSequenceChange(object sender, EventArgs e)
        {
            //eventReporter(EventClass.Status, EventName.SequenceChange, "SequenceChange");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC SequenceChange");
        }

        private void AcquisitionSequenceClosed(object sender, EventArgs e)
        {
            //eventReporter(EventClass.Status, EventName.SequenceClosed, "SequenceClosed");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC SequenceClosed");
        }

        private void AcquisitionSequenceOpened(object sender, EventArgs e)
        {
            //eventReporter(EventClass.Status, EventName.SequenceOpened, "SequenceOpened");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC SequenceOpened");
        }

        private void AcquisitionStartCmdSent(object sender, EventArgs e)
        {
            // TODO: may be worth reporting as a status - this appears to be fired a ~10 ms before "Waiting for contact closure" is reported by the device
            //eventReporter(EventClass.Status, EventName.StartCmdSent, "StartCmdSent");
            ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC StartCmdSent");
        }

        private void AcquisitionStartedProcessing(object sender, EventArgs e)
        {
            // Not reporting anything, because we don't do any processing anyway
            //eventReporter(EventClass.Status, EventName.StartedProcessing, "StartedProcessing");
            //ApplicationLogger.LogMessage(LogLevel.Info, "Xcalibur LC StartedProcessing");
        }

        private void AcquisitionStateChange(object sender, EventArgs e)
        {
            var status = acquisitionLib.GetRunManagerStatus();
            eventReporter(EventClass.ManagerStateChange, EventName.StateChange, status);
            ApplicationLogger.LogMessage(LogLevel.Info, $"Xcalibur LC acquisition state change: {status}");
        }

        private readonly IReadOnlyList<int> notLoggedDeviceStatusCodes = new[] { 0, 2, 3, 4, 7, 14, 16 };

        private void AcquisitionVDStateChange(object sender, EventArgs e)
        {
            var statuses = GetDeviceStatuses2();
            if (statuses.Count > 0)
            {
                // TODO: Maybe add support for multiple devices? I currently see no reason we would have a configuration with multiple devices in Xcalibur and be controlling it from LCMSNet...
                // There is the theoretical idea of "have a Thermo LC Pump and Instrument running under Xcalibur with a PAL autosampler", but we don't run that way...
                eventReporter(EventClass.DeviceStateChange, EventName.VDStateChange, $"{statuses[0].StatusCode}:{statuses[0].Name}:{statuses[0].StatusString}");
                if (statuses.Any(x => !notLoggedDeviceStatusCodes.Contains(x.StatusCode)))
                {
                    // Only log certain report messages, mostly the critical state change ones and errors.
                    ApplicationLogger.LogMessage(LogLevel.Info, $"Xcalibur LC device state change: {string.Join(";", statuses.Select(x => $"{x.Name}: {x.StatusString} ({x.StatusCode})"))}");
                }
            }
        }

        private void AcquisitionInformationalMessage(object sender, _DAcquisitionEvents_InformationalMessageEvent e)
        {
            var instError = e.instrumentError ? " (Instrument error)" : "";
            //eventReporter(EventClass.Info, EventName.InformationalMessage, $"{e.title}{instError}: {e.message}");
            ApplicationLogger.LogMessage(LogLevel.Info, $"Xcalibur LC info: {e.title}{instError}: {e.message}");
        }

        private void AcquisitionWarningMessage(object sender, _DAcquisitionEvents_WarningMessageEvent e)
        {
            var instError = e.instrumentError ? " (Instrument error)" : "";
            eventReporter(EventClass.Warning, EventName.WarningMessage, $"{e.title}{instError}: {e.message}");
            ApplicationLogger.LogMessage(LogLevel.Warning, $"Xcalibur LC warning: {e.title}{instError}: {e.message}");
        }

        private void AcquisitionErrorMessage(object sender, _DAcquisitionEvents_ErrorMessageEvent e)
        {
            var instError = e.instrumentError ? " (Instrument error)" : "";
            eventReporter(EventClass.Error, EventName.ErrorMessage, $"{e.title}{instError}: {e.message}");
            ApplicationLogger.LogError(LogLevel.Error, $"Xcalibur LC error: {e.title}{instError}: {e.message}");
        }

        private void AcquisitionProgramError(object sender, _DAcquisitionEvents_ProgramErrorEvent e)
        {
            var instError = e.instrumentError ? " (Instrument error)" : "";
            eventReporter(EventClass.Error, EventName.ProgramError, $"{e.title}{instError}: {e.message}");
            ApplicationLogger.LogError(LogLevel.Error, $"Xcalibur LC program error: {e.title}{instError}: {e.message}");
        }

        /*
        public void Test()
        {
            ACQUISITIONLib.Acquisition x = new ACQUISITIONLib.AcquisitionClass();
            var sequenceName = "test";

            var deviceNames = "";
            x.GetDeviceNames(ref deviceNames); // Gets a comma-separated list of device names

            x.SubmitSequence(
                Environment.UserName, // May be limited to 10 characters?
                ref sequenceName,
                1, // First row number to run (0-based)
                1, // Last row number to run (0-based)
                false, // If true, starts this sequence immediately after current sequence; if false, adds to the end of the current queue
                true, // Automatically start the sample when the instrument is ready
                "", // Path of method to run before first sample
                "", // Path of method to run after last sample
                "", // preacquisitionProgram, an .exe or .bat to run
                "", // postacquisitionProgram, an .exe or .bat to run
                true, // Specifically refers to running the preAcquisitionProgram synchronous or asynchronous
                false, // Specifically refers to running the postAcquisitionProgram synchronous or asynchronous
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                false, // Post-acquisition actions: Run ...
                deviceNames,
                0, // 0-based
                1 // 1: ON, 2: OFF, 3: Standby
            );

            //x.GetDeviceStatuses();
        }

        public void CreateTestSequence()
        {
            IXSequence x = new XSequenceClass();
            //x.New();
            x.Open(@"C:\Xcalibur\data\Template.sld", 0);

            var samplesObj = x.Samples;
            var samples = (IXSamples)samplesObj;

            for (var i = samples.Count; i > 0; i--)
            {
                samples.Remove(i);
            }

            var sample = (IXSample)samples.Add(-1);
            sample.FileName = "test0";
            sample.Path = @"C:\Xcalibur\Data";
            sample.InstMeth = "instMethod";
            //sample.RowNumber = 1;

            sample = (IXSample)samples.Add(-1);
            sample.FileName = "test1";
            sample.Path = @"C:\Xcalibur\Data";
            sample.InstMeth = "instMethod";
            //sample.RowNumber = 2;

            sample = (IXSample)samples.Add(-1);
            sample.FileName = "test2";
            sample.Path = @"C:\Xcalibur\Data";
            sample.InstMeth = "instMethod";
            //sample.RowNumber = 3;

            x.SaveAs(@"C:\Xcalibur\data\LCMSNet.sld", "LCMSOperator", "", 1);
        }
        */

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
