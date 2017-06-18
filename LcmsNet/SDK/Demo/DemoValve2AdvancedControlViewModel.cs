using System;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using FluidicsSDK.Base;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace DemoPluginLibrary
{
    public class DemoValve2AdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControlWpf
    {
        private DemoValve2 valve;

        public DemoValve2AdvancedControlViewModel()
        {
            valvePositionComboBoxOptions = new ReactiveUI.ReactiveList<EightPositionState>(Enum.GetValues(typeof(EightPositionState)).Cast<EightPositionState>());
            SetCommand = ReactiveUI.ReactiveCommand.Create(() => Set());
            RefreshCommand = ReactiveUI.ReactiveCommand.Create(() => Refresh());
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
                valve = value as DemoValve2;
                SetBaseDevice(value);
            }
        }

        private readonly ReactiveUI.ReactiveList<EightPositionState> valvePositionComboBoxOptions;
        private EightPositionState selectedPosition;
        private string state = "";

        public ReactiveUI.ReactiveList<EightPositionState> ValvePositionComboBoxOptions => valvePositionComboBoxOptions;

        public EightPositionState SelectedPosition
        {
            get { return selectedPosition; }
            set { this.RaiseAndSetIfChanged(ref selectedPosition, value); }
        }

        public string State
        {
            get { return state; }
            set { this.RaiseAndSetIfChanged(ref state, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshCommand { get; private set; }

        public UserControl GetDefaultView()
        {
            return new DemoValve2AdvancedControlView();
        }

        public void Set()
        {
            valve.SetPosition(SelectedPosition);
        }

        public void Refresh()
        {
            State = ((EightPositionState)valve.GetPosition()).ToString();
        }
    }
}
