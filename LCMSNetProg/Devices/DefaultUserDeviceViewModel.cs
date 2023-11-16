using System;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices
{
    public class DefaultUserDeviceViewModel : ReactiveObject, IDeviceControl
    {
        public DefaultUserDeviceViewModel()
        {
        }

        private IDevice device;
        private string name = string.Empty;
        private string deviceStatus = string.Empty;
        private bool running = false;
        private bool deviceTabSelected = false;

        public IDevice Device
        {
            get => device;
            set => this.RaiseAndSetIfChanged(ref device, value);
        }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string DeviceStatus
        {
            get => deviceStatus;
            set => this.RaiseAndSetIfChanged(ref deviceStatus, value);
        }

        /// <summary>
        /// Used for conditional control (like key bindings) when the device control is visible
        /// </summary>
        public bool DeviceTabSelected
        {
            get => deviceTabSelected;
            set => this.RaiseAndSetIfChanged(ref deviceTabSelected, value);
        }


        public UserControl GetDefaultView()
        {
            return new DefaultUserDeviceView();
        }

        /// <summary>
        /// Used to disable conditional control without clearing selection states
        /// </summary>
        public virtual void OutOfView()
        { }

        /// <summary>
        /// An event that indicates the name of the column has changed.
        /// </summary>
        /// <remarks>This event is required by IDeviceControl but this class does not use it</remarks>
        public event EventHandler<string> NameChanged
        {
            add { }
            remove { }
        }

        public bool Running
        {
            get => running;
            set => this.RaiseAndSetIfChanged(ref running, value);
        }

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        /// <remarks>This event is required by IDeviceControl but this class does not use it</remarks>
        public event Action SaveRequired
        {
            add { }
            remove { }
        }
    }
}
