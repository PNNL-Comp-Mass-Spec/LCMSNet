using System.Reactive;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace ASUTGen.Devices.Valves
{
    public class IDEXValveViewModel : BaseDeviceControlViewModelReactive
    {
        public IDEXValveViewModel()
        {
            InjectFailureCommand = ReactiveCommand.Create(InjectFailure);
            InjectStatusCommand = ReactiveCommand.Create(InjectStatus);
        }

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXValve mobj_valve;


        public override IDevice Device
        {
            get => mobj_valve;
            set => RegisterDevice(value);
        }

        public ReactiveCommand<Unit, Unit> InjectFailureCommand { get; }
        public ReactiveCommand<Unit, Unit> InjectStatusCommand { get; }

        public override UserControl GetDefaultView()
        {
            return new IDEXValveView();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_valve = device as IDEXValve;
        }

        private void InjectFailure()
        {
            mobj_valve.ChangePosition(100, 100);
        }

        private void InjectStatus()
        {

        }
    }
}
