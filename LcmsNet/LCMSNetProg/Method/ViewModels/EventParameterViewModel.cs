using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Method;
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

        public EventParameterViewModel(ParameterTypeEnum paramType, Type type)
        {
            ParameterType = paramType;
            NumberMinimum = 0.0;
            NumberMaximum = 10000000000.0;
            Type = type;

            CanShowDecimals = paramType == ParameterTypeEnum.Numeric && (type == typeof(double) || type == typeof(float));
        }

        private string parameterLabel = "";

        public ParameterTypeEnum ParameterType { get; }
        public bool ShowComboBox => ParameterType == ParameterTypeEnum.Enum;
        public bool ShowNumericUpDown => ParameterType == ParameterTypeEnum.Numeric;
        public bool ShowTextBox => ParameterType == ParameterTypeEnum.Text;
        public bool ShowCheckBox => ParameterType == ParameterTypeEnum.Boolean;

        public event EventHandler EventChanged;

        public Type Type { get; }

        public string ParameterLabel
        {
            get => parameterLabel;
            set => this.RaiseAndSetIfChanged(ref parameterLabel, value);
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
                        return Convert.ChangeType(NumberValue, Type);
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
            get => selectedOption;
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
        /// Event method for storing objects in the list view.
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

                if (SelectedOption == null || !comboBoxOptions.Contains(SelectedOption))
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
            get => textValue;
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

        #endregion

        #region CheckBox settings

        private bool boolValue;

        public bool BoolValue
        {
            get => boolValue;
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
