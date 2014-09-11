using System;
using System.Collections.Generic;

using LcmsNetDataClasses.Devices;
using LcmsNetSDK.Notifications;

namespace LcmsNetDataClasses.Devices
{

    public class classDeviceStatusEventArgs : EventArgs
    {
        public classDeviceStatusEventArgs(enumDeviceStatus status, string notification, INotifier device) :
            this(status, notification, "", device)
        {
        }
        public classDeviceStatusEventArgs(enumDeviceStatus status, string notification, string message, INotifier notifier)
        {
            Message         = message;
            Status          = status;
            Notification    = notification;
            Notifier        = notifier;
        }

        public INotifier Notifier
        {
            get;
            private set;
        }

        #region Properties
        /// <summary>
        /// Gets the device status.
        /// </summary>
        public enumDeviceStatus Status
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get;
            private set;
		}
	   /// <summary>
	   /// Notification string
	   /// </summary>
		public string Notification
		{
			get;
			private set;
		}
		#endregion
    }	
}
