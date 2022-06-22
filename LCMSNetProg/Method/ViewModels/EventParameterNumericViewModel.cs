using System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterNumericViewModel : ReactiveObject, ILCEventParameter
    {
        public EventParameterNumericViewModel(Type numberType)
        {
            NumberMinimum = 0.0;
            NumberMaximum = 10000000000.0;
            NumberType = numberType;

            CanShowDecimals = NumberType == typeof(double) || NumberType == typeof(float);
        }

        private string parameterLabel = "";
        private double numberValue;
        private double numberMinimum;
        private double numberMaximum;
        private int decimalPlaces;

        public Type NumberType { get; }

        public string ParameterLabel
        {
            get => parameterLabel;
            set => this.RaiseAndSetIfChanged(ref parameterLabel, value);
        }

        public object ParameterValue
        {
            get => Convert.ChangeType(NumberValue, NumberType);
            set
            {
                if (value == null)
                {
                    NumberValue = 0;
                    return;
                }

                NumberValue = Convert.ToDouble(value);
            }
        }

        public double NumberValue
        {
            get => numberValue;
            set
            {
                var oldValue = numberValue;
                this.RaiseAndSetIfChanged(ref numberValue, value);
                if (!Equals(oldValue, numberValue))
                {
                    OnEventChanged();
                }
            }
        }

        public double NumberMinimum
        {
            get => numberMinimum;
            set => this.RaiseAndSetIfChanged(ref numberMinimum, value);
        }

        public double NumberMaximum
        {
            get => numberMaximum;
            set => this.RaiseAndSetIfChanged(ref numberMaximum, value);
        }

        public bool CanShowDecimals { get; }

        public int DecimalPlaces
        {
            get => decimalPlaces;
            set
            {
                var oldValue = decimalPlaces;
                if (!CanShowDecimals)
                {
                    value = 0;
                }
                this.RaiseAndSetIfChanged(ref decimalPlaces, value);
                if (oldValue != decimalPlaces)
                {
                    this.RaisePropertyChanged(nameof(NumberFormat));
                    this.RaisePropertyChanged(nameof(Increment));
                }
            }
        }

        public string NumberFormat => $"F{DecimalPlaces}";

        public double Increment => 1.0 / Math.Pow(10, DecimalPlaces);

        public event EventHandler EventChanged;

        public ILCEventParameter CreateDuplicate()
        {
            return new EventParameterNumericViewModel(NumberType);
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
        }
    }
}
