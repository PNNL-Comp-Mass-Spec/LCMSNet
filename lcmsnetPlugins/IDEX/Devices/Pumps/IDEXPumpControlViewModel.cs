using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace ASUTGen.Devices.Pumps
{
    public class IDEXPumpControlViewModel : ReactiveObject, IDeviceControl
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
        public event DelegateNameChanged NameChanged;
        public event DelegateSaveRequired SaveRequired;
#pragma warning restore 67

        public bool Running { get; set; }

        public IDevice Device
        {
            get { return m_valve; }
            set { RegisterDevice(value); }
        }

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public UserControl GetDefaultView()
        {
            return new IDEXPumpControlView();
        }

        public void ShowProps()
        {

        }

        #endregion
    }
}
