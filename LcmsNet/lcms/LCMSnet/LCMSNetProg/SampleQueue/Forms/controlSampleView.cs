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

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Control that displays a sample list.
    /// </summary>
    public partial class controlSampleView : UserControl
    {
        private const int ROWID_CELL_INDEX = 7;

        protected formMoveToColumnSelector m_selector;

        private readonly classDMSSampleValidator mValidator;

        /// <summary>
        /// Edits the selected samples in the sample view.
        /// </summary>
        protected virtual void EditDMSData()
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
        /// Index Offset for going from a zero based array for configuration data to the user
        /// readable column display.
        /// </summary>
        protected const int CONST_COLUMN_INDEX_OFFSET = 1;

        /// <summary>
        /// Index of check box indicating sample selection
        /// </summary>
        protected const int CONST_COLUMN_CHECKED = 0;

        /// <summary>
        /// Index Offset for going from a zero based array for configuration data to the user
        /// readable column display.
        /// </summary>
        protected const int CONST_COLUMN_STATUS = 1;

        /// <summary>
        /// Index of sequence number id column.
        /// </summary>
        protected const int CONST_COLUMN_SEQUENCE_ID = 2;

        /// <summary>
        /// Index of Column ID column, not a typo
        /// </summary>
        protected const int CONST_COLUMN_COLUMN_ID = 3;

        /// <summary>
        /// Index of Unique ID column.
        /// </summary>
        protected const int CONST_COLUMN_UNIQUE_ID = 4;

        /// <summary>
        /// Index of the block data (from DMS) column.
        /// </summary>
        protected const int CONST_COLUMN_BLOCK = 5;

        /// <summary>
        /// Index of DMS Run order column.
        /// </summary>
        protected const int CONST_COLUMN_RUN_ORDER = 6;

        /// <summary>
        /// Index of Request Name column
        /// Initially shows request name, but changed to dataset name if it is updated
        /// </summary>
        protected const int CONST_COLUMN_REQUEST_NAME = 7;

        /// <summary>
        /// Index of pal tray column
        /// </summary>
        protected const int CONST_COLUMN_PAL_TRAY = 8;

        /// <summary>
        /// Index of pal vial column
        /// </summary>
        protected const int CONST_COLUMN_PAL_VIAL = 9;

        /// <summary>
        /// Index of volume column.
        /// </summary>
        protected const int CONST_COLUMN_VOLUME = 10;

        /// <summary>
        /// Index of experiment method column (aka LC method)
        /// </summary>
        protected const int CONST_COLUMN_EXPERIMENT_METHOD = 11;

        /// <summary>
        /// Index of instrument method column.
        /// </summary>
        protected const int CONST_COLUMN_INSTRUMENT_METHOD = 12;

        /// <summary>
        /// Index of dataset type column.
        /// </summary>
        protected const int CONST_COLUMN_DATASET_TYPE = 13;

        /// <summary>
        /// Index of batch ID column.
        /// </summary>
        protected const int CONST_COLUMN_BATCH_ID = 14;

        /// <summary>
        /// String that should be displayed when new data is added but is not initialized.
        /// </summary>
        protected const string CONST_NOT_SELECTED = "(Select)";

        /// <summary>
        /// Minimum wellplate number.
        /// </summary>
        protected const int CONST_MIN_WELLPLATE = 1;

        /// <summary>
        /// Maximum wellplate number.
        /// </summary>
        protected const int CONST_MAX_WELLPLATE = 1250;

        /// <summary>
        /// Minimum volume that can be injected.
        /// </summary>
        protected const int CONST_MIN_VOLUME = 0;

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
        /// Last position of the sample that is queued used to track how the user is queuing and dequeuing samples.
        /// </summary>
        private int m_lastQueuedSamplePosition;

        /// <summary>
        /// First position of the pulldown bar to track what samples can be queued or edited.
        /// </summary>
        private int m_firstQueuedSamplePosition;

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        protected bool m_autoscroll = true;

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public controlSampleView(formDMSView dmsView, classSampleQueue sampleQueue)
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
            finally
            {
                mdataGrid_samples.RowHeadersVisible = false;
            }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        public controlSampleView()
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
            finally
            {
                mdataGrid_samples.RowHeadersVisible = false;
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

            mdataGrid_samples.ContextMenuStrip = mcontextMenu_options;

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

            //
            // Add the dataset type items to the data grid
            //
            try
            {
                var datasetTypes = classSQLiteTools.GetDatasetTypeList(false);
                foreach (var datasetType in datasetTypes)
                {
                    mcolumn_datasetType.Items.Add(datasetType);
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(1, "The sample queue could not load the dataset type list.", ex);
            }

            //
            // Events to make sure the editing is done correctly.
            //
            mdataGrid_samples.CellBeginEdit += mdataGrid_samples_CellBeginEdit;
            mdataGrid_samples.CellEndEdit += mdataGrid_samples_CellEndEdit;
            mdataGrid_samples.DataError += mdataGrid_samples_DataError;
            mdataGrid_samples.KeyUp += mdataGrid_samples_KeyUp;

            // Make sure to set the styles/data for a row before it is displayed
            mdataGrid_samples.RowPrePaint += RowPrePaint;

            DisplayColumn(CONST_COLUMN_COLUMN_ID, true);
            DisplayColumn(CONST_COLUMN_DATASET_TYPE, true);
            DisplayColumn(CONST_COLUMN_BLOCK, false);
            DisplayColumn(CONST_COLUMN_RUN_ORDER, false);
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, true);
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, true);
            DisplayColumn(CONST_COLUMN_PAL_TRAY, true);
            DisplayColumn(CONST_COLUMN_PAL_VIAL, true);
            DisplayColumn(CONST_COLUMN_VOLUME, true);
            DisplayColumn(CONST_COLUMN_BATCH_ID, false);
            DisplayColumn(CONST_COLUMN_UNIQUE_ID, false);

            mdataGrid_samples.CellFormatting += mdataGrid_samples_CellFormatting;
            mdataGrid_samples.BringToFront();
            m_sampleContainer.BringToFront();
            PerformLayout();
        }

        /// <summary>
        /// Enables whether the queue handler will be enabled or not.
        /// </summary>
        /// <param name="state"></param>
        protected void EnableQueueing(bool state)
        {
            if (state)
            {
                //
                // Handles for queuing data!
                //
                // Use the CellClick event so we can make the ComboBoxes drop down properly.
                //mdataGrid_samples.CellContentClick += DataGridViewCellContentClicked;
                mdataGrid_samples.CellClick += DataGridViewCellContentClicked;
                mdataGrid_samples.CellEndEdit += InvalidateGridView;
            }
            else
            {
                mdataGrid_samples.Dock = DockStyle.Fill;
                mdataGrid_samples.SendToBack();
            }
        }

        #endregion

        #region Queue handling

        /// <summary>
        /// Determines if a queue can be pulled down or not.
        /// </summary>
        /// <returns></returns>
        bool SelectionChangeValid(int rowNum, classSampleData sample, bool doNotValidate = false)
        {
            // Make sure we aren't at the end of the queue.
            var N = mdataGrid_samples.Rows.Count;
            if (rowNum <= N/* * m_sampleItemSize*/)
            {
                var currentSample =
                    /*Convert.ToInt32(Math.Round(Convert.ToDouble(y) / Convert.ToDouble(m_sampleItemSize))) +
                    mdataGrid_samples.FirstDisplayedScrollingRowIndex*/ rowNum;

                if (currentSample < 1 || currentSample > mdataGrid_samples.Rows.Count)
                    return false;
                if (!doNotValidate)
                {
                    /////// This is very, very expensive...... ///////
                    Validate();
                }
                //Validation call ensures that any changes to datagridview have been commited, and that valid values exist in all rows.
                //classSampleData sample = RowToSample(mdataGrid_samples.Rows[currentSample - 1]);

                // Dont let us re-run something that has been run, errored, or is currently running.
                if (sample.RunningStatus != enumSampleRunningStatus.Queued
                    && sample.RunningStatus != enumSampleRunningStatus.WaitingToRun)
                {
                    return false;
                }

                // Validate sample.

                if (classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS, false))
                {
                    var isSampleValid = mValidator.IsSampleValid(sample);
                    if (!isSampleValid && false)
                    {
                        return false;
                    }
                }
                else
                {
                    // DMS sample validation is disabled
                    return false;
                }

                // Validate other parts of the sample.
                var errors = new List<classSampleValidationError>();
                foreach (
                    var reference in
                        classSampleValidatorManager.Instance.Validators)
                {
#if DEBUG
                    Console.WriteLine("Validating sample with validator: " + reference.Metadata.Name);
#endif
                    var sampleValidator = reference.Value;
                    errors.AddRange(sampleValidator.ValidateSamples(sample));
                }
                if (errors.Count > 0)
                {
                    //TODO: Add notifications to what was wrong with the samples.
                    return false;
                }

                return true;
            }
            // We were at the end of the rope.
            return false;
        }

        /// <summary>
        /// Handles validation of samples and queue operations.
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="isRecursive"></param>
        private void HandleSampleValidationAndQueuing(int rowNum, bool isRecursive)
        {
            //var y = mdataGrid_samples.Rows.IndexOf(row) + 1;
            var currentTask = "Initializing";

            // Make sure we aren't at the end of the queue.
            var N = mdataGrid_samples.Rows.Count;

            try
            {
                currentTask = "Determine number of selected items";
                var offset = mdataGrid_samples.FirstDisplayedScrollingRowIndex;

                // Find the number of items that are selected.
                if (rowNum <= N)
                {
                    if (rowNum < 0 || rowNum > mdataGrid_samples.Rows.Count)
                        return;
                    if (!isRecursive && !(rowNum <= m_firstQueuedSamplePosition || m_lastQueuedSamplePosition > rowNum))
                    {
                        ////// This is very, very expensive...... //////
                        Validate();

                    }
                }

                if (Math.Abs(m_lastQueuedSamplePosition - rowNum) > 1)
                {
                    // process: if more than 1 selected, ask user for confirmation; if confirmed, recursively call this function?
                    var isDequeue = m_lastQueuedSamplePosition - rowNum > 0;
                    if (isDequeue) // dequeuing samples
                    {
                        HandleSampleValidationAndQueuing(rowNum + 1, true);
                    }
                    else // queuing samples
                    {
                        HandleSampleValidationAndQueuing(rowNum - 1, true);
                    }
                    //return;
                }

                currentTask = "Examine samples";
                int neededRowId;
                if (rowNum <= m_firstQueuedSamplePosition || m_lastQueuedSamplePosition > rowNum)
                {
                    neededRowId = Convert.ToInt32(mdataGrid_samples.Rows[rowNum].Cells[CONST_COLUMN_UNIQUE_ID].Value);
                }
                else
                {
                    neededRowId = Convert.ToInt32(mdataGrid_samples.Rows[rowNum - 1].Cells[CONST_COLUMN_UNIQUE_ID].Value);
                }
                var sample = m_sampleQueue.FindSample(neededRowId);
                if (sample == null)
                {
                    LogSampleIdNotFound("HandleSampleValidationAndQueuing", neededRowId);
                    return;
                }

                if (rowNum <= m_firstQueuedSamplePosition)
                {
                    if (rowNum < m_lastQueuedSamplePosition && rowNum >= m_firstQueuedSamplePosition)
                    {
                        //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples]);
                        m_sampleQueue.DequeueSampleFromRunningQueue(sample);
                    }
                    if (!isRecursive)
                    {
                        m_lastQueuedSamplePosition = Math.Max(0, rowNum);
                        m_sampleContainer.Refresh();
                    }
                    return;
                }
                if (m_lastQueuedSamplePosition > rowNum)
                {
                    //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples]);
                    m_sampleQueue.DequeueSampleFromRunningQueue(sample);

                    if (!isRecursive)
                    {
                        m_lastQueuedSamplePosition = rowNum;
                        m_sampleContainer.Refresh();
                    }
                    //m_sampleContainer.Refresh();
                    return;
                }

                currentTask = "Call to CanPullDownQueueBar";
                //bool canPullDown = SelectionChangeValid(y/* - m_columnSize*/, tsample, isRecursive);
                //
                //if (!canPullDown)
                //{
                //    return;
                //}
                if (rowNum <= N)
                {
                    var currentSample = rowNum;

                    if (currentSample < 1 || currentSample > mdataGrid_samples.Rows.Count)
                        return;
                    //if (!isRecursive)
                    //{
                    //    /////// This is very, very expensive...... ///////
                    //    this.Validate();
                    //}
                    //Validation call ensures that any changes to datagridview have been commited, and that valid values exist in all rows.
                    //classSampleData sample = RowToSample(mdataGrid_samples.Rows[currentSample - 1]);

                    // Dont let us re-run something that has been run, errored, or is currently running.
                    if (sample.RunningStatus != enumSampleRunningStatus.Queued
                        && sample.RunningStatus != enumSampleRunningStatus.WaitingToRun)
                    {
                        return;
                    }

                    // Validate sample.

                    if (classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS, false))
                    {
                        var isSampleValid = mValidator.IsSampleValid(sample);
                    }
                    else
                    {
                        // DMS Sample validation is disabled
                        return;
                    }

                    // Validate other parts of the sample.
                    var errors = new List<classSampleValidationError>();
                    foreach (
                        var reference in
                            classSampleValidatorManager.Instance.Validators)
                    {
#if DEBUG
                        Console.WriteLine("Validating sample with validator: " +
                                                           reference.Metadata.Name);
#endif
                        var sampleValidator = reference.Value;
                        errors.AddRange(sampleValidator.ValidateSamples(sample));
                    }
                    if (errors.Count > 0)
                    {
                        //TODO: Add notifications to what was wrong with the samples.
                        return;
                    }
                }
                else
                {
                    // All samples processed
                    return;
                }

                currentTask =
                    "Handle queueing the samples by first tracking the current sample index, then seeing if it changed from last to current";

                // Handle queueing the samples by first tracking the current sample index, then
                // seeing if it changed from last to current.
                if (m_lastQueuedSamplePosition < rowNum)
                {
                    //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples - 1]);
                    m_sampleQueue.MoveSamplesToRunningQueue(sample);
                }

                if (!isRecursive)
                {
                    m_lastQueuedSamplePosition = rowNum;
                    m_sampleContainer.Refresh();
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

        void InvalidateGridView(object sender, DataGridViewCellEventArgs e)
        {
            InvalidateGridView();
        }

        protected void InvalidateGridView()
        {
            mdataGrid_samples.ResetBindings();
            mdataGrid_samples.Invalidate();
        }

        void DataGridViewCellContentClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == CONST_COLUMN_CHECKED)
            {
                var row = mdataGrid_samples.CurrentRow;
                var checkbox = row?.Cells[CONST_COLUMN_CHECKED] as DataGridViewCheckBoxCell;
                if (checkbox == null)
                {
                    return;
                }
                //enumCheckboxStatus gridEditState =
                //    GetCheckboxStatusFromCheckbox(checkbox, checkbox.EditedFormattedValue); // use checkbox.EditedFormattedValue
                var gridSavedState = GetCheckboxStatusFromCheckbox(checkbox, checkbox.Value);
                if (gridSavedState != enumCheckboxStatus.Disabled)
                {
                    if (gridSavedState == enumCheckboxStatus.Unchecked /*&& gridState == enumCheckboxStatus.Checked*/)
                    {
                        HandleSampleValidationAndQueuing(mdataGrid_samples.Rows.IndexOf(row) + 1, false);
                    }
                    else if (gridSavedState == enumCheckboxStatus.Checked /*&& gridState == enumCheckboxStatus.Unchecked*/)
                    {
                        HandleSampleValidationAndQueuing(mdataGrid_samples.Rows.IndexOf(row), false);
                    }
                    // Other conditions: no change (disabled cannot be unset, and no change for state to same state
                }
            }
            //else if (e.ColumnIndex == CONST_COLUMN_PAL_TRAY || e.ColumnIndex == CONST_COLUMN_INSTRUMENT_METHOD ||
            //         e.ColumnIndex == CONST_COLUMN_INSTRUMENT_METHOD || e.ColumnIndex == CONST_COLUMN_DATASET_TYPE)
            else if (mdataGrid_samples.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                var row = mdataGrid_samples.CurrentRow;
                var comboBox = row?.Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (comboBox == null)
                {
                    return;
                }
                try
                {
                    comboBox.Selected = true;
                    mdataGrid_samples.BeginEdit(true);
                    ((DataGridViewComboBoxEditingControl)mdataGrid_samples.EditingControl).DroppedDown = true;
                }
                catch (Exception)
                {
                    mdataGrid_samples.CancelEdit();
                }
            }
        }

        void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            UpdateRow(e.RowIndex);
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
                return true;

            foreach (var o in mcolumn_LCMethod.Items)
            {
                var name = Convert.ToString(o);
                if (name == method.Name)
                {
                    mcolumn_LCMethod.Items.Remove(method.Name);
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
            object oFound = null;
            foreach (var o in mcolumn_LCMethod.Items)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    oFound = o;
                }
            }
            if (oFound != null)
            {
                mcolumn_LCMethod.Items.Remove(oFound);
            }
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        protected bool ContainsMethod(string methodName)
        {
            foreach (var o in mcolumn_LCMethod.Items)
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
            // make sure the method is not null...we dont like this
            if (method == null)
                return true;

            // Find the method if name exists
            var found = false;
            foreach (var o in mcolumn_LCMethod.Items)
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
                mcolumn_LCMethod.Items.Add(method.Name);
                // If we just added a guy, then we want to
                // make sure the samples have a method selected.
                if (mcolumn_LCMethod.Items.Count == 1)
                {
                }
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                var indexOf = mcolumn_LCMethod.Items.IndexOf(method.Name);
                if (indexOf >= 0)
                    mcolumn_LCMethod.Items[indexOf] = method;
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
        protected virtual bool IterateThroughColumns { get; set; }

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

        #region Column Display Context Menu Handlers

        /// <summary>
        /// Toggles the display the the PAL Tray column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pALTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_PAL_TRAY, pALTrayToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the view of the pal vial column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pALVialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_PAL_VIAL, pALVialToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the views of the volume column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_VOLUME, volumeToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the LC Method column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lCMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_EXPERIMENT_METHOD, lCMethodToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the dataset type column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datasetTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_DATASET_TYPE, datasetTypeToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the batch id column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void batchIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_BATCH_ID, batchIDToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the instrument column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instrumentMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_INSTRUMENT_METHOD, instrumentMethodToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the block column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_BLOCK, blockToolStripMenuItem.Checked == false);
        }

        /// <summary>
        /// Toggles the run order column view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayColumn(CONST_COLUMN_RUN_ORDER, runOrderToolStripMenuItem.Checked == false);
        }

        #endregion

        #region DataGridView Events and Methods

        /// <summary>
        /// Handles retarded data grid view errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Ignore retarded MS formatting issues
            if (e.Context == DataGridViewDataErrorContexts.Formatting)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Hides or Shows the suggested column.
        /// </summary>
        /// <param name="column">Column to show or hide.</param>
        /// <param name="visible">Flag indiciating if it should be visible or not.  True is visible.  False is invisible.</param>
        protected void DisplayColumn(int column, bool visible)
        {
            //
            // The reason why we do this is because we programmatically want to change
            // what columns are visible.  This makes it easier to the outside caller to do so
            // and also allows us to maintain this modification to the user interface
            // without having to track independent individual user clicks.
            //
            // If we embedded this code into
            //
            switch (column)
            {
                case CONST_COLUMN_PAL_TRAY:
                    pALTrayToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_PAL_VIAL:
                    pALVialToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_VOLUME:
                    volumeToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_EXPERIMENT_METHOD:
                    lCMethodToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_INSTRUMENT_METHOD:
                    instrumentMethodToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_DATASET_TYPE:
                    datasetTypeToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_BATCH_ID:
                    batchIDToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_BLOCK:
                    blockToolStripMenuItem.Checked = visible;
                    break;
                case CONST_COLUMN_RUN_ORDER:
                    runOrderToolStripMenuItem.Checked = visible;
                    break;
            }
            mdataGrid_samples.Columns[column].Visible = visible;
        }

        /// <summary>
        /// Validates the input from the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                mdataGrid_samples.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                var row = mdataGrid_samples.Rows[e.RowIndex];

                //
                // Get the new value
                //
                var cellData = row.Cells[e.ColumnIndex].Value;

                //
                // Make sure the value is not null, and that the user has selected an item.
                //
                if (cellData == null)
                {
                    //
                    // Revert back to the old data.
                    //
                    row.Cells[e.ColumnIndex].Value = m_cellValue;
                    classApplicationLogger.LogMessage(5, "Sample View Grid Data was null.");
                    return;
                }
                if (cellData.ToString() == CONST_NOT_SELECTED)
                {
                    //
                    // Revert back to the old data.
                    //
                    row.Cells[e.ColumnIndex].Value = m_cellValue;
                    return;
                }

                //
                // Find the sample in the queue.
                //
                var uniqueID = Convert.ToInt64(row.Cells[CONST_COLUMN_UNIQUE_ID].Value);
                var data = m_sampleQueue.FindSample(uniqueID);
                if (data == null)
                {
                    LogSampleIdNotFound("mdataGrid_samples_CellEndEdit", uniqueID);
                    return;
                }

                bool success;

                //
                // Update the sample data
                //
                switch (e.ColumnIndex)
                {
                    case CONST_COLUMN_REQUEST_NAME:
                        data.DmsData.DatasetName = Convert.ToString(cellData);
                        break;
                    case CONST_COLUMN_EXPERIMENT_METHOD:
                        //
                        // Make sure that we have a valid method here.
                        //
                        var name = Convert.ToString(cellData);
                        if (classLCMethodManager.Manager.Methods.ContainsKey(name))
                        {
                            var tempMethod = classLCMethodManager.Manager.Methods[name];
                            if (tempMethod.Column != data.ColumnData.ID)
                            {
                                if (tempMethod.Column >= 0)
                                {
                                    data.ColumnData = classCartConfiguration.Columns[tempMethod.Column];
                                }
                            }
                            data.LCMethod = tempMethod;
                        }
                        break;
                    case CONST_COLUMN_PAL_TRAY:
                        data.PAL.PALTray = Convert.ToString(cellData);
                        break;
                    case CONST_COLUMN_PAL_VIAL:
                        int vial;
                        success = int.TryParse(cellData.ToString(), out vial);
                        if (success)
                        {
                            data.PAL.Well = Math.Max(CONST_MIN_WELLPLATE,
                                Math.Min(vial, CONST_MAX_WELLPLATE));
                        }
                        break;
                    case CONST_COLUMN_VOLUME:
                        double volume;
                        success = double.TryParse(cellData.ToString(), out volume);
                        if (success)
                        {
                            data.Volume = Math.Max(CONST_MIN_VOLUME, volume);
                        }
                        break;
                    case CONST_COLUMN_INSTRUMENT_METHOD:
                        data.InstrumentData.MethodName = Convert.ToString(cellData);
                        break;
                    case CONST_COLUMN_DATASET_TYPE:
                        data.DmsData.DatasetType = Convert.ToString(cellData);
                        break;
                }

                m_sampleQueue.UpdateSample(data);

            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception in mdataGrid_samples_CellEndEdit: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Copies the current value of the cell that is being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == CONST_COLUMN_PAL_VIAL)
                {
                }

                var uniqueID = Convert.ToInt64(mdataGrid_samples.Rows[e.RowIndex].Cells[CONST_COLUMN_UNIQUE_ID].Value);
                var data = m_sampleQueue.FindSample(uniqueID);
                if (data == null)
                {
                    LogSampleIdNotFound("mdataGrid_samples_CellBeginEdit", uniqueID);
                    return;
                }

                if (data.RunningStatus != enumSampleRunningStatus.Queued)
                {
                    e.Cancel = true;
                    return;
                }

                mdataGrid_samples.SelectionMode = DataGridViewSelectionMode.CellSelect;

                switch (e.ColumnIndex)
                {
                    default:
                        m_cellValue = mdataGrid_samples.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        break;
                }

            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception in mdataGrid_samples_CellBeginEdit: " + ex.Message, ex);
            }

        }

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
            mcolumn_PalTray.Items.Clear();
            mcolumn_PalTray.Items.Add(CONST_NOT_SELECTED);

            foreach (var tray in m_autosamplerTrays)
            {
                mcolumn_PalTray.Items.Add(tray);
            }
        }

        /// <summary>
        /// Updates the Instrument Method Column Combo Box.
        /// </summary>
        protected virtual void ShowInstrumentMethods()
        {
            mcolumn_instrumentMethod.Items.Clear();
            mcolumn_instrumentMethod.Items.Add(CONST_NOT_SELECTED);

            foreach (var tray in m_instrumentMethods)
            {
                mcolumn_instrumentMethod.Items.Add(tray);
            }
        }

        /// <summary>
        /// Updates the LC-Method  to the LC Separate Method Box.
        /// </summary>
        protected virtual void ShowLCSeparationMethods()
        {
            mcolumn_LCMethod.Items.Clear();
            mcolumn_LCMethod.Items.Add(CONST_NOT_SELECTED);

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
            mcolumn_LCMethod.Items.Add(method.Name);
        }

        /// <summary>
        /// Handles formatting the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == CONST_COLUMN_SEQUENCE_ID)
            {
                e.Value = string.Format("{0:0000}", Convert.ToInt32(e.Value));
            }
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
            samples.RemoveAll(
                sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

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
            samples.RemoveAll(
                sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

            if (samples.Count < 1)
                return;

            foreach (var sample in samples)
            {
                classSampleData.ResetDatasetNameToRequestName(sample);
            }

            m_sampleQueue.UpdateSamples(samples);
        }

        protected virtual void EditTrayAndVial()
        {
            var samples = GetSelectedSamples();
            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

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
        protected virtual void FillDown()
        {
            //
            // Get the list of selected samples
            //
            var samples = GetSelectedSamples();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                sample => sample.RunningStatus != enumSampleRunningStatus.Queued);

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
        /// Lookup the value in column rowIdColIndex of the current row in mDataGrid_Samples, returning that as a string
        /// </summary>
        /// <param name="getAdjacentRow">Find the adjacent row instead of the current row; used when deleting a row</param>
        /// <param name="currentColIndex">Output: currently selected column</param>
        /// <param name="scrollPosition">Output: the row index of the first visible row</param>
        /// <returns>Row Id string</returns>
        private string GetCurrentDataGridPosition(bool getAdjacentRow, out int currentColIndex, out int scrollPosition)
        {
            scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);

            string selectedRowId;
            var rowIndexToUse = mdataGrid_samples.CurrentRow?.Index ?? 0;

            if (getAdjacentRow)
            {
                if (rowIndexToUse == 0)
                    rowIndexToUse++;
                else
                    rowIndexToUse--;
            }

            if (mdataGrid_samples.Rows.Count > rowIndexToUse && mdataGrid_samples.Rows[rowIndexToUse].Cells.Count > ROWID_CELL_INDEX)
            {
                selectedRowId = mdataGrid_samples.Rows[rowIndexToUse].Cells[ROWID_CELL_INDEX].Value.ToString();
            }
            else
            {
                selectedRowId = string.Empty;
            }

            currentColIndex = mdataGrid_samples.CurrentCell?.ColumnIndex ?? 0;

            return selectedRowId;
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
                // Make sure the samples can actually run, e.g. dont put a sample on column 2 already back onto column 2.
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
                var ids = new List<long>();
                foreach (var sample in samples)
                {
                    ids.Add(sample.UniqueID);
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
        /// Move the selection back to a saved row and cell in the data grid
        /// </summary>
        /// <param name="selectedRowId"></param>
        /// <param name="currentColIndex"></param>
        /// <param name="scrollPosition"></param>
        private void RestoreSelectedDataGridCell(string selectedRowId, int currentColIndex, int scrollPosition)
        {
            var count = mdataGrid_samples.Rows.Count;
            if (count <= 0)
                return;

            mdataGrid_samples.FirstDisplayedScrollingRowIndex = Math.Min(scrollPosition, count);

            // Make sure the desired row is selected
            if (string.IsNullOrWhiteSpace(selectedRowId))
                return;

            var cellSelected = false;
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                if (row.Cells.Count > ROWID_CELL_INDEX &&
                    row.Cells[ROWID_CELL_INDEX].Value.ToString() == selectedRowId &&
                    row.Visible &&
                    !cellSelected)
                {
                    if (mdataGrid_samples.Rows[row.Index].Cells.Count > currentColIndex && row.Cells[currentColIndex].Visible)
                    {
                        mdataGrid_samples.CurrentCell = row.Cells[currentColIndex];
                        //row.Selected = true;
                        cellSelected = true;
                    }
                    else if (row.Cells[ROWID_CELL_INDEX].Visible)
                    {
                        mdataGrid_samples.CurrentCell = row.Cells[ROWID_CELL_INDEX];
                        //row.Selected = true;
                        cellSelected = true;
                    }

                }
                else if (row.Selected)
                    row.Selected = false;
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
        protected virtual void ShowDMSView()
        {
            if (m_dmsView == null) return;

            var result = m_dmsView.ShowDialog();

            //
            // If the user clicks ok , then add the samples from the
            // form into the sample queue.  Don't add them directly to the
            // form so that the event model will update both this view
            // and any other views that we may have.  For the sequence
            // we dont care how we add them to the form.
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
                newSample.LCMethod = null;
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
        /// Returns a list of all queued samples
        /// </summary>
        /// <returns></returns>
        private List<classSampleData> GetAllSamples()
        {
            var samples = new List<classSampleData>();
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                var data = RowToSample(row);
                samples.Add(data);
            }

            samples.Sort(new classAscendingSampleIDComparer());

            return samples;
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
                foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
                {
                    var data = RowToSample(row);
                    samples.Add(data);
                }

                samples.Sort(new classAscendingSampleIDComparer());
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
        protected virtual void AddNewSample(bool insertIntoUnused)
        {
            classSampleData newData = null;

            //
            // If we have a sample, get the previous sample data.
            //
            if (mdataGrid_samples.Rows.Count > 0)
            {
                var row = mdataGrid_samples.Rows[mdataGrid_samples.Rows.Count - 1];
                var data = RowToSample(row);
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

            var found = false;
            for (var i = 0; i < mdataGrid_samples.Rows.Count; i++)
            {
                var sampleNext = RowToSample(mdataGrid_samples.Rows[i]);
                if (sample.SequenceID < sampleNext.SequenceID &&
                    sampleNext.RunningStatus == enumSampleRunningStatus.Queued)
                {
                    found = true;
                    sampleToRowTranslatorBindingSource.List.Insert(i, new SampleToRowTranslator(sample));
                    UpdateRow(i);
                    break;
                }
            }

            if (!found)
            {
                sampleToRowTranslatorBindingSource.List.Add(new SampleToRowTranslator(sample));
                if (mdataGrid_samples.RowCount > 0)
                    UpdateRow(mdataGrid_samples.RowCount - 1);
            }

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
            // Get a list of sequence ID's to remove
            //
            var removes = new List<long>();
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                removes.Add((long)row.Cells[CONST_COLUMN_UNIQUE_ID].Value);
            }

            //
            // Remove all of them from the sample queue.
            // This should update the other views as well.
            //
            m_sampleQueue.RemoveSample(removes, enumColumnDataHandling.LeaveAlone);
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
        protected virtual void MoveSelectedSamples(int offset, enumMoveSampleType moveType)
        {
            var items = new DataGridViewRow[mdataGrid_samples.SelectedRows.Count];
            mdataGrid_samples.SelectedRows.CopyTo(items, 0);
            var data = ConvertRowsToData(items);


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

            //
            // By default the first cell wants to be selected.
            //
            mdataGrid_samples.Rows[0].Selected = false;

            foreach (var datum in data)
            {
                var rowID = FindRowIndexFromUID(datum.UniqueID);
                if (rowID != -1)
                {
                    var row = mdataGrid_samples.Rows[rowID];
                    row.Selected = true;
                }
            }
        }

        /// <summary>
        /// Randomizes the selected samples for the sample queue.
        /// </summary>
        protected void RandomizeSelectedSamples()
        {
            var samplesToRandomize = new List<classSampleData>();
            var selectedIndices = new List<int>();
            //
            // Get all the data references that we want to randomize.
            //
            foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
            {
                var tempData = RowToSample(row);
                var data = m_sampleQueue.FindSample(tempData.UniqueID);
                if (data != null && data.RunningStatus == enumSampleRunningStatus.Queued)
                {
                    selectedIndices.Add(row.Index);
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
                    //
                    // The data grid has a peculiar bug that selects the first index.
                    // So we programmatically deselect it here.  If the user had that index
                    // selected, then it will inherently show up when we go back through and reselect.
                    //
                    mdataGrid_samples.Rows[0].Selected = false;

                    var newSamples = form.OutputSampleList;
                    m_sampleQueue.ReorderSamples(newSamples, enumColumnDataHandling.LeaveAlone);
                    foreach (var i in selectedIndices)
                    {
                        mdataGrid_samples.Rows[i].Selected = true;
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
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        protected virtual void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            m_sampleQueue.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Removes the single sample from the list view.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        protected virtual bool RemoveSamplesFromList(classSampleData sample)
        {
            //mlistview_samples.Items.RemoveByKey(sample.UniqueID.ToString());
            var index = FindRowIndexFromUID(sample.UniqueID);
            if (index >= 0)
                mdataGrid_samples.Rows.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes all the samples provided in the list of samples from the list view.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        protected virtual bool RemoveSamplesFromList(IEnumerable<classSampleData> samples)
        {
            var removed = true;
            foreach (var data in samples)
            {
                removed = (removed && RemoveSamplesFromList(data));
            }
            return removed;
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        protected virtual void RemoveSelectedSamples(enumColumnDataHandling resortColumns)
        {
            try
            {
                int scrollPosition;
                int currentColIndex;
                var selectedRowId = GetCurrentDataGridPosition(true, out currentColIndex, out scrollPosition);

                //
                // Get a list of sequence ID's to remove
                //
                var removes = new List<long>();
                foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
                {
                    var data = RowToSample(row);
                    removes.Add(data.UniqueID);
                }

                //
                // Remove all of them from the sample queue.
                // This should update the other views as well.
                //
                m_sampleQueue.RemoveSample(removes, resortColumns);

                RestoreSelectedDataGridCell(selectedRowId, currentColIndex, scrollPosition);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception in RemoveSelectedSamples: " + ex.Message, ex);
            }

        }

        /// <summary>
        /// Updates the provided row by determining if the sample data class is valid or not.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="data"></param>
        protected virtual void UpdateValidCell(DataGridViewRow row, classSampleData data)
        {
            //
            // Color duplicates or invalid cells with certain colors!
            //
            var validResult = m_sampleQueue.IsSampleDataValid(data);
            if (validResult == enumSampleValidResult.DuplicateRequestName &&
                !data.DmsData.DatasetName.Contains(m_sampleQueue.UnusedSampleName))
            {
                var styleDuplicate = new DataGridViewCellStyle(row.DefaultCellStyle)
                {
                    BackColor = Color.Crimson
                };
                row.Cells[CONST_COLUMN_REQUEST_NAME].Style = styleDuplicate;
                row.Cells[CONST_COLUMN_REQUEST_NAME].ToolTipText = "Duplicate Request Name Found!";
            }
            else
            {
                row.Cells[CONST_COLUMN_REQUEST_NAME].Style = row.DefaultCellStyle;
                row.Cells[CONST_COLUMN_REQUEST_NAME].ToolTipText = "";
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
        /// <param name="index">Index to update.</param>
        ///
        protected virtual void UpdateRow(int index)
        {
            var row = mdataGrid_samples.Rows[index];
            if (!row.Displayed)
            {
                mdataGrid_samples.Invalidate();
                return;
            }
            var data = (SampleToRowTranslator)row.DataBoundItem;


            var style = row.Cells[CONST_COLUMN_COLUMN_ID].Style;
            if (data.Sample.ColumnData == null)
            {
                throw new Exception("The column data cannot be null.");
            }
            if (data.Sample?.LCMethod != null && data.Sample.LCMethod.IsSpecialMethod)
            {
                data.SpecialColumnNumber = "S";
            }
            else
            {
                data.Sample.ColumnData = classCartConfiguration.Columns[data.Sample.ColumnData.ID];
                style.BackColor = data.Sample.ColumnData.Color;
            }

            // Make sure selected rows column colors don't change for running and waiting to run
            // but only for queued, or completed (including error) sample status.
            if (data.Sample.RunningStatus == enumSampleRunningStatus.Running ||
                data.Sample.RunningStatus == enumSampleRunningStatus.WaitingToRun)
            {
                style.SelectionBackColor = style.BackColor;
                style.SelectionForeColor = style.ForeColor;
            }

            row.Cells[CONST_COLUMN_CHECKED].Tag = data.CheckboxTag;

            //
            // If the name of the sample is the un-used, it means that the Sample Queue has backfilled the
            // samples to help the user normalize samples on columns.
            //
            var name = data.Sample.DmsData.DatasetName;
            if (name.Contains(m_sampleQueue.UnusedSampleName))
            {
                var rowStyle = row.DefaultCellStyle;
                rowStyle.BackColor = Color.LightGray;
                rowStyle.ForeColor = Color.DarkGray;
                if (!string.Equals(row.Cells[CONST_COLUMN_REQUEST_NAME].Value, data.Sample.DmsData.RequestName))
                    row.Cells[CONST_COLUMN_REQUEST_NAME].Value = data.Sample.DmsData.RequestName;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                var rowStyle = GetRowStyleFromSample(data.Sample, row.DefaultCellStyle);
                if (!string.Equals(row.Cells[CONST_COLUMN_REQUEST_NAME].Value, data.Sample.DmsData.DatasetName))
                    row.Cells[CONST_COLUMN_REQUEST_NAME].Value = data.Sample.DmsData.DatasetName;
            }
            row.Cells[CONST_COLUMN_STATUS].ToolTipText = data.StatusToolTipText;

            //
            // Setup the style for the column color
            //
            row.Cells[CONST_COLUMN_COLUMN_ID].Style = style;

            //
            // Make sure we color the valid row.
            //
            UpdateValidCell(row, data.Sample);
        }

        /// <summary>
        /// Updates the row at index with the data provided.
        /// </summary>
        /// <param name="data">Data to update the row with.</param>
        /// <param name="index">Index to update.</param>
        [Obsolete("Version used before databinding", true)]
        protected virtual void UpdateRow(classSampleData data, int index)
        {
            var row = mdataGrid_samples.Rows[index];
            row.Cells[CONST_COLUMN_SEQUENCE_ID].Value = data.SequenceID;


            var style = new DataGridViewCellStyle();
            if (data.ColumnData == null)
            {
                throw new Exception("The column data cannot be null.");
            }
            if (data.LCMethod != null && data.LCMethod.IsSpecialMethod)
            {
                row.Cells[CONST_COLUMN_COLUMN_ID].Value = "S";
            }
            else
            {
                row.Cells[CONST_COLUMN_COLUMN_ID].Value = data.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET;
                data.ColumnData = classCartConfiguration.Columns[data.ColumnData.ID];
                style.BackColor = data.ColumnData.Color;
            }

            // Make sure selected rows column colors don't change for running and waiting to run
            // but only for queued, or completed (including error) sample status.
            if (data.RunningStatus == enumSampleRunningStatus.Running ||
                data.RunningStatus == enumSampleRunningStatus.WaitingToRun)
            {
                style.SelectionBackColor = style.BackColor;
                style.SelectionForeColor = style.ForeColor;
            }

            GetCheckboxStatusAndSetCheckbox(data, row.Cells[CONST_COLUMN_CHECKED] as DataGridViewCheckBoxCell);

            //
            // If the name of the sample is the un-used, it means that the Sample Queue has backfilled the
            // samples to help the user normalize samples on columns.
            //
            var name = data.DmsData.DatasetName;
            if (name.Contains(m_sampleQueue.UnusedSampleName))
            {
                var rowStyle = new DataGridViewCellStyle(row.DefaultCellStyle)
                {
                    BackColor = Color.LightGray,
                    ForeColor = Color.DarkGray
                };
                row.DefaultCellStyle = rowStyle;
                row.Cells[CONST_COLUMN_REQUEST_NAME].Value = data.DmsData.RequestName;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                var rowStyle = GetRowStyleFromSample(data, row.DefaultCellStyle);
                row.DefaultCellStyle = rowStyle;
                row.Cells[CONST_COLUMN_REQUEST_NAME].Value = data.DmsData.DatasetName;
            }
            row.Cells[CONST_COLUMN_STATUS].Value = GetStatusMessageFromSampleStatus(data);
            row.Cells[CONST_COLUMN_STATUS].ToolTipText = GetToolTipMessageFromSampleStatus(data);

            //
            // Setup the style for the column color
            //
            row.Cells[CONST_COLUMN_COLUMN_ID].Style = style;
            row.Cells[CONST_COLUMN_UNIQUE_ID].Value = data.UniqueID;

            //
            // Make sure we color the valid row.
            //
            UpdateValidCell(row, data);

            //
            // Strings may be blank or null indicating they are not set.
            // If they are, then dont update the row.
            //
            if (string.IsNullOrEmpty(data.PAL.PALTray) == false)
                row.Cells[CONST_COLUMN_PAL_TRAY].Value = data.PAL.PALTray;

            row.Cells[CONST_COLUMN_PAL_VIAL].Value = data.PAL.Well;
            row.Cells[CONST_COLUMN_VOLUME].Value = data.Volume;

            //
            // Again, make sure we have data available.
            //
            if (data.LCMethod != null && string.IsNullOrEmpty(data.LCMethod.Name) == false)
                row.Cells[CONST_COLUMN_EXPERIMENT_METHOD].Value = data.LCMethod.Name;

            if (string.IsNullOrEmpty(data.InstrumentData.MethodName) == false)
                row.Cells[CONST_COLUMN_INSTRUMENT_METHOD].Value = data.InstrumentData.MethodName;

            if (string.IsNullOrEmpty(data.DmsData.DatasetType) == false)
                row.Cells[CONST_COLUMN_DATASET_TYPE].Value = data.DmsData.DatasetType;

            row.Cells[CONST_COLUMN_BATCH_ID].Value = data.DmsData.Batch;
        }

        /// <summary>
        /// Updates all the rows for all of the listening controls.
        /// </summary>
        private void RefreshRows()
        {
            foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
            {
                var tempData = RowToSample(row);
                var data = m_sampleQueue.FindSample(tempData.UniqueID);
                if (data == null)
                {
                    LogSampleIdNotFound("LogSampleIdNotFound", tempData.UniqueID);
                    return;
                }

                m_sampleQueue.UpdateSample(data);
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
            m_lastQueuedSamplePosition = data.CompleteQueueTotal;
            UpdateDataView();
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
            if (samples == null)
                return;

            var sampleList = samples.ToList();

            var dtStart = DateTime.UtcNow;
            var samplesUpdated = 0;
            var totalSamples = sampleList.Count();
            var lastCompletedRow = 0;
            var waitTimeSeconds = 5;

            mdataGrid_samples.SuspendLayout();

            foreach (var sample in sampleList)
            {
                var rowid = FindRowIndexFromUID(sample.UniqueID);
                if (rowid >= 0)
                {
                    ((SampleToRowTranslator)mdataGrid_samples.Rows[rowid].DataBoundItem).Sample = sample;
                    UpdateRow(rowid);
                    if (GetCheckboxStatusFromSampleStatus(sample) == enumCheckboxStatus.Disabled)
                    {
                        lastCompletedRow = Math.Max(lastCompletedRow, rowid);
                    }
                }

                if (QueryUserAbortSlowUpdates(ref dtStart, ref waitTimeSeconds, samplesUpdated, totalSamples))
                    break;

                samplesUpdated++;
            }

            if (m_autoscroll && lastCompletedRow > 3)
            {
                mdataGrid_samples.FirstDisplayedScrollingRowIndex = lastCompletedRow - 3;
            }
            mdataGrid_samples.ResumeLayout();
        }

        private bool QueryUserAbortSlowUpdates(ref DateTime dtStart, ref int waitTimeSeconds, int samplesUpdated,
            int totalSamples)
        {
            if (DateTime.UtcNow.Subtract(dtStart).TotalSeconds >= waitTimeSeconds)
            {
                var message = @"Updating the sample grid is taking a long time " +
                              @"(" + samplesUpdated + @" / " + totalSamples + " updated); " +
                              @"abort the task?";

                var response = MessageBox.Show(message, @"Abort?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (response == DialogResult.Yes)
                    return true;

                if (waitTimeSeconds < 40)
                    waitTimeSeconds *= 2;
                else
                    waitTimeSeconds += 30;

                dtStart = DateTime.UtcNow;
            }

            return false;
        }

        /// <summary>
        /// Finds the first index of the latest queable sample.
        /// </summary>
        private void UpdateDataView()
        {
            // Find the latest run, error, or running sample in the list.
            var maxComplete = -1;
            var maxWaitingToRun = -1;
            var i = 0;
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                i++;
                var sample = RowToSample(row);
                if (sample.RunningStatus == enumSampleRunningStatus.Complete ||
                    sample.RunningStatus == enumSampleRunningStatus.Stopped ||
                    sample.RunningStatus == enumSampleRunningStatus.Error ||
                    sample.RunningStatus == enumSampleRunningStatus.Running)
                {
                    maxComplete = i;
                    maxWaitingToRun = i;
                }
                if (sample.RunningStatus == enumSampleRunningStatus.WaitingToRun)
                {
                    maxWaitingToRun = i;
                }
            }
            m_firstQueuedSamplePosition = Math.Max(0, maxComplete);
            m_lastQueuedSamplePosition = Math.Max(0, maxWaitingToRun);

            m_sampleContainer.Refresh();
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
                BeginInvoke(new MethodInvoker(UpdateDataView));
            }
            else
            {
                UpdateRows(data.Samples);
                UpdateDataView();
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

            var scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);

            mdataGrid_samples.Rows.Clear();
            AddSamplesToList(data.Samples);

            UpdateDataView();

            var count = mdataGrid_samples.Rows.Count;
            if (count > 0)
            {
                if (scrollPosition == count)
                {
                    scrollPosition = Math.Max(0, count - 1);
                }
                try
                {
                    mdataGrid_samples.FirstDisplayedScrollingRowIndex = Math.Min(scrollPosition, count);
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Error setting the queue scroll position properly.", ex);
                    mdataGrid_samples.FirstDisplayedScrollingRowIndex = 0;
                }
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
                BeginInvoke(new MethodInvoker(UpdateDataView));
            }
            else
            {
                UpdateRows(data.Samples);
                UpdateDataView();
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
                BeginInvoke(new MethodInvoker(UpdateDataView));
            }
            else
            {
                UpdateRows(data.Samples);
                UpdateDataView();
            }
        }

        /// <summary>
        /// Handle when a sample is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void m_sampleQueue_SampleAdded(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DelegateUpdateRows(SamplesAddedFromQueue), data.Samples);
            }
            else
            {
                SamplesAddedFromQueue(data.Samples);
            }
        }

        protected virtual void SamplesAddedFromQueue(IEnumerable<classSampleData> samples)
        {
            //
            // The sample queue gives all of the samples
            //
            int scrollPosition;
            int currentColIndex;
            var selectedRowId = GetCurrentDataGridPosition(false, out currentColIndex, out scrollPosition);

            if (mdataGrid_samples.Rows.Count > 0)
            {
                try
                {
                    mdataGrid_samples.Rows.Clear();
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Ignoring exception in SamplesAddedFromQueue: " + ex.Message);
                }

            }

            AddSamplesToList(samples);

            UpdateDataView();

            RestoreSelectedDataGridCell(selectedRowId, currentColIndex, scrollPosition);
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void m_sampleQueue_SamplesReordered(object sender, classSampleQueueArgs data)
        {
            var scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);
            mdataGrid_samples.Rows.Clear();
            AddSamplesToList(data.Samples);
            UpdateDataView();
            var count = mdataGrid_samples.Rows.Count;
            if (count > 0)
            {
                mdataGrid_samples.FirstDisplayedScrollingRowIndex = Math.Min(scrollPosition, count);
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
                    "You are about to clear your queued samples.  Select Ok to clear, or Cancel to have no change.",
                    "Clear Queue Confirmation", MessageBoxButtons.OKCancel);

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

        #region Row to Sample Conversion

        public enum enumCheckboxStatus
        {
            Unchecked = 0,
            Checked,
            Disabled
        }

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <param name="sample">Sample to extract status message from.</param>
        /// <returns>String representing the status of the running state.</returns>
        protected enumCheckboxStatus GetCheckboxStatusFromSampleStatus(classSampleData sample)
        {
            var status = enumCheckboxStatus.Disabled;
            switch (sample.RunningStatus)
            {
                case enumSampleRunningStatus.Complete:
                    status = enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.Error:
                    status = enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Stopped:
                    status = enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Queued:
                    status = enumCheckboxStatus.Unchecked;
                    break;
                case enumSampleRunningStatus.Running:
                    status = enumCheckboxStatus.Disabled;
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    status = enumCheckboxStatus.Checked;
                    break;
                default:
                    //
                    // Should never get here
                    //
                    break;
            }

            return status;
        }

        protected enumCheckboxStatus GetCheckboxStatusAndSetCheckbox(classSampleData sample,
            DataGridViewCheckBoxCell checkbox)
        {
            var status = GetCheckboxStatusFromSampleStatus(sample);
            if (checkbox != null)
            {
                if (status == enumCheckboxStatus.Disabled)
                {
                    checkbox.Value = enumCheckboxStatus.Checked;
                    checkbox.Tag = "Disabled";
                }
                else
                {
                    checkbox.Value = status;
                }
            }
            return status;
        }

        protected enumCheckboxStatus GetCheckboxStatusFromCheckbox(DataGridViewCheckBoxCell checkbox)
        {
            return GetCheckboxStatusFromCheckbox(checkbox, checkbox.Value);
        }

        protected enumCheckboxStatus GetCheckboxStatusFromCheckbox(DataGridViewCheckBoxCell checkbox, object chkboxValue)
        {
            var status = enumCheckboxStatus.Unchecked;
            if (checkbox != null)
            {
                if (checkbox.Tag != null && checkbox.Tag.ToString() == "Disabled")
                {
                    status = enumCheckboxStatus.Disabled;
                }
                else
                {
                    if (chkboxValue is enumCheckboxStatus)
                    {
                        status = (enumCheckboxStatus)chkboxValue;
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <param name="sample">Sample to extract status message from.</param>
        /// <returns>String representing the status of the running state.</returns>
        protected string GetStatusMessageFromSampleStatus(classSampleData sample)
        {
            var statusMessage = "";
            switch (sample.RunningStatus)
            {
                case enumSampleRunningStatus.Complete:
                    statusMessage = "Complete";
                    break;
                case enumSampleRunningStatus.Error:
                    if (sample.DmsData.Block > 0)
                    {
                        statusMessage = "Block Error";
                    }
                    else
                    {
                        statusMessage = "Error";
                    }
                    break;
                case enumSampleRunningStatus.Stopped:
                    statusMessage = "Stopped";
                    break;
                case enumSampleRunningStatus.Queued:
                    statusMessage = "Queued";
                    break;
                case enumSampleRunningStatus.Running:
                    statusMessage = "Running";
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    statusMessage = "Waiting";
                    break;
                default:
                    //
                    // Should never get here
                    //
                    break;
            }

            return statusMessage;
        }

        /// <summary>
        /// Checks to see if the LC method is valid or not to add to the list of available methods for that given sample.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected virtual bool CanAddLCMethodToItem(classLCMethod method)
        {
            return true;
        }

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <param name="sample">Sample to extract status message from.</param>
        /// <returns>String representing the status of the running state.</returns>
        protected string GetToolTipMessageFromSampleStatus(classSampleData sample)
        {
            var statusMessage = "";
            switch (sample.RunningStatus)
            {
                case enumSampleRunningStatus.Complete:
                    statusMessage = "The sample ran successfully.";
                    break;
                case enumSampleRunningStatus.Error:
                    if (sample.DmsData.Block > 0)
                    {
                        statusMessage =
                            "There was an error and this sample was part of a block.  You should re-run the block of samples";
                    }
                    else
                    {
                        statusMessage = "An error occured while running this sample.";
                    }
                    break;
                case enumSampleRunningStatus.Stopped:
                    if (sample.DmsData.Block > 0)
                    {
                        statusMessage =
                            "The sample was stopped but was part of a block.  You should re-run the block of samples";
                    }
                    else
                    {
                        statusMessage = "The sample execution was stopped.";
                    }
                    break;
                case enumSampleRunningStatus.Queued:
                    statusMessage = "The sample is queued but not scheduled to run.";
                    break;
                case enumSampleRunningStatus.Running:
                    statusMessage = "The sample is running.";
                    break;
                case enumSampleRunningStatus.WaitingToRun:
                    statusMessage = "The sample is scheduled to run and waiting.";
                    break;
                default:
                    //
                    // Should never get here
                    //
                    break;
            }

            return statusMessage;
        }

        /// <summary>
        /// Creates a data grid cell type from a sample.
        /// </summary>
        /// <param name="sample">Sample to create data from</param>
        /// <returns>New DataGridViewCellRow if sample is not null.  Null if sample is null.</returns>
        protected virtual DataGridViewRow SampleToRow(classSampleData sample)
        {
            if (sample == null)
                return null;

            var row = new DataGridViewRow();
            var checkCell = new DataGridViewCheckBoxCell
            {
                TrueValue = enumCheckboxStatus.Checked,
                FalseValue = enumCheckboxStatus.Unchecked
            };

            GetCheckboxStatusAndSetCheckbox(sample, checkCell);

            var statusCell = new DataGridViewTextBoxCell
            {
                Value = GetStatusMessageFromSampleStatus(sample),
                ToolTipText = GetToolTipMessageFromSampleStatus(sample)
            };
            var sequenceCell = new DataGridViewTextBoxCell { Value = sample.SequenceID.ToString() };
            var uniqueIDCell = new DataGridViewTextBoxCell { Value = sample.UniqueID.ToString() };
            var columnIDCell = new DataGridViewTextBoxCell();
            var style = new DataGridViewCellStyle();

            var requestNameCell = new DataGridViewTextBoxCell { Value = sample.DmsData.DatasetName };
            var batchIDCell = new DataGridViewTextBoxCell { Value = sample.DmsData.Batch };
            var name = sample.DmsData.DatasetName;

            if (name.Contains(m_sampleQueue.UnusedSampleName))
            {
                var rowStyle = new DataGridViewCellStyle(row.DefaultCellStyle)
                {
                    BackColor = Color.LightGray,
                    ForeColor = Color.DarkGray
                };
                row.DefaultCellStyle = rowStyle;
                requestNameCell.Value = sample.DmsData.RequestName;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                var rowStyle = GetRowStyleFromSample(sample, row.DefaultCellStyle);
                row.DefaultCellStyle = rowStyle;
            }

            // Dataset Type
            var datasetTypeCell = new DataGridViewComboBoxCell();
            try
            {
                var dsTypes = classSQLiteTools.GetDatasetTypeList(false);
                datasetTypeCell.Items.Add(CONST_NOT_SELECTED);
                foreach (var dsType in dsTypes)
                    datasetTypeCell.Items.Add(dsType);

                datasetTypeCell.Value = sample.DmsData.DatasetType ?? CONST_NOT_SELECTED;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(1, "Could not load the dataset types.", ex);
            }


            // PAL - Check to make sure pal data was initialized correctly
            var palData = sample.PAL;
            if (palData == null)
            {
                palData = new classPalData
                {
                    PALTray = CONST_NOT_SELECTED,
                    Well = 0
                };
            }

            if (palData.PALTray == null)
                palData.PALTray = CONST_NOT_SELECTED;


            // -----------------------------------------------------------------------------
            // PAL Vial
            // -----------------------------------------------------------------------------
            var vial = new DataGridViewTextBoxCell
            {
                Value = palData.Well
            };

            // -----------------------------------------------------------------------------
            // PAL Trays
            // -----------------------------------------------------------------------------
            var palTray = new DataGridViewComboBoxCell();
            palTray.Items.Add(CONST_NOT_SELECTED);
            foreach (var data in m_autosamplerTrays)
            {
                palTray.Items.Add(data);
            }

            if (palData.PALTray.Replace(" ", "") != "" && palTray.Items.Contains(palData.PALTray) == false)
            {
                var palTrayStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.Red
                };
                palTray.Style = palTrayStyle;
                palTray.ToolTipText = "This tray was not available from the PAL";
                palTray.Items.Add(palData.PALTray);
                palTray.Value = palData.PALTray;
            }
            else if (palData.PALTray.Replace(" ", "") != "")
            {
                palTray.Value = palData.PALTray;
            }
            else
            {
                if (mcolumn_PalTray.Items.Count > 0)
                    palTray.Value = mcolumn_PalTray.Items[0];
            }

            // -----------------------------------------------------------------------------
            // Sample Volume
            // -----------------------------------------------------------------------------
            var volume = new DataGridViewTextBoxCell
            {
                Value = sample.Volume
            };

            // -----------------------------------------------------------------------------
            // LC Experiments
            // -----------------------------------------------------------------------------
            var lcMethods = new DataGridViewComboBoxCell();
            lcMethods.Items.Add(CONST_NOT_SELECTED);
            foreach (var info in classLCMethodManager.Manager.Methods.Values)
            {
                if (CanAddLCMethodToItem(info))
                {
                    lcMethods.Items.Add(info.Name);
                }
            }
            lcMethods.Value = CONST_NOT_SELECTED;

            if (sample.LCMethod != null && string.IsNullOrEmpty(sample.LCMethod.Name) == false)
                lcMethods.Value = sample.LCMethod.Name;
            else
                lcMethods.Value = CONST_NOT_SELECTED;


            if (sample.ColumnData == null)
            {
                if (sample.LCMethod != null)
                {
                    if (sample.LCMethod.IsSpecialMethod)
                    {
                        columnIDCell.Value = "S";
                    }
                }
                else
                {
                    throw new Exception("The sample column data cannot be null.");
                }
            }
            else
            {
                var columnID = sample.ColumnData.ID;
                if (columnID >= 0)
                {
                    columnIDCell.Value = sample.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET;
                    sample.ColumnData = classCartConfiguration.Columns[sample.ColumnData.ID];
                    style.BackColor = sample.ColumnData.Color;
                }
                else
                {
                    sample.ColumnData = new classColumnData
                    {
                        ID = columnID
                    };
                }
            }
            columnIDCell.Style = style;

            // -----------------------------------------------------------------------------
            // Instruments
            // -----------------------------------------------------------------------------
            var instrumentMethod = new DataGridViewComboBoxCell();
            instrumentMethod.Items.Add(CONST_NOT_SELECTED);
            foreach (var info in m_instrumentMethods)
                instrumentMethod.Items.Add(info);

            instrumentMethod.Value = CONST_NOT_SELECTED;
            if (string.IsNullOrEmpty(sample.InstrumentData.MethodName) == false)
                instrumentMethod.Value = sample.InstrumentData.MethodName;

            // -----------------------------------------------------------------------------
            // Run order information and blocking factors
            // -----------------------------------------------------------------------------
            var blockID = new DataGridViewTextBoxCell
            {
                Value = sample.DmsData.Block
            };

            var runOrder = new DataGridViewTextBoxCell
            {
                Value = sample.DmsData.RunOrder
            };

            row.Cells.Add(checkCell);
            row.Cells.Add(statusCell);
            row.Cells.Add(sequenceCell);
            row.Cells.Add(columnIDCell);
            row.Cells.Add(uniqueIDCell);
            row.Cells.Add(blockID);
            row.Cells.Add(runOrder);
            row.Cells.Add(requestNameCell);
            row.Cells.Add(palTray);
            row.Cells.Add(vial);
            row.Cells.Add(volume);
            row.Cells.Add(lcMethods);
            row.Cells.Add(instrumentMethod);
            row.Cells.Add(datasetTypeCell);
            row.Cells.Add(batchIDCell);

            UpdateValidCell(row, sample);

            return row;
        }

        /// <summary>
        /// Convert a data grid view row to a sample object.
        /// </summary>
        /// <param name="row">Row to convert.</param>
        /// <returns>Sample data created from row information.</returns>
        protected virtual classSampleData RowToSample(DataGridViewRow row)
        {
            //
            // Don't find the real sample so we don't change anything in the reference if this is a temporary operation.
            //
            var sample = new classSampleData(isDummySample: true)
            {
                SequenceID = Convert.ToInt32(row.Cells[CONST_COLUMN_SEQUENCE_ID].Value)
            };

            int id;
            var value = row.Cells[CONST_COLUMN_COLUMN_ID].Value.ToString();
            var parsed = int.TryParse(value, out id);
            if (parsed)
            {
                sample.ColumnData.ID = id - CONST_COLUMN_INDEX_OFFSET;
            }
            else
            {
                sample.ColumnData.ID = -1;
            }


            sample.UniqueID = Convert.ToInt32(row.Cells[CONST_COLUMN_UNIQUE_ID].Value);
            sample.PAL.PALTray = Convert.ToString(row.Cells[CONST_COLUMN_PAL_TRAY].Value);
            sample.PAL.Well = (int)row.Cells[CONST_COLUMN_PAL_VIAL].Value;
            sample.Volume = Convert.ToDouble(row.Cells[CONST_COLUMN_VOLUME].Value);

            var methodName = Convert.ToString(row.Cells[CONST_COLUMN_EXPERIMENT_METHOD].Value);
            if (classLCMethodManager.Manager.Methods.ContainsKey(methodName))
            {
                sample.LCMethod = classLCMethodManager.Manager.Methods[methodName].Clone() as classLCMethod;
            }
            else
                sample.LCMethod = new classLCMethod();

            var realSample = m_sampleQueue.FindSample(sample.UniqueID);
            if (realSample == null)
            {
                LogSampleIdNotFound("RowToSample", sample.UniqueID);
                return sample;
            }

            //
            // Copy the required DMS data.
            //
            sample.DmsData = new classDMSData
            {
                Batch = realSample.DmsData.Batch,
                Block = realSample.DmsData.Block,
                CartName = realSample.DmsData.CartName,
                CartConfigName = realSample.DmsData.CartConfigName,
                Comment = realSample.DmsData.Comment,
                DatasetName = realSample.DmsData.DatasetName,
                DatasetType = realSample.DmsData.DatasetType,
                Experiment = realSample.DmsData.Experiment,
                MRMFileID = realSample.DmsData.MRMFileID,
                ProposalID = realSample.DmsData.ProposalID,
                RequestID = realSample.DmsData.RequestID,
                RequestName = realSample.DmsData.RequestName,
                RunOrder = realSample.DmsData.RunOrder,
                UsageType = realSample.DmsData.UsageType,
                UserList = realSample.DmsData.UserList
            };

            sample.DmsData.DatasetName = Convert.ToString(row.Cells[CONST_COLUMN_REQUEST_NAME].Value);
            sample.DmsData.DatasetType = Convert.ToString(row.Cells[CONST_COLUMN_DATASET_TYPE].Value);


            sample.ColumnData.Name = realSample.ColumnData.Name;
            sample.RunningStatus = realSample.RunningStatus;
            sample.InstrumentData.MethodName = Convert.ToString(row.Cells[CONST_COLUMN_INSTRUMENT_METHOD].Value);

            return sample;
        }

        /// <summary>
        /// Converts the selected list view items into a list of sample data
        /// references used in the waiting queue using the unique ID of the samples.
        /// </summary>
        /// <returns></returns>
        protected virtual List<classSampleData> ConvertRowsToData(IEnumerable<DataGridViewRow> rows)
        {
            var data = new List<classSampleData>();
            // For every selected item find the
            foreach (var row in rows)
            {
                var tempData = RowToSample(row);
                if (tempData != null)
                {
                    var foundSample = m_sampleQueue.FindSample(tempData.UniqueID);
                    if (foundSample == null)
                    {
                        LogSampleIdNotFound("ConvertRowsToData", tempData.UniqueID);
                        continue;
                    }

                    data.Add(foundSample);
                }
            }
            return data;
        }

        /// <summary>
        /// Finds the index of the row in the sample data grid view using the samples unique ID.
        /// </summary>
        /// <param name="UID">Key to find sample row with.</param>
        /// <returns>Index of row >= 0 if found.  Or negative index if not found.</returns>
        protected int FindRowIndexFromUID(long UID)
        {
            var index = -1;
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                if (Convert.ToInt64(row.Cells[CONST_COLUMN_UNIQUE_ID].Value) == UID)
                {
                    index = row.Index;
                    return index;
                }
            }
            return index;
        }

        #endregion
    }

    public class classAscendingSampleIDComparer : IComparer<classSampleData>
    {
        #region IComparer<classSampleData> Members

        public int Compare(classSampleData x, classSampleData y)
        {
            return x.SequenceID.CompareTo(y.SequenceID);
        }

        #endregion
    }
}
