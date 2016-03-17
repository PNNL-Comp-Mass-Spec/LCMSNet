using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNet.Method;
using LcmsNet.Notification;
using LcmsNetDataClasses;
using LcmsNet.Devices;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Notification.Forms
{
    public partial class formNotificationSystem : Form
    {
        public formNotificationSystem(classDeviceManager manager)
        {
            InitializeComponent();
            Disable();

            mbool_updateUI = false;
            mbool_updatingTimer = false;
            mdict_deviceEventTable = new Dictionary<INotifier, classNotificationLinker>();
            mdict_itemToDeviceMap = new Dictionary<ListViewItem, INotifier>();
            mobj_currentSetting = null;

            // Setup notifier and options.
            mobj_notifier = new classNotifier();
            Array actions = Enum.GetValues(typeof (enumDeviceNotificationAction));
            object[] data = new object[actions.Length];
            actions.CopyTo(data, 0);
            mcomboBox_actions.Items.AddRange(data);

            // Set the total number of minutes between system health writes.
            mnum_statusMinutes.Value = Properties.Settings.Default.NotificationWriteTimeMinutes;
            mcheckBox_writeStatus.Checked = Properties.Settings.Default.NotificationShouldNotify;

            // Place to put system health.
            mtextBox_path.Text = Properties.Settings.Default.NotificationDirectoryPath;
            mobj_notifier.Path = mtextBox_path.Text;

            // Add any existing devices.
            List<IDevice> devices = classDeviceManager.Manager.Devices;
            if (devices.Count > 0)
            {
                foreach (IDevice device in devices)
                {
                    manager_DeviceAdded(this, device);
                }
            }

            // Synch the device manager.
            manager.DeviceRenamed += new DelegateDeviceUpdated(manager_DeviceRenamed);
            manager.DeviceRemoved += new DelegateDeviceUpdated(manager_DeviceRemoved);
            manager.DeviceAdded += new DelegateDeviceUpdated(manager_DeviceAdded);

            var model = FluidicsSDK.classFluidicsModerator.Moderator;
            model.ModelCheckAdded += Model_ModelCheckAdded;
            foreach (var modelCheck in model.GetModelCheckers())
            {
                AddNotifier(modelCheck as INotifier);
            }

            foreach (INotifier notifier in Notification.NotificationBroadcaster.Manager.Notifiers)
            {
                AddNotifier(notifier);
            }
            Notification.NotificationBroadcaster.Manager.Added +=
                new EventHandler<NotifierChangedEventArgs>(Manager_Added);
            Notification.NotificationBroadcaster.Manager.Removed +=
                new EventHandler<NotifierChangedEventArgs>(Manager_Removed);

            // Add any existing methods.
            Dictionary<string, classLCMethod> methods = classLCMethodManager.Manager.Methods;
            if (methods.Count > 0)
            {
                foreach (string key in methods.Keys)
                {
                    Manager_MethodAdded(this, methods[key]);
                }
            }


            // Synch methods
            classLCMethodManager.Manager.MethodAdded += new DelegateMethodUpdated(Manager_MethodAdded);
            classLCMethodManager.Manager.MethodUpdated += new DelegateMethodUpdated(Manager_MethodUpdated);
            classLCMethodManager.Manager.MethodRemoved += new DelegateMethodUpdated(Manager_MethodRemoved);
        }

        /// <summary>
        /// Gets or sets whether to ignore messages for the time being or not.
        /// </summary>
        private bool IgnoreEvents { get; set; }

        void Model_ModelCheckAdded(object sender, ModelCheckControllerEventArgs e)
        {
            AddNotifier(e.ModelChecker);
        }

        void Manager_Added(object sender, NotifierChangedEventArgs e)
        {
            AddNotifier(e.Notifier);
        }

        void Manager_Removed(object sender, NotifierChangedEventArgs e)
        {
            RemoveNotifier(e.Notifier);
        }

        public void Enable()
        {
            IgnoreEvents = false;
            settingsPanel.Enabled = true;
            mlabel_enabled.Visible = false;
        }

        public void Disable()
        {
            IgnoreEvents = true;
            settingsPanel.Enabled = false;
            mlabel_enabled.Visible = true;
        }

        private void mlistBox_events_Click(object sender, EventArgs e)
        {
        }

        #region Members

        /// <summary>
        /// Maintains map between device and object that holds the notifications settings.
        /// </summary>
        private Dictionary<INotifier, classNotificationLinker> mdict_deviceEventTable;

        /// <summary>
        /// Fired when an event is required to be handled by an external component.
        /// </summary>
        public event EventHandler<NotificationSetting> ActionRequired;

        /// <summary>
        /// Notifier object that sends status to remote listeners.
        /// </summary>
        private classNotifier mobj_notifier;

        /// <summary>
        /// Flag indicating whether the timer is being set.
        /// </summary>
        private bool mbool_updatingTimer;

        /// <summary>
        /// Current setting object.
        /// </summary>
        private NotificationSetting mobj_currentSetting;

        /// <summary>
        /// Maps a list view item to a device.
        /// </summary>
        private Dictionary<ListViewItem, INotifier> mdict_itemToDeviceMap;

        /// <summary>
        /// Flag indicating the ui is being updated.
        /// </summary>
        private bool mbool_updateUI;

        /// <summary>
        /// The current notification string that is used to trigger an event.
        /// </summary>
        private string mstring_currentNotification;

        /// <summary>
        /// Current device being set.
        /// </summary>
        private INotifier mobj_currentDevice = null;

        #endregion

        #region Loading and Saving

        /// <summary>
        /// Loads the notification file from path.
        /// </summary>
        public void LoadNotificationFile()
        {
            Disable();

            string path = Properties.Settings.Default.NotificationFilePath;
            if (System.IO.File.Exists(path))
            {
                classXMLDeviceNotifierConfigurationReader reader = new classXMLDeviceNotifierConfigurationReader();
                NotificationConfiguration config = reader.ReadConfiguration(path);

                List<INotifier> devices = config.GetMappedNotifiers();
                foreach (INotifier device in devices)
                {
                    List<NotificationSetting> settings = config.GetDeviceSettings(device);
                    foreach (NotificationSetting setting in settings)
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
            NotificationConfiguration configuration = new NotificationConfiguration();
            configuration.IgnoreNotifications = IgnoreEvents;

            foreach (INotifier device in mdict_deviceEventTable.Keys)
            {
                classNotificationLinker linker = mdict_deviceEventTable[device];
                foreach (string key in linker.EventMap.Keys)
                {
                    NotificationSetting setting = linker.EventMap[key];
                    if (setting.Action != enumDeviceNotificationAction.Ignore)
                    {
                        setting.Name = key;
                        configuration.AddSetting(device, setting);
                    }
                }
            }
            classXMLDeviceNotificationConfigurationWriter writer = new classXMLDeviceNotificationConfigurationWriter();
            writer.WriteConfiguration(Properties.Settings.Default.NotificationFilePath, configuration);

            classApplicationLogger.LogMessage(0,
                "Notification file saved to: " + Properties.Settings.Default.NotificationFilePath);
        }

        #endregion

        #region Method Manager Events

        bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            if (mcomboBox_methods.Items.Contains(method))
            {
                int index = mcomboBox_methods.Items.IndexOf(method);
                mcomboBox_methods.Items[index] = method;
            }
            return true;
        }

        bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (mcomboBox_methods.Items.Contains(method))
            {
                mcomboBox_methods.Items.Remove(method);
            }
            return true;
        }

        bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            if (!mcomboBox_methods.Items.Contains(method))
            {
                mcomboBox_methods.Items.Add(method);
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
        void manager_DeviceRenamed(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            if (device.DeviceType == enumDeviceType.Fluidics)
                return;
            classNotificationLinker linker = mdict_deviceEventTable[device];
            mlistview_devices.BeginUpdate();
            linker.Item.Text = device.Name;
            mlistview_devices.EndUpdate();
        }

        /// <summary>
        /// adds a device to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void manager_DeviceAdded(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            if (device == null)
                return;

            // Make sure we only add components, and the error device(for testing)
            if (device.DeviceType != enumDeviceType.Component && !device.GetType().Equals(typeof (classErrorDevice)))
                return;

            AddNotifier(device);
        }

        private void AddNotifier(INotifier device)
        {
            device.Error += new EventHandler<classDeviceErrorEventArgs>(device_Error);
            device.StatusUpdate += new EventHandler<classDeviceStatusEventArgs>(device_StatusUpdate);

            classNotificationLinker linker = new classNotificationLinker(device.Name);
            List<string> errors = device.GetErrorNotificationList();
            if (errors != null)
            {
                foreach (string error in errors)
                {
                    linker.EventMap.Add(error, new NotificationAlwaysSetting());
                }

                mdict_itemToDeviceMap.Add(linker.Item, device);
                mdict_deviceEventTable.Add(device, linker);
            }


            List<string> statuses = device.GetStatusNotificationList();
            if (statuses != null)
            {
                foreach (string status in statuses)
                {
                    linker.EventMap.Add(status, new NotificationAlwaysSetting());
                }
            }

            mlistview_devices.Items.Add(linker.Item);
        }

        void RemoveNotifier(INotifier device)
        {
            if (device == null || ((IDevice) device).DeviceType == enumDeviceType.Fluidics)
                return;

            device.StatusUpdate -= device_StatusUpdate;

            classNotificationLinker linker = mdict_deviceEventTable[device];
            mlistview_devices.BeginUpdate();
            mlistview_devices.Items.Remove(linker.Item);

            if (mobj_currentDevice == device)
            {
                mobj_currentDevice = null;
                mlistBox_events.Items.Clear();
                mgroupBox_actions.Enabled = false;
                mgroupBox_conditions.Enabled = false;
            }

            mdict_deviceEventTable.Remove(device);
            mlistview_devices.EndUpdate();
        }

        /// <summary>
        /// Handles dereferencing the devices from the lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void manager_DeviceRemoved(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            RemoveNotifier(device as INotifier);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles events from the devices.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="setting"></param>
        void HandleEvents(string message, NotificationSetting setting)
        {
            if (IgnoreEvents)
                return;

            // Do we even need to fire an event?
            bool actionRequired = setting.ActionRequired(message);
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
                    classApplicationLogger.LogMessage(0,
                        string.Format("Handling a user defined event from a device status change - {0}.",
                            setting.Name));
                    mobj_notifier.Path = mtextBox_path.Text;
                    //mobj_notifier.Notify(message, setting);
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
        void device_StatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            if (e == null)
                return;

            if (e.Notifier == null)
                return;

            if (e.Notification == null)
                return;

            try
            {
                // Get the device link.
                classNotificationLinker linker = mdict_deviceEventTable[e.Notifier];
                // Then the settings for those devices.
                if (e.Notification.ToLower() != "none")
                {
                    NotificationSetting setting = linker.EventMap[e.Notification];
                    // Handles the events.
                    HandleEvents(e.Message, setting);
                }
            }
            catch (KeyNotFoundException ex)
            {
                classApplicationLogger.LogError(0,
                    string.Format("The device {0} mentioned an unpublished status {1}.", e.Notifier.Name, e.Notification),
                    ex);
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
        void device_Error(object sender, classDeviceErrorEventArgs e)
        {
            if (e == null)
                return;

            if (e.Device == null)
                return;

            if (e.Notification == null)
                return;

            try
            {
                // Get the device link
                classNotificationLinker linker = mdict_deviceEventTable[e.Device];
                // Then the settings for those devices.
                NotificationSetting setting = linker.EventMap[e.Notification];

                HandleEvents(e.Error, setting);

                mnotify_icon.BalloonTipText = "LCMSNet requries your attention.";
                mnotify_icon.ShowBalloonTip(10000);
            }
            catch (KeyNotFoundException ex)
            {
                classApplicationLogger.LogError(0,
                    string.Format("The device {0} mentioned an unpublished error {1}.", e.Device.Name, e.Notification),
                    ex);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not execute status update.", ex);
            }
        }

        #endregion

        #region Timer Event Setup And Events

        /// <summary>
        /// Updates whether to send notifications or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcheckBox_writeStatus_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NotificationShouldNotify = mcheckBox_writeStatus.Checked;
            Properties.Settings.Default.Save();

            mtimer_notifier.Enabled = mcheckBox_writeStatus.Checked;
        }

        /// <summary>
        /// Updates how long to monitor status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnum_statusMinutes_ValueChanged(object sender, EventArgs e)
        {
            mbool_updatingTimer = true;
            int minutes = Convert.ToInt32(mnum_statusMinutes.Value);
            SetTime(minutes);
            mbool_updatingTimer = false;
        }

        /// <summary>
        /// Sets the timer value.
        /// </summary>
        /// <param name="minutes"></param>
        private void SetTime(int rawMinutes)
        {
            int milliSeconds = rawMinutes * 1000 * 60; // 60 seconds / minute * 1000 ms / second
            mtimer_notifier.Interval = milliSeconds;

            Properties.Settings.Default.NotificationWriteTimeMinutes = rawMinutes;
            Properties.Settings.Default.Save();

            if (!mbool_updatingTimer)
            {
                mnum_statusMinutes.Value = Convert.ToDecimal(milliSeconds);
            }
        }

        /// <summary>
        /// Updates how to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtimer_notifier_Tick(object sender, EventArgs e)
        {
            if (mcheckBox_writeStatus.Checked)
            {
                mobj_notifier.Path = mtextBox_path.Text;
                // mobj_notifier.WriteSystemHealth();
            }
        }

        #endregion

        #region Setting Actions and Methods

        /// <summary>
        /// Sets the action for the current notification.
        /// </summary>
        private void SetAction()
        {
            enumDeviceNotificationAction action = ((enumDeviceNotificationAction) mcomboBox_actions.SelectedItem);
            if (action == enumDeviceNotificationAction.RunMethodNext ||
                action == enumDeviceNotificationAction.StopAndRunMethodNow)
            {
                mcomboBox_methods.Enabled = true;
            }
            else
            {
                mcomboBox_methods.Enabled = false;
            }

            if (mobj_currentSetting != null)
            {
                mbool_updateUI = true;
                if (mobj_currentSetting.Action == enumDeviceNotificationAction.Ignore)
                {
                    mlistBox_events.Items.Remove(mstring_currentNotification);
                    mlistbox_assignedEvents.Items.Add(mstring_currentNotification);
                }
                else if (action == enumDeviceNotificationAction.Ignore)
                {
                    mlistbox_assignedEvents.Items.Remove(mstring_currentNotification);
                    mlistBox_events.Items.Add(mstring_currentNotification);
                }
                mbool_updateUI = false;
                mobj_currentSetting.Action = action;
            }
        }

        /// <summary>
        /// Sets the method for the current notification.
        /// </summary>
        private void SetMethod()
        {
            classLCMethod method = mcomboBox_methods.SelectedItem as classLCMethod;
            if (method != null)
            {
                if (mobj_currentSetting != null)
                {
                    mobj_currentSetting.Method = method;
                }
            }
        }

        #endregion

        #region Form Event Handlers that change the action view.

        private void mnum_minimum_ValueChanged(object sender, EventArgs e)
        {
            if (mbool_updateUI)
                return;


            mnum_maximum.Minimum = mnum_minimum.Value;

            NotificationNumberSetting setting = new NotificationNumberSetting();
            setting.Minimum = Convert.ToDouble(mnum_minimum.Value);
            setting.Maximum = Convert.ToDouble(mnum_maximum.Value);
            setting.Method = mobj_currentSetting.Method;
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mnum_maximum_ValueChanged(object sender, EventArgs e)
        {
            mnum_minimum.Maximum = mnum_maximum.Value;

            if (mbool_updateUI)
                return;

            NotificationNumberSetting setting = new NotificationNumberSetting();
            setting.Minimum = Convert.ToDouble(mnum_minimum.Value);
            setting.Maximum = Convert.ToDouble(mnum_maximum.Value);
            setting.Method = mobj_currentSetting.Method;
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mradioButton_number_CheckedChanged(object sender, EventArgs e)
        {
            if (!mradioButton_number.Checked)
            {
                mgroupBox_number.Enabled = false;
            }
            else
            {
                mgroupBox_number.Enabled = true;
            }
            if (mbool_updateUI)
                return;

            NotificationNumberSetting setting = new NotificationNumberSetting();
            setting.Method = mobj_currentSetting.Method;
            setting.Minimum = Convert.ToDouble(mnum_minimum.Value);
            setting.Maximum = Convert.ToDouble(mnum_maximum.Value);
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mradioButton_text_CheckedChanged(object sender, EventArgs e)
        {
            if (!mradioButton_text.Checked)
            {
                mgroupBox_text.Enabled = false;
            }
            else
            {
                mgroupBox_text.Enabled = true;
            }

            if (mbool_updateUI)
                return;

            NotificationTextSetting setting = new NotificationTextSetting();
            setting.Text = mtextBox_statusText.Text;
            setting.Method = mobj_currentSetting.Method;
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mradioButton_happens_CheckedChanged(object sender, EventArgs e)
        {
            if (mbool_updateUI)
                return;

            NotificationAlwaysSetting setting = new NotificationAlwaysSetting();
            setting.Method = mobj_currentSetting.Method;
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mtextBox_statusText_TextChanged(object sender, EventArgs e)
        {
            if (mbool_updateUI)
                return;

            NotificationTextSetting setting = new NotificationTextSetting();
            setting.Text = mtextBox_statusText.Text;
            setting.Method = mobj_currentSetting.Method;
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;
        }

        private void mcomboBox_actions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mbool_updateUI)
                return;

            SetAction();
        }

        /// <summary>
        /// Handles changing the LC method for the current setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mcomboBox_methods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mbool_updateUI)
                return;

            SetMethod();
        }

        #endregion

        #region Form Event Handlers that set the action view

        //TODO: Remove this reference if the current device is ever removed.
        private void mlistview_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mlistview_devices.SelectedItems.Count > 0)
            {
                ListViewItem item = mlistview_devices.SelectedItems[0];
                INotifier device = mdict_itemToDeviceMap[item];
                mobj_currentDevice = device;
                classNotificationLinker linker = mdict_deviceEventTable[device];

                mlabel_device.Text = "Notifier: " + device.Name;

                mlistBox_events.BeginUpdate();
                mlistBox_events.Items.Clear();
                mlistbox_assignedEvents.Items.Clear();
                mstring_currentNotification = "";
                foreach (string key in linker.EventMap.Keys)
                {
                    NotificationSetting setting = linker.EventMap[key];
                    if (setting.Action != enumDeviceNotificationAction.Ignore)
                    {
                        mlistbox_assignedEvents.Items.Add(key);
                    }
                    else
                    {
                        mlistBox_events.Items.Add(key);
                    }
                }
                if (mlistBox_events.Items.Count < 1)
                {
                    mlistBox_events.Enabled = false;
                    mgroupBox_actions.Enabled = false;
                    mgroupBox_conditions.Enabled = false;
                }
                else
                {
                    mlistBox_events.Enabled = true;
                    mgroupBox_actions.Enabled = false;
                    mgroupBox_conditions.Enabled = false;
                }
                mlistBox_events.EndUpdate();
            }
        }

        private void SetSetting(NotificationSetting setting)
        {
            mobj_currentSetting = setting;
            mdict_deviceEventTable[mobj_currentDevice].EventMap[mstring_currentNotification] = setting;

            Type type = setting.GetType();

            if (type == typeof (NotificationTextSetting))
            {
                NotificationTextSetting text = setting as NotificationTextSetting;
                mtextBox_statusText.Text = text.Text;
                mradioButton_text.Checked = true;
            }
            else if (type == typeof (NotificationNumberSetting))
            {
                NotificationNumberSetting number = setting as NotificationNumberSetting;
                mnum_minimum.Minimum = Convert.ToDecimal(number.Minimum);
                mnum_minimum.Value = Convert.ToDecimal(number.Minimum);
                mnum_maximum.Maximum = Convert.ToDecimal(number.Maximum);
                mnum_maximum.Value = Convert.ToDecimal(number.Maximum);

                mradioButton_number.Checked = true;
            }
            else
            {
                mradioButton_happens.Checked = true;
            }
            mgroupBox_actions.Enabled = true;
            mgroupBox_conditions.Enabled = true;
            mcomboBox_actions.SelectedItem = setting.Action;

            if (setting.Action == enumDeviceNotificationAction.RunMethodNext ||
                setting.Action == enumDeviceNotificationAction.StopAndRunMethodNow)
            {
                mcomboBox_methods.Enabled = true;
            }

            if (setting.Method != null)
            {
                try
                {
                    mcomboBox_methods.SelectedItem = setting.Method;
                }
                catch
                {
                }
            }
        }

        private void AddSetting(NotificationSetting setting, string notification, INotifier device)
        {
            mdict_deviceEventTable[device].EventMap[notification] = setting;
        }

        private void mlistBox_events_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mobj_currentDevice != null && !mbool_updateUI)
            {
                mbool_updateUI = true;
                classNotificationLinker linker = mdict_deviceEventTable[mobj_currentDevice];
                if (mlistBox_events.SelectedItem != null)
                {
                    string key = mlistBox_events.SelectedItem.ToString();
                    if (key != null)
                    {
                        mlabel_setting.Text = key;
                        mstring_currentNotification = key;
                        if (linker.EventMap.ContainsKey(key))
                        {
                            if (mlistbox_assignedEvents.SelectedIndex >= 0)
                            {
                                mlistbox_assignedEvents.SetSelected(mlistbox_assignedEvents.SelectedIndex, false);
                            }
                            NotificationSetting setting = linker.EventMap[key];
                            SetSetting(setting);
                        }
                    }
                }
                mbool_updateUI = false;
            }
        }

        /// <summary>
        /// Changes the settings object for assigned events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mlistbox_assignedEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mobj_currentDevice != null && !mbool_updateUI)
            {
                mbool_updateUI = true;
                classNotificationLinker linker = mdict_deviceEventTable[mobj_currentDevice];

                if (mlistbox_assignedEvents.SelectedItem == null)
                    return;

                string key = mlistbox_assignedEvents.SelectedItem.ToString();
                if (key != null)
                {
                    mstring_currentNotification = key;
                    if (linker.EventMap.ContainsKey(key))
                    {
                        mlabel_setting.Text = key;
                        if (mlistBox_events.SelectedIndex >= 0)
                        {
                            mlistBox_events.SetSelected(mlistBox_events.SelectedIndex, false);
                        }

                        NotificationSetting setting = linker.EventMap[key];
                        SetSetting(setting);
                    }
                }
                mbool_updateUI = false;
            }
        }

        /// <summary>
        /// Clears the actions for all notificaiton settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_ignoreAll_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to clear all assigned notifications?",
                "Confirm", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                mbool_updateUI = true;
                List<string> removals = new List<string>();
                foreach (object item in mlistbox_assignedEvents.Items)
                {
                    string key = item.ToString();
                    NotificationSetting setting = mdict_deviceEventTable[mobj_currentDevice].EventMap[key];
                    setting.Action = enumDeviceNotificationAction.Ignore;
                    removals.Add(key);
                }
                foreach (string eventKey in removals)
                {
                    mlistbox_assignedEvents.Items.Remove(eventKey);
                    mlistBox_events.Items.Add(eventKey);
                }
                mgroupBox_actions.Enabled = false;
                mgroupBox_conditions.Enabled = false;
                mbool_updateUI = false;
            }
        }

        private void mtextBox_path_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NotificationDirectoryPath = mtextBox_path.Text;
            Properties.Settings.Default.Save();
        }

        private void mbutton_save_Click(object sender, EventArgs e)
        {
            SaveNotificationFile();
        }

        private void mbutton_enable_Click(object sender, EventArgs e)
        {
            Enable();
        }

        private void mbutton_disable_Click(object sender, EventArgs e)
        {
            Disable();
        }

        /// <summary>
        /// Ignores the currently selected setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_ignoreThisSetting_Click(object sender, EventArgs e)
        {
            mcomboBox_actions.SelectedItem = enumDeviceNotificationAction.Ignore;
        }

        #endregion
    }
}