using System.Windows.Controls;
using LcmsNetSDK.Devices;

namespace ASUTGen.Devices.Detectors
{
    public class UVDetectorViewModel : BaseDeviceControlViewModel
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private UVDetector mobj_valve;

        public override IDevice Device
        {
            get => mobj_valve;
            set => RegisterDevice(value);
        }

        public override UserControl GetDefaultView()
        {
            return new UVDetectorView();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_valve = device as UVDetector;
        }
    }
}
