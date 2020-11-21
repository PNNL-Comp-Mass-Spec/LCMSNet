using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterEnumViewModel : ReactiveObject, ILCEventParameterWithDataProvider
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
        /// Method for storing objects in the list view.
        /// </summary>
        /// <param name="sender">This parameter is needed for 'IDevice.RegisterDataProvider' support</param>
        /// <param name="data"></param>
        public void FillData(object sender, IReadOnlyList<object> data)
        {
            using (comboBoxOptions.SuppressChangeNotifications())
            {
                comboBoxOptions.Clear();

                if (data == null || data.Count < 1)
                    return;

                comboBoxOptions.AddRange(data);
            }

            if (data.Count > 0)
            {
                if (SelectedOption == null || !comboBoxOptions.Contains(SelectedOption))
                {
                    SelectedOption = comboBoxOptions.FirstOrDefault();
                }
            }
        }

        public event EventHandler EventChanged;

        public ILCEventParameter CreateDuplicate()
        {
            var copy = new EventParameterEnumViewModel();
            copy.FillData(null, ComboBoxOptions);
            return copy;
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
