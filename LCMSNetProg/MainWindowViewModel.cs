using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LcmsNet.Configuration;
using LcmsNet.Configuration.ViewModels;
using LcmsNet.Data;
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
using LcmsNet.SampleQueue.ViewModels;
using LcmsNet.Simulator.ViewModels;
using LcmsNetCommonControls.ViewModels;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using PDFGenerator;
using ReactiveUI;

namespace LcmsNet
{
    public class MainWindowViewModel : ReactiveObject, IDisposable, IHandlesLogging
    {
        /// <summary>
        /// Log File folder name.
        /// </summary>
        public const string CONST_LC_LOG_FOLDER = "Log";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            Thread.CurrentThread.Name = "Main Thread";
            windowTitleBase = "LcmsNet Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            windowTitleBase += " Cart - " + LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME);

            // simulator stuff, don't add the simulator to the system if not in emulation mode.
            var emulation = LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED, false);
            if (emulation)
            {
                windowTitleBase += " [EMULATED] ";

                //add simulator button to main form and add simulator forms.
                SimulatorVm = new SimulatorCombinedViewModel();
                CanSimulate = true;
            }
            else
            {
                CanSimulate = false;
            }

            // Construct the sample queue object that holds and manages sample data ordering
            SampleManagerVm = new SampleManagerViewModel(sampleQueue);
            scheduler = new LCMethodScheduler(sampleQueue);

            // Displays the pump data.
            PumpDisplaysPopoutVm = new PopoutViewModel(PumpDisplaysVm);

            Initialize();

            ShowAboutCommand = ReactiveCommand.Create(() => this.ShowAboutWindow());
            ReportErrorCommand = ReactiveCommand.Create<ContentControl[]>(param => this.ReportError(param));
            OpenQueueCommand = ReactiveCommand.Create(() => SampleManagerVm.ImportQueue(), this.WhenAnyValue(x => x.QueueTabSelected));
            SaveQueueCommand = ReactiveCommand.Create(() => SampleManagerVm.SaveQueue(), this.WhenAnyValue(x => x.QueueTabSelected));
            SaveQueueAsCommand = ReactiveCommand.Create(() => SampleManagerVm.SaveQueueAs(), this.WhenAnyValue(x => x.QueueTabSelected));
            ImportQueueFromClipboardCommand = ReactiveCommand.Create(() => SampleManagerVm.ImportQueueFromClipboard(), this.WhenAnyValue(x => x.QueueTabSelected));
            ImportQueueFromCsvCommand = ReactiveCommand.Create(() => SampleManagerVm.ImportQueueFromCsv(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToXmlCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToXML(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToCsvCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToCsv(), this.WhenAnyValue(x => x.QueueTabSelected));
            ExportQueueToXcaliburCommand = ReactiveCommand.Create(() => SampleManagerVm.ExportQueueToXcalibur(), this.WhenAnyValue(x => x.QueueTabSelected));

            this.WhenAnyValue(x => x.SampleManagerVm.TitleBarTextAddition).Subscribe(x => this.RaisePropertyChanged(nameof(WindowTitle)));
        }

        public ReactiveCommand<Unit, Unit> ShowAboutCommand { get; }
        public ReactiveCommand<ContentControl[], Unit> ReportErrorCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenQueueCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveQueueCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveQueueAsCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportQueueFromClipboardCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportQueueFromCsvCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportQueueToXmlCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportQueueToCsvCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportQueueToXcaliburCommand { get; }

        /// <summary>
        /// Show the About box
        /// </summary>
        private void ShowAboutWindow()
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

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

        /// <summary>
        /// Calls all objects to shutdown and finish doing what they are doing before disposal.
        /// </summary>
        public void Shutdown()
        {
            // Save fluidics designer config, if desired
            //m_fluidicsDesign.SaveConfiguration();

            // Shut off the scheduler...
            try
            {
                scheduler.Shutdown();

                // Save queue to the cache
                sampleQueue.StopRunningQueue();

                if (sampleQueue.IsDirty || !LCMSSettings.GetParameter(LCMSSettings.PARAM_EnableUndoRedo, true))
                {
                    var result =
                        MessageBox.Show(string.Format("Do you want to save changes to your queue: {0}",
                            LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME)
                        ), "Confirm Queue Save", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        sampleQueue.CacheQueue();
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

        public string WindowTitle => windowTitleBase + " " + SampleManagerVm.TitleBarTextAddition;

        /// <summary>
        /// ViewModel that manages views of the sample queue class for handling sample
        /// ordering and running.
        /// </summary>
        public SampleManagerViewModel SampleManagerVm { get; }

        /// <summary>
        /// ViewModel for showing/managing the cart hardware configuration and plumbing
        /// </summary>
        public FluidicsDesignViewModel FluidicsDesignVm { get; } = new FluidicsDesignViewModel();

        public SimulatorCombinedViewModel SimulatorVm { get; }

        /// <summary>
        /// Displays progress of samples along each column.
        /// </summary>
        public ColumnSampleProgressViewModel SampleProgressVm { get; } = new ColumnSampleProgressViewModel();

        /// <summary>
        /// Method editor viewmodel to construct new LC separations.
        /// </summary>
        public LCMethodEditorViewModel MethodEditorVm { get; } = new LCMethodEditorViewModel();

        public PopoutViewModel PumpDisplaysPopoutVm { get; }

        /// <summary>
        /// Pump display ViewModel.
        /// </summary>
        public PumpDisplaysViewModel PumpDisplaysVm { get; } = new PumpDisplaysViewModel();

        /// <summary>
        /// Notifications ViewModel.
        /// </summary>
        public NotificationSystemViewModel NotificationSystemVm { get; } = new NotificationSystemViewModel();

        /// <summary>
        /// ViewModel for displaying the messages from the different parts of the program.
        /// </summary>
        public MessagesViewModel MessagesVm { get; } = new MessagesViewModel();

        /// <summary>
        /// ViewModel for displaying the configuration of the columns and the cart metadata.
        /// </summary>
        public SystemConfigurationViewModel SystemConfiguration { get; } = new SystemConfigurationViewModel();

        public string StatusMessage
        {
            get => statusMessage;
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
            get => canSimulate;
            set => this.RaiseAndSetIfChanged(ref canSimulate, value);
        }

        public bool IsMessageError
        {
            get => isMessageError;
            private set
            {
                this.RaiseAndSetIfChanged(ref isMessageError, value);
                this.RaisePropertyChanged(nameof(IsMessageNormal));
            }
        }

        public bool IsMessageNormal => !isMessageError;

        public bool QueueTabSelected
        {
            get => queueTabSelected;
            set => this.RaiseAndSetIfChanged(ref queueTabSelected, value);
        }

        /// <summary>
        /// Error message importance level (0 is most important, 5 is least important)
        /// </summary>
        public int ErrorLevel { get; set; }

        /// <summary>
        /// Status message importance level (0 is most important, 5 is least important)
        /// </summary>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        public int MessageLevel { get; set; }

        /// <summary>
        /// Error message importance level
        /// </summary>
        public LogLevel ErrorLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(ErrorLevel);
            set => ErrorLevel = (int)value;
        }

        /// <summary>
        /// Status message importance level
        /// </summary>
        public LogLevel MessageLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(MessageLevel);
            set => MessageLevel = (int)value;
        }

        private readonly string windowTitleBase;
        private string statusMessage = "Status";
        private bool canSimulate = true;
        private bool isMessageError = false;
        private bool queueTabSelected = true;

        /// <summary>
        /// Method Scheduler and execution engine.
        /// </summary>
        private readonly LCMethodScheduler scheduler;

        /// <summary>
        /// Object that manages operation of the samples - sample queue object that holds and manages sample data ordering
        /// </summary>
        private readonly SampleQueue.SampleQueue sampleQueue = new SampleQueue.SampleQueue();

        /// <summary>
        /// default PDF generator, used to generate a pdf of information after a sample run.
        /// </summary>
        private readonly IPDF pdfGen = new PDFGen();

        /// <summary>
        /// Initializes all the forms and objects
        /// </summary>
        private void Initialize()
        {
            foreach (var column in CartConfiguration.Columns)
            {
                column.NameChanged += Column_NameChanged;
                column.StatusChanged += Column_StatusChanged;
            }

            SystemConfiguration.ColumnNameChanged += SystemConfiguration_ColumnNameChanged;

            // Notification System
            NotificationSystemVm.ActionRequired += m_notifications_ActionRequired;

            DeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            DeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
            DeviceManager.Manager.DeviceRenamed += Manager_DeviceRenamed;
            DeviceManager.Manager.DevicesInitialized += Manager_DevicesInitialized;

            ApplicationLogger.LogMessage(0, "Loading the hardware configuration.");
            FluidicsDesignVm.LoadConfiguration();

            // Create and initialize the scheduler that handles executing LC-Methods (separations, e.g. experiments)
            scheduler.SchedulerError += Scheduler_Error;
            scheduler.SampleProgress += Scheduler_SampleProgress;
            scheduler.Initialize();

            // Logging and messaging
            MessagesVm.ErrorCleared += Messages_ErrorCleared;
            MessagesVm.ErrorPresent += Messages_ErrorPresent;
            ApplicationLogger.Error += MessagesVm.LogError;
            ApplicationLogger.Error += ApplicationLogger_Error;
            ApplicationLogger.Message += MessagesVm.LogMessage;
            ApplicationLogger.Message += ApplicationLogger_Message;

            SampleManagerVm.Stop += SampleManager_Stop;

            // Initialize the hardware
            DeviceManager.Manager.InitializingDevice += Manager_InitializingDevice;
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_INITIALIZEHARDWAREONSTARTUP, false))
            {
                ApplicationLogger.LogMessage(0, "Initializing hardware.");
                failedDevices = DeviceManager.Manager.InitializeDevices();
            }

            // Load the methods from the LC-Methods folder.
            ApplicationLogger.LogMessage(0, "Reading User Methods.");
            userMethodErrors = LCMethodXmlFile.LoadMethods(Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), LCMethodFactory.CONST_LC_METHOD_FOLDER));

            ApplicationLogger.LogMessage(0, "Loading Sample Queue...");
            // Tell the sample queue to load samples from cache after everything is loaded.
            try
            {
                sampleQueue.RetrieveQueueFromCache();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message, ex);
            }

            if (failedDevices != null && failedDevices.Count > 0)
            {
                ApplicationLogger.LogMessage(0,
                    string.Format("System Unsure.  {0} devices failed to initialize.", failedDevices.Count));
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

        private Dictionary<string, List<Exception>> userMethodErrors = null;
        private List<DeviceErrorEventArgs> failedDevices = null;

        /// <summary>
        /// Don't show the errors over the splash screen, show them after the main window has been displayed.
        /// </summary>
        public void ShowInitializeFailureWindows()
        {
            if (failedDevices != null && failedDevices.Count > 0)
            {
                var display = new FailedDevicesWindow()
                {
                    DataContext = new FailedDevicesViewModel(failedDevices),
                    ShowActivated = true,
                    Owner = Application.Current.MainWindow
                };
                display.ShowDialog();
                failedDevices = null;
            }

            if (userMethodErrors != null && userMethodErrors.Count > 0)
            {
                var failedMethods = new FailedMethodLoadWindow
                {
                    DataContext = new FailedMethodLoadViewModel(userMethodErrors),
                    ShowActivated = true,
                    Owner = Application.Current.MainWindow
                };
                failedMethods.ShowDialog();
                userMethodErrors = null;
            }
        }

        private void SystemConfiguration_ColumnNameChanged(object sender, EventArgs e)
        {
            sampleQueue.UpdateAllSamples();
        }

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
                    scheduler.Stop();
                    //m_sampleQueue.StopRunningQueue();
                    ApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case DeviceNotificationAction.Shutdown:
                    ApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    //m_sampleQueue.StopRunningQueue();
                    scheduler.Stop();
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
                        scheduler.Stop();
                        var dummySample = new SampleData
                        {
                            Name = string.Format("NotificationAction--{0}", e.Method.Name)
                        };

                        if (columnID >= 0)
                        {
                            dummySample.ColumnIndex = CartConfiguration.Columns[columnID].ID;
                        }

                        dummySample.LCMethodName = e.Method.Name;
                        sampleQueue.RunNext(dummySample);


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
                            stupidSample.ColumnIndex = CartConfiguration.Columns[columnID].ID;
                        }
                        stupidSample.LCMethodName = e.Method.Name;
                        sampleQueue.RunNext(stupidSample);

                        ApplicationLogger.LogError(0,
                            string.Format(
                                "The special method {2} was queued to run next on the column {0} because of the notification system event {1} ",
                                columnID,
                                e.Name,
                                e.Method.Name));

                        if (!sampleQueue.IsRunning)
                        {
                            sampleQueue.StartSamples();
                        }
                    }
                    break;
            }
        }

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
            SampleManagerVm.DevicesChanged();
            ApplicationLogger.LogMessage(0, "Device initialization complete.");
        }

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
            SampleManagerVm.DevicesChanged();
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
            SampleManagerVm.DevicesChanged();
            ApplicationLogger.LogMessage(0, "Added device " + device.Name);
        }

        /// <summary>
        /// Updates status window when device is being initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manager_InitializingDevice(object sender, DeviceManagerStatusArgs e)
        {
            ApplicationLogger.LogMessage(-1, e.Message);
            SampleManagerVm.DevicesChanged();
        }

        /// <summary>
        /// Handles displaying a message when an object notifies LCMS at level 0.
        /// </summary>
        /// <param name="messageLevel">Message filter.</param>
        /// <param name="args">Data associated with messages.</param>
        private void ApplicationLogger_Message(int messageLevel, MessageLoggerArgs args)
        {
            if (messageLevel > MessageLevel || messageLevel > ApplicationLogger.CONST_STATUS_LEVEL_USER)
            {
                return;
            }

            StatusMessage = args.Message;
        }

        /// <summary>
        /// Handles displaying an error message when notified LCMS.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        private void ApplicationLogger_Error(int errorLevel, ErrorLoggerArgs args)
        {
            if (errorLevel > ErrorLevel)
            {
                return;
            }

            StatusMessage = args.Message;
        }

        public void LogError(int errorLevel, ErrorLoggerArgs args)
        {
            ApplicationLogger_Error(errorLevel, args);
        }

        public void LogMessage(int msgLevel, MessageLoggerArgs args)
        {
            ApplicationLogger_Message(msgLevel, args);
        }

        /// <summary>
        /// Handles the stop event to stop any sample runs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SampleManager_Stop(object sender, EventArgs e)
        {
            // Tell the scheduler to stop running!
            scheduler.Stop();
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
                        sample.ColumnIndex + 1,
                        CartConfiguration.Columns[sample.ColumnIndex].Name,
                        lcEvent.Device.Name,
                        lcEvent.Name,
                        sample.Name);
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
                            FileUtilities.GetUniqueFileName(Path.Combine(docPath, sample.Name), ".pdf");
                        pdfGen.WritePDF(filePath, sample.Name, sample,
                            CartConfiguration.NumberOfEnabledColumns.ToString(), CartConfiguration.Columns,
                            DeviceManager.Manager.Devices, configImage);
                    }
                    catch (Exception ex)
                    {
                        ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                            "PDF Generation Error for " + sample.Name + " " + ex.Message, ex, sample);
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

        private void ReportError(ContentControl[] controls)
        {
            var manager = LCMethodManager.Manager;
            var logPath = FileLogger.LogPath;

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

        public void Dispose()
        {
            SampleProgressVm.Dispose();
            NotificationSystemVm.Dispose();
            FluidicsDesignVm.Dispose();
            DMSDataContainer.DBTools.CloseConnection();
            TaskBarManipulation.Instance.Dispose();
        }
    }
}
