using System;
using System.Reactive;
using ReactiveUI;

namespace LcmsNetCommonControls.ViewModels
{
    /// <summary>
    /// ViewModel to wrap a viewmodel that can be popped out of the containing control into a separate window
    /// </summary>
    public class PopoutViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public PopoutViewModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="child">The child view model</param>
        public PopoutViewModel(ReactiveObject child)
        {
            this.child = child;
            TackUnTackCommand = ReactiveCommand.Create(() => Tacked = !Tacked);
            //this.WhenAnyValue(x => x.Tacked).Subscribe(x => Tack?.Invoke(this, new TackEventArgs(x)));
        }

        private readonly ReactiveObject child;
        private bool tacked = true;

        /// <summary>
        /// The child view model
        /// </summary>
        public ReactiveObject Child => child;

        /// <summary>
        /// Command to tack or untack this control's view
        /// </summary>
        public ReactiveCommand<Unit, bool> TackUnTackCommand { get; private set; }

        /// <summary>
        /// If the control is tacked or not
        /// </summary>
        public bool Tacked
        {
            get => tacked;
            set
            {
                this.RaiseAndSetIfChanged(ref tacked, value);
                this.RaisePropertyChanged(nameof(TackType));
            }
        }

        /// <summary>
        /// Text to display on the button
        /// </summary>
        public string TackType => Tacked ? "Untack" : "Tack";

        //public event EventHandler<TackEventArgs> Tack;
    }
}
