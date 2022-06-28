using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using DynamicData;
using FluidicsSDK;
using LcmsNet.Devices;
using LcmsNet.IO;
using LcmsNet.Properties;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.Notification.ViewModels
{
    public class NotificationSystemViewModel : ReactiveObject, IDisposable
    {
        public NotificationSystemViewModel()
        {
            Disable();

            devicesList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var devicesListBound).Subscribe();
            eventsList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var eventsListBound).Subscribe();
            assignedEventsList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var assignedEventsListBound).Subscribe();
            methodsComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodsComboBoxOptionsBound).Subscribe();
            DevicesList = devicesListBound;
            EventsList = eventsListBound;
            AssignedEventsList = assignedEventsListBound;
            MethodsComboBoxOptions = methodsComboBoxOptionsBound;

            deviceEventTable = new Dictionary<INotifier, NotificationLinker>();
            itemToDeviceMap = new Dictionary<string, INotifier>();
            currentSetting = new NotificationAlwaysSetting();

            // Setup notifier and options.
            classNotifier = new Notifier();
            ActionsComboBoxOptions = new ReadOnlyObservableCollection<DeviceNotificationAction>(new ObservableCollection<DeviceNotificationAction>(
                Enum.GetValues(typeof(DeviceNotificationAction)).Cast<DeviceNotificationAction>()));

            // Set the total number of minutes between system health writes.
            RemoteStatusLogInterval = Settings.Default.NotificationWriteTimeMinutes;
            LogStatusToRemotePath = Settings.Default.NotificationShouldNotify;

            // Place to put system health.
            RemoteStatusLogPath = Settings.Default.NotificationDirectoryPath;
            classNotifier.Path = RemoteStatusLogPath;

            // Add any existing devices.
            var devices = DeviceManager.Manager.Devices;
            if (devices.Count > 0)
            {
                foreach (var device in devices)
                {
                    Manager_DeviceAdded(this, device);
                }
            }

            // Synch the device manager.
            var manager = DeviceManager.Manager;
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
            foreach (var method in LCMethodManager.Manager.AllLCMethods)
            {
                Manager_MethodAdded(this, method);
            }

            // Synch methods
            LCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
            LCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;
            LCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;

            remoteStatusLogTimer = new Timer(WriteLogData, this, Timeout.Infinite, Timeout.Infinite);
            ResetRemoteLoggingInterval();

            this.WhenAnyValue(x => x.RemoteStatusLogInterval, x => x.LogStatusToRemotePath).Subscribe(x => this.UpdateTimer());
            this.WhenAnyValue(x => x.SelectedDevice).Subscribe(x => this.SelectedDeviceChanged());
            this.WhenAnyValue(x => x.SelectedEvent).Subscribe(x => this.SelectedEventChanged());
            this.WhenAnyValue(x => x.ConditionNumberChecked, x => x.ConditionTextChecked, x => x.ConditionOccurredChecked,
                    x => x.NumberConditionMinimum, x => x.NumberConditionMaximum, x => x.ConditionTextMatchString)
                .Subscribe(x => this.StoreNotificationSettings());

            this.WhenAnyValue(x => x.SelectedAction).Subscribe(x => this.SetAction());
            this.WhenAnyValue(x => x.SelectedLCMethod).Subscribe(x => this.SetMethod());

            SaveCommand = ReactiveCommand.Create(SaveNotificationFile);
            EnableCommand = ReactiveCommand.Create(Enable, this.WhenAnyValue(x => x.SettingsDisabled).Select(x => x));
            DisableCommand = ReactiveCommand.Create(Disable, this.WhenAnyValue(x => x.SettingsDisabled).Select(x => !x));
            IgnoreSettingCommand = ReactiveCommand.Create(() => SelectedAction = DeviceNotificationAction.Ignore, this.WhenAnyValue(x => x.SelectedAction).Select(x => x != DeviceNotificationAction.Ignore));
            IgnoreAllCommand = ReactiveCommand.Create(IgnoreAll, this.WhenAnyValue(x => x.assignedEventsList.Count).Select(x => x > 0));
        }

        ~NotificationSystemViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            remoteStatusLogTimer.Dispose();
            GC.SuppressFinalize(this);
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

        /// <summary>
        /// Maintains map between device and object that holds the notifications settings.
        /// </summary>
        private readonly Dictionary<INotifier, NotificationLinker> deviceEventTable;

        /// <summary>
        /// Fired when an event is required to be handled by an external component.
        /// </summary>
        public event EventHandler<NotificationSetting> ActionRequired;

        /// <summary>
        /// Notifier object that sends status to remote listeners.
        /// </summary>
        private readonly Notifier classNotifier;

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
        private DeviceNotificationAction selectedAction;
        private string selectedEvent = "";
        private double numberConditionMinimum = 0;
        private double numberConditionMaximum = 0;
        private LCMethod selectedLCMethod;
        private readonly SourceList<string> devicesList = new SourceList<string>();
        private readonly SourceList<string> eventsList = new SourceList<string>();
        private readonly SourceList<string> assignedEventsList = new SourceList<string>();
        private readonly SourceList<LCMethod> methodsComboBoxOptions = new SourceList<LCMethod>();

        /// <summary>
        /// Gets or sets whether to ignore messages for the time being or not.
        /// </summary>
        private bool IgnoreEvents
        {
            get => ignoreEvents;
            set => this.RaiseAndSetIfChanged(ref ignoreEvents, value);
        }

        public bool SettingsDisabled
        {
            get => settingsDisabled;
            set => this.RaiseAndSetIfChanged(ref settingsDisabled, value);
        }

        public bool EventSettingsEnabled
        {
            get => eventSettingsEnabled;
            set => this.RaiseAndSetIfChanged(ref eventSettingsEnabled, value);
        }

        public bool MethodSettingEnabled
        {
            get => methodSettingEnabled;
            set => this.RaiseAndSetIfChanged(ref methodSettingEnabled, value);
        }

        public bool EventsListEnabled
        {
            get => eventsListEnabled;
            set => this.RaiseAndSetIfChanged(ref eventsListEnabled, value);
        }

        public bool ConditionNumberChecked
        {
            get => conditionNumberChecked;
            set => this.RaiseAndSetIfChanged(ref conditionNumberChecked, value);
        }

        public bool ConditionTextChecked
        {
            get => conditionTextChecked;
            set => this.RaiseAndSetIfChanged(ref conditionTextChecked, value);
        }

        public bool ConditionOccurredChecked
        {
            get => conditionOccurredChecked;
            set => this.RaiseAndSetIfChanged(ref conditionOccurredChecked, value);
        }

        public string ConditionTextMatchString
        {
            get => conditionTextMatchString;
            set => this.RaiseAndSetIfChanged(ref conditionTextMatchString, value);
        }

        public bool LogStatusToRemotePath
        {
            get => logStatusToRemotePath;
            set => this.RaiseAndSetIfChanged(ref logStatusToRemotePath, value);
        }

        public int RemoteStatusLogInterval
        {
            get => remoteStatusLogInterval;
            set => this.RaiseAndSetIfChanged(ref remoteStatusLogInterval, value);
        }

        public string RemoteStatusLogPath
        {
            get => remoteStatusLogPath;
            set => this.RaiseAndSetIfChanged(ref remoteStatusLogPath, value);
        }

        public string SettingLabelText
        {
            get => settingLabelText;
            set => this.RaiseAndSetIfChanged(ref settingLabelText, value);
        }

        public string DeviceLabelText
        {
            get => deviceLabelText;
            set => this.RaiseAndSetIfChanged(ref deviceLabelText, value);
        }

        public double NumberConditionMinimum
        {
            get => numberConditionMinimum;
            set => this.RaiseAndSetIfChanged(ref numberConditionMinimum, value);
        }

        public double NumberConditionMaximum
        {
            get => numberConditionMaximum;
            set => this.RaiseAndSetIfChanged(ref numberConditionMaximum, value);
        }

        public string SelectedDevice
        {
            get => selectedDevice;
            set => this.RaiseAndSetIfChanged(ref selectedDevice, value);
        }

        public DeviceNotificationAction SelectedAction
        {
            get => selectedAction;
            set => this.RaiseAndSetIfChanged(ref selectedAction, value);
        }

        public string SelectedEvent
        {
            get => selectedEvent;
            set => this.RaiseAndSetIfChanged(ref selectedEvent, value);
        }

        public LCMethod SelectedLCMethod
        {
            get => selectedLCMethod;
            set => this.RaiseAndSetIfChanged(ref selectedLCMethod, value);
        }

        public ReadOnlyObservableCollection<string> DevicesList { get; }
        public ReadOnlyObservableCollection<string> EventsList { get; }
        public ReadOnlyObservableCollection<string> AssignedEventsList { get; }
        public ReadOnlyObservableCollection<DeviceNotificationAction> ActionsComboBoxOptions { get; }
        public ReadOnlyObservableCollection<LCMethod> MethodsComboBoxOptions { get; }


        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> EnableCommand { get; }
        public ReactiveCommand<Unit, Unit> DisableCommand { get; }
        public ReactiveCommand<Unit, DeviceNotificationAction> IgnoreSettingCommand { get; }
        public ReactiveCommand<Unit, Unit> IgnoreAllCommand { get; }

        /// <summary>
        /// Loads the notification file from path.
        /// </summary>
        public void LoadNotificationFile()
        {
            Disable();

            var path = Settings.Default.NotificationFilePath;
            if (!Path.IsPathRooted(path))
            {
                path = PersistDataPaths.GetFileLoadPath(path);
            }

            if (File.Exists(path))
            {
                var config = NotifierConfigurationXmlFile.ReadConfiguration(path);

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
        /// Save the notification configuration to file
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
                    if (setting.Action != DeviceNotificationAction.Ignore)
                    {
                        setting.Name = key;
                        configuration.AddSetting(device, setting);
                    }
                }
            }

            var path = PersistDataPaths.GetFileSavePath(Settings.Default.NotificationFilePath);
            NotifierConfigurationXmlFile.WriteConfiguration(path, configuration);

            ApplicationLogger.LogMessage(0, "Notification file saved to: " + Settings.Default.NotificationFilePath);
        }

        private void Manager_MethodUpdated(object sender, LCMethod method)
        {
            methodsComboBoxOptions.Edit(list =>
            {
                if (list.Contains(method))
                {
                    var index = list.IndexOf(method);
                    list[index] = method;
                }
            });
        }

        private void Manager_MethodRemoved(object sender, LCMethod method)
        {
            methodsComboBoxOptions.Remove(method);
        }

        private void Manager_MethodAdded(object sender, LCMethod method)
        {
            methodsComboBoxOptions.Edit(list =>
            {
                if (!list.Contains(method))
                {
                    list.Add(method);
                }
            });
        }

        /// <summary>
        /// Renames the device in the listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        private void Manager_DeviceRenamed(object sender, IDevice device)
        {
            if (device.DeviceType == DeviceType.Fluidics)
                return;

            if (!deviceEventTable.TryGetValue(device, out var linker))
            {
                // Add the device - it doesn't exist, maybe because of a name conflict
                Manager_DeviceAdded(sender, device);
                return;
            }
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
            if (device.DeviceType != DeviceType.Component && !(device.GetType() == typeof(ErrorDevice)))
                return;

            AddNotifier(device);
        }

        private void AddNotifier(INotifier device)
        {
            if (itemToDeviceMap.ContainsKey(device.Name))
            {
                ApplicationLogger.LogMessage(0, $"Skipping addition of notifiers for device {device.Name}: notifiers for device with same name already exist!");
                return;
            }
            device.Error += Device_Error;
            device.StatusUpdate += Device_StatusUpdate;

            var linker = new NotificationLinker(device.Name);
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
            if (device == null || ((IDevice)device).DeviceType == DeviceType.Fluidics)
                return;

            device.StatusUpdate -= Device_StatusUpdate;

            if (!deviceEventTable.TryGetValue(device, out var linker))
            {
                return;
            }

            devicesList.Remove(linker.Name);

            if (currentDevice == device)
            {
                currentDevice = null;
                eventsList.Clear();

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
                case DeviceNotificationAction.Ignore:
                    break;
                case DeviceNotificationAction.NotifyOnly:
                    ApplicationLogger.LogMessage(0, string.Format("Handling a user defined event from a device status change - {0}, message: '{1}'.", setting.Name, message));
                    classNotifier.Path = RemoteStatusLogPath;
                    //m_classNotifier.Notify(message, setting);
                    break;
                default:

                    if (ActionRequired != null)
                    {
                        ApplicationLogger.LogMessage(0, string.Format("Action has been set for {0}.", setting.Name));
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
        private void Device_StatusUpdate(object sender, DeviceStatusEventArgs e)
        {
            if (e?.Notifier == null)
                return;

            if (e.Notification == null)
                return;

            var message = string.Empty;
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                message = ": " + e.Message;
            }
            try
            {
                // Get the device link.
                var linker = deviceEventTable[e.Notifier];
                // Then the settings for those devices.
                if (e.Notification.ToLower() != "none")
                {
                    if (linker.EventMap.TryGetValue(e.Notification, out var setting))
                    {
                        // Handles the events.
                        HandleEvents(e.Message, setting);
                    }
                    else
                    {
                        ApplicationLogger.LogMessage(0, string.Format("The device {0} mentioned an unpublished status: \"{1}\".", e.Notifier.Name, e.Notification + message));
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                ApplicationLogger.LogError(0, string.Format("The device {0} mentioned an unpublished status: \"{1}\".", e.Notifier.Name, e.Notification + message), ex);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not execute status update.", ex);
            }

            if (e.LoggingType == DeviceEventLoggingType.Default || e.LoggingType == DeviceEventLoggingType.Message)
            {
                ApplicationLogger.LogMessage(0, $"Device {e.Notifier.Name} reported status: {e.Notification}{message}");
            }
            else if (e.LoggingType == DeviceEventLoggingType.Error)
            {
                ApplicationLogger.LogError(0, $"Device {e.Notifier.Name} reported status: {e.Notification}{message}");
            }
        }

        /// <summary>
        /// Handles errors from devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_Error(object sender, DeviceErrorEventArgs e)
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
                ApplicationLogger.LogError(0, string.Format("The device {0} mentioned an unpublished error: \"{1}\".", e.Device.Name, e.Notification), ex);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not execute status update.", ex);
            }

            var msg = "";
            if (!string.IsNullOrWhiteSpace(e.Error))
            {
                msg = $": {e.Error}";
            }
            if (e.LoggingType == DeviceEventLoggingType.Message)
            {
                ApplicationLogger.LogMessage(0, $"Device {e.Device.Name} reported error: {e.Notification}{msg}");
            }
            else if (e.LoggingType == DeviceEventLoggingType.Default || e.LoggingType == DeviceEventLoggingType.Error)
            {
                ApplicationLogger.LogError(0, $"Device {e.Device.Name} reported error: {e.Notification}{msg}");
            }
        }

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
            if (action == DeviceNotificationAction.RunMethodNext || action == DeviceNotificationAction.StopAndRunMethodNow)
            {
                MethodSettingEnabled = true;
            }
            else
            {
                MethodSettingEnabled = false;
            }

            if (currentSetting != null && action != currentSetting.Action)
            {
                var previousAction = currentSetting.Action;
                currentSetting.Action = action;
                if (previousAction == DeviceNotificationAction.Ignore && !AssignedEventsList.Contains(currentNotification))
                {
                    eventsList.Remove(currentNotification);
                    assignedEventsList.Add(currentNotification);
                }
                else if (action == DeviceNotificationAction.Ignore && !EventsList.Contains(currentNotification))
                {
                    assignedEventsList.Remove(currentNotification);
                    eventsList.Add(currentNotification);
                }
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

        private void StoreNotificationSettings()
        {
            if (string.IsNullOrWhiteSpace(SelectedEvent))
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
                item = devicesList.Items.First();
            }
            if (!string.IsNullOrEmpty(item))
            {
                ClearEventData();
                var device = itemToDeviceMap[item];
                currentDevice = device;
                var linker = deviceEventTable[device];

                DeviceLabelText = "Notifier: " + device.Name;

                assignedEventsList.Clear();
                eventsList.Clear();
                foreach (var key in linker.EventMap.Keys)
                {
                    var setting = linker.EventMap[key];
                    if (setting.Action != DeviceNotificationAction.Ignore)
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
                if (eventsList.Count > 0)
                {
                    SelectedEvent = eventsList.Items.First();
                }
                else if (assignedEventsList.Count > 0)
                {
                    SelectedEvent = assignedEventsList.Items.First();
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

            if (setting.Action == DeviceNotificationAction.RunMethodNext ||
                setting.Action == DeviceNotificationAction.StopAndRunMethodNow)
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

        private void ClearEventData()
        {
            currentNotification = "";
            SelectedEvent = "";
            SelectedLCMethod = null;

            SettingLabelText = "Setting:";
            ConditionNumberChecked = false;
            NumberConditionMinimum = 0;
            NumberConditionMaximum = 0;

            ConditionTextChecked = false;
            ConditionTextMatchString = "";

            ConditionOccurredChecked = true;

            SelectedAction = DeviceNotificationAction.Ignore;
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
                foreach (var item in assignedEventsList.Items)
                {
                    var setting = deviceEventTable[currentDevice].EventMap[item];
                    setting.Action = DeviceNotificationAction.Ignore;
                    removals.Add(item);
                }

                assignedEventsList.Edit(list =>
                {
                    foreach (var removal in removals)
                    {
                        list.Remove(removal);
                    }
                });
                eventsList.AddRange(removals);

                EventSettingsEnabled = false;
            }
        }
    }
}
