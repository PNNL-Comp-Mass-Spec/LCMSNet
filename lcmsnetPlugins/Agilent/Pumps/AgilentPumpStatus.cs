using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpStatus : ReactiveObject
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
        private readonly ObservableAsPropertyHelper<int> notReadyReasonsInt;
        private readonly ObservableAsPropertyHelper<string> notReadyReasonsString;
        private readonly ObservableAsPropertyHelper<int> startNotReadyReasonsInt;
        private readonly ObservableAsPropertyHelper<string> startNotReadyReasonsString;

        public AgilentPumpStatus()
        {
            notReadyReasonsInt = this.WhenAnyValue(x => x.NotReadyReasons).Select(x => (int) x).ToProperty(this, x => x.NotReadyReasonsInt);
            notReadyReasonsString = this.WhenAnyValue(x => x.NotReadyReasons).Select(GetFlags).ToProperty(this, x => x.NotReadyReasonsString);
            startNotReadyReasonsInt = this.WhenAnyValue(x => x.StartNotReadyReasons).Select(x => (int)x).ToProperty(this, x => x.StartNotReadyReasonsInt);
            startNotReadyReasonsString = this.WhenAnyValue(x => x.StartNotReadyReasons).Select(GetFlags).ToProperty(this, x => x.StartNotReadyReasonsString);
        }

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

        public int NotReadyReasonsInt => notReadyReasonsInt?.Value ?? 0;
        public string NotReadyReasonsString => notReadyReasonsString?.Value ?? "";

        public AgilentPumpStartNotReadyStates StartNotReadyReasons
        {
            get => startNotReadyReasons;
            set => this.RaiseAndSetIfChanged(ref startNotReadyReasons, value);
        }

        public int StartNotReadyReasonsInt => startNotReadyReasonsInt?.Value ?? 0;
        public string StartNotReadyReasonsString => startNotReadyReasonsString?.Value ?? "";

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

        private string GetFlags(AgilentPumpNotReadyStates enumValue)
        {
            GetFlags(enumValue, out var existing, out var nonExisting);
            if (nonExisting > 0)
            {
                return $"{((AgilentPumpNotReadyStates) existing).ToString()},{nonExisting}";
            }

            return enumValue.ToString();
        }

        private string GetFlags(AgilentPumpStartNotReadyStates enumValue)
        {
            GetFlags(enumValue, out var existing, out var nonExisting);
            if (existing > 0 && nonExisting > 0)
            {
                return $"{((AgilentPumpStartNotReadyStates)existing).ToString()},{nonExisting}";
            }
            if (nonExisting > 0)
            {
                return nonExisting.ToString();
            }

            return enumValue.ToString();
        }

        private void GetFlags(Enum enumValue, out int existing, out int nonExisting)
        {
            existing = 0;
            nonExisting = 0;
            var intValue = Convert.ToInt32(enumValue);
            if (intValue == 0)
            {
                return;
            }
            var allValues = Enum.GetValues(enumValue.GetType()).Cast<Enum>().ToList();
            var allValuesValue = 0;
            foreach (var value in allValues)
            {
                allValuesValue |= Convert.ToInt32(value);
            }

            nonExisting = ~allValuesValue & intValue;
            existing = allValuesValue & intValue;
        }
    }
}
