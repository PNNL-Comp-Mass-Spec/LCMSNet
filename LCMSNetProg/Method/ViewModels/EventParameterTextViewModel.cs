using System;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterTextViewModel : ReactiveObject, ILCEventParameter
    {
        private string parameterLabel = "";
        private string textValue;

        public string ParameterLabel
        {
            get => parameterLabel;
            set => this.RaiseAndSetIfChanged(ref parameterLabel, value);
        }

        public object ParameterValue
        {
            get => TextValue;
            set => TextValue = value?.ToString();
        }

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

        public event EventHandler EventChanged;

        public ILCEventParameter CreateDuplicate()
        {
            return new EventParameterTextViewModel();
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
