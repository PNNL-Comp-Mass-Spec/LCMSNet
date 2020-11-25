using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetData;
using LcmsNetSDK.Devices;

namespace DemoPluginLibrary
{
    public class DemoValve2AdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        private DemoValve2 valve;

        public DemoValve2AdvancedControlViewModel()
        {
            ValvePositionComboBoxOptions = Enum.GetValues(typeof(EightPositionState)).Cast<EightPositionState>().Where(x => x != EightPositionState.Unknown).ToList().AsReadOnly();
            SetCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Set()));
            RefreshCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Refresh()));
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
                valve = value as DemoValve2;
                SetBaseDevice(value);
            }
        }

        private EightPositionState selectedPosition;
        private string state = "";

        public ReadOnlyCollection<EightPositionState> ValvePositionComboBoxOptions { get; }

        public EightPositionState SelectedPosition
        {
            get => selectedPosition;
            set => this.RaiseAndSetIfChanged(ref selectedPosition, value);
        }

        public string State
        {
            get => state;
            set => this.RaiseAndSetIfChanged(ref state, value);
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SetCommand { get; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshCommand { get; }

        public override UserControl GetDefaultView()
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
