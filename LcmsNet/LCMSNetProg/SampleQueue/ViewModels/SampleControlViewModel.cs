using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue.Forms;
using LcmsNet.SampleQueue.Views;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;
using ReactiveUI;

namespace LcmsNet.SampleQueue.ViewModels
{
    public class SampleControlViewModel : ReactiveObject
    {
        public virtual ReactiveList<SampleViewModel> Samples => SampleDataManager.Samples;

        private SampleViewModel selectedSample;

        public SampleViewModel SelectedSample
        {
            get { return selectedSample; }
            set { this.RaiseAndSetIfChanged(ref selectedSample, value); }
        }

        public ReactiveList<SampleViewModel> SelectedSamples { get; private set; }

        /// <summary>
        /// Edits the selected samples in the sample view.
        /// </summary>
        private void EditDMSData()
        {
            var samples = GetSelectedSamples();

            if (samples.Count < 1)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                    "You must select a sample to edit the DMS information.");
                return;
            }

            try
            {
                var dmsDisplay = new formSampleDMSValidatorDisplay(samples);
                // We don't care what the result is..
                if (dmsDisplay.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                // If samples are not valid...then what?
                if (!dmsDisplay.AreSamplesValid)
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                        "Some samples do not contain all necessary DMS information.  This will affect automatic uploads.");
                }
            }
            catch (InvalidOperationException ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                    "Unable to edit dmsdata:" + ex.Message, ex);
            }

            //
            // Then update the sample queue...
            //
            SampleDataManager.SampleQueue.UpdateSamples(samples);

            // Re-select the first sample
            SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        /// <summary>
        /// Delegate defining when status updates are available in batches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messages"></param>
        public delegate void DelegateStatusUpdates(object sender, List<string> messages);

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
            get { return isViewEnabled; }
            protected set { this.RaiseAndSetIfChanged(ref isViewEnabled, value); }
        }

        public SolidColorBrush BackColor
        {
            get { return backColor; }
            protected set { this.RaiseAndSetIfChanged(ref backColor, value); }
        }

        #endregion

        #region Constants

        /// <summary>
        /// String that should be displayed when new data is added but is not initialized.
        /// </summary>
        public const string CONST_NOT_SELECTED = "(Select)";

        #endregion

        #region Members

        /// <summary>
        /// Form that provides user interface to retrieve samples from DMS.
        /// </summary>
        private DMSDownloadViewModel m_dmsView;

        /// <summary>
        /// Fill down form for updating lots of samples at once.
        /// </summary>
        private SampleMethodFillDownViewModel fillDownViewModel;

        /// <summary>
        /// Tray and vial assignment form.
        /// </summary>
        private formTrayVialAssignment m_trayVial;

        private bool autoScroll = true;

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        public bool AutoScroll
        {
            get { return autoScroll; }
            set { this.RaiseAndSetIfChanged(ref autoScroll, value); }
        }

        public SampleDataManager SampleDataManager { get; private set; }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleDataManager
        /// </summary>
        public SampleControlViewModel(DMSDownloadViewModel dmsView, SampleDataManager sampleDataManager)
        {
            SampleDataManager = sampleDataManager;
            SampleDataManager.WhenAnyValue(x => x.HasData, x => x.HasValidColumns).Subscribe(x => SetEnabledDisabled());
            SampleDataManager.SampleQueue.SamplesUpdated += SamplesUpdated;

            Initialize(dmsView);
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
            Initialize(null);
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        /// <param name="dmsView"></param>
        private void Initialize(DMSDownloadViewModel dmsView)
        {
            DMSView = dmsView;

            SelectedSamples = new ReactiveList<SampleViewModel>();

            //
            // Background colors
            //
            // TODO: Alternating back colors to enhance user visual feedback.
            // TODO: m_colors = new Color[2];
            // TODO: m_colors[0] = Color.White;
            // TODO: m_colors[1] = Color.Gainsboro;

            //
            // Fill-down form for batch sample editing And Tray Vial assignmenet
            //
            fillDownViewModel = new SampleMethodFillDownViewModel();
            m_trayVial = new formTrayVialAssignment();
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        protected virtual void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            SampleDataManager.AddSamplesToManager(samples, insertIntoUnused);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DMS View form.
        /// </summary>
        public virtual DMSDownloadViewModel DMSView
        {
            get { return m_dmsView; }
            set { m_dmsView = value; }
        }

        #endregion

        #region Utility Methods

        public void AddDateCartnameColumnIDToDatasetName()
        {
            var samples = GetSelectedSamples();
            if (samples.Count < 1)
                return;

            SampleDataManager.AddDateCartnameColumnIDToDatasetName(samples);

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
            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (samples.Count < 1)
            {
                return;
            }

            if (SampleDataManager.AutoSamplerTrays.Count < 6)
            {
                classApplicationLogger.LogError(0, "Not enough PAL Trays are available.");
                return;
            }

            // TODO: if (ParentForm != null)
            // TODO:     m_trayVial.Icon = ParentForm.Icon;

            m_trayVial.LoadSampleList(SampleDataManager.AutoSamplerTrays, samples);
            if (m_trayVial.ShowDialog() == DialogResult.OK)
            {
                using (Samples.SuppressChangeNotifications())
                {
                    // TODO: Is this copy/update still needed?
                    samples = m_trayVial.SampleList;
                    SampleDataManager.SampleQueue.UpdateSamples(samples);
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
            //
            // Get the list of selected samples
            //
            var samples = GetSelectedSamples();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (samples.Count < 1)
            {
                return;
            }

            //
            // Create a new fill down form.
            //
            fillDownViewModel.Samples = samples;
            fillDownViewModel.EnsureItemsAreSelected();
            var dialog = new SampleMethodFillDownWindow();
            dialog.DataContext = fillDownViewModel;
            // Apparently required to allow keyboard input in a WPF Window launched from a WinForms app?
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(dialog);

            dialog.ShowDialog();

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
            if (m_dmsView == null)
                return;

            var dmsWindow = new DMSDownloadWindow() { DataContext = m_dmsView };
            // Apparently required to allow keyboard input in a WPF Window launched from a WinForms app?
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(dmsWindow);
            var result = dmsWindow.ShowDialog();

            //
            // If the user clicks ok , then add the samples from the
            // form into the sample queue.  Don't add them directly to the
            // form so that the event model will update both this view
            // and any other views that we may have.  For the sequence
            // we don't care how we add them to the form.
            //
            if (result.HasValue && result.Value)
            {
                var samples = m_dmsView.GetNewSamplesDMSView();
                m_dmsView.ClearForm();

                var insertToUnused = false;
                if (HasUnusedSamples())
                {
                    //
                    // Ask the user what to do with these samples?
                    //
                    var dialog = new InsertOntoUnusedWindow();
                    var insertResult = dialog.ShowDialog();

                    insertToUnused = insertResult.HasValue && insertResult.Value;
                }

                AddSamplesToManager(samples, insertToUnused);

                //
                // Don't add directly to the user interface in case the
                // sample manager class has something to say about one of the samples
                //
                classApplicationLogger.LogMessage(0, samples.Count + " samples added to the queue");
            }
        }

        /// <summary>
        /// Returns the list of selected samples.
        /// </summary>
        /// <returns></returns>
        public virtual List<classSampleData> GetSelectedSamples()
        {
            var samples = new List<classSampleData>();
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
                classApplicationLogger.LogError(0, "Error in GetSelectedSamples: " + ex.Message, ex);
            }

            return samples;
        }

        /// <summary>
        /// Preview the selected samples on the data grid.
        /// </summary>
        public void PreviewSelectedThroughput()
        {
            var samples = GetSelectedSamples();
            samples.RemoveAll(data => data.DmsData.DatasetName.Contains(SampleDataManager.SampleQueue.UnusedSampleName));
            if (samples.Count > 0)
            {
                //
                // Validate the samples, and make sure we want to run these.
                //
                var preview = new formThroughputPreview();
                preview.Show();

                foreach (var data in samples)
                {
                    data.LCMethod.SetStartTime(TimeKeeper.Instance.Now);
                    //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)));
                }

                preview.ShowAlignmentForSamples(samples);
                preview.Visible = false;
                preview.ShowDialog();

                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
            }
        }

        #endregion

        #region Queue User Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        protected virtual classSampleData AddNewSample(bool insertIntoUnused)
        {
            var newData = SampleDataManager.AddNewSample(insertIntoUnused);

            if (newData != null)
            {
                foreach (var sample in Samples.Reverse())
                {
                    if (sample.Sample.DmsData.RequestName.Equals(newData.DmsData.RequestName))
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
        protected virtual void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
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
        /// Randomizes the selected samples for the sample queue.
        /// </summary>
        private void RandomizeSelectedSamples()
        {
            var samplesToRandomize = new List<classSampleData>();
            //
            // Get all the data references that we want to randomize.
            //
            foreach (var row in SelectedSamples)
            {
                var data = row.Sample;
                if (data != null && data.RunningStatus == enumSampleRunningStatus.Queued)
                {
                    var sample = data.Clone() as classSampleData;
                    if (sample?.LCMethod?.Name != null)
                    {
                        if (classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                        {
                            //
                            // Because sample clones are deep copies, we cannot trust that
                            // every object in the sample is serializable...so...we are stuck
                            // making sure we re-hash the method using the name which
                            // is copied during the serialization.
                            //
                            sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                        }
                    }
                    samplesToRandomize.Add(sample);
                }
            }
            //
            // If we have something selected then randomize them.
            //
            if (samplesToRandomize.Count > 1)
            {
                formSampleRandomizer form;
                try
                {
                    form = new formSampleRandomizer(samplesToRandomize)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    // TODO: if (ParentForm != null)
                    // TODO:     form.Icon = ParentForm.Icon;
                }
                catch
                {
                    classApplicationLogger.LogError(0, "No randomization plug-ins exist.");
                    return;
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    using (Samples.SuppressChangeNotifications())
                    {
                        var newSamples = form.OutputSampleList;
                        SampleDataManager.SampleQueue.ReorderSamples(newSamples, enumColumnDataHandling.LeaveAlone);
                    }
                }
            }
            else if (samplesToRandomize.Count == 1)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Select more than one sample for randomization.");
            }
            else
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "No samples selected for randomization.");
            }
            if (samplesToRandomize.Count > 0)
            {
                // Re-select the first sample
                SelectedSample = Samples.First(x => x.Sample.Equals(samplesToRandomize.First()));
            }
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        private void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            try
            {
                //
                // Get a list of sequence ID's to remove
                //
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
                classApplicationLogger.LogError(0, "Exception in RemoveSelectedSamples: " + ex.Message, ex);
            }
        }

        #endregion

        #region Queue Manager Event Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            if (AutoScroll)
            {
                // Create a list copy first to try to avoid collection modified exceptions...
                var lastCompletedSample = Samples.ToList().Where(x => !x.Sample.HasNotRun).DefaultIfEmpty(null).Last();
                if (lastCompletedSample != null)
                {
                    var pos = Samples.IndexOf(lastCompletedSample);
                    var scrollTo = Math.Min(pos + 4, Samples.Count - 1);
                    ScrollIntoView(Samples[scrollTo]);
                }
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
        private bool datasetTypeColumnVisible = true;
        private bool cartConfigColumnVisible = true;
        private bool batchIdColumnVisible = false;
        private bool blockColumnVisible = false;
        private bool runOrderColumnVisible = false;

        /// <summary>
        /// Number of columns on the left of the DataGrid that cannot be scrolled; columns in this group cannot be hidden
        /// Set to 0 to disable column freezing
        /// </summary>
        public virtual int NumFrozenColumns
        {
            get { return 5; }
        }

        public bool CheckboxColumnVisible
        {
            get { return checkboxColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref checkboxColumnVisible, value); }
        }

        public bool StatusColumnVisible
        {
            get { return statusColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref statusColumnVisible, value); }
        }

        public bool ColumnIdColumnVisible
        {
            get { return columnIdColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref columnIdColumnVisible, value); }
        }

        public bool PalTrayColumnVisible
        {
            get { return palTrayColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref palTrayColumnVisible, value); }
        }

        public bool PalVialColumnVisible
        {
            get { return palVialColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref palVialColumnVisible, value); }
        }

        public bool VolumeColumnVisible
        {
            get { return volumeColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref volumeColumnVisible, value); }
        }

        public bool LcMethodColumnVisible
        {
            get { return lcMethodColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref lcMethodColumnVisible, value); }
        }

        public bool InstrumentMethodColumnVisible
        {
            get { return instrumentMethodColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref instrumentMethodColumnVisible, value); }
        }

        public bool DatasetTypeColumnVisible
        {
            get { return datasetTypeColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref datasetTypeColumnVisible, value); }
        }

        public bool CartConfigColumnVisible
        {
            get { return cartConfigColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref cartConfigColumnVisible, value); }
        }

        public bool BatchIdColumnVisible
        {
            get { return batchIdColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref batchIdColumnVisible, value); }
        }

        public bool BlockColumnVisible
        {
            get { return blockColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref blockColumnVisible, value); }
        }

        public bool RunOrderColumnVisible
        {
            get { return runOrderColumnVisible; }
            set { this.RaiseAndSetIfChanged(ref runOrderColumnVisible, value); }
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

        private void ClearSamplesConfirm()
        {
            var result = System.Windows.MessageBox.Show(
                @"You are about to clear your queued samples.  Select Ok to clear, or Cancel to have no change.", @"Clear Queue Confirmation",
                MessageBoxButton.OKCancel);

            classApplicationLogger.LogMessage(3, "The user clicked to clear the samples");
            if (result == MessageBoxResult.OK)
            {
                classApplicationLogger.LogMessage(3, "The user clicked to ok to clear the samples");
                SampleDataManager.ClearAllSamples();
            }
            else
            {
                classApplicationLogger.LogMessage(3, "The user clicked to cancel clearing samples");
            }
        }

        #endregion

        public void RestoreUserUIState()
        {
            ScrollIntoView(SelectedSample);
        }

        #region ReactiveCommands

        public ReactiveCommand AddBlankCommand { get; protected set; }
        public ReactiveCommand AddBlankToUnusedCommand { get; protected set; }
        public ReactiveCommand AddDMSCommand { get; protected set; }
        public ReactiveCommand RemoveSelectedCommand { get; protected set; }
        public ReactiveCommand FillDownCommand { get; protected set; }
        public ReactiveCommand TrayVialCommand { get; protected set; }
        public ReactiveCommand RandomizeCommand { get; protected set; }
        public ReactiveCommand MoveDownCommand { get; protected set; }
        public ReactiveCommand MoveUpCommand { get; protected set; }
        public ReactiveCommand DeleteUnusedCommand { get; protected set; }
        public ReactiveCommand CartColumnDateCommand { get; protected set; }
        public ReactiveCommand DmsEditCommand { get; protected set; }
        public ReactiveCommand UndoCommand { get; protected set; }
        public ReactiveCommand RedoCommand { get; protected set; }
        public ReactiveCommand PreviewThroughputCommand { get; protected set; }
        public ReactiveCommand ClearAllSamplesCommand { get; protected set; }

        protected virtual void SetupCommands()
        {
            // Protected to allow inheriting class to override some of these commands, as needed

            AddBlankCommand = ReactiveCommand.Create(() => this.AddNewSample(false));
            AddBlankToUnusedCommand = ReactiveCommand.Create(() => this.AddNewSample(true));
            AddDMSCommand = ReactiveCommand.Create(() => this.ShowDMSView());
            RemoveSelectedCommand = ReactiveCommand.Create(() => this.RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone));
            FillDownCommand = ReactiveCommand.Create(() => this.FillDown());
            TrayVialCommand = ReactiveCommand.Create(() => this.EditTrayAndVial());
            RandomizeCommand = ReactiveCommand.Create(() => this.RandomizeSelectedSamples());
            MoveDownCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(1, enumMoveSampleType.Sequence));
            MoveUpCommand = ReactiveCommand.Create(() => this.MoveSelectedSamples(-1, enumMoveSampleType.Sequence));
            DeleteUnusedCommand = ReactiveCommand.Create(() => this.RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone));
            CartColumnDateCommand = ReactiveCommand.Create(() => this.AddDateCartnameColumnIDToDatasetName());
            DmsEditCommand = ReactiveCommand.Create(() => this.EditDMSData());
            UndoCommand = ReactiveCommand.Create(() => this.SampleDataManager.Undo());
            RedoCommand = ReactiveCommand.Create(() => this.SampleDataManager.Redo());
            PreviewThroughputCommand = ReactiveCommand.Create(() => this.PreviewSelectedThroughput());
            ClearAllSamplesCommand = ReactiveCommand.Create(() => this.ClearSamplesConfirm());
        }

        #endregion

        public event EventHandler<SampleScrollChangeEventArgs> ScrollUpdateEvent;

        private void ScrollIntoView(SampleViewModel sampleToShow)
        {
            ScrollUpdateEvent?.Invoke(this, new SampleScrollChangeEventArgs(sampleToShow));
        }

        public class SampleScrollChangeEventArgs : EventArgs
        {
            public SampleViewModel SampleToShow { get; private set; }

            public SampleScrollChangeEventArgs(SampleViewModel sampleToShow)
            {
                SampleToShow = sampleToShow;
            }
        }
    }
}
