using System.Collections.Generic;
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
        private readonly Dictionary<INotifier, List<NotificationSetting>> m_settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotificationConfiguration()
        {
            m_settings = new Dictionary<INotifier, List<NotificationSetting>>();
            IgnoreNotifications = true;
        }

        /// <summary>
        /// Gets or sets whether to ignore all notifications.
        /// </summary>
        public bool IgnoreNotifications { get; set; }

        #region IEnumerable<IDevice> Members

        public IEnumerator<INotifier> GetEnumerator()
        {
            return m_settings.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_settings.Keys.GetEnumerator();
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
            if (!m_settings.ContainsKey(device))
            {
                m_settings.Add(device, new List<NotificationSetting>());
            }
            m_settings[device].Add(setting);
        }

        /// <summary>
        /// Retrieves the device settings for the specified device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public List<NotificationSetting> GetDeviceSettings(INotifier device)
        {
            return m_settings[device];
        }

        public List<INotifier> GetMappedNotifiers()
        {
            var devices = new List<INotifier>();
            foreach (var device in m_settings.Keys)
            {
                devices.Add(device);
            }
            return devices;
        }
    }
}