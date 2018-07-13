using System;
using LcmsNetData.Configuration;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class ColumnManagerViewModel : ReactiveObject
    {
        /// <summary>
        /// Default constructor for the column manager view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ColumnManagerViewModel()
        {
            Column1ViewModel = new ColumnControlViewModel(false);
            Column2ViewModel = new ColumnControlViewModel(false);
            Column3ViewModel = new ColumnControlViewModel(false);
            Column4ViewModel = new ColumnControlViewModel(false);
        }

        public ColumnManagerViewModel(DMSDownloadViewModel dmsView, SampleDataManager sampleDataManager)
        {
            Column1ViewModel = new ColumnControlViewModel(dmsView, sampleDataManager, CartConfiguration.Columns[0], false);
            Column2ViewModel = new ColumnControlViewModel(dmsView, sampleDataManager, CartConfiguration.Columns[1], false);
            Column3ViewModel = new ColumnControlViewModel(dmsView, sampleDataManager, CartConfiguration.Columns[2], false);
            Column4ViewModel = new ColumnControlViewModel(dmsView, sampleDataManager, CartConfiguration.Columns[3], false);

            this.WhenAnyValue(x => x.Column1ViewModel, x => x.Column1ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedColumn = x.Item2 ? x.Item1 : this.FocusedColumn);
            this.WhenAnyValue(x => x.Column2ViewModel, x => x.Column2ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedColumn = x.Item2 ? x.Item1 : this.FocusedColumn);
            this.WhenAnyValue(x => x.Column3ViewModel, x => x.Column3ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedColumn = x.Item2 ? x.Item1 : this.FocusedColumn);
            this.WhenAnyValue(x => x.Column4ViewModel, x => x.Column4ViewModel.ContainsKeyboardFocus).Subscribe(x => this.FocusedColumn = x.Item2 ? x.Item1 : this.FocusedColumn);

            FocusedColumn = Column1ViewModel;
        }

        private ColumnControlViewModel column1ViewModel;
        private ColumnControlViewModel column2ViewModel;
        private ColumnControlViewModel column3ViewModel;
        private ColumnControlViewModel column4ViewModel;
        private ColumnControlViewModel focusedColumn;

        public ColumnControlViewModel Column1ViewModel
        {
            get { return column1ViewModel; }
            private set { this.RaiseAndSetIfChanged(ref column1ViewModel, value); }
        }

        public ColumnControlViewModel Column2ViewModel
        {
            get { return column2ViewModel; }
            private set { this.RaiseAndSetIfChanged(ref column2ViewModel, value); }
        }

        public ColumnControlViewModel Column3ViewModel
        {
            get { return column3ViewModel; }
            private set { this.RaiseAndSetIfChanged(ref column3ViewModel, value); }
        }

        public ColumnControlViewModel Column4ViewModel
        {
            get { return column4ViewModel; }
            private set { this.RaiseAndSetIfChanged(ref column4ViewModel, value); }
        }

        public ColumnControlViewModel FocusedColumn
        {
            get { return focusedColumn; }
            private set { this.RaiseAndSetIfChanged(ref focusedColumn, value); }
        }

        public void ClearFocused()
        {
            Column1ViewModel.ContainsKeyboardFocus = false;
            Column2ViewModel.ContainsKeyboardFocus = false;
            Column3ViewModel.ContainsKeyboardFocus = false;
            Column4ViewModel.ContainsKeyboardFocus = false;
        }
    }
}
