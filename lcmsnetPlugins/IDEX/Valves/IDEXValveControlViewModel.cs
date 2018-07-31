using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.IDEX.Valves
{
    public class IDEXValveControlViewModel : ReactiveObject, IDeviceControl
    {
        public IDEXValveControlViewModel()
        {
            InjectFailureCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => InjectFailure()));
            InjectStatusCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Console.WriteLine(@"Not Implemented!")));
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
#pragma warning disable 67
        public event EventHandler<string> NameChanged;
        public event Action SaveRequired;
#pragma warning restore 67

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
