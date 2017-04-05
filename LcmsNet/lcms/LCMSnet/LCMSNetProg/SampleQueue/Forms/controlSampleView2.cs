//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
// - 03/16/2009 (BLL) - Added LC and PAL Methods
// - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
// - 9/26/2014  (CJW) - Modified to use MEF for DMS and sample validations
// - 9/30/2014  (CJW) - bug fixes
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using LcmsNet.Method;
using LcmsNet.Method.Forms;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetDmsTools;
using LcmsNetSDK;
using LcmsNetSQLiteTools;
using ReactiveUI;
using LcmsNet.WPFControls.ViewModels;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Control that displays a sample list.
    /// </summary>
    public partial class controlSampleView2 : UserControl
    {
        public ReactiveList<sampleViewModel> Samples { get; private set; }

        protected formMoveToColumnSelector m_selector;

        private readonly classDMSSampleValidator mValidator;

        /// <summary>
        /// Edits the selected samples in the sample view.
        /// </summary>
        internal virtual void EditDMSData()
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
        }

        #region Constants

        /// <summary>
        /// String that should be displayed when new data is added but is not initialized.
        /// </summary>
        public const string CONST_NOT_SELECTED = "(Select)";

        /// <summary>
        /// Minimum wellplate number.
        /// </summary>
        public const int CONST_MIN_WELLPLATE = 1;

        /// <summary>
        /// Maximum wellplate number.
        /// </summary>
        public const int CONST_MAX_WELLPLATE = 1250;

        /// <summary>
        /// Minimum volume that can be injected.
        /// </summary>
        public const int CONST_MIN_VOLUME = 0;

        #endregion

        #region Members

        /// <summary>
        /// Form that provides user interface to retrieve samples from DMS.
        /// </summary>
        protected formDMSView m_dmsView;

        /// <summary>
        /// Object that manages the list of all samples.
        /// </summary>
        protected classSampleQueue m_sampleQueue;

        /// <summary>
        /// Alternating back colors to enhance user visual feedback.
        /// </summary>
        private Color[] m_colors;

        /// <summary>
        /// Default sample name when a new sample is added.
        /// </summary>
        //protected string m_defaultSampleName;
        /// <summary>
        /// Starting index of listview items that can be edited.
        /// </summary>
        protected int m_editableIndex;

        /// <summary>
        /// Value of the cell before edits.
        /// </summary>
        protected object m_cellValue;

        /// <summary>
        /// Names of the methods available on the PAL
        /// </summary>
        protected List<string> m_autoSamplerMethods;

        /// <summary>
        /// Names of the trays available on the PAL
        /// </summary>
        protected List<string> m_autosamplerTrays;

        /// <summary>
        /// Names of the instrument methods available on the MS.
        /// </summary>
        protected List<string> m_instrumentMethods;

        /// <summary>
        /// Flag that turns off the coloring when a PAL item (method, tray) was not downloadable from the PAL.
        /// </summary>
        protected bool m_ignoreMissingPALValues;

        /// <summary>
        /// Fill down form for updating lots of samples at once.
        /// </summary>
        protected formMethodFillDown m_filldown;

        /// <summary>
        /// Tray and vial assignment form.
        /// </summary>
        protected formTrayVialAssignment m_trayVial;

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        internal bool m_autoscroll = true;

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public controlSampleView2(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                InitializeComponent();
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
        public controlSampleView2()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ColumnHandling = enumColumnDataHandling.Resort;

            try
            {
                InitializeComponent();
                mValidator = new classDMSSampleValidator();
                Initialize(null, null);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 2: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        /// <param name="dmsView"></param>
        /// <param name="sampleQueue"></param>
        private void Initialize(formDMSView dmsView, classSampleQueue sampleQueue)
        {
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
            m_colors = new Color[2];
            m_colors[0] = Color.White;
            m_colors[1] = Color.Gainsboro;
            m_editableIndex = 0;

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

            m_sampleContainer.BringToFront();
            PerformLayout();

            Samples = new ReactiveList<sampleViewModel>() {ChangeTrackingEnabled = true};
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.IsChecked))).Subscribe(x => HandleSampleValidationAndQueuing(x.Sender));
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.RequestName))).Subscribe(x => UpdateValidCell(x.Sender.Sample));
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.IsDuplicateRequestName))).Subscribe(x => HandleDuplicateRequestNameChanged(x.Sender.Sample));
            // TODO: Check for side effects
            Samples.ItemChanged.Subscribe(x => m_sampleQueue.UpdateSample(x.Sender.Sample));
            mdataGrid_samples.DataContext = this;
        }

        #endregion

        #region Queue handling

        private bool currentlyProcessingQueueChange = false;

        /// <summary>
        /// Handles validation of samples and queue operations.
        /// </summary>
        /// <param name="changedSample"></param>
        private void HandleSampleValidationAndQueuing(sampleViewModel changedSample)
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
        protected virtual bool Manager_MethodRemoved(object sender, classLCMethod method)
        {
            if (method == null)
                return false;

            foreach (var o in sampleViewModel.LcMethodOptionsStr)
            {
                var name = Convert.ToString(o);
                if (name == method.Name)
                {
                    sampleViewModel.LcMethodOptionsStr.Remove(method.Name);
                    break;
                }
            }

            foreach (var o in sampleViewModel.LcMethodOptions)
            {
                if (o.Equals(method))
                {
                    sampleViewModel.LcMethodOptions.Remove(method);
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
        protected void RemoveMethodName(string methodName)
        {
            string nFound = null;
            foreach (var o in sampleViewModel.LcMethodOptionsStr)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    nFound = o;
                }
            }
            if (nFound != null)
            {
                sampleViewModel.LcMethodOptionsStr.Remove(nFound);
            }
            classLCMethod oFound = null;
            foreach (var o in sampleViewModel.LcMethodOptions)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    oFound = o;
                }
            }
            if (oFound != null)
            {
                sampleViewModel.LcMethodOptions.Remove(oFound);
            }
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        protected bool ContainsMethod(string methodName)
        {
            foreach (var o in sampleViewModel.LcMethodOptionsStr)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    return true;
                }
            }
            foreach (var o in sampleViewModel.LcMethodOptions)
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
        protected virtual bool Manager_MethodUpdated(object sender, classLCMethod method)
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
        protected virtual bool Manager_MethodAdded(object sender, classLCMethod method)
        {
            // make sure the method is not null
            if (method == null)
                return false;

            // Find the method if name exists
            var found = false;
            foreach (var o in sampleViewModel.LcMethodOptionsStr)
            {
                var name = Convert.ToString(o);
                if (name == method.Name)
                {
                    found = true;
                    break;
                }
            }

            // Update or add the method
            if (found == false)
            {
                sampleViewModel.LcMethodOptionsStr.Add(method.Name);
                // If we just added a sample, we want to make sure the samples have a method selected.
                if (sampleViewModel.LcMethodOptionsStr.Count == 1)
                {
                }
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                var indexOf = sampleViewModel.LcMethodOptionsStr.IndexOf(method.Name);
                if (indexOf >= 0)
                    sampleViewModel.LcMethodOptionsStr[indexOf] = method.Name;
            }
            found = false;
            foreach (var o in sampleViewModel.LcMethodOptions)
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
                sampleViewModel.LcMethodOptions.Add(method);
                // If we just added a sample, we want to make sure the samples have a method selected.
                if (sampleViewModel.LcMethodOptions.Count == 1)
                {
                }
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                var indexOf = sampleViewModel.LcMethodOptions.IndexOf(method);
                if (indexOf >= 0)
                    sampleViewModel.LcMethodOptions[indexOf] = method;
            }

            return true;
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        protected virtual void Undo()
        {
            m_sampleQueue.Undo();
        }

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        protected virtual void Redo()
        {
            m_sampleQueue.Redo();
        }

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        protected virtual void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
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

                Enabled = (value != null);
            }
        }

        /// <summary>
        /// Gets or sets whether when adding samples,
        /// the column data should cycle through, (e.g. 1,2,3,4,1,2)
        /// </summary>
        internal virtual bool IterateThroughColumns { get; set; }

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
        protected virtual void ShowAutoSamplerMethods()
        {
            //mcolumn_PalMethod.Items.Clear();
            //mcolumn_PalMethod.Items.Add(CONST_NOT_SELECTED);
        }

        /// <summary>
        /// Updates the PAL Tray Column Combo Box.
        /// </summary>
        protected virtual void ShowAutoSamplerTrays()
        {
            sampleViewModel.PalTrayOptions.Clear();

            foreach (var tray in m_autosamplerTrays)
            {
                sampleViewModel.PalTrayOptions.Add(tray);
            }
        }

        /// <summary>
        /// Updates the Instrument Method Column Combo Box.
        /// </summary>
        protected virtual void ShowInstrumentMethods()
        {
            //sampleViewModel.DatasetTypeOptions.Clear();
            ////sampleViewModel.DatasetTypeOptions.Add(CONST_NOT_SELECTED);
            //
            //foreach (var tray in m_instrumentMethods)
            //{
            //    sampleViewModel.DatasetTypeOptions.Add(tray);
            //}
        }

        /// <summary>
        /// Updates the LC-Method to the LC Separation Method Box.
        /// </summary>
        protected virtual void ShowLCSeparationMethods()
        {
            sampleViewModel.LcMethodOptionsStr.Clear();
            sampleViewModel.LcMethodOptions.Clear();

            foreach (var method in classLCMethodManager.Manager.Methods.Values)
            {
                AddLCMethod(method);
            }
        }

        /// <summary>
        /// Adds the method to the user interface.
        /// </summary>
        /// <param name="method"></param>
        protected virtual void AddLCMethod(classLCMethod method)
        {
            sampleViewModel.LcMethodOptionsStr.Add(method.Name);
            sampleViewModel.LcMethodOptions.Add(method);
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

            foreach (var sample in samples)
            {
                classSampleData.AddDateCartColumnToDatasetName(sample);
            }
            m_sampleQueue.UpdateSamples(samples);
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

            foreach (var sample in samples)
            {
                classSampleData.ResetDatasetNameToRequestName(sample);
            }

            m_sampleQueue.UpdateSamples(samples);
        }

        internal virtual void EditTrayAndVial()
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

            if (ParentForm != null)
                m_trayVial.Icon = ParentForm.Icon;

            m_trayVial.LoadSampleList(m_autosamplerTrays, samples);
            if (m_trayVial.ShowDialog() == DialogResult.OK)
            {
                samples = m_trayVial.SampleList;
                m_sampleQueue.UpdateSamples(samples);
            }
        }

        /// <summary>
        /// Performs fill down methods for sample data.
        /// </summary>
        internal virtual void FillDown()
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
            if (ParentForm != null)
                m_filldown.Icon = ParentForm.Icon;

            m_filldown.InitForm(samples);

            m_filldown.StartPosition = FormStartPosition.CenterScreen;
            if (m_filldown.ShowDialog() == DialogResult.OK)
            {
                //
                // Then update the sample queue...
                //
                var newSamples = m_filldown.GetModifiedSampleList();
                m_sampleQueue.UpdateSamples(newSamples);
            }
        }

        /// <summary>
        /// Moves the selected samples to another column selected through a dialog window.
        /// </summary>
        protected void MoveSamplesToColumn(enumColumnDataHandling handling)
        {
            m_selector.StartPosition = FormStartPosition.CenterParent;
            if (DesignMode)
                return;

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
        }

        /// <summary>
        /// Handles when a column color has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        void column_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            m_sampleQueue.UpdateAllSamples();
        }

        /// <summary>
        /// Handles when a column name has been changed to update the sample queue accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
        void column_NameChanged(object sender, string name, string oldName)
        {
            m_sampleQueue.UpdateWaitingSamples();
        }

        /// <summary>
        /// Returns whether the sample queue has an unused samples.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasUnusedSamples()
        {
            return m_sampleQueue.HasUnusedSamples();
        }

        /// <summary>
        /// Displays the DMS View Dialog Window.
        /// </summary>
        internal virtual void ShowDMSView()
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
        protected virtual classSampleData CopyRequiredSampleData(classSampleData sampleToCopy)
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
                foreach (var sample in mdataGrid_samples.SelectedSamples)
                {
                    samples.Add(sample.Sample);
                }

                samples.Sort((x,y) => x.SequenceID.CompareTo(y.SequenceID));
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
            samples.RemoveAll(
                data => data.DmsData.DatasetName.Contains(m_sampleQueue.UnusedSampleName));
            PreviewSampleThroughput(samples);
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
        internal virtual void AddNewSample(bool insertIntoUnused)
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
                AddSamplesToManager(new List<classSampleData> { newData }, insertIntoUnused);
            }
        }

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        protected virtual bool AddSamplesToList(classSampleData sample)
        {
            if (sample == null)
            {
                return false;
            }

            Samples.Add(new sampleViewModel(sample));
            Samples.Sort((x,y) => x.SequenceNumber.CompareTo(y.SequenceNumber));
            UpdateRow(sample);

            return true;
    }

        /// <summary>
        /// Adds samples to the list but optimizes layout and updates for rendering controls.
        /// </summary>
        /// <param name="samples">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        protected virtual bool AddSamplesToList(IEnumerable<classSampleData> samples)
        {
            foreach (var data in samples)
            {
                AddSamplesToList(data);
            }
            return true;
        }

        /// <summary>
        /// Clear all of the samples (deleting them from the queue as well).
        /// </summary>
        protected virtual void ClearAllSamples()
        {
            //
            // Remove all of them from the sample queue.
            // This should update the other views as well.
            //
            m_sampleQueue.RemoveSample(Samples.Select(x => x.Sample.UniqueID).ToList(), enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Inserts samples into unused sample positions.
        /// </summary>
        protected virtual void InsertIntoUnused()
        {
        }

        /// <summary>
        /// Moves all the selected samples an offset of their original sequence id.
        /// </summary>
        internal virtual void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            var data = mdataGrid_samples.SelectedSamples.Select(x => x.Sample).ToList();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            data.RemoveAll(
                sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (data.Count < 1)
                return;

            //
            // We have to make sure the data is sorted by sequence
            // number in order for the sample queue movement to work
            //
            data.Sort(new classSequenceComparer());

            //
            // Move in the sample queue
            //
            m_sampleQueue.MoveQueuedSamples(data, m_editableIndex, offset, moveType);
        }

        /// <summary>
        /// Randomizes the selected samples for the sample queue.
        /// </summary>
        internal void RandomizeSelectedSamples()
        {
            var samplesToRandomize = new List<classSampleData>();
            //
            // Get all the data references that we want to randomize.
            //
            foreach (var row in mdataGrid_samples.SelectedSamples)
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
                    if (ParentForm != null)
                        form.Icon = ParentForm.Icon;
                }
                catch
                {
                    classApplicationLogger.LogError(0, "No randomization plug-ins exist.");
                    return;
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newSamples = form.OutputSampleList;
                    m_sampleQueue.ReorderSamples(newSamples, enumColumnDataHandling.LeaveAlone);
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
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        internal virtual void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            m_sampleQueue.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        internal virtual void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            try
            {
                var scrollOffset = mdataGrid_samples.GetCurrentScrollOffset();

                //
                // Get a list of sequence ID's to remove
                //
                var removes = new List<long>();
                foreach (var sample in mdataGrid_samples.SelectedSamples)
                {
                    removes.Add(sample.Sample.UniqueID);
                }

                //
                // Remove all of them from the sample queue.
                // This should update the other views as well.
                //
                m_sampleQueue.RemoveSample(removes, resortColumns);

                mdataGrid_samples.SetScrollOffset(scrollOffset);
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
        protected virtual void UpdateValidCell(classSampleData data)
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
        /// Retrieves the style for the row based on sample input data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultStyle"></param>
        /// <returns></returns>
        protected virtual DataGridViewCellStyle GetRowStyleFromSample(classSampleData data,
            DataGridViewCellStyle defaultStyle)
        {
            var rowStyle = defaultStyle;
            switch (data.RunningStatus)
            {
                case enumSampleRunningStatus.Running:
                    rowStyle.BackColor = Color.Lime;
                    rowStyle.ForeColor = Color.Black;
                    rowStyle.SelectionBackColor = rowStyle.BackColor;
                    rowStyle.SelectionForeColor = rowStyle.ForeColor;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    rowStyle.ForeColor = Color.Black;
                    rowStyle.BackColor = Color.Yellow;
                    rowStyle.SelectionBackColor = rowStyle.BackColor;
                    rowStyle.SelectionForeColor = rowStyle.ForeColor;
                    break;
                case enumSampleRunningStatus.Error:
                    if (data.DmsData.Block > 0)
                    {
                        rowStyle.BackColor = Color.Orange;
                        rowStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        rowStyle.BackColor = Color.DarkRed;
                        rowStyle.ForeColor = Color.White;
                    }
                    rowStyle.SelectionForeColor = Color.White;
                    rowStyle.SelectionBackColor = Color.Navy;
                    break;
                case enumSampleRunningStatus.Stopped:
                    if (data.DmsData.Block > 0)
                    {
                        rowStyle.BackColor = Color.SeaGreen;
                        rowStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        rowStyle.BackColor = Color.Tomato;
                        rowStyle.ForeColor = Color.Black;
                    }
                    rowStyle.SelectionForeColor = Color.White;
                    rowStyle.SelectionBackColor = Color.Navy;
                    break;
                case enumSampleRunningStatus.Complete:
                    rowStyle.BackColor = Color.DarkGreen;
                    rowStyle.ForeColor = Color.White;
                    rowStyle.SelectionForeColor = Color.White;
                    rowStyle.SelectionBackColor = Color.Navy;
                    break;
                case enumSampleRunningStatus.Queued:
                    goto default;
                default:
                    rowStyle.BackColor = Color.White;
                    rowStyle.ForeColor = Color.Black;
                    rowStyle.SelectionForeColor = Color.White;
                    rowStyle.SelectionBackColor = Color.Navy;
                    break;
            }

            var status = data.ColumnData.Status;
            if (status == enumColumnStatus.Disabled)
            {
                rowStyle.BackColor = Color.FromArgb(Math.Max(0, rowStyle.BackColor.R - 128),
                    Math.Max(0, rowStyle.BackColor.G - 128),
                    Math.Max(0, rowStyle.BackColor.B - 128));
                rowStyle.ForeColor = Color.LightGray;
            }

            return rowStyle;
        }

        /// <summary>
        /// Updates the row at index with the data provided.
        /// </summary>
        /// <param name="sample">Index to update.</param>
        protected virtual void UpdateRow(classSampleData sample)
        {
            UpdateValidCell(sample);
        }

        internal void InvalidateSampleView()
        {
            mdataGrid_samples.InvalidateVisual();
            foreach (var sample in Samples)
            {
                sample.SetRowColors();
            }
        }

        #endregion

        #region Queue Manager Event Handlers

        /// <summary>
        /// Handles when a sample is updated somewhere and the user interface needs to be updated
        /// for that cell.
        /// </summary>
        /// <param name="sender">Queue manager that is updated.</param>
        /// <param name="data">Data arguments that contain the updated sample information.</param>
        protected virtual void m_sampleQueue_SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesUpdated), sender, data);
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
        protected virtual void SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Sets the pulldownbar position and updates the view of the samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void SamplesStopped(object sender, classSampleQueueArgs data)
        {
            SamplesUpdated(sender, data);
            m_sampleContainer.Refresh();
        }

        /// <summary>
        /// Handled when the samples are stopped from the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void m_sampleQueue_SamplesStopped(object sender, classSampleQueueArgs data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesStopped), sender, data);
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

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
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
        protected virtual void UpdateRows(IEnumerable<classSampleData> samples)
        {
            foreach (var sample in samples)
            {
                UpdateRow(sample);
            }

            if (m_autoscroll)
            {
                var lastCompletedRow = 0;

                var lastCompletedSample = Samples.Where(x => !x.CheckboxEnabled).DefaultIfEmpty(null).Last();
                if (lastCompletedSample != null)
                {
                    lastCompletedRow = Samples.IndexOf(lastCompletedSample);
                }

                if (lastCompletedRow > 3)
                {
                    mdataGrid_samples.SetScrollOffset(lastCompletedRow - 3);
                }
            }
        }

        /// <summary>
        /// Handles when a sample is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void m_sampleQueue_SampleStarted(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
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
        protected virtual void m_sampleQueue_SampleRemoved(object sender, classSampleQueueArgs data)
        {
            // Start fresh and add the samples from the queue to the list.
            // But track the position of the scroll bar to be nice to the user.
            var scrollPosition = mdataGrid_samples.GetCurrentScrollOffset();

            Samples.Clear();
            AddSamplesToList(data.Samples);

            if (Samples.Count > 0)
            {
                mdataGrid_samples.SetScrollOffset(scrollPosition);
            }
        }

        /// <summary>
        /// Handles when a sample is finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void m_sampleQueue_SampleFinished(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
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
        protected virtual void m_sampleQueue_SampleCancelled(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), data.Samples);
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
        protected virtual void m_sampleQueue_SampleAdded(object sender, classSampleQueueArgs data, bool replaceExistingRows)
        {
            if (data?.Samples == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateSampleAdded(SamplesAddedFromQueue), data.Samples, replaceExistingRows);
            }
            else
            {
                SamplesAddedFromQueue(data.Samples, replaceExistingRows);
            }
        }

        protected virtual void SamplesAddedFromQueue(IEnumerable<classSampleData> samples, bool replaceExistingRows)
        {
            //
            // The sample queue gives all of the samples
            //
            var scrollOffset = mdataGrid_samples.GetCurrentScrollOffset();

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

            mdataGrid_samples.SetScrollOffset(scrollOffset);
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void m_sampleQueue_SamplesReordered(object sender, classSampleQueueArgs data)
        {
            var scrollPosition = mdataGrid_samples.GetCurrentScrollOffset();
            Samples.Clear();
            AddSamplesToList(data.Samples);
            if (Samples.Count > 0)
            {
                mdataGrid_samples.SetScrollOffset(scrollPosition);
            }
        }

        #endregion

        #region Form Control Event Handlers

        /// <summary>
        /// Gets or sets how to handle samples being deleted from columns
        /// </summary>
        public enumColumnDataHandling ColumnHandling { get; set; }

        /// <summary>
        /// Deletes the specified row from the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone);
            }
        }

        /*
         * Unused
         *
        /// <summary>
        /// Handle when the user clicks to load data from DMS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_dmsImport_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }
        */

        /// <summary>
        /// Randomizes the selected samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeSelectedSamples();
        }

        /*
         * Unused methods
         *
        /// <summary>
        /// Randomizes the selected samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_randomize_Click(object sender, EventArgs e)
        {
            RandomizeSelectedSamples();
        }

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_addNew_Click(object sender, EventArgs e)
        {
            Validate();
            // Ensures that rows are valid and that we don't get any bad data from previous row when adding a new sample.
            AddNewSample(false);
        }

        /// <summary>
        /// Removes all the selected samples from the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_deleteSelected_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Moves the selected samples up by one location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_moveUp_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(-1, enumMoveSampleType.Sequence);
        }

        /// <summary>
        /// Moves the selected samples down in the waiting queue by one position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_moveDown_Click(object sender, EventArgs e)
        {
            MoveSelectedSamples(1, enumMoveSampleType.Sequence);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(ColumnHandling);
        }
        */

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
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

        /*
         * Unused methods
         *
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
        */

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedSamples(enumColumnDataHandling.LeaveAlone);
        }

        private void addBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewSample(false);
        }

        private void importFromDMSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

        private void insertBlankIntoUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewSample(true);
        }

        /*
         * Unused
         *
        private void mbutton_redo_Click(object sender, EventArgs e)
        {
            Redo();
        }
        */

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        /// <summary>
        /// Previews the throughput for the selected items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewThroughputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewSelectedThroughput();
        }

        /*
         * Unused
         *
        private void mbutton_moveSelectedSamplesToColumn_Click(object sender, EventArgs e)
        {
            MoveSamplesToColumn(enumColumnDataHandling.LeaveAlone);
        }
        */

        /// <summary>
        /// Add the data cart name and column information to the sample datasetname.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDateCartNameColumnIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

        /*
         * Unused methods
         *
        private void resetNameToOriginalRequestNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDatasetName();
        }

        private void mbutton_trayVial_Click(object sender, EventArgs e)
        {
        }

        private void mbutton_resetDatasetName_Click(object sender, EventArgs e)
        {
            ResetDatasetName();
        }

        private void mbutton_addCartNameColIDDate_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

        private void mbutton_fillDown_Click(object sender, EventArgs e)
        {
            FillDown();
        }

        private void mbutton_trayVial_Click_1(object sender, EventArgs e)
        {
            EditTrayAndVial();
        }
        */

        private void deleteUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone);
        }

        /*
         * Unused
         *
        /// <summary>
        /// Handles undo-ing any operations on the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_undo_Click(object sender, EventArgs e)
        {
            Undo();
        }
        */

        /// <summary>
        /// Handles undo-ing any operations on the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        #endregion

        public void RestoreUserUIState()
        {
            var timer = new System.Threading.Timer(FixScrollPosition, this, 50, System.Threading.Timeout.Infinite);
        }

        private void FixScrollPosition(object obj)
        {
            this.BeginInvoke(new Action(mdataGrid_samples.FixScrollPosition));
        }
    }
}
