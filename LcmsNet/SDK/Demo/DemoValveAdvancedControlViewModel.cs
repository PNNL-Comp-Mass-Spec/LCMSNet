using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace DemoPluginLibrary
{
    public class DemoValveAdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControlWpf
    {
        private DemoValve valve;

        public DemoValveAdvancedControlViewModel()
        {
            SetACommand = ReactiveUI.ReactiveCommand.Create(() => SetStateA());
            SetBCommand = ReactiveUI.ReactiveCommand.Create(() => SetStateB());
            RefreshCommand = ReactiveUI.ReactiveCommand.Create(() => Refresh());
        }

        private void SetStateA()
        {
            valve.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionA);
        }

        private void SetStateB()
        {
            valve.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionB);
        }

        private void Refresh()
        {
            State = ((FluidicsSDK.Base.TwoPositionState)valve.GetPosition()).ToString();
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
                return valve;
            }
            set
            {
                valve = value as DemoValve;
                SetBaseDevice(value);
            }
        }

        private string state = "";

        public string State
        {
            get { return state; }
            set { this.RaiseAndSetIfChanged(ref state, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetACommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetBCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshCommand { get; private set; }

        public UserControl GetDefaultView()
        {
            return new DemoValveAdvancedControlView();
        }
    }
}
