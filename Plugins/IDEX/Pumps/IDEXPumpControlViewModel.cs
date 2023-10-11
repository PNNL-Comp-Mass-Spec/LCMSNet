using System;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.IDEX.Pumps
{
    public class IDEXPumpControlViewModel : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        public IDEXPumpControlViewModel()
        {
        }

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXPump m_valve;

        private string name;

        public void RegisterDevice(IDevice device)
        {
            m_valve = device as IDEXPump;
        }

        #region IDeviceControl Members
#pragma warning disable 67
        public event EventHandler<string> NameChanged;
        public event Action SaveRequired;
#pragma warning restore 67

        public bool Running { get; set; }

        public override IDevice Device
        {
            get => m_valve;
            set => RegisterDevice(value);
        }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public override UserControl GetDefaultView()
        {
            return new IDEXPumpControlView();
        }

        public void ShowProps()
        {
        }

        #endregion
    }
}
