using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicData;
using DynamicData.Binding;
using LcmsNet.Data;
using LcmsNet.Method.ViewModels;
using LcmsNet.Method.Views;
using LcmsNet.SampleQueue.IO;
using LcmsNet.SampleQueue.Views;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    /// <summary>
    /// Class that displays sample data as a sequence.
    /// </summary>
    public class SampleControlViewModel : ReactiveObject
    {
        public virtual ReadOnlyObservableCollection<SampleViewModel> Samples => SampleDataManager.Samples;

        private SampleViewModel selectedSample;

        public SampleViewModel SelectedSample
        {
            get => selectedSample;
            set => this.RaiseAndSetIfChanged(ref selectedSample, value);
        }

        public ObservableCollectionExtended<SampleViewModel> SelectedSamples { get; } = new ObservableCollectionExtended<SampleViewModel>();

        #region Manipulation Enablement

        /// <summary>
        /// Sets enabled to true if we have data and valid columns
        /// </summary>
        private void SetEnabledDisabled()
        {
            IsViewEnabled = SampleDataManager.HasData && SampleDataManager.HasValidColumns;
            if (IsViewEnabled)
            {
                BackColor = Brushes.White;
            }
            else
            {
                BackColor = Brushes.LightGray;
            }
        }

        private bool isViewEnabled = true;
        private SolidColorBrush backColor = Brushes.White;

        public bool IsViewEnabled
        {
            get => isViewEnabled;
            protected set => this.RaiseAndSetIfChanged(ref isViewEnabled, value);
        }

        public SolidColorBrush BackColor
        {
            get => backColor;
            protected set => this.RaiseAndSetIfChanged(ref backColor, value);
        }

        #endregion

        #region Constants

        /// <summary>
        /// String that should be displayed when new data is added but is not initialized.
        /// </summary>
        public const string CONST_NOT_SELECTED = "(Select)";

        #endregion

        #region Members

        private bool autoScroll = true;
        private readonly ObservableAsPropertyHelper<bool> itemsSelected;

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        public bool AutoScroll
        {
            get => autoScroll;
            set => this.RaiseAndSetIfChanged(ref autoScroll, value);
        }

        public bool ItemsSelected => this.itemsSelected?.Value ?? false;

        public SampleDataManager SampleDataManager { get; }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleDataManager
        /// </summary>
        public SampleControlViewModel(DMSDownloadViewModel dmsView, SampleDataManager sampleDataManager)
        {
            DMSView = dmsView;
            SampleDataManager = sampleDataManager;
            SampleDataManager.WhenAnyValue(x => x.HasData, x => x.HasValidColumns).ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => SetEnabledDisabled());
            SampleDataManager.SamplesSource.Connect().WhenValueChanged(x => x.HasNotRun).ObserveOn(RxApp.MainThreadScheduler)
                .Throttle(TimeSpan.FromMilliseconds(250)).Subscribe(x => PerformAutoScroll());
            this.WhenAnyValue(x => x.SampleDataManager.Samples).ObserveOn(RxApp.MainThreadScheduler).Throttle(TimeSpan.FromSeconds(0.25)).Subscribe(x => this.PerformAutoScroll());
            itemsSelected = this.WhenAnyValue(x => x.SelectedSamples, x => x.SelectedSample, x => x.SelectedSamples.Count).Select(x => x.Item1.Count > 0 || x.Item2 != null).ToProperty(this, x => x.ItemsSelected, initialValue: false);

            SetupCommands();
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample data manager and dms form is supplied.
        /// Calling this constructor is only for the windows ui designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleControlViewModel()
        {
            SampleDataManager = new SampleDataManager();
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        protected virtual void AddSamplesToManager(List<SampleData> samples, bool insertIntoUnused)
        {
            SampleDataManager.AddSamplesToManager(samples, insertIntoUnused);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DMS View form.
        /// </summary>
        public virtual DMSDownloadViewModel DMSView { get; }

        #endregion

        #region Utility Methods

        public void AddDateCartnameColumnIDToDatasetName()
        {
            var samples = GetSelectedSamples();
            if (samples.Count < 1)
                return;

            using (SampleDataManager.StartBatchChange())
            {
                SampleDataManager.AddDateCartnameColumnIDToDatasetName(samples);
            }

            // Re-select the first sample
            SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        public void ResetDatasetName()
        {
            var samples = GetSelectedSamples();

            SampleDataManager.ResetDatasetName(samples);

            // Re-select the first sample
            SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        private void EditTrayAndVial()
        {
            var samples = GetSelectedSamples();
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            samples.RemoveAll(sample => sample.RunningStatus != SampleRunningStatus.Queued);

            if (samples.Count < 1)
            {
                return;
            }

            if (SampleDataManager.AutoSamplerTrays.Count < 6)
            {
                ApplicationLogger.LogError(0, "Not enough PAL Trays are available.");
                return;
            }

            var trayVial = new TrayVialAssignmentViewModel(SampleDataManager.AutoSamplerTrays, samples);
            var trayVialWindow = new TrayVialAssignmentWindow() { DataContext = trayVial };

            using (var batchDisp = SampleDataManager.StartBatchChange())
            {
                var result = trayVialWindow.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    batchDisp.Cancelled = true;
                }
            }

            // Re-select the first sample
            SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        /// <summary>
        /// Performs fill down methods for sample data.
        /// </summary>
        private void FillDown()
        {
            // Get the list of selected samples
            var samples = GetSelectedSamples();

            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            samples.RemoveAll(sample => sample.RunningStatus != SampleRunningStatus.Queued);

            if (samples.Count < 1)
            {
                return;
            }

            // Create a new fill down form.
            var fillDownViewModel = new SampleMethodFillDownViewModel();
            fillDownViewModel.Samples = samples;
            fillDownViewModel.EnsureItemsAreSelected();
            var dialog = new SampleMethodFillDownWindow();
            dialog.DataContext = fillDownViewModel;

            using (var batchDisp = SampleDataManager.StartBatchChange())
            {
                var result = dialog.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    batchDisp.Cancelled = true;
                }
            }

            // Re-select the first sample
            var firstValid = Samples.Select(x => x.Sample).Intersect(samples).FirstOrDefault();
            if (firstValid != null)
            {
                SelectedSample = Samples.First(x => x.Sample.Equals(firstValid));
            }
        }

        /// <summary>
        /// Displays the DMS View Dialog Window.
        /// </summary>
        private void ShowDMSView()
        {
            if (DMSView == null)
                return;

            var dmsWindow = new DMSDownloadWindow() { DataContext = DMSView };
            var result = dmsWindow.ShowDialog();

            // If the user clicks ok , then add the samples from the
            // form into the sample queue.  Don't add them directly to the
            // form so that the event model will update both this view
            // and any other views that we may have.  For the sequence
            // we don't care how we add them to the form.
            if (result.HasValue && result.Value)
            {
                var samples = DMSView.GetNewSamplesDMSView();
                DMSView.ClearForm();

                using (SampleDataManager.StartBatchChange())
                {
                    var insertToUnused = false;
                    if (HasUnusedSamples())
                    {
                        // Ask the user what to do with these samples?
                        var dialog = new InsertOntoUnusedWindow();
                        var insertResult = dialog.ShowDialog();

                        insertToUnused = insertResult.HasValue && insertResult.Value;
                    }

                    AddSamplesToManager(samples, insertToUnused);
                }

                // Don't add directly to the user interface in case the
                // sample manager class has something to say about one of the samples
                ApplicationLogger.LogMessage(0, samples.Count + " samples added to the queue");
            }
        }

        /// <summary>
        /// Returns the list of selected samples.
        /// </summary>
        /// <returns></returns>
        public virtual List<SampleData> GetSelectedSamples()
        {
            var samples = new List<SampleData>();
            try
            {
                foreach (var sample in SelectedSamples)
                {
                    samples.Add(sample.Sample);
                }

                samples.Sort((x, y) => x.SequenceID.CompareTo(y.SequenceID));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Error in GetSelectedSamples: " + ex.Message, ex);
            }

            return samples;
        }

        /// <summary>
        /// Preview the selected samples on the data grid.
        /// </summary>
        public void PreviewSelectedThroughput()
        {
            var samples = GetSelectedSamples();
            samples.RemoveAll(data => data.Name.Contains(SampleDataManager.UnusedSampleName));
            if (samples.Count > 0)
            {
                // Validate the samples, and make sure we want to run these.
                var previewView = new ThroughputPreviewWindow();
                var previewVm = new ThroughputPreviewViewModel();
                previewView.DataContext = previewVm;

                foreach (var data in samples)
                {
                    data.SetActualLcMethod();
                    data.ActualLCMethod?.SetStartTime(TimeKeeper.Instance.Now);
                    //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)));
                }

                previewVm.ShowAlignmentForSamples(samples);
                previewView.ShowDialog();

                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
            }
        }

        #endregion

        #region Queue User Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        protected virtual SampleData AddNewSample(bool insertIntoUnused)
        {
            var newData = SampleDataManager.AddNewSample(insertIntoUnused);

            if (newData != null)
            {
                foreach (var sample in Samples.Reverse())
                {
                    if (sample.Sample.Name.Equals(newData.Name))
                    {
                        ScrollIntoView(sample);
                        SelectedSample = sample;
                        break;
                    }
                }
            }

            return newData;
        }

        /// <summary>
        /// Handles removing unused samples from the sample queue.
        /// </summary>
        protected virtual void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            SampleDataManager.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Returns whether the sample queue has any unused samples.
        /// </summary>
        /// <returns>True if an unused sample exists.</returns>
        protected virtual bool HasUnusedSamples()
        {
            return SampleDataManager.HasUnusedSamples();
        }

        /// <summary>
        /// Moves all the selected samples an offset of their original sequence id.
        /// </summary>
        protected virtual void MoveSelectedSamples(int offset, MoveSampleType moveType)
        {
            var data = SelectedSamples.Select(x => x.Sample).ToList();

            SampleDataManager.MoveSamples(data, offset, moveType);

            if (data.Count > 0)
            {
                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(data.First()));
            }
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        protected void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            try
            {
                // Get a list of sequence ID's to remove
                var samplesToRemove = SelectedSamples.OrderBy(x => x.Sample.SequenceID).ToList();

                // Select the sample just before or the first sample following the sample(s) deleted
                SampleViewModel sampleToSelect = null;
                if (samplesToRemove.Count > 0)
                {
                    var foundToDelete = false;
                    foreach (var sample in Samples)
                    {
                        if (samplesToRemove.Contains(sample))
                        {
                            foundToDelete = true;
                            if (sampleToSelect != null)
                            {
                                break;
                            }
                            continue;
                        }

                        sampleToSelect = sample;
                        if (foundToDelete)
                        {
                            break;
                        }
                    }
                }

                SampleDataManager.RemoveSamples(samplesToRemove, resortColumns);

                if (sampleToSelect != null)
                {
                    ScrollIntoView(sampleToSelect);
                    SelectedSample = sampleToSelect;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception in RemoveSelectedSamples: " + ex.Message, ex);
            }
        }

        #endregion

        #region Column visibility

        private bool checkboxColumnVisible = true;
        private bool statusColumnVisible = true;
        private bool columnIdColumnVisible = true;
        private bool palTrayColumnVisible = true;
        private bool palVialColumnVisible = true;
        private bool volumeColumnVisible = true;
        private bool lcMethodColumnVisible = true;
        private bool instrumentMethodColumnVisible = false;
        private bool batchIdColumnVisible = false;
        private bool blockColumnVisible = false;
        private bool runOrderColumnVisible = false;

        /// <summary>
        /// Number of columns on the left of the DataGrid that cannot be scrolled; columns in this group cannot be hidden
        /// Set to 0 to disable column freezing
        /// </summary>
        public virtual int NumFrozenColumns => 5;

        public bool CheckboxColumnVisible
        {
            get => checkboxColumnVisible;
            set => this.RaiseAndSetIfChanged(ref checkboxColumnVisible, value);
        }

        public bool StatusColumnVisible
        {
            get => statusColumnVisible;
            set => this.RaiseAndSetIfChanged(ref statusColumnVisible, value);
        }

        public bool ColumnIdColumnVisible
        {
            get => columnIdColumnVisible;
            set => this.RaiseAndSetIfChanged(ref columnIdColumnVisible, value);
        }

        public bool PalTrayColumnVisible
        {
            get => palTrayColumnVisible;
            set => this.RaiseAndSetIfChanged(ref palTrayColumnVisible, value);
        }

        public bool PalVialColumnVisible
        {
            get => palVialColumnVisible;
            set => this.RaiseAndSetIfChanged(ref palVialColumnVisible, value);
        }

        public bool VolumeColumnVisible
        {
            get => volumeColumnVisible;
            set => this.RaiseAndSetIfChanged(ref volumeColumnVisible, value);
        }

        public bool LcMethodColumnVisible
        {
            get => lcMethodColumnVisible;
            set => this.RaiseAndSetIfChanged(ref lcMethodColumnVisible, value);
        }

        public bool InstrumentMethodColumnVisible
        {
            get => instrumentMethodColumnVisible;
            set => this.RaiseAndSetIfChanged(ref instrumentMethodColumnVisible, value);
        }

        public bool BatchIdColumnVisible
        {
            get => batchIdColumnVisible;
            set => this.RaiseAndSetIfChanged(ref batchIdColumnVisible, value);
        }

        public bool BlockColumnVisible
        {
            get => blockColumnVisible;
            set => this.RaiseAndSetIfChanged(ref blockColumnVisible, value);
        }

        public bool RunOrderColumnVisible
        {
            get => runOrderColumnVisible;
            set => this.RaiseAndSetIfChanged(ref runOrderColumnVisible, value);
        }

        #endregion

        #region Form Control Event Handlers

        /// <summary>
        /// Gets or sets how to handle samples being deleted from columns
        /// </summary>
        public enumColumnDataHandling ColumnHandling { get; set; }

        /*
         * Unused methods
        private void mbutton_moveSelectedSamplesToColumn_Click(object sender, EventArgs e)
        {
            MoveSamplesToColumn(enumColumnDataHandling.LeaveAlone);
        }

        private void resetNameToOriginalRequestNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDatasetName();
        }
        */

        public void ImportQueueFromClipboard()
        {
            try
            {
                var samples = QueueImportClipboard.ReadSamples();
                SampleDataManager.SampleQueue.LoadQueue(samples);
                ApplicationLogger.LogMessage(0, "The queue was successfully imported from clipboard.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not load the queue from clipboard", ex);
            }
        }

        private void ClearSamplesConfirm()
        {
            var result = System.Windows.MessageBox.Show(
                @"You are about to clear your queued samples.  Select Ok to clear, or Cancel to have no change.", @"Clear Queue Confirmation",
                MessageBoxButton.OKCancel);

            ApplicationLogger.LogMessage(3, "The user clicked to clear the samples");
            if (result == MessageBoxResult.OK)
            {
                ApplicationLogger.LogMessage(3, "The user clicked to ok to clear the samples");
                SampleDataManager.ClearAllSamples();
            }
            else
            {
                ApplicationLogger.LogMessage(3, "The user clicked to cancel clearing samples");
            }
        }

        #endregion

        public void RestoreUserUIState()
        {
            ScrollIntoView(SelectedSample);
        }

        #region ReactiveCommands

        public ReactiveCommand<Unit, SampleData> AddBlankCommand { get; protected set; }
        public ReactiveCommand<Unit, SampleData> AddBlankToUnusedCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AddDMSCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> RemoveSelectedCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> FillDownCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> TrayVialCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> MoveDownCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> MoveUpCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DeleteUnusedCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> CartColumnDateCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UndoCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> RedoCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PreviewThroughputCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ClearAllSamplesCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ClipboardPasteCommand { get; protected set; }

        protected virtual void SetupCommands()
        {
            // Protected to allow inheriting class to override some of these commands, as needed

            AddBlankCommand = ReactiveCommand.Create(() => this.AddNewSample(false));
            AddBlankToUnusedCommand = ReactiveCommand.Create(() => this.AddNewSample(true));
            AddDMSCommand = ReactiveCommand.Create(() => this.ShowDMSView());
            RemoveSelectedCommand = ReactiveCommand.Create(() => this.RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone), this.WhenAnyValue(x => x.ItemsSelected));
            FillDownCommand = ReactiveCommand.Create(() => this.FillDown(), this.WhenAnyValue(x => x.ItemsSelected));
            TrayVialCommand = ReactiveCommand.Create(() => this.EditTrayAndVial(), this.WhenAnyValue(x => x.ItemsSelected));
            MoveDownCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(1, MoveSampleType.Sequence), this.WhenAnyValue(x => x.ItemsSelected));
            MoveUpCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(-1, MoveSampleType.Sequence), this.WhenAnyValue(x => x.ItemsSelected));
            DeleteUnusedCommand = ReactiveCommand.Create(() => this.RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone));
            CartColumnDateCommand = ReactiveCommand.Create(() => this.AddDateCartnameColumnIDToDatasetName(), this.WhenAnyValue(x => x.ItemsSelected));
            UndoCommand = ReactiveCommand.Create(() => this.SampleDataManager.Undo(), this.WhenAnyValue(x => x.SampleDataManager.CanUndo).ObserveOn(RxApp.MainThreadScheduler));
            RedoCommand = ReactiveCommand.Create(() => this.SampleDataManager.Redo(), this.WhenAnyValue(x => x.SampleDataManager.CanRedo).ObserveOn(RxApp.MainThreadScheduler));
            PreviewThroughputCommand = ReactiveCommand.Create(() => this.PreviewSelectedThroughput(), this.WhenAnyValue(x => x.ItemsSelected));
            ClearAllSamplesCommand = ReactiveCommand.Create(() => this.ClearSamplesConfirm());
            ClipboardPasteCommand = ReactiveCommand.Create(ImportQueueFromClipboard);
        }

        #endregion

        private void PerformAutoScroll()
        {
            if (AutoScroll && Samples != null)
            {
                // Create a list copy first to try to avoid collection modified exceptions...
                var samples = Samples.ToList();
                var lastCompletedSample = samples.Where(x => !x.Sample.HasNotRun).DefaultIfEmpty(null).Last();
                if (lastCompletedSample != null)
                {
                    var pos = samples.IndexOf(lastCompletedSample);
                    var scrollTo = Math.Min(pos + 4, Samples.Count - 1);
                    ScrollIntoView(Samples[scrollTo]);
                }
            }
        }

        public event EventHandler<SampleScrollChangeEventArgs> ScrollUpdateEvent;

        private void ScrollIntoView(SampleViewModel sampleToShow)
        {
            ScrollUpdateEvent?.Invoke(this, new SampleScrollChangeEventArgs(sampleToShow));
        }

        public class SampleScrollChangeEventArgs : EventArgs
        {
            public SampleViewModel SampleToShow { get; }

            public SampleScrollChangeEventArgs(SampleViewModel sampleToShow)
            {
                SampleToShow = sampleToShow;
            }
        }
    }
}
