﻿using System.Collections.Generic;

namespace LcmsNet.Notification
{
    public class NotificationConditionNode
    {
        public NotificationConditionNode()
        {
            Conditions = new Dictionary<string, object>();
        }

        public string Name { get; set; }

        public Dictionary<string, object> Conditions { get; set; }
    }
}
