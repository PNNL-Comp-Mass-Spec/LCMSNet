﻿using System;

namespace LcmsNet.Notification
{
    /// <summary>
    /// Compares numbers.
    /// </summary>
    [Serializable]
    public class NotificationNumberSetting : NotificationSetting
    {
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double Maximum { get; set; }

        public override NotificationConditionNode GetConditions()
        {
            var node = new NotificationConditionNode { Name = "Number" };
            node.Conditions.Add("minimum", Minimum);
            node.Conditions.Add("maximum", Maximum);
            return node;
        }

        public override void SetConditions(NotificationConditionNode node)
        {
            Maximum = Convert.ToDouble(node.Conditions["maximum"]);
            Minimum = Convert.ToDouble(node.Conditions["minimum"]);
        }

        /// <summary>
        /// Determines if the action is required based on the value of the notification.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ActionRequired(string value)
        {
            //bool isNumeric = IsNumeric(value.GetType());
            //if (isNumeric)
            if (value != null)
            {
                if (double.TryParse(value, out var result))
                {
                    return result < Maximum && result < Minimum;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the type is a double, short, long, int, uint, ushort, ulong, or float.
        /// </summary>
        /// <param name="t">Type to interrogate.</param>
        /// <returns>True if numeric, false if not or null.</returns>
        protected static bool IsNumeric(Type t)
        {
            if (t == null)
                return false;

            var isNumeric = false;
            isNumeric = isNumeric || (typeof (int) == t);
            isNumeric = isNumeric || (typeof (uint) == t);
            isNumeric = isNumeric || (typeof (ulong) == t);
            isNumeric = isNumeric || (typeof (long) == t);
            isNumeric = isNumeric || (typeof (short) == t);
            isNumeric = isNumeric || (typeof (ushort) == t);
            isNumeric = isNumeric || (typeof (double) == t);
            isNumeric = isNumeric || (typeof (float) == t);
            return isNumeric;
        }
    }
}
