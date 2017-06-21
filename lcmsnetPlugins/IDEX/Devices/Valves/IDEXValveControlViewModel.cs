using System;
using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using ReactiveUI;

namespace ASUTGen.Devices.Valves
{
    public class IDEXValveControlViewModel : ReactiveObject, IDeviceControlWpf
    {
        public IDEXValveControlViewModel()
        {
            InjectFailureCommand = ReactiveCommand.Create(() => InjectFailure());
            InjectStatusCommand = ReactiveCommand.Create(() => Console.WriteLine(@"Not Implemented!"));
        }

        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXValve m_valve;

        private string name;

        private void InjectFailure()
        {
            m_valve.ChangePosition(100, 100);
        }

        public void RegisterDevice(IDevice device)
        {
            m_valve = device as IDEXValve;
        }

        public ReactiveCommand<Unit, Unit> InjectFailureCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> InjectStatusCommand { get; private set; }

        #region IDeviceControl Members

        public event DelegateNameChanged NameChanged;
        public event DelegateSaveRequired SaveRequired;

        public bool Running { get; set; }

        public IDevice Device
        {
            get { return m_valve; }
            set { RegisterDevice(value); }
        }

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public UserControl GetDefaultView()
        {
            return new IDEXValveControlView();
        }

        public void ShowProps()
        {

        }

        #endregion
    }
}
