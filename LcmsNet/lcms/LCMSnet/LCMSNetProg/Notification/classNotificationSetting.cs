using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Notification
{ 
    /// <summary>
    /// Holds settings for actions to take on given notifications.
    /// </summary>
    public abstract class classNotificationSetting : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public classNotificationSetting()
        {
            Action           = enumDeviceNotificationAction.Ignore;            
        }

        #region Properties
        /// <summary>
        /// Gets or sets the method to run if notification setting is set to run a method.
        /// </summary>
        public LcmsNetDataClasses.Method.classLCMethod Method
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the action to take when an error occurs.
        /// </summary>
        public enumDeviceNotificationAction Action
        {
            get;
            set;
        }
        #endregion
        /// <summary>
        /// Determines if the action is required based on the value of the notification. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ActionRequired(string value);        
    }
    /// <summary>
    /// Compares text.
    /// </summary>
    public class classNotificationTextSetting: classNotificationSetting
    {
        public classNotificationTextSetting()
        {
            Text = "";
        }

        /// <summary>
        /// Text to compare to.
        /// </summary>
        public string Text
        {
            get;
            set;
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
    /// <summary>
    /// Compares numbers.
    /// </summary>
    public class classNotificationNumberSetting: classNotificationSetting
    {

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double Minimum
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        public double Maximum
        {
            get;
            set;
        }
        /// <summary>
        /// Determines if the action is required based on the value of the notification. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ActionRequired(string value)
        {            
            bool isNumeric = IsNumeric(value.GetType());
            if (isNumeric)
            {
                double result = 0;
                if (double.TryParse(value, out result))
                {
                    return result < Maximum && result < Minimum;
                }
            }
            return false;            
        }

        //TODO: Find a better place for this.
        #region Util
        /// <summary>
        /// Returns true if the type is a double, short, long, int, uint, ushort, ulong, or float.
        /// </summary>
        /// <param name="t">Type to interrogate.</param>
        /// <returns>True if numeric, false if not or null.</returns>
        protected static bool IsNumeric(Type t)
        {

            if (t == null)
                return false;

            bool isNumeric = false;
            isNumeric = isNumeric || (typeof(int) == t);
            isNumeric = isNumeric || (typeof(uint) == t);
            isNumeric = isNumeric || (typeof(ulong) == t);
            isNumeric = isNumeric || (typeof(long) == t);
            isNumeric = isNumeric || (typeof(short) == t);
            isNumeric = isNumeric || (typeof(ushort) == t);
            isNumeric = isNumeric || (typeof(double) == t);
            isNumeric = isNumeric || (typeof(float) == t);
            return isNumeric;
        }
        #endregion
    }
    /// <summary>
    /// Always returns true.
    /// </summary>
    public class classNotificationAlwaysSetting : classNotificationSetting
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
    }
}
