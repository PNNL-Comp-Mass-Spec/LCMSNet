using ReactiveUI;

namespace LcmsNet.Method.ViewModels
{
    public class LCMethodStageSharedSettings : ReactiveObject
    {
        private bool? commentsEnabled = null;

        public bool? CommentsEnabled
        {
            get => commentsEnabled;
            set => this.RaiseAndSetIfChanged(ref commentsEnabled, value);
        }
    }
}
