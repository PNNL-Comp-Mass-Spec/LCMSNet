using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using FluidicsSDK.Devices.Valves;
using LcmsNetData;
using LcmsNetSDK.Devices;

namespace DemoPluginLibrary
{
    public class DemoValveAdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        private ITwoPositionValve valveControls;
        private IDevice valve;

        public DemoValveAdvancedControlViewModel()
        {
            SetACommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetStateA()));
            SetBCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => SetStateB()));
            RefreshCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Refresh()));
        }

        private void SetStateA()
        {
            valveControls.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionA);
        }

        private void SetStateB()
        {
            valveControls.SetPosition(FluidicsSDK.Base.TwoPositionState.PositionB);
        }

        private void Refresh()
        {
            State = ((FluidicsSDK.Base.TwoPositionState)valveControls.GetPosition()).ToString();
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
            get => valve;
            set
            {
                valve = value;
                valveControls = value as ITwoPositionValve;
                SetBaseDevice(value);
            }
        }

        private string state = "";

        public string State
        {
            get => state;
            set => this.RaiseAndSetIfChanged(ref state, value);
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetACommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> SetBCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        public override UserControl GetDefaultView()
        {
            return new DemoValveAdvancedControlView();
        }
    }
}
