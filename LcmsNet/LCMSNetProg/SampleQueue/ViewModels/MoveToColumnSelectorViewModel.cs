using System;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class MoveToColumnSelectorViewModel : ReactiveObject
    {
        /// <summary>
        /// Value if the user never selected a column to move samples to and thus cancelled the operation.
        /// </summary>
        public const int CONST_NO_COLUMN_SELECTED = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MoveToColumnSelectorViewModel()
        {
            SelectedColumn = CONST_NO_COLUMN_SELECTED;
            InsertIntoUnused = true;
            SetupCommands();
        }

        private int selectedColumn;
        private bool insertIntoUnused = true;

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public int SelectedColumn
        {
            get { return selectedColumn; }
            set { this.RaiseAndSetIfChanged(ref selectedColumn, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert into unused.
        /// </summary>
        public bool InsertIntoUnused
        {
            get { return insertIntoUnused; }
            set { this.RaiseAndSetIfChanged(ref insertIntoUnused, value); }
        }

        #region Commands

        public ReactiveCommand Column1Command { get; private set; }
        public ReactiveCommand Column2Command { get; private set; }
        public ReactiveCommand Column3Command { get; private set; }
        public ReactiveCommand Column4Command { get; private set; }
        public ReactiveCommand CancelCommand { get; private set; }

        private void SetupCommands()
        {
            Column1Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 0));
            Column2Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 1));
            Column3Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 2));
            Column4Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 3));
            CancelCommand = ReactiveCommand.Create(new Action(() => CloseCleanup()));
        }

        private void CloseCleanup()
        {

        }

        #endregion
    }
}
