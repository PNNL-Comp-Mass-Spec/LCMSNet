using System.Windows.Controls;
using LcmsNetSDK.Devices;

namespace ASUTGen.Devices.Pumps
{
    public class IDEXPumpViewModel : BaseDeviceControlViewModel
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXPump mobj_pump;

        public override IDevice Device
        {
            get => mobj_pump;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new IDEXPumpView();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_pump = device as IDEXPump;
        }
    }
}
