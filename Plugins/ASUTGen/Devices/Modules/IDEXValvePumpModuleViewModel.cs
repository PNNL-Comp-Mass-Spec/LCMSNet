using System.Windows.Controls;
using LcmsNetSDK.Devices;

namespace ASUTGen.Devices.Modules
{
    public class IDEXValvePumpModuleViewModel : BaseDeviceControlViewModel
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXValvePumpModule mobj_valve;

        public override IDevice Device
        {
            get => mobj_valve;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new IDEXValvePumpModuleView();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_valve = device as IDEXValvePumpModule;
        }
    }
}
