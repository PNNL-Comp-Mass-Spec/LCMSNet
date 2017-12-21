using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace DemoPluginLibrary
{
    public class DemoClosureAdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        private DemoClosure closure;

        public DemoClosureAdvancedControlViewModel()
        {
            SendCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Send()));
        }

        public void RegisterDevice(IDevice device)
        {
            Device = device;
        }

        public void UnRegisterDevice()
        {
            Device = null;
        }

        public IDevice Device
        {
            get
            {
                return closure;
            }
            set
            {
                closure = value as DemoClosure;
                SetBaseDevice(value);
            }
        }

        private int pulseLength = 1;
        private double voltage = 0;

        public int PulseLength
        {
            get { return pulseLength; }
            set { this.RaiseAndSetIfChanged(ref pulseLength, value); }
        }

        public double Voltage
        {
            get { return voltage; }
            set { this.RaiseAndSetIfChanged(ref voltage, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SendCommand { get; private set; }

        public UserControl GetDefaultView()
        {
            return new DemoClosureAdvancedControlView();
        }

        private void Send()
        {
            closure.Trigger(PulseLength, "Port1", Voltage);
        }
    }
}
