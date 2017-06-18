using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace DemoPluginLibrary
{
    public class DemoPALAdvancedControlViewModel : BaseDeviceControlViewModel, IDeviceControlWpf
    {
        private DemoPAL m_PALdevice;

        public DemoPALAdvancedControlViewModel()
        {
            RunCommand = ReactiveUI.ReactiveCommand.Create(() => Run());
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
                return m_PALdevice;
            }
            set
            {
                m_PALdevice = value as DemoPAL;
                SetBaseDevice(value);
            }
        }

        private readonly ReactiveUI.ReactiveList<string> methodsComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private string selectedMethod = "";
        private double timeout = 0;

        public ReactiveUI.ReactiveList<string> MethodsComboBoxOptions => methodsComboBoxOptions;

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public double Timeout
        {
            get { return timeout; }
            set { this.RaiseAndSetIfChanged(ref timeout, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> RunCommand { get; private set; }

        public UserControl GetDefaultView()
        {
            return new DemoPALAdvancedControlView();
        }

        private void Run()
        {
            // use a defaulted sampledata object since there's no sample associated with a user clicking "run"
            m_PALdevice.RunMethod(Timeout, new LcmsNetDataClasses.classSampleData(), SelectedMethod);
        }
    }
}
