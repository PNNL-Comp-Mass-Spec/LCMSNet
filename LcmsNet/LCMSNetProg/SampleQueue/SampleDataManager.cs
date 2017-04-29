using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using LcmsNet.Method;
using LcmsNet.WPFControls.ViewModels;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;
using LcmsNetDmsTools;
using ReactiveUI;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Class to manage sample data and limit the number of copied data and operations that occur
    /// </summary>
    public class SampleDataManager : ReactiveObject
    {
        public ReactiveList<SampleViewModel> Samples { get; private set; }

        private readonly classDMSSampleValidator mValidator;

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
            HasValidColumns = enabled;

            SampleQueue.UpdateAllSamples();
        }

        private bool hasValidColumns = true;
        private bool hasData = true;

        public bool HasValidColumns
        {
            get { return hasValidColumns; }
            private set { this.RaiseAndSetIfChanged(ref hasValidColumns, value); }
        }

        public bool HasData
        {
            get { return hasData; }
            private set { this.RaiseAndSetIfChanged(ref hasData, value); }
        }

        #endregion

        #region Members

        /// <summary>
        /// Object that manages the list of all samples.
        /// </summary>
        private classSampleQueue sampleQueue;

        /// <summary>
        /// Names of the methods available on the PAL
        /// </summary>
        private List<string> autoSamplerMethods;

        /// <summary>
        /// Names of the trays available on the PAL
        /// </summary>
        private List<string> autosamplerTrays;

        /// <summary>
        /// Names of the instrument methods available on the MS.
        /// </summary>
        private List<string> instrumentMethods;

        /// <summary>
        /// Flag that turns off the coloring when a PAL item (method, tray) was not downloadable from the PAL.
        /// </summary>
        private bool ignoreMissingPALValues;

        private bool dmsAvailable = false;
        private bool cycleColumns = false;

        public bool DMSAvailable
        {
            get { return dmsAvailable; }
            set { this.RaiseAndSetIfChanged(ref dmsAvailable, value); }
        }

        /// <summary>
        /// Gets or sets whether when adding samples,
        /// the column data should cycle through, (e.g. 1,2,3,4,1,2)
        /// </summary>
        public bool CycleColumns
        {
            get { return cycleColumns; }
            set { this.RaiseAndSetIfChanged(ref cycleColumns, value); }
        }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public SampleDataManager(classSampleQueue sampleQueue)
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

            ColumnHandling = enumColumnDataHandling.Resort;

            try
            {
                mValidator = new classDMSSampleValidator(SampleQueueComboBoxOptions.CartConfigOptions);
                Initialize(sampleQueue);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the Fsample queue controls.  Constructor 1: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Default constructor for the sample view control that takes no arguments
        /// but also no functionality unless the sample queue and dms form is supplied.
        /// Calling this constructor is only for the windows form designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public SampleDataManager()
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

            try
            {
                mValidator = new classDMSSampleValidator();
                Initialize(null);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 2: " + ex.Message, ex);
            }
        }

        void classLCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName.Equals(classLCMSSettings.PARAM_DMSTOOL) && e.SettingValue != string.Empty)
            {
                DMSAvailable = true;
            }
            if (e.SettingName.Equals(classLCMSSettings.PARAM_CARTCONFIGNAME))
            {
                foreach (var sample in Samples)
                {
                    var data = sample.Sample.DmsData;
                    data.CartConfigName = e.SettingValue;
                }
            }
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        /// <param name="sampleQueue"></param>
        private void Initialize(classSampleQueue sampleQueue)
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
                });

            foreach (var column in classCartConfiguration.Columns)
            {
                column.NameChanged += column_NameChanged;
                column.ColorChanged += column_ColorChanged;
            }

            SampleQueue = sampleQueue;

            //
            // Lists that hold information to be used by the sample queue combo boxes.
            //
            autoSamplerMethods = new List<string>();
            autosamplerTrays = new List<string>();
            instrumentMethods = new List<string>();

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
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.RequestName))).Subscribe(x => CheckForDuplicates(x.Sender.Sample));
            Samples.ItemChanged.Where(x => x.PropertyName.Equals(nameof(x.Sender.Sample.IsDuplicateRequestName))).Subscribe(x => HandleDuplicateRequestNameChanged(x.Sender.Sample));
            // TODO: Check for side effects
            // TODO: The idea of this is that it would detect the minor changes to the queue, where a value was changed using the databinding. There needs to be a lockout for actions not taken via the databinding, since those already handle this process...
            Samples.ItemChanged.Where(x =>
            {
                var prop = x.PropertyName;
                var obj = x.Sender;
                return prop.Equals(nameof(obj.RequestName)) ||
                       prop.Equals(nameof(obj.Sample.DmsData.Batch)) ||
                       prop.Equals(nameof(obj.Sample.DmsData.Block)) ||
                       prop.Equals(nameof(obj.Sample.DmsData.RunOrder)) ||
                       prop.Equals(nameof(obj.Sample.DmsData.CartConfigName)) ||
                       prop.Equals(nameof(obj.Sample.DmsData.DatasetType)) ||
                       prop.Equals(nameof(obj.ColumnNumber)) ||
                       prop.Equals(nameof(obj.InstrumentMethod)) ||
                       prop.Equals(nameof(obj.Sample.LCMethod)) ||
                       prop.Equals(nameof(obj.Sample.PAL.PALTray)) ||
                       prop.Equals(nameof(obj.Sample.SequenceID));
            }).Throttle(TimeSpan.FromSeconds(.25))
            .Subscribe(x => this.SampleQueue.UpdateSample(x.Sender.Sample));
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
                        var foundError = false;
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
                                var sampleValidErrors = mValidator.IsSampleValidDetailed(sample.Sample);

                                if (sampleValidErrors != DMSSampleValidatorErrors.NoError)
                                {
                                    sample.Sample.SampleErrors = mValidator.CreateErrorListFromErrors(sampleValidErrors);
                                    foundError = true;
                                }
                                else
                                {
                                    sample.Sample.SampleErrors = null;
                                }
                            }
                            else
                            {
                                // DMS Sample validation is disabled
                                return;
                            }

                            if (string.IsNullOrEmpty(sample.Sample.SampleErrors))
                            {
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
                                    foundError = true;
                                }
                            }

                            if (!foundError)
                            {
                                // Add to run queue
                                SampleQueue.MoveSamplesToRunningQueue(sample.Sample);
                            }

                            if (sample.Equals(changedSample))
                            {
                                if (foundError)
                                {
                                    sample.IsChecked = false;
                                }
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
                            SampleQueue.DequeueSampleFromRunningQueue(sample.Sample);

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
                    MessageBox.Show(@"Error in HandleSampleValidationAndQueueing, task " + currentTask + @": " + ex.Message, @"Error",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                if (string.IsNullOrWhiteSpace(sampleToCheck.PAL.PALTray))
                {
                    // PAL tray not defined
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(sampleToCheck.LCMethod.Name))
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

            foreach (var o in SampleQueueComboBoxOptions.LcMethodOptions)
            {
                if (o.Equals(method))
                {
                    SampleQueueComboBoxOptions.LcMethodOptions.Remove(method);
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
            foreach (var o in SampleQueueComboBoxOptions.LcMethodOptions)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    oFound = o;
                }
            }
            if (oFound != null)
            {
                SampleQueueComboBoxOptions.LcMethodOptions.Remove(oFound);
            }
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool ContainsMethod(string methodName)
        {
            foreach (var o in SampleQueueComboBoxOptions.LcMethodOptions)
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
            var samples = SampleQueue.GetWaitingQueue();
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

            SampleQueue.UpdateSamples(updateSamples);

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
            foreach (var o in SampleQueueComboBoxOptions.LcMethodOptions)
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
                SampleQueueComboBoxOptions.LcMethodOptions.Add(method);
                // If we just added a sample, we want to make sure the samples have a method selected.
                if (SampleQueueComboBoxOptions.LcMethodOptions.Count == 1)
                {
                }
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                var indexOf = SampleQueueComboBoxOptions.LcMethodOptions.IndexOf(method);
                if (indexOf >= 0)
                    SampleQueueComboBoxOptions.LcMethodOptions[indexOf] = method;
            }

            return true;
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        public void Undo()
        {
            using (Samples.SuppressChangeNotifications())
            {
                SampleQueue.Undo();
            }
        }

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        public void Redo()
        {
            using (Samples.SuppressChangeNotifications())
            {
                SampleQueue.Redo();
            }
        }

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        public void AddSamplesToManager(List<classSampleData> samples, bool insertIntoUnused)
        {
            using (Samples.SuppressChangeNotifications())
            {
                if (insertIntoUnused == false)
                {
                    SampleQueue.QueueSamples(samples, ColumnHandling);
                }
                else
                {
                    SampleQueue.InsertIntoUnusedSamples(samples, ColumnHandling);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sample queue that handles all queue management at a low level.
        /// </summary>
        public virtual classSampleQueue SampleQueue
        {
            get { return sampleQueue; }
            set
            {
                sampleQueue = value;
                if (value != null)
                {
                    sampleQueue.SamplesAdded += SampleQueue_SampleAdded;
                    sampleQueue.SamplesCancelled += SampleQueue_SampleCancelled;
                    sampleQueue.SamplesFinished += SampleQueue_SampleFinished;
                    sampleQueue.SamplesRemoved += SampleQueue_SampleRemoved;
                    sampleQueue.SamplesStarted += SampleQueue_SampleStarted;
                    sampleQueue.SamplesReordered += SampleQueue_SamplesReordered;
                    sampleQueue.SamplesUpdated += SampleQueue_SamplesUpdated;
                    sampleQueue.SamplesWaitingToRun += SampleQueue_SamplesWaitingToRun;
                    sampleQueue.SamplesStopped += SampleQueue_SamplesStopped;
                    HasData = true;
                }
                else
                {
                    HasData = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of pal method names.
        /// </summary>
        public virtual List<string> AutoSamplerMethods
        {
            get { return autoSamplerMethods; }
            set
            {
                autoSamplerMethods = value ?? new List<string>();
                ShowAutoSamplerMethods();
            }
        }

        /// <summary>
        /// Gets or sets a list of pal tray names.
        /// </summary>
        public virtual List<string> AutoSamplerTrays
        {
            get { return autosamplerTrays; }
            set
            {
                //classApplicationLogger.LogMessage(0, "SAMPLE VIEW PROCESSING AUTOSAMPLER TRAYS!");
                autosamplerTrays = value ?? new List<string>();
                ShowAutoSamplerTrays();
            }
        }

        /// <summary>
        /// Gets or sets a list of instrument method names stored on the MS instrument.
        /// </summary>
        public virtual List<string> InstrumentMethods
        {
            get { return instrumentMethods; }
            set
            {
                instrumentMethods = value ?? new List<string>();
                ShowInstrumentMethods();
            }
        }

        #endregion

        #region DataGridView Events and Methods

        private void LogSampleIdNotFound(string callingMethod, long sampleId)
        {
            var knownSampleIDs = new List<long>();
            foreach (var sample in SampleQueue.GetWaitingQueue())
            {
                knownSampleIDs.Add(sample.UniqueID);
            }

            foreach (var sample in SampleQueue.GetRunningQueue())
            {
                knownSampleIDs.Add(sample.UniqueID);
            }

            var sampleIdCount = knownSampleIDs.Count;

            string sampleQueueDescription;
            if (sampleIdCount == 0)
                sampleQueueDescription = "SampleQueue is empty";
            else if (sampleIdCount == 1)
                sampleQueueDescription = "SampleQueue has one waiting or running item, ID " + knownSampleIDs.First();
            else if (sampleIdCount < 10)
                sampleQueueDescription = "Waiting and running items in SampleQueue are " +
                                         string.Join(", ", knownSampleIDs);
            else
                sampleQueueDescription = "Waiting and running items in SampleQueue are " +
                                         string.Join(", ", knownSampleIDs.Take(9) + " ... (" + sampleIdCount + " total items)");

            var msg = callingMethod + " could not find sample ID " + sampleId + " in SampleQueue; " + sampleQueueDescription;
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
            SampleQueueComboBoxOptions.PalTrayOptions.Clear();

            foreach (var tray in autosamplerTrays)
            {
                SampleQueueComboBoxOptions.PalTrayOptions.Add(tray);
            }
        }

        /// <summary>
        /// Updates the Instrument Method Column Combo Box.
        /// </summary>
        private void ShowInstrumentMethods()
        {
            SampleQueueComboBoxOptions.InstrumentMethodOptions.Clear();

            foreach (var tray in instrumentMethods)
            {
                SampleQueueComboBoxOptions.InstrumentMethodOptions.Add(tray);
            }
        }

        /// <summary>
        /// Updates the LC-Method to the LC Separation Method Box.
        /// </summary>
        private void ShowLCSeparationMethods()
        {
            SampleQueueComboBoxOptions.LcMethodOptions.Clear();

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
            SampleQueueComboBoxOptions.LcMethodOptions.Add(method);
        }

        #endregion

        #region Utility Methods

        public void AddDateCartnameColumnIDToDatasetName(List<classSampleData> samples)
        {
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
                SampleQueue.UpdateSamples(samples);
            }
        }

        public void ResetDatasetName(List<classSampleData> samples)
        {
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

                SampleQueue.UpdateSamples(samples);
            }
        }

        /// <summary>
        /// Handles when a column color has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        private void column_ColorChanged(object sender, System.Drawing.Color previousColor, System.Drawing.Color newColor)
        {
            using (Samples.SuppressChangeNotifications())
            {
                SampleQueue.UpdateAllSamples();

                // The following is necessary due to how samples are stored and read from a database
                // May be removed if code is updated to re-set LCMethod and ColumnData after data is loaded from a database or imported.
                foreach (var s in Samples)
                {
                    s.Sample.ColumnData = classCartConfiguration.Columns[s.Sample.ColumnData.ID];
                }
            }
        }

        /// <summary>
        /// Handles when a column name has been changed to update the sample queue accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
        private void column_NameChanged(object sender, string name, string oldName)
        {
            using (Samples.SuppressChangeNotifications())
            {
                SampleQueue.UpdateWaitingSamples();

                // The following is necessary due to how samples are stored and read from a database
                // May be removed if code is updated to re-set LCMethod and ColumnData after data is loaded from a database or imported.
                foreach (var s in Samples)
                {
                    s.Sample.ColumnData = classCartConfiguration.Columns[s.Sample.ColumnData.ID];
                }
            }
        }

        /// <summary>
        /// Returns whether the sample queue has an unused samples.
        /// </summary>
        /// <returns></returns>
        public bool HasUnusedSamples()
        {
            return SampleQueue.HasUnusedSamples();
        }

        /// <summary>
        /// Copies the sample data from one object required to make a new Blank sample entry in the sample queue.
        /// </summary>
        /// <param name="sampleToCopy">Sample to copy</param>
        /// <returns>New object reference of a sample with only required data copied.</returns>
        public classSampleData CopyRequiredSampleData(classSampleData sampleToCopy)
        {
            var newSample = new classSampleData(false)
            {
                DmsData =
                {
                    RequestName = string.Format("{0}_{1:0000}",
                        SampleQueue.DefaultSampleName,
                        SampleQueue.RunningSampleIndex++),
                    CartName = sampleToCopy.DmsData.CartName,
                    CartConfigName = sampleToCopy.DmsData.CartConfigName,
                    DatasetType = sampleToCopy.DmsData.DatasetType
                },
                PAL =
                {
                    Method = sampleToCopy.PAL.Method,
                    Well = classPalData.CONST_DEFAULT_VIAL_NUMBER,
                    PALTray = "",
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

        #endregion

        #region Queue User Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        public classSampleData AddNewSample(bool insertIntoUnused)
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
                    var actualData = SampleQueue.FindSample(data.UniqueID);
                    if (actualData == null)
                    {
                        LogSampleIdNotFound("AddNewSample", data.UniqueID);
                        return null;
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
                            SampleQueue.DefaultSampleName,
                            SampleQueue.RunningSampleIndex++),
                        CartName = classCartConfiguration.CartName,
                        CartConfigName = classCartConfiguration.CartConfigName
                    }
                };
            }

            if (newData != null)
            {
                using (Samples.SuppressChangeNotifications())
                {
                    AddSamplesToManager(new List<classSampleData> { newData }, insertIntoUnused);
                }
            }
            return newData;
        }

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        public bool AddSamplesToList(classSampleData sample)
        {
            if (sample == null)
            {
                return false;
            }
            if (!Samples.Any(x => x.Sample.Equals(sample)))
            {
                using (Samples.SuppressChangeNotifications())
                {
                    if (string.IsNullOrWhiteSpace(sample.DmsData.CartConfigName))
                    {
                        sample.DmsData.CartConfigName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME);
                    }
                    Samples.Add(new SampleViewModel(sample));
                    Samples.Sort((x, y) => x.Sample.SequenceID.CompareTo(y.Sample.SequenceID));
                    UpdateRow(sample);
                }
            }
            return true;
        }

        /// <summary>
        /// Adds samples to the list but optimizes layout and updates for rendering controls.
        /// </summary>
        /// <param name="samples">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        public bool AddSamplesToList(IEnumerable<classSampleData> samples)
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
        public void ClearAllSamples()
        {
            using (Samples.SuppressChangeNotifications())
            {
                //
                // Remove all of them from the sample queue.
                // This should update the other views as well.
                //
                SampleQueue.RemoveSample(Samples.Select(x => x.Sample.UniqueID).ToList(), enumColumnDataHandling.LeaveAlone);
            }
        }

        /// <summary>
        /// Moves all the selected samples an offset of their original sequence id.
        /// </summary>
        public void MoveSamples(List<classSampleData> data, int offset, enumMoveSampleType moveType)
        {
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
                SampleQueue.MoveQueuedSamples(data, 0, offset, moveType);
            }
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        public void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            using (Samples.SuppressChangeNotifications())
            {
                SampleQueue.RemoveUnusedSamples(resortColumns);
            }
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        public void RemoveSamples(List<SampleViewModel> samples, enumColumnDataHandling resortColumns)
        {
            try
            {
                //
                // Get a list of sequence ID's to remove
                //
                var removes = new List<long>();
                var samplesToRemove = samples.OrderBy(x => x.Sample.SequenceID).ToList();
                foreach (var sample in samplesToRemove)
                {
                    removes.Add(sample.Sample.UniqueID);
                }

                using (Samples.SuppressChangeNotifications())
                {
                    //
                    // Remove all of them from the sample queue.
                    // This should update the other views as well.
                    //
                    SampleQueue.RemoveSample(removes, resortColumns);
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
        private void CheckForDuplicates(classSampleData data)
        {
            //
            // Color duplicates or invalid cells with certain colors!
            //
            var validResult = SampleQueue.IsSampleDataValid(data);
            if (validResult == enumSampleValidResult.DuplicateRequestName &&
                !data.DmsData.DatasetName.Contains(SampleQueue.UnusedSampleName))
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
                    CheckForDuplicates(sample.Sample);
                }
            }
            else
            {
                // This sample is no longer a duplicate, so we need to hit everything that was flagged as a duplicate name
                foreach (var sample in Samples.Where(x => x.Sample.IsDuplicateRequestName))
                {
                    if (sample.Sample.Equals(data))
                    {
                        continue;
                    }
                    CheckForDuplicates(sample.Sample);
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
        public void UpdateRow(classSampleData sample)
        {
            CheckForDuplicates(sample);
        }

        #endregion

        #region Queue Manager Event Handlers

        /// <summary>
        /// Handles when a sample is updated somewhere and the user interface needs to be updated
        /// for that cell.
        /// </summary>
        /// <param name="sender">Queue manager that is updated.</param>
        /// <param name="data">Data arguments that contain the updated sample information.</param>
        private void SampleQueue_SamplesUpdated(object sender, classSampleQueueArgs data)
        {
            SamplesUpdated(sender, data);
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
        private void SampleQueue_SamplesStopped(object sender, classSampleQueueArgs data)
        {
            SamplesStopped(sender, data);
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
        void SampleQueue_SamplesWaitingToRun(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Updates the rows for the given samples.
        /// </summary>
        /// <param name="samples">Samples to update view of.</param>
        public void UpdateRows(IEnumerable<classSampleData> samples)
        {
            foreach (var sample in samples)
            {
                UpdateRow(sample);
            }
        }

        /// <summary>
        /// Handles when a sample is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SampleStarted(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        ///// <summary>
        /// Handle when a sample is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SampleRemoved(object sender, classSampleQueueArgs data)
        {
            // Start fresh and add the samples from the queue to the list.
            // But track the position of the scroll bar to be nice to the user.
            // TODO: var backup = Samples.ToList();

            using (Samples.SuppressChangeNotifications())
            {
                Samples.Clear();
                AddSamplesToList(data.Samples);
            }

            // TODO: if (Samples.Count > 0)
            // TODO: {
            // TODO:     SampleViewModel sampleToScroll = Samples.First();
            // TODO:     for (var i = 0; i < backup.Count; i++)
            // TODO:     {
            // TODO:         if (i >= Samples.Count || !backup[i].Equals(Samples[i]))
            // TODO:         {
            // TODO:             sampleToScroll = Samples[i - 1];
            // TODO:             break;
            // TODO:         }
            // TODO:     }
            // TODO:     ScrollIntoView(sampleToScroll);
            // TODO: }
        }

        /// <summary>
        /// Handles when a sample is finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SampleFinished(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Handle when a sample is cancelled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SampleCancelled(object sender, classSampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Handle when a sample is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <param name="replaceExistingRows"></param>
        private void SampleQueue_SampleAdded(object sender, classSampleQueueArgs data, bool replaceExistingRows)
        {
            if (data?.Samples == null)
                return;

            SamplesAddedFromQueue(data.Samples, replaceExistingRows);
        }

        private void SamplesAddedFromQueue(IEnumerable<classSampleData> samples, bool replaceExistingRows)
        {
            //
            // The sample queue gives all of the samples
            //
            var sampleList = samples.ToList();

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

                AddSamplesToList(sampleList);
            }

            // TODO: if (Samples.Count > 0 && sampleList.Count > 0)
            // TODO: {
            // TODO:     ScrollIntoView(Samples.Last(x => x.Sample.Equals(sampleList.Last())));
            // TODO: }
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void SampleQueue_SamplesReordered(object sender, classSampleQueueArgs data)
        {
            using (Samples.SuppressChangeNotifications())
            {
                Samples.Clear();
                AddSamplesToList(data.Samples);
            }
            // TODO: if (Samples.Count > 0 && data.Samples.Any())
            // TODO: {
            // TODO:     ScrollIntoView(Samples.Last(x => x.Sample.Equals(data.Samples.Last())));
            // TODO: }
        }

        #endregion

        #region Form Control Event Handlers

        /// <summary>
        /// Gets or sets how to handle samples being deleted from columns
        /// </summary>
        public enumColumnDataHandling ColumnHandling { get; set; }

        #endregion
    }
}
