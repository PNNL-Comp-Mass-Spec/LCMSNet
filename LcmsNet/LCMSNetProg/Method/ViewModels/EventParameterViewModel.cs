using System;
using System.Collections.Generic;
using System.Linq;
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
            Boolean,
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
        }

        private string parameterLabel = "";

        public ParameterTypeEnum ParameterType { get; private set; }
        public bool ShowComboBox { get { return ParameterType == ParameterTypeEnum.Enum; } }
        public bool ShowNumericUpDown { get { return ParameterType == ParameterTypeEnum.Numeric; } }
        public bool ShowTextBox { get { return ParameterType == ParameterTypeEnum.Text; } }
        public bool ShowCheckBox { get { return ParameterType == ParameterTypeEnum.Boolean; } }

        public event EventHandler EventChanged;

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
                    case ParameterTypeEnum.Boolean:
                        return BoolValue;
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
                    BoolValue = false;
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
                    case ParameterTypeEnum.Boolean:
                        BoolValue = Convert.ToBoolean(value);
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

        public IReadOnlyReactiveList<object> ComboBoxOptions => comboBoxOptions;

        /// <summary>
        /// Event method to storing objects in the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void FillData(object sender, List<object> data)
        {
            comboBoxOptions.Clear();
            if (data == null || data.Count < 1)
                return;

            if (data.Count > 0)
            {
                using (comboBoxOptions.SuppressChangeNotifications())
                {
                    comboBoxOptions.AddRange(data);
                }

                if (SelectedOption != null && !comboBoxOptions.Contains(SelectedOption))
                {
                    SelectedOption = comboBoxOptions.FirstOrDefault();
                }
            }
        }

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

        #region CheckBox settings

        private bool boolValue;

        public bool BoolValue
        {
            get { return boolValue; }
            set
            {
                var oldValue = boolValue;
                this.RaiseAndSetIfChanged(ref boolValue, value);
                if (!Equals(oldValue, boolValue))
                {
                    OnEventChanged();
                }
            }
        }

        #endregion
    }
}
