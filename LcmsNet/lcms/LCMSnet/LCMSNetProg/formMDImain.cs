using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNet.Configuration;
using LcmsNet.Devices;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNet.Notification;
using LcmsNet.Notification.Forms;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetSQLiteTools;
using LcmsNet.Devices.Fluidics;
using LcmsNetSDK;
using PDFGenerator;
using System.Drawing;
using System.IO;
using LcmsNet.IO;
using LcmsNet.Simulator;

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

        #region Members
          private FluidicsDesign mform_fluidicsDesign;
        /// <summary>
        /// Method Scheduler and execution engine.
        /// </summary>
        private classLCMethodScheduler mobj_scheduler;
        /// <summary>
        /// Form that manages views of the sample queue class for handling sample
        /// ordering and running.
        /// </summary>
        private formSampleManager mform_sampleManager;
        /// <summary>
        /// Object that manages operation of the samples.
        /// </summary>
        private classSampleQueue mobj_sampleQueue;
        /// <summary>
        /// Form that displays the configuration of the columns.
        /// </summary>
        private formSystemConfiguration mform_systemConfiguration;        
        /// <summary>
        /// Form that displays the messages from the different parts of the program.
        /// </summary>
        private formMessageWindow mform_messages;
        /// <summary>
        /// Method editor form to construct new LC separations.
        /// </summary>
        private formMethodEditor mform_methodEditor;
        /// <summary>
        /// Displays progress of samples along each column.
        /// </summary>
        private formColumnSampleProgress mform_sampleProgress;		
		/// <summary>
        /// Notifications form.
        /// </summary>
        private formNotificationSystem mform_notifications;
        /// <summary>
        /// About box.
        /// </summary>
        private formAbout mform_about;
        /// <summary>
        /// Display form for the pumps.
        /// </summary>
        private LcmsNetDataClasses.Devices.Pumps.formPumpDisplays mform_displays;
        /// <summary>
        /// Class for logging to the log files and other listeners.  Wraps the application logger static methods.
        /// </summary>
        private classApplicationLogger mobj_logger;
        /// <summary>
        /// default PDF generator, used to generate a pdf of information after a sample run.
        /// </summary>
        private IPDF m_pdfGen = new PDFGen();
        private formSimulatorCombined mform_simCombined;
        private formSimConfiguration mform_simConfig;
        private formSimulatorControlsAndCharts mform_simControlsAndCharts;

        #endregion

        /// <summary>
        /// For updating the status message.
        /// </summary>
        /// <param name="message">Message to display.</param>
        private delegate void DelegateUpdateMessage(int level, string message);
        
        /// <summary>
        /// Default constructor.
        /// </summary>        
        public formMDImain()
        {
            InitializeComponent();
            Initialize();
        }

        #region Initialization
		/// <summary>
        /// Initializes all the forms and objects.and I ass
        // </summary>
        private void Initialize()
        {            
            System.Threading.Thread.CurrentThread.Name = "Main Thread";
            mobj_logger = new classApplicationLogger();
			this.Text  = "LcmsNet Version: " + Application.ProductVersion;
            this.Text += " Cart - " + classLCMSSettings.GetParameter("CartName");
            string emulation = classLCMSSettings.GetParameter("EmulationEnabled");
            if (emulation != null)
            {
                bool isEmulated = Convert.ToBoolean(emulation);
                if (isEmulated)
                {
                    this.Text += " [EMULATED] ";
                }                
            }

            foreach (classColumnData column in classCartConfiguration.Columns)
            {
                column.NameChanged += new classColumnData.DelegateNameChanged(column_NameChanged);
            }

				
			mform_systemConfiguration               = new formSystemConfiguration();      
            mform_systemConfiguration.ColumnNames   = classSQLiteTools.GetColumnList(false);
			mform_systemConfiguration.Users         = classSQLiteTools.GetUserList(false);    
            mform_systemConfiguration.ColumnNameChanged += new EventHandler(mform_systemConfiguration_ColumnNameChanged);            
            classSQLiteTools.GetSepTypeList(false);
            
            // Fludics Design display
            mform_fluidicsDesign = new FluidicsDesign();
            mform_fluidicsDesign.Icon = this.Icon;
            mform_fluidicsDesign.Dock = DockStyle.Fill;

            // Notification System
            mform_notifications = new formNotificationSystem(classDeviceManager.Manager);
            mform_notifications.ActionRequired += new EventHandler<NotificationSetting>(mform_notifications_ActionRequired);

            
            // Construct the sample queue object that holds and manages sample data ordering            
            mobj_sampleQueue                         = new classSampleQueue();
            mform_sampleManager                      = new formSampleManager(mobj_sampleQueue);

            classDeviceManager.Manager.DeviceAdded   += new DelegateDeviceUpdated(Manager_DeviceAdded);
            classDeviceManager.Manager.DeviceRemoved += new DelegateDeviceUpdated(Manager_DeviceRemoved);
            classDeviceManager.Manager.DeviceRenamed += new DelegateDeviceUpdated(Manager_DeviceRenamed);
            classDeviceManager.Manager.DevicesInitialized += new EventHandler(Manager_DevicesInitialized);

            // Displays the pump data.
            mform_displays           = new LcmsNetDataClasses.Devices.Pumps.formPumpDisplays();
            mform_displays.Tack     += new EventHandler(mform_displays_Tack);
            mform_displays.UnTack   += new EventHandler(mform_displays_UnTack);
            mform_displays.Icon      = Icon;
            mform_displays.IsTacked  = true;

            classApplicationLogger.LogMessage(0, "Loading the hardware configuration.");
            mform_fluidicsDesign.LoadConfiguration();

            
            // Create and initialize the scheduler that handles executing LC-Methods (separations, e.g. experiments)
            mobj_scheduler                              = new classLCMethodScheduler(mobj_sampleQueue);
            mobj_scheduler.Logger                       = mobj_logger;
            mobj_scheduler.SchedulerError               += new DelegateError(Scheduler_Error);
            mobj_scheduler.SampleProgress               += new DelegateSampleProgress(Scheduler_SampleProgress);
            mobj_scheduler.Initialize();
                        
            /// 
            /// Logging and messaging
            /// 
            mform_messages      = new formMessageWindow();
            mform_messages.ErrorCleared     += new EventHandler(mform_messages_ErrorCleared);
            mform_messages.ErrorPresent     += new EventHandler(mform_messages_ErrorPresent);
            classApplicationLogger.Error    += new classApplicationLogger.DelegateErrorHandler(mform_messages.ShowErrors);
            classApplicationLogger.Error    += new classApplicationLogger.DelegateErrorHandler(classApplicationLogger_Error);
            classApplicationLogger.Message  += new classApplicationLogger.DelegateMessageHandler(mform_messages.ShowMessage);
            classApplicationLogger.Message  += new classApplicationLogger.DelegateMessageHandler(classApplicationLogger_Message);

			// Method Editor
            mform_methodEditor           = new formMethodEditor();            
            mform_sampleProgress         = new formColumnSampleProgress();            
            mform_sampleManager.Stop    += new EventHandler(mform_sampleManager_Stop);


            // Get the most recently used separation type
			classLCMSSettings.SetParameter("SeparationType", classSQLiteTools.GetDefaultSeparationType());

            // Initialize the hardware
            bool failedDeviceFlag   = false;
            int  failedCount        = 0;
            classDeviceManager.Manager.InitialzingDevice += new EventHandler<classDeviceManagerStatusArgs>(Manager_InitialzingDevice);
            formFailedDevicesDisplay display              = null;    
            if (Convert.ToBoolean(classLCMSSettings.GetParameter("InitializeHardwareOnStartup")) == true)
            {
                classApplicationLogger.LogMessage(0, "Initializing hardware.");
                List<classDeviceErrorEventArgs> failedDevices = classDeviceManager.Manager.InitializeDevices();
                if (failedDevices != null && failedDevices.Count > 0)
                {
                    failedDeviceFlag        = true;
                    failedCount             = failedDevices.Count;
                    display                 = new formFailedDevicesDisplay(failedDevices);
                    display.StartPosition   = FormStartPosition.CenterParent;
                    display.Icon            = this.Icon;
                }
            }         

            // simulator stuff, don't add the simulator to the system if not in emulation mode.
            if (Convert.ToBoolean(classLCMSSettings.GetParameter("EmulationEnabled")) == true)
            {
                //add simulator button to main form and add simulator forms.
                mform_simCombined = new formSimulatorCombined();
                mform_simCombined.Tack += mform_simCombined_Tack;
                mform_simConfig = new formSimConfiguration();
                mform_simControlsAndCharts = new formSimulatorControlsAndCharts();
                AddForm(mform_simCombined);
                AddForm(mform_simConfig);
                AddForm(mform_simControlsAndCharts);
            }
            else
            {
                toolStripButtonSimulate.Visible = false;
                toolStripSeparatorSimulator.Visible = false;
            }

            // Load the methods from the LC-Methods folder.
            classApplicationLogger.LogMessage(0, "Reading User Methods.");
            Dictionary<string, List<Exception>> userMethodErrors = classLCMethodManager.Manager.LoadMethods(System.IO.Path.Combine(classLCMSSettings.GetParameter("ApplicationPath"),                                                  classLCMethodFactory.CONST_LC_METHOD_FOLDER));

            if (userMethodErrors.Count > 0)
            {
                formFailedMethodLoadDisplay failedMethods   = new formFailedMethodLoadDisplay(userMethodErrors);
                failedMethods.Icon                          = Icon;
                failedMethods.StartPosition                 = FormStartPosition.CenterScreen;
                failedMethods.ShowDialog();
            }            

            AddForm(mform_fluidicsDesign);
            mform_about = new formAbout();
            AddForm(mform_displays);
            AddForm(mform_about);            
            AddForm(mform_messages);
            AddForm(mform_notifications);
            AddForm(mform_methodEditor);
            AddForm(mform_sampleProgress);
            AddForm(mform_systemConfiguration);
            AddForm(mform_sampleManager);

            classApplicationLogger.LogMessage(0, "Loading Sample Queue...");
            Application.DoEvents();
            ///           
            /// Tell the sample queue to load samples from cache after everything is loaded.
            /// 
            mobj_sampleQueue.RetrieveQueueFromCache();
            mobj_sampleQueue.IsDirty = false;

            if (failedDeviceFlag == true)
            {
                classApplicationLogger.LogMessage(0, string.Format("System Unsure.  {0} devices failed to initialize.", failedCount));
            }
            else if (userMethodErrors.Count > 0)
            {
                classApplicationLogger.LogMessage(0, "All devices initialized.  Errors were found in the LC-Methods");
            }
            else
            {
                classApplicationLogger.LogMessage(0, "System Ready.");
            }

            if (display != null)
            {
                display.ShowDialog();
            }
            mform_sampleProgress.PreviewAvailable += new EventHandler<SampleProgressPreviewArgs>(mform_sampleManager.PreviewAvailable);            
            mform_notifications.LoadNotificationFile();            
        }

        void mform_systemConfiguration_ColumnNameChanged(object sender, EventArgs e)
        {
            mobj_sampleQueue.UpdateAllSamples();
        }

        #region Notification System.
        /// <summary>
        /// Handles events from the notification system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mform_notifications_ActionRequired(object sender, NotificationSetting e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<NotificationSetting>(NotifyHandler), new object[] { sender, e });
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
                    mobj_scheduler.Stop();
                    //mobj_sampleQueue.StopRunningQueue();
                    classApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case enumDeviceNotificationAction.Shutdown:
                    classApplicationLogger.LogError(0, "The notification system is shutting down the queue");
                    //mobj_sampleQueue.StopRunningQueue();
                    mobj_scheduler.Stop();
                    classApplicationLogger.LogError(0, string.Format("Queue was shutdown by {0} ", e.Name));
                    break;
                case enumDeviceNotificationAction.StopAndRunMethodNow:
                    // Runs the method if it is not null.
                    if (e.Method != null)
                    {

                        classApplicationLogger.LogError(0, 
                            string.Format(
                                "The notification system is stopping all current runs and running the special method {1} because of the setting {0}", e.Name, e.Method.Name));
                        int columnID = e.Method.Column;
                        
                        //mobj_sampleQueue.StopRunningQueue();
                        mobj_scheduler.Stop();
                        classSampleData stupidSample    = new classSampleData();
                        stupidSample.DmsData.DatasetName = string.Format("NotificationAction--{0}", e.Method.Name);
                        if (columnID >= 0)
                        {
                            stupidSample.ColumnData     = classCartConfiguration.Columns[columnID];
                        }
                        stupidSample.LCMethod           = e.Method;
                        mobj_sampleQueue.RunNext(stupidSample);                        
                        

                        classApplicationLogger.LogError(0, 
                            string.Format(
                                "Queue restarted with special method {1} because of the setting {0}", e.Name, e.Method.Name));
                    }
                    break;
                case enumDeviceNotificationAction.RunMethodNext:
                    if (e.Method != null)
                    {

                        int columnID                 = e.Method.Column;

                        classApplicationLogger.LogError(0, 
                            string.Format(
                                "The notification system is queuing the special method {1} because of the setting {0}.", 
                                    e.Name, e.Method.Name));                                        

                        classSampleData stupidSample = new classSampleData();
                        if (columnID >= 0)
                        {
                            stupidSample.ColumnData  = classCartConfiguration.Columns[columnID];
                        }
                        stupidSample.LCMethod        = e.Method;
                        mobj_sampleQueue.RunNext(stupidSample);

                        classApplicationLogger.LogError(0, 
                            string.Format(
                                "The special method {2} was queued to run next on the column {0} because of the notification system event {1} ",
                                        columnID,
                                        e.Name,
                                        e.Method.Name));

                        if (!mobj_sampleQueue.IsRunning)
                        {
                            mobj_sampleQueue.StartSamples();
                        }
                    }
                    break;
                        
            }
        }
        #endregion 

        void mform_messages_ErrorPresent(object sender, EventArgs e)
        {
            mtoolButton_showMessages.Image = global::LcmsNet.Properties.Resources.StatusMessagesError;
        }

        void mform_messages_ErrorCleared(object sender, EventArgs e)
        {

            mtoolButton_showMessages.Image = global::LcmsNet.Properties.Resources.StatusMessages;
        }

        void Manager_DevicesInitialized(object sender, EventArgs e)
        {
            classApplicationLogger.LogMessage(0, "Device initialization complete.");
        }

        void mform_simCombined_Tack(object sender, TackEventArgs e)
        {
            if (sender == SimControlsAndChartsControl.GetInstance)
            {
                //controls and charts untacked
                if (!e.Tacked && SimConfigControl.GetInstance.Tacked)
                {
                    //show config in main window
                    mform_simConfig.MdiParent = this;
                    mform_simConfig.Hide();
                    mform_simConfig.BringToFront();
                    mform_simConfig.Show();
                    mform_simControlsAndCharts.Show();
                }
                // charts and controls is tacked and config is already tacked.
                else if (e.Tacked && SimConfigControl.GetInstance.Tacked)
                {
                    mform_simControlsAndCharts.MdiParent = this;
                    mform_simCombined.Hide();
                    mform_simCombined.Show();
                    mform_simCombined.BringToFront();
                }
                //config was untacked, and untack of controls and charts requested
                else if (!e.Tacked && !SimConfigControl.GetInstance.Tacked)
                {
                    //tack config to main window
                    SimConfigControl.GetInstance.TackOnRequest();
                    mform_simConfig.MdiParent = this;
                    mform_simConfig.Hide();
                    mform_simConfig.Show();
                    mform_simControlsAndCharts.Show();
                    mform_simConfig.BringToFront();
                }

            }
            else if (sender == SimConfigControl.GetInstance)
            {
                // config is untacked and charts and controls is already tacked
                if (!e.Tacked && SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    //show charts and controls in main window
                    mform_simControlsAndCharts.MdiParent = this;
                    mform_simControlsAndCharts.Hide();
                    mform_simControlsAndCharts.BringToFront();
                    mform_simControlsAndCharts.Show();
                    mform_simConfig.Show();
                }
                // config is tacked and charts and controls is already tacked
                else if (e.Tacked && SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    mform_simConfig.MdiParent = this;
                    mform_simCombined.Show();
                    mform_simCombined.BringToFront();
                }
                // config is untacked and charts and controls is already untacked
                else if (!e.Tacked && !SimControlsAndChartsControl.GetInstance.Tacked)
                {
                    // tack carts and controls to main window and show it
                    SimControlsAndChartsControl.GetInstance.TackOnRequest();
                    mform_simControlsAndCharts.MdiParent = this;
                    mform_simControlsAndCharts.Hide();
                    mform_simControlsAndCharts.Show();
                    mform_simConfig.Show();
                    mform_simControlsAndCharts.BringToFront();

                }

            }

        }

        #endregion

		#region "Column event handlers"
        void column_NameChanged(object sender, string name, string oldName)
        {
            classColumnData column = sender as classColumnData;
            if (column != null)
            {
                classLCMSSettings.SetParameter("ColumnName" + column.ID, column.Name);
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
            IAutoSampler sampler = device as IAutoSampler;
            if (sampler != null)
            {                
                sampler.TrayNames += new EventHandler<classAutoSampleEventArgs>(mform_sampleManager.AutoSamplerTrayList);                
            }

            INetworkStart network = device as INetworkStart;
            if (network != null)
            {
                network.MethodNames += new EventHandler<classNetworkStartEventArgs>(mform_sampleManager.InstrumentMethodList);
            }            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        void DeRegisterDeviceEventHandlers(IDevice device)
        {
            Type type = device.GetType();
            if (type.IsAssignableFrom(typeof(IAutoSampler)))
            {
                IAutoSampler sampler = device as IAutoSampler;
                sampler.TrayNames -= mform_sampleManager.AutoSamplerTrayList;
            }

            if (type.IsAssignableFrom(typeof(INetworkStart)))
            {
                INetworkStart network = device as INetworkStart;
                network.MethodNames -= mform_sampleManager.InstrumentMethodList;
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
            form.Icon           = Icon;
            form.MdiParent      = this;
            form.ControlBox     = false;
            form.WindowState    = FormWindowState.Maximized;
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
                if(this.InvokeRequired)
                {                   ;
                    this.Invoke(new statusDelegate(SetStatusMessage), messageLevel, args.Message);
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
            if (this.InvokeRequired)
            {                
                this.Invoke(new statusDelegate(SetStatusMessage), errorLevel, args.Message);
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
        void mform_sampleManager_Stop(object sender, EventArgs e)
        {
            /// 
            /// Tell the scheduler to stop running!
            /// 
            mobj_scheduler.Stop();
        }

        /// <summary>
        /// Updates the status windows and log files with messages.
        /// </summary>
        /// <param name="sample"></param>
        void UpdateSampleProgress(object sender, classSampleProgressEventArgs args)
        {           
            string message          = "";
            classSampleData sample  = args.Sample;
            LcmsNetDataClasses.Method.classLCEvent  lcEvent  = null;
            LcmsNetDataClasses.Method.classLCMethod lcMethod = null;
			bool isError = false;
            
            /// 
            /// Construct the message to display 
            /// 
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
                    int eventNumber     = lcMethod.CurrentEventNumber;                
                    if (eventNumber < lcMethod.Events.Count && eventNumber > -1)
                    {
                        lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];                                
                    }                         
                    mform_sampleProgress.UpdateError(sample, lcEvent);
                    isError = true;
                    break;
                case enumSampleProgress.Stopped:
                    lcMethod = sample.LCMethod;
                    lcEvent = lcMethod.Events[lcMethod.CurrentEventNumber];
                    message = string.Empty;
                    mform_sampleProgress.UpdateError(sample, lcEvent);
                    isError = true;
                    break;
                case enumSampleProgress.Complete:
                    Bitmap configImage = mform_fluidicsDesign.GetImage();
                    string docPath = string.Empty;
                    try
                    {
                        docPath =  classLCMSSettings.GetParameter("PdfPath"); 
                    }
                    catch
                    {
                        // default to this path..
                        docPath = "C:\\pdftest\\";
                    }
                    try
                    {
                        lcMethod = sample.LCMethod;
                        message = string.Empty; 
                        if (bool.Parse(classLCMSSettings.GetParameter("CopyMethodFolders")))
                        {
                            SampleQueue.IO.classMethodFileTools.MoveLocalMethodFiles();
                        }
                        string filePath = FileUtilities.UniqifyFileName(Path.Combine(docPath, sample.DmsData.DatasetName), ".pdf");
                        m_pdfGen.WritePDF(filePath, sample.DmsData.DatasetName, sample, classCartConfiguration.NumberOfEnabledColumns.ToString(), classCartConfiguration.Columns,
                            classDeviceManager.Manager.Devices, configImage);
                        classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "PDF written to: " + filePath);
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER, "PDF Generation Error on" + sample.DmsData.DatasetName + " " + ex.Message);
                    }
                    break;
                case enumSampleProgress.Started:
                    lcMethod = sample.LCMethod;
					message = string.Empty; 
                    break;
            }
            //This is a workaround until this can be addressed properly.
            //The Mainform was creating misleading entries in the log file, and in removing
            //them, created empty messages, so until it can be determined if this whole section
            //is going to be removed/majorly simplified(the more likely option) this is here. --Chris
            if (!message.Equals(string.Empty))
            {
                classApplicationLogger.LogMessage(0, message);
            }


			// Update visual progress. -- Errors are already updated above.			 
			if (!isError)
			{
				mform_sampleProgress.UpdateSample(sample);
			}
        }
        /// <summary>
        /// Handles updating the status window when the next event starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sample"></param>
        /// <param name="eventNumber"></param>
        void Scheduler_SampleProgress(object sender, classSampleProgressEventArgs args)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateSampleProgress(UpdateSampleProgress), new object[] { null, args });
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
        /// <param name="errorMessage"></param>
        void Scheduler_Error(object sender, classSampleData sample, string errorMessage)
        {            
			classSampleProgressEventArgs args = new classSampleProgressEventArgs(errorMessage,
			 																	 sample,
																				 enumSampleProgress.Error);
            if (InvokeRequired == true)
            {                
                BeginInvoke(new DelegateSampleProgress(UpdateSampleProgress), new object [] {null, args});
            }
            else
            {
				UpdateSampleProgress(sender, args);
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
		    mform_fluidicsDesign.SaveConfiguration();
    		              
			// Cache the selected separation type
			classSQLiteTools.SaveSelectedSeparationType(classLCMSSettings.GetParameter("SeparationType"));

			// Shut off the scheduler...            
            try
            {
                mobj_scheduler.Shutdown();

                // Save queue to the cache                 
                mobj_sampleQueue.StopRunningQueue();

                if (mobj_sampleQueue.IsDirty)
                {

                    DialogResult result = MessageBox.Show(string.Format("Do you want to save changes to your queue: {0}",
                                                   classLCMSSettings.GetParameter("CacheFileName")
                                                   ), "Confirm Queue Save", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        mobj_sampleQueue.CacheQueue(true);
                    }
                }
            }
            catch
            {

            }

            //TODO: Check to see if things shutdown.            
        }
        #endregion

        #region Show Methods
        /// <summary>
        /// Displays the cart configuration.
        /// </summary>
        private void ShowCartConfiguration()
        {
            List<string> separationTypes = classSQLiteTools.GetSepTypeList(false);
            mform_systemConfiguration.SeparationTypes = separationTypes;
            if (classLCMSSettings.GetParameter("SeparationType") == "")
            {
	            mform_systemConfiguration.SetSeparationType("none");
            }
            else
            {
	            mform_systemConfiguration.SetSeparationType(classLCMSSettings.GetParameter("SeparationType"));
            }
            mform_systemConfiguration.BringToFront();    
        }
        /// <summary>
        /// Displays the messages window.
        /// </summary>
        private void ShowMessagesWindow()
        {
            mform_messages.Show();
            mform_messages.BringToFront();
        }
        /// <summary>
        /// Displays the sample queue window.
        /// </summary>
        private void ShowSampleQueue()
        {
            mform_sampleManager.Show();
            mform_sampleManager.BringToFront();
        }
        /// <summary>
        /// Displays the method editor 
        /// </summary>
        private void ShowMethodEditor()
        {
          mform_methodEditor.Show();
          mform_methodEditor.BringToFront();
        }
        /// <summary>
        /// Displays the sample progress window.
        /// </summary>
        private void ShowSampleProgress()
        {
            mform_sampleProgress.Show();
            mform_sampleProgress.BringToFront();
        }

        private void ShowSimulator()
        {
            if (SimControlsAndChartsControl.GetInstance.Tacked && !SimConfigControl.GetInstance.Tacked)
            {
                mform_simControlsAndCharts.BringToFront();
                mform_simControlsAndCharts.Show();

            }
            else if (!SimControlsAndChartsControl.GetInstance.Tacked && SimConfigControl.GetInstance.Tacked)
            {
                mform_simConfig.BringToFront();
                mform_simConfig.Show();

            }
            else
            {
                mform_simCombined.Show();
                mform_simCombined.BringToFront();
            }
        }
        #endregion

        #region Fluidics Form Event Handlers
        /// <summary>
        /// Updates status message on main status window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mform_fluidicsDesign_Status(object sender, classDeviceStatusEventArgs e)
        {
            classApplicationLogger.LogMessage(0, e.Message);            
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
          string msg = "Application shutdown requested. If samples are running, data may be lost" + Environment.NewLine +
						        Environment.NewLine + "Are you sure you want to shut down?";
          DialogResult result = MessageBox.Show(msg, "Closing Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
          if (result != DialogResult.OK)
          {
	          e.Cancel = true;
	          return;
          }
        } 
    
        private void mbutton_cartButton_Click(object sender, EventArgs e)
        {
            ShowCartConfiguration();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            mform_about.BringToFront();
            mform_about.Show();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            mform_displays.BringToFront();
            mform_displays.Show();
        }
        void mform_displays_UnTack(object sender, EventArgs e)
        {

            mform_displays.MdiParent = null;
            mform_displays.WindowState = FormWindowState.Normal;

        }
        void mform_displays_Tack(object sender, EventArgs e)
        {
            mform_displays.MdiParent = this;
            mform_displays.WindowState = FormWindowState.Maximized;
        }
        private void toolButton_notificationSystem_Click(object sender, EventArgs e)
        {

            mform_notifications.BringToFront();
            mform_notifications.Show();
        }
        private void mbutton_reportError_Click(object sender, EventArgs e)
        {
            classLCMethodManager manager = classLCMethodManager.Manager;
            string logPath               = classFileLogging.LogPath;
            List<Form> forms             = new List<Form>();

            forms.AddRange(new Form[] { mform_about, 
                                        mform_displays, 
                                        mform_messages,
                                        mform_methodEditor,
                                        mform_notifications,
                                        mform_sampleProgress,
                                        mform_sampleManager});
                                        

            using (Reporting.Forms.formCreateErrorReport report 
                            = new LcmsNet.Reporting.Forms.formCreateErrorReport(manager,
                                logPath, 
                                forms
                                                                                
                                                                                ))
            {
                report.ShowDialog();
            }
        }

        private void toolStripButtonFludics_Click(object sender, EventArgs e)
        {
            mform_fluidicsDesign.BringToFront();
            mform_fluidicsDesign.Show();
        }


        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            mform_about.BringToFront();
            mform_about.Show();
        }

        private void toolStripButtonSimulate_Click(object sender, EventArgs e)
        {
            ShowSimulator();
        }

        #endregion
	 }	
}	
