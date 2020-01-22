using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetData;
using LcmsNetSDK.Devices;

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

        public override IDevice Device
        {
            get => closure;
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
            get => pulseLength;
            set => this.RaiseAndSetIfChanged(ref pulseLength, value);
        }

        public double Voltage
        {
            get => voltage;
            set => this.RaiseAndSetIfChanged(ref voltage, value);
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SendCommand { get; }

        public override UserControl GetDefaultView()
        {
            return new DemoClosureAdvancedControlView();
        }

        private void Send()
        {
            closure.Trigger(PulseLength, "Port1", Voltage);
        }
    }
}
