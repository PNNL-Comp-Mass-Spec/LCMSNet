using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class PumpPurgeData : ReactiveObject
    {
        private bool enabled;
        private double flowRate = 500;
        private double duration = 10;

        public const double MaxPurgeFlowRate = 2500;
        public const double MaxPurgeDuration = 99999;

        public PumpPurgeData(PumpPurgeChannel channel)
        {
            Channel = channel;
        }

        public PumpPurgeChannel Channel { get; }

        public bool Enabled
        {
            get => enabled;
            set => this.RaiseAndSetIfChanged(ref enabled, value);
        }

        public double MaxFlowRate => MaxPurgeFlowRate;

        public double FlowRate
        {
            get => flowRate;
            set => this.RaiseAndSetIfChanged(ref flowRate, value);
        }

        public double MaxDuration => MaxPurgeDuration;

        public double Duration
        {
            get => duration;
            set => this.RaiseAndSetIfChanged(ref duration, value);
        }
    }
}
