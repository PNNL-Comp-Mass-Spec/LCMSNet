using System;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.XcaliburLC
{
    public class XcaliburLCViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        /// <summary>
        /// The default Constructor.
        /// </summary>
        public XcaliburLCViewModel()
        {
        }

        ~XcaliburLCViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

        public override UserControl GetDefaultView()
        {
            return new XcaliburLCView();
        }

        /// <summary>
        /// A pump object to use.
        /// </summary>
        private XcaliburLCPump pump;

        private XcaliburConfigViewModel configVm = null;

        public override IDevice Device
        {
            get => pump;
            set => RegisterDevice(value);
        }

        public XcaliburLCPump Pump
        {
            get => pump;
            private set => this.RaiseAndSetIfChanged(ref pump, value);
        }

        public XcaliburConfigViewModel ConfigVm
        {
            get => configVm;
            set => this.RaiseAndSetIfChanged(ref configVm, value);
        }

        /// <summary>
        /// Determines whether or not pump is in emulation mode
        /// </summary>
        public bool Emulation
        {
            get => Pump.Emulation;
            set => Pump.Emulation = value;
        }

        private void RegisterDevice(IDevice device)
        {
            Pump = device as XcaliburLCPump;

            // Initialize the underlying device class
            if (Pump != null)
            {
                ConfigVm = new XcaliburConfigViewModel(Pump);
            }

            // Add to the device manager.
            SetBaseDevice(Pump);
        }
    }
}
