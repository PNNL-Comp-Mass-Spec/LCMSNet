﻿//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//                      - 03/16/2009 (BLL) - Added LC and PAL Methods
//                      - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
//                      - 9/26/2014  (CJW) - Modified to use MEF for DMS and sample validations
//                      - 9/30/2014  (CJW) - bug fixes
//*********************************************************************************************************

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using LcmsNetSQLiteTools;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Configuration;
using LcmsNet.Method;
using LcmsNet.SampleQueue;
using LcmsNet.Configuration;

namespace LcmsNet.SampleQueue.Forms
{
    /// <summary>
    /// Control that displays a sample list.
    /// </summary>
    public partial class controlSampleView : UserControl
    {
        protected formMoveToColumnSelector m_selector;

        /// <summary>
        /// Edits the selected samples in the sample view.
        /// </summary>
        protected virtual void EditDMSData()
        {
            List<classSampleData> samples = GetSelectedSamples();

            if (samples.Count < 1)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                    "You must select a sample to edit the DMS information.");
                return;
            }

            try
            {
                formSampleDMSValidatorDisplay dmsDisplay = new formSampleDMSValidatorDisplay(samples);
                // We don't care what the result is..
                if (dmsDisplay.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                // If samples are not valid...then what?
                else if (!dmsDisplay.AreSamplesValid)
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
            mobj_sampleQueue.UpdateSamples(samples);
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
        /// Index of experiment method column.
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

        private const int CONST_BAR_SIZE = 5;

        #endregion

        #region Members

        /// <summary>
        /// Form that provides user interface to retrieve samples from DMS.
        /// </summary>
        protected formDMSView mform_dmsView;

        /// <summary>
        /// Object that manages the list of all samples.
        /// </summary>
        protected classSampleQueue mobj_sampleQueue;

        /// <summary>
        /// Alternating back colors to enhance user visual feedback.
        /// </summary>
        private Color[] mobj_colors;

        /// <summary>
        /// Default sample name when a new sample is added.
        /// </summary>
        //protected string mstring_defaultSampleName;
        /// <summary>
        /// Starting index of listview items that can be edited.
        /// </summary>
        protected int mint_editableIndex;

        /// <summary>
        /// Value of the cell before edits.
        /// </summary>
        protected object mobj_cellValue;

        /// <summary>
        /// Names of the methods available on the PAL
        /// </summary>
        protected List<string> mlist_autoSamplerMethods;

        /// <summary>
        /// Names of the trays available on the PAL
        /// </summary>
        protected List<string> mlist_autosamplerTrays;

        /// <summary>
        /// Names of the instrument methods available on the MS.
        /// </summary>
        protected List<string> mlist_instrumentMethods;

        /// <summary>
        /// Flag that turns off the coloring when a PAL item (method, tray) was not downloadable from the PAL.
        /// </summary>
        protected bool mbool_ignoreMissingPALValues;

        /// <summary>
        /// Fill down form for updating lots of samples at once.
        /// </summary>
        protected formMethodFillDown mform_filldown;

        /// <summary>
        /// Tray and vial assignment form.
        /// </summary>
        protected formTrayVialAssignment mform_trayVial;

        /// <summary>
        /// Last position of the sample that is queued used to track how the user is queuing and dequeuing samples.
        /// </summary>
        private int mint_lastQueuedSamplePosition = 0;

        /// <summary>
        /// First position of the pulldown bar to track what samples can be queued or edited.
        /// </summary>
        private int mint_firstQueuedSamplePosition = 0;

        /// <summary>
        /// If autoscroll during sequence run is enabled
        /// </summary>
        protected bool m_autoscroll = true;

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlSampleView(formDMSView dmsView, classSampleQueue sampleQueue)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            try
            {
                InitializeComponent();
                Initialize(dmsView, sampleQueue);
            }
            catch (Exception Ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor.", Ex);
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
                Initialize(null, null);
            }
            catch (Exception Ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor", Ex);
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

            foreach (classColumnData column in classCartConfiguration.Columns)
            {
                column.NameChanged += column_NameChanged;
                column.ColorChanged += column_ColorChanged;
            }

            DMSView = dmsView;
            SampleQueue = sampleQueue;

            //
            // Background colors
            //
            mobj_colors = new Color[2];
            mobj_colors[0] = Color.White;
            mobj_colors[1] = Color.Gainsboro;
            mint_editableIndex = 0;

            //
            // Fill-down form for batch sample editing And Tray Vial assignmenet
            //
            mform_filldown = new formMethodFillDown(classLCMethodManager.Manager, new List<string>());
            mform_trayVial = new formTrayVialAssignment();

            mdataGrid_samples.ContextMenuStrip = mcontextMenu_options;

            //
            // Lists that hold information to be used by the sample queue combo boxes.
            //
            mlist_autoSamplerMethods = new List<string>();
            mlist_autosamplerTrays = new List<string>();
            mlist_instrumentMethods = new List<string>();

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
            List<string> datasetTypes;
            try
            {
                datasetTypes = classSQLiteTools.GetDatasetTypeList(false);
                foreach (string datasetType in datasetTypes)
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
            if (state == true)
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
        /// <param name="rowNum">The row number</param>
        /// <returns></returns>
        bool SelectionChangeValid(int rowNum, classSampleData sample, bool doNotValidate = false)
        {
            // Make sure we aren't at the end of the queue.
            int N = mdataGrid_samples.Rows.Count;
            if (rowNum <= N/* * mint_sampleItemSize*/)
            {
                int currentSample =
                    /*Convert.ToInt32(Math.Round(Convert.ToDouble(y) / Convert.ToDouble(mint_sampleItemSize))) +
                    mdataGrid_samples.FirstDisplayedScrollingRowIndex*/ rowNum;

                if (currentSample < 1 || currentSample > mdataGrid_samples.Rows.Count)
                    return false;
                if (!doNotValidate)
                {
                    this.Validate(); ///////////////////////////////////////////////////////////////////////// This is very, very expensive...... /////////////////////////////////////////////////////////////////////
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
                IDMSValidator validator;
                try
                {
                    validator = LcmsNetSDK.classDMSToolsManager.Instance.Validator;
                }
                catch (Exception)
                {
                    // no dms tools validator
                    validator = null;
                }
                bool isSampleValid;
                if (Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS)) && validator != null)
                {
                    isSampleValid = validator.IsSampleValid(sample);
                    if (!isSampleValid && false)
                    {
                        return false;
                    }
                }
                else
                {
                    classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                        "DMS validator not found and DMS validation enabled. Item not queued.");
                    return false;
                }

                // Validate other parts of the sample.
                List<classSampleValidationError> errors = new List<classSampleValidationError>();
                foreach (
                    Lazy<ISampleValidator, ISampleValidatorMetaData> reference in
                        LcmsNetDataClasses.Experiment.classSampleValidatorManager.Instance.Validators)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Validating sample with validator: " + reference.Metadata.Name);
#endif
                    ISampleValidator sampleValidator = reference.Value;
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
            int N = mdataGrid_samples.Rows.Count;

            try
            {
                currentTask = "Determine number of selected items";
                int offset = mdataGrid_samples.FirstDisplayedScrollingRowIndex;

                // Find the number of items that are selected.
                if (rowNum <= N)
                {
                    if (rowNum < 0 || rowNum > mdataGrid_samples.Rows.Count)
                        return;
                    if (!isRecursive && !(rowNum <= mint_firstQueuedSamplePosition || mint_lastQueuedSamplePosition > rowNum))
                    {
                        this.Validate();
                            ///////////////////////////////////////////////////////////////////////// This is very, very expensive...... /////////////////////////////////////////////////////////////////////
                    }
                }

                if (Math.Abs(mint_lastQueuedSamplePosition - rowNum) > 1)
                {
                    // process: if more than 1 selected, ask user for confirmation; if confirmed, recursively call this function?
                    bool isDequeue = mint_lastQueuedSamplePosition - rowNum > 0;
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
                int neededRowId = 0;
                if (rowNum <= mint_firstQueuedSamplePosition || mint_lastQueuedSamplePosition > rowNum)
                {
                    neededRowId = Convert.ToInt32(mdataGrid_samples.Rows[rowNum].Cells[CONST_COLUMN_UNIQUE_ID].Value);
                }
                else
                {
                    neededRowId = Convert.ToInt32(mdataGrid_samples.Rows[rowNum - 1].Cells[CONST_COLUMN_UNIQUE_ID].Value);
                }
                classSampleData sample = mobj_sampleQueue.FindSample(neededRowId);

                if (rowNum <= mint_firstQueuedSamplePosition)
                {
                    if (rowNum < mint_lastQueuedSamplePosition && rowNum >= mint_firstQueuedSamplePosition)
                    {
                        //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples]);
                        mobj_sampleQueue.DequeueSampleFromRunningQueue(sample);
                    }
                    if (!isRecursive)
                    {
                        mint_lastQueuedSamplePosition = Math.Max(0, rowNum);
                        m_sampleContainer.Refresh();
                    }
                    return;
                }
                if (mint_lastQueuedSamplePosition > rowNum)
                {
                    //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples]);
                    mobj_sampleQueue.DequeueSampleFromRunningQueue(sample);

                    if (!isRecursive)
                    {
                        mint_lastQueuedSamplePosition = rowNum;
                        m_sampleContainer.Refresh();
                    }
                    //m_sampleContainer.Refresh();
                    return;
                }

                currentTask = "Call to CanPullDownQueueBar";
                //bool canPullDown = SelectionChangeValid(y/* - mint_columnSize*/, tsample, isRecursive);
                //
                //if (!canPullDown)
                //{
                //    return;
                //}
                if (rowNum <= N)
                {
                    int currentSample = rowNum;

                    if (currentSample < 1 || currentSample > mdataGrid_samples.Rows.Count)
                        return;
                    //if (!isRecursive)
                    //{
                    //    this.Validate(); ///////////////////////////////////////////////////////////////////////// This is very, very expensive...... /////////////////////////////////////////////////////////////////////
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
                    IDMSValidator validator;
                    try
                    {
                        validator = LcmsNetSDK.classDMSToolsManager.Instance.Validator;
                    }
                    catch (Exception)
                    {
                        // no dms tools validator
                        validator = null;
                    }
                    bool isSampleValid;
                    if (Convert.ToBoolean(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_VALIDATESAMPLESFORDMS)) && validator != null)
                    {
                        isSampleValid = validator.IsSampleValid(sample);
                        if (!isSampleValid && false)
                        {
                            return;
                        }
                    }
                    else
                    {
                        classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                            "DMS validator not found and DMS validation enabled. Item not queued.");
                        return;
                    }

                    // Validate other parts of the sample.
                    List<classSampleValidationError> errors = new List<classSampleValidationError>();
                    foreach (
                        Lazy<ISampleValidator, ISampleValidatorMetaData> reference in
                            LcmsNetDataClasses.Experiment.classSampleValidatorManager.Instance.Validators)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("Validating sample with validator: " +
                                                           reference.Metadata.Name);
#endif
                        ISampleValidator sampleValidator = reference.Value;
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
                    // We were at the end of the rope.
                    return;
                }

                currentTask =
                    "Handle queueing the samples by first tracking the current sample index, then seeing if it changed from last to current";

                // Handle queueing the samples by first tracking the current sample index, then
                // seeing if it changed from last to current.
                if (mint_lastQueuedSamplePosition < rowNum)
                {
                    //classSampleData sample = RowToSample(mdataGrid_samples.Rows[totalSamples - 1]);
                    mobj_sampleQueue.MoveSamplesToRunningQueue(sample);
                }

                if (!isRecursive)
                {
                    mint_lastQueuedSamplePosition = rowNum;
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
                if (row == null)
                {
                    return;
                }
                DataGridViewCheckBoxCell checkbox = row.Cells[CONST_COLUMN_CHECKED] as DataGridViewCheckBoxCell;
                if (checkbox == null)
                {
                    return;
                }
                //enumCheckboxStatus gridEditState =
                //    GetCheckboxStatusFromCheckbox(checkbox, checkbox.EditedFormattedValue); // use checkbox.EditedFormattedValue
                enumCheckboxStatus gridSavedState = GetCheckboxStatusFromCheckbox(checkbox, checkbox.Value);
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
                if (row == null)
                {
                    return;
                }
                DataGridViewComboBoxCell comboBox = row.Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (comboBox == null)
                {
                    return;
                }
                try
                {
                    comboBox.Selected = true;
                    mdataGrid_samples.BeginEdit(true);
                    ((DataGridViewComboBoxEditingControl) mdataGrid_samples.EditingControl).DroppedDown = true;
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

            foreach (object o in mcolumn_LCMethod.Items)
            {
                string name = Convert.ToString(o);
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
            foreach (object o in mcolumn_LCMethod.Items)
            {
                string name = o.ToString();
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
            foreach (object o in mcolumn_LCMethod.Items)
            {
                string name = o.ToString();
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
            List<classSampleData> samples = mobj_sampleQueue.GetWaitingQueue();
            List<classSampleData> updateSamples = new List<classSampleData>();
            foreach (classSampleData sample in samples)
            {
                if (sample.LCMethod != null && sample.LCMethod.Name == method.Name)
                {
                    int newColID = method.Column;
                    if (newColID >= 0)
                    {
                        sample.ColumnData = classCartConfiguration.Columns[newColID];
                    }
                    sample.LCMethod = method;
                    updateSamples.Add(sample);
                }
            }

            mobj_sampleQueue.UpdateSamples(updateSamples);

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
            bool found = false;
            foreach (object o in mcolumn_LCMethod.Items)
            {
                string name = Convert.ToString(o);
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
                int indexOf = mcolumn_LCMethod.Items.IndexOf(method.Name);
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
            mobj_sampleQueue.Undo();
        }

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        protected virtual void Redo()
        {
            mobj_sampleQueue.Redo();
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
                mobj_sampleQueue.QueueSamples(samples, ColumnHandling);
            }
            else
            {
                mobj_sampleQueue.InsertIntoUnusedSamples(samples, ColumnHandling);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DMS View form.
        /// </summary>
        public virtual formDMSView DMSView
        {
            get { return mform_dmsView; }
            set { mform_dmsView = value; }
        }

        /// <summary>
        /// Gets or sets the sample queue that handles all queue management at a low level.
        /// </summary>
        public virtual classSampleQueue SampleQueue
        {
            get { return mobj_sampleQueue; }
            set
            {
                mobj_sampleQueue = value;
                if (value != null)
                {
                    mobj_sampleQueue.SamplesAdded += mobj_sampleQueue_SampleAdded;
                    mobj_sampleQueue.SamplesCancelled += mobj_sampleQueue_SampleCancelled;
                    mobj_sampleQueue.SamplesFinished += mobj_sampleQueue_SampleFinished;
                    mobj_sampleQueue.SamplesRemoved += mobj_sampleQueue_SampleRemoved;
                    mobj_sampleQueue.SamplesStarted += mobj_sampleQueue_SampleStarted;
                    mobj_sampleQueue.SamplesReordered += mobj_sampleQueue_SamplesReordered;
                    mobj_sampleQueue.SamplesUpdated += mobj_sampleQueue_SamplesUpdated;
                    mobj_sampleQueue.SamplesWaitingToRun += mobj_sampleQueue_SamplesWaitingToRun;
                    mobj_sampleQueue.SamplesStopped += mobj_sampleQueue_SamplesStopped;
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
            get { return mlist_autoSamplerMethods; }
            set
            {
                mlist_autoSamplerMethods = value;
                if (mlist_autoSamplerMethods == null)
                    mlist_autoSamplerMethods = new List<string>();
                ShowAutoSamplerMethods();
            }
        }

        /// <summary>
        /// Gets or sets a list of pal tray names.
        /// </summary>
        public virtual List<string> AutoSamplerTrays
        {
            get { return mlist_autosamplerTrays; }
            set
            {
                //classApplicationLogger.LogMessage(0, "SAMPLE VIEW PROCESSING AUTOSAMPLER TRAYS!");
                mlist_autosamplerTrays = value;
                if (mlist_autosamplerTrays == null)
                    mlist_autosamplerTrays = new List<string>();
                ShowAutoSamplerTrays();
            }
        }

        /// <summary>
        /// Gets or sets a list of instrument method names stored on the MS instrument.
        /// </summary>
        public virtual List<string> InstrumentMethods
        {
            get { return mlist_instrumentMethods; }
            set
            {
                mlist_instrumentMethods = value;
                if (mlist_instrumentMethods == null)
                    mlist_instrumentMethods = new List<string>();
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
            mdataGrid_samples.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewRow row = mdataGrid_samples.Rows[e.RowIndex];

            //
            // Get the new value
            //
            object cellData = row.Cells[e.ColumnIndex].Value;

            //
            // Make sure the value is not null, and that the user has selected an item.
            //
            if (cellData == null)
            {
                //
                // Revert back to the old data.
                //
                row.Cells[e.ColumnIndex].Value = mobj_cellValue;
                classApplicationLogger.LogMessage(5, "Sample View Grid Data was null.");
                return;
            }
            else if (cellData.ToString() == CONST_NOT_SELECTED)
            {
                //
                // Revert back to the old data.
                //
                row.Cells[e.ColumnIndex].Value = mobj_cellValue;
                return;
            }

            //
            // Find the sample in the queue.
            //
            long uniqueID = Convert.ToInt64(row.Cells[CONST_COLUMN_UNIQUE_ID].Value);
            classSampleData data = mobj_sampleQueue.FindSample(uniqueID);
            bool success = false;
            double volume = 0.0;
            int vial = 0;
            string name = "";

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
                    name = Convert.ToString(cellData);
                    if (classLCMethodManager.Manager.Methods.ContainsKey(name))
                    {
                        classLCMethod tempMethod = classLCMethodManager.Manager.Methods[name];
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
                    success = int.TryParse(cellData.ToString(), out vial);
                    if (success)
                    {
                        data.PAL.Well = Math.Max(CONST_MIN_WELLPLATE,
                            Math.Min(vial, CONST_MAX_WELLPLATE));
                    }
                    break;
                case CONST_COLUMN_VOLUME:
                    success = double.TryParse(cellData.ToString(), out volume);
                    if (success == true)
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

            mobj_sampleQueue.UpdateSample(data);
        }

        /// <summary>
        /// Copies the current value of the cell that is being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mdataGrid_samples_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == CONST_COLUMN_PAL_VIAL)
            {
            }

            long uniqueID = Convert.ToInt64(mdataGrid_samples.Rows[e.RowIndex].Cells[CONST_COLUMN_UNIQUE_ID].Value);
            classSampleData data = mobj_sampleQueue.FindSample(uniqueID);
            if (data.RunningStatus != enumSampleRunningStatus.Queued)
            {
                e.Cancel = true;
                return;
            }

            mdataGrid_samples.SelectionMode = DataGridViewSelectionMode.CellSelect;

            switch (e.ColumnIndex)
            {
                default:
                    mobj_cellValue = mdataGrid_samples.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    break;
            }
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

            foreach (string tray in mlist_autosamplerTrays)
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

            foreach (string tray in mlist_instrumentMethods)
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

            foreach (classLCMethod method in classLCMethodManager.Manager.Methods.Values)
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
            List<classSampleData> samples = GetSelectedSamples();
            if (samples.Count < 1)
                return;

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                delegate(classSampleData sample) { return sample.RunningStatus != enumSampleRunningStatus.Queued; });

            if (samples.Count < 1)
                return;

            foreach (classSampleData sample in samples)
            {
                classSampleData.AddDateCartColumnToDatasetName(sample);
            }
            mobj_sampleQueue.UpdateSamples(samples);
        }

        public void ResetDatasetName()
        {
            List<classSampleData> samples = GetSelectedSamples();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                delegate(classSampleData sample) { return sample.RunningStatus != enumSampleRunningStatus.Queued; });

            if (samples.Count < 1)
                return;

            foreach (classSampleData sample in samples)
            {
                classSampleData.ResetDatasetNameToRequestName(sample);
            }

            mobj_sampleQueue.UpdateSamples(samples);
        }

        protected virtual void EditTrayAndVial()
        {
            List<classSampleData> samples = GetSelectedSamples();
            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                delegate(classSampleData sample) { return sample.RunningStatus != enumSampleRunningStatus.Queued; });

            if (samples.Count < 1)
            {
                return;
            }

            if (mlist_autosamplerTrays.Count < 6)
            {
                classApplicationLogger.LogError(0, "Not enough PAL Trays are available.");
                return;
            }

            mform_trayVial.Icon = ParentForm.Icon;
            mform_trayVial.LoadSampleList(mlist_autosamplerTrays, samples);
            if (mform_trayVial.ShowDialog() == DialogResult.OK)
            {
                samples = mform_trayVial.SampleList;
                mobj_sampleQueue.UpdateSamples(samples);
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
            List<classSampleData> samples = GetSelectedSamples();

            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            samples.RemoveAll(
                delegate(classSampleData sample) { return sample.RunningStatus != enumSampleRunningStatus.Queued; });

            if (samples.Count < 1)
            {
                return;
            }

            //
            // Create a new fill down form.
            //
            mform_filldown.Icon = ParentForm.Icon;
            mform_filldown.InitForm(samples);


            mform_filldown.StartPosition = FormStartPosition.CenterScreen;
            if (mform_filldown.ShowDialog() == DialogResult.OK)
            {
                //
                // Then update the sample queue...
                //
                List<classSampleData> newSamples = mform_filldown.GetModifiedSampleList();
                mobj_sampleQueue.UpdateSamples(newSamples);
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
                int column = m_selector.SelectedColumn;
                List<classSampleData> selectedSamples = GetSelectedSamples();


                if (selectedSamples.Count < 1)
                    return;

                //
                // Make sure the samples can actually run, e.g. dont put a sample on column 2 already back onto column 2.
                // Don't put a column that has been run, at the end of the queue again.
                //
                List<classSampleData> samples = new List<classSampleData>();
                foreach (classSampleData sample in selectedSamples)
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
                List<long> ids = new List<long>();
                foreach (classSampleData sample in samples)
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

                //mobj_sampleQueue.RemoveSample(ids, backFill);

                ////
                //// Then re-queue the samples.
                ////
                //try
                //{
                //    if (m_selector.InsertIntoUnused)
                //    {
                //        mobj_sampleQueue.InsertIntoUnusedSamples(samples, handling);
                //    }
                //    else
                //    {
                //        mobj_sampleQueue.UpdateSamples(
                //        mobj_sampleQueue.QueueSamples(samples, handling);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    classApplicationLogger.LogError(0, "Could not queue the samples when moving between columns.", ex);
                //}
                if (samples.Count > 0)
                {
                    mobj_sampleQueue.UpdateSamples(samples);
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
            mobj_sampleQueue.UpdateAllSamples();
        }

        /// <summary>
        /// Handles when a column name has been changed to update the sample queue accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        void column_NameChanged(object sender, string name, string oldName)
        {
            mobj_sampleQueue.UpdateWaitingSamples();
        }

        /// <summary>
        /// Returns whether the sample queue has an unused samples.
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasUnusedSamples()
        {
            return mobj_sampleQueue.HasUnusedSamples();
        }

        /// <summary>
        /// Displays the DMS View Dialog Window.
        /// </summary>
        protected virtual void ShowDMSView()
        {
            if (mform_dmsView != null)
            {
                DialogResult result = mform_dmsView.ShowDialog();

                //
                // If the user clicks ok , then add the samples from the
                // form into the sample queue.  Don't add them directly to the
                // form so that the event model will update both this view
                // and any other views that we may have.  For the sequence
                // we dont care how we add them to the form.
                //
                if (result == DialogResult.OK)
                {
                    List<classSampleData> samples = mform_dmsView.GetNewSamplesDMSView();
                    mform_dmsView.ClearForm();

                    bool insertToUnused = false;
                    if (HasUnusedSamples())
                    {
                        //
                        // Ask the user what to do with these samples?
                        //
                        formInsertOntoUnusedDialog dialog = new formInsertOntoUnusedDialog();
                        DialogResult insertResult = dialog.ShowDialog();

                        insertToUnused = (insertResult == DialogResult.Yes);
                    }


                    AddSamplesToManager(samples, insertToUnused);

                    //
                    // Don't add directly to the user interface in case the
                    // sample manager class has something to say about one of the samples
                    //
                    classApplicationLogger.LogMessage(0, samples.Count.ToString() + " samples added to the queue");
                }
            }
        }

        /// <summary>
        /// Copies the sample data from one object required to make a new sample entry in the sample queue.
        /// </summary>
        /// <param name="sampleToCopy">Sample to copy</param>
        /// <returns>New object reference of a sample with only required data copied.</returns>
        protected virtual classSampleData CopyRequiredSampleData(classSampleData sampleToCopy)
        {
            classSampleData newSample = new classSampleData(false)
            {
                DmsData =
                {
                    RequestName = string.Format("{0}_{1:0000}",
                                                mobj_sampleQueue.DefaultSampleName,
                                                mobj_sampleQueue.RunningSampleIndex++),
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
                int id = sampleToCopy.ColumnData.ID;
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

            // Then we make sure that hey, even if the column data is null, that we want to
            // make sure the column is pertitent to the method, since what column we run on depends
            // on what method we are trying to run.
            if (sampleToCopy.LCMethod != null)
            {
                newSample.LCMethod = sampleToCopy.LCMethod.Clone() as classLCMethod;
                if (newSample.LCMethod != null && newSample.LCMethod.Column >= 0)
                {
                    int id = newSample.LCMethod.Column;
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
            //
            // Get the list of selected samples
            //
            List<classSampleData> samples = new List<classSampleData>();
            foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
            {
                classSampleData data = RowToSample(row);
                samples.Add(data);
            }

            samples.Sort(new classAscendingSampleIDComparer());

            return samples;
        }

        /// <summary>
        /// Preview the selected samples on the data grid.
        /// </summary>
        public void PreviewSelectedThroughput()
        {
            List<classSampleData> samples = GetSelectedSamples();
            samples.RemoveAll(
                delegate(classSampleData data)
                {
                    return data.DmsData.DatasetName.Contains(mobj_sampleQueue.UnusedSampleName);
                });
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
                LcmsNet.Method.Forms.formThroughputPreview preview = new LcmsNet.Method.Forms.formThroughputPreview();
                preview.Show();

                foreach (classSampleData data in samples)
                {
                    data.LCMethod.SetStartTime(LcmsNetSDK.TimeKeeper.Instance.Now);
                    //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)));
                }

                preview.ShowAlignmentForSamples(samples);
                preview.Visible = false;
                preview.ShowDialog();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
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
                DataGridViewRow row = mdataGrid_samples.Rows[mdataGrid_samples.Rows.Count - 1];
                classSampleData data = RowToSample(row);
                if (data != null)
                {
                    classSampleData actualData = mobj_sampleQueue.FindSample(data.UniqueID);
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
                                                    mobj_sampleQueue.DefaultSampleName,
                                                    mobj_sampleQueue.RunningSampleIndex++),
                        CartName = classCartConfiguration.CartName,
                        CartConfigName = classCartConfiguration.CartConfigName
                    }
                };
            }

            if (newData != null)
            {
                AddSamplesToManager(new List<classSampleData>() {newData}, insertIntoUnused);
            }
        }

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        protected virtual bool AddSamplesToList(classSampleData sample)
        {
            bool added = true;
            if (sample == null)
            {
                added = false;
            }
            else
            {
                bool found = false;
                for (int i = 0; i < mdataGrid_samples.Rows.Count; i++)
                {
                    classSampleData sampleNext = RowToSample(mdataGrid_samples.Rows[i]);
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
            }
            return added;
        }

        /// <summary>
        /// Adds samples to the list but optimizes layout and updates for rendering controls.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        protected virtual bool AddSamplesToList(IEnumerable<classSampleData> samples)
        {
            bool added = true;
            foreach (classSampleData data in samples)
            {
                AddSamplesToList(data);
            }
            return added;
        }

        /// <summary>
        /// Clear all of the samples (deleting them from the queue as well).
        /// </summary>
        protected virtual void ClearAllSamples()
        {
            //
            // Get a list of sequence ID's to remove
            //
            List<long> removes = new List<long>();
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                removes.Add((long)row.Cells[CONST_COLUMN_UNIQUE_ID].Value);
            }

            //
            // Remove all of them from the sample queue.
            // This should update the other views as well.
            //
            mobj_sampleQueue.RemoveSample(removes, enumColumnDataHandling.LeaveAlone);
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
            DataGridViewRow[] items = new DataGridViewRow[mdataGrid_samples.SelectedRows.Count];
            mdataGrid_samples.SelectedRows.CopyTo(items, 0);
            List<classSampleData> data = ConvertRowsToData(items);


            //
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            //
            data.RemoveAll(
                delegate(classSampleData sample) { return sample.RunningStatus != enumSampleRunningStatus.Queued; });

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
            mobj_sampleQueue.MoveQueuedSamples(data, mint_editableIndex, offset, moveType);

            //
            // By default the first cell wants to be selected.
            //
            mdataGrid_samples.Rows[0].Selected = false;

            foreach (classSampleData datum in data)
            {
                int rowID = FindRowIndexFromUID(datum.UniqueID);
                if (rowID != -1)
                {
                    DataGridViewRow row = mdataGrid_samples.Rows[rowID];
                    if (row != null)
                        row.Selected = true;
                }
            }
        }

        /// <summary>
        /// Randomizes the selected samples for the sample queue.
        /// </summary>
        protected void RandomizeSelectedSamples()
        {
            List<classSampleData> samplesToRandomize = new List<classSampleData>();
            List<int> selectedIndices = new List<int>();
            //
            // Get all the data references that we want to randomize.
            //
            foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
            {
                classSampleData tempData = RowToSample(row);
                classSampleData data = mobj_sampleQueue.FindSample(tempData.UniqueID);
                if (data != null && data.RunningStatus == enumSampleRunningStatus.Queued)
                {
                    selectedIndices.Add(row.Index);
                    classSampleData sample = data.Clone() as classSampleData;
                    if (sample.LCMethod != null && sample.LCMethod.Name != null)
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
                formSampleRandomizer form = null;
                try
                {
                    form = new formSampleRandomizer(samplesToRandomize);
                    form.Icon = ParentForm.Icon;
                    form.StartPosition = FormStartPosition.CenterParent;
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

                    List<classSampleData> newSamples = form.OutputSampleList;
                    mobj_sampleQueue.ReorderSamples(newSamples, enumColumnDataHandling.LeaveAlone);
                    foreach (int i in selectedIndices)
                    {
                        mdataGrid_samples.Rows[i].Selected = true;
                    }
                }
            }
            else if (samplesToRandomize.Count == 1)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "Select more than one sample for randomization.");
                return;
            }
            else
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_USER,
                    "No samples selected for randomization.");
                return;
            }
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        protected virtual void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            mobj_sampleQueue.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Removes the single sample from the list view.
        /// </summary>
        /// <param name="samples"></param>
        /// <returns></returns>
        protected virtual bool RemoveSamplesFromList(classSampleData sample)
        {
            //mlistview_samples.Items.RemoveByKey(sample.UniqueID.ToString());
            int index = FindRowIndexFromUID(sample.UniqueID);
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
            bool removed = true;
            foreach (classSampleData data in samples)
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
            //
            // Get a list of sequence ID's to remove
            //
            List<long> removes = new List<long>();
            foreach (DataGridViewRow row in mdataGrid_samples.SelectedRows)
            {
                classSampleData data = RowToSample(row);
                removes.Add(data.UniqueID);
            }

            //
            // Remove all of them from the sample queue.
            // This should update the other views as well.
            //
            mobj_sampleQueue.RemoveSample(removes, resortColumns);
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
            enumSampleValidResult validResult = mobj_sampleQueue.IsSampleDataValid(data);
            if (validResult == enumSampleValidResult.DuplicateRequestName &&
                !data.DmsData.DatasetName.Contains(mobj_sampleQueue.UnusedSampleName))
            {
                DataGridViewCellStyle styleDuplicate = new DataGridViewCellStyle(row.DefaultCellStyle);
                styleDuplicate.BackColor = Color.Crimson;
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
            DataGridViewCellStyle rowStyle = defaultStyle;
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

            enumColumnStatus status = data.ColumnData.Status;
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
            DataGridViewRow row = mdataGrid_samples.Rows[index];
            if (!row.Displayed)
            {
                mdataGrid_samples.Invalidate();
                return;
            }
            SampleToRowTranslator data = (SampleToRowTranslator)row.DataBoundItem;


            DataGridViewCellStyle style = row.Cells[CONST_COLUMN_COLUMN_ID].Style;
            if (data.Sample.ColumnData == null)
            {
                throw new Exception("The column data cannot be null.");
            }
            else if (data.Sample != null && data.Sample.LCMethod != null && data.Sample.LCMethod.IsSpecialMethod)
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
            string name = data.Sample.DmsData.DatasetName;
            if (name.Contains(mobj_sampleQueue.UnusedSampleName))
            {
                DataGridViewCellStyle rowStyle = row.DefaultCellStyle;
                rowStyle.BackColor = Color.LightGray;
                rowStyle.ForeColor = Color.DarkGray;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                DataGridViewCellStyle rowStyle = GetRowStyleFromSample(data.Sample, row.DefaultCellStyle);
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
            DataGridViewRow row = mdataGrid_samples.Rows[index];
            row.Cells[CONST_COLUMN_SEQUENCE_ID].Value = data.SequenceID;


            DataGridViewCellStyle style = new DataGridViewCellStyle();
            if (data.ColumnData == null)
            {
                throw new Exception("The column data cannot be null.");
            }
            else if (data.LCMethod != null && data.LCMethod.IsSpecialMethod)
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
            string name = data.DmsData.DatasetName;
            if (name.Contains(mobj_sampleQueue.UnusedSampleName))
            {
                DataGridViewCellStyle rowStyle = new DataGridViewCellStyle(row.DefaultCellStyle);
                rowStyle.BackColor = Color.LightGray;
                rowStyle.ForeColor = Color.DarkGray;
                row.DefaultCellStyle = rowStyle;
                row.Cells[CONST_COLUMN_REQUEST_NAME].Value = data.DmsData.RequestName;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                DataGridViewCellStyle rowStyle = GetRowStyleFromSample(data, row.DefaultCellStyle);
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
                classSampleData tempData = RowToSample(row);
                classSampleData data = mobj_sampleQueue.FindSample(tempData.UniqueID);
                mobj_sampleQueue.UpdateSample(data);
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
        protected virtual void mobj_sampleQueue_SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesUpdated),
                    new object[] {sender, data});
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
            mint_lastQueuedSamplePosition = data.CompleteQueueTotal;
            UpdateDataView();
            SamplesUpdated(sender, data);
            m_sampleContainer.Refresh();
        }

        /// <summary>
        /// Handled when the samples are stopped from the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void mobj_sampleQueue_SamplesStopped(object sender, classSampleQueueArgs data)
        {
            if (InvokeRequired == true)
            {
                BeginInvoke(new classSampleQueue.DelegateSamplesModifiedHandler(SamplesStopped),
                    new object[] {sender, data});
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
        void mobj_sampleQueue_SamplesWaitingToRun(object sender, classSampleQueueArgs data)
        {
            if (data == null || data.Samples == null)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), new object[] {data.Samples});
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
            var dtStart = DateTime.UtcNow;
            var samplesUpdated = 0;
            var totalSamples = samples.Count();
            var lastCompletedRow = 0;
            var waitTimeSeconds = 5;

            mdataGrid_samples.SuspendLayout();

            foreach (var sample in samples)
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
            int maxComplete = -1;
            int maxWaitingToRun = -1;
            int i = 0;
            foreach (DataGridViewRow row in mdataGrid_samples.Rows)
            {
                i++;
                classSampleData sample = RowToSample(row);
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
            mint_firstQueuedSamplePosition = Math.Max(0, maxComplete);
            mint_lastQueuedSamplePosition = Math.Max(0, maxWaitingToRun);

            m_sampleContainer.Refresh();
        }

        /// <summary>
        /// Handles when a sample is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void mobj_sampleQueue_SampleStarted(object sender, classSampleQueueArgs data)
        {
            if (data == null || data.Samples == null)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), new object[] {data.Samples});
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
        protected virtual void mobj_sampleQueue_SampleRemoved(object sender, classSampleQueueArgs data)
        {
            // Start fresh and add the samples from the queue to the list.
            // But track the position of the scroll bar to be nice to the user.

            int scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);

            mdataGrid_samples.Rows.Clear();
            AddSamplesToList(data.Samples);

            UpdateDataView();

            int count = mdataGrid_samples.Rows.Count;
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
        protected virtual void mobj_sampleQueue_SampleFinished(object sender, classSampleQueueArgs data)
        {
            if (data == null || data.Samples == null)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), new object[] {data.Samples});
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
        protected virtual void mobj_sampleQueue_SampleCancelled(object sender, classSampleQueueArgs data)
        {
            if (data == null || data.Samples == null)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdateRows(UpdateRows), new object[] {data.Samples});
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
        protected virtual void mobj_sampleQueue_SampleAdded(object sender, classSampleQueueArgs data)
        {
            if (data == null || data.Samples == null)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new DelegateUpdateRows(SamplesAddedFromQueue), new object[] {data.Samples});
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
            int scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);

            if (mdataGrid_samples.Rows.Count > 0)
            {
                // Rows.Clear() cannot be used since it results in error 
                //  "Rows collection cannot be programmatically cleared
                //   when the DataGridView control is data-bound 
                //   to anything else than an IBindingList that supports
                //   change notification and allows deletion"
                // mdataGrid_samples.Rows.Clear();

                // Instead, use GetAllSamples then .RemoveAll
                var existingSamples = GetAllSamples();
                existingSamples.RemoveAll(delegate { return true; });

            }

            AddSamplesToList(samples);

            UpdateDataView();

            int count = mdataGrid_samples.Rows.Count;
            if (count > 0)
            {
                mdataGrid_samples.FirstDisplayedScrollingRowIndex = Math.Min(scrollPosition, count);
            }
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void mobj_sampleQueue_SamplesReordered(object sender, classSampleQueueArgs data)
        {
            int scrollPosition = Math.Max(0, mdataGrid_samples.FirstDisplayedScrollingRowIndex);
            mdataGrid_samples.Rows.Clear();
            AddSamplesToList(data.Samples);
            UpdateDataView();
            int count = mdataGrid_samples.Rows.Count;
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

        /// <summary>
        /// Handle when the user clicks to load data from DMS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_dmsImport_Click(object sender, EventArgs e)
        {
            ShowDMSView();
        }

        /// <summary>
        /// Randomizes the selected samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomizeSelectedSamples();
        }

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
            this.Validate();
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

        private void mbutton_redo_Click(object sender, EventArgs e)
        {
            Redo();
        }

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

        private void mbutton_moveSelectedSamplesToColumn_Click(object sender, EventArgs e)
        {
            MoveSamplesToColumn(enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Add the data cart name and column information to the sample datasetname.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDateCartNameColumnIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDateCartnameColumnIDToDatasetName();
        }

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

        private void deleteUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveUnusedSamples(enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Handles undo-ing any operations on the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_undo_Click(object sender, EventArgs e)
        {
            Undo();
        }

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
            Disabled,
        }

        /// <summary>
        /// Returns the status message pertaining to a given samples running status.
        /// </summary>
        /// <param name="sample">Sample to extract status message from.</param>
        /// <returns>String representing the status of the running state.</returns>
        protected enumCheckboxStatus GetCheckboxStatusFromSampleStatus(classSampleData sample)
        {
            enumCheckboxStatus status = enumCheckboxStatus.Disabled;
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
            enumCheckboxStatus status = GetCheckboxStatusFromSampleStatus(sample);
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
            enumCheckboxStatus status = enumCheckboxStatus.Unchecked;
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
            string statusMessage = "";
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
            string statusMessage = "";
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

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
            checkCell.TrueValue = enumCheckboxStatus.Checked;
            checkCell.FalseValue = enumCheckboxStatus.Unchecked;
            GetCheckboxStatusAndSetCheckbox(sample, checkCell);
            DataGridViewTextBoxCell statusCell = new DataGridViewTextBoxCell();
            statusCell.Value = GetStatusMessageFromSampleStatus(sample);
            statusCell.ToolTipText = GetToolTipMessageFromSampleStatus(sample);
            DataGridViewTextBoxCell sequenceCell = new DataGridViewTextBoxCell();
            sequenceCell.Value = sample.SequenceID.ToString();
            DataGridViewTextBoxCell uniqueIDCell = new DataGridViewTextBoxCell();
            uniqueIDCell.Value = sample.UniqueID.ToString();
            DataGridViewTextBoxCell columnIDCell = new DataGridViewTextBoxCell();
            DataGridViewCellStyle style = new DataGridViewCellStyle();

            DataGridViewTextBoxCell requestNameCell = new DataGridViewTextBoxCell();
            requestNameCell.Value = sample.DmsData.DatasetName;
            DataGridViewTextBoxCell batchIDCell = new DataGridViewTextBoxCell();
            batchIDCell.Value = sample.DmsData.Batch;
            string name = sample.DmsData.DatasetName;

            if (name.Contains(mobj_sampleQueue.UnusedSampleName))
            {
                DataGridViewCellStyle rowStyle = new DataGridViewCellStyle(row.DefaultCellStyle);
                rowStyle.BackColor = Color.LightGray;
                rowStyle.ForeColor = Color.DarkGray;
                row.DefaultCellStyle = rowStyle;
                requestNameCell.Value = sample.DmsData.RequestName;
            }
            else
            {
                //
                // We need to color the sample based on its status.
                //
                DataGridViewCellStyle rowStyle = GetRowStyleFromSample(sample, row.DefaultCellStyle);
                row.DefaultCellStyle = rowStyle;
            }

            // Dataset Type
            DataGridViewComboBoxCell datasetTypeCell = new DataGridViewComboBoxCell();
            try
            {
                List<string> dsTypes = classSQLiteTools.GetDatasetTypeList(false);
                datasetTypeCell.Items.Add(CONST_NOT_SELECTED);
                foreach (string dsType in dsTypes)
                    datasetTypeCell.Items.Add(dsType);

                if (sample.DmsData.DatasetType == null)
                    datasetTypeCell.Value = CONST_NOT_SELECTED;
                else
                    datasetTypeCell.Value = sample.DmsData.DatasetType;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(1, "Could not load the dataset types.", ex);
            }


            // PAL - Check to make sure pal data was initialized correctly
            classPalData palData = sample.PAL;
            if (palData == null)
            {
                palData = new classPalData();
                palData.PALTray = CONST_NOT_SELECTED;
                palData.Well = 0;
            }

            if (palData.PALTray == null)
                palData.PALTray = CONST_NOT_SELECTED;


            // -----------------------------------------------------------------------------
            // PAL Vial
            // -----------------------------------------------------------------------------
            DataGridViewTextBoxCell vial = new DataGridViewTextBoxCell();
            vial.Value = palData.Well;

            // -----------------------------------------------------------------------------
            // PAL Trays
            // -----------------------------------------------------------------------------
            DataGridViewComboBoxCell palTray = new DataGridViewComboBoxCell();
            palTray.Items.Add(CONST_NOT_SELECTED);
            foreach (string data in mlist_autosamplerTrays)
            {
                palTray.Items.Add(data);
            }

            if (palData.PALTray.Replace(" ", "") != "" && palTray.Items.Contains(palData.PALTray) == false)
            {
                DataGridViewCellStyle palTrayStyle = new DataGridViewCellStyle();
                palTrayStyle.BackColor = Color.Red;
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
            DataGridViewTextBoxCell volume = new DataGridViewTextBoxCell();
            volume.Value = sample.Volume;

            // -----------------------------------------------------------------------------
            // LC Experiments
            // -----------------------------------------------------------------------------
            DataGridViewComboBoxCell lcMethods = new DataGridViewComboBoxCell();
            lcMethods.Items.Add(CONST_NOT_SELECTED);
            foreach (classLCMethod info in classLCMethodManager.Manager.Methods.Values)
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
                int columnID = sample.ColumnData.ID;
                if (columnID >= 0)
                {
                    columnIDCell.Value = sample.ColumnData.ID + CONST_COLUMN_INDEX_OFFSET;
                    sample.ColumnData = classCartConfiguration.Columns[sample.ColumnData.ID];
                    style.BackColor = sample.ColumnData.Color;
                }
                else
                {
                    sample.ColumnData = new classColumnData();
                    sample.ColumnData.ID = columnID;
                }
            }
            columnIDCell.Style = style;

            // -----------------------------------------------------------------------------
            // Instruments
            // -----------------------------------------------------------------------------
            DataGridViewComboBoxCell instrumentMethod = new DataGridViewComboBoxCell();
            instrumentMethod.Items.Add(CONST_NOT_SELECTED);
            foreach (string info in mlist_instrumentMethods)
                instrumentMethod.Items.Add(info);

            instrumentMethod.Value = CONST_NOT_SELECTED;
            if (string.IsNullOrEmpty(sample.InstrumentData.MethodName) == false)
                instrumentMethod.Value = sample.InstrumentData.MethodName;

            // -----------------------------------------------------------------------------
            // Run order information and blocking factors
            // -----------------------------------------------------------------------------
            DataGridViewTextBoxCell blockID = new DataGridViewTextBoxCell();
            blockID.Value = sample.DmsData.Block;

            DataGridViewTextBoxCell runOrder = new DataGridViewTextBoxCell();
            runOrder.Value = sample.DmsData.RunOrder;


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
            // Don't find the real sample so we dont change anything in the reference if this is a temporary operation.
            //
            classSampleData sample = new classSampleData(true); // Mark it as a dummy sample
            sample.SequenceID = Convert.ToInt32(row.Cells[CONST_COLUMN_SEQUENCE_ID].Value);
            int id = 0;
            string value = row.Cells[CONST_COLUMN_COLUMN_ID].Value.ToString();
            bool parsed = int.TryParse(value, out id);
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
            sample.PAL.Well = (int) row.Cells[CONST_COLUMN_PAL_VIAL].Value;
            sample.Volume = Convert.ToDouble(row.Cells[CONST_COLUMN_VOLUME].Value);

            string methodName = Convert.ToString(row.Cells[CONST_COLUMN_EXPERIMENT_METHOD].Value);
            if (classLCMethodManager.Manager.Methods.ContainsKey(methodName))
            {
                sample.LCMethod = classLCMethodManager.Manager.Methods[methodName].Clone() as classLCMethod;
            }
            else
                sample.LCMethod = new classLCMethod();

            classSampleData realSample = mobj_sampleQueue.FindSample(sample.UniqueID);

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
            List<classSampleData> data = new List<classSampleData>();
            // For every selected item find the
            foreach (DataGridViewRow row in rows)
            {
                classSampleData tempData = this.RowToSample(row);
                if (tempData != null)
                {
                    tempData = mobj_sampleQueue.FindSample(tempData.UniqueID);
                    data.Add(tempData);
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
            int index = -1;
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