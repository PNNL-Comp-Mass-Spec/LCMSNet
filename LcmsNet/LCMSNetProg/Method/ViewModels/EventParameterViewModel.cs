using System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterViewModel : ReactiveObject, ILCEventParameter
    {
        public enum ParameterTypeEnum
        {
            Enum,
            Numeric,
            Text,
        }

        /// <summary>
        /// Default constructor for the event parameter view model that takes no arguments
        /// Calling this constructor is only for the IDE designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public EventParameterViewModel()
        {
        }

        public EventParameterViewModel(ParameterTypeEnum type)
        {
            ParameterType = type;
            NumberMinimum = 0.0;
            NumberMaximum = 10000000000.0;
            WinFormsParameter = null;
        }

        private string parameterLabel = "";

        public ParameterTypeEnum ParameterType { get; private set; }
        public bool ShowComboBox { get { return ParameterType == ParameterTypeEnum.Enum; } }
        public bool ShowNumericUpDown { get { return ParameterType == ParameterTypeEnum.Numeric; } }
        public bool ShowTextBox { get { return ParameterType == ParameterTypeEnum.Text; } }

        public event EventHandler EventChanged;

        /// <summary>
        /// Reference to the WinForms parameter to properly update the values everywhere else
        /// </summary>
        public ILCEventParameter WinFormsParameter { get; set; }

        public string ParameterLabel
        {
            get { return parameterLabel; }
            set { this.RaiseAndSetIfChanged(ref parameterLabel, value); }
        }

        public object ParameterValue
        {
            get
            {
                switch (ParameterType)
                {
                    case ParameterTypeEnum.Enum:
                        return SelectedOption;
                    case ParameterTypeEnum.Numeric:
                        return NumberValue;
                    case ParameterTypeEnum.Text:
                        return TextValue;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    SelectedOption = null;
                    TextValue = null;
                    NumberValue = 0;
                    return;
                }
                switch (ParameterType)
                {
                    case ParameterTypeEnum.Enum:
                        SelectedOption = value;
                        break;
                    case ParameterTypeEnum.Numeric:
                        NumberValue = Convert.ToDouble(value);
                        break;
                    case ParameterTypeEnum.Text:
                        TextValue = value.ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
            if (WinFormsParameter != null)
            {
                WinFormsParameter.ParameterValue = ParameterValue;
            }
        }

        #region ComboBox settings

        private object selectedOption;
        private readonly ReactiveList<object> comboBoxOptions = new ReactiveList<object>();

        public object SelectedOption
        {
            get { return selectedOption; }
            set
            {
                var oldValue = selectedOption;
                this.RaiseAndSetIfChanged(ref selectedOption, value);
                if (!Equals(oldValue, selectedOption))
                {
                    OnEventChanged();
                }
            }
        }

        public ReactiveList<object> ComboBoxOptions => comboBoxOptions;

        #endregion

        #region TextBox settings

        private string textValue;

        public string TextValue
        {
            get { return textValue; }
            set
            {
                var oldValue = textValue;
                this.RaiseAndSetIfChanged(ref textValue, value);
                if (!Equals(oldValue, textValue))
                {
                    OnEventChanged();
                }
            }
        }

        #endregion

        #region NumericUpDown settings

        private double numberValue;
        private double numberMinimum;
        private double numberMaximum;
        private int decimalPlaces;

        public double NumberValue
        {
            get { return numberValue; }
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
            get { return numberMinimum; }
            set { this.RaiseAndSetIfChanged(ref numberMinimum, value); }
        }

        public double NumberMaximum
        {
            get { return numberMaximum; }
            set { this.RaiseAndSetIfChanged(ref numberMaximum, value); }
        }

        public int DecimalPlaces
        {
            get { return decimalPlaces; }
            set
            {
                var oldValue = decimalPlaces;
                this.RaiseAndSetIfChanged(ref decimalPlaces, value);
                if (oldValue != decimalPlaces)
                {
                    this.RaisePropertyChanged(nameof(NumberFormat));
                    this.RaisePropertyChanged(nameof(Increment));
                }
            }
        }

        public string NumberFormat
        {
            get { return $"F{DecimalPlaces}"; }
        }

        public double Increment
        {
            get { return 1.0 / Math.Pow(10, DecimalPlaces); }
        }

        #endregion
    }
}
