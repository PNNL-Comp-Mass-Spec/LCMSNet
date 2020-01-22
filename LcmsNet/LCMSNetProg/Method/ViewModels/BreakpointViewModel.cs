using System;
using System.Reactive;
using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class BreakpointViewModel : ReactiveObject
    {
        public BreakpointViewModel()
        {
            IsSet = false;
            ClickCommand = ReactiveCommand.Create(() => this.IsSet = !IsSet);
        }

        private bool isSet;

        public ReactiveCommand<Unit, bool> ClickCommand { get; }

        public bool IsSet
        {
            get => isSet;
            set
            {
                if (isSet == value)
                    return;

                this.RaiseAndSetIfChanged(ref isSet, value);
                BreakpointChanged?.Invoke(this, new BreakpointArgs(isSet));
            }
        }

        public event EventHandler<BreakpointArgs> BreakpointChanged;
    }

    public class BreakpointArgs : EventArgs
    {
        public BreakpointArgs(bool set)
        {
            IsSet = set;
        }

        public bool IsSet { get; }
    }
}
