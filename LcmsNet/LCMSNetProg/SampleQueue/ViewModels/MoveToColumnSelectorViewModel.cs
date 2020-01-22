using System;
using System.Reactive;
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

            Column1Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 0));
            Column2Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 1));
            Column3Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 2));
            Column4Command = ReactiveCommand.Create(new Action(() => SelectedColumn = 3));
            CancelCommand = ReactiveCommand.Create(new Action(CloseCleanup));
        }

        private int selectedColumn;
        private bool insertIntoUnused = true;

        /// <summary>
        /// Gets or sets the column the user selected to move samples to.  This value is zero-based.
        /// </summary>
        public int SelectedColumn
        {
            get => selectedColumn;
            set => this.RaiseAndSetIfChanged(ref selectedColumn, value);
        }

        /// <summary>
        /// Gets or sets whether to insert into unused.
        /// </summary>
        public bool InsertIntoUnused
        {
            get => insertIntoUnused;
            set => this.RaiseAndSetIfChanged(ref insertIntoUnused, value);
        }

        public ReactiveCommand<Unit, Unit> Column1Command { get; }
        public ReactiveCommand<Unit, Unit> Column2Command { get; }
        public ReactiveCommand<Unit, Unit> Column3Command { get; }
        public ReactiveCommand<Unit, Unit> Column4Command { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private void CloseCleanup()
        {

        }
    }
}
