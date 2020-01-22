using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class ConvertToolViewModel : ReactiveObject
    {
        public ConvertToolViewModel()
        {
        }

        public ConvertToolViewModel(int seconds, int precision)
        {
            TotalSeconds = seconds;

            DecimalPlaces = precision;
        }

        private bool isTimeConversion;
        private bool isDisplayPrecision;
        private int minutes;
        private int seconds;
        private int decimalPlaces;

        public bool IsTimeConversion
        {
            get => isTimeConversion;
            set => this.RaiseAndSetIfChanged(ref isTimeConversion, value);
        }

        public bool IsDisplayPrecision
        {
            get => isDisplayPrecision;
            set => this.RaiseAndSetIfChanged(ref isDisplayPrecision, value);
        }

        public int Minutes
        {
            get => minutes;
            set => this.RaiseAndSetIfChanged(ref minutes, value);
        }

        public int Seconds
        {
            get => seconds;
            set => this.RaiseAndSetIfChanged(ref seconds, value);
        }

        public int DecimalPlaces
        {
            get => decimalPlaces;
            set => this.RaiseAndSetIfChanged(ref decimalPlaces, value);
        }

        public int TotalSeconds
        {
            get => Minutes * 60 + Seconds;
            set
            {
                Minutes = value / 60;
                Seconds = value % 60;
            }
        }

        public ConversionType ConversionType
        {
            get
            {
                if (IsTimeConversion)
                {
                    return ConversionType.Time;
                }
                return ConversionType.Precision;
            }
        }
    }

    public enum ConversionType
    {
        Time,
        Precision
    }
}
