using System;
using LcmsNetSDK.Method;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Holds settings for actions to take on given notifications.
    /// </summary>
    [Serializable]
    public abstract class NotificationSetting : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected NotificationSetting()
        {
            Action = DeviceNotificationAction.Ignore;
        }

        /// <summary>
        /// Gets the condition of the setting
        /// </summary>
        public abstract NotificationConditionNode GetConditions();

        /// <summary>
        /// Sets the conditions
        /// </summary>
        /// <returns></returns>
        public abstract void SetConditions(NotificationConditionNode node);

        /// <summary>
        /// Determines if the action is required based on the value of the notification.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ActionRequired(string value);

        #region Properties

        /// <summary>
        /// Gets or sets the method to run if notification setting is set to run a method.
        /// </summary>
        public LCMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the action to take when an error occurs.
        /// </summary>
        public DeviceNotificationAction Action { get; set; }

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}