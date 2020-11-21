using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterEnumViewModel : ReactiveObject, ILCEventParameter
    {
        private string parameterLabel = "";
        private object selectedOption;
        private readonly ReactiveList<object> comboBoxOptions = new ReactiveList<object>();

        public string ParameterLabel
        {
            get => parameterLabel;
            set => this.RaiseAndSetIfChanged(ref parameterLabel, value);
        }

        public object ParameterValue
        {
            get => SelectedOption;
            set
            {
                if (value == null)
                {
                    SelectedOption = null;
                    return;
                }

                SelectedOption = value;
            }
        }

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

        public event EventHandler EventChanged;

        /// <summary>
        /// Fires the event changed event.
        /// </summary>
        private void OnEventChanged()
        {
            EventChanged?.Invoke(this, null);
        }
    }
}
