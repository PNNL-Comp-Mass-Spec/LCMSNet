using System;
using System.Reactive;
using ReactiveUI;

namespace LcmsNet.ViewModels
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

        public PopoutViewModel(ReactiveObject child, string title)
        {
            this.child = child;
            this.title = title;
            TackUnTackCommand = ReactiveCommand.Create(() => Tacked = !Tacked);
            //this.WhenAnyValue(x => x.Tacked).Subscribe(x => Tack?.Invoke(this, new TackEventArgs(x)));
        }

        public PopoutViewModel(ReactiveObject child, string title, int windowWidth, int windowHeight) : this(child, title)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
        }

        private readonly ReactiveObject child;
        private readonly string title;
        private bool tacked = true;
        private int windowWidth = 477;
        private int windowHeight = 356;

        public ReactiveObject Child => child;
        public string Title => title;
        public int WindowWidth => windowWidth;
        public int WindowHeight => windowHeight;

        public ReactiveCommand<Unit, bool> TackUnTackCommand { get; private set; }

        public bool Tacked
        {
            get { return tacked; }
            set { this.RaiseAndSetIfChanged(ref tacked, value); }
        }

        //public event EventHandler<TackEventArgs> Tack;
    }
}
