using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace LcmsNetPlugins.Agilent.Pumps
{
    public class AgilentPumpStatusViewModel : ReactiveObject
    {
        private AgilentPumpStatus pumpStatus;
        private readonly ObservableAsPropertyHelper<string> boardTemperatureC;
        private readonly ObservableAsPropertyHelper<string> leakSensorCurrentMa;
        private readonly ObservableAsPropertyHelper<string> leakState;
        private readonly ObservableAsPropertyHelper<AgilentPumpStateGeneric> genericState;
        private readonly ObservableAsPropertyHelper<AgilentPumpStateAnalysis> analysisState;
        private readonly ObservableAsPropertyHelper<AgilentPumpStateError> errorState;
        private readonly ObservableAsPropertyHelper<AgilentPumpStateNotReady> notReadyState;
        private readonly ObservableAsPropertyHelper<AgilentPumpStateTest> testState;
        private readonly ObservableAsPropertyHelper<AgilentPumpNotReadyStates> notReadyReasons;
        private readonly ObservableAsPropertyHelper<AgilentPumpStartNotReadyStates> startNotReadyReasons;
        private readonly ObservableAsPropertyHelper<int> notReadyReasonsInt;
        private readonly ObservableAsPropertyHelper<string> notReadyReasonsString;
        private readonly ObservableAsPropertyHelper<int> startNotReadyReasonsInt;
        private readonly ObservableAsPropertyHelper<string> startNotReadyReasonsString;

        public AgilentPumpStatusViewModel()
        {
            boardTemperatureC = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.BoardTemperatureC).Select(x => x.Item2).ToProperty(this, nameof(BoardTemperatureC), "", true, RxApp.MainThreadScheduler);
            leakSensorCurrentMa = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.LeakSensorCurrentMa).Select(x => x.Item2).ToProperty(this, nameof(LeakSensorCurrentMa), "", true, RxApp.MainThreadScheduler);
            leakState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.LeakState).Select(x => x.Item2).ToProperty(this, nameof(LeakState), "", true, RxApp.MainThreadScheduler);
            genericState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.GenericState).Select(x => x.Item2).ToProperty(this, nameof(GenericState), AgilentPumpStateGeneric.SHUTDOWN, true, RxApp.MainThreadScheduler);
            analysisState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.AnalysisState).Select(x => x.Item2).ToProperty(this, nameof(AnalysisState), AgilentPumpStateAnalysis.NO_ANALYSIS, true, RxApp.MainThreadScheduler);
            errorState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.ErrorState).Select(x => x.Item2).ToProperty(this, nameof(ErrorState), AgilentPumpStateError.NO_ERROR, true, RxApp.MainThreadScheduler);
            notReadyState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.NotReadyState).Select(x => x.Item2).ToProperty(this, nameof(NotReadyState), AgilentPumpStateNotReady.NOT_READY, true, RxApp.MainThreadScheduler);
            testState = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.TestState).Select(x => x.Item2).ToProperty(this, nameof(TestState), AgilentPumpStateTest.NO_TEST, true, RxApp.MainThreadScheduler);
            notReadyReasons = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.NotReadyReasons).Select(x => x.Item2).ToProperty(this, nameof(NotReadyReasons), AgilentPumpNotReadyStates.Shutdown, true, RxApp.MainThreadScheduler);
            startNotReadyReasons = this.WhenAnyValue(x => x.PumpStatus, x => x.PumpStatus.StartNotReadyReasons).Select(x => x.Item2).ToProperty(this, nameof(StartNotReadyReasons), AgilentPumpStartNotReadyStates.Shutdown, true, RxApp.MainThreadScheduler);

            notReadyReasonsInt = this.WhenAnyValue(x => x.NotReadyReasons).Select(x => (int)x).ToProperty(this, x => x.NotReadyReasonsInt);
            notReadyReasonsString = this.WhenAnyValue(x => x.NotReadyReasons).Select(GetFlags).ToProperty(this, x => x.NotReadyReasonsString);
            startNotReadyReasonsInt = this.WhenAnyValue(x => x.StartNotReadyReasons).Select(x => (int)x).ToProperty(this, x => x.StartNotReadyReasonsInt);
            startNotReadyReasonsString = this.WhenAnyValue(x => x.StartNotReadyReasons).Select(GetFlags).ToProperty(this, x => x.StartNotReadyReasonsString);
        }

        public AgilentPumpStatus PumpStatus
        {
            get => pumpStatus;
            set => this.RaiseAndSetIfChanged(ref pumpStatus, value);
        }

        public string BoardTemperatureC => boardTemperatureC?.Value ?? "";
        public string LeakSensorCurrentMa => leakSensorCurrentMa?.Value ?? "";
        public string LeakState => leakState?.Value ?? "";
        public AgilentPumpStateGeneric GenericState => genericState?.Value ?? AgilentPumpStateGeneric.SHUTDOWN;
        public AgilentPumpStateAnalysis AnalysisState => analysisState?.Value ?? AgilentPumpStateAnalysis.NO_ANALYSIS;
        public AgilentPumpStateError ErrorState => errorState?.Value ?? AgilentPumpStateError.NO_ERROR;
        public AgilentPumpStateNotReady NotReadyState => notReadyState?.Value ?? AgilentPumpStateNotReady.NOT_READY;
        public AgilentPumpStateTest TestState => testState?.Value ?? AgilentPumpStateTest.NO_TEST;
        public AgilentPumpNotReadyStates NotReadyReasons => notReadyReasons?.Value ?? AgilentPumpNotReadyStates.Shutdown;
        public AgilentPumpStartNotReadyStates StartNotReadyReasons => startNotReadyReasons?.Value ?? AgilentPumpStartNotReadyStates.Shutdown;
        public int NotReadyReasonsInt => notReadyReasonsInt?.Value ?? 0;
        public string NotReadyReasonsString => notReadyReasonsString?.Value ?? "";
        public int StartNotReadyReasonsInt => startNotReadyReasonsInt?.Value ?? 0;
        public string StartNotReadyReasonsString => startNotReadyReasonsString?.Value ?? "";

        private string GetFlags(AgilentPumpNotReadyStates enumValue)
        {
            GetFlags(enumValue, out var existing, out var nonExisting);
            if (nonExisting > 0)
            {
                return $"{((AgilentPumpNotReadyStates)existing).ToString()},{nonExisting}";
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
