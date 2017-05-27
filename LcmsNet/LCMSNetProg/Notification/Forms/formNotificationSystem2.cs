using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using FluidicsSDK;
using LcmsNet.Devices;
using LcmsNet.Method;
using LcmsNet.Notification.ViewModels;
using LcmsNet.Properties;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Notification.Forms
{
    public partial class formNotificationSystem2 : Form
    {
        private NotificationSystemViewModel notificationSystemViewModel;

        public formNotificationSystem2(classDeviceManager manager)
        {
            InitializeComponent();

            notificationSystemViewModel = new NotificationSystemViewModel(manager);
            notificationSystemView.DataContext = notificationSystemViewModel;

            notificationSystemViewModel.ActionRequired += ActionRequiredEventHandler;
        }

        private void ActionRequiredEventHandler(object sender, NotificationSetting setting)
        {
            ActionRequired?.Invoke(sender, setting);
        }

        /// <summary>
        /// Fired when an event is required to be handled by an external component.
        /// </summary>
        public event EventHandler<NotificationSetting> ActionRequired;

        /// <summary>
        /// Loads the notification file from path.
        /// </summary>
        public void LoadNotificationFile()
        {
            notificationSystemViewModel.LoadNotificationFile();
        }
    }
}