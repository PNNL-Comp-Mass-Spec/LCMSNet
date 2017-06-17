using System;
using System.Reactive;
using ReactiveUI;

namespace LcmsNetCommonControls.ViewModels
{
    public class PopoutViewModel : ReactiveObject
    {
        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public PopoutViewModel()
        {
        }

        public PopoutViewModel(ReactiveObject child)
        {
            this.child = child;
            TackUnTackCommand = ReactiveCommand.Create(() => Tacked = !Tacked);
            //this.WhenAnyValue(x => x.Tacked).Subscribe(x => Tack?.Invoke(this, new TackEventArgs(x)));
        }

        private readonly ReactiveObject child;
        private bool tacked = true;

        public ReactiveObject Child => child;

        public ReactiveCommand<Unit, bool> TackUnTackCommand { get; private set; }

        public bool Tacked
        {
            get { return tacked; }
            set
            {
                this.RaiseAndSetIfChanged(ref tacked, value);
                this.RaisePropertyChanged(nameof(TackType));
            }
        }

        public string TackType
        {
            get { return Tacked ? "Untack" : "Tack"; }
        }

        //public event EventHandler<TackEventArgs> Tack;
    }
}
