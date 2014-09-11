using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;
using System.Security.Principal;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNet.Configuration;
using LcmsNetDataClasses.Configuration;
using LcmsNetSDK;
using LcmsNetSQLiteTools;
using LcmsNet.Method;
using LcmsNet.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet
{    
	 /// <summary>
	 /// LCMS.net Program that controls LC-Cart hardware components and interfaces with the PRISM LIMS system (DMS).
	 /// </summary>
	static class Program
	{
		#region "Constants"
			/// <summary>
			/// Default logging levels
			/// </summary>
			private const int CONST_DEFAULT_ERROR_LOG_LEVEL = 5;
			private const int CONST_DEFAULT_MESSAGE_LOG_LEVEL = 5;
		#endregion

		#region "Class variables"
		/// <summary>
			/// Reference to splash screen window.
			/// </summary>
			private static formSplashScreen mform_splashScreen;
		#endregion

		#region Configuration Loading
		/// <summary>
		/// Loads the application settings.
		/// </summary>
		/// <returns>An object that holds the application settings.</returns>
		static void LoadSettings()
		{            
			SettingsPropertyCollection propColl = Properties.Settings.Default.Properties;
			foreach (SettingsProperty currProperty in propColl)
			{                
				string propertyName  = currProperty.Name;
				string propertyValue = Properties.Settings.Default[propertyName].ToString();
				classLCMSSettings.SetParameter(propertyName, propertyValue);                
			}

			// Add path to executable as a saved setting
			FileInfo fi = new FileInfo(Application.ExecutablePath);
			classLCMSSettings.SetParameter("ApplicationPath", fi.DirectoryName);


            string emulation = classLCMSSettings.GetParameter("EmulationEnabled");
            if (emulation != null)
            {
                bool isEmulated = Convert.ToBoolean(emulation);
                mform_splashScreen.SetEmulatedLabelVisibility(classLCMSSettings.GetParameter("CartName"), isEmulated);
            }
			return;
		}
      #endregion

	    #region Methods
		/// <summary>
		/// Kills any hanging PAL exe processes.
		/// </summary>
		static void KillExistingPalProcesses()
		{
			try
			{
				Process[] processes = Process.GetProcessesByName("paldriv");
				foreach (Process process in processes)
				{
					process.Kill();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Could not kill the PAL processes. " + ex.Message);
			}
		}			

		/// <summary>
		/// Creates the path required for local operation.
		/// </summary>
		/// <param name="path">Local path to create.</param>
		static void CreatePath(string localPath)
		{
			string path = Path.Combine(Application.StartupPath, localPath);
			/// 
			/// See if the logging directory exists
			/// 
			if (Directory.Exists(path) == false)
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch(UnauthorizedAccessException ex)
				{
					/// 
					/// Not much we can do here...
					/// 
					string errorMessage = string.Format("LCMS could not create missing folder {0} required for operation.  Please run application with higher priveleges.  {1}",
																  localPath, ex.Message);
					MessageBox.Show(errorMessage);
					Application.Exit();
				}
			}
		}

		/// <summary>
		/// Updates the splash screen with the appropiate messages.
		/// </summary>
		/// <param name="messageLevel">Filter for displaying messages.</param>
		/// <param name="args">Messages and other arguments passed from the sender.</param>
		static void classApplicationLogger_Message(int messageLevel, classMessageLoggerArgs args)
		{
			if (messageLevel < 1)
			{
				mform_splashScreen.Status = args.Message;
			}
		}

        /// <summary>
        /// Creates the configurations for the columns and systems
        /// </summary>
        static void InitializeSystemConfigurations()
        {
            /// 
            /// Create system data with columns.
            /// 
            classColumnData columnOne   = new classColumnData();
            classColumnData columnTwo   = new classColumnData();
            classColumnData columnThree = new classColumnData();
            classColumnData columnFour  = new classColumnData();


            columnOne.ID = 0;
            columnOne.Name = classLCMSSettings.GetParameter("ColumnName0");
            columnOne.Status = enumColumnStatus.Idle;
            columnOne.Color = Color.Tomato;
            columnOne.First = true;

            columnTwo.ID = 1;
            columnTwo.Name = classLCMSSettings.GetParameter("ColumnName1");
            columnTwo.Status = enumColumnStatus.Idle;
            columnTwo.Color = Color.Lime;

            columnThree.ID = 2;
            columnThree.Name = classLCMSSettings.GetParameter("ColumnName2");
            columnThree.Status = enumColumnStatus.Idle;
            columnThree.Color = Color.LightSteelBlue;

            columnFour.ID = 3;
            columnFour.Name = classLCMSSettings.GetParameter("ColumnName3");
            columnFour.Status = enumColumnStatus.Idle;
            columnFour.Color = Color.LightSalmon;

            classCartConfiguration.Columns = new List<classColumnData>();
            classCartConfiguration.Columns.Add(columnOne);
            classCartConfiguration.Columns.Add(columnTwo);
            classCartConfiguration.Columns.Add(columnThree);
            classCartConfiguration.Columns.Add(columnFour);
        }		
	    #endregion

        public static void LogVersionNumbers()
        {
            string information = SystemInformationReporter.BuildApplicationInformation();
            classApplicationLogger.LogMessage(0, information);
        }

        public static void LogMachineInformation()
        {
            string systemInformation = SystemInformationReporter.BuildSystemInformation();
            classApplicationLogger.LogMessage(0, systemInformation);
        }

	    #region Application entry point
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			CreatePath(formMDImain.CONST_LC_LOG_FOLDER);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			string mutexName = "Global\\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
			
			/// 
			/// Before we do anything, let's initialize the file logging capability.
			/// 
			classApplicationLogger.Error   += new classApplicationLogger.DelegateErrorHandler(classFileLogging.LogError);
			classApplicationLogger.Message += new classApplicationLogger.DelegateMessageHandler(classFileLogging.LogMessage);          
			
            /*
            Duplicate logging entries...disabling for now due to time spent for each log.
            classApplicationLogger.Error += new classApplicationLogger.DelegateErrorHandler(classDbLogger.LogError);
			classApplicationLogger.Message += new classApplicationLogger.DelegateMessageHandler(classDbLogger.LogMessage);
            */

            /// Note that we used icons from here for the gears on the main form window.
            ///     http://labs.chemist2dio.com/free-vector-gears.php/
            /// 
			/// 
			/// Use a mutex to ensure a single copy of program is running. If we can create a new mutex then
			///		no instance of the application is running. Otherwise, we exit.
			/// Code adapated from K. Scott Allen's OdeToCode.com at
			///		http://odetocode.com/Blogs/scott/archive/2004/08/20/401.aspx
			///		
			using (Mutex mutex = new Mutex(false, mutexName))
			{
				if (!mutex.WaitOne(0, false))
				{
					MessageBox.Show("A copy of LcmsNet is already running.");
					classApplicationLogger.LogError(0, "A copy of LcmsNet is already running.", null, null);                    
					return;
				}

				KillExistingPalProcesses();
				GC.KeepAlive(mutex);

				/// 
				/// Synch to the logger so we can display any messages coming back from the 
				/// rest of the program and interface.
				/// 
				classApplicationLogger.Message += new classApplicationLogger.DelegateMessageHandler(classApplicationLogger_Message);

				/// 
				/// Show the splash screen
				/// 
				mform_splashScreen = new formSplashScreen();
				mform_splashScreen.Show();

                LogVersionNumbers();
                LogMachineInformation();
                classApplicationLogger.LogMessage(0, string.Format("[Log]"));

				/// 
				/// Display the splash screen mesasge first! make the log folder, 
				/// after that we will route messages through the logger instead of 
				/// through this static call.  That way we know the log folder has been
				/// created.                
				/// 
				classApplicationLogger.LogMessage(-1, "Creating pertinent folders");
				Application.DoEvents();

				/// 
				/// Make sure we can log/error report locally before we do anything!
				/// 
                try
                {

                    CreatePath(classLCMethodFactory.CONST_LC_METHOD_FOLDER);
                    CreatePath(classDeviceManager.CONST_PUMP_METHOD_PATH);
                    CreatePath(classDeviceManager.CONST_DEVICE_PLUGIN_PATH);
                }
                catch (UnauthorizedAccessException ex)
                {
                    classApplicationLogger.LogError(0, "Could not create necessary directories.", ex);
                    MessageBox.Show("The application does not have the required privileges to run.  Please run as administrator.");
                    return;
                }
                /// 
                /// Load settings 
                /// 
                classApplicationLogger.LogMessage(-1, "Loading settings");
                Application.DoEvents();
                LoadSettings();

                classApplicationLogger.LogMessage(-1, "Creating Initial System Configurations");
                InitializeSystemConfigurations();

				/// 
				/// Create a device manager.
				/// 
				classApplicationLogger.LogMessage(-1, "Creating the Device Manager");
				Application.DoEvents();
                classDeviceManager deviceManager = classDeviceManager.Manager;
                deviceManager.Emulate = Convert.ToBoolean(classLCMSSettings.GetParameter("EmulationEnabled"));
				deviceManager.AddDevice(new LcmsNet.Method.classTimerDevice());
                deviceManager.AddDevice(new LcmsNet.Devices.classBlockDevice());
				deviceManager.AddDevice(new LcmsNet.Devices.classLogDevice());
                deviceManager.AddDevice(new LcmsNet.Devices.classApplicationDevice());


                /// 
                /// Load the device plug-ins.
                /// 
                classApplicationLogger.LogMessage(-1, "Loading necessary device plug-ins.");
                bool areDevicesLoaded = true;
                try
                {
                    classDeviceManager.Manager.LoadPlugins(Assembly.GetExecutingAssembly(), true);
                }
                catch
                {
                    areDevicesLoaded = false;
                    classApplicationLogger.LogError(0, "Could not load internal device plug-ins.");
                }
                classApplicationLogger.LogMessage(-1, "Loading external device plug-ins.");
                try
                {

                    classDeviceManager.Manager.LoadSatelliteAssemblies(classDeviceManager.CONST_DEVICE_PLUGIN_PATH);
                    classDeviceManager.Manager.LoadPlugins(classDeviceManager.CONST_DEVICE_PLUGIN_PATH, "*.dll", true);
                }
                catch
                {
                    areDevicesLoaded = false;
                    classApplicationLogger.LogError(0, "Could not load external device plug-ins."); 
                }
                //TODO: Whoops! not all of the device plug-in loading should kill this effort.  If one set loaded,
                // that may be ok.
                if (!areDevicesLoaded)
                {
                    classApplicationLogger.LogError(-1, "Failed to load some of the device plug-ins.");
                    return;
                }
                classApplicationLogger.LogMessage(-1, "Device plug-ins are loaded.");
				Application.DoEvents();

#if DEBUG
				/// 
				/// Create a device we can use for testing errors with.
				/// 
				LcmsNetDataClasses.Devices.IDevice dev = new LcmsNet.Devices.classErrorDevice();                
				deviceManager.AddDevice(dev);                
#endif

				/// 
				/// Create the method manager
				/// 
				classApplicationLogger.LogMessage(-1, "Creating the Method Manager");
				Application.DoEvents();


				///
				/// Set the logging levels
				///
				if (classLCMSSettings.GetParameter("LoggingErrorLevel") != null)
				{
					classApplicationLogger.ErrorLevel = int.Parse(classLCMSSettings.GetParameter("LoggingErrorLevel"));
				}
				else classApplicationLogger.ErrorLevel = CONST_DEFAULT_ERROR_LOG_LEVEL;


				if (classLCMSSettings.GetParameter("LoggingMsgLevel") != null)
				{
					classApplicationLogger.MessageLevel = int.Parse(classLCMSSettings.GetParameter("LoggingMsgLevel"));
				}
				else classApplicationLogger.MessageLevel = CONST_DEFAULT_MESSAGE_LOG_LEVEL;

				classApplicationLogger.LogMessage(-1, "Loading DMS data");
				Application.DoEvents();
                try
                {
                    classDMSToolsManager.Instance.SelectedTool.LoadCacheFromDMS(false);
                }
                catch(Exception ex)
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Unable to load cache from DMS: " + ex.Message);
                }

				///
				/// Check to see if any trigger files need to be copied to the transfer server, and copy if necessary
				/// 
				if (bool.Parse(classLCMSSettings.GetParameter("CopyTriggerFiles")))
				{
					if (LcmsNetDataClasses.Data.classTriggerFileTools.CheckLocalTriggerFiles())
					{
						classApplicationLogger.LogMessage(-1, "Copying trigger files to DMS");
						LcmsNetDataClasses.Data.classTriggerFileTools.MoveLocalTriggerFiles();
					}
				}
                
				///
				/// Check to see if any method folders need to be copied to the transfer server, and copy if necessary
				/// 
				if (bool.Parse(classLCMSSettings.GetParameter("CopyMethodFolders")))
				{
					if (SampleQueue.IO.classMethodFileTools.CheckLocalMethodFolders())
					{
						classApplicationLogger.LogMessage(-1, "Copying method folders to DMS");
                        SampleQueue.IO.classMethodFileTools.MoveLocalMethodFiles();
					}
				}

 				/// 
				/// Load the main application and run
				/// 
				classApplicationLogger.LogMessage(-1, "Loading main form"); 
				Application.DoEvents();
				formMDImain main = new formMDImain();
				mform_splashScreen.Hide();

				classApplicationLogger.Message -= classApplicationLogger_Message;

				try
				{
				  Application.Run(main);
				}
				catch (Exception ex)
				{
				  classFileLogging.LogError(0, new classErrorLoggerArgs("Program Failed!", ex));
				}
				finally
				{
				  /// 
				  /// Just to make sure...let's kill the PAL at the end of the program as well.
				  /// 
				  KillExistingPalProcesses();

				  // Save the user settings
				  Properties.Settings.Default.ColumnName0           = classLCMSSettings.GetParameter("ColumnName0");
				  Properties.Settings.Default.ColumnName1           = classLCMSSettings.GetParameter("ColumnName1");
				  Properties.Settings.Default.ColumnName2           = classLCMSSettings.GetParameter("ColumnName2");
				  Properties.Settings.Default.ColumnName3           = classLCMSSettings.GetParameter("ColumnName3");
				  Properties.Settings.Default.ValidateSamplesForDMS = bool.Parse(classLCMSSettings.GetParameter("ValidateSamplesForDMS"));
                  Properties.Settings.Default.InstName              = classLCMSSettings.GetParameter("InstName");
                  Properties.Settings.Default.Operator              = classLCMSSettings.GetParameter("Operator");
                  Properties.Settings.Default.CacheFileName         = classLCMSSettings.GetParameter("CacheFileName");
                  Properties.Settings.Default.MinimumVolume         = classLCMSSettings.GetParameter("MinimumVolume");
                  Properties.Settings.Default.SeparationType        = classLCMSSettings.GetParameter("SeparationType");
                  Properties.Settings.Default.TimeZone              = classLCMSSettings.GetParameter("TimeZone");
                  Properties.Settings.Default.DMSTool               = classLCMSSettings.GetParameter("DMSTool");
				  Properties.Settings.Default.Save();

				  classFileLogging.LogMessage(0, new classMessageLoggerArgs("---------------------------------------"));
				}

			}
		}
	#endregion
	}	
}	
