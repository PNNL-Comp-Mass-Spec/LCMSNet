using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Media;
using LcmsNet.SampleQueue;
using LcmsNet.WPFControls.Views;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using ReactiveUI;

namespace LcmsNet.WPFControls.ViewModels
{
    public class MethodControlViewModel : SampleControlViewModel
    {
        public override ReactiveList<SampleViewModel> Samples => FilteredSamples;

        public ReactiveList<SampleViewModel> FilteredSamples { get; private set; }

        // Local "wrapper" around the static class options, for data binding purposes
        public ReactiveList<classLCMethod> LcMethodComboBoxOptions => SampleQueueComboBoxOptions.LcMethodOptions;

        private classLCMethod selectedLCMethod;

        public classLCMethod SelectedLCMethod
        {
            get { return selectedLCMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedLCMethod, value); }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public MethodControlViewModel() : base()
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
            CheckboxColumnVisible = false;
            StatusColumnVisible = false;
            ColumnIdColumnVisible = false;
            DatasetTypeColumnVisible = false;
            LcMethodColumnVisible = false;
            InstrumentMethodColumnVisible = false;
            PalTrayColumnVisible = false;
            PalVialColumnVisible = false;
            VolumeColumnVisible = false;

            IsViewEnabled = true;
            BackColor = Brushes.White;
        }

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public MethodControlViewModel(formDMSView dmsView, SampleDataManager sampleDataManager) : base(dmsView, sampleDataManager)
        {
            FilteredSamples = new ReactiveList<SampleViewModel>();
            BindingOperations.EnableCollectionSynchronization(FilteredSamples, this);
            ResetFilteredSamples();

            this.WhenAnyValue(x => x.SelectedLCMethod).Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamples());

            SampleDataManager.Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.Sample.LCMethod)))
                .Throttle(TimeSpan.FromSeconds(0.25))
                .Subscribe(x => ResetFilteredSamples());

            SampleDataManager.Samples.Changed.Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => ResetFilteredSamples());

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

            SetupCommands();
            this.WhenAnyValue(x => x.ContainsKeyboardFocus).Subscribe(x => this.SetBackground());

            IsViewEnabled = true;
            BackColor = Brushes.White;
        }

        private bool containsKeyboardFocus = false;
        private bool commandsVisible = true;
        private SolidColorBrush normalColor = null;
        private bool methodVisible = true;

        public bool ContainsKeyboardFocus
        {
            get { return containsKeyboardFocus; }
            set { this.RaiseAndSetIfChanged(ref containsKeyboardFocus, value); }
        }

        public bool CommandsVisible
        {
            get { return commandsVisible; }
            set { this.RaiseAndSetIfChanged(ref commandsVisible, value); }
        }

        public bool MethodVisible
        {
            get { return methodVisible; }
            set { this.RaiseAndSetIfChanged(ref methodVisible, value); }
        }

        private void SetBackground()
        {
            if (ContainsKeyboardFocus)
            {
                normalColor = BackColor;
                BackColor = Brushes.DodgerBlue;
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

        private void ResetFilteredSamples()
        {
            using (FilteredSamples.SuppressChangeNotifications())
            {
                FilteredSamples.Clear();
                if (SelectedLCMethod == null)
                {
                    FilteredSamples.AddRange(SampleDataManager.Samples);
                    return;
                }
                FilteredSamples.AddRange(SampleDataManager.Samples.Where(x => SelectedLCMethod.Equals(x.Sample.LCMethod)));
            }
        }

        #region Commands

        public ReactiveCommand AddBlankAppendCommand { get; protected set; }
        public ReactiveCommand MoveToColumnCommand { get; protected set; }

        protected override void SetupCommands()
        {
            base.SetupCommands();
            AddBlankCommand = ReactiveCommand.Create(() => this.AddNewSample(true));
            AddBlankAppendCommand = ReactiveCommand.Create(() => this.AddNewSample(false));
            RemoveSelectedCommand = ReactiveCommand.Create(() => this.SampleDataManager.RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone));
            MoveDownCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(1, enumMoveSampleType.Sequence));
            MoveUpCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(-1, enumMoveSampleType.Sequence));
            MoveToColumnCommand = ReactiveCommand.Create(() => this.MoveSamplesToMethod(enumColumnDataHandling.LeaveAlone));
        }

        #endregion

        #region Sample Queue Management Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        protected override classSampleData AddNewSample(bool insertIntoUnused)
        {
            var newData = base.AddNewSample(insertIntoUnused);

            if (newData != null && !newData.LCMethod.Equals(this.SelectedLCMethod))
            {
                newData.LCMethod = this.SelectedLCMethod;
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
            //
            // For every sample, add the column data to it, then add it into the manager.
            // We don't add to our list first so the manager can verify the sample and
            // make sure we don't have duplicates.
            //
            foreach (var sample in samples)
                sample.LCMethod = this.SelectedLCMethod;

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
            base.MoveSelectedSamples(offset, enumMoveSampleType.Column);
        }

        /// <summary>
        /// Handles removing unused samples from the sample queue only for this column.
        /// </summary>
        protected override void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            SampleDataManager.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Returns whether the sample queue has any unused samples on this column.
        /// </summary>
        /// <returns>True if an unused sample exists.</returns>
        protected override bool HasUnusedSamples()
        {
            return SampleDataManager.HasUnusedSamples();
        }

        /// <summary>
        /// Moves the selected samples to another column selected through a dialog window.
        /// </summary>
        protected void MoveSamplesToMethod(enumColumnDataHandling handling)
        {
            var selector = new MoveToMethodSelectorViewModel();
            var selectorWindow = new MoveToMethodSelectorWindow() { DataContext = selector };

            var result = selectorWindow.ShowDialog();

            if (result.HasValue && result.Value &&
                selector.SelectedLcMethod != null)
            {
                var method = selector.SelectedLcMethod;
                var selectedSamples = GetSelectedSamples();

                if (selectedSamples.Count < 1)
                    return;

                //
                // Make sure the samples can actually run, e.g. don't put a sample on column 2 already back onto column 2.
                // Don't put a column that has been run, at the end of the queue again.
                //
                var samples = new List<classSampleData>();
                foreach (var sample in selectedSamples)
                {
                    if (sample.RunningStatus == enumSampleRunningStatus.Queued && !method.Equals(sample.LCMethod))
                    {
                        samples.Add(sample);
                    }
                }

                using (Samples.SuppressChangeNotifications())
                {
                    //
                    // Get the list of unique id's from the samples and
                    // change the column to put the samples on.
                    //

                    // Could keep track of updated IDs with
                    // var ids = new List<long>();

                    foreach (var sample in samples)
                    {
                        // ids.Add(sample.UniqueID);
                        sample.LCMethod = method;
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
