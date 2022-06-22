using System;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class EventParameterBoolViewModel : ReactiveObject, ILCEventParameter
    {
        private string parameterLabel = "";
        private bool boolValue;

        public string ParameterLabel
        {
            get => parameterLabel;
            set => this.RaiseAndSetIfChanged(ref parameterLabel, value);
        }

        public object ParameterValue
        {
            get => BoolValue;
            set
            {
                if (value == null)
                {
                    BoolValue = false;
                    return;
                }

                BoolValue = Convert.ToBoolean(value);
            }
        }

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

        public event EventHandler EventChanged;

        public ILCEventParameter CreateDuplicate()
        {
            return new EventParameterBoolViewModel();
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
