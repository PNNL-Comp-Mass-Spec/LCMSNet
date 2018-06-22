﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using LcmsNet.Devices;
using LcmsNet.Method;
using LcmsNet.Properties;
using LcmsNet.SampleQueue.IO;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSQLiteTools;

namespace LcmsNet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Event handler that is triggered on application start up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //var mainWindow = new MainWindow();
            //var mainWindowVm = new MainWindowViewModel();
            //mainWindow.DataContext = mainWindowVm;
            //mainWindow.Show();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;

            MainLoad();
        }

        /// <summary>
        /// Event handler that is triggered on application shutdown.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            ShutdownCleanup();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.ExceptionObject is Exception ex)
            {
                UnhandledErrorShutdown(ex);
            }
            else
            {
                Shutdown();
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            UnhandledErrorShutdown(args.Exception);
        }

        private void UnhandledErrorShutdown(Exception ex)
        {
            lock (unhandledErrorLock)
            {
                if (unhandledError)
                {
                    return;
                }
                unhandledError = true;
            }

            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                "Shutting down due to unhandled error.", ex);

            var message = ex.Message;
            var exi = ex.InnerException;
            while (exi != null)
            {
                message += "\nInner Exception: " + exi.Message;
                exi = exi.InnerException;
            }
            MessageBox.Show("Shutting down due to unhandled error: " + message + " \nFor additional information, see the log files at " +
                            DbLogger.LogFolderPath + "\\");

            Shutdown();
        }

        private void ShutdownCleanup()
        {
            lock (cleanedUpLock)
            {
                if (cleanedUp)
                {
                    return;
                }
                cleanedUp = true;
            }

            // Make sure the splash screen is closed.
            if (!splashScreenEnded)
            {
                splashScreen.LoadComplete();
            }

            mainVm?.Dispose();

            // Just to make sure...let's kill the PAL at the end of the program as well.
            KillExistingPalProcesses();
            LogMessage("-----------------shutdown complete----------------------");

            singleInstanceMutex?.ReleaseMutex();
            singleInstanceMutex?.Dispose();
        }

        private bool unhandledError = false;
        private bool cleanedUp = false;
        private readonly object unhandledErrorLock = new object();
        private readonly object cleanedUpLock = new object();

        #region "Constants"

        public const string SOFTWARE_COPYRIGHT = "Copyright Battelle Memorial Institute, 2017";

        public const string SOFTWARE_DEVELOPERS =
            "Brian LaMarche, Christopher Walters, " +
            "Gordon Anderson, David Clark, Bryson Gibbons, Derek Hopkins, " +
            "Matthew Monroe, Ron Moore, Danny Orton, John Ryan, Richard Smith";

        #endregion

        #region "Class variables"

        /// <summary>
        /// Reference to splash screen window.
        /// </summary>
        private DynamicSplashScreenWindow splashScreen;

        private bool splashScreenEnded = false;

        private ManualResetEvent resetSplashCreated;
        private Thread splashThread;

        private MainWindow main = null;
        private MainWindowViewModel mainVm = null;

        private Mutex singleInstanceMutex = null;

        #endregion

        #region Configuration Loading

        /// <summary>
        /// Loads the application settings.
        /// </summary>
        /// <returns>An object that holds the application settings.</returns>
        private void LoadSettings()
        {
            // Note that settings are persisted in file user.config in a randomly named folder below %userprofile%\appdata\local
            // For example:
            // C:\Users\LCMSOperator\AppData\Local\Battelle_PNNL\LcmsNet.exe_Url_v4mgwguq03wniutgfta3xp2qm0ieudrl\4.2.0.0\user.config

            // Possibly upgrade the settings from a previous version
            if (Settings.Default.UpgradeSettings)
            {
                // User settings for this version was not found
                // Try to upgrade from the previous version
                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }

            // Settings: There are the default LcmsNet.exe.config application config and [user\appdata\local\...\user.config] user config files,
            // but some settings apply to the whole system (application config, LcmsNet.exe.config), yet shouldn't be replaced when we install
            // a new version of the program (but the application config file has other important configuration information that should be replaced).
            // To properly handle this, these settings are specified in the application configuration, but we use a different file (specified in
            // app.config, the appSettings file 'LcmsNet_PersistentSettings.config') to overrule the matching settings in the application config file.

            var persistentSettings = ConfigurationManager.AppSettings;
            // If there are developer settings, load them instead of the persistent settings
            var devSettings = (NameValueCollection)(ConfigurationManager.GetSection("developerAppSettings"));
            if (devSettings != null && devSettings.Count > 0)
            {
                persistentSettings = devSettings;
            }

            var mainSettings = Settings.Default.Properties.Cast<SettingsProperty>().ToList();
            foreach (var setting in persistentSettings.AllKeys)
            {
                // Can't add new settings, and we don't want to override user-scope settings
                var settingValue = persistentSettings[setting];
                var match = mainSettings.First(x => x.Name.Equals(setting));
                // Dev settings can override any setting, but the normal persistent settings are only allowed for application settings
                if (match != null && (match.Attributes.Contains(typeof(ApplicationScopedSettingAttribute)) || persistentSettings == devSettings))
                {
                    try
                    {
                        var changedTypeValue = Convert.ChangeType(settingValue, match.PropertyType);
                        Settings.Default[setting] = changedTypeValue;
                    }
                    catch (Exception e)
                    {
                        ApplicationLogger.LogError(0, $"Could not apply persistent setting '{setting}' with value '{settingValue}' to type '{match.PropertyType.FullName}'", e);
                    }
                }
                else
                {
                    ApplicationLogger.LogError(0, $"Could not apply persistent setting '{setting}' with value '{settingValue}': Setting does not exist, or it is a user setting.");
                }
            }

            var propColl = Settings.Default.Properties;
            foreach (SettingsProperty currProperty in propColl)
            {
                var propertyName = currProperty.Name;
                var propertyValue = Settings.Default[propertyName].ToString();
                LCMSSettings.SetParameter(propertyName, propertyValue);
            }

            // Add path to executable as a saved setting
            var fi = new FileInfo(Assembly.GetEntryAssembly().Location);
            LCMSSettings.SetParameter(LCMSSettings.PARAM_APPLICATIONPATH, fi.DirectoryName);

            var emulation = LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED);
            if (!string.IsNullOrWhiteSpace(emulation))
            {
                var isEmulated = Convert.ToBoolean(emulation);
                splashScreen.SetEmulatedLabelVisibility(LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME), isEmulated);
            }
        }

        #endregion

        public void LogVersionNumbers()
        {
            var information = SystemInformationReporter.BuildApplicationInformation();
            LogMessage(information);
        }

        public void LogMachineInformation()
        {
            var systemInformation = SystemInformationReporter.BuildSystemInformation();
            LogMessage(systemInformation);
        }

        private void LCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            var type = Settings.Default[e.SettingName].GetType();
            object newValue = null;
            try
            {
                newValue = Convert.ChangeType(e.SettingValue, type);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, $"Could not update setting \"{e.SettingName}\" of type \"{type.Name}\" with value \"{e.SettingValue}\".", ex);
            }
            Settings.Default[e.SettingName] = newValue;
            Settings.Default.Save();
        }

        #region "Constants"

        /// <summary>
        /// Default logging levels
        /// </summary>
        private const int CONST_DEFAULT_ERROR_LOG_LEVEL = 5;

        private const int CONST_DEFAULT_MESSAGE_LOG_LEVEL = 5;

        #endregion

        #region Methods

        /// <summary>
        /// Kills any hanging PAL exe processes.
        /// </summary>
        private void KillExistingPalProcesses()
        {
            try
            {
                var processes = Process.GetProcessesByName("paldriv");
                foreach (var process in processes)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not kill the PAL processes. " + ex.Message);
            }
        }

        /// <summary>
        /// Creates the path required for local operation.
        /// </summary>
        /// <param name="localPath">Local path to create.</param>
        private void CreatePath(string localPath)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), localPath);

            // See if the logging directory exists
            if (Directory.Exists(path) == false)
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Not much we can do here...
                    var errorMessage =
                        string.Format(
                            "LCMS could not create missing folder {0} required for operation.  Please run application with higher priveleges.  {1}",
                            localPath, ex.Message);
                    MessageBox.Show(errorMessage);
                    Shutdown();
                }
            }
        }

        /// <summary>
        /// Creates the configurations for the columns and systems
        /// </summary>
        private void InitializeSystemConfigurations()
        {
            var columnStatus1 = ColumnStatus.Idle;
            var columnStatus2 = ColumnStatus.Idle;
            var columnStatus3 = ColumnStatus.Idle;
            var columnStatus4 = ColumnStatus.Idle;
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED0, false))
            {
                columnStatus1 = ColumnStatus.Disabled;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED1, false))
            {
                columnStatus2 = ColumnStatus.Disabled;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED2, false))
            {
                columnStatus3 = ColumnStatus.Disabled;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED3, false))
            {
                columnStatus4 = ColumnStatus.Disabled;
            }

            // Create system data with columns.
            var columnOne = new ColumnData
            {
                ID = 0,
                Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME0),
                Status = columnStatus1,
                Color = Colors.Tomato,
                First = true
            };

            var columnTwo = new ColumnData
            {
                ID = 1,
                Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME1),
                Status = columnStatus2,
                Color = Colors.Lime
            };

            var columnThree = new ColumnData
            {
                ID = 2,
                Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME2),
                Status = columnStatus3,
                Color = Colors.LightSteelBlue
            };

            var columnFour = new ColumnData
            {
                ID = 3,
                Name = LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME3),
                Status = columnStatus4,
                Color = Colors.LightSalmon
            };

            CartConfiguration.Columns = new List<ColumnData> {
                columnOne, columnTwo, columnThree, columnFour};
        }

        #endregion

        #region Application entry point

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private async void MainLoad()
        {
            var mutexName = "Global\\" + Assembly.GetExecutingAssembly().GetName().Name;

            // Before we do anything, let's initialize the file logging capability.
            ApplicationLogger.Error += FileLogging.LogError;
            ApplicationLogger.Message += FileLogging.LogMessage;

            // Now lets initialize logging to SQLite database.
            ApplicationLogger.Error += DbLogger.LogError;
            ApplicationLogger.Message += DbLogger.LogMessage;

            // Note that we used icons from here for the gears on the main form window.
            //     http://labs.chemist2dio.com/free-vector-gears.php/
            //
            // Use a mutex to ensure a single copy of program is running. If we can create a new mutex then
            //      no instance of the application is running. Otherwise, we exit.
            // Code adapted from K. Scott Allen's OdeToCode.com at
            //      http://odetocode.com/Blogs/scott/archive/2004/08/20/401.aspx
            singleInstanceMutex = new Mutex(false, mutexName);
            try
            {
                if (!singleInstanceMutex.WaitOne(0, false))
                {
                    MessageBox.Show("A copy of LcmsNet is already running.");
                    ApplicationLogger.LogError(0, "A copy of LcmsNet is already running.", null, null);
                    return;
                }
            }
            catch (AbandonedMutexException)
            {
                ApplicationLogger.LogMessage(0, "Warning: The last LcmsNet session did not close properly (AbandonedMutexException).");
            }

            KillExistingPalProcesses();

            // Show the splash screen
            resetSplashCreated = new ManualResetEvent(false);

            // Run it on a different thread.
            splashThread = new Thread(ShowSplashScreen);
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.IsBackground = true;
            splashThread.Name = "SplashScreen";
            splashThread.Start();

            // Block until the splash screen is displayed
            resetSplashCreated.WaitOne();

            var splashLoadTime = DateTime.UtcNow;

            if (!LoadEverything())
            {
                return;
            }

            // Load the main application and run
            LogMessage(-1, "Loading main window");
            main = new MainWindow();
            this.MainWindow = main;
            mainVm = new MainWindowViewModel();
            main.DataContext = mainVm;
            main.ShowActivated = true;

            Application.Current.MainWindow = main;
            MainWindow = main;

            // Assure that the splash screen has been visible for at least 3 seconds
            while (DateTime.UtcNow.Subtract(splashLoadTime).TotalMilliseconds < 3000)
            {
                await Task.Delay(250);
            }

            splashScreen.LoadComplete();
            splashScreenEnded = true;

            main.Show();
            main.Activate();
        }

        private void ShowSplashScreen()
        {
            splashScreen = new DynamicSplashScreenWindow(SOFTWARE_COPYRIGHT, SOFTWARE_DEVELOPERS);
            splashScreen.Show();

            // set the reset, to allow startup to continue
            resetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        private bool LoadEverything()
        {
            LogVersionNumbers();
            LogMachineInformation();
            LogMessage("[Log]");

            // Display the splash screen mesasge first! make the log folder,
            // after that we will route messages through the logger instead of
            // through this static call.  That way we know the log folder has been
            // created.
            //LogMessage(-1, "Creating pertinent folders");

            // Make sure we can log/error report locally before we do anything!
            try
            {
                CreatePath(LCMethodFactory.CONST_LC_METHOD_FOLDER);
                CreatePath(DeviceManager.CONST_PUMP_METHOD_PATH);
                CreatePath(DeviceManager.CONST_DEVICE_PLUGIN_PATH);
            }
            catch (UnauthorizedAccessException ex)
            {
                ApplicationLogger.LogError(0, "Could not create necessary directories.", ex);
                MessageBox.Show("The application does not have the required privileges to run.  Please run as administrator.");
                return false;
            }

            // Load settings
            LogMessage(-1, "Loading settings");
            LoadSettings();

            // Now that settings have been loaded, set the event handler so that when any of these settings change, we save them to disk.
            LCMSSettings.SettingChanged += LCMSSettings_SettingChanged;

            //LogMessage(-1, "Creating Initial System Configurations");
            InitializeSystemConfigurations();

            // Create a device manager.
            //LogMessage(-1, "Creating the Device Manager");
            var deviceManager = DeviceManager.Manager;
            deviceManager.Emulate = LCMSSettings.GetParameter(LCMSSettings.PARAM_EMULATIONENABLED, false);
            deviceManager.AddDevice(new TimerDevice());
            deviceManager.AddDevice(new BlockDevice());
            deviceManager.AddDevice(new LogDevice());
            deviceManager.AddDevice(new ApplicationDevice());

            // Load the device plug-ins.
            LogMessage(-1, "Loading necessary device plug-ins.");
            var areDevicesLoaded = true;
            try
            {
                DeviceManager.Manager.LoadPlugins(Assembly.GetExecutingAssembly(), true);
            }
            catch
            {
                areDevicesLoaded = false;
                ApplicationLogger.LogError(0, "Could not load internal device plug-ins.");
            }
            LogMessage(-1, "Loading external device plug-ins.");
            try
            {
                DeviceManager.Manager.LoadSatelliteAssemblies(DeviceManager.CONST_DEVICE_PLUGIN_PATH);
                DeviceManager.Manager.LoadPlugins(DeviceManager.CONST_DEVICE_PLUGIN_PATH, "*.dll", true);
            }
            catch
            {
                areDevicesLoaded = false;
                ApplicationLogger.LogError(0, "Could not load external device plug-ins.");
            }
            //TODO: Whoops! not all of the device plug-in loading should kill this effort.  If one set loaded,
            // that may be ok.
            if (!areDevicesLoaded)
            {
                ApplicationLogger.LogError(-1, "Failed to load some of the device plug-ins.");
                return false;
            }
            LogMessage(-1, "Device plug-ins are loaded.");

#if DEBUG
            // Create a device we can use for testing errors with.
            IDevice dev = new ErrorDevice();
            deviceManager.AddDevice(dev);
#endif

            // Create the method manager
            //LogMessage(-1, "Creating the Method Manager");

            // Set the logging levels
            var logLevelErrors = LCMSSettings.GetParameter(LCMSSettings.PARAM_LOGGINGERRORLEVEL);
            if (!string.IsNullOrWhiteSpace(logLevelErrors))
                ApplicationLogger.ErrorLevel = int.Parse(logLevelErrors);
            else
                ApplicationLogger.ErrorLevel = CONST_DEFAULT_ERROR_LOG_LEVEL;

            var logLevelMessages = LCMSSettings.GetParameter(LCMSSettings.PARAM_LOGGINGMSGLEVEL);
            if (!string.IsNullOrWhiteSpace(logLevelMessages))
                ApplicationLogger.MessageLevel = int.Parse(logLevelMessages);
            else
                ApplicationLogger.MessageLevel = CONST_DEFAULT_MESSAGE_LOG_LEVEL;

            CreateSQLCache();
            LogMessage(-1, "Loading DMS data");

            try
            {

                var dmsTools = LcmsNet.Configuration.DMSDataContainer.DBTools;
                LCMSSettings.SetParameter(LCMSSettings.PARAM_DMSTOOL, dmsTools.DMSVersion);

                dmsTools.ProgressEvent += DmsToolsManager_ProgressEvent;
                LcmsNet.Configuration.DMSDataContainer.LogDBToolsEvents = false;

                dmsTools.LoadCacheFromDMS(false);

                LcmsNet.Configuration.DMSDataContainer.LogDBToolsEvents = true;
                dmsTools.ProgressEvent -= DmsToolsManager_ProgressEvent;

            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Error caching DMS data: " + ex.Message);
                if (ex.StackTrace != null)
                    ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Stack trace: " + ex.StackTrace);
            }

            // Check to see if any trigger files need to be copied to the transfer server, and copy if necessary
            if (bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYTRIGGERFILES)))
            {
                if (TriggerFileTools.CheckLocalTriggerFiles())
                {
                    LogMessage(-1, "Copying trigger files to DMS");
                    TriggerFileTools.MoveLocalTriggerFiles();
                }
            }

            // Check to see if any method folders need to be copied to the transfer server, and copy if necessary
            if (bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYMETHODFOLDERS)))
            {
                if (MethodFileTools.CheckLocalMethodFolders())
                {
                    LogMessage(-1, "Copying method folders to DMS");
                    MethodFileTools.MoveLocalMethodFiles();
                }
            }

            return true;
        }

        private void DmsToolsManager_ProgressEvent(object sender, ProgressEventArgs e)
        {
            LogMessage(e.CurrentTask);
        }

        /// <summary>
        /// Create the SQL cache file, if necessary (SQLiteTools does all the heavy lifting.)
        /// </summary>
        private void CreateSQLCache()
        {
            SQLiteTools.SaveSingleColumnListToCache(new List<string> { "default" }, DatabaseTableTypes.CartList, false);
            SQLiteTools.SaveSingleColumnListToCache(new List<string> { "default" }, DatabaseTableTypes.SeparationTypeList, false);
            SQLiteTools.SaveSingleColumnListToCache(new List<string> { "default" }, DatabaseTableTypes.DatasetTypeList, false);
            SQLiteTools.SaveInstListToCache(new List<InstrumentInfo>(), false);
            SQLiteTools.SaveUserListToCache(new List<UserInfo>(), false);
            SQLiteTools.SaveSingleColumnListToCache(new List<string> { "0", "1", "2", "3", "4" }, DatabaseTableTypes.ColumnList, false);
            SQLiteTools.SaveExperimentListToCache(new List<ExperimentData> { new ExperimentData() }, false);
            SQLiteTools.SaveProposalUsers(new List<ProposalUser>(),
                new List<UserIDPIDCrossReferenceEntry>(),
                new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>(), false);
        }

        private void LogMessage(string message)
        {
            LogMessage(0, message);
        }

        private void LogMessage(int msgLevel, string message)
        {
            FileLogging.LogMessage(msgLevel, new MessageLoggerArgs(message));
        }

        #endregion
    }
}