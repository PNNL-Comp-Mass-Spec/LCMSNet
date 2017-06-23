using System;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class MoveToMethodSelectorViewModel : ReactiveObject
    {
        public IReadOnlyReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleDataManager.LcMethodOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MoveToMethodSelectorViewModel()
        {
            SelectedLcMethod = null;
            SetupCommands();
        }

        private classLCMethod selectedLcMethod;

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public classLCMethod SelectedLcMethod
        {
            get { return selectedLcMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedLcMethod, value); }
        }

        #region Commands

        public ReactiveCommand OkCommand { get; private set; }
        public ReactiveCommand CancelCommand { get; private set; }

        private void SetupCommands()
        {
            OkCommand = ReactiveCommand.Create(new Action(() => CloseCleanup()));
            CancelCommand = ReactiveCommand.Create(new Action(() => CloseCleanup()));
        }

        private void CloseCleanup()
        {

        }

        #endregion
    }
}
