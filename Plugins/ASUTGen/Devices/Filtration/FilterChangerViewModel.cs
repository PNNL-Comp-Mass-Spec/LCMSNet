using System.Windows.Controls;
using LcmsNetSDK.Devices;


namespace ASUTGen.Devices.Filtration
{
    public class FilterChangerViewModel : BaseDeviceControlViewModel
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private FilterChanger mobj_valve;

        public override IDevice Device
        {
            get => mobj_valve;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new FilterChangerView();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_valve = device as FilterChanger;
        }
    }
}
