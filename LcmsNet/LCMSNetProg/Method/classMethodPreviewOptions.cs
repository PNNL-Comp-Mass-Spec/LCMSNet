using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNetSDK;

namespace LcmsNet.Method
{
    /// <summary>
    /// Options for previewing a method alignment.
    /// </summary>
    public class classMethodPreviewOptions : INotifyPropertyChangedExt
    {
        private bool animate;
        private int animateDelay;
        private int frameDelay;

        /// <summary>
        /// Gets or sets whether to animate the method alignment.
        /// </summary>
        public bool Animate
        {
            get { return animate; }
            set { this.RaiseAndSetIfChanged(ref animate, value); }
        }

        /// <summary>
        /// Gets or sets the animation delay.
        /// </summary>
        public int AnimateDelay
        {
            get { return animateDelay; }
            set { this.RaiseAndSetIfChanged(ref animateDelay, value); }
        }

        /// <summary>
        /// Gets or sets the frame delay count.
        /// </summary>
        public int FrameDelay
        {
            get { return frameDelay; }
            set { this.RaiseAndSetIfChanged(ref frameDelay, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}