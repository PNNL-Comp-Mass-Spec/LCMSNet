using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using FluidicsSDK;
using LcmsNet.Devices;
using LcmsNet.Method;
using LcmsNet.Properties;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK.Notifications;
using ReactiveUI;

namespace LcmsNet.Notification.ViewModels
{
    public class NotificationSystemViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the notification system view control that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public NotificationSystemViewModel()
        {
        }

        public NotificationSystemViewModel(classDeviceManager manager)
        {
            Disable();

            deviceEventTable = new Dictionary<INotifier, classNotificationLinker>();
            itemToDeviceMap = new Dictionary<string, INotifier>();
            currentSetting = new NotificationAlwaysSetting();

            // Setup notifier and options.
            classNotifier = new classNotifier();
            var actions = Enum.GetValues(typeof(enumDeviceNotificationAction));
            using (actionsComboBoxOptions.SuppressChangeNotifications())
            {
                actionsComboBoxOptions.AddRange(actions.Cast<enumDeviceNotificationAction>());
            }

            // Set the total number of minutes between system health writes.
            RemoteStatusLogInterval = Settings.Default.NotificationWriteTimeMinutes;
            LogStatusToRemotePath = Settings.Default.NotificationShouldNotify;

            // Place to put system health.
            RemoteStatusLogPath = Settings.Default.NotificationDirectoryPath;
            classNotifier.Path = RemoteStatusLogPath;

            // Add any existing devices.
            var devices = classDeviceManager.Manager.Devices;
            if (devices.Count > 0)
            {
                foreach (var device in devices)
                {
                    Manager_DeviceAdded(this, device);
                }
            }

            // Synch the device manager.
            manager.DeviceRenamed += Manager_DeviceRenamed;
            manager.DeviceRemoved += Manager_DeviceRemoved;
            manager.DeviceAdded += Manager_DeviceAdded;

            var model = FluidicsModerator.Moderator;
            model.ModelCheckAdded += Model_ModelCheckAdded;
            foreach (var modelCheck in model.GetModelCheckers())
            {
                AddNotifier(modelCheck);
            }

            foreach (var notifier in NotificationBroadcaster.Manager.Notifiers)
            {
                AddNotifier(notifier);
            }
            NotificationBroadcaster.Manager.Added += Manager_Added;
            NotificationBroadcaster.Manager.Removed += Manager_Removed;

            // Add any existing methods.
            var methods = classLCMethodManager.Manager.Methods;
            if (methods.Count > 0)
            {
                foreach (var key in methods.Keys)
                {
                    Manager_MethodAdded(this, methods[key]);
                }
            }

            // Synch methods
            classLCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
            classLCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;
            classLCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;

            remoteStatusLogTimer = new Timer(WriteLogData, this, Timeout.Infinite, Timeout.Infinite);
            ResetRemoteLoggingInterval();

            this.WhenAnyValue(x => x.RemoteStatusLogInterval, x => x.LogStatusToRemotePath).Subscribe(x => this.UpdateTimer());
            this.WhenAnyValue(x => x.SelectedDevice).Subscribe(x => this.SelectedDeviceChanged());
            this.WhenAnyValue(x => x.SelectedEvent).Subscribe(x => this.SelectedEventChanged());
            this.WhenAnyValue(x => x.SelectedAssignedEvent).Subscribe(x => this.SelectedAssignedEventChanged());
            this.WhenAnyValue(x => x.ConditionNumberChecked, x => x.ConditionTextChecked, x => x.ConditionOccurredChecked,
                    x => x.NumberConditionMinimum, x => x.NumberConditionMaximum, x => x.ConditionTextMatchString)
                .Subscribe(x => this.StoreNotificationSettings());

            this.WhenAnyValue(x => x.SelectedAction).Subscribe(x => this.SetAction());
            this.WhenAnyValue(x => x.SelectedLCMethod).Subscribe(x => this.SetMethod());

            SetupCommands();
        }

        private void Model_ModelCheckAdded(object sender, ModelCheckControllerEventArgs e)
        {
            AddNotifier(e.ModelChecker);
        }

        private void Manager_Added(object sender, NotifierChangedEventArgs e)
        {
            AddNotifier(e.Notifier);
        }

        private void Manager_Removed(object sender, NotifierChangedEventArgs e)
        {
            RemoveNotifier(e.Notifier);
        }

        public void Enable()
        {
            IgnoreEvents = false;
            SettingsDisabled = false;
        }

        public void Disable()
        {
            IgnoreEvents = true;
            SettingsDisabled = true;
        }

        #region Members

        /// <summary>
        /// Maintains map between device and object that holds the notifications settings.
        /// </summary>
        private readonly Dictionary<INotifier, classNotificationLinker> deviceEventTable;

        /// <summary>
        /// Fired when an event is required to be handled by an external component.
        /// </summary>
        public event EventHandler<NotificationSetting> ActionRequired;

        /// <summary>
        /// Notifier object that sends status to remote listeners.
        /// </summary>
        private readonly classNotifier classNotifier;

        /// <summary>
        /// Current setting object.
        /// </summary>
        private NotificationSetting currentSetting;

        /// <summary>
        /// Maps a list view item to a device.
        /// </summary>
        private readonly Dictionary<string, INotifier> itemToDeviceMap;

        /// <summary>
        /// The current notification string that is used to trigger an event.
        /// </summary>
        private string currentNotification;

        /// <summary>
        /// Current device being set.
        /// </summary>
        private INotifier currentDevice;

        private readonly Timer remoteStatusLogTimer;

        private bool ignoreEvents;
        private bool settingsDisabled = false;
        private bool eventSettingsEnabled = false;
        private bool methodSettingEnabled = false;
        private bool eventsListEnabled = false;
        private bool conditionNumberChecked = false;
        private bool conditionTextChecked = false;
        private bool conditionOccurredChecked = true;
        private string conditionTextMatchString = "";
        private bool logStatusToRemotePath = false;
        private int remoteStatusLogInterval = 10;
        private string remoteStatusLogPath = "";
        private string deviceLabelText = "Device:";
        private string settingLabelText = "Setting:";
        private string selectedDevice = "";
        private enumDeviceNotificationAction selectedAction;
        private string selectedEvent = "";
        private string selectedAssignedEvent = "";
        private double numberConditionMinimum = 0;
        private double numberConditionMaximum = 0;
        private classLCMethod selectedLCMethod;
        private readonly ReactiveList<string> devicesList = new ReactiveList<string>();
        private readonly ReactiveList<string> eventsList = new ReactiveList<string>();
        private readonly ReactiveList<string> assignedEventsList = new ReactiveList<string>();
        private readonly ReactiveList<enumDeviceNotificationAction> actionsComboBoxOptions = new ReactiveList<enumDeviceNotificationAction>();
        private readonly ReactiveList<classLCMethod> methodsComboBoxOptions = new ReactiveList<classLCMethod>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether to ignore messages for the time being or not.
        /// </summary>
        private bool IgnoreEvents
        {
            get { return ignoreEvents; }
            set { this.RaiseAndSetIfChanged(ref ignoreEvents, value); }
        }

        public bool SettingsDisabled
        {
            get { return settingsDisabled; }
            set { this.RaiseAndSetIfChanged(ref settingsDisabled, value); }
        }

        public bool EventSettingsEnabled
        {
            get { return eventSettingsEnabled; }
            set { this.RaiseAndSetIfChanged(ref eventSettingsEnabled, value); }
        }

        public bool MethodSettingEnabled
        {
            get { return methodSettingEnabled; }
            set { this.RaiseAndSetIfChanged(ref methodSettingEnabled, value); }
        }

        public bool EventsListEnabled
        {
            get { return eventsListEnabled; }
            set { this.RaiseAndSetIfChanged(ref eventsListEnabled, value); }
        }

        public bool ConditionNumberChecked
        {
            get { return conditionNumberChecked; }
            set { this.RaiseAndSetIfChanged(ref conditionNumberChecked, value); }
        }

        public bool ConditionTextChecked
        {
            get { return conditionTextChecked; }
            set { this.RaiseAndSetIfChanged(ref conditionTextChecked, value); }
        }

        public bool ConditionOccurredChecked
        {
            get { return conditionOccurredChecked; }
            set { this.RaiseAndSetIfChanged(ref conditionOccurredChecked, value); }
        }

        public string ConditionTextMatchString
        {
            get { return conditionTextMatchString; }
            set { this.RaiseAndSetIfChanged(ref conditionTextMatchString, value); }
        }

        public bool LogStatusToRemotePath
        {
            get { return logStatusToRemotePath; }
            set { this.RaiseAndSetIfChanged(ref logStatusToRemotePath, value); }
        }

        public int RemoteStatusLogInterval
        {
            get { return remoteStatusLogInterval; }
            set { this.RaiseAndSetIfChanged(ref remoteStatusLogInterval, value); }
        }

        public string RemoteStatusLogPath
        {
            get { return remoteStatusLogPath; }
            set { this.RaiseAndSetIfChanged(ref remoteStatusLogPath, value); }
        }

        public string SettingLabelText
        {
            get { return settingLabelText; }
            set { this.RaiseAndSetIfChanged(ref settingLabelText, value); }
        }

        public string DeviceLabelText
        {
            get { return deviceLabelText; }
            set { this.RaiseAndSetIfChanged(ref deviceLabelText, value); }
        }

        public double NumberConditionMinimum
        {
            get { return numberConditionMinimum; }
            set { this.RaiseAndSetIfChanged(ref numberConditionMinimum, value); }
        }

        public double NumberConditionMaximum
        {
            get { return numberConditionMaximum; }
            set { this.RaiseAndSetIfChanged(ref numberConditionMaximum, value); }
        }

        public string SelectedDevice
        {
            get { return selectedDevice; }
            set { this.RaiseAndSetIfChanged(ref selectedDevice, value); }
        }

        public enumDeviceNotificationAction SelectedAction
        {
            get { return selectedAction; }
            set { this.RaiseAndSetIfChanged(ref selectedAction, value); }
        }

        public string SelectedEvent
        {
            get { return selectedEvent; }
            set { this.RaiseAndSetIfChanged(ref selectedEvent, value); }
        }

        public string SelectedAssignedEvent
        {
            get { return selectedAssignedEvent; }
            set { this.RaiseAndSetIfChanged(ref selectedAssignedEvent, value); }
        }

        public classLCMethod SelectedLCMethod
        {
            get { return selectedLCMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedLCMethod, value); }
        }

        public IReadOnlyReactiveList<string> DevicesList => devicesList;
        public IReadOnlyReactiveList<string> EventsList => eventsList;
        public IReadOnlyReactiveList<string> AssignedEventsList => assignedEventsList;
        public IReadOnlyReactiveList<enumDeviceNotificationAction> ActionsComboBoxOptions => actionsComboBoxOptions;
        public IReadOnlyReactiveList<classLCMethod> MethodsComboBoxOptions => methodsComboBoxOptions;

        #endregion

        #region Loading and Saving

        /// <summary>
        /// Loads the notification file from path.
        /// </summary>
        public void LoadNotificationFile()
        {
            Disable();

            var path = Settings.Default.NotificationFilePath;
            if (File.Exists(path))
            {
                var reader = new classXMLDeviceNotifierConfigurationReader();
                var config = reader.ReadConfiguration(path);

                var devices = config.GetMappedNotifiers();
                foreach (var device in devices)
                {
                    var settings = config.GetDeviceSettings(device);
                    foreach (var setting in settings)
                    {
                        AddSetting(setting, setting.Name, device);
                    }
                }
                if (config.IgnoreNotifications)
                    Disable();
                else
                    Enable();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void SaveNotificationFile()
        {
            var configuration = new NotificationConfiguration
            {
                IgnoreNotifications = IgnoreEvents
            };

            foreach (var device in deviceEventTable.Keys)
            {
                var linker = deviceEventTable[device];
                foreach (var key in linker.EventMap.Keys)
                {
                    var setting = linker.EventMap[key];
                    if (setting.Action != enumDeviceNotificationAction.Ignore)
                    {
                        setting.Name = key;
                        configuration.AddSetting(device, setting);
                    }
                }
            }
            var writer = new classXMLDeviceNotificationConfigurationWriter();
            writer.WriteConfiguration(Settings.Default.NotificationFilePath, configuration);

            classApplicationLogger.LogMessage(0, "Notification file saved to: " + Settings.Default.NotificationFilePath);
        }

        #endregion

        #region Method Manager Events

        private bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            if (methodsComboBoxOptions.Contains(method))
            {
                var index = methodsComboBoxOptions.IndexOf(method);
                methodsComboBoxOptions[index] = method;
            }
            return true;
        }

        private bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (methodsComboBoxOptions.Contains(method))
            {
                methodsComboBoxOptions.Remove(method);
            }
            return true;
        }

        private bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            if (!methodsComboBoxOptions.Contains(method))
            {
                methodsComboBoxOptions.Add(method);
            }
            return true;
        }

        #endregion

        #region Device Manager Events

        /// <summary>
        /// Renames the device in the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRenamed(object sender, IDevice device)
        {
            if (device.DeviceType == enumDeviceType.Fluidics)
                return;
            var linker = deviceEventTable[device];
            linker.Name = device.Name;
        }

        /// <summary>
        /// adds a device to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceAdded(object sender, IDevice device)
        {
            if (device == null)
                return;

            // Make sure we only add components, and the error device(for testing)
            if (device.DeviceType != enumDeviceType.Component && !(device.GetType() == typeof(classErrorDevice)))
                return;

            AddNotifier(device);
        }

        private void AddNotifier(INotifier device)
        {
            device.Error += Device_Error;
            device.StatusUpdate += Device_StatusUpdate;

            var linker = new classNotificationLinker(device.Name);
            var errors = device.GetErrorNotificationList();
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    linker.EventMap.Add(error, new NotificationAlwaysSetting());
                }

                itemToDeviceMap.Add(linker.Name, device);
                deviceEventTable.Add(device, linker);
            }


            var statuses = device.GetStatusNotificationList();
            if (statuses != null)
            {
                foreach (var status in statuses)
                {
                    linker.EventMap.Add(status, new NotificationAlwaysSetting());
                }
            }

            devicesList.Add(linker.Name);
        }

        private void RemoveNotifier(INotifier device)
        {
            if (device == null || ((IDevice)device).DeviceType == enumDeviceType.Fluidics)
                return;

            device.StatusUpdate -= Device_StatusUpdate;

            var linker = deviceEventTable[device];
            devicesList.Remove(linker.Name);

            if (currentDevice == device)
            {
                currentDevice = null;
                using (eventsList.SuppressChangeNotifications())
                {
                    eventsList.Clear();
                }

                EventSettingsEnabled = false;
            }

            deviceEventTable.Remove(device);
        }

        /// <summary>
        /// Handles dereferencing the devices from the lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRemoved(object sender, IDevice device)
        {
            RemoveNotifier(device);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles events from the devices.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        private void HandleEvents(string message, NotificationSetting setting)
        {
            if (IgnoreEvents)
                return;

            // Do we even need to fire an event?
            var actionRequired = setting.ActionRequired(message);
            if (!actionRequired)
            {
                return;
            }

            // Run the action now!
            switch (setting.Action)
            {
                case enumDeviceNotificationAction.Ignore:
                    break;
                case enumDeviceNotificationAction.NotifyOnly:
                    classApplicationLogger.LogMessage(0, string.Format("Handling a user defined event from a device status change - {0}.", setting.Name));
                    classNotifier.Path = RemoteStatusLogPath;
                    //m_classNotifier.Notify(message, setting);
                    break;
                default:

                    if (ActionRequired != null)
                    {
                        classApplicationLogger.LogMessage(0, string.Format("Action has been set for {0}.", setting.Name));
                        ActionRequired(this, setting);
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles status updates from errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_StatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            if (e?.Notifier == null)
                return;

            if (e.Notification == null)
                return;

            try
            {
                // Get the device link.
                var linker = deviceEventTable[e.Notifier];
                // Then the settings for those devices.
                if (e.Notification.ToLower() != "none")
                {
                    var setting = linker.EventMap[e.Notification];
                    // Handles the events.
                    HandleEvents(e.Message, setting);
                }
            }
            catch (KeyNotFoundException ex)
            {
                classApplicationLogger.LogError(0, string.Format("The device {0} mentioned an unpublished status: \"{1}\".", e.Notifier.Name, e.Notification), ex);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not execute status update.", ex);
            }
        }

        /// <summary>
        /// Handles errors from devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_Error(object sender, classDeviceErrorEventArgs e)
        {
            if (e?.Device == null)
                return;

            if (e.Notification == null)
                return;

            try
            {
                // Get the device link
                var linker = deviceEventTable[e.Device];
                // Then the settings for those devices.
                var setting = linker.EventMap[e.Notification];

                HandleEvents(e.Error, setting);

                // automatically stop after 10 seconds
                // TODO: Should probably blink or highlight the notifications tab until it is selected.
                TaskBarManipulation.Instance.BlinkTaskbar(10000);
            }
            catch (KeyNotFoundException ex)
            {
                classApplicationLogger.LogError(0, string.Format("The device {0} mentioned an unpublished error: \"{1}\".", e.Device.Name, e.Notification), ex);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not execute status update.", ex);
            }
        }

        #endregion

        #region Timer Event Setup And Events

        /// <summary>
        /// Updates the timer - if notifications should be sent, and if so how often to send them.
        /// </summary>
        private void UpdateTimer()
        {
            ResetRemoteLoggingInterval();

            Settings.Default.NotificationShouldNotify = LogStatusToRemotePath;
            Settings.Default.NotificationWriteTimeMinutes = RemoteStatusLogInterval;
            Settings.Default.Save();
        }

        private void ResetRemoteLoggingInterval()
        {
            var time = Timeout.InfiniteTimeSpan;
            if (LogStatusToRemotePath)
            {
                time = TimeSpan.FromMinutes(RemoteStatusLogInterval);
            }
            remoteStatusLogTimer.Change(time, time);
        }

        /// <summary>
        /// Updates how to
        /// </summary>
        /// <param name="sender"></param>
        private void WriteLogData(object sender)
        {
            if (LogStatusToRemotePath)
            {
                classNotifier.Path = RemoteStatusLogPath;
                // classNotifier.WriteSystemHealth();
            }
        }

        #endregion

        #region Setting Actions and Methods

        /// <summary>
        /// Sets the action for the current notification.
        /// </summary>
        private void SetAction()
        {
            if (string.IsNullOrWhiteSpace(currentNotification))
            {
                return;
            }
            var action = SelectedAction;
            if (action == enumDeviceNotificationAction.RunMethodNext || action == enumDeviceNotificationAction.StopAndRunMethodNow)
            {
                MethodSettingEnabled = true;
            }
            else
            {
                MethodSettingEnabled = false;
            }

            if (currentSetting != null)
            {
                if (currentSetting.Action == enumDeviceNotificationAction.Ignore && !assignedEventsList.Contains(currentNotification))
                {
                    eventsList.Remove(currentNotification);
                    assignedEventsList.Add(currentNotification);
                }
                else if (action == enumDeviceNotificationAction.Ignore && !eventsList.Contains(currentNotification))
                {
                    assignedEventsList.Remove(currentNotification);
                    eventsList.Add(currentNotification);
                }
                currentSetting.Action = action;
            }
        }

        /// <summary>
        /// Sets the method for the current notification.
        /// </summary>
        private void SetMethod()
        {
            var method = SelectedLCMethod;
            if (method != null)
            {
                if (currentSetting != null)
                {
                    currentSetting.Method = method;
                }
            }
        }

        #endregion

        #region Form Event Handlers that change the action view.

        private void StoreNotificationSettings()
        {
            if (string.IsNullOrWhiteSpace(SelectedEvent) && string.IsNullOrWhiteSpace(SelectedAssignedEvent))
            {
                return;
            }
            NotificationSetting setting = null;
            if (ConditionNumberChecked)
            {
                setting = new NotificationNumberSetting
                {
                    Minimum = NumberConditionMinimum,
                    Maximum = NumberConditionMaximum,
                    Method = currentSetting.Method
                };
            }
            else if (ConditionTextChecked)
            {
                setting = new NotificationTextSetting
                {
                    Text = ConditionTextMatchString,
                    Method = currentSetting.Method
                };
            }
            else if (ConditionOccurredChecked)
            {
                setting = new NotificationAlwaysSetting
                {
                    Method = currentSetting.Method
                };
            }

            currentSetting = setting;
            deviceEventTable[currentDevice].EventMap[currentNotification] = setting;
        }

        #endregion

        #region Form Event Handlers that set the action view

        //TODO: Remove this reference if the current device is ever removed.
        private void SelectedDeviceChanged()
        {
            string item = "";
            if (!string.IsNullOrEmpty(SelectedDevice))
            {
                item = SelectedDevice;
            }
            else if (devicesList.Count > 0)
            {
                item = devicesList.First();
            }
            if (!string.IsNullOrEmpty(item))
            {
                ClearEventData();
                var device = itemToDeviceMap[item];
                currentDevice = device;
                var linker = deviceEventTable[device];

                DeviceLabelText = "Notifier: " + device.Name;

                using (assignedEventsList.SuppressChangeNotifications())
                using (eventsList.SuppressChangeNotifications())
                {
                    assignedEventsList.Clear();
                    eventsList.Clear();
                }
                foreach (var key in linker.EventMap.Keys)
                {
                    var setting = linker.EventMap[key];
                    if (setting.Action != enumDeviceNotificationAction.Ignore)
                    {
                        assignedEventsList.Add(key);
                    }
                    else
                    {
                        eventsList.Add(key);
                    }
                }
                if (eventsList.Count < 1 && assignedEventsList.Count < 1)
                {
                    EventsListEnabled = false;
                    EventSettingsEnabled = false;
                }
                else
                {
                    EventsListEnabled = true;
                    EventSettingsEnabled = false;
                }
            }
        }

        private void SetSetting(NotificationSetting setting)
        {
            currentSetting = setting;
            deviceEventTable[currentDevice].EventMap[currentNotification] = setting;

            if (setting is NotificationTextSetting text)
            {
                ConditionTextMatchString = text.Text;
                ConditionTextChecked = true;
            }
            else if (setting is NotificationNumberSetting number)
            {
                NumberConditionMinimum = number.Minimum;
                NumberConditionMaximum = number.Maximum;

                ConditionNumberChecked = true;
            }
            else
            {
                ConditionOccurredChecked = true;
            }
            EventSettingsEnabled = true;
            SelectedAction = setting.Action;

            if (setting.Action == enumDeviceNotificationAction.RunMethodNext ||
                setting.Action == enumDeviceNotificationAction.StopAndRunMethodNow)
            {
                MethodSettingEnabled = true;
            }

            if (setting.Method != null)
            {
                try
                {
                    SelectedLCMethod = setting.Method;
                }
                catch
                {
                }
            }
        }

        private void AddSetting(NotificationSetting setting, string notification, INotifier device)
        {
            deviceEventTable[device].EventMap[notification] = setting;
        }

        private void SelectedEventChanged()
        {
            if (currentDevice != null)
            {
                var linker = deviceEventTable[currentDevice];
                var key = SelectedEvent;

                if (!string.IsNullOrWhiteSpace(key))
                {
                    SettingLabelText = key;
                    currentNotification = key;
                    if (linker.EventMap.ContainsKey(key))
                    {
                        var setting = linker.EventMap[key];
                        SetSetting(setting);
                    }
                }
            }
        }

        /// <summary>
        /// Changes the settings object for assigned events.
        /// </summary>
        private void SelectedAssignedEventChanged()
        {
            if (currentDevice != null)
            {
                var linker = deviceEventTable[currentDevice];

                var key = SelectedAssignedEvent;
                if (string.IsNullOrWhiteSpace(key))
                {
                    return;
                }
                currentNotification = key;

                if (linker.EventMap.ContainsKey(key))
                {
                    SettingLabelText = key;

                    var setting = linker.EventMap[key];
                    SetSetting(setting);
                }
            }
        }

        private void ClearEventData()
        {
            currentNotification = "";
            SelectedEvent = "";
            SelectedAssignedEvent = "";
            SelectedLCMethod = null;

            SettingLabelText = "Setting:";
            ConditionNumberChecked = false;
            NumberConditionMinimum = 0;
            NumberConditionMaximum = 0;

            ConditionTextChecked = false;
            ConditionTextMatchString = "";

            ConditionOccurredChecked = true;

            SelectedAction = enumDeviceNotificationAction.Ignore;
        }

        /// <summary>
        /// Clears the actions for all notification settings.
        /// </summary>
        private void IgnoreAll()
        {
            var result = MessageBox.Show("Are you sure you want to clear all assigned notifications?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var removals = new List<string>();
                foreach (var item in assignedEventsList)
                {
                    var setting = deviceEventTable[currentDevice].EventMap[item];
                    setting.Action = enumDeviceNotificationAction.Ignore;
                    removals.Add(item);
                }

                using (eventsList.SuppressChangeNotifications())
                using (assignedEventsList.SuppressChangeNotifications())
                {
                    assignedEventsList.RemoveAll(removals);
                    eventsList.AddRange(removals);
                }

                EventSettingsEnabled = false;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand SaveCommand { get; private set; }
        public ReactiveCommand EnableCommand { get; private set; }
        public ReactiveCommand DisableCommand { get; private set; }
        public ReactiveCommand IgnoreSettingCommand { get; private set; }
        public ReactiveCommand IgnoreAllCommand { get; private set; }

        private void SetupCommands()
        {
            SaveCommand = ReactiveCommand.Create(() => SaveNotificationFile());
            EnableCommand = ReactiveCommand.Create(() => Enable(), this.WhenAnyValue(x => x.SettingsDisabled).Select(x => x));
            DisableCommand = ReactiveCommand.Create(() => Disable(), this.WhenAnyValue(x => x.SettingsDisabled).Select(x => !x));
            IgnoreSettingCommand = ReactiveCommand.Create(() => SelectedAction = enumDeviceNotificationAction.Ignore, this.WhenAnyValue(x => x.SelectedAction).Select(x => x != enumDeviceNotificationAction.Ignore));
            IgnoreAllCommand = ReactiveCommand.Create(() => IgnoreAll(), this.WhenAnyValue(x => x.assignedEventsList.Count).Select(x => x > 0));
        }

        #endregion
    }
}
