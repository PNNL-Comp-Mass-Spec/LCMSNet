using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using LcmsNetData.Logging;
using LcmsNetPlugins.Agilent.Pumps;
using ReactiveUI;

namespace AgilentPumpExe
{
    public class MainWindowViewModel : ReactiveObject, IDisposable
    {
        private string status = "uninitialized";
        private bool initialized = false;
        private ReactiveList<string> statusHistory = new ReactiveList<string>();

        public AgilentPumpViewModel AgilentPumpVm { get; } = new AgilentPumpViewModel();
        public AgilentPump AgilentPump { get; } = new AgilentPump(true);
        public ReactiveCommand<Unit, Unit> InitializePumpCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearStatusHistoryCommand { get; }

        public string Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }

        public bool Initialized
        {
            get => initialized;
            set => this.RaiseAndSetIfChanged(ref initialized, value);
        }

        public IReadOnlyReactiveList<string> StatusHistory => statusHistory;

        public MainWindowViewModel()
        {
            AgilentPump.DisableMonitoring();
            AgilentPumpVm.Device = AgilentPump;

            InitializePumpCommand = ReactiveCommand.Create(InitializePump, this.WhenAnyValue(x => x.Initialized).Select(x => !x));
            ApplicationLogger.Error += ApplicationLoggerOnError;
            ApplicationLogger.Message += ApplicationLoggerOnMessage;
        }

        private void ApplicationLoggerOnError(int errorLevel, ErrorLoggerArgs args)
        {
            // TODO: Add exception unwind
            RxApp.MainThreadScheduler.Schedule(() => AddStatusHistoryItem("ERROR", args.Message));
        }

        private void ApplicationLoggerOnMessage(int messageLevel, MessageLoggerArgs args)
        {
            RxApp.MainThreadScheduler.Schedule(() => AddStatusHistoryItem("INFO", args.Message));
        }

        private void AddStatusHistoryItem(string type, string message)
        {
            statusHistory.Add($"{type}:   {message}");
        }

        public void InitializePump()
        {
            var errors = "";
            AgilentPump.Initialize(ref errors);
            Status = errors;
            if (string.IsNullOrWhiteSpace(errors))
            {
                initialized = true;
            }
        }

        public void ClearStatusHistory()
        {
            using (statusHistory.SuppressChangeNotifications())
            {
                statusHistory.Clear();
            }
        }

        public void Dispose()
        {
            AgilentPumpVm?.Dispose();
            AgilentPump?.Dispose();
            InitializePumpCommand?.Dispose();
            ClearStatusHistoryCommand?.Dispose();
        }
    }
}
