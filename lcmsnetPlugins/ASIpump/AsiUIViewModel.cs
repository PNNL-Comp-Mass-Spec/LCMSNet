using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using ReactiveUI;

namespace ASIpump
{
    public class AsiUIViewModel : ReactiveObject, IDeviceControlWpf
    {
        public AsiUIViewModel()
        {
            SetupCommands();
        }

        public event DelegateSaveRequired SaveRequired;
        public event DelegateNameChanged NameChanged;

        private AsiPump pump;
        private string name = "";
        private string pumpLog = "";

        public AsiPump Pump
        {
            get { return pump; }
            set { this.RaiseAndSetIfChanged(ref pump, value); }
        }

        public IDevice Device
        {
            get { return pump; }
            set { RegisterDevice(value); }
        }

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public string PumpLog
        {
            get { return pumpLog; }
            set { this.RaiseAndSetIfChanged(ref pumpLog, value); }
        }

        public bool Running { get; set; }

        public UserControl GetDefaultView()
        {
            return new AsiUIView();
        }

        private void RegisterDevice(IDevice value)
        {
            pump = value as AsiPump;
            if (pump != null)
            {
                Pump.MessageStreamed += Pump_MessageStreamed;
                Pump.MessageSent += Pump_MessageSent;
            }
        }

        public void Pump_MessageStreamed(string message)
        {
            PumpLog += message + "\r\n";
        }

        public void Pump_MessageSent(string message)
        {
            PumpLog += message + "\r\n";
        }

        private void Connect()
        {
            var errMsg = "";

            Pump.Initialize(ref errMsg);
        }

        private void Run()
        {
            Pump.StartProgram();
        }

        private void Abort()
        {
            Pump.Escape();
        }

        private void GetPositionA()
        {
            Pump.Send("Apr p");
        }

        private void GetPositionB()
        {
            Pump.Send("Bpr p");
        }

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RunCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AbortCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> GetPosACommand { get; private set; }
        public ReactiveCommand<Unit, Unit> GetPosBCommand { get; private set; }

        private void SetupCommands()
        {
            ConnectCommand = ReactiveCommand.Create(() => Connect());
            RunCommand = ReactiveCommand.Create(() => Run());
            AbortCommand = ReactiveCommand.Create(() => Abort());
            GetPosACommand = ReactiveCommand.Create(() => GetPositionA());
            GetPosBCommand = ReactiveCommand.Create(() => GetPositionB());
        }
    }
}
