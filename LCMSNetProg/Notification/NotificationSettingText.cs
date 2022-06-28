using System;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Compares text.
    /// </summary>
    [Serializable]
    public class NotificationTextSetting : NotificationSetting
    {
        public NotificationTextSetting()
        {
            Text = "";
        }

        /// <summary>
        /// Text to compare to.
        /// </summary>
        public string Text { get; set; }

        public override NotificationConditionNode GetConditions()
        {
            var node = new NotificationConditionNode();
            node.Name = "Text";
            node.Conditions.Add("equals", Text);
            return node;
        }

        public override void SetConditions(NotificationConditionNode node)
        {
            Text = node.Conditions["equals"].ToString();
        }

        /// <summary>
        /// Determines if the action is required based on the value of the notification.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ActionRequired(string value)
        {
            return value == Text;
        }
    }
}
