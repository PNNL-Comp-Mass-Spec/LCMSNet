using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterEnumViewModel : ReactiveObject, ILCEventParameterWithDataProvider
    {
        public EventParameterEnumViewModel()
        {
            comboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var comboBoxOptionsBound).Subscribe();
            ComboBoxOptions = comboBoxOptionsBound;
        }

        private string parameterLabel = "";
        private object selectedOption;
        private readonly SourceList<object> comboBoxOptions = new SourceList<object>();

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

        public ReadOnlyObservableCollection<object> ComboBoxOptions { get; }

        /// <summary>
        /// Method for storing objects in the list view.
        /// </summary>
        /// <param name="sender">This parameter is needed for 'IDevice.RegisterDataProvider' support</param>
        /// <param name="data"></param>
        public void FillData(object sender, IReadOnlyList<object> data)
        {
            comboBoxOptions.Edit(list =>
            {
                list.Clear();

                if (data != null && data.Count > 0)
                {
                    list.AddRange(data);
                }
            });

            if (comboBoxOptions.Count > 0 && (SelectedOption == null || !ComboBoxOptions.Contains(SelectedOption)))
            {
                SelectedOption = ComboBoxOptions.FirstOrDefault();
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
