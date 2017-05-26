using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using LcmsNet.Configuration;
using LcmsNet.Devices.Fluidics;
using LcmsNet.IO;
using LcmsNet.Logging;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNet.Notification;
using LcmsNet.Notification.Forms;
using LcmsNet.Properties;
using LcmsNet.Reporting.Forms;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.Forms;
using LcmsNet.SampleQueue.IO;
using LcmsNet.Simulator;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;
using LcmsNetSQLiteTools;
using PDFGenerator;

namespace LcmsNet
{
    public partial class formMDImain : Form
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
        public formMDImain()
        {
            InitializeComponent();
            Initialize();
        }

        #region "Column event handlers"

        void column_NameChanged(object sender, string name, string oldName)
        {
            var column = sender as classColumnData;
            if (column != null)
            {
                classLCMSSettings.SetParameter(classLCMSSettings.PARAM_COLUMNNAME + column.ID, column.Name);
            }
        }

        #endregion

        #region Cleanup Methods

        /// <summary>
        /// Calls all objects to shutdown and finish doing what they are doing before disposal.
        /// </summary>
        private void Shutdown()
        {
            // Save fluidics designer config, if desired
            m_fluidicsDesign.SaveConfiguration();

            // Cache the selected separation type
            classSQLiteTools.SaveSelectedSeparationType(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE));

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
                            classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME)
                            ), "Confirm Queue Save", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        m_sampleQueue.CacheQueue(true);
                    }
                }
            }
            catch
            {
                // Ignore errors here
            }

            //TODO: Check to see if things shutdown.
        }

        #endregion

        #region Fluidics Form Event Handlers

        /// <summary>
        /// Updates status message on main status window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("Unused")]
        void m_fluidicsDesign_Status(object sender, classDeviceStatusEventArgs e)
        {
            classApplicationLogger.LogMessage(0, e.Message);
        }

        #endregion

        /// <summary>
        /// For updating the status message.
        /// </summary>
        /// <param name="message">Message to display.</param>
        [Obsolete("Unused")]
        private delegate void DelegateUpdateMessage(int level, string message);

        #region Members

        private FluidicsDesign m_fluidicsDesign;

        /// <summary>
        /// Method Scheduler and execution engine.
        /// </summary>
        private classLCMethodScheduler m_scheduler;

        /// <summary>
        /// Form that manages views of the sample queue class for handling sample
        /// ordering and running.
        /// </summary>
        private formSampleManager2 m_sampleManager;

        /// <summary>
        /// Object that manages operation of the samples.
        /// </summary>
        private classSampleQueue m_sampleQueue;

        /// <summary>
        /// Form that displays the configuration of the columns.
        /// </summary>
        private formSystemConfiguration2 m_systemConfiguration;

        /// <summary>
        /// Form that displays the messages from the different parts of the program.
        /// </summary>
        private formMessageWindow2 m_messages;

        /// <summary>
        /// Method editor form to construct new LC separations.
        /// </summary>
        private formMethodEditor m_methodEditor;

        /// <summary>
        /// Displays progress of samples along each column.
        /// </summary>
        private formColumnSampleProgress m_sampleProgress;

        /// <summary>
        /// Notifications form.
        /// </summary>
        private formNotificationSystem m_notifications;

        /// <summary>
        /// About box.
        /// </summary>
        private formAbout m_about;

        /// <summary>
        /// Display form for the pumps.
        /// </summary>
        private formPumpDisplays m_displays;

        /// <summary>
        /// Class for logging to the log files and other listeners.  Wraps the application logger static methods.
        /// </summary>
        private classApplicationLogger m_logger;

        /// <summary>
        /// default PDF generator, used to generate a pdf of information after a sample run.
        /// </summary>
        private readonly IPDF m_pdfGen = new PDFGen();

        private formSimulatorCombined m_simCombined;
        private formSimConfiguration m_simConfig;
        private formSimulatorControlsAndCharts m_simControlsAndCharts;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes all the forms and objects
        /// </summary>
        private void Initialize()
        {
            Thread.CurrentThread.Name = "Main Thread";
            m_logger = new classApplicationLogger();
            Text = "LcmsNet Version: " + Application.ProductVersion;
            Text += " Cart - " + classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
            var emulation = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED);
            if (emulation != null)
            {
                var isEmulated = Convert.ToBoolean(emulation);
                if (isEmulated)
                {
                    Text += " [EMULATED] ";
                }
            }

            foreach (var column in classCartConfiguration.Columns)
            {
                column.NameChanged += column_NameChanged;
            }


            m_systemConfiguration = new formSystemConfiguration2();
            m_systemConfiguration.ColumnNameChanged += m_systemConfiguration_ColumnNameChanged;
            classSQLiteTools.GetSepTypeList(false);

            // Fludics Design display
            m_fluidicsDesign = new FluidicsDesign
            {
                Icon = Icon,
                Dock = DockStyle.Fill
            };

            // Notification System
            m_notifications = new formNotificationSystem(classDeviceManager.Manager);
            m_notifications.ActionRequired +=
                m_notifications_ActionRequired;


            // Construct the sample queue object that holds and manages sample data ordering
            m_sampleQueue = new classSampleQueue();
            m_sampleManager = new formSampleManager2(m_sampleQueue);

            classDeviceManager.Manager.DeviceAdded += Manager_DeviceAdded;
            classDeviceManager.Manager.DeviceRemoved += Manager_DeviceRemoved;
            classDeviceManager.Manager.DeviceRenamed += Manager_DeviceRenamed;
            classDeviceManager.Manager.DevicesInitialized += Manager_DevicesInitialized;

            // Displays the pump data.
            m_displays = new formPumpDisplays();
            m_displays.Tack += m_displays_Tack;
            m_displays.UnTack += m_displays_UnTack;
            m_displays.Icon = Icon;
            m_displays.IsTacked = true;

            classApplicationLogger.LogMessage(0, "Loading the hardware configuration.");
            m_fluidicsDesign.LoadConfiguration();


            // Create and initialize the scheduler that handles executing LC-Methods (separations, e.g. experiments)
            m_scheduler = new classLCMethodScheduler(m_sampleQueue) {Logger = m_logger};
            m_scheduler.SchedulerError += Scheduler_Error;
            m_scheduler.SampleProgress += Scheduler_SampleProgress;
            m_scheduler.Initialize();

            //
            // Logging and messaging
            //
            m_messages = new formMessageWindow2();
            m_messages.ErrorCleared += m_messages_ErrorCleared;
            m_messages.ErrorPresent += m_messages_ErrorPresent;
            classApplicationLogger.Error += m_messages.ShowErrors;
            classApplicationLogger.Error += classApplicationLogger_Error;
            classApplicationLogger.Message += m_messages.ShowMessage;
            classApplicationLogger.Message += classApplicationLogger_Message;

            // Method Editor
            m_methodEditor = new formMethodEditor();
            m_sampleProgress = new formColumnSampleProgress();
            m_sampleManager.SampleManagerViewModel.Stop += m_sampleManager_Stop;


            // Get the most recently used separation type
            classLCMSSettings.SetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE, classSQLiteTools.GetDefaultSeparationType());

            // Initialize the hardware
            var failedDeviceFlag = false;
            var failedCount = 0;
            classDeviceManager.Manager.InitialzingDevice +=
                Manager_InitialzingDevice;
            formFailedDevicesDisplay display = null;
            if (classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INITIALIZEHARDWAREONSTARTUP, false))
            {
                classApplicationLogger.LogMessage(0, "Initializing hardware.");
                var failedDevices = classDeviceManager.Manager.InitializeDevices();
                if (failedDevices != null && failedDevices.Count > 0)
                {
                    failedDeviceFlag = true;
                    failedCount = failedDevices.Count;
                    display = new formFailedDevicesDisplay(failedDevices)
                    {
                        StartPosition = FormStartPosition.CenterParent,
                        Icon = Icon
                    };
                }
            }

            // simulator stuff, don't add the simulator to the system if not in emulation mode.
            if (classLCMSSettings.GetParameter(classLCMSSettings.PARAM_EMULATIONENABLED, false))
            {
                //add simulator button to main form and add simulator forms.
                m_simCombined = new formSimulatorCombined();
                m_simCombined.Tack += m_simCombined_Tack;
                m_simConfig = new formSimConfiguration();
                m_simControlsAndCharts = new formSimulatorControlsAndCharts();
                AddForm(m_simCombined);
                AddForm(m_simConfig);
                AddForm(m_simControlsAndCharts);
            }
            else
            {
                toolStripButtonSimulate.Visible = false;
                toolStripSeparatorSimulator.Visible = false;
            }

            // Load the methods from the LC-Methods folder.
            classApplicationLogger.LogMessage(0, "Reading User Methods.");
            var userMethodErrors =
                classLCMethodManager.Manager.LoadMethods(
                    Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH),
                        classLCMethodFactory.CONST_LC_METHOD_FOLDER));

            if (userMethodErrors.Count > 0)
            {
                var failedMethods = new formFailedMethodLoadDisplay(userMethodErrors)
                {
                    Icon = Icon,
                    StartPosition = FormStartPosition.CenterScreen
                };
                failedMethods.ShowDialog();
            }

            AddForm(m_fluidicsDesign);
            m_about = new formAbout
            {
                SoftwareCopyright = Program.SOFTWARE_COPYRIGHT,
                SoftwareDevelopers = Program.SOFTWARE_DEVELOPERS
            };

            AddForm(m_displays);
            AddForm(m_about);
            AddForm(m_messages);
            AddForm(m_notifications);
            AddForm(m_methodEditor);
            AddForm(m_sampleProgress);
            AddForm(m_systemConfiguration);
            AddForm(m_sampleManager);

            classApplicationLogger.LogMessage(0, "Loading Sample Queue...");
            Application.DoEvents();
            //
            // Tell the sample queue to load samples from cache after everything is loaded.
            //
            try
            {
                m_sampleQueue.RetrieveQueueFromCache();
                m_sampleQueue.IsDirty = false;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message, ex);
            }

            if (failedDeviceFlag)
            {
                classApplicationLogger.LogMessage(0,
                    string.Format("System Unsure.  {0} devices failed to initialize.", failedCount));
            }
            else if (userMethodErrors.Count > 0)
            {
                classApplicationLogger.LogMessage(0, "All devices initialized.  Errors were found in the LC-Methods");
            }
            else
            {
                classApplicationLogger.LogMessage(0, "System Ready.");
            }

            display?.ShowDialog();
            m_sampleProgress.PreviewAvailable += m_sampleManager.PreviewAvailable;
            m_notifications.LoadNotificationFile();
        }

        void m_systemConfiguration_ColumnNameChanged(object sender, EventArgs e)
        {
            m_sampleQueue.UpdateAllSamples();
        }

        #region Notification System.

        /// <summary>
        /// Handles events from the notification system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_notifications_ActionRequired(object sender, NotificationSetting e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<NotificationSetting>(NotifyHandler), sender, e);
            }
            else
            {
                NotifyHandler(sender, e);
            }
        }

        private void NotifyHandler(object sender, NotificationSetting e)
        {
            switch (e.Action)
            {
                case enumDeviceNotificationAction.Stop:
                    classApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    m_scheduler.Stop();
                    //m_sampleQueue.StopRunningQueue();
                    classApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case enumDeviceNotificationAction.Shutdown:
                    classApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    //m_sampleQueue.StopRunningQueue();
                    m_scheduler.Stop();
                    classApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case enumDeviceNotificationAction.StopAndRunMethodNow:
                    // Runs the method if it is not null.
                    if (e.Method != null)
                    {
                        classApplicationLogger.LogError(0,
                            string.Format(
                                "The notification system is stopping all current runs and running the special method {1} because of the setting {0}",
                                e.Name, e.Method.Name));
                        var columnID = e.Method.Column;

                        //m_sampleQueue.StopRunningQueue();
                        m_scheduler.Stop();
                        var dummySample = new classSampleData {
                            DmsData = {
                                DatasetName = string.Format("NotificationAction--{0}", e.Method.Name)
                            }
                        };

                        if (columnID >= 0)
                        {
                            dummySample.ColumnData = classCartConfiguration.Columns[columnID];
                        }

                        dummySample.LCMethod = e.Method;
                        m_sampleQueue.RunNext(dummySample);


                        classApplicationLogger.LogError(0,
                            string.Format(
                                "Queue restarted with special method {1} because of the setting {0}", e.Name,
                                e.Method.Name));
                    }
                    break;
                case enumDeviceNotificationAction.RunMethodNext:
                    if (e.Method != null)
                    {
                        var columnID = e.Method.Column;

                        classApplicationLogger.LogError(0,
                            string.Format(
                                "The notification system is queuing the special method {1} because of the setting {0}.",
                                e.Name, e.Method.Name));

                        var stupidSample = new classSampleData();
                        if (columnID >= 0)
                        {
                            stupidSample.ColumnData = classCartConfiguration.Columns[columnID];
                        }
                        stupidSample.LCMethod = e.Method;
                        m_sampleQueue.RunNext(stupidSample);

                        classApplicationLogger.LogError(0,
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

        void m_messages_ErrorPresent(object sender, EventArgs e)
        {
            mtoolButton_showMessages.Image = Resources.StatusMessagesError;
        }

        void m_messages_ErrorCleared(object sender, EventArgs e)
        {
            mtoolButton_showMessages.Image = Resources.StatusMessages;
        }

        void Manager_DevicesInitialized(object sender, EventArgs e)
        {
            classApplicationLogger.LogMessage(0, "Device initialization complete.");
        }

        void m_simCombined_Tack(object sender, TackEventArgs e)
        {
            if (sender == SimControlsAndChartsControl.GetInstance)
            {
                //controls and charts untacked
                if (!e.Tacked && SimConfigControl.GetInstance.Tacked)
                {
                    //show config in main window
                    m_simConfig.MdiParent = this;
                    m_simConfig.Hide();
                    m_simConfig.BringToFront();
                    m_simConfig.Show();
                    m_simControlsAndCharts.Show();
                }
                // charts and controls is tacked and config is already tacked.
                else if (e.Tacked && SimConfigControl.GetInstance.Tacked)
                {
                    m_simControlsAndCharts.MdiParent = this;
                    m_simCombined.Hide();
                    m_simCombined.Show();
                    m_simCombined.BringToFront();
                }
                //config was untacked, and untack of controls and charts requested
                else if (!e.Tacked && !SimConfigControl.GetInstance.Tacked)
                {
                    //tack config to main window
                    SimConfigControl.GetInstance.TackOnRequest();
                    m_simConfig.MdiParent = this;
                    m_simConfig.Hide();
                    m_simConfig.Show();
                    m_simControlsAndCharts.Show();
                    m_simConfig.BringToFront();
                }
            }
            else if (sender == SimConfigControl.GetInstance)
            {
                // config is untacked and charts and controls is already tacked
                if (!e.Tacked && SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    //show charts and controls in main window
                    m_simControlsAndCharts.MdiParent = this;
                    m_simControlsAndCharts.Hide();
                    m_simControlsAndCharts.BringToFront();
                    m_simControlsAndCharts.Show();
                    m_simConfig.Show();
                }
                // config is tacked and charts and controls is already tacked
                else if (e.Tacked && SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    m_simConfig.MdiParent = this;
                    m_simCombined.Show();
                    m_simCombined.BringToFront();
                }
                // config is untacked and charts and controls is already untacked
                else if (!e.Tacked && !SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    // tack carts and controls to main window and show it
                    SimControlsAndChartsControl.GetInstance.TackOnRequest();
                    m_simControlsAndCharts.MdiParent = this;
                    m_simControlsAndCharts.Hide();
                    m_simControlsAndCharts.Show();
                    m_simConfig.Show();
                    m_simControlsAndCharts.BringToFront();
                }
            }
        }

        #endregion

        #region Device Manager Events

        /// <summary>
        ///
        /// </summary>
        /// <param name="device"></param>
        void RegisterDeviceEventHandlers(IDevice device)
        {
            var sampler = device as IAutoSampler;
            if (sampler != null)
            {
                sampler.TrayNames += m_sampleManager.AutoSamplerTrayList;
            }

            var network = device as INetworkStart;
            if (network != null)
            {
                network.MethodNames +=
                    m_sampleManager.InstrumentMethodList;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="device"></param>
        void DeRegisterDeviceEventHandlers(IDevice device)
        {
            var type = device.GetType();
            if (type.IsAssignableFrom(typeof (IAutoSampler)))
            {
                var sampler = device as IAutoSampler;
                if (sampler != null)
                    sampler.TrayNames -= m_sampleManager.AutoSamplerTrayList;
            }

            if (type.IsAssignableFrom(typeof (INetworkStart)))
            {
                var network = device as INetworkStart;
                if (network != null)
                    network.MethodNames -= m_sampleManager.InstrumentMethodList;
            }
        }

        /// <summary>
        /// Handles when a device is renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRenamed(object sender, IDevice device)
        {
            classApplicationLogger.LogMessage(0, "Renamed a device to " + device.Name);
        }

        /// <summary>
        /// Handles when a device is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            DeRegisterDeviceEventHandlers(device);
            classApplicationLogger.LogMessage(0, "Removed device " + device.Name);
        }

        /// <summary>
        /// Handles when a device is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            RegisterDeviceEventHandlers(device);
            classApplicationLogger.LogMessage(0, "Added device " + device.Name);
        }

        /// <summary>
        /// Updates status window when device is being initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Manager_InitialzingDevice(object sender, classDeviceManagerStatusArgs e)
        {
            classApplicationLogger.LogMessage(-1, e.Message);
        }

        #region Methods

        /// <summary>
        /// Sets up the given form to be added as a mdi-child
        /// </summary>
        /// <param name="form">Form to add</param>
        void AddForm(Form form)
        {
            form.BringToFront();
            form.Icon = Icon;
            form.MdiParent = this;
            form.ControlBox = false;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        public void SetStatusMessage(int level, string message)
        {
            toolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Displays the suggested string on the status bar.
        /// </summary>
        /// <param name="message">Message to display to the user.</param>
        public void SetStatusMessage(string message)
        {
            toolStripStatusLabel.Text = message;
        }

        #endregion

        public delegate void statusDelegate(int messageLevel, string message);

        /// <summary>
        /// Handles displaying a message when an object notifies LCMS at level 0.
        /// </summary>
        /// <param name="messageLevel">Message filter.</param>
        /// <param name="args">Data associated with messages.</param>
        void classApplicationLogger_Message(int messageLevel, classMessageLoggerArgs args)
        {
            if (args != null && messageLevel <= classApplicationLogger.CONST_STATUS_LEVEL_USER)
            {
                if (InvokeRequired)
                {
                    if (!IsDisposed)
                    {
                        try
                        {
                            Invoke(new statusDelegate(SetStatusMessage), messageLevel, args.Message);
                        }
                        catch
                        {
                            // Ignore exceptions here
                        }
                    }
                }
                else
                {
                    SetStatusMessage(args.Message);
                }
            }
        }

        /// <summary>
        /// Handles displaying an error mesasge when notified LCMS.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        void classApplicationLogger_Error(int errorLevel, classErrorLoggerArgs args)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed)
                {
                    try
                    {
                        Invoke(new statusDelegate(SetStatusMessage), errorLevel, args.Message);
                    }
                    catch
                    {
                        // Ignore exceptions here
                    }
                }
            }
            else
            {
                SetStatusMessage(args.Message);
            }
        }

        #endregion

        #region Scheduler and Related Events

        /// <summary>
        /// Handles the stop event to stop any sample runs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_sampleManager_Stop(object sender, EventArgs e)
        {
            //
            // Tell the scheduler to stop running!
            //
            m_scheduler.Stop();
        }

        /// <summary>
        /// Updates the status windows and log files with messages.
        /// </summary>
        void UpdateSampleProgress(object sender, classSampleProgressEventArgs args)
        {
            var message = "";
            var sample = args.Sample;
            classLCEvent lcEvent = null;
            classLCMethod lcMethod;
            var isError = false;

            //
            // Construct the message to display
            //
            switch (args.ProgressType)
            {
                case enumSampleProgress.RunningNextEvent:
                    lcMethod = sample.LCMethod;
                    lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    message = string.Format("Sample Event: Column={0}: ColumnID={1}: Device={2}.{3}: Sample={4}",
                        sample.ColumnData.ID + 1,
                        sample.ColumnData.Name,
                        lcEvent.Device.Name,
                        lcEvent.Name,
                        sample.DmsData.DatasetName);
                    break;
                case enumSampleProgress.Error:
                    message = "";
                    lcMethod = sample.LCMethod;
                    var eventNumber = lcMethod.CurrentEventNumber;
                    if (eventNumber < lcMethod.Events.Count && eventNumber > -1)
                    {
                        lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    }
                    m_sampleProgress.UpdateError(sample, lcEvent);
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, args.Message,
                        null, args.Sample);
                    isError = true;
                    break;
                case enumSampleProgress.Stopped:
                    lcMethod = sample.LCMethod;
                    lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    message = string.Empty;
                    m_sampleProgress.UpdateError(sample, lcEvent);
                    isError = true;
                    break;
                case enumSampleProgress.Complete:
                    var configImage = m_fluidicsDesign.GetImage();
                    string docPath;
                    if (sample == null)
                    {
                        break;
                    }
                    try
                    {
                        docPath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_PDFPATH);
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
                                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                                    "Invalid PDF document folder defined in LcmsNet.exe.config (drive not found): " +
                                    docPath);
                                break;
                            }
                            pdfFolder.Create();
                        }

                        message = string.Empty;
                        if (bool.Parse(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYMETHODFOLDERS)))
                        {
                            classMethodFileTools.MoveLocalMethodFiles();
                        }
                        var filePath =
                            FileUtilities.UniqifyFileName(Path.Combine(docPath, sample.DmsData.DatasetName), ".pdf");
                        m_pdfGen.WritePDF(filePath, sample.DmsData.DatasetName, sample,
                            classCartConfiguration.NumberOfEnabledColumns.ToString(), classCartConfiguration.Columns,
                            classDeviceManager.Manager.Devices, configImage);
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                            "PDF Generation Error for " + sample.DmsData.DatasetName + " " + ex.Message, ex, sample);
                    }
                    break;
                case enumSampleProgress.Started:
                    message = string.Empty;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                classApplicationLogger.LogMessage(0, message, sample);
            }


            // Update visual progress. -- Errors are already updated above.
            if (!isError && sample != null)
            {
                m_sampleProgress.UpdateSample(sample);
            }
        }

        /// <summary>
        /// Handles updating the status window when the next event starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void Scheduler_SampleProgress(object sender, classSampleProgressEventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateSampleProgress(UpdateSampleProgress), null, args);
            }
            else
            {
                UpdateSampleProgress(null, args);
            }
        }

        /// <summary>
        /// Handles displaying the error message on the status bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sample"></param>
        /// <param name="errorMessage"></param>
        void Scheduler_Error(object sender, classSampleData sample, string errorMessage)
        {
            var args = new classSampleProgressEventArgs(errorMessage,
                sample,
                enumSampleProgress.Error);
            if (InvokeRequired)
            {
                BeginInvoke(new DelegateSampleProgress(UpdateSampleProgress), null, args);
            }
            else
            {
                UpdateSampleProgress(sender, args);
            }
        }

        #endregion

        #region Show Methods

        /// <summary>
        /// Displays the cart configuration.
        /// </summary>
        private void ShowCartConfiguration()
        {
            m_systemConfiguration.Show();
            m_systemConfiguration.BringToFront();
        }

        /// <summary>
        /// Displays the messages window.
        /// </summary>
        private void ShowMessagesWindow()
        {
            m_messages.Show();
            m_messages.BringToFront();
        }

        /// <summary>
        /// Displays the sample queue window.
        /// </summary>
        private void ShowSampleQueue()
        {
            m_sampleManager.Show();
            m_sampleManager.BringToFront();
            m_sampleManager.RestoreUserUIState();
        }

        /// <summary>
        /// Displays the method editor
        /// </summary>
        private void ShowMethodEditor()
        {
            m_methodEditor.Show();
            m_methodEditor.BringToFront();
        }

        /// <summary>
        /// Displays the sample progress window.
        /// </summary>
        private void ShowSampleProgress()
        {
            m_sampleProgress.Show();
            m_sampleProgress.BringToFront();
        }

        private void ShowSimulator()
        {
            if (SimControlsAndChartsControl.GetInstance.Tacked && !SimConfigControl.GetInstance.Tacked)
            {
                m_simControlsAndCharts.BringToFront();
                m_simControlsAndCharts.Show();
            }
            else if (!SimControlsAndChartsControl.GetInstance.Tacked && SimConfigControl.GetInstance.Tacked)
            {
                m_simConfig.BringToFront();
                m_simConfig.Show();
            }
            else
            {
                m_simCombined.Show();
                m_simCombined.BringToFront();
            }
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Handles closing the program when the Exit button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Displays the messages and error window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtoolButton_showMessages_Click(object sender, EventArgs e)
        {
            ShowMessagesWindow();
        }

        /// <summary>
        /// Displays the method editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_MethodEditor_Click(object sender, EventArgs e)
        {
            ShowMethodEditor();
        }

        /// <summary>
        /// Displays the sample queue window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtoolButton_showSampleQueue_Click(object sender, EventArgs e)
        {
            ShowSampleQueue();
        }

        private void mbutton_sampleProgress_Click(object sender, EventArgs e)
        {
            ShowSampleProgress();
        }

        private void formMDImain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //string msg = "Hey, man. Some dude said to close this here durned program. " + Environment.NewLine +
            //             "Y'all should check ta be shore no samples is runnin' afore closin' things up." + Environment.NewLine +
            //             Environment.NewLine + "Is you'uns sure ya wants ta do this?";
            var msg = "Application shutdown requested. If samples are running, data may be lost" +
                         Environment.NewLine +
                         Environment.NewLine + "Are you sure you want to shut down?";
            var result = MessageBox.Show(msg, "Closing Application", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (result != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void mbutton_cartButton_Click(object sender, EventArgs e)
        {
            ShowCartConfiguration();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            m_about.BringToFront();
            m_about.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            m_displays.BringToFront();
            m_displays.Show();
        }

        void m_displays_UnTack(object sender, EventArgs e)
        {
            m_displays.MdiParent = null;
            m_displays.WindowState = FormWindowState.Normal;
        }

        void m_displays_Tack(object sender, EventArgs e)
        {
            m_displays.MdiParent = this;
            m_displays.WindowState = FormWindowState.Maximized;
        }

        private void toolButton_notificationSystem_Click(object sender, EventArgs e)
        {
            m_notifications.BringToFront();
            m_notifications.Show();
        }

        private void mbutton_reportError_Click(object sender, EventArgs e)
        {
            var manager = classLCMethodManager.Manager;
            var logPath = classFileLogging.LogPath;
            var forms = new List<Form>();

            forms.AddRange(new Form[]
            {
                m_about,
                m_displays,
                m_messages,
                m_methodEditor,
                m_notifications,
                m_sampleProgress,
                m_sampleManager
            });


            using (var report
                = new formCreateErrorReport(manager,
                    logPath,
                    forms
                    ))
            {
                report.ShowDialog();
            }
        }

        private void toolStripButtonFludics_Click(object sender, EventArgs e)
        {
            m_fluidicsDesign.BringToFront();
            m_fluidicsDesign.Show();
        }


        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            m_about.BringToFront();
            m_about.Show();
        }

        private void toolStripButtonSimulate_Click(object sender, EventArgs e)
        {
            ShowSimulator();
        }

        #endregion
    }
}