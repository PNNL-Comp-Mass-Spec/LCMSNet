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
            get { return isTimeConversion; }
            set { this.RaiseAndSetIfChanged(ref isTimeConversion, value); }
        }

        public bool IsDisplayPrecision
        {
            get { return isDisplayPrecision; }
            set { this.RaiseAndSetIfChanged(ref isDisplayPrecision, value); }
        }

        public int Minutes
        {
            get { return minutes; }
            set { this.RaiseAndSetIfChanged(ref minutes, value); }
        }

        public int Seconds
        {
            get { return seconds; }
            set { this.RaiseAndSetIfChanged(ref seconds, value); }
        }

        public int DecimalPlaces
        {
            get { return decimalPlaces; }
            set { this.RaiseAndSetIfChanged(ref decimalPlaces, value); }
        }

        public int TotalSeconds
        {
            get { return Minutes * 60 + Seconds; }
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
