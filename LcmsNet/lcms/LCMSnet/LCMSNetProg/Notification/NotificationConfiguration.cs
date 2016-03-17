using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Configuration of the notifications for each device.
    /// </summary>
    public class NotificationConfiguration : IEnumerable<INotifier>
    {
        /// <summary>
        /// Maps devices to their settings.
        /// </summary>
        private Dictionary<INotifier, List<NotificationSetting>> mdict_settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotificationConfiguration()
        {
            mdict_settings = new Dictionary<INotifier, List<NotificationSetting>>();
            IgnoreNotifications = true;
        }

        /// <summary>
        /// Gets or sets whether to ignore all notifications.
        /// </summary>
        public bool IgnoreNotifications { get; set; }

        #region IEnumerable<IDevice> Members

        public IEnumerator<INotifier> GetEnumerator()
        {
            return this.mdict_settings.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.mdict_settings.Keys.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Adds a setting for the given device.
        /// </summary>
        /// <param name="device">Device to persist.  If not added before will be saved</param>
        /// <param name="name">Name of setting.</param>
        /// <param name="value">Value to assign.</param>
        public void AddSetting(INotifier device, NotificationSetting setting)
        {
            if (!mdict_settings.ContainsKey(device))
            {
                mdict_settings.Add(device, new List<NotificationSetting>());
            }
            mdict_settings[device].Add(setting);
        }

        /// <summary>
        /// Retrieves the device settings for the specified device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public List<NotificationSetting> GetDeviceSettings(INotifier device)
        {
            return mdict_settings[device];
        }

        public List<INotifier> GetMappedNotifiers()
        {
            List<INotifier> devices = new List<INotifier>();
            foreach (INotifier device in mdict_settings.Keys)
            {
                devices.Add(device);
            }
            return devices;
        }
    }
}