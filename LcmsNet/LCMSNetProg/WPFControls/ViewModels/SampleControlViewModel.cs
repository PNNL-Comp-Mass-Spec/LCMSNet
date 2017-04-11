using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetDmsTools;
using LcmsNetSDK;
using ReactiveUI;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.Forms.MessageBox;

namespace LcmsNet.WPFControls.ViewModels
{
    public class SampleControlViewModel : ReactiveObject
    {
        public ReactiveList<SampleViewModel> Samples { get; private set; }

        private formMoveToColumnSelector m_selector;

        private readonly classDMSSampleValidator mValidator;

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
            m_sampleQueue.UpdateSamples(samples);

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        /// <summary>
        /// Delegate defining when status updates are available in batches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messages"></param>
        public delegate void DelegateStatusUpdates(object sender, List<string> messages);

        #region Column Events

        /// <summary>
        /// Handles when a property about a column changes and rebuilds the column ordering list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        void column_StatusChanged(object sender, enumColumnStatus previousStatus, enumColumnStatus newStatus)
        {
            //
            // Make sure we have at least one column that is enabled
            //
            var enabled = false;
            foreach (var column in classCartConfiguration.Columns)
            {
                if (column.Status != enumColumnStatus.Disabled)
                {
                    enabled = true;
                    break;
                }
            }

            //
            // If at least one column is not enabled, then we disable the sample queue
            //
            if (enabled == false)
            {
                IsViewEnabled = false;
                BackColor = Brushes.LightGray;
            }
            else
            {
                IsViewEnabled = true;
                BackColor = Brushes.White;
            }

            SampleQueue.UpdateAllSamples();
        }

        private bool isViewEnabled = true;
        private SolidColorBrush backColor = Brushes.White;

        public bool IsViewEnabled
        {
            get { return isViewEnabled; }
            private set { this.RaiseAndSetIfChanged(ref isViewEnabled, value); }
        }

        public SolidColorBrush BackColor
        {
            get { return backColor; }
            private set { this.RaiseAndSetIfChanged(ref backColor, value); }
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
        private formDMSView m_dmsView;

        /// <summary>
        /// Object that manages the list of all samples.
        /// </summary>
        private classSampleQueue m_sampleQueue;

        /// <summary>
        /// Names of the methods available on the PAL
        /// </summary>
        private List<string> m_autoSamplerMethods;

        /// <summary>
        /// Names of the trays available on the PAL
        /// </summary>
        private List<string> m_autosamplerTrays;

        /// <summary>
        /// Names of the instrument methods available on the MS.
        /// </summary>
        private List<string> m_instrumentMethods;

        /// <summary>
        /// Flag that turns off the coloring when a PAL item (method, tray) was not downloadable from the PAL.
        /// </summary>
        private bool m_ignoreMissingPALValues;

        /// <summary>
        /// Fill down form for updating lots of samples at once.
        /// </summary>
        private formMethodFillDown m_filldown;

        /// <summary>
        /// Tray and vial assignment form.
        /// </summary>
        private formTrayVialAssignment m_trayVial;

        private bool dmsAvailable = false;
        private bool cycleColumns = false;
        private bool autoScroll = true;

        public bool DMSAvailable
        {
            get { return dmsAvailable; }
            set { this.RaiseAndSetIfChanged(ref dmsAvailable, value); }
        }

        public bool CycleColumns
        {
            get { return cycleColumns; }
            set { this.RaiseAndSetIfChanged(ref cycleColumns, value); }
        }

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        public bool AutoScroll
        {
            get { return autoScroll; }
            set { this.RaiseAndSetIfChanged(ref autoScroll, value); }
        }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public SampleControlViewModel(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            foreach (var column in classCartConfiguration.Columns)
            {
                column.StatusChanged += column_StatusChanged;
            }

            DMSAvailable = true;
            if (string.IsNullOrWhiteSpace(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL)))
            {
                DMSAvailable = false;
            }
            classLCMSSettings.SettingChanged += classLCMSSettings_SettingChanged;

            // TODO: SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            // TODO: SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                mValidator = new classDMSSampleValidator();
                Initialize(dmsView, sampleQueue);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 1: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        public SampleControlViewModel()
        {
            if (classCartConfiguration.Columns != null)
            {
                foreach (var column in classCartConfiguration.Columns)
                {
                    column.StatusChanged += column_StatusChanged;
                }
            }

            DMSAvailable = true;
            if (string.IsNullOrWhiteSpace(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_DMSTOOL)))
            {
                DMSAvailable = false;
            }
            classLCMSSettings.SettingChanged += classLCMSSettings_SettingChanged;

            // TODO: SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            // TODO: SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ColumnHandling = enumColumnDataHandling.Resort;

            try
            {
                mValidator = new classDMSSampleValidator();
                Initialize(null, null);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 2: " + ex.Message, ex);
            }
        }

        void classLCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName == "DMSTool" && e.SettingValue != string.Empty)
            {
                DMSAvailable = true;
            }
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        /// <param name="dmsView"></param>
        /// <param name="sampleQueue"></param>
        private void Initialize(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            this.WhenAnyValue(x => x.CycleColumns)
                .Subscribe(x =>
                {
                    var handling = enumColumnDataHandling.LeaveAlone;
                    if (CycleColumns)
                    {
                        handling = enumColumnDataHandling.Resort;
                    }
                    ColumnHandling = handling;

                    IterateThroughColumns = CycleColumns;
                });
            SetupCommands();

            m_selector = new formMoveToColumnSelector();

            foreach (var column in classCartConfiguration.Columns)
            {
                column.NameChanged += column_NameChanged;
                column.ColorChanged += column_ColorChanged;
            }

            DMSView = dmsView;
            SampleQueue = sampleQueue;

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
            m_filldown = new formMethodFillDown(classLCMethodManager.Manager, new List<string>());
            m_trayVial = new formTrayVialAssignment();

            // Old: mdataGrid_samples.ContextMenuStrip = mcontextMenu_options;

            //
            // Lists that hold information to be used by the sample queue combo boxes.
            //
            m_autoSamplerMethods = new List<string>();
            m_autosamplerTrays = new List<string>();
            m_instrumentMethods = new List<string>();

            //
            // Hook into the method manager so we can update list boxes when methods change...
            //
            if (classLCMethodManager.Manager != null)
            {
                classLCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
                classLCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
                classLCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;
            }

            //
            // Update Method Combo Boxes
            //
            ShowAutoSamplerMethods();
            ShowAutoSamplerTrays();
            ShowInstrumentMethods();
            ShowLCSeparationMethods();

            Samples = new ReactiveList<SampleViewModel>() { ChangeTrackingEnabled = true };
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.IsChecked))).Subscribe(x => HandleSampleValidationAndQueuing(x.Sender));
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.RequestName))).Subscribe(x => UpdateValidCell(x.Sender.Sample));
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.IsDuplicateRequestName))).Subscribe(x => HandleDuplicateRequestNameChanged(x.Sender.Sample));
            // TODO: Check for side effects
            // TODO: The idea of this is that it would detect the minor changes to the queue, where a value was changed using the databinding. There needs to be a lockout for actions not taken via the databinding, since those already handle this process...
            Samples.ItemChanged.Where(x =>
            {
                var prop = x.PropertyName;
                var obj = x.Sender;
                return prop.Equals(nameof(obj.RequestName)) ||
                       prop.Equals(nameof(obj.BatchID)) ||
                       prop.Equals(nameof(obj.BlockNumber)) ||
                       prop.Equals(nameof(obj.ColumnNumber)) ||
                       prop.Equals(nameof(obj.DatasetType)) ||
                       prop.Equals(nameof(obj.InstrumentMethod)) ||
                       prop.Equals(nameof(obj.LCMethod)) ||
                       prop.Equals(nameof(obj.PALTray)) ||
                       prop.Equals(nameof(obj.PALVial)) ||
                       prop.Equals(nameof(obj.PALVolume)) ||
                       prop.Equals(nameof(obj.RunOrder)) ||
                       prop.Equals(nameof(obj.SequenceNumber));
            }).Throttle(TimeSpan.FromSeconds(.25))
            .Subscribe(x => m_sampleQueue.UpdateSample(x.Sender.Sample));
        }

        #endregion

        #region Queue handling

        private bool currentlyProcessingQueueChange = false;

        /// <summary>
        /// Handles validation of samples and queue operations.
        /// </summary>
        /// <param name="changedSample"></param>
        private void HandleSampleValidationAndQueuing(SampleViewModel changedSample)
        {
            lock (this)
            {
                if (currentlyProcessingQueueChange)
                {
                    return;
                }
                currentlyProcessingQueueChange = true;
            }

            using (Samples.SuppressChangeNotifications())
            {
                var currentTask = "Initializing";

                try
                {
                    currentTask = "Determine number of selected items";
                    currentTask = "Examine samples";

                    if (changedSample.IsChecked)
                    {
                        // Queue samples
                        foreach (var sample in Samples)
                        {
                            // bypass samples already in running queue
                            if (sample.Sample.IsSetToRunOrHasRun)
                            {
                                continue;
                            }
                            // Validate sample and add it to the run queue

                            // Validate sample.

                            if (classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS, false))
                            {
                                var isSampleValid = mValidator.IsSampleValid(sample.Sample);
                                if (!isSampleValid)
                                {
                                    // EUS Usage for this sample is not valid; ignore for now
                                    return;
                                }
                            }
                            else
                            {
                                // DMS Sample validation is disabled
                                return;
                            }

                            // Validate other parts of the sample.
                            var errors = new List<classSampleValidationError>();
                            foreach (var reference in classSampleValidatorManager.Instance.Validators)
                            {
#if DEBUG
                                Console.WriteLine("Validating sample with validator: " +
                                                  reference.Metadata.Name);
#endif
                                var sampleValidator = reference.Value;
                                errors.AddRange(sampleValidator.ValidateSamples(sample.Sample));
                            }

                            if (errors.Count > 0)
                            {
                                //TODO: Add notifications to what was wrong with the samples.
                                return;
                            }

                            // Add to run queue
                            m_sampleQueue.MoveSamplesToRunningQueue(sample.Sample);

                            if (sample.Equals(changedSample))
                            {
                                //Stop validating and queuing samples
                                break;
                            }

                        }
                    }
                    else
                    {
                        // Dequeue samples - iterate in reverse
                        foreach (var sample in Samples.Reverse())
                        {
                            // bypass samples not set to run
                            if (!sample.Sample.IsSetToRunOrHasRun)
                            {
                                continue;
                            }
                            // Remove sample from run queue
                            m_sampleQueue.DequeueSampleFromRunningQueue(sample.Sample);

                            if (sample.Equals(changedSample))
                            {
                                // Stop removing sample from run queue
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Error in HandleSampleValidationAndQueueing, task " + currentTask, ex);
                    MessageBox.Show(
                        @"Error in HandleSampleValidationAndQueueing, task " + currentTask + @": " + ex.Message, @"Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            lock (this)
            {
                currentlyProcessingQueueChange = false;
            }
        }

        // TODO: Should this be called by HandleSampleValidationAndQueuing?
        private bool ValidateSampleRowReadyToRun(classSampleData sampleToCheck)
        {
            var isValid = true;

            if (sampleToCheck != null)
            {
                if (string.IsNullOrWhiteSpace(sampleToCheck.PAL.PALTray) || sampleToCheck.PAL.PALTray == CONST_NOT_SELECTED)
                {
                    // PAL tray not defined
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(sampleToCheck.LCMethod.Name) || sampleToCheck.LCMethod.Name == CONST_NOT_SELECTED)
                {
                    // LC Method not defined
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion

        #region LC Method Manager Events

        /// <summary>
        /// Handles removing a method from the list of available running methods.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (method == null)
                return false;

            foreach (var o in SampleViewModel.LcMethodOptions)
            {
                if (o.Equals(method))
                {
                    SampleViewModel.LcMethodOptions.Remove(method);
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private void RemoveMethodName(string methodName)
        {
            classLCMethod oFound = null;
            foreach (var o in SampleViewModel.LcMethodOptions)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    oFound = o;
                }
            }
            if (oFound != null)
            {
                SampleViewModel.LcMethodOptions.Remove(oFound);
            }
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool ContainsMethod(string methodName)
        {
            foreach (var o in SampleViewModel.LcMethodOptions)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the column ID's if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool Manager_MethodUpdated(object sender, classLCMethod method)
        {
            var samples = m_sampleQueue.GetWaitingQueue();
            var updateSamples = new List<classSampleData>();
            foreach (var sample in samples)
            {
                if (sample.LCMethod != null && sample.LCMethod.Name == method.Name)
                {
                    var newColID = method.Column;
                    if (newColID >= 0)
                    {
                        sample.ColumnData = classCartConfiguration.Columns[newColID];
                    }
                    sample.LCMethod = method;
                    updateSamples.Add(sample);
                }
            }

            m_sampleQueue.UpdateSamples(updateSamples);

            return true;
        }

        /// <summary>
        /// Handles adding a new method to the list of available running methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            // make sure the method is not null
            if (method == null)
                return false;

            // Find the method if name exists
            var found = false;
            foreach (var o in SampleViewModel.LcMethodOptions)
            {
                var name = o.ToString();
                if (name == method.Name)
                {
                    found = true;
                    break;
                }
            }

            // Update or add the method
            if (found == false)
            {
                SampleViewModel.LcMethodOptions.Add(method);
                // If we just added a sample, we want to make sure the samples have a method selected.
                if (SampleViewModel.LcMethodOptions.Count == 1)
                {
                }
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                var indexOf = SampleViewModel.LcMethodOptions.IndexOf(method);
                if (indexOf >= 0)
                    SampleViewModel.LcMethodOptions[indexOf] = method;
            }

            return true;
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        private void Undo()
        {
            using (Samples.SuppressChangeNotifications())
            {
                m_sampleQueue.Undo();
            }
        }

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        private void Redo()
        {
            using (Samples.SuppressChangeNotifications())
            {
                m_sampleQueue.Redo();
            }
        }

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        private void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            using (Samples.SuppressChangeNotifications())
            {
                if (insertIntoUnused == false)
                {
                    m_sampleQueue.QueueSamples(samples, ColumnHandling);
                }
                else
                {
                    m_sampleQueue.InsertIntoUnusedSamples(samples, ColumnHandling);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DMS View form.
        /// </summary>
        public virtual formDMSView DMSView
        {
            get { return m_dmsView; }
            set { m_dmsView = value; }
        }

        /// <summary>
        /// Gets or sets the sample queue that handles all queue management at a low level.
        /// </summary>
        public virtual classSampleQueue SampleQueue
        {
            get { return m_sampleQueue; }
            set
            {
                m_sampleQueue = value;
                if (value != null)
                {
                    m_sampleQueue.SamplesAdded += m_sampleQueue_SampleAdded;
                    m_sampleQueue.SamplesCancelled += m_sampleQueue_SampleCancelled;
                    m_sampleQueue.SamplesFinished += m_sampleQueue_SampleFinished;
                    m_sampleQueue.SamplesRemoved += m_sampleQueue_SampleRemoved;
                    m_sampleQueue.SamplesStarted += m_sampleQueue_SampleStarted;
                    m_sampleQueue.SamplesReordered += m_sampleQueue_SamplesReordered;
                    m_sampleQueue.SamplesUpdated += m_sampleQueue_SamplesUpdated;
                    m_sampleQueue.SamplesWaitingToRun += m_sampleQueue_SamplesWaitingToRun;
                    m_sampleQueue.SamplesStopped += m_sampleQueue_SamplesStopped;
                }

                IsViewEnabled = (value != null);
            }
        }

        /// <summary>
        /// Gets or sets whether when adding samples,
        /// the column data should cycle through, (e.g. 1,2,3,4,1,2)
        /// </summary>
        private bool IterateThroughColumns { get; set; }

        /// <summary>
        /// Gets or sets a list of pal method names.
        /// </summary>
        public virtual List<string> AutoSamplerMethods
        {
            get { return m_autoSamplerMethods; }
            set
            {
                m_autoSamplerMethods = value ?? new List<string>();
                ShowAutoSamplerMethods();
            }
        }

        /// <summary>
        /// Gets or sets a list of pal tray names.
        /// </summary>
        public virtual List<string> AutoSamplerTrays
        {
            get { return m_autosamplerTrays; }
            set
            {
                //classApplicationLogger.LogMessage(0, "SAMPLE VIEW PROCESSING AUTOSAMPLER TRAYS!");
                m_autosamplerTrays = value ?? new List<string>();
                ShowAutoSamplerTrays();
            }
        }

        /// <summary>
        /// Gets or sets a list of instrument method names stored on the MS instrument.
        /// </summary>
        public virtual List<string> InstrumentMethods
        {
            get { return m_instrumentMethods; }
            set
            {
                m_instrumentMethods = value ?? new List<string>();
                ShowInstrumentMethods();
            }
        }

        #endregion

        #region DataGridView Events and Methods

        private void LogSampleIdNotFound(string callingMethod, long sampleId)
        {
            var knownSampleIDs = new List<long>();
            foreach (var sample in m_sampleQueue.GetWaitingQueue())
            {
                knownSampleIDs.Add(sample.UniqueID);
            }

            foreach (var sample in m_sampleQueue.GetRunningQueue())
            {
                knownSampleIDs.Add(sample.UniqueID);
            }

            var sampleIdCount = knownSampleIDs.Count;

            string sampleQueueDescription;
            if (sampleIdCount == 0)
                sampleQueueDescription = "m_sampleQueue is empty";
            else if (sampleIdCount == 1)
                sampleQueueDescription = "m_sampleQueue has one waiting or running item, ID " + knownSampleIDs.First();
            else if (sampleIdCount < 10)
                sampleQueueDescription = "Waiting and running items in m_sampleQueue are " +
                                         string.Join(", ", knownSampleIDs);
            else
                sampleQueueDescription = "Waiting and running items in m_sampleQueue are " +
                                         string.Join(", ", knownSampleIDs.Take(9) + " ... (" + sampleIdCount + " total items)");

            var msg = callingMethod + " could not find sample ID " + sampleId + " in m_sampleQueue; " + sampleQueueDescription;
            classApplicationLogger.LogError(0, msg);

        }

        /// <summary>
        /// Updates the PAL Method Column Combo Box
        /// </summary>
        private void ShowAutoSamplerMethods()
        {
            //mcolumn_PalMethod.Items.Clear();
            //mcolumn_PalMethod.Items.Add(CONST_NOT_SELECTED);
        }

        /// <summary>
        /// Updates the PAL Tray Column Combo Box.
        /// </summary>
        private void ShowAutoSamplerTrays()
        {
            SampleViewModel.PalTrayOptions.Clear();

            foreach (var tray in m_autosamplerTrays)
            {
                SampleViewModel.PalTrayOptions.Add(tray);
            }
        }

        /// <summary>
        /// Updates the Instrument Method Column Combo Box.
        /// </summary>
        private void ShowInstrumentMethods()
        {
            SampleViewModel.InstrumentMethodOptions.Clear();

            foreach (var tray in m_instrumentMethods)
            {
                SampleViewModel.InstrumentMethodOptions.Add(tray);
            }
        }

        /// <summary>
        /// Updates the LC-Method to the LC Separation Method Box.
        /// </summary>
        private void ShowLCSeparationMethods()
        {
            SampleViewModel.LcMethodOptions.Clear();

            foreach (var method in classLCMethodManager.Manager.Methods.Values)
            {
                AddLCMethod(method);
            }
        }

        /// <summary>
        /// Adds the method to the user interface.
        /// </summary>
        /// <param name="method"></param>
        private void AddLCMethod(classLCMethod method)
        {
            SampleViewModel.LcMethodOptions.Add(method);
        }

        #endregion

        #region Utility Methods

        public void AddDateCartnameColumnIDToDatasetName()
        {
            var samples = GetSelectedSamples();
            if (samples.Count < 1)
                return;

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (samples.Count < 1)
                return;

            using (Samples.SuppressChangeNotifications())
            {
                foreach (var sample in samples)
                {
                    classSampleData.AddDateCartColumnToDatasetName(sample);
                }
                m_sampleQueue.UpdateSamples(samples);
            }

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        public void ResetDatasetName()
        {
            var samples = GetSelectedSamples();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (samples.Count < 1)
                return;

            using (Samples.SuppressChangeNotifications())
            {
                foreach (var sample in samples)
                {
                    classSampleData.ResetDatasetNameToRequestName(sample);
                }

                m_sampleQueue.UpdateSamples(samples);
            }

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
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

            if (m_autosamplerTrays.Count < 6)
            {
                classApplicationLogger.LogError(0, "Not enough PAL Trays are available.");
                return;
            }

            // TODO: if (ParentForm != null)
            // TODO:     m_trayVial.Icon = ParentForm.Icon;

            m_trayVial.LoadSampleList(m_autosamplerTrays, samples);
            if (m_trayVial.ShowDialog() == DialogResult.OK)
            {
                using (Samples.SuppressChangeNotifications())
                {
                    samples = m_trayVial.SampleList;
                    m_sampleQueue.UpdateSamples(samples);
                }
            }

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
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
            // TODO: if (ParentForm != null)
            // TODO:     m_filldown.Icon = ParentForm.Icon;

            m_filldown.InitForm(samples);

            m_filldown.StartPosition = FormStartPosition.CenterScreen;
            if (m_filldown.ShowDialog() == DialogResult.OK)
            {
                //
                // Then update the sample queue...
                //
                using (Samples.SuppressChangeNotifications())
                {
                    var newSamples = m_filldown.GetModifiedSampleList();
                    m_sampleQueue.UpdateSamples(newSamples);
                }
            }

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
        }

        /// <summary>
        /// Moves the selected samples to another column selected through a dialog window.
        /// </summary>
        private void MoveSamplesToColumn(enumColumnDataHandling handling)
        {
            m_selector.StartPosition = FormStartPosition.CenterParent;
            // TODO: if (DesignMode)
            // TODO:     return;

            if (m_selector.ShowDialog() == DialogResult.OK &&
                m_selector.SelectedColumn != formMoveToColumnSelector.CONST_NO_COLUMN_SELECTED)
            {
                var column = m_selector.SelectedColumn;
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
                    if (sample.RunningStatus == enumSampleRunningStatus.Queued && column != sample.ColumnData.ID)
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
                        sample.ColumnData = classCartConfiguration.Columns[column];
                    }

                    //
                    // Then remove them from the queue
                    //

                    //enumColumnDataHandling backFill = enumColumnDataHandling.CreateUnused;
                    //if (m_selector.InsertIntoUnused)
                    //{
                    //    backFill = enumColumnDataHandling.LeaveAlone;
                    //}

                    //m_sampleQueue.RemoveSample(ids, backFill);

                    ////
                    //// Then re-queue the samples.
                    ////
                    //try
                    //{
                    //    if (m_selector.InsertIntoUnused)
                    //    {
                    //        m_sampleQueue.InsertIntoUnusedSamples(samples, handling);
                    //    }
                    //    else
                    //    {
                    //        m_sampleQueue.UpdateSamples(
                    //        m_sampleQueue.QueueSamples(samples, handling);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    classApplicationLogger.LogError(0, "Could not queue the samples when moving between columns.", ex);
                    //}
                    if (samples.Count > 0)
                    {
                        m_sampleQueue.UpdateSamples(samples);
                    }
                }

                // Re-select the first sample
                SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(selectedSamples.First()));
            }
        }

        /// <summary>
        /// Handles when a column color has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        void column_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            using (Samples.SuppressChangeNotifications())
            {
                m_sampleQueue.UpdateAllSamples();
            }
        }

        /// <summary>
        /// Handles when a column name has been changed to update the sample queue accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
        void column_NameChanged(object sender, string name, string oldName)
        {
            using (Samples.SuppressChangeNotifications())
            {
                m_sampleQueue.UpdateWaitingSamples();
            }
        }

        /// <summary>
        /// Returns whether the sample queue has an unused samples.
        /// </summary>
        /// <returns></returns>
        private bool HasUnusedSamples()
        {
            using (Samples.SuppressChangeNotifications())
            {
                return m_sampleQueue.HasUnusedSamples();
            }
        }

        /// <summary>
        /// Displays the DMS View Dialog Window.
        /// </summary>
        private void ShowDMSView()
        {
            if (m_dmsView == null)
                return;

            var result = m_dmsView.ShowDialog();

            //
            // If the user clicks ok , then add the samples from the
            // form into the sample queue.  Don't add them directly to the
            // form so that the event model will update both this view
            // and any other views that we may have.  For the sequence
            // we don't care how we add them to the form.
            //
            if (result == DialogResult.OK)
            {
                var samples = m_dmsView.GetNewSamplesDMSView();
                m_dmsView.ClearForm();

                var insertToUnused = false;
                if (HasUnusedSamples())
                {
                    //
                    // Ask the user what to do with these samples?
                    //
                    var dialog = new formInsertOntoUnusedDialog();
                    var insertResult = dialog.ShowDialog();

                    insertToUnused = (insertResult == DialogResult.Yes);
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
        /// Copies the sample data from one object required to make a new Blank sample entry in the sample queue.
        /// </summary>
        /// <param name="sampleToCopy">Sample to copy</param>
        /// <returns>New object reference of a sample with only required data copied.</returns>
        private classSampleData CopyRequiredSampleData(classSampleData sampleToCopy)
        {
            var newSample = new classSampleData(false)
            {
                DmsData =
                {
                    RequestName = string.Format("{0}_{1:0000}",
                        m_sampleQueue.DefaultSampleName,
                        m_sampleQueue.RunningSampleIndex++),
                    CartName = sampleToCopy.DmsData.CartName,
                    CartConfigName = sampleToCopy.DmsData.CartConfigName,
                    DatasetType = sampleToCopy.DmsData.DatasetType
                },
                PAL =
                {
                    Method = sampleToCopy.PAL.Method,
                    Well = classPalData.CONST_DEFAULT_VIAL_NUMBER,
                    PALTray = CONST_NOT_SELECTED,
                    WellPlate = sampleToCopy.PAL.WellPlate
                }
            };

            // Make sure we copy the column data.  If it's null, then it's probably a special method.
            if (sampleToCopy.ColumnData != null)
            {
                var id = sampleToCopy.ColumnData.ID;
                if (id < classCartConfiguration.Columns.Count && id >= 0)
                {
                    newSample.ColumnData = classCartConfiguration.Columns[sampleToCopy.ColumnData.ID];
                }
                else
                {
                    newSample.ColumnData = classCartConfiguration.Columns[0];
                    classApplicationLogger.LogError(1,
                        string.Format("The column data from the previous sample has an invalid column ID: {0}", id));
                }
            }

            // Then we make sure that even if the column data is null, we want to
            // make sure the column is pertinent to the method, since what column we run on depends
            // on what method we are trying to run.
            if (sampleToCopy.LCMethod != null)
            {
                newSample.LCMethod = sampleToCopy.LCMethod.Clone() as classLCMethod;
                if (newSample.LCMethod != null && newSample.LCMethod.Column >= 0)
                {
                    var id = newSample.LCMethod.Column;
                    if (id < classCartConfiguration.Columns.Count && id >= 0)
                    {
                        newSample.ColumnData = classCartConfiguration.Columns[newSample.LCMethod.Column];
                    }
                    else
                    {
                        newSample.ColumnData = classCartConfiguration.Columns[0];
                        classApplicationLogger.LogError(1,
                            string.Format(
                                "The column data from the previous sample's method has an invalid column ID: {0}", id));
                    }
                }
            }
            else
            {
                newSample.LCMethod = new classLCMethod();
            }

            //
            // Clear out any DMS information from the blank
            //
            newSample.DmsData.Batch = 0;
            newSample.DmsData.Block = 0;
            newSample.DmsData.Comment = "Blank";
            newSample.DmsData.Experiment = "";
            newSample.DmsData.MRMFileID = 0;
            newSample.DmsData.ProposalID = "";
            newSample.DmsData.RequestID = 0;
            newSample.DmsData.RunOrder = 0;
            newSample.DmsData.UsageType = "";

            return newSample;
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
                foreach (var sample in SampleView.SelectedSamples)
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
            samples.RemoveAll(data => data.DmsData.DatasetName.Contains(m_sampleQueue.UnusedSampleName));
            PreviewSampleThroughput(samples);
            if (samples.Count > 0)
            {
                // Re-select the first sample
                SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samples.First()));
            }
        }

        /// <summary>
        /// Preview the selected samples on the data grid.
        /// </summary>
        public void PreviewSampleThroughput(List<classSampleData> samples)
        {
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
            }
        }

        #endregion

        #region Queue User Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        private void AddNewSample(bool insertIntoUnused)
        {
            classSampleData newData = null;

            //
            // If we have a sample, get the previous sample data.
            //
            if (Samples.Count > 0)
            {
                var data = Samples.Last().Sample;
                if (data != null)
                {
                    var actualData = m_sampleQueue.FindSample(data.UniqueID);
                    if (actualData == null)
                    {
                        LogSampleIdNotFound("AddNewSample", data.UniqueID);
                        return;
                    }

                    newData = CopyRequiredSampleData(actualData);
                }
            }
            else
            {
                //
                // Otherwise, add a new sample.
                //
                newData = new classSampleData(false)
                {
                    DmsData =
                    {
                        RequestName = string.Format("{0}_{1:0000}",
                            m_sampleQueue.DefaultSampleName,
                            m_sampleQueue.RunningSampleIndex++),
                        CartName = classCartConfiguration.CartName,
                        CartConfigName = classCartConfiguration.CartConfigName
                    }
                };
            }

            if (newData != null)
            {
                using (Samples.SuppressChangeNotifications())
                {
                    AddSamplesToManager(new List<classSampleData> {newData}, insertIntoUnused);
                }

                foreach (var sample in Samples.Reverse())
                {
                    if (sample.Sample.DmsData.RequestName.Equals(newData.DmsData.RequestName))
                    {
                        SampleView.SampleGrid.ScrollIntoView(sample);
                        SampleView.SelectedSample = sample;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        private bool AddSamplesToList(classSampleData sample)
        {
            if (sample == null)
            {
                return false;
            }
            using (Samples.SuppressChangeNotifications())
            {

                Samples.Add(new SampleViewModel(sample));
                Samples.Sort((x, y) => x.SequenceNumber.CompareTo(y.SequenceNumber));
                UpdateRow(sample);

                return true;
            }
        }

        /// <summary>
        /// Adds samples to the list but optimizes layout and updates for rendering controls.
        /// </summary>
        /// <param name="samples">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        private bool AddSamplesToList(IEnumerable<classSampleData> samples)
        {
            using (Samples.SuppressChangeNotifications())
            {
                foreach (var data in samples)
                {
                    AddSamplesToList(data);
                }
                return true;
            }
        }

        /// <summary>
        /// Clear all of the samples (deleting them from the queue as well).
        /// </summary>
        private void ClearAllSamples()
        {
            using (Samples.SuppressChangeNotifications())
            {
                //
                // Remove all of them from the sample queue.
                // This should update the other views as well.
                //
                m_sampleQueue.RemoveSample(Samples.Select(x => x.Sample.UniqueID).ToList(), enumColumnDataHandling.LeaveAlone);
            }
        }

        /// <summary>
        /// Moves all the selected samples an offset of their original sequence id.
        /// </summary>
        private void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            var data = SampleView.SelectedSamples.Select(x => x.Sample).ToList();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            data.RemoveAll(sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (data.Count < 1)
                return;

            using (Samples.SuppressChangeNotifications())
            {
                //
                // We have to make sure the data is sorted by sequence
                // number in order for the sample queue movement to work
                //
                data.Sort(new classSequenceComparer());

                //
                // Move in the sample queue
                //
                m_sampleQueue.MoveQueuedSamples(data, 0, offset, moveType);
            }

            // Re-select the first sample
            SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(data.First()));
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
            foreach (var row in SampleView.SelectedSamples)
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
                        m_sampleQueue.ReorderSamples(newSamples, enumColumnDataHandling.LeaveAlone);
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
                SampleView.SelectedSample = Samples.First(x => x.Sample.Equals(samplesToRandomize.First()));
            }
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        private void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            using (Samples.SuppressChangeNotifications())
            {
                m_sampleQueue.RemoveUnusedSamples(resortColumns);
            }
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        private void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            try
            {
                var scrollOffset = SampleView.GetCurrentScrollOffset();

                //
                // Get a list of sequence ID's to remove
                //
                var removes = new List<long>();
                var samplesToRemove = SampleView.SelectedSamples.OrderBy(x => x.SequenceNumber).ToList();
                foreach (var sample in samplesToRemove)
                {
                    removes.Add(sample.Sample.UniqueID);
                }
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

                using (Samples.SuppressChangeNotifications())
                {
                    //
                    // Remove all of them from the sample queue.
                    // This should update the other views as well.
                    //
                    m_sampleQueue.RemoveSample(removes, resortColumns);
                }

                SampleView.SetScrollOffset(scrollOffset);
                if (sampleToSelect != null)
                {
                    SampleView.SelectedSample = sampleToSelect;
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception in RemoveSelectedSamples: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Updates the provided row by determining if the sample data class is valid or not.
        /// </summary>
        /// <param name="data"></param>
        private void UpdateValidCell(classSampleData data)
        {
            //
            // Color duplicates or invalid cells with certain colors!
            //
            var validResult = m_sampleQueue.IsSampleDataValid(data);
            if (validResult == enumSampleValidResult.DuplicateRequestName &&
                !data.DmsData.DatasetName.Contains(m_sampleQueue.UnusedSampleName))
            {
                data.IsDuplicateRequestName = true;
            }
            else
            {
                data.IsDuplicateRequestName = false;
            }
        }

        private bool duplicateRequestNameProcessingLimiter = false;

        private void HandleDuplicateRequestNameChanged(classSampleData data)
        {
            lock (this)
            {
                if (duplicateRequestNameProcessingLimiter)
                {
                    return;
                }
                duplicateRequestNameProcessingLimiter = true;
            }
            if (data.IsDuplicateRequestName)
            {
                // We only need to look for duplicates matching this one's requestname
                foreach (var sample in Samples.Where(x => x.RequestName.Equals(data.DmsData.RequestName)))
                {
                    if (sample.Sample.Equals(data))
                    {
                        continue;
                    }
                    UpdateValidCell(sample.Sample);
                }
            }
            else
            {
                // This sample is no longer a duplicate, so we need to hit everything that was flagged as a duplicate name
                foreach (var sample in Samples.Where(x => x.IsDuplicateRequestName))
                {
                    if (sample.Sample.Equals(data))
                    {
                        continue;
                    }
                    UpdateValidCell(sample.Sample);
                }
            }
            lock (this)
            {
                duplicateRequestNameProcessingLimiter = false;
            }
        }

        /// <summary>
        /// Updates the row at index with the data provided.
        /// </summary>
        /// <param name="sample">Index to update.</param>
        private void UpdateRow(classSampleData sample)
        {
            UpdateValidCell(sample);
        }

        #endregion

        #region Queue Manager Event Handlers

        public Dispatcher UIDispatcher { get; set; }
        private Views.SampleView _sampleView;

        public Views.SampleView SampleView
        {
            get
            {
                return _sampleView;
            }
            set
            {
                _sampleView = value;
                if (_sampleView != null)
                {
                    _sampleView.DataContext = this;
                }
            }
        }

        /// <summary>
        /// Handles when a sample is updated somewhere and the user interface needs to be updated
        /// for that cell.
        /// </summary>
        /// <param name="sender">Queue manager that is updated.</param>
        /// <param name="data">Data arguments that contain the updated sample information.</param>
        private void m_sampleQueue_SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesUpdated), sender, data);
            }
            else
            {
                SamplesUpdated(sender, data);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Sets the pulldownbar position and updates the view of the samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SamplesStopped(object sender, classSampleQueueArgs data)
        {
            SamplesUpdated(sender, data);
        }

        /// <summary>
        /// Handled when the samples are stopped from the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void m_sampleQueue_SamplesStopped(object sender, classSampleQueueArgs data)
        {
            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesStopped), sender, data);
            }
            else
            {
                SamplesStopped(sender, data);
            }
        }

        /// <summary>
        /// Delegate defining how to update samples from another thread.
        /// </summary>
        /// <param name="samples"></param>
        private delegate void DelegateUpdateRows(IEnumerable<classSampleData> samples);

        /// <summary>
        /// Delegate defining how to add samples from another thread
        /// </summary>
        /// <param name="samples"></param>
        private delegate void DelegateSampleAdded(IEnumerable<classSampleData> samples, bool replaceExistingRows);

        /// <summary>
        /// Handles when a sample is queued for running but no open slot exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void m_sampleQueue_SamplesWaitingToRun(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
            }
            else
            {
                UpdateRows(data.Samples);
            }
        }

        /// <summary>
        /// Updates the rows for the given samples.
        /// </summary>
        /// <param name="samples">Samples to update view of.</param>
        private void UpdateRows(IEnumerable<classSampleData> samples)
        {
            foreach (var sample in samples)
            {
                UpdateRow(sample);
            }

            if (AutoScroll)
            {
                var lastCompletedRow = 0;

                var lastCompletedSample = Samples.Where(x => !x.CheckboxEnabled).DefaultIfEmpty(null).Last();
                if (lastCompletedSample != null)
                {
                    lastCompletedRow = Samples.IndexOf(lastCompletedSample);
                }

                if (lastCompletedRow > 3)
                {
                    SampleView.SetScrollOffset(lastCompletedRow - 3);
                }
            }
        }

        /// <summary>
        /// Handles when a sample is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void m_sampleQueue_SampleStarted(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
            }
            else
            {
                UpdateRows(data.Samples);
            }
        }

        /// <summary>
        /// Handle when a sample is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void m_sampleQueue_SampleRemoved(object sender, classSampleQueueArgs data)
        {
            // Start fresh and add the samples from the queue to the list.
            // But track the position of the scroll bar to be nice to the user.
            var scrollPosition = SampleView.GetCurrentScrollOffset();

            using (Samples.SuppressChangeNotifications())
            {
                Samples.Clear();
                AddSamplesToList(data.Samples);
            }

            if (Samples.Count > 0)
            {
                SampleView.SetScrollOffset(scrollPosition);
            }
        }

        /// <summary>
        /// Handles when a sample is finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void m_sampleQueue_SampleFinished(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
            }
            else
            {
                UpdateRows(data.Samples);
            }
        }

        /// <summary>
        /// Handle when a sample is cancelled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void m_sampleQueue_SampleCancelled(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
            }
            else
            {
                UpdateRows(data.Samples);
            }
        }

        /// <summary>
        /// Handle when a sample is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <param name="replaceExistingRows"></param>
        private void m_sampleQueue_SampleAdded(object sender, classSampleQueueArgs data, bool replaceExistingRows)
        {
            if (data?.Samples == null)
                return;

            if (!UIDispatcher.CheckAccess())
            {
                UIDispatcher.BeginInvoke(new DelegateSampleAdded(SamplesAddedFromQueue), data.Samples, replaceExistingRows);
            }
            else
            {
                SamplesAddedFromQueue(data.Samples, replaceExistingRows);
            }
        }

        private void SamplesAddedFromQueue(IEnumerable<classSampleData> samples, bool replaceExistingRows)
        {
            //
            // The sample queue gives all of the samples
            //
            var scrollOffset = SampleView.GetCurrentScrollOffset();

            using (Samples.SuppressChangeNotifications())
            {
                if (replaceExistingRows && Samples.Count > 0)
                {
                    try
                    {
                        Samples.Clear();
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "Ignoring exception in SamplesAddedFromQueue: " + ex.Message);
                    }
                }

                AddSamplesToList(samples);
            }

            SampleView.SetScrollOffset(scrollOffset);
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void m_sampleQueue_SamplesReordered(object sender, classSampleQueueArgs data)
        {
            var scrollPosition = SampleView.GetCurrentScrollOffset();
            using (Samples.SuppressChangeNotifications())
            {
                Samples.Clear();
                AddSamplesToList(data.Samples);
            }
            if (Samples.Count > 0)
            {
                SampleView.SetScrollOffset(scrollPosition);
            }
        }

        #endregion

        #region Column visibility

        private bool palTrayColumnVisible = true;
        private bool palVialColumnVisible = true;
        private bool volumeColumnVisible = true;
        private bool lcMethodColumnVisible = true;
        private bool instrumentMethodColumnVisible = false;
        private bool datasetTypeColumnVisible = true;
        private bool batchIdColumnVisible = false;
        private bool blockColumnVisible = false;
        private bool runOrderColumnVisible = false;

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
         *private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(ColumnHandling);
        }

        private void blankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void dMSImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void dMSImportUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

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
            var result =
                MessageBox.Show(
                    @"You are about to clear your queued samples.  Select Ok to clear, or Cancel to have no change.",
                    @"Clear Queue Confirmation", MessageBoxButtons.OKCancel);

            classApplicationLogger.LogMessage(3, "The user clicked to clear the samples");
            if (result == DialogResult.OK)
            {
                classApplicationLogger.LogMessage(3, "The user clicked to ok to clear the samples");
                ClearAllSamples();
            }
            else
            {
                classApplicationLogger.LogMessage(3, "The user clicked to cancel clearing samples");
            }
        }

        #endregion

        public void RestoreUserUIState()
        {
            SampleView.FixScrollPosition();
        }

        #region ReactiveCommands

        public ReactiveCommand AddBlankCommand { get; private set; }
        public ReactiveCommand AddBlankToUnusedCommand { get; private set; }
        public ReactiveCommand AddDMSCommand { get; private set; }
        public ReactiveCommand RemoveSelectedCommand { get; private set; }
        public ReactiveCommand FillDownCommand { get; private set; }
        public ReactiveCommand TrayVialCommand { get; private set; }
        public ReactiveCommand RandomizeCommand { get; private set; }
        public ReactiveCommand MoveDownCommand { get; private set; }
        public ReactiveCommand MoveUpCommand { get; private set; }
        public ReactiveCommand DeleteUnusedCommand { get; private set; }
        public ReactiveCommand CartColumnDateCommand { get; private set; }
        public ReactiveCommand DmsEditCommand { get; private set; }
        public ReactiveCommand UndoCommand { get; private set; }
        public ReactiveCommand RedoCommand { get; private set; }
        public ReactiveCommand PreviewThroughputCommand { get; private set; }
        public ReactiveCommand ClearAllSamplesCommand { get; private set; }

        private void SetupCommands()
        {
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
            UndoCommand = ReactiveCommand.Create(() => this.Undo());
            RedoCommand = ReactiveCommand.Create(() => this.Redo());
            PreviewThroughputCommand = ReactiveCommand.Create(() => this.PreviewSelectedThroughput());
            ClearAllSamplesCommand = ReactiveCommand.Create(() => this.ClearSamplesConfirm());
        }

        #endregion
    }
}
