using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var node = new NotificationConditionNode();
            node.Name = "Always";
            return node;
        }

        public override void SetConditions(NotificationConditionNode node)
        {
        }
    }
}