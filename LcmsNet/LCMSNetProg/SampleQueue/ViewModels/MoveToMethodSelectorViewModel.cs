using System.Reactive;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class MoveToMethodSelectorViewModel : ReactiveObject
    {
        public IReadOnlyReactiveList<string> LcMethodComboBoxOptions => SampleDataManager.LcMethodNameOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MoveToMethodSelectorViewModel()
        {
            SelectedLcMethod = null;

            OkCommand = ReactiveCommand.Create(CloseCleanup);
            CancelCommand = ReactiveCommand.Create(CloseCleanup);
        }

        private string selectedLcMethod;

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public string SelectedLcMethod
        {
            get => selectedLcMethod;
            set => this.RaiseAndSetIfChanged(ref selectedLcMethod, value);
        }

        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private void CloseCleanup()
        {

        }
    }
}
