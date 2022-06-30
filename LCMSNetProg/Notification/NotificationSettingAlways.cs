using System;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Always returns true.
    /// </summary>
    [Serializable]
    public class NotificationAlwaysSetting : NotificationSetting
    {
        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ActionRequired(string value)
        {
            return true;
        }

        public override NotificationConditionNode GetConditions()
        {
            return new NotificationConditionNode { Name = "Always" };
        }

        public override void SetConditions(NotificationConditionNode node)
        {
        }
    }
}
