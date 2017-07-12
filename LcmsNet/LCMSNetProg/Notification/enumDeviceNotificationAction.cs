using System.Collections.Generic;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Enumerates the type of actions that can be taken.
    /// </summary>
    public enum enumDeviceNotificationAction
    {
        /// <summary>
        /// Ignores the notification.
        /// </summary>
        Ignore,

        /// <summary>
        /// Stops the sample runs.
        /// </summary>
        Stop,

        /// <summary>
        /// Shuts the system down and turns off devices.
        /// </summary>
        Shutdown,

        /// <summary>
        /// Notifies remote users.
        /// </summary>
        NotifyOnly,

        /// <summary>
        /// Runs a given method immediately.
        /// </summary>
        StopAndRunMethodNow,

        /// <summary>
        /// Runs a method at the next earliest time.
        /// </summary>
        RunMethodNext
    }

    /// <summary>
    /// class that holds information about a device, and it's values/UI properties.
    /// </summary>
    internal class classNotificationLinker
    {
        /// <summary>
        /// constructor.
        /// </summary>
        public classNotificationLinker(string name)
        {
            EventMap = new Dictionary<string, NotificationSetting>();
            Name = name;
        }

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets map that strings a notification to an action.
        /// </summary>
        public Dictionary<string, NotificationSetting> EventMap { get; set; }

        #endregion
    }
}