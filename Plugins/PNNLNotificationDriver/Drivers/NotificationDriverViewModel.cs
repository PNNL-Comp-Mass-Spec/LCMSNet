﻿using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.FailureInjector.Drivers
{
    public class NotificationDriverViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private NotificationDriver m_driver;

        public NotificationDriverViewModel()
        {
            InjectFailureCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(InjectFailure));
        }

        public ReactiveCommand<Unit, Unit> InjectFailureCommand { get; }

        private void InjectFailure()
        {
            m_driver.InjectFailure();
        }

        public void RegisterDevice(IDevice device)
        {
            m_driver = device as NotificationDriver;
            SetBaseDevice(m_driver);
        }

        public override IDevice Device
        {
            get => m_driver;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new NotificationDriverView();
        }

        public void ShowProps()
        {
        }
    }
}
