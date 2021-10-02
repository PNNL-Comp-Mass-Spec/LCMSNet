using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicData;
using DynamicData.Binding;
using LcmsNet.IO.DMS;
using LcmsNet.SampleQueue.ViewModels;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Experiment;
using LcmsNetSDK.Method;
using LcmsNetSQLiteTools;
using ReactiveUI;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Class to manage sample data and limit the number of copied data and operations that occur
    /// </summary>
    public class SampleDataManager : ReactiveObject
    {
        private readonly SourceList<SampleViewModel> samplesList = new SourceList<SampleViewModel>();

        public ReadOnlyObservableCollection<SampleViewModel> Samples { get; }
        public IObservableList<SampleViewModel> SamplesSource => samplesList;

        private readonly DMSSampleValidator mValidator;

        #region Column Events

        /// <summary>
        /// Handles when a property about a column changes and rebuilds the column ordering list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousStatus"></param>
        /// <param name="newStatus"></param>
        private void column_StatusChanged(object sender, ColumnStatus previousStatus, ColumnStatus newStatus)
        {
            // Make sure we have at least one column that is enabled
            var enabled = false;
            foreach (var column in CartConfiguration.Columns)
            {
                if (column.Status != ColumnStatus.Disabled)
                {
                    enabled = true;
                    break;
                }
            }

            // If at least one column is not enabled, then we disable the sample queue
            HasValidColumns = enabled;

            SampleQueue.UpdateAllSamples();
        }

        private bool hasValidColumns = true;
        private bool hasData = true;

        public bool HasValidColumns
        {
            get => hasValidColumns;
            private set => this.RaiseAndSetIfChanged(ref hasValidColumns, value);
        }

        public bool HasData
        {
            get => hasData;
            private set => this.RaiseAndSetIfChanged(ref hasData, value);
        }

        #endregion

        #region Members

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

        ///// <summary>
        ///// Flag that turns off the coloring when a PAL item (method, tray) was not downloadable from the PAL.
        ///// </summary>
        //private bool ignoreMissingPALValues;

        private bool dmsAvailable = false;
        private bool cycleColumns = false;

        public bool DMSAvailable
        {
            get => dmsAvailable;
            private set => this.RaiseAndSetIfChanged(ref dmsAvailable, value);
        }

        /// <summary>
        /// Gets or sets whether when adding samples,
        /// the column data should cycle through, (e.g. 1,2,3,4,1,2)
        /// </summary>
        public bool CycleColumns
        {
            get => cycleColumns;
            set => this.RaiseAndSetIfChanged(ref cycleColumns, value);
        }

        #endregion

        #region Constructors and Initialization

        /// <summary>
        /// Constructor that accepts dmsView and sampleQueue
        /// </summary>
        public SampleDataManager(SampleQueue sampleQueue)
        {
            var resortTrigger = samplesList.Connect().WhenValueChanged(x => x.Sample.SequenceID).Throttle(TimeSpan.FromMilliseconds(250)).Select(_ => Unit.Default);
            samplesList.Connect().Sort(SortExpressionComparer<SampleViewModel>.Ascending(x => x.Sample.SequenceID), resort: resortTrigger).ObserveOn(RxApp.MainThreadScheduler).Bind(out var samplesListBound).Subscribe();
            Samples = samplesListBound;

            foreach (var column in CartConfiguration.Columns)
            {
                column.StatusChanged += column_StatusChanged;
            }

            DMSAvailable = true;
            if (string.IsNullOrWhiteSpace(LCMSSettings.GetParameter(LCMSSettings.PARAM_DMSTOOL)))
            {
                DMSAvailable = false;
            }
            LCMSSettings.SettingChanged += LCMSSettings_SettingChanged;

            ColumnHandling = enumColumnDataHandling.Resort;

            try
            {
                mValidator = new DMSSampleValidator(CartConfigOptions);

                SampleQueue = sampleQueue;
                if (SampleQueue != null)
                {
                    SampleQueue.SamplesAdded += SampleQueue_SampleAdded;
                    SampleQueue.SamplesCancelled += SampleQueue_SampleCancelled;
                    SampleQueue.SamplesFinished += SampleQueue_SampleFinished;
                    SampleQueue.SamplesRemoved += SampleQueue_SampleRemoved;
                    SampleQueue.SamplesStarted += SampleQueue_SampleStarted;
                    SampleQueue.SamplesReordered += SampleQueue_SamplesReordered;
                    SampleQueue.SamplesUpdated += SampleQueue_SamplesUpdated;
                    SampleQueue.SamplesWaitingToRun += SampleQueue_SamplesWaitingToRun;
                    SampleQueue.SamplesStopped += SampleQueue_SamplesStopped;
                    HasData = true;
                }
                else
                {
                    HasData = false;
                }

                Initialize();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0,
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
            if (CartConfiguration.Columns != null)
            {
                foreach (var column in CartConfiguration.Columns)
                {
                    column.StatusChanged += column_StatusChanged;
                }
            }

            DMSAvailable = true;
            if (string.IsNullOrWhiteSpace(LCMSSettings.GetParameter(LCMSSettings.PARAM_DMSTOOL)))
            {
                DMSAvailable = false;
            }
            LCMSSettings.SettingChanged += LCMSSettings_SettingChanged;

            try
            {
                mValidator = new DMSSampleValidator();
                HasData = false;
                Initialize();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0,
                    "An exception occurred while trying to build the sample queue controls.  Constructor 2: " + ex.Message, ex);
            }

            samplesList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var samplesListBound).Subscribe();
            Samples = samplesListBound;

            #region DesignTimeData
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Queued",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Running",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Running,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Waiting",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.WaitingToRun,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Error",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Error,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_ErrorBlocked",
                    Block = 1,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Error,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Stopped",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Stopped,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_StoppedBlocked",
                    Block = 1,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Stopped,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Complete",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Complete,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Disabled_Column",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_" + SampleQueue.CONST_DEFAULT_INTEGRATE_SAMPLENAME,
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_HasErrorData",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "This is an error!",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design_Dataset_Duplicate",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = true,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "",
            }));
            samplesList.Add(new SampleViewModel(new SampleData()
            {
                DmsData = new DMSData()
                {
                    RequestName = "Design Dataset Bad Name",
                    Block = 0,
                },
                ColumnIndex = 0,
                IsDuplicateRequestName = false,
                RunningStatus = SampleRunningStatus.Queued,
                SampleErrors = "",
            }));
            #endregion
        }

        private void LCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName.Equals(LCMSSettings.PARAM_DMSTOOL) && e.SettingValue != string.Empty)
            {
                DMSAvailable = true;
            }
            if (e.SettingName.Equals(LCMSSettings.PARAM_CARTCONFIGNAME))
            {
                foreach (var sample in samplesList.Items)
                {
                    var data = sample.Sample.DmsData;
                    data.CartConfigName = e.SettingValue;
                }
            }
        }

        /// <summary>
        /// Performs initialization for the constructors.
        /// </summary>
        private void Initialize()
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

            foreach (var column in CartConfiguration.Columns)
            {
                column.NameChanged += column_NameChanged;
                column.ColorChanged += column_ColorChanged;
            }

            // Lists that hold information to be used by the sample queue combo boxes.
            autoSamplerMethods = new List<string>();
            autosamplerTrays = new List<string>();
            instrumentMethods = new List<string>();

            // Hook into the method manager so we can update list boxes when methods change...
            if (LCMethodManager.Manager != null)
            {
                LCMethodManager.Manager.MethodAdded += Manager_MethodAdded;
                LCMethodManager.Manager.MethodRemoved += Manager_MethodRemoved;
                LCMethodManager.Manager.MethodUpdated += Manager_MethodUpdated;
            }

            // Update Method Combo Boxes
            ShowAutoSamplerMethods();
            ShowAutoSamplerTrays();
            ShowInstrumentMethods();
            ShowLCSeparationMethods();

            samplesList.Connect().WhenPropertyChanged(x => x.IsChecked).Subscribe(x => HandleSampleValidationAndQueuing(x.Sender));
            samplesList.Connect().WhenPropertyChanged(x => x.RequestName).Subscribe(x => CheckForDuplicates(x.Sender.Sample));
            samplesList.Connect().WhenPropertyChanged(x => x.Sample.IsDuplicateRequestName).Subscribe(x => HandleDuplicateRequestNameChanged(x.Sender.Sample));
            // TODO: Check for side effects
            // TODO: The idea of this is that it would detect the minor changes to the queue, where a value was changed using the databinding. There needs to be a lockout for actions not taken via the databinding, since those already handle this process...
            samplesList.Connect().WhenAnyPropertyChanged(nameof(SampleViewModel.RequestName), nameof(SampleViewModel.Sample.DmsData),
                nameof(SampleViewModel.Sample.PAL), nameof(SampleViewModel.Sample.InstrumentMethod), nameof(SampleViewModel.ColumnNumber),
                nameof(SampleViewModel.InstrumentMethod), nameof(SampleViewModel.Sample.LCMethodName), nameof(SampleViewModel.Sample.SequenceID))
                .Throttle(TimeSpan.FromSeconds(.25))
                .Subscribe(this.ChangeMade);

            this.WhenAnyValue(x => x.SampleQueue.CanUndo).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.CanUndo, out canUndo);
            this.WhenAnyValue(x => x.SampleQueue.CanRedo).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.CanRedo, out canRedo);
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

            var currentTask = "Initializing";

            try
            {
                currentTask = "Determine number of selected items";
                currentTask = "Examine samples";

                if (changedSample.IsChecked)
                {
                    var foundError = false;
                    // Queue samples
                    foreach (var sample in samplesList.Items.OrderBy(x => x.Sample.UniqueID))
                    {
                        // bypass samples already in running queue
                        if (sample.Sample.IsSetToRunOrHasRun)
                        {
                            continue;
                        }

                        if (sample.Sample.UniqueID > changedSample.Sample.UniqueID)
                        {
                            // Safety check: we've already gone past the sample that was changed.
                            break;
                        }

                        var sampleErrors = string.Empty;
                        if (!sample.Sample.DmsData.DatasetNameCharactersValid())
                        {
                            sampleErrors += "Request name contains invalid characters!\n" +
                                                          DMSData.ValidDatasetNameCharacters + "\n";
                            foundError = true;
                        }
                        // Validate sample and add it to the run queue

                        // Validate sample.
                        if (string.IsNullOrEmpty(sampleErrors))
                        {
                            // Validate other parts of the sample.
                            var errors = new List<SampleValidationError>();
                            foreach (var reference in SampleValidatorManager.Instance.Validators)
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
                                if (sampleErrors.Length > 0)
                                {
                                    sampleErrors += "\n";
                                }
                                sampleErrors += string.Join("\n", errors.Select(x => x.ToString()));
                                foundError = true;
                            }
                        }

                        RxApp.MainThreadScheduler.Schedule(() => sample.Sample.SampleErrors = sampleErrors);

                        if (!foundError)
                        {
                            // Add to run queue
                            SampleQueue.MoveSamplesToRunningQueue(sample.Sample); // TODO: MainThreadWrap?
                        }

                        if (sample.Equals(changedSample))
                        {
                            if (foundError)
                            {
                                RxApp.MainThreadScheduler.Schedule(() => sample.IsChecked = false);
                            }
                            //Stop validating and queuing samples
                            break;
                        }
                    }
                }
                else
                {
                    // Dequeue samples - iterate in reverse
                    foreach (var sample in samplesList.Items.OrderBy(x => x.Sample.UniqueID).Reverse())
                    {
                        // bypass samples not set to run
                        if (!sample.Sample.IsSetToRunOrHasRun ||
                            sample.Sample.RunningStatus == SampleRunningStatus.Complete ||
                            sample.Sample.RunningStatus == SampleRunningStatus.Running)
                        {
                            continue;
                        }

                        if (sample.Sample.UniqueID < changedSample.Sample.UniqueID)
                        {
                            // Safety check: we've already gone past the sample that was changed.
                            break;
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
                ApplicationLogger.LogError(0, "Error in HandleSampleValidationAndQueuing, task " + currentTask, ex);
                MessageBox.Show(@"Error in HandleSampleValidationAndQueuing, task " + currentTask + @": " + ex.Message, @"Error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            lock (this)
            {
                currentlyProcessingQueueChange = false;
            }
        }

        // TODO: Should this be called by HandleSampleValidationAndQueuing?
        private bool ValidateSampleRowReadyToRun(SampleData sampleToCheck)
        {
            var isValid = true;

            if (sampleToCheck != null)
            {
                if (string.IsNullOrWhiteSpace(sampleToCheck.PAL.PALTray))
                {
                    // PAL tray not defined
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(sampleToCheck.LCMethodName))
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
        private void Manager_MethodRemoved(object sender, LCMethod method)
        {
            RemoveMethod(method);
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private void RemoveMethodName(string methodName)
        {
            RemoveMethodByName(methodName);
        }

        /// <summary>
        /// Determines if the method is associated with this sample view.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private bool ContainsMethod(string methodName)
        {
            foreach (var o in LcMethodOptions)
            {
                if (methodName == o.Name)
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
        private void Manager_MethodUpdated(object sender, LCMethod method)
        {
            // Update the samples first, then update the methods list.
            var samples = SampleQueue.GetWaitingQueue();
            var updateSamples = new List<SampleData>();
            foreach (var sample in samples)
            {
                if (sample.LCMethodName != null && sample.LCMethodName == method.Name)
                {
                    var newColID = method.Column;
                    if (newColID >= 0)
                    {
                        sample.ColumnIndex = CartConfiguration.Columns[newColID].ID;
                    }
                    sample.LCMethodName = method.Name;
                    updateSamples.Add(sample);
                }
            }

            var success = AddOrUpdateLCMethod(method);
            if (!success)
            {
                return;
            }

            SampleQueue.UpdateSamples(updateSamples);
        }

        /// <summary>
        /// Handles adding a new method to the list of available running methods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="method"></param>
        private void Manager_MethodAdded(object sender, LCMethod method)
        {
            var success = AddOrUpdateLCMethod(method);
            if (!success)
            {
                return;
            }

            // If we just added a sample, we want to make sure the samples have a method selected.
            if (lcMethodOptions.Count == 1)
            {
                foreach (var sample in samplesList.Items)
                {
                    sample.Sample.LCMethodName = method.Name;
                }

                SampleQueue.UpdateAllSamples();
            }
        }

        #endregion

        #region Virtual Queue Methods

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        public void Undo()
        {
            SampleQueue.Undo();
        }

        /// <summary>
        /// Undoes the last operation on the queue.
        /// </summary>
        public void Redo()
        {
            SampleQueue.Redo();
        }

        private bool recentlyChanged = false;

        private bool RecentlyChanged()
        {
            return recentlyChanged;
        }

        private void ChangeMade(SampleViewModel sample)
        {
            recentlyChanged = true;
            SampleQueue.UpdateSample(sample.Sample);
        }

        /// <summary>
        /// Start a batch change, where change tracking for undo operations will not occur until the returned object is disposed.
        /// </summary>
        /// <returns></returns>
        public SampleQueue.BatchChangeDisposable StartBatchChange()
        {
            recentlyChanged = false;
            return SampleQueue.StartBatchChange(RecentlyChanged);
        }

        /// <summary>
        /// Adds a sequence of samples to the manager.
        /// </summary>
        /// <param name="samples">List of samples to add to the manager.</param>
        /// <param name="insertIntoUnused"></param>
        public void AddSamplesToManager(List<SampleData> samples, bool insertIntoUnused)
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets the sample queue that handles all queue management at a low level.
        /// </summary>
        public SampleQueue SampleQueue { get; }

        /// <summary>
        /// Gets or sets a list of pal method names.
        /// </summary>
        public List<string> AutoSamplerMethods
        {
            get => autoSamplerMethods;
            set
            {
                autoSamplerMethods = value ?? new List<string>();
                ShowAutoSamplerMethods();
            }
        }

        /// <summary>
        /// Gets or sets a list of pal tray names.
        /// </summary>
        public List<string> AutoSamplerTrays
        {
            get => autosamplerTrays;
            set
            {
                //ApplicationLogger.LogMessage(0, "SAMPLE VIEW PROCESSING AUTOSAMPLER TRAYS!");
                autosamplerTrays = value ?? new List<string>();
                ShowAutoSamplerTrays();
            }
        }

        /// <summary>
        /// Gets or sets a list of instrument method names stored on the MS instrument.
        /// </summary>
        public List<string> InstrumentMethods
        {
            get => instrumentMethods;
            set
            {
                instrumentMethods = value ?? new List<string>();
                ShowInstrumentMethods();
            }
        }

        private ObservableAsPropertyHelper<bool> canUndo;
        private ObservableAsPropertyHelper<bool> canRedo;

        public bool CanUndo => canUndo?.Value ?? false;

        public bool CanRedo => canRedo?.Value ?? false;

        /// <summary>
        /// Gets the name of the un-used sample.
        /// </summary>
        public string UnusedSampleName => SampleQueue.UnusedSampleName;

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
            ApplicationLogger.LogError(0, msg);

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
            SetPalTrays(autosamplerTrays);
        }

        /// <summary>
        /// Updates the Instrument Method Column Combo Box.
        /// </summary>
        private void ShowInstrumentMethods()
        {
            SetInstrumentMethods(instrumentMethods);
        }

        /// <summary>
        /// Updates the LC-Method to the LC Separation Method Box.
        /// </summary>
        private void ShowLCSeparationMethods()
        {
            SetLCMethods(LCMethodManager.Manager.AllLCMethods);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="samples"></param>
        public void UpdateSamples(List<SampleData> samples)
        {
            SampleQueue.UpdateSamples(samples);
        }

        /// <summary>
        /// Reorders the samples provided as the argument by inserting the items in the queue.  Re-orders in place.
        /// </summary>
        /// <param name="newOrders">List of samples that contain the new ordering.</param>
        /// <param name="handling"></param>
        public void ReorderSamples(List<SampleData> newOrders, enumColumnDataHandling handling)
        {
            SampleQueue.ReorderSamples(newOrders, handling);
        }

        public void AddDateCartnameColumnIDToDatasetName(List<SampleData> samples)
        {
            if (samples.Count < 1)
                return;

            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            samples.RemoveAll(sample => sample.RunningStatus != SampleRunningStatus.Queued);

            if (samples.Count < 1)
                return;

            foreach (var sample in samples)
            {
                SampleData.AddDateCartColumnToDatasetName(sample);
            }
            SampleQueue.UpdateSamples(samples);
        }

        public void ResetDatasetName(List<SampleData> samples)
        {
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            samples.RemoveAll(sample => sample.RunningStatus != SampleRunningStatus.Queued);

            if (samples.Count < 1)
                return;

            foreach (var sample in samples)
            {
                SampleData.ResetDatasetNameToRequestName(sample);
            }

            SampleQueue.UpdateSamples(samples);
        }

        /// <summary>
        /// Handles when a column color has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="previousColor"></param>
        /// <param name="newColor"></param>
        private void column_ColorChanged(object sender, Color previousColor, Color newColor)
        {
            SampleQueue.UpdateAllSamples();

            // The following is necessary due to how samples are stored and read from a database
            // May be removed if code is updated to re-set LCMethod and ColumnData after data is loaded from a database or imported.
            foreach (var s in samplesList.Items)
            {
                s.Sample.ColumnIndex = CartConfiguration.Columns[s.Sample.ColumnIndex].ID;
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
            SampleQueue.UpdateWaitingSamples();

            // The following is necessary due to how samples are stored and read from a database
            // May be removed if code is updated to re-set LCMethod and ColumnData after data is loaded from a database or imported.
            foreach (var s in samplesList.Items)
            {
                s.Sample.ColumnIndex = CartConfiguration.Columns[s.Sample.ColumnIndex].ID;
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
        /// Returns whether the sample queue has an unused samples.
        /// </summary>
        /// <returns></returns>
        public bool HasUnusedSamples(ColumnData column)
        {
            return SampleQueue.HasUnusedSamples(column);
        }

        /// <summary>
        /// Copies the sample data from one object required to make a new Blank sample entry in the sample queue.
        /// </summary>
        /// <param name="sampleToCopy">Sample to copy</param>
        /// <returns>New object reference of a sample with only required data copied.</returns>
        public SampleData CopyRequiredSampleData(SampleData sampleToCopy)
        {
            var newSample = new SampleData(false)
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
                    Well = PalData.CONST_DEFAULT_VIAL_NUMBER,
                    PALTray = "",
                    WellPlate = sampleToCopy.PAL.WellPlate
                }
            };

            // Make sure we copy the column data.  If it's null, then it's probably a special method.
            if (sampleToCopy.ColumnIndex > -1)
            {
                var id = sampleToCopy.ColumnIndex;
                if (id < CartConfiguration.Columns.Count && id >= 0)
                {
                    newSample.ColumnIndex = CartConfiguration.Columns[sampleToCopy.ColumnIndex].ID;
                }
                else
                {
                    newSample.ColumnIndex = CartConfiguration.Columns[0].ID;
                    ApplicationLogger.LogError(1,
                        string.Format("The column data from the previous sample has an invalid column ID: {0}", id));
                }
            }

            // Then we make sure that even if the column data is null, we want to
            // make sure the column is pertinent to the method, since what column we run on depends
            // on what method we are trying to run.
            if (sampleToCopy.LCMethodName != null)
            {
                newSample.LCMethodName = sampleToCopy.LCMethodName;
                if (LCMethodManager.Manager.TryGetLCMethod(newSample.LCMethodName, out var method))
                {
                    var id = method.Column;
                    if (id < CartConfiguration.Columns.Count && id >= 0)
                    {
                        newSample.ColumnIndex = method.Column;
                    }
                    else
                    {
                        newSample.ColumnIndex = CartConfiguration.Columns[0].ID;
                        ApplicationLogger.LogError(1, $"The column data from the previous sample's method has an invalid column ID: {id}");
                    }
                }
            }
            else
            {
                newSample.LCMethodName = "";
            }

            // Clear out any DMS information from the blank
            newSample.DmsData.Batch = 0;
            newSample.DmsData.Block = 0;
            newSample.DmsData.Comment = "Blank";
            newSample.DmsData.Experiment = "";
            newSample.DmsData.MRMFileID = 0;
            newSample.DmsData.EMSLProposalID = "";
            newSample.DmsData.RequestID = 0;
            newSample.DmsData.RunOrder = 0;
            newSample.DmsData.EMSLUsageType = "";

            return newSample;
        }

        #endregion

        #region Queue User Interface Methods

        /// <summary>
        /// Adds a new sample to the list view.
        /// </summary>
        public SampleData AddNewSample(bool insertIntoUnused)
        {
            SampleData newData = null;

            // If we have a sample, get the previous sample data.
            if (samplesList.Count > 0)
            {
                var data = samplesList.Items.Last().Sample;
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
                // Otherwise, add a new sample.
                newData = new SampleData(false)
                {
                    DmsData =
                    {
                        RequestName = string.Format("{0}_{1:0000}",
                            SampleQueue.DefaultSampleName,
                            SampleQueue.RunningSampleIndex++),
                        CartName = CartConfiguration.CartName,
                        CartConfigName = CartConfiguration.CartConfigName
                    }
                };
            }

            if (newData != null)
            {
                AddSamplesToManager(new List<SampleData> { newData }, insertIntoUnused);
            }
            return newData;
        }

        /// <summary>
        /// Adds a sample to the listview.
        /// </summary>
        /// <param name="sample">Sample to display in the list view.</param>
        /// <returns>True if addition was a success, or false if adding sample failed.</returns>
        public bool AddSamplesToList(SampleData sample)
        {
            if (sample == null)
            {
                return false;
            }
            if (!samplesList.Items.Any(x => x.Sample.Equals(sample)))
            {
                if (string.IsNullOrWhiteSpace(sample.DmsData.CartConfigName))
                {
                    sample.DmsData.CartConfigName = LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME);
                }
                samplesList.Add(new SampleViewModel(sample));
                // TODO: Verify: sorting should be handled by the SourceList observers
                //samplesList.Sort((x, y) => x.Sample.SequenceID.CompareTo(y.Sample.SequenceID));
                UpdateRow(sample);
            }
            return true;
        }

        /// <summary>
        /// Adds samples to the list but optimizes layout and updates for rendering controls.
        /// </summary>
        /// <param name="samples">Sample to display in the list view.</param>
        /// <returns>True if addition was a success (or samples were already present), or false if adding sample failed.</returns>
        public bool AddSamplesToList(IEnumerable<SampleData> samples)
        {
            var existingItems = samplesList.Items.ToList();
            var newItems = new List<SampleViewModel>();
            foreach (var sample in samples.Where(x => x != null && !existingItems.Any(y => y.Sample.Equals(x))))
            {
                //AddSamplesToList(sample);
                if (newItems.Any(x => x.Sample.Equals(sample)))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(sample.DmsData.CartConfigName))
                {
                    sample.DmsData.CartConfigName = LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME);
                }
                // Add the new items to a temporary list.
                newItems.Add(new SampleViewModel(sample));
                // TODO: Verify: sorting should be handled by the SourceList observers
                //samplesList.Sort((x, y) => x.Sample.SequenceID.CompareTo(y.Sample.SequenceID));
                // The following does not reference Samples or samplesList.
                UpdateRow(sample);
            }

            // Add all new samples to the observed list as a single change
            samplesList.AddRange(newItems);

            return true;
        }

        /// <summary>
        /// Clear all of the samples (deleting them from the queue as well).
        /// </summary>
        public void ClearAllSamples()
        {
            // Remove all of them from the sample queue.
            // This should update the other views as well.
            SampleQueue.RemoveSample(samplesList.Items.Select(x => x.Sample.UniqueID).ToList(), enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Moves all the selected samples an offset of their original sequence id.
        /// </summary>
        public void MoveSamples(List<SampleData> data, int offset, MoveSampleType moveType)
        {
            // Remove any samples that have already been run, waiting to run, or had an error (== has run).
            data.RemoveAll(sample => sample.RunningStatus != SampleRunningStatus.Queued);

            if (data.Count < 1)
                return;

            // We have to make sure the data is sorted by sequence
            // number in order for the sample queue movement to work
            data.Sort(new SequenceComparer());

            // Move in the sample queue
            SampleQueue.MoveQueuedSamples(data, 0, offset, moveType);
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        public void RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            SampleQueue.RemoveUnusedSamples(resortColumns);
        }

        /// <summary>
        /// Removes the unused samples in the columns.
        /// </summary>
        public void RemoveUnusedSamples(ColumnData column, enumColumnDataHandling resortColumns)
        {
            SampleQueue.RemoveUnusedSamples(column, resortColumns);
        }

        /// <summary>
        /// Removes the selected samples from the list view.
        /// </summary>
        public void RemoveSamples(List<SampleViewModel> samples, enumColumnDataHandling resortColumns)
        {
            try
            {
                // Get a list of sequence ID's to remove
                var removes = new List<long>();
                var samplesToRemove = samples.OrderBy(x => x.Sample.SequenceID).ToList();
                foreach (var sample in samplesToRemove)
                {
                    removes.Add(sample.Sample.UniqueID);
                }

                // Remove all of them from the sample queue.
                // This should update the other views as well.
                SampleQueue.RemoveSample(removes, resortColumns);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception in RemoveSelectedSamples: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Updates the provided row by determining if the sample data class is valid or not.
        /// </summary>
        /// <param name="data"></param>
        private void CheckForDuplicates(SampleData data)
        {
            // Color duplicates or invalid cells with certain colors!
            var validResult = SampleQueue.IsSampleDataValid(data);
            if (validResult == SampleValidResult.DuplicateRequestName &&
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

        private void HandleDuplicateRequestNameChanged(SampleData data)
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
                foreach (var sample in samplesList.Items.Where(x => x.RequestName.Equals(data.DmsData.RequestName)))
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
                foreach (var sample in samplesList.Items.Where(x => x.Sample.IsDuplicateRequestName))
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
        public void UpdateRow(SampleData sample)
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
        private void SampleQueue_SamplesUpdated(object sender, SampleQueueArgs data)
        {
            SamplesUpdated(sender, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SamplesUpdated(object sender, SampleQueueArgs data)
        {
            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Sets the pulldownbar position and updates the view of the samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SamplesStopped(object sender, SampleQueueArgs data)
        {
            SamplesUpdated(sender, data);
        }

        /// <summary>
        /// Handled when the samples are stopped from the sample queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SamplesStopped(object sender, SampleQueueArgs data)
        {
            SamplesStopped(sender, data);
        }

        /// <summary>
        /// Handles when a sample is queued for running but no open slot exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SamplesWaitingToRun(object sender, SampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Updates the rows for the given samples.
        /// </summary>
        /// <param name="samples">Samples to update view of.</param>
        public void UpdateRows(IEnumerable<SampleData> samples)
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
        private void SampleQueue_SampleStarted(object sender, SampleQueueArgs data)
        {
            if (data?.Samples == null)
                return;

            UpdateRows(data.Samples);
        }

        /// <summary>
        /// Handle when a sample is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SampleRemoved(object sender, SampleQueueArgs data)
        {
            // Start fresh and add the samples from the queue to the list.
            // But track the position of the scroll bar to be nice to the user.
            // TODO: var backup = samplesList.ToList();

            samplesList.Clear();
            AddSamplesToList(data.Samples);

            // TODO: if (samplesList.Count > 0)
            // TODO: {
            // TODO:     SampleViewModel sampleToScroll = samplesList.First();
            // TODO:     for (var i = 0; i < backup.Count; i++)
            // TODO:     {
            // TODO:         if (i >= samplesList.Count || !backup[i].Equals(samplesList[i]))
            // TODO:         {
            // TODO:             sampleToScroll = samplesList[i - 1];
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
        private void SampleQueue_SampleFinished(object sender, SampleQueueArgs data)
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
        private void SampleQueue_SampleCancelled(object sender, SampleQueueArgs data)
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
        private void SampleQueue_SampleAdded(object sender, SampleQueueArgs data, bool replaceExistingRows)
        {
            if (data?.Samples == null)
                return;

            SamplesAddedFromQueue(data.Samples, replaceExistingRows);
        }

        private void SamplesAddedFromQueue(IEnumerable<SampleData> samples, bool replaceExistingRows)
        {
            // The sample queue gives all of the samples
            var sampleList = samples.ToList();

            if (replaceExistingRows && samplesList.Count > 0)
            {
                try
                {
                    samplesList.Clear();
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Ignoring exception in SamplesAddedFromQueue: " + ex.Message);
                }
            }

            AddSamplesToList(sampleList);

            // TODO: if (samplesList.Count > 0 && sampleList.Count > 0)
            // TODO: {
            // TODO:     ScrollIntoView(samplesList.Last(x => x.Sample.Equals(sampleList.Last())));
            // TODO: }
        }

        /// <summary>
        /// Reorders the samples after the queue determines which ones to re-order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SampleQueue_SamplesReordered(object sender, SampleQueueArgs data)
        {
            samplesList.Clear();
            AddSamplesToList(data.Samples);

            // TODO: if (samplesList.Count > 0 && data.Samples.Any())
            // TODO: {
            // TODO:     ScrollIntoView(samplesList.Last(x => x.Sample.Equals(data.Samples.Last())));
            // TODO: }
        }

        #endregion

        #region Form Control Event Handlers

        /// <summary>
        /// Gets or sets how to handle samples being deleted from columns
        /// </summary>
        public enumColumnDataHandling ColumnHandling { get; set; }

        #endregion

        #region Static data - ComboBoxOptions

        private static readonly SourceList<LCMethod> lcMethodOptions = new SourceList<LCMethod>();
        private static readonly SourceList<string> instrumentMethodOptions = new SourceList<string>();
        private static readonly SourceList<string> palTrayOptions = new SourceList<string>();

        public static ReadOnlyObservableCollection<LCMethod> LcMethodOptions { get; }
        public static ReadOnlyObservableCollection<string> LcMethodNameOptions { get; }
        public static ReadOnlyObservableCollection<string> InstrumentMethodOptions { get; }
        public static ReadOnlyCollection<string> DatasetTypeOptions { get; }
        public static ReadOnlyCollection<string> CartConfigOptions { get; }
        public static ReadOnlyObservableCollection<string> PalTrayOptions { get; }

        // If null, no error retrieving the cart config names from the database; otherwise, the error that occurred
        public static string CartConfigOptionsError { get; }

        static SampleDataManager()
        {
            CartConfigOptionsError = null;

#if DEBUG
            // Avoid exceptions caused from not being able to access program settings, when being run to provide design-time data context for the designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                CartConfigOptionsError = "Values not read, because we are running in Visual Studio's Design Mode";
                return;
            }
#endif

            // Add the dataset type items to the data grid
            var datasetTypeOptions = new List<string>();
            DatasetTypeOptions = datasetTypeOptions.AsReadOnly();
            try
            {
                var datasetTypes = SQLiteTools.GetDatasetTypeList(false);
                datasetTypeOptions.AddRange(datasetTypes.ToList());
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(1, "The sample queue could not load the dataset type list.", ex);
                DatasetTypeOptions = new List<string>().AsReadOnly();
            }

            // Get the list of cart configuration names from DMS
            var cartConfigOptions = new List<string>();
            CartConfigOptions = cartConfigOptions.AsReadOnly();
            try
            {
                var totalConfigCount = SQLiteTools.GetCartConfigNameList(false).Count();
                var cartName = CartConfiguration.CartName;
                var cartConfigList = SQLiteTools.GetCartConfigNameList(cartName, false).ToList();
                if (cartConfigList.Count > 0)
                {
                    cartConfigOptions.AddRange(cartConfigList.ToList());
                }
                else
                {
                    if (totalConfigCount > 0)
                    {
                        CartConfigOptionsError = "No cart configurations found that match the supplied cart name: \"" + cartName + "\".\n" +
                                                 "Fix: close, fix the cart name, and restart.";
                    }
                    else
                    {
                        CartConfigOptionsError = "No cart configurations found. Can this computer communicate with DMS? Fix and restart LCMSNet";
                    }
                }
            }
            catch (DatabaseConnectionStringException ex)
            {
                // The SQLite connection string wasn't found
                var errMsg = ex.Message + " while getting LC cart config name listing.\r\n" +
                             "Please close LcmsNet program and correct the configuration file";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
            }
            catch (DatabaseDataException ex)
            {
                // There was a problem getting the list of LC carts from the cache db
                var innerException = string.Empty;
                if (ex.InnerException != null)
                    innerException = ex.InnerException.Message;
                var errMsg = "Exception getting LC cart config name list from DMS: " + innerException + "\r\n" +
                             "As a workaround, you may manually type the cart config name when needed.\r\n" +
                             "You may retry retrieving the cart list later, if desired.";
                MessageBox.Show(errMsg, "LcmsNet", MessageBoxButton.OK);
            }

            lcMethodOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcMethodOptionsBound).Subscribe();
            lcMethodOptions.Connect().Transform(x => x.Name).ObserveOn(RxApp.MainThreadScheduler).Bind(out var lcMethodNamesBound).Subscribe();
            instrumentMethodOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var instrumentMethodOptionsBound).Subscribe();
            palTrayOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var palTrayOptionsBound).Subscribe();
            LcMethodOptions = lcMethodOptionsBound;
            LcMethodNameOptions = lcMethodNamesBound;
            InstrumentMethodOptions = instrumentMethodOptionsBound;
            PalTrayOptions = palTrayOptionsBound;
        }

        private static void SetPalTrays(IEnumerable<string> trays)
        {
            palTrayOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(trays);
            });
        }

        private static void SetInstrumentMethods(IEnumerable<string> instrumentMethods)
        {
            instrumentMethodOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(instrumentMethods);
            });
        }

        private static void SetLCMethods(IEnumerable<LCMethod> lcMethods)
        {
            lcMethodOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(lcMethods);
            });
        }

        private static bool AddOrUpdateLCMethod(LCMethod method)
        {
            // make sure the method is not null
            if (method == null)
                return false;

            // Find the method if name exists
            var found = false;
            LCMethod foundMethod = null;
            foreach (var o in LcMethodOptions)
            {
                var name = o.Name;
                if (name.Equals(method.Name))
                {
                    found = true;
                    foundMethod = o;
                    break;
                }
            }

            // Update or add the method
            if (found == false)
            {
                lcMethodOptions.Add(method);
            }
            else
            {
                // Here we update the method that was in the list, with the new one that was added/updated
                lcMethodOptions.Edit(list =>
                {
                    var indexOf = list.IndexOf(foundMethod);
                    if (indexOf >= 0)
                        list[indexOf] = method;
                });
            }

            return true;
        }

        private static bool RemoveMethod(LCMethod method)
        {
            if (method == null)
                return false;

            foreach (var o in LcMethodOptions.ToList())
            {
                if (o.Name.Equals(method.Name))
                {
                    lcMethodOptions.Remove(o);
                    break;
                }
            }
            return true;
        }

        private static void RemoveMethodByName(string methodName)
        {
            LCMethod oFound = null;
            foreach (var o in LcMethodOptions)
            {
                var name = o.ToString();
                if (methodName == name)
                {
                    oFound = o;
                }
            }
            if (oFound != null)
            {
                lcMethodOptions.Remove(oFound);
            }
        }

        #endregion
    }
}
