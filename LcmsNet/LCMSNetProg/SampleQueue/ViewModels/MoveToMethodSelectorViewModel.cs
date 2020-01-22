using System;
using System.Reactive;
using LcmsNetSDK.Method;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class MoveToMethodSelectorViewModel : ReactiveObject
    {
        public IReadOnlyReactiveList<LCMethod> LcMethodComboBoxOptions => SampleDataManager.LcMethodOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MoveToMethodSelectorViewModel()
        {
            SelectedLcMethod = null;

            OkCommand = ReactiveCommand.Create(CloseCleanup);
            CancelCommand = ReactiveCommand.Create(CloseCleanup);
        }

        private LCMethod selectedLcMethod;

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public LCMethod SelectedLcMethod
        {
            get => selectedLcMethod;
            set => this.RaiseAndSetIfChanged(ref selectedLcMethod, value);
        }

        public ReactiveCommand<Unit, Unit> OkCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; private set; }

        private void CloseCleanup()
        {

        }
    }
}
