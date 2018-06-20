﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LcmsNet.Configuration;
using LcmsNet.Configuration.ViewModels;
using LcmsNet.Devices.Fluidics.ViewModels;
using LcmsNet.Devices.Pumps.ViewModels;
using LcmsNet.Devices.ViewModels;
using LcmsNet.Devices.Views;
using LcmsNet.IO;
using LcmsNet.Logging.ViewModels;
using LcmsNet.Method;
using LcmsNet.Method.ViewModels;
using LcmsNet.Method.Views;
using LcmsNet.Notification;
using LcmsNet.Notification.ViewModels;
using LcmsNet.Reporting;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.IO;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNet.Simulator.ViewModels;
using LcmsNetCommonControls.ViewModels;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSQLiteTools;
using PDFGenerator;
using ReactiveUI;

namespace LcmsNet
{
    public class MainWindowViewModel : ReactiveObject, IDisposable
    {

        #region Constants

        /// <summary>
        /// Log File folder name.
        /// </summary>
        public const string CONST_LC_LOG_FOLDER = "Log";

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            Initialize();
            SetupCommands();
        }

        #region Commands

        public ReactiveCommand<Unit, Unit> ShowAboutCommand { get; private set; }
        public ReactiveCommand<ContentControl[], Unit> ReportErrorCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OpenQueueCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveQueueCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SaveQueueAsCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ExportQueueToXmlCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ExportQueueToCsvCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ExportQueueToXcaliburCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ExportQueueToMRMCommand { get; private set; }

        private void SetupCommands()
        {
            ShowAboutCommand = ReactiveCommand.Create(() => this.ShowAboutWindow());
            ReportErrorCommand = ReactiveCommand.Create<ContentControl[]>(param => this.ReportError(param));
            OpenQueueCommand = ReactiveCommand.Create(() => SampleManagerVm.ImportQueue(), this.WhenAnyValue(x => x.QueueTabSelected));
            SaveQueueCommand = ReactiveCommand.Create(() => SampleManagerVm.SaveQueue(), this.WhenAnyValue(x => x.QueueTabSelected));
            SaveQueueAsCommand = ReactiveCommand.Create(() => SampleManagerVm.SaveQueueAs(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToXmlCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToXML(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToCsvCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToCsv(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToXcaliburCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToXcalibur(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToMRMCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportMRMFiles(), this.WhenAnyValue(x => x.QueueTabSelected));
        }

        #endregion

        /// <summary>
        /// Show the About box
        /// </summary>
        private void ShowAboutWindow()
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

        #region "Column event handlers"

        void Column_NameChanged(object sender, string name, string oldName)
        {
            var column = sender as ColumnData;
            if (column != null)
            {
                LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNNAME + column.ID, column.Name);
            }
        }

        void Column_StatusChanged(object sender, ColumnStatus status, ColumnStatus oldStatus)
        {
            var column = sender as ColumnData;
            if (column != null && status != ColumnStatus.Running && oldStatus != ColumnStatus.Running)
            {
                LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNDISABLED + column.ID, (column.Status == ColumnStatus.Disabled).ToString());
            }
        }

        #endregion

        #region Cleanup Methods

        /// <summary>
        /// Calls all objects to shutdown and finish doing what they are doing before disposal.
        /// </summary>
        public void Shutdown()
        {
            // Save fluidics designer config, if desired
            //m_fluidicsDesign.SaveConfiguration();

            // Cache the selected separation type
            SQLiteTools.SaveSelectedSeparationType(LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE));

            // Shut off the scheduler...
            try
            {
                m_scheduler.Shutdown();

                // Save queue to the cache
                m_sampleQueue.StopRunningQueue();

                if (m_sampleQueue.IsDirty)
                {
                    var result =
                        MessageBox.Show(string.Format("Do you want to save changes to your queue: {0}",
                            LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME)
                        ), "Confirm Queue Save", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        m_sampleQueue.CacheQueue(true);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Warning: Exception occurred saving data to cache. Exception Message: \"" + e.Message + "\". See log for more details.");
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Error occurred when saving queue to cache!", e);
            }

            //TODO: Check to see if things shutdown.
        }

        #endregion

        #region Properties

        public string WindowTitle
        {
            get { return windowTitle; }
            private set { this.RaiseAndSetIfChanged(ref windowTitle, value); }
        }

        /// <summary>
        /// ViewModel that manages views of the sample queue class for handling sample
        /// ordering and running.
        /// </summary>
        public SampleManagerViewModel SampleManagerVm
        {
            get { return sampleManager; }
            private set { this.RaiseAndSetIfChanged(ref sampleManager, value); }
        }

        public FluidicsDesignViewModel FluidicsDesignVm
        {
            get { return fluidicsDesign; }
            private set { this.RaiseAndSetIfChanged(ref fluidicsDesign, value); }
        }

        public SimulatorCombinedViewModel SimulatorVm
        {
            get { return simCombined; }
            private set { this.RaiseAndSetIfChanged(ref simCombined, value); }
        }

        public ColumnSampleProgressViewModel SampleProgressVm
        {
            get { return sampleProgress; }
            private set { this.RaiseAndSetIfChanged(ref sampleProgress, value); }
        }

        public LCMethodEditorViewModel MethodEditorVm
        {
            get { return methodEditor; }
            private set { this.RaiseAndSetIfChanged(ref methodEditor, value); }
        }

        public PopoutViewModel PumpDisplaysPopoutVm
        {
            get { return pumpDisplaysPopoutVm; }
            private set { this.RaiseAndSetIfChanged(ref pumpDisplaysPopoutVm, value); }
        }

        public PumpDisplaysViewModel PumpDisplaysVm
        {
            get { return pumpDisplays; }
            private set { this.RaiseAndSetIfChanged(ref pumpDisplays, value); }
        }

        public NotificationSystemViewModel NotificationSystemVm
        {
            get { return notifications; }
            private set { this.RaiseAndSetIfChanged(ref notifications, value); }
        }

        public MessagesViewModel MessagesVm
        {
            get { return messages; }
            private set { this.RaiseAndSetIfChanged(ref messages, value); }
        }

        public SystemConfigurationViewModel SystemConfiguration
        {
            get { return systemConfiguration; }
            private set { this.RaiseAndSetIfChanged(ref systemConfiguration, value); }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.Replace("\r\n", "\t").Replace("\n", "\t");
                }
                this.RaiseAndSetIfChanged(ref statusMessage, value);
            }
        }

        public bool CanSimulate
        {
            get { return canSimulate; }
            set { this.RaiseAndSetIfChanged(ref canSimulate, value); }
        }

        public bool IsMessageError
        {
            get { return isMessageError; }
            private set
            {
                this.RaiseAndSetIfChanged(ref isMessageError, value);
                this.RaisePropertyChanged(nameof(IsMessageNormal));
            }
        }

        public bool IsMessageNormal
        {
            get { return !isMessageError; }
        }

        public bool QueueTabSelected
        {
            get { return queueTabSelected; }
            set { this.RaiseAndSetIfChanged(ref queueTabSelected, value); }
        }

        #endregion

        #region Members

        private string windowTitle;
        private string statusMessage = "Status";
        private bool canSimulate = true;
        private bool isMessageError = false;
        private bool queueTabSelected = true;
        private readonly SQLiteTools sqlInstance = SQLiteTools.GetInstance();

        private FluidicsDesignViewModel fluidicsDesign;

        /// <summary>
        /// Method Scheduler and execution engine.
        /// </summary>
        private LCMethodScheduler m_scheduler;

        /// <summary>
        /// Form that manages views of the sample queue class for handling sample
        /// ordering and running.
        /// </summary>
        private SampleManagerViewModel sampleManager;

        /// <summary>
        /// Object that manages operation of the samples.
        /// </summary>
        private SampleQueue.SampleQueue m_sampleQueue;

        /// <summary>
        /// Form that displays the configuration of the columns.
        /// </summary>
        private SystemConfigurationViewModel systemConfiguration;

        /// <summary>
        /// Form that displays the messages from the different parts of the program.
        /// </summary>
        private MessagesViewModel messages;

        /// <summary>
        /// Method editor form to construct new LC separations.
        /// </summary>
        private LCMethodEditorViewModel methodEditor;

        /// <summary>
        /// Displays progress of samples along each column.
        /// </summary>
        private ColumnSampleProgressViewModel sampleProgress;

        /// <summary>
        /// Notifications form.
        /// </summary>
        private NotificationSystemViewModel notifications;

        /// <summary>
        /// Display form for the pumps.
        /// </summary>
        private PumpDisplaysViewModel pumpDisplays;

        private PopoutViewModel pumpDisplaysPopoutVm = null;

        /// <summary>
        /// Class for logging to the log files and other listeners.  Wraps the application logger static methods.
        /// </summary>
        private ApplicationLogger m_logger;

        /// <summary>
        /// default PDF generator, used to generate a pdf of information after a sample run.
        /// </summary>
        private readonly IPDF m_pdfGen = new PDFGen();

        private SimulatorCombinedViewModel simCombined;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes all the forms and objects
        /// </summary>
        private void Initialize()
        {
            Thread.CurrentThread.Name = "Main Thread";
            m_logger = new ApplicationLogger();
            WindowTitle = "LcmsNet Version: " + Assembly.GetEntryAssembly().GetName().Version;
            WindowTitle += " Cart - " + LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME);
            var emulation = LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED);
            if (emulation != null)
            {
                var isEmulated = Convert.ToBoolean(emulation);
                if (isEmulated)
                {
                    WindowTitle += " [EMULATED] ";
                }
            }

            foreach (var column in CartConfiguration.Columns)
            {
                column.NameChanged += Column_NameChanged;
                column.StatusChanged += Column_StatusChanged;
            }


            SystemConfiguration = new SystemConfigurationViewModel();
            SystemConfiguration.ColumnNameChanged += SystemConfiguration_ColumnNameChanged;
            SQLiteTools.GetSepTypeList(false);

            // Fludics Design display
            FluidicsDesignVm = new FluidicsDesignViewModel();

            // Notification System
            NotificationSystemVm = new NotificationSystemViewModel(DeviceManager.Manager);
            NotificationSystemVm.ActionRequired += m_notifications_ActionRequired;


            // Construct the sample queue object that holds and manages sample data ordering
            m_sampleQueue = new SampleQueue.SampleQueue();
            SampleManagerVm = new SampleManagerViewModel(m_sampleQueue);

            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
            DeviceManager.Manager.DeviceRenamed += Manager_DeviceRenamed;
            DeviceManager.Manager.DevicesInitialized += Manager_DevicesInitialized;

            // Displays the pump data.
            PumpDisplaysVm = new PumpDisplaysViewModel();
            PumpDisplaysPopoutVm = new PopoutViewModel(PumpDisplaysVm);

            ApplicationLogger.LogMessage(0, "Loading the hardware configuration.");
            FluidicsDesignVm.LoadConfiguration();


            // Create and initialize the scheduler that handles executing LC-Methods (separations, e.g. experiments)
            m_scheduler = new LCMethodScheduler(m_sampleQueue) { Logger = m_logger };
            m_scheduler.SchedulerError += Scheduler_Error;
            m_scheduler.SampleProgress += Scheduler_SampleProgress;
            m_scheduler.Initialize();

            //
            // Logging and messaging
            //
            MessagesVm = new MessagesViewModel();
            MessagesVm.ErrorCleared += Messages_ErrorCleared;
            MessagesVm.ErrorPresent += Messages_ErrorPresent;
            ApplicationLogger.Error += MessagesVm.ShowErrors;
            ApplicationLogger.Error += ApplicationLogger_Error;
            ApplicationLogger.Message += MessagesVm.ShowMessage;
            ApplicationLogger.Message += ApplicationLogger_Message;

            // Method Editor
            MethodEditorVm = new LCMethodEditorViewModel();
            SampleProgressVm = new ColumnSampleProgressViewModel();
            SampleManagerVm.Stop += SampleManager_Stop;


            // Get the most recently used separation type
            LCMSSettings.SetParameter(LCMSSettings.PARAM_SEPARATIONTYPE, SQLiteTools.GetDefaultSeparationType());

            // Initialize the hardware
            var failedDeviceFlag = false;
            var failedCount = 0;
            DeviceManager.Manager.InitialzingDevice += Manager_InitialzingDevice;
            FailedDevicesWindow display = null;
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_INITIALIZEHARDWAREONSTARTUP, false))
            {
                ApplicationLogger.LogMessage(0, "Initializing hardware.");
                var failedDevices = DeviceManager.Manager.InitializeDevices();
                if (failedDevices != null && failedDevices.Count > 0)
                {
                    failedDeviceFlag = true;
                    failedCount = failedDevices.Count;
                    var displayVm = new FailedDevicesViewModel(failedDevices);
                    display = new FailedDevicesWindow()
                    {
                        DataContext = displayVm,
                        ShowActivated = true,
                    };
                    display.ShowDialog();
                }
            }

            // simulator stuff, don't add the simulator to the system if not in emulation mode.
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED, false))
            {
                //add simulator button to main form and add simulator forms.
                SimulatorVm = new SimulatorCombinedViewModel();
                //AddForm(simCombined);
                CanSimulate = true;
            }
            else
            {
                //toolStripButtonSimulate.Visible = false;
                //toolStripSeparatorSimulator.Visible = false;
                CanSimulate = false;
            }

            // Load the methods from the LC-Methods folder.
            ApplicationLogger.LogMessage(0, "Reading User Methods.");
            var userMethodErrors = LCMethodManager.Manager.LoadMethods(Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONPATH), LCMethodFactory.CONST_LC_METHOD_FOLDER));

            if (userMethodErrors.Count > 0)
            {
                var failedMethods = new FailedMethodLoadWindow
                {
                    DataContext = new FailedMethodLoadViewModel(userMethodErrors),
                    ShowActivated = true,
                };
                failedMethods.ShowDialog();
            }

            //AddForm(fluidicsDesign);

            //AddForm(pumpDisplays);
            //AddForm(messages);
            //AddForm(m_notifications);
            //AddForm(methodEditor);
            //AddForm(sampleProgress);
            //AddForm(systemConfiguration);
            //AddForm(m_sampleManager);

            ApplicationLogger.LogMessage(0, "Loading Sample Queue...");
            //Application.DoEvents();
            //
            // Tell the sample queue to load samples from cache after everything is loaded.
            //
            try
            {
                m_sampleQueue.RetrieveQueueFromCache();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message, ex);
            }

            if (failedDeviceFlag)
            {
                ApplicationLogger.LogMessage(0,
                    string.Format("System Unsure.  {0} devices failed to initialize.", failedCount));
            }
            else if (userMethodErrors.Count > 0)
            {
                ApplicationLogger.LogMessage(0, "All devices initialized.  Errors were found in the LC-Methods");
            }
            else
            {
                ApplicationLogger.LogMessage(0, "System Ready.");
            }

            SampleProgressVm.PreviewAvailable += SampleManagerVm.PreviewAvailable;
            NotificationSystemVm.LoadNotificationFile();
        }

        #endregion

        private void SystemConfiguration_ColumnNameChanged(object sender, EventArgs e)
        {
            m_sampleQueue.UpdateAllSamples();
        }

        #region Notification System.

        /// <summary>
        /// Handles events from the notification system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_notifications_ActionRequired(object sender, NotificationSetting e)
        {
            NotifyHandler(sender, e);
        }

        private void NotifyHandler(object sender, NotificationSetting e)
        {
            switch (e.Action)
            {
                case DeviceNotificationAction.Stop:
                    ApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    m_scheduler.Stop();
                    //m_sampleQueue.StopRunningQueue();
                    ApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case DeviceNotificationAction.Shutdown:
                    ApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    //m_sampleQueue.StopRunningQueue();
                    m_scheduler.Stop();
                    ApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case DeviceNotificationAction.StopAndRunMethodNow:
                    // Runs the method if it is not null.
                    if (e.Method != null)
                    {
                        ApplicationLogger.LogError(0,
                            string.Format(
                                "The notification system is stopping all current runs and running the special method {1} because of the setting {0}",
                                e.Name, e.Method.Name));
                        var columnID = e.Method.Column;

                        //m_sampleQueue.StopRunningQueue();
                        m_scheduler.Stop();
                        var dummySample = new SampleData
                        {
                            DmsData = {
                                DatasetName = string.Format("NotificationAction--{0}", e.Method.Name)
                            }
                        };

                        if (columnID >= 0)
                        {
                            dummySample.ColumnData = CartConfiguration.Columns[columnID];
                        }

                        dummySample.LCMethod = e.Method;
                        m_sampleQueue.RunNext(dummySample);


                        ApplicationLogger.LogError(0,
                            string.Format(
                                "Queue restarted with special method {1} because of the setting {0}", e.Name,
                                e.Method.Name));
                    }
                    break;
                case DeviceNotificationAction.RunMethodNext:
                    if (e.Method != null)
                    {
                        var columnID = e.Method.Column;

                        ApplicationLogger.LogError(0,
                            string.Format(
                                "The notification system is queuing the special method {1} because of the setting {0}.",
                                e.Name, e.Method.Name));

                        var stupidSample = new SampleData();
                        if (columnID >= 0)
                        {
                            stupidSample.ColumnData = CartConfiguration.Columns[columnID];
                        }
                        stupidSample.LCMethod = e.Method;
                        m_sampleQueue.RunNext(stupidSample);

                        ApplicationLogger.LogError(0,
                            string.Format(
                                "The special method {2} was queued to run next on the column {0} because of the notification system event {1} ",
                                columnID,
                                e.Name,
                                e.Method.Name));

                        if (!m_sampleQueue.IsRunning)
                        {
                            m_sampleQueue.StartSamples();
                        }
                    }
                    break;
            }
        }

        #endregion

        private void Messages_ErrorPresent(object sender, EventArgs e)
        {
            IsMessageError = true;
        }

        private void Messages_ErrorCleared(object sender, EventArgs e)
        {
            IsMessageError = false;
        }

        private void Manager_DevicesInitialized(object sender, EventArgs e)
        {
            ApplicationLogger.LogMessage(0, "Device initialization complete.");
        }

        #region Device Manager Events

        /// <summary>
        ///
        /// </summary>
        /// <param name="device"></param>
        private void RegisterDeviceEventHandlers(IDevice device)
        {
            var sampler = device as IAutoSampler;
            if (sampler != null)
            {
                sampler.TrayNamesRead += SampleManagerVm.AutoSamplerTrayList;
            }

            var network = device as INetworkStart;
            if (network != null)
            {
                network.MethodNames += SampleManagerVm.InstrumentMethodList;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="device"></param>
        private void DeRegisterDeviceEventHandlers(IDevice device)
        {
            var type = device.GetType();
            if (type.IsAssignableFrom(typeof(IAutoSampler)))
            {
                var sampler = device as IAutoSampler;
                if (sampler != null)
                    sampler.TrayNamesRead -= SampleManagerVm.AutoSamplerTrayList;
            }

            if (type.IsAssignableFrom(typeof(INetworkStart)))
            {
                var network = device as INetworkStart;
                if (network != null)
                    network.MethodNames -= SampleManagerVm.InstrumentMethodList;
            }
        }

        /// <summary>
        /// Handles when a device is renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRenamed(object sender, IDevice device)
        {
            ApplicationLogger.LogMessage(0, "Renamed a device to " + device.Name);
        }

        /// <summary>
        /// Handles when a device is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRemoved(object sender, IDevice device)
        {
            DeRegisterDeviceEventHandlers(device);
            ApplicationLogger.LogMessage(0, "Removed device " + device.Name);
        }

        /// <summary>
        /// Handles when a device is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceAdded(object sender, IDevice device)
        {
            RegisterDeviceEventHandlers(device);
            ApplicationLogger.LogMessage(0, "Added device " + device.Name);
        }

        /// <summary>
        /// Updates status window when device is being initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manager_InitialzingDevice(object sender, DeviceManagerStatusArgs e)
        {
            ApplicationLogger.LogMessage(-1, e.Message);
        }

        #region Methods

        public void SetStatusMessage(int level, string message)
        {
            StatusMessage = message;
        }

        /// <summary>
        /// Displays the suggested string on the status bar.
        /// </summary>
        /// <param name="message">Message to display to the user.</param>
        public void SetStatusMessage(string message)
        {
            //toolStripStatusLabel.Text = message;
            StatusMessage = message;
        }

        #endregion

        /// <summary>
        /// Handles displaying a message when an object notifies LCMS at level 0.
        /// </summary>
        /// <param name="messageLevel">Message filter.</param>
        /// <param name="args">Data associated with messages.</param>
        private void ApplicationLogger_Message(int messageLevel, MessageLoggerArgs args)
        {
            if (args != null && messageLevel <= ApplicationLogger.CONST_STATUS_LEVEL_USER)
            {
                SetStatusMessage(args.Message);
            }
        }

        /// <summary>
        /// Handles displaying an error message when notified LCMS.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        private void ApplicationLogger_Error(int errorLevel, ErrorLoggerArgs args)
        {
            SetStatusMessage(args.Message);
        }

        #endregion

        #region Scheduler and Related Events

        /// <summary>
        /// Handles the stop event to stop any sample runs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleManager_Stop(object sender, EventArgs e)
        {
            // Tell the scheduler to stop running!
            m_scheduler.Stop();
            SampleProgressVm.ResetVisuals();
        }

        /// <summary>
        /// Updates the status windows and log files with messages.
        /// </summary>
        private void UpdateSampleProgress(object sender, SampleProgressEventArgs args)
        {
            var message = "";
            var sample = args.Sample;
            LCEvent lcEvent = null;
            LCMethod lcMethod;
            var isError = false;

            //
            // Construct the message to display
            //
            switch (args.ProgressType)
            {
                case SampleProgressType.RunningNextEvent:
                    lcMethod = sample.ActualLCMethod;
                    lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    message = string.Format("Sample Event: Column={0}: ColumnID={1}: Device={2}.{3}: Sample={4}",
                        sample.ColumnData.ID + 1,
                        sample.ColumnData.Name,
                        lcEvent.Device.Name,
                        lcEvent.Name,
                        sample.DmsData.DatasetName);
                    break;
                case SampleProgressType.Error:
                    message = "";
                    lcMethod = sample.ActualLCMethod;
                    var eventNumber = lcMethod.CurrentEventNumber;
                    if (eventNumber < lcMethod.Events.Count && eventNumber > -1)
                    {
                        lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    }
                    SampleProgressVm.UpdateError(sample, lcEvent);
                    ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, args.Message,
                        null, args.Sample);
                    isError = true;
                    break;
                case SampleProgressType.Stopped:
                    lcMethod = sample.ActualLCMethod;
                    lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    message = string.Empty;
                    SampleProgressVm.UpdateError(sample, lcEvent);
                    isError = true;
                    break;
                case SampleProgressType.Complete:
                    if (FluidicsDesignVm == null)
                    {
                        break;
                    }
                    var configImage = FluidicsDesignVm.GetImage();
                    string docPath;
                    if (sample == null)
                    {
                        break;
                    }
                    try
                    {
                        docPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_PDFPATH);
                    }
                    catch
                    {
                        // default to this path..
                        docPath = @"C:\pdftest\";
                    }
                    try
                    {
                        var pdfFolder = new DirectoryInfo(docPath);
                        if (!pdfFolder.Exists)
                        {
                            if (!pdfFolder.Root.Exists)
                            {
                                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                                    "Invalid PDF document folder defined in LcmsNet.exe.config (drive not found): " +
                                    docPath);
                                break;
                            }
                            pdfFolder.Create();
                        }

                        message = string.Empty;
                        if (bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYMETHODFOLDERS)))
                        {
                            MethodFileTools.MoveLocalMethodFiles();
                        }
                        var filePath =
                            FileUtilities.UniqifyFileName(Path.Combine(docPath, sample.DmsData.DatasetName), ".pdf");
                        m_pdfGen.WritePDF(filePath, sample.DmsData.DatasetName, sample,
                            CartConfiguration.NumberOfEnabledColumns.ToString(), CartConfiguration.Columns,
                            DeviceManager.Manager.Devices, configImage);
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                            "PDF Generation Error for " + sample.DmsData.DatasetName + " " + ex.Message, ex, sample);
                    }
                    break;
                case SampleProgressType.Started:
                    SampleProgressVm.ResetColumn(sample);
                    message = string.Empty;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                ApplicationLogger.LogMessage(0, message, sample);
            }


            // Update visual progress. -- Errors are already updated above.
            if (!isError && sample != null)
            {
                SampleProgressVm.UpdateSample(sample);
            }
        }

        /// <summary>
        /// Handles updating the status window when the next event starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Scheduler_SampleProgress(object sender, SampleProgressEventArgs args)
        {
            UpdateSampleProgress(null, args);
        }

        /// <summary>
        /// Handles displaying the error message on the status bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sample"></param>
        /// <param name="errorMessage"></param>
        private void Scheduler_Error(object sender, SampleData sample, string errorMessage)
        {
            var args = new SampleProgressEventArgs(errorMessage, sample, SampleProgressType.Error);
            UpdateSampleProgress(sender, args);
        }

        #endregion

        #region Error Reporting

        private void ReportError(ContentControl[] controls)
        {
            var manager = LCMethodManager.Manager;
            var logPath = FileLogging.LogPath;

            var controlList = new List<ContentControl>();
            var about = new AboutWindow();
            // Open it to make it render, then close it when done.
            about.Show();
            controlList.Add(about);
            controlList.AddRange(controls);

            var reportVm = new CreateErrorReportViewModel(manager, logPath, controlList);
            var reportWindow = new CreateErrorReportWindow() {DataContext = reportVm};

            reportWindow.ShowDialog();
            about.Close();
        }

        #endregion

        public void Dispose()
        {
            DMSDataContainer.DBTools.CloseConnection();
            SQLiteTools.CloseConnection();
            sqlInstance.Dispose();
        }
    }
}
