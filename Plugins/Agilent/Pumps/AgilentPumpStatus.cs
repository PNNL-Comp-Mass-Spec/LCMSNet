using System.ComponentModel;
using LcmsNetSDK;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpStatus : INotifyPropertyChangedExt
    {
        private string boardTemperatureC;
        private string leakSensorCurrentMa;
        private string leakState;
        private AgilentPumpStateGeneric genericState;
        private AgilentPumpStateAnalysis analysisState;
        private AgilentPumpStateError errorState;
        private AgilentPumpStateNotReady notReadyState;
        private AgilentPumpStateTest testState;
        private AgilentPumpNotReadyStates notReadyReasons;
        private AgilentPumpStartNotReadyStates startNotReadyReasons;

        public string BoardTemperatureC
        {
            get => boardTemperatureC;
            set => this.RaiseAndSetIfChanged(ref boardTemperatureC, value);
        }

        public string LeakSensorCurrentMa
        {
            get => leakSensorCurrentMa;
            set => this.RaiseAndSetIfChanged(ref leakSensorCurrentMa, value);
        }

        public string LeakState
        {
            get => leakState;
            set => this.RaiseAndSetIfChanged(ref leakState, value);
        }

        public AgilentPumpStateGeneric GenericState
        {
            get => genericState;
            set => this.RaiseAndSetIfChanged(ref genericState, value);
        }

        public AgilentPumpStateAnalysis AnalysisState
        {
            get => analysisState;
            set => this.RaiseAndSetIfChanged(ref analysisState, value);
        }

        public AgilentPumpStateError ErrorState
        {
            get => errorState;
            set => this.RaiseAndSetIfChanged(ref errorState, value);
        }

        public AgilentPumpStateNotReady NotReadyState
        {
            get => notReadyState;
            set => this.RaiseAndSetIfChanged(ref notReadyState, value);
        }

        public AgilentPumpStateTest TestState
        {
            get => testState;
            set => this.RaiseAndSetIfChanged(ref testState, value);
        }

        public AgilentPumpNotReadyStates NotReadyReasons
        {
            get => notReadyReasons;
            set => this.RaiseAndSetIfChanged(ref notReadyReasons, value);
        }

        public AgilentPumpStartNotReadyStates StartNotReadyReasons
        {
            get => startNotReadyReasons;
            set => this.RaiseAndSetIfChanged(ref startNotReadyReasons, value);
        }

        public void UpdateValues(AgilentPumpStatus other)
        {
            BoardTemperatureC = other.BoardTemperatureC;
            LeakSensorCurrentMa = other.LeakSensorCurrentMa;
            LeakState = other.LeakState;
            GenericState = other.GenericState;
            AnalysisState = other.AnalysisState;
            ErrorState = other.ErrorState;
            NotReadyState = other.NotReadyState;
            TestState = other.TestState;
            NotReadyReasons = other.NotReadyReasons;
            StartNotReadyReasons = other.StartNotReadyReasons;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
