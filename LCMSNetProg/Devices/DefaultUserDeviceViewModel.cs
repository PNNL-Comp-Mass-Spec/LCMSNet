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


        public UserControl GetDefaultView()
        {
            return new DefaultUserDeviceView();
        }

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
