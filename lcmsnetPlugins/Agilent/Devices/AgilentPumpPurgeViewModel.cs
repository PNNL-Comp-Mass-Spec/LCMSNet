using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace Agilent.Devices.Pumps
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
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pump"></param>
        public AgilentPumpPurgeViewModel(classPumpAgilent pump)
        {
            m_pump = pump;
            pump.DeviceSaveRequired += pump_DeviceSaveRequired;
            Title = "Purge Pumps " + m_pump.Name;
            SetupCommands();
        }

        /// <summary>
        /// Pump to purge.
        /// </summary>
        private readonly classPumpAgilent m_pump;

        private string title = "";
        private double a1Duration = 10;
        private double a1FlowRate = 500;
        private double a2Duration = 10;
        private double a2FlowRate = 500;
        private double b1Duration = 10;
        private double b1FlowRate = 500;
        private double b2Duration = 10;
        private double b2FlowRate = 500;

        public string Title
        {
            get { return title; }
            set { this.RaiseAndSetIfChanged(ref title, value); }
        }

        public double A1Duration
        {
            get { return a1Duration; }
            set { this.RaiseAndSetIfChanged(ref a1Duration, value); }
        }

        public double A1FlowRate
        {
            get { return a1FlowRate; }
            set { this.RaiseAndSetIfChanged(ref a1FlowRate, value); }
        }

        public double A2Duration
        {
            get { return a2Duration; }
            set { this.RaiseAndSetIfChanged(ref a2Duration, value); }
        }

        public double A2FlowRate
        {
            get { return a2FlowRate; }
            set { this.RaiseAndSetIfChanged(ref a2FlowRate, value); }
        }

        public double B1Duration
        {
            get { return b1Duration; }
            set { this.RaiseAndSetIfChanged(ref b1Duration, value); }
        }

        public double B1FlowRate
        {
            get { return b1FlowRate; }
            set { this.RaiseAndSetIfChanged(ref b1FlowRate, value); }
        }

        public double B2Duration
        {
            get { return b2Duration; }
            set { this.RaiseAndSetIfChanged(ref b2Duration, value); }
        }

        public double B2FlowRate
        {
            get { return b2FlowRate; }
            set { this.RaiseAndSetIfChanged(ref b2FlowRate, value); }
        }

        void pump_DeviceSaveRequired(object sender, EventArgs e)
        {
            Title = "Purge Pumps " + m_pump.Name;
        }

        public ReactiveCommand<Unit, bool> PurgeA1Command { get; private set; }
        public ReactiveCommand<Unit, bool> PurgeA2Command { get; private set; }
        public ReactiveCommand<Unit, bool> PurgeB1Command { get; private set; }
        public ReactiveCommand<Unit, bool> PurgeB2Command { get; private set; }
        public ReactiveCommand<Unit, bool> AbortPurgesCommand { get; private set; }

        private void SetupCommands()
        {
            PurgeA1Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => m_pump.PurgePump(0, enumPurgePumpChannel.A1, A1FlowRate, A1Duration)));
            PurgeA2Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => m_pump.PurgePump(0, enumPurgePumpChannel.A2, A2FlowRate, A2Duration)));
            PurgeB1Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => m_pump.PurgePump(0, enumPurgePumpChannel.B1, B1FlowRate, B1Duration)));
            PurgeB2Command = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => m_pump.PurgePump(0, enumPurgePumpChannel.B2, B2FlowRate, B2Duration)));
            AbortPurgesCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => m_pump.AbortPurges(0)));
        }
    }
}
