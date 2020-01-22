using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.ASIpump
{
    public class AsiUIViewModel : ReactiveObject, IDeviceControl
    {
        public AsiUIViewModel()
        {
            ConnectCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Connect));
            RunCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Run));
            AbortCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(Abort));
            GetPosACommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(GetPositionA));
            GetPosBCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(GetPositionB));
        }

#pragma warning disable 67
        public event Action SaveRequired;
        public event EventHandler<string> NameChanged;
#pragma warning restore 67

        private AsiPump pump;
        private string name = "";
        private string pumpLog = "";

        public AsiPump Pump
        {
            get => pump;
            set => this.RaiseAndSetIfChanged(ref pump, value);
        }

        public IDevice Device
        {
            get => pump;
            set => RegisterDevice(value);
        }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string PumpLog
        {
            get => pumpLog;
            set => this.RaiseAndSetIfChanged(ref pumpLog, value);
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

        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public ReactiveCommand<Unit, Unit> AbortCommand { get; }
        public ReactiveCommand<Unit, Unit> GetPosACommand { get; }
        public ReactiveCommand<Unit, Unit> GetPosBCommand { get; }
    }
}
