using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNet.SampleQueue.Views;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class ColumnControlViewModel : SampleControlViewModel
    {
        public override IReadOnlyReactiveList<SampleViewModel> Samples => FilteredSamples;

        private readonly IReadOnlyReactiveList<SampleViewModel> filteredSamples;

        public IReadOnlyReactiveList<SampleViewModel> FilteredSamples => filteredSamples;

        // Local "wrapper" around the static class options, for data binding purposes
        public IReadOnlyReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleDataManager.LcMethodOptions;

        private readonly ObservableAsPropertyHelper<string> columnHeader;

        public string ColumnHeader => this.columnHeader?.Value ?? string.Empty;

        /// <summary>
        /// Default constructor for the column view control that takes no arguments
        /// but also no functionality unless the sample data manager and dms form is supplied.
        /// Calling this constructor is only for the windows ui designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public ColumnControlViewModel(bool commandsAreVisible = true) : base()
        {
            filteredSamples = new ReactiveList<SampleViewModel>();
            CheckboxColumnVisible = false;
            StatusColumnVisible = false;
            ColumnIdColumnVisible = false;
            DatasetTypeColumnVisible = false;
            LcMethodColumnVisible = false;
            InstrumentMethodColumnVisible = false;
            PalTrayColumnVisible = false;
            PalVialColumnVisible = false;
            VolumeColumnVisible = false;

            this.WhenAnyValue(x => x.Column, x => x.Column.ID, x => x.Column.Name)
                .Select(x => $"Column: (# {x.Item1.ID + 1}) {x.Item1.Name}")
                .ToProperty(this, x => x.ColumnHeader, out this.columnHeader, "Column: NOT SET");

            Column = new classColumnData() {ID = -2, Name = "DevColumn"};
            CommandsVisible = commandsAreVisible;
        }

        /// <summary>
        /// Constructor that accepts dmsView and sampleDataManager
        /// </summary>
        public ColumnControlViewModel(DMSDownloadViewModel dmsView, SampleDataManager sampleDataManager, classColumnData columnData, bool commandsAreVisible = true) : base(dmsView, sampleDataManager)
        {
            filteredSamples = SampleDataManager.Samples.CreateDerivedCollection(x => x, x => Column == null || Column.Equals(x.Sample.ColumnData));

            this.WhenAnyValue(x => x.Column, x => x.Column.ID, x => x.Column.Name)
                .Select(x => $"Column: (# {x.Item1.ID + 1}) {x.Item1.Name}")
                .ToProperty(this, x => x.ColumnHeader, out this.columnHeader, "Column: NOT SET");

            this.WhenAnyValue(x => x.Column.Status).Subscribe(x => this.SetColumnStatus());

            CheckboxColumnVisible = false;
            StatusColumnVisible = false;
            ColumnIdColumnVisible = false;
            DatasetTypeColumnVisible = false;
            LcMethodColumnVisible = false;
            InstrumentMethodColumnVisible = false;
            PalTrayColumnVisible = false;
            PalVialColumnVisible = false;
            VolumeColumnVisible = false;
            CartConfigColumnVisible = false;

            m_columnData = new classColumnData
            {
                ID = -1
            };

            SetupCommands();

            this.WhenAnyValue(x => x.ContainsKeyboardFocus).Subscribe(x => this.SetBackground());
            this.WhenAnyValue(x => x.Column.Status).Subscribe(x => ColumnEnabled = x != enumColumnStatus.Disabled);

            Column = columnData;
            CommandsVisible = commandsAreVisible;
        }

        private bool containsKeyboardFocus = false;
        private bool commandsVisible = true;
        private bool columnEnabled = true;
        private SolidColorBrush normalColor = null;

        public bool ContainsKeyboardFocus
        {
            get { return containsKeyboardFocus; }
            set { this.RaiseAndSetIfChanged(ref containsKeyboardFocus, value); }
        }

        public bool CommandsVisible
        {
            get { return commandsVisible; }
            private set { this.RaiseAndSetIfChanged(ref commandsVisible, value); }
        }

        public bool ColumnEnabled
        {
            get { return columnEnabled; }
            private set { this.RaiseAndSetIfChanged(ref columnEnabled, value); }
        }

        private void SetBackground()
        {
            if (ContainsKeyboardFocus)
            {
                normalColor = BackColor;
                BackColor = Brushes.DodgerBlue;
                var columnColor = Column.Color;
                if (columnColor != Color.FromArgb(0,0,0,0) && columnColor != Colors.Transparent)
                {
                    BackColor = new SolidColorBrush(columnColor);
                }
            }
            else if (normalColor != null)
            {
                //BackColor = SystemColors.WindowBrush;
                BackColor = normalColor;
            }
        }

        /// <summary>
        /// Number of columns on the left of the DataGrid that cannot be scrolled; columns in this group cannot be hidden
        /// Set to 0 to disable column freezing
        /// </summary>
        public override int NumFrozenColumns
        {
            get { return 0; }
        }

        #region Members

        /// <summary>
        /// Data object reference to synchronize column data with.
        /// </summary>
        private classColumnData m_columnData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the column number id for this control.
        /// </summary>
        public classColumnData Column
        {
            get { return m_columnData; }
            private set { this.RaiseAndSetIfChanged(ref m_columnData, value); }
        }

        #endregion

        #region Commands

        public ReactiveCommand AddBlankAppendCommand { get; protected set; }
        public ReactiveCommand MoveToColumnCommand { get; protected set; }

        protected override void SetupCommands()
        {
            base.SetupCommands();
            AddBlankCommand = ReactiveCommand.Create(() => this.AddNewSample(true));
            AddBlankAppendCommand = ReactiveCommand.Create(() => this.AddNewSample(false));
            RemoveSelectedCommand = ReactiveCommand.Create(() => this.RemoveSelectedSamples(enumColumnDataHandling.CreateUnused), this.WhenAnyValue(x => x.ItemsSelected));
            DeleteUnusedCommand = ReactiveCommand.Create(() => this.SampleDataManager.RemoveUnusedSamples(Column, enumColumnDataHandling.CreateUnused));
            MoveDownCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(1, enumMoveSampleType.Column), this.WhenAnyValue(x => x.ItemsSelected));
            MoveUpCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(-1, enumMoveSampleType.Column), this.WhenAnyValue(x => x.ItemsSelected));
            MoveToColumnCommand = ReactiveCommand.Create(() => this.MoveSamplesToColumn(enumColumnDataHandling.CreateUnused), this.WhenAnyValue(x => x.ItemsSelected));
        }

        #endregion

        #region Column Event Handlers and Methods

        /// <summary>
        /// Sets the user controls status
        /// </summary>
        private void SetColumnStatus()
        {
            // Status updates
            if (m_columnData.Status == enumColumnStatus.Disabled)
            {
                IsViewEnabled = false;
                BackColor = Brushes.DarkGray;
            }
            else
            {
                IsViewEnabled = true;
                BackColor = Brushes.White;
            }
        }

        #endregion

        #region Sample Queue Management Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        protected override classSampleData AddNewSample(bool insertIntoUnused)
        {
            var newData = base.AddNewSample(insertIntoUnused);

            if (newData != null && !newData.ColumnData.Equals(this.Column))
            {
                if (FilteredSamples.Count > 0)
                {
                    newData.LCMethod = FilteredSamples.ToList().Last().Sample.LCMethod;
                }
                else
                {
                    newData.LCMethod = null;
                }
                newData.ColumnData = this.Column;
            }

            return newData;
        }

        /// <summary>
        /// Adds the specified samples ot this column.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="insertIntoUnused"></param>
        /// <returns></returns>
        protected override void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            // For every sample, add the column data to it, then add it into the manager.
            // We don't add to our list first so the manager can verify the sample and
            // make sure we don't have duplicates.
            foreach (var sample in samples)
                sample.ColumnData = m_columnData;

            base.AddSamplesToManager(samples, insertIntoUnused);
        }

        /// <summary>
        /// Moves the selected samples by the offset.  First calculates how far to move the samples on this
        /// column by finding out how many columns are enabled in the configuration.
        /// </summary>
        /// <param name="offset">Amount to move the samples (-1 for lower sequence numbers) (1 for higher sequence numbers)</param>
        /// <param name="moveType"></param>
        protected override void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            var numEnabledColumns = classCartConfiguration.NumberOfEnabledColumns;

            // We are moving the sample by N in the queue to offset for the enabled / disabled columns.
            offset *= numEnabledColumns;
            base.MoveSelectedSamples(offset, enumMoveSampleType.Column);
        }

        /// <summary>
        /// Handles removing unused samples from the sample queue only for this column.
        /// </summary>
        protected override void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            SampleDataManager.RemoveUnusedSamples(m_columnData, resortColumns);
        }

        /// <summary>
        /// Returns whether the sample queue has any unused samples on this column.
        /// </summary>
        /// <returns>True if an unused sample exists.</returns>
        protected override bool HasUnusedSamples()
        {
            return SampleDataManager.HasUnusedSamples(Column);
        }

        /// <summary>
        /// Moves the selected samples to another column selected through a dialog window.
        /// </summary>
        protected void MoveSamplesToColumn(enumColumnDataHandling handling)
        {
            var selector = new MoveToColumnSelectorViewModel();
            var selectorWindow = new MoveToColumnSelectorWindow() { DataContext = selector };

            var result = selectorWindow.ShowDialog();

            if (result.HasValue && result.Value &&
                selector.SelectedColumn != MoveToColumnSelectorViewModel.CONST_NO_COLUMN_SELECTED)
            {
                var column = selector.SelectedColumn;
                var selectedSamples = GetSelectedSamples();

                if (selectedSamples.Count < 1)
                    return;

                // Make sure the samples can actually run, e.g. don't put a sample on column 2 already back onto column 2.
                // Don't put a column that has been run, at the end of the queue again.
                var samples = new List<classSampleData>();
                foreach (var sample in selectedSamples)
                {
                    if (sample.RunningStatus == enumSampleRunningStatus.Queued && column != sample.ColumnData.ID)
                    {
                        samples.Add(sample);
                    }
                }

                // Find the first valid LC Method that uses the specified column?
                classLCMethod method = null;
                foreach (var lcMethod in SampleDataManager.LcMethodOptions)
                {
                    if (lcMethod.Column == column)
                    {
                        method = lcMethod;
                        break;
                    }
                }

                using (Samples.SuppressChangeNotifications())
                {
                    // Get the list of unique id's from the samples and
                    // change the column to put the samples on.

                    // Could keep track of updated IDs with
                    // var ids = new List<long>();

                    foreach (var sample in samples)
                    {
                        // ids.Add(sample.UniqueID);
                        sample.LCMethod = method;
                        sample.ColumnData = classCartConfiguration.Columns[column];
                    }

                    // TODO: The below code was what would do the moving into unused samples, long disabled Should it be deleted?.
                    /*
                    // Then remove them from the queue
                    enumColumnDataHandling backFill = enumColumnDataHandling.CreateUnused;
                    if (selector.InsertIntoUnused)
                    {
                        backFill = enumColumnDataHandling.LeaveAlone;
                    }

                    SampleDataManager.SampleQueue.RemoveSample(ids, backFill);

                    // Then re-queue the samples.
                    try
                    {
                        if (selector.InsertIntoUnused)
                        {
                            SampleDataManager.SampleQueue.InsertIntoUnusedSamples(samples, handling);
                        }
                        else
                        {
                            //SampleDataManager.UpdateSamples(
                            SampleDataManager.SampleQueue.QueueSamples(samples, handling);
                        }
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "Could not queue the samples when moving between columns.", ex);
                    }
                    if (samples.Count > 0)
                    {
                        SampleDataManager.SampleQueue.UpdateSamples(samples);
                    }
                    */
                }

                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(selectedSamples.First()));
            }
        }

        #endregion
    }
}
