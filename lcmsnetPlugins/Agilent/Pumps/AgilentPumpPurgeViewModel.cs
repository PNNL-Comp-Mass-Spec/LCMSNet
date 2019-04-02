using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpPurgeViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public AgilentPumpPurgeViewModel()
        {
            Title = "Purge Pumps Unknown";
            SetupCommands();
            Pump = new AgilentPump(true);
            Pump.Emulation = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pump"></param>
        public AgilentPumpPurgeViewModel(AgilentPump pump)
        {
            Pump = pump;
            Pump.DeviceSaveRequired += Pump_DeviceSaveRequired;
            Title = "Purge Pumps " + Pump.Name;
            SetupCommands();
        }

        /// <summary>
        /// Pump to purge.
        /// </summary>
        public AgilentPump Pump { get; }

        private string title = "";
        private AgilentPumpReplyErrorCodes pumpError;

        public PumpPurgeData ChannelA1 => Pump.PurgeA1;
        public PumpPurgeData ChannelA2 => Pump.PurgeA2;
        public PumpPurgeData ChannelB1 => Pump.PurgeB1;
        public PumpPurgeData ChannelB2 => Pump.PurgeB2;

        public string Title
        {
            get { return title; }
            set { this.RaiseAndSetIfChanged(ref title, value); }
        }

        public AgilentPumpReplyErrorCodes PumpError
        {
            get => pumpError;
            set => this.RaiseAndSetIfChanged(ref pumpError, value);
        }

        // In case the name was changed, trigger an update of the title bar
        private void Pump_DeviceSaveRequired(object sender, EventArgs e)
        {
            Title = "Purge Pumps " + Pump.Name;
        }

        public ReactiveCommand<Unit, bool> SetA1Command { get; private set; }
        public ReactiveCommand<Unit, bool> SetA2Command { get; private set; }
        public ReactiveCommand<Unit, bool> SetB1Command { get; private set; }
        public ReactiveCommand<Unit, bool> SetB2Command { get; private set; }
        public ReactiveCommand<Unit, AgilentPumpReplyErrorCodes> AbortPurgesCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpOnCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpOffCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PumpStandbyCommand { get; private set; }
        public ReactiveCommand<Unit, bool> RefreshPurgeSettingsCommand { get; private set; }
        public ReactiveCommand<Unit, AgilentPumpReplyErrorCodes> PurgeCommand { get; private set; }

        private void SetupCommands()
        {
            SetA1Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPurgeData(ChannelA1)));
            SetA2Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPurgeData(ChannelA2)));
            SetB1Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPurgeData(ChannelB1)));
            SetB2Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.SetPurgeData(ChannelB2)));
            AbortPurgesCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.AbortPurges(0)));
            PumpOnCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOn()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.On));
            PumpOffCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpOff()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.Off));
            PumpStandbyCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.PumpStandby()), this.WhenAnyValue(x => x.Pump.PumpState).Select(x => x != PumpState.Standby));
            RefreshPurgeSettingsCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.LoadPurgeData()));
            PurgeCommand = ReactiveCommand.CreateFromTask(StartPumpPurge, this.WhenAnyValue(x => x.Pump.PumpState, x => x.Pump.PumpStatus.NotReadyState, x => x.Pump.PumpStatus.NotReadyReasons)
                .Select(x => x.Item1 != PumpState.Off && x.Item1 != PumpState.Standby && (x.Item2 == AgilentPumpStateNotReady.READY || x.Item3.HasFlag(AgilentPumpNotReadyStates.Flow_Init))));
        }

        private async Task<AgilentPumpReplyErrorCodes> StartPumpPurge()
        {
            var result = await Task.Run(() => Pump.StartPurge());
            RxApp.MainThreadScheduler.Schedule(() => PumpError = result);
            return result;
        }
    }
}
