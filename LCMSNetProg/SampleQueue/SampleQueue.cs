using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using DynamicData;
using LcmsNet.Data;
using LcmsNet.SampleQueue.IO;
using LcmsNetSDK;
using LcmsNetSDK.Configuration;
using LcmsNetSDK.Data;
using LcmsNetSDK.Experiment;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Defines what to do with the sample queue
    /// </summary>
    public enum enumColumnDataHandling
    {
        /// <summary>
        /// Resorts the samples to the appropriate columns.
        /// </summary>
        Resort,

        /// <summary>
        /// Distributes samples across columns appropriately.
        /// </summary>
        CreateUnused,

        /// <summary>
        /// Don't do anything to the samples column Data
        /// </summary>
        LeaveAlone
    }

    /// <summary>
    /// Class that manages the order, updating, addition, and deletion of samples.
    /// </summary>
    public class SampleQueue : INotifyPropertyChangedExt
    {
        /// <summary>
        /// Default new sample name.
        /// </summary>
        private const string CONST_DEFAULT_SAMPLENAME = "blank";

        /// <summary>
        /// Name used when distributed samples across columns.
        /// </summary>
        public const string CONST_DEFAULT_INTEGRATE_SAMPLENAME = "(unused)";

        private const bool REPLACE_EXISTING_ROWS = true;

        /// <summary>
        /// Running count of the sequence index to use when running samples.
        /// </summary>
        private int m_sequenceIndex;

        /// <summary>
        /// Index of next available sample in the running queue.
        /// </summary>
        private int m_nextAvailableSample;

        /// <summary>
        /// Index count of samples
        /// </summary>
        private int m_sampleIndex;

        /// <summary>
        /// SourceList of all samples - not sorted, items are not moved in this list.
        /// </summary>
        private readonly SourceList<SampleData> sampleQueue = new SourceList<SampleData>();

        /// <summary>
        /// Collection of all samples, sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> SortedQueue => sampleQueue.Items.OrderBy(x => x.SequenceID);

        /// <summary>
        /// Collection of sample data not set to be run. Index 0 should be the next sample to run. Sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> NotScheduledSamples => sampleQueue.Items.Where(x => x.HasNotRun && x.RunningStatus != SampleRunningStatus.WaitingToRun).OrderBy(x => x.SequenceID);

        /// <summary>
        /// Collection of samples that are currently waiting to run. Sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> WaitingToRunQueue => sampleQueue.Items.Where(x => x.RunningStatus == SampleRunningStatus.WaitingToRun).OrderBy(x => x.SequenceID);

        /// <summary>
        /// Collection of samples that are currently running. Sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> RunningQueue => sampleQueue.Items.Where(x => x.RunningStatus == SampleRunningStatus.WaitingToRun || x.RunningStatus == SampleRunningStatus.Running).OrderBy(x => x.SequenceID);

        /// <summary>
        /// Collection of samples that are currently running or have completed running. Sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> RunningOrCompletedQueue => sampleQueue.Items.Where(x => x.IsSetToRunOrHasRun).OrderBy(x => x.SequenceID).ToList();

        /// <summary>
        /// Collection of samples that are currently running or have completed running. Sorted by sequenceID
        /// </summary>
        private IEnumerable<SampleData> CompletedQueue => sampleQueue.Items.Where(x => x.RunningStatus == SampleRunningStatus.Complete).OrderBy(x => x.SequenceID);

        /// <summary>
        /// Next unique ID to use in the sample queue.
        /// </summary>
        // TODO: Make sure this plays nice with the undo/redo capabilities.
        private long lastUniqueId = 0;

        /// <summary>
        /// List of columns that are enabled or disabled.
        /// </summary>
        private List<ColumnData> m_columnOrders;

        /// <summary>
        /// Flag indicating whether the samples were started explicitly by the caller.
        /// If false the queries for next samples will return null indicating no samples can be started.
        /// If true, and samples finish or are cancelled (i.e. errors) then the flag will reset to false.
        /// </summary>
        private bool m_startedSamples;

        private readonly SampleQueueUndoRedo undoRedoHandler = new SampleQueueUndoRedo();
        private bool canUndo = false;
        private bool canRedo = false;

        /// <summary>
        /// Default constructor for a sample queue.
        /// </summary>
        public SampleQueue()
        {
            DefaultSampleName = CONST_DEFAULT_SAMPLENAME;
            AutoColumnData = true;
            m_sampleIndex = 1;
            m_sampleWaitingEvent = new AutoResetEvent(false);
            m_columnOrders = new List<ColumnData>();

            // Pointer to the next available sample that is queued for running.
            m_nextAvailableSample = 0;

            // Tracks what is the current sequence number that has been run previously.
            m_sequenceIndex = 1;

            UpdateColumnList();
            foreach (var column in CartConfiguration.Columns)
            {
                column.FirstChanged += column_FirstChanged;
            }
        }

        void column_FirstChanged(object sender, bool first)
        {
            if (first)
            {
                UpdateColumnList();
            }
        }

        private void UpdateColumnList()
        {
            m_columnOrders.Clear();
            m_columnOrders = CartConfiguration.BuildColumnList(true);
        }

        public IObservableList<SampleData> SampleQueueSource => sampleQueue;

        /// <summary>
        /// Gets the next column a sample will be added on.
        /// </summary>
        public ColumnData NextColumnData
        {
            get
            {
                // Make sure that we have enough columns to add data on.
                if (m_columnOrders.Count < 1)
                    return null;

                // Get the index of the last sample that is queued, or -1...
                var index = SortedQueue.LastOrDefault()?.ColumnIndex ?? -1;

                // Then return the column data available at the end of queue
                return m_columnOrders[(index + 1) % m_columnOrders.Count];
            }
        }

        /// <summary>
        /// Gets the name of the un-used sample - Name of samples that are added through distribution across columns.
        /// </summary>
        public string UnusedSampleName => CONST_DEFAULT_INTEGRATE_SAMPLENAME;

        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public List<ColumnData> ColumnOrder => m_columnOrders;

        /// <summary>
        /// Gets or sets whether to reset the column data when a queue operation is performed.
        /// </summary>
        public bool AutoColumnData { get; set; }

        /// <summary>
        /// Gets or sets the default name of the sample to add when distributing across columns.
        /// </summary>
        public string DefaultSampleName { get; set; }

        /// <summary>
        /// Gets or sets the running sample index of samples that have been
        /// added to the queue.
        /// </summary>
        public int RunningSampleIndex
        {
            get => m_sampleIndex;
            set => m_sampleIndex = value;
        }

        /// <summary>
        /// Gets the threading event when a sample is queued.
        /// </summary>
        public AutoResetEvent SampleQueuedEvent => m_sampleWaitingEvent;

        /// <summary>
        /// If there are queues in the undo queue
        /// </summary>
        /// <returns></returns>
        public bool CanUndo
        {
            get => canUndo;
            private set => this.RaiseAndSetIfChanged(ref canUndo, value);
        }

        /// <summary>
        /// If there are queues in the redo queue
        /// </summary>
        /// <returns></returns>
        public bool CanRedo
        {
            get => canRedo;
            private set => this.RaiseAndSetIfChanged(ref canRedo, value);
        }

        /// <summary>
        /// Delegate definition for when a sample is modified.
        /// </summary>
        /// <param name="sender">Sample Queue that made the call.</param>
        /// <param name="data">Data associated with the addition.</param>
        public delegate void DelegateSamplesAddedHandler(object sender, SampleQueueArgs data, bool replaceExistingRows);

        /// <summary>
        /// Delegate definition for when a sample is modified.
        /// </summary>
        /// <param name="sender">Sample Queue that made the call.</param>
        /// <param name="data">Data associated with the addition.</param>
        public delegate void DelegateSamplesModifiedHandler(object sender, SampleQueueArgs data);

        /// <summary>
        /// Fired when a sample is added to a queue.
        /// </summary>
        public event DelegateSamplesAddedHandler SamplesAdded;

        /// <summary>
        /// Fired when a sample is cancelled.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesCancelled;

        /// <summary>
        /// Fired when a sample completes its run.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesFinished;

        /// <summary>
        /// Fired when a sample is removed from the queue.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesRemoved;

        /// <summary>
        /// Fired when a sample has started running.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesStarted;

        /// <summary>
        /// Fired when sample information is updated in the queue.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesUpdated;

        /// <summary>
        /// Fired when the sample execution is stopped.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesStopped;

        /// <summary>
        /// Fired when a sample is reordered.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesReordered;

        /// <summary>
        /// Fired when a sample has been told to run, and is waiting for a free column thread.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesWaitingToRun;

        /// <summary>
        /// Event to tell listeners that the sample is waiting to be run.
        /// </summary>
        public AutoResetEvent m_sampleWaitingEvent;

        /// <summary>
        /// Creates a new unique ID number not used and stores it.  This always generates the largest unique ID.
        /// </summary>
        /// <returns>Unique ID number.</returns>
        private long GenerateUniqueID()
        {
            return Interlocked.Increment(ref lastUniqueId);
        }

        /// <summary>
        /// Updates the provided row by determining if the sample data class is valid or not.
        /// If <paramref name="sample"/> is a duplicate name, all entries that match are flagged
        /// </summary>
        /// <param name="sample"></param>
        public void CheckForDuplicates(SampleData sample)
        {
            // Determine if the sample has a duplicate request name.
            // If it has one match, then it should be itself.
            // Search the queue and add any instance of a copy
            var samples = sampleQueue.Items.Where(x => x.Name.Equals(sample.Name)).ToList();

            var isDuplicate = samples.Count > 1 && !sample.Name.Contains(UnusedSampleName);
            foreach (var item in samples)
            {
                item.IsDuplicateName = isDuplicate;
            }
        }

        /// <summary>
        /// Checks all samples marked at duplicate names to determine if that flag should be cleared.
        /// </summary>
        public void CheckClearDuplicateFlag()
        {
            // Check everything that was flagged as a duplicate name
            foreach (var sample in sampleQueue.Items.Where(x => x.IsDuplicateName).GroupBy(x => x.Name))
            {
                var groupItems = sample.ToList();
                // If it has one match, then it should be itself.
                if (groupItems.Count == 1 || sample.Key.Contains(UnusedSampleName))
                {
                    foreach (var item in groupItems)
                    {
                        item.IsDuplicateName = false;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the waiting sample queue has any unused samples.
        /// </summary>
        /// <returns></returns>
        public bool HasUnusedSamples()
        {
            // Find the first sample that has an unused name
            foreach (var sample in NotScheduledSamples)
            {
                if (sample.Name == UnusedSampleName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a column has unused samples (samples that are not tied to experimental information.);
        /// </summary>
        /// <param name="column">Column to check</param>
        /// <returns>True if the column has unused samples, false if not.</returns>
        public bool HasUnusedSamples(ColumnData column)
        {
            // Find the first sample that has an unused name
            foreach (var sample in NotScheduledSamples)
            {
                if (sample.Name == UnusedSampleName && column.ID == sample.ColumnIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Builds a histogram keyed on column that contains a list of samples.
        /// </summary>
        /// <returns>Histogram.  Empty if no samples exist.</returns>
        private Dictionary<ColumnData, List<SampleData>> BuildSampleHistogram()
        {
            // Create a queue histogram.
            var sampleHistogram =
                new Dictionary<ColumnData, List<SampleData>>();
            foreach (var col in m_columnOrders)
            {
                sampleHistogram.Add(col, new List<SampleData>());
            }

            // Calculate the Histogram
            foreach (var data in NotScheduledSamples.ToList())
            {
                var col = CartConfiguration.Columns[data.ColumnIndex];
                if (sampleHistogram.ContainsKey(col))
                {
                    sampleHistogram[col].Add(data);
                }
            }

            return sampleHistogram;
        }

        /// <summary>
        /// Distributes samples across columns to evenly add samples to the queue.
        /// </summary>
        private void DistributeSamplesAcrossColumns(Dictionary<ColumnData, List<SampleData>> histogram)
        {
            var queue = NotScheduledSamples.ToList();

            // Figure out how many items we have on this column.
            var maxCount = -1;
            foreach (var data in histogram.Values)
            {
                maxCount = Math.Max(data.Count, maxCount);
            }

            // Dummy start index - just needs to be in the range of int and sortable, and beyond the SequenceID of any queued ot completed sample
            var sequenceId = 1000000000;
            var unusedSamplesToAdd = new List<SampleData>();

            // Now we have a histogram of columns whose entries are a List of samples
            // For every column whose List Count is less than the maximum, add unused samples to it
            // At this point the waiting list does not mean anything.  We will re-construct
            // the waiting queue after this point.
            foreach (var col in histogram.Keys)
            {
                var data = histogram[col];
                while (data.Count < maxCount)
                {
                    SampleData sample;
                    if (queue.Count > 0)
                    {
                        sample = queue[queue.Count - 1].Clone(false);
                    }
                    else
                    {
                        sample = new SampleData();
                        unusedSamplesToAdd.Add(sample);
                    }

                    var sampleToAdd = sample;

                    // Make the request an "Unused sample"
                    sampleToAdd.Name = UnusedSampleName;
                    sampleToAdd.ColumnIndex = col.ID;
                    sampleToAdd.UniqueID = GenerateUniqueID();
                    data.Add(sampleToAdd);
                }
            }

            var renumberList = new List<SampleData>(histogram.Values.Sum(x => x.Count));
            // Here we reconstruct the waiting queue.  We use the column orders list (tells us
            // what column is the first one to use) to add samples in order.
            var firstList = histogram[m_columnOrders[0]];
            for (var i = 0; i < firstList.Count; i++)
            {
                var firstSample = firstList[i];
                renumberList.Add(firstSample);

                // Now add each sample found on another column
                for (var j = 1; j < m_columnOrders.Count; j++)
                {
                    var col = m_columnOrders[j];
                    var sample = histogram[col][i];
                    renumberList.Add(sample);
                }
            }

            foreach (var item in renumberList)
            {
                item.SequenceID = sequenceId++;
            }

            sampleQueue.AddRange(unusedSamplesToAdd);
        }

        /// <summary>
        /// Removes excess samples (names who are just placeholders) from the ends of the queue.
        /// </summary>
        private void RemoveExcessUnusedSamples()
        {
            // Remove excess items from the end of the list.
            var toRemove = new List<SampleData>();
            var notScheduled = NotScheduledSamples.ToList();
            for (var i = notScheduled.Count - 1; i >= 0 && notScheduled[i].Name == UnusedSampleName; i--)
            {
                toRemove.Add(notScheduled[i]);
            }

            sampleQueue.RemoveMany(toRemove);
        }

        /// <summary>
        /// Re-sequences the waiting queue starting from the smallest sequence number.  It always starts the sequence number from 1
        /// </summary>
        public void ResequenceQueuedSamples()
        {
            if (NotScheduledSamples.Any())
            {
                var sequence = RunningOrCompletedQueue.LastOrDefault()?.SequenceID ?? 0;

                sequence++;

                m_sequenceIndex = sequence;

                ResequenceQueuedSamples(sequence);
            }
        }

        /// <summary>
        /// Resequences the samples in the queued samples.
        /// </summary>
        /// <param name="startSequence">Start offset to resequence from.</param>
        private void ResequenceQueuedSamples(int startSequence)
        {
            foreach (var data in NotScheduledSamples.ToList())
            {
                data.SequenceID = startSequence++;
            }
            m_sequenceIndex = startSequence;
        }

        #region Undo/Redo

        /// <summary>
        /// Adds the current waiting queue to the list of undoable actions
        /// </summary>
        public void AddToUndoable()
        {
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_EnableUndoRedo, true))
            {
                undoRedoHandler.Clear();
                return;
            }

            if (isUndoRedoing || isBatchChange)
            {
                return;
            }
            undoRedoHandler.AddToUndoable(NotScheduledSamples.ToList());
            SetCanUndoRedo();
        }

        /// <summary>
        /// Tells the undo/redo tracking that a batch change is going to occur, and to not track individual undo/redo actions until the returned object is disposed.
        /// </summary>
        /// <returns></returns>
        public BatchChangeDisposable StartBatchChange(Func<bool> changeMonitor)
        {
            // Make sure the current queue is saved
            AddToUndoable();
            isBatchChange = true;
            return new BatchChangeDisposable(EndBatchChange, changeMonitor);
        }

        private void EndBatchChange(bool cancelled = false, bool changesApplied = false)
        {
            isBatchChange = false;
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_EnableUndoRedo, true))
            {
                undoRedoHandler.Clear();
                return;
            }

            if (changesApplied)
            {
                if (cancelled)
                {
                    Undo();
                }
                else
                {
                    AddToUndoable();
                }
            }
        }

        public class BatchChangeDisposable : IDisposable
        {
            private readonly Action<bool, bool> disposeMethod;
            private readonly Func<bool> changeMonitor;

            /// <summary>
            /// If set to true, the performed changes will be removed.
            /// </summary>
            public bool Cancelled { get; set; }

            public BatchChangeDisposable(Action<bool, bool> onDisposeMethod, Func<bool> changeCheckMethod)
            {
                disposeMethod = onDisposeMethod;
                changeMonitor = changeCheckMethod;
                Cancelled = false;
            }

            public void Dispose()
            {
                disposeMethod(Cancelled, changeMonitor());
                GC.SuppressFinalize(this);
            }

            ~BatchChangeDisposable()
            {
                Dispose();
            }
        }

        public bool IsDirty { get; private set; }
        private bool isUndoRedoing = false;
        private bool isBatchChange = false;

        private void SetCanUndoRedo()
        {
            CanUndo = undoRedoHandler.CanUndo;
            CanRedo = undoRedoHandler.CanRedo;
            IsDirty = undoRedoHandler.IsDirty;
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        public void Undo()
        {
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_EnableUndoRedo, true))
            {
                undoRedoHandler.Clear();
                return;
            }

            if (isUndoRedoing || isBatchChange)
            {
                return;
            }
            isUndoRedoing = true;

            if (undoRedoHandler.Undo(out var undoItems))
            {
                sampleQueue.Edit(list =>
                {
                    var toRemove = new List<SampleData>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i].HasNotRun && list[i].RunningStatus != SampleRunningStatus.WaitingToRun)
                        {
                            SampleData item = null;
                            var matchFound = false;
                            foreach (var undoItem in undoItems)
                            {
                                if (undoItem.UniqueID == list[i].UniqueID)
                                {
                                    item = undoItem;
                                    matchFound = true;
                                }
                            }

                            if (matchFound)
                            {
                                undoItems.Remove(item);
                                list[i] = item;
                            }
                            else
                            {
                                toRemove.Add(list[i]);
                            }
                        }
                    }

                    list.RemoveMany(toRemove);
                    // Add any "undoItems" that didn't have a match
                    list.AddRange(undoItems);
                });

                //ResetColumnData();
                SamplesAdded?.Invoke(this, new SampleQueueArgs(GetAllSamples()), REPLACE_EXISTING_ROWS);
            }

            SetCanUndoRedo();
            isUndoRedoing = false;
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        public void Redo()
        {
            if (!LCMSSettings.GetParameter(LCMSSettings.PARAM_EnableUndoRedo, true))
            {
                undoRedoHandler.Clear();
                return;
            }

            if (isUndoRedoing || isBatchChange)
            {
                return;
            }
            isUndoRedoing = true;

            if (undoRedoHandler.Redo(out var redoItems))
            {
                sampleQueue.Edit(list =>
                {
                    var toRemove = new List<SampleData>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i].HasNotRun && list[i].RunningStatus != SampleRunningStatus.WaitingToRun)
                        {
                            SampleData item = null;
                            var matchFound = false;
                            foreach (var redoItem in redoItems)
                            {
                                if (redoItem.UniqueID == list[i].UniqueID)
                                {
                                    item = redoItem;
                                    matchFound = true;
                                }
                            }

                            if (matchFound)
                            {
                                redoItems.Remove(item);
                                list[i] = item;
                            }
                            else
                            {
                                toRemove.Add(list[i]);
                            }
                        }
                    }

                    list.RemoveMany(toRemove);
                    // Add any "redoItems" that didn't have a match
                    list.AddRange(redoItems);
                });

                //ResetColumnData();
                SamplesAdded?.Invoke(this, new SampleQueueArgs(GetAllSamples()), REPLACE_EXISTING_ROWS);
            }

            SetCanUndoRedo();
            isUndoRedoing = false;
        }

        #endregion

        #region Adding, Removing, Updating Samples from Queue

        /// <summary>
        /// Inserts samples into the
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="handling"></param>
        public void InsertIntoUnusedSamples(List<SampleData> samples, enumColumnDataHandling handling)
        {
            // Overwrite all of the samples that are unused first.
            // Then add to the queue if the number of unused samples are less
            // than the number of provided ones.
            sampleQueue.Edit(list =>
            {
                var i = 0;
                while (i < list.Count && samples.Count > 0)
                {
                    if (!list[i].HasNotRun || list[i].RunningStatus == SampleRunningStatus.WaitingToRun)
                    {
                        continue;
                    }

                    if (list[i].Name.Contains(UnusedSampleName))
                    {
                        var sample = samples[0];
                        samples.RemoveAt(0);
                        sample.SequenceID = list[i].SequenceID;
                        list[i] = sample;
                        list[i].UniqueID = GenerateUniqueID();
                    }

                    i++;
                }
            });

            // ask if there are any leftovers
            if (samples.Count > 0)
            {
                // Don't need to re-sequence or notify
                // because queue samples does it for us.
                QueueSamples(samples, handling);
            }
            else
            {
                // Always re-sequence, and notify
                // TODO: This isn't actually changing anything!!!
                ResequenceQueuedSamples();

                if (handling == enumColumnDataHandling.Resort)
                {
                    ResetColumnData();
                }

                SamplesAdded?.Invoke(this, new SampleQueueArgs(GetAllSamples()), REPLACE_EXISTING_ROWS);
            }

            AddToUndoable();
        }

        /// <summary>
        /// Moves the samples provided in the list by offset in the waiting queue.
        /// Order of input samples should be current order in queue.
        /// </summary>
        /// <param name="samples">Samples to move.</param>
        /// <param name="baseOffset"></param>
        /// <param name="offset">Number of samples to move by.</param>
        /// <param name="moveType">How to move the samples.</param>
        public void MoveQueuedSamples(List<SampleData> samples, int baseOffset, int offset,
            MoveSampleType moveType)
        {
            SwapQueuedSamplesColumn(samples, baseOffset, offset);
        }

        /// <summary>
        /// Adds a sample to the list of waiting samples.
        /// </summary>
        /// <param name="sampleList">List of samples to add.</param>
        /// <param name="distributeSamplesEvenlyAcrossColumns">Tells the queue operation whether to distribute samples on other columns</param>
        /// <returns>True if addition was a success.  False if addition failed.</returns>
        public bool QueueSamples(IEnumerable<SampleData> sampleList,
            enumColumnDataHandling distributeSamplesEvenlyAcrossColumns)
        {
            var added = false;

            // Here we add to the waiting queue.
            //     1. Distribute the samples across the waiting queue using a histogram
            //         to figure out the distribution of samples.  This version will
            //         add unused samples that never get run but help balance the
            //         queue building.
            //     2. Add the samples directly back to the waiting queue - append.
            //         This just puts samples on the queue.

            // Keep track of new samples
            var tempQueue = new List<SampleData>();
            var notScheduled = NotScheduledSamples.ToList();
            if (distributeSamplesEvenlyAcrossColumns == enumColumnDataHandling.CreateUnused)
            {
                var sampleHistogram = BuildSampleHistogram();
                // Add samples to their respective columns.
                foreach (var sample in sampleList)
                {
                    // Check for object references.
                    // We allow for the same request name to be had in the
                    // sample list.
                    if (!notScheduled.Contains(sample))
                    {
                        added = true;
                        sample.UniqueID = GenerateUniqueID();
                        var col = CartConfiguration.Columns[sample.ColumnIndex];
                        sampleHistogram[col].Add(sample);
                        tempQueue.Add(sample);
                    }
                }

                DistributeSamplesAcrossColumns(sampleHistogram);
            }
            else
            {
                // Dummy start index - just needs to be in the range of int and sortable, and beyond the SequenceID of any queued ot completed sample
                var sequenceId = 1000000000;
                foreach (var data in sampleList)
                {
                    data.UniqueID = GenerateUniqueID();
                    data.SequenceID = sequenceId++;
                    tempQueue.Add(data);
                    added = true;
                }
            }

            sampleQueue.AddRange(tempQueue);

            // Now make sure we alert everyone that we added a sample
            if (added)
            {
                // Only re-sequence and remove if we changed the queue to not waste time
                // doing something if we don't have to.
                RemoveExcessUnusedSamples();
                ResequenceQueuedSamples();

                SamplesAdded?.Invoke(this, new SampleQueueArgs(GetAllSamples()), REPLACE_EXISTING_ROWS);
            }

            AddToUndoable();

            return added;
        }

        /// <summary>
        /// Updates all listeners that the sample data has been updated.
        /// </summary>
        public void UpdateAllSamples()
        {
            if (SamplesAdded != null)
            {
                var args = new SampleQueueArgs(GetAllSamples());
                SamplesAdded?.Invoke(this, args, false);
            }
        }

        /// <summary>
        /// Updates all listeners that the sample data has been updated.
        /// </summary>
        public void UpdateWaitingSamples()
        {
            if (SamplesAdded != null)
            {
                var args = new SampleQueueArgs(GetWaitingQueue());
                SamplesAdded?.Invoke(this, args, false);
            }
        }

        /// <summary>
        /// Compiles all of the samples from the sample queue.
        /// </summary>
        /// <returns>List of all run, running, and waiting samples.</returns>
        private List<SampleData> GetAllSamples()
        {
            return SortedQueue.ToList();
        }

        /// <summary>
        /// Gets a list of all the samples in the waiting queue
        /// </summary>
        /// <returns>List of samples in waiting queue</returns>
        public List<SampleData> GetWaitingQueue()
        {
            return NotScheduledSamples.ToList();
        }

        /// <summary>
        /// Gets a list of all the samples in the waiting queue
        /// </summary>
        /// <returns>List of samples in waiting queue</returns>
        public List<SampleData> GetRunningQueue()
        {
            return RunningQueue.ToList();
        }

        /// <summary>
        /// Removes samples from the waiting queue found in the list of unique ids.
        /// </summary>
        /// <param name="uniqueIDs">List of unique id's.</param>
        /// <param name="resortColumns"></param>
        /// <returns>True if removed, false if not.</returns>
        public bool RemoveSample(List<long> uniqueIDs, enumColumnDataHandling resortColumns)
        {
            // Remove the sample from the complete queue.
            // First we figure out what samples we need to remove based on their
            // Unique ID's.  Then we remove them.
            var removed = false;
            sampleQueue.Edit(list =>
            {
                var toRemove = list.Where(x => x.RunningStatus != SampleRunningStatus.Running && x.RunningStatus != SampleRunningStatus.WaitingToRun
                    && uniqueIDs.Contains(x.UniqueID)).ToList();
                removed = toRemove.Count > 0;
                list.RemoveMany(toRemove);
            });

            // If we removed then we want to tell someone what samples were removed.
            // Later we re-sequence them.
            // Also, make sure we reseat samples on new columns.  We do this because
            // removed can only be true if we removed some samples instead of renaming.
            if (removed)
            {
                switch (resortColumns)
                {
                    case enumColumnDataHandling.Resort:
                        ResetColumnData();
                        break;
                    case enumColumnDataHandling.LeaveAlone:
                        break;
                    case enumColumnDataHandling.CreateUnused:
                        // Then, if some were removed we build up the distribution list and fill back into the queue
                        // allowing for the correct spacing on other columns, but inserting unused samples at the end of this column list
                        // so we don't disturb other column Data.  We do this using the same mechanism that adding samples uses
                        // but here we are not adding new samples to the queue.
                        var histogram = BuildSampleHistogram();
                        DistributeSamplesAcrossColumns(histogram);
                        RemoveExcessUnusedSamples();
                        break;
                }

                ResequenceQueuedSamples();

                SamplesRemoved?.Invoke(this, new SampleQueueArgs(GetAllSamples()));

                SamplesUpdated?.Invoke(this, new SampleQueueArgs(NotScheduledSamples.ToList()));
            }

            AddToUndoable();

            return removed;
        }

        /// <summary>
        /// Remove all of the samples that are in the queue that are unused.  Columns will
        /// be resorted since no column data was specified.
        /// </summary>
        /// <returns>True if any samples were removed.  False if not.</returns>
        public bool RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            // Find the unique id's of all the samples that are unused.
            var uniqueList = new List<long>();
            foreach (var sample in NotScheduledSamples)
            {
                if (sample.Name == UnusedSampleName)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            var ret = false;
            // Removes the samples from this list, AND! resorts the columns.
            // This is important for this method because it means that
            // we don't care about preserving column information.
            if (uniqueList.Count > 0)
            {
                ret = RemoveSample(uniqueList, resortColumns);
            }

            AddToUndoable();

            return ret;
        }

        /// <summary>
        /// Removes unused samples on the given column.
        /// </summary>
        /// <param name="column">Column to remove samples from.</param>
        /// <param name="resortColumns"></param>
        /// <returns>True if any samples were removed.  False if not.</returns>
        public bool RemoveUnusedSamples(ColumnData column, enumColumnDataHandling resortColumns)
        {
            // Find the unique id's of all the samples that are unused.
            var uniqueList = new List<long>();
            foreach (var sample in NotScheduledSamples)
            {
                if (sample.Name == UnusedSampleName && sample.ColumnIndex == column.ID)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            var ret = false;
            // Removes the samples from this list, AND DOES NOT! Resort the columns.
            // This is important for this method because it means that
            // we DO care about preserving column information.
            if (uniqueList.Count > 0)
            {
                ret = RemoveSample(uniqueList, resortColumns);
            }

            AddToUndoable();

            return ret;
        }

        /// <summary>
        /// Resets the column data after a move, delete operation.
        /// </summary>
        public void ResetColumnData()
        {
            var index = (RunningOrCompletedQueue.LastOrDefault()?.ColumnIndex ?? -1) + 1;

            sampleQueue.Edit(list =>
            {
                foreach (var data in list.Where(x => x.HasNotRun).OrderBy(x => x.SequenceID))
                {
                    var columnData = m_columnOrders[index % m_columnOrders.Count];
                    data.ColumnIndex = columnData.ID;
                    index++;
                }
            });
        }

        /// <summary>
        /// Moves samples specified by the offset
        /// </summary>
        /// <param name="samples">List of samples to swap.</param>
        /// <param name="baseOffset">Base offset to swap by.</param>
        /// <param name="offset">Spacing between columns.</param>
        private void SwapQueuedSamplesColumn(List<SampleData> samples, int baseOffset, int offset)
        {
            // First restrict the list of sample we move to samples that are not completed, running, or waiting to run
            var sampleIds = samples.Select(x => x.UniqueID).ToList();
            var notScheduled = NotScheduledSamples.ToList();
            samples = notScheduled.Where(x => sampleIds.Contains(x.UniqueID)).ToList();

            if (samples.Count < 1)
                return;

            var firstSequenceNumber = notScheduled.FirstOrDefault()?.SequenceID ?? 1;

            // If the offset is positive then move the samples down in the list - towards end of queue
            if (offset > 0)
            {
                // Sort descending
                samples.Sort((x, y) => y.SequenceID.CompareTo(x.SequenceID));

                //sampleQueue.Edit(list =>
                //{
                    // Items that have to accomodate the move of samples
                    var toMove = notScheduled.Where(x => x.HasNotRun && x.RunningStatus != SampleRunningStatus.WaitingToRun).Except(samples).OrderByDescending(x => x.SequenceID).ToList();
                    //var toMove = list.Where(x => x.HasNotRun && x.RunningStatus != SampleRunningStatus.WaitingToRun).Except(samples).OrderByDescending(x => x.SequenceID).ToList();
                    var sampleIndex = 0;

                    // While there are still items to move
                    while (sampleIndex < samples.Count)
                    {
                        // Find a continuous blocks of moving samples, from largest SequenceID to smallest; adjust the SequenceIDs of the samples we are moving as we go
                        var continuousHigh = samples[sampleIndex].SequenceID;
                        samples[sampleIndex].SequenceID += offset;
                        var continuousLow = continuousHigh;
                        for (var i = sampleIndex + 1; i < samples.Count; i++)
                        {
                            var id = samples[i].SequenceID;
                            if (continuousLow - id > 1)
                            {
                                // There's a gap, update the "non-moved" samples to accomodate this shift, then continue after the gap
                                break;
                            }

                            // update sampleIndex to avoid undesirable double moves
                            sampleIndex = i;
                            continuousLow = id;
                            samples[i].SequenceID += offset;
                    }

                        // update sampleIndex to avoid undesirable double moves
                        sampleIndex++;

                        // Update the "non-moved" samples with new SequenceIDs; should perfectly accommodate the moved samples without a gap (unless the moved samples were moved beyond the end of the list)
                        var toMoveOffset = continuousHigh - continuousLow + 1;
                        foreach (var item in toMove.Where(x => continuousLow <= x.SequenceID && x.SequenceID <= continuousHigh + offset))
                        {
                            item.SequenceID -= toMoveOffset;
                        }
                    }
                //});
            }
            // Otherwise move the samples up in the queue order.
            else if (offset < 0)
            {
                // Sort ascending
                samples.Sort((x, y) => x.SequenceID.CompareTo(y.SequenceID));

                //sampleQueue.Edit(list =>
                //{
                    // Items that have to accomodate the move of samples
                    var toMove = notScheduled.Where(x => x.HasNotRun && x.RunningStatus != SampleRunningStatus.WaitingToRun).Except(samples).OrderBy(x => x.SequenceID).ToList();
                    //var toMove = list.Where(x => x.HasNotRun && x.RunningStatus != SampleRunningStatus.WaitingToRun).Except(samples).OrderBy(x => x.SequenceID).ToList();
                    var sampleIndex = 0;

                    // While there are still items to move
                    while (sampleIndex < samples.Count)
                    {
                        // Find a continuous blocks of moving samples, from largest SequenceID to smallest; adjust the SequenceIDs of the samples we are moving as we go
                        var continuousLow = samples[sampleIndex].SequenceID;
                        samples[sampleIndex].SequenceID += offset;
                        var continuousHigh = continuousLow;
                        for (var i = sampleIndex + 1; i < samples.Count; i++)
                        {
                            var id = samples[i].SequenceID;
                            if (id - continuousHigh > 1)
                            {
                                // There's a gap, update the "non-moved" samples to accomodate this shift, then continue after the gap
                                break;
                            }

                            // update sampleIndex to avoid undesirable double moves
                            sampleIndex = i;
                            continuousHigh = id;
                            samples[i].SequenceID += offset;
                    }

                        // update sampleIndex to avoid undesirable double moves
                        sampleIndex++;

                        // Update the "non-moved" samples with new SequenceIDs; should perfectly accommodate the moved samples without a gap (unless the moved samples were moved beyond the end of the list)
                        var toMoveOffset = continuousHigh - continuousLow + 1;
                        foreach (var item in toMove.Where(x => continuousLow + offset <= x.SequenceID && x.SequenceID <= continuousHigh))
                        {
                            item.SequenceID += toMoveOffset;
                        }
                    }
                //});
            }

            // Update the sequence numbers
            ResequenceQueuedSamples(firstSequenceNumber);

            // Update the column data
            //if (handling == enumColumnDataHandling.Resort)
            //{
            //    ResetColumnData();
            //}

            // Tell listeners that we have re-sequenced the queue.
            SamplesReordered?.Invoke(this, new SampleQueueArgs(GetAllSamples()));
        }

        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="data"></param>
        public void UpdateSample(SampleData data)
        {
            var updated = false;
            sampleQueue.Edit(list =>
            {
                // Find the sample
                // IEquatable implementation only checks UniqueID
                var index = list.IndexOf(data);
                if (index >= 0 && list[index].HasNotRun)
                {
                    updated = true;
                    // Update...although the reference should be updated.
                    list[index] = data;
                }
            });

            if (updated)
            {
                // Alert listening objects.
                SamplesUpdated?.Invoke(this, new SampleQueueArgs(NotScheduledSamples));
            }

            AddToUndoable();
        }

        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="samples"></param>
        public void UpdateSamples(List<SampleData> samples)
        {
            var updated = false;
            sampleQueue.Edit(list =>
            {
                foreach (var sample in samples)
                {
                    // IEquatable implementation only checks UniqueID
                    var index = list.IndexOf(sample);
                    if (index >= 0)
                    {
                        updated = true;
                        list[index] = sample;
                    }
                }
            });

            // Alert listening objects.
            if (updated)
            {
                SamplesUpdated?.Invoke(this, new SampleQueueArgs(NotScheduledSamples));
            }

            AddToUndoable();
        }

        #endregion

        /// <summary>
        /// Add a special sample to be run immediately, preempting other methods not yet run
        /// </summary>
        /// <param name="sample"></param>
        public void RunNext(SampleData sample)
        {
            sampleQueue.Edit(list =>
            {
                var firstNotRunSequenceId = 1;
                foreach (var notRunSample in list.Where(x => x.HasNotRun).OrderByDescending(x => x.SequenceID))
                {
                    // Capture the lowest "not run" SequenceID, and increment the SequenceID for all of the "not run" samples
                    firstNotRunSequenceId = notRunSample.SequenceID++;
                }

                sample.SequenceID = firstNotRunSequenceId;
                sample.RunningStatus = SampleRunningStatus.WaitingToRun;
                list.Add(sample);
            });


            UpdateAllSamples();

            if (!IsRunning)
            {
                StartSamples();
            }
        }

        #region Running Samples and Queue Operation

        /// <summary>
        /// Gets a value indicating if samples are ready to be run.
        /// </summary>
        public bool AreSamplesAvailableToRun => WaitingToRunQueue.Any();

        /// <summary>
        /// Gets whether there are samples currently set with running status.
        /// </summary>
        public bool IsRunning => m_nextAvailableSample > 0;

        /// <summary>
        /// Starts the samples
        /// </summary>
        public void StartSamples()
        {
            // Don't run if nothing is there to run.
            var waitingToRun = WaitingToRunQueue.ToList();
            if (waitingToRun.Count == 0)
            {
                return;
            }

            if (m_nextAvailableSample > 0)
            {
                return;
            }

            // Setup the queue to be optimized.

            // Could keep track of the samples with this:
            // var validSamples = new List<SampleData>();

            foreach (var sample in waitingToRun)
            {
                var next = TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10));

                if (sample.LCMethodName == null)
                {
                    sample.LCMethodName = "";
                }

                if (sample.ActualLCMethod == null)
                {
                    ApplicationLogger.LogError(0, "LCMethod.Clone() returned a null method in StartSamples");
                    continue;
                }

                // validSamples.Add(sample);
                sample.ActualLCMethod.SetStartTime(next);

                // We need to look for Daylight Savings Time Transitions and adjust for them here.
                if (TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(sample.ActualLCMethod.Start,
                    sample.ActualLCMethod.End))
                {
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                        "QUEUE: some samples have been moved forward 1 hour due to a Daylight Savings Transition, this will avoid odd behavior while running the queue.");

                    sample.ActualLCMethod.SetStartTime(next.Add(new TimeSpan(1, 0, 0)));
                }
            }

            // We have not started to run so optimize this way.
            var optimizer = new LCMethodOptimizer();
            Debug.WriteLine("Optimizing samples that are queued to run before starting the queue");
            optimizer.AlignSamples(RunningQueue.Cast<ISampleInfo>().ToList());

            // Set the listening event so that time sensitive items will know that
            // a sample is waiting on the running queue.
            m_sampleWaitingEvent.Set();
            m_startedSamples = true;
        }

        /// <summary>
        /// Puts the sample on the running queue and starts if the operator has appended this sample to an
        /// already running queue.
        /// </summary>
        /// <param name="sample">Sample to run.</param>
        public void MoveSamplesToRunningQueue(SampleData sample)
        {
            MoveSamplesToRunningQueue(new List<SampleData> { sample });
        }

        /// <summary>
        /// Tells the scheduler to run these samples, putting them on the waiting (running) queue.
        /// </summary>
        /// <param name="samples"></param>
        public void MoveSamplesToRunningQueue(List<SampleData> samples)
        {
            // For each sample to run, set the status and delay the run for 10 seconds
            var validSamples = new List<SampleData>();

            var optimizer = new LCMethodOptimizer();
            foreach (var sample in samples)
            {
                //DateTime next = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).Add(new TimeSpan(0, 0, 10));
                var next = TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10));
                var realSample = sample;
                if (sample.IsDummySample)
                {
                    realSample = SortedQueue.FirstOrDefault(x => x.UniqueID == sample.UniqueID);
                }
                if (realSample == null)
                {
                    // this sample does not exist on the sample queue!
                    throw new NullReferenceException("This sample does not exist in the waiting queue! " +
                                                     sample.Name);
                }

                if (realSample.LCMethodName == null)
                {
                    var requestOrDatasetName = realSample.Name;

                    ApplicationLogger.LogError(0, "Method not defined for sample ID " + realSample.UniqueID + ", " + requestOrDatasetName);
                    continue;
                }

                sample.SetActualLcMethod();

                if (realSample.ActualLCMethod == null)
                {
                    ApplicationLogger.LogError(0, "LCMethod.Clone() returned a null method in MoveSamplesToRunningQueue");
                    continue;
                }
                validSamples.Add(realSample);

                if (RunningQueue.Any() && m_nextAvailableSample > 0)
                {
                    Debug.WriteLine("Optimizing sample against running queue.");
                    // We aren't the first ones on the queue, but we are running,
                    // so we need to hurry up and go!
                    realSample.ActualLCMethod.SetStartTime(next);
                    optimizer.AlignSamples(RunningQueue.Cast<ISampleInfo>().ToList(), realSample);
                }
                else if (!RunningQueue.Any())
                {
                    Debug.WriteLine("Setting sample start time as it is first in running queue.");
                    // Otherwise we are the first ones on the queue, but we don't need to do anything
                    // for alignment.
                    realSample.ActualLCMethod.SetStartTime(next);
                }

                realSample.RunningStatus = SampleRunningStatus.WaitingToRun;
            }

            var args = new SampleQueueArgs(validSamples, m_nextAvailableSample, RunningQueue.Count());

            SamplesWaitingToRun?.Invoke(this, args);
        }

        /// <summary>
        /// Dequeues the samples from the running queue back onto the waiting queue.
        /// </summary>
        /// <param name="sample">Sample to dequeue.</param>
        public void DequeueSampleFromRunningQueue(SampleData sample)
        {
            DequeueSampleFromRunningQueue(new List<SampleData> { sample });
        }

        /// <summary>
        /// Moves a sample from the running queue back onto the waiting queue.
        /// </summary>
        /// <param name="samples">Samples to put back on the waiting queue.</param>
        public void DequeueSampleFromRunningQueue(List<SampleData> samples)
        {
            // For each sample to run, set the status and delay the run for 10 seconds
            var validSamples = new List<SampleData>();
            foreach (var sample in samples)
            {
                var realSample = sample;
                if (sample.IsDummySample)
                {
                    realSample = SortedQueue.FirstOrDefault(x => x.UniqueID == sample.UniqueID);
                }

                if (realSample == null)
                {
                    // this sample does not exist on the sample queue!
                    throw new NullReferenceException("This sample does not exist in the waiting queue! " + sample.Name);
                }

                realSample.RunningStatus = SampleRunningStatus.Queued;
                validSamples.Add(realSample);
            }

            var args = new SampleQueueArgs(validSamples, m_nextAvailableSample, RunningQueue.Count());

            SamplesWaitingToRun?.Invoke(this, args);
        }

        /// <summary>
        /// Cancel the run provided and appends to the waiting queue.
        /// </summary>
        /// <param name="sampleData">Sample to cancel.</param>
        /// <param name="error"></param>
        public void CancelRunningSample(SampleData sampleData, bool error)
        {
            if (sampleData == null)
                throw new NullReferenceException("The sample provided to cancel was null.");

            // Find the sample provided to cancel.
            var sample = RunningQueue.FirstOrDefault(item => item.UniqueID == sampleData.UniqueID);
            if (sample == null)
            {
                var errorMessage = string.Format("The sample {0} was not found on the running Queue.", sampleData);
                //throw new classSampleNotRunningException(errorMessage);
                return;
            }

            // Remove the sample from the running queue
            //var status = SampleRunningStatus.Stopped;
            //if (error)
            //{
            //    status = SampleRunningStatus.Error;
            //}
            sample.RunningStatus = SampleRunningStatus.Stopped;
            m_nextAvailableSample = Math.Max(m_nextAvailableSample - 1, 0);

            var samples = new[] { sample };
            var args = new SampleQueueArgs(samples, m_nextAvailableSample, RunningQueue.Count());

            if (m_nextAvailableSample == 0 && !WaitingToRunQueue.Any())
            {
                m_startedSamples = false;
            }

            SamplesCancelled?.Invoke(this, args);
            SamplesWaitingToRun?.Invoke(this, args);
        }

        /// <summary>
        /// Stops all of the queue jobs and puts anything on the running queue back into the regular queue to be run later.
        /// </summary>
        public void StopRunningQueue()
        {
            sampleQueue.Edit(list =>
            {
                foreach (var sample in list.Where(x => x.RunningStatus == SampleRunningStatus.Running || x.RunningStatus == SampleRunningStatus.WaitingToRun))
                {
                    sample.RunningStatus = SampleRunningStatus.Queued;
                }
            });

            m_nextAvailableSample = 0;

            var args = new SampleQueueArgs(NotScheduledSamples, m_nextAvailableSample, RunningQueue.Count());
            m_startedSamples = false;

            SamplesStopped?.Invoke(this, args);

            SamplesWaitingToRun?.Invoke(this, args);
        }

        /// <summary>
        /// Moves the sample from the running queue to the finished queue.
        /// </summary>
        /// <param name="sampleData"></param>
        public void FinishSampleRun(SampleData sampleData)
        {
            // uhhh this sample was null
            if (sampleData == null)
            {
                //TODO: Add error display.
                //throw new NullReferenceException("The sample that is said done was not an actual sample");
                return;
            }

            // Find the sample provided to complete.
            var sample = RunningQueue.FirstOrDefault(item => item.UniqueID == sampleData.UniqueID);
            if (sample == null)
            {
                var errorMessage = $"The sample {sampleData} was not found on the running Queue.";
                ApplicationLogger.LogError(1, errorMessage, null, sampleData);
                //TODO: BLL Removed because of the notification system.
                // throw new classSampleNotRunningException(errorMessage);
                return;
            }

            // Moves the sample pointer backward to the front of the running queue.
            m_nextAvailableSample = Math.Max(m_nextAvailableSample - 1, 0);
            sample.RunningStatus = SampleRunningStatus.Complete;

            var args = new SampleQueueArgs(new[] { sample }, m_nextAvailableSample, RunningQueue.Count());

            if (m_nextAvailableSample == 0 && !WaitingToRunQueue.Any())
            {
                m_startedSamples = false;
            }

            SamplesFinished?.Invoke(this, args);
            SamplesWaitingToRun?.Invoke(this, args);
        }

        /// <summary>
        /// Start the next sample
        /// </summary>
        public SampleData NextSampleStart()
        {
            SampleData sample = null;

            // Dequeue the sample, and start it.
            var nextSample = WaitingToRunQueue.FirstOrDefault();
            if (nextSample != null && m_startedSamples)
            {
                sample = nextSample;
                m_nextAvailableSample++;
                sample.RunningStatus = SampleRunningStatus.Running;

                SamplesStarted?.Invoke(this, new SampleQueueArgs(new[] { sample }));
            }

            var args = new SampleQueueArgs(new[] { sample }, m_nextAvailableSample, RunningQueue.Count());

            SamplesWaitingToRun?.Invoke(this, args);
            return sample;
        }

        /// <summary>
        /// Gets the next sample waiting to be run.  Similar to a peek method for file reading,
        /// i.e. does not remove from queue or change running status.
        /// </summary>
        /// <returns></returns>
        public SampleData NextSampleQuery()
        {
            return WaitingToRunQueue.FirstOrDefault();
        }

        #endregion

        #region Cache and Save Operations

        /// <summary>
        /// Writes each of the queue lists to a CSV cache file
        /// </summary>
        public void CacheQueue()
        {
            // Clean up the queues first

            CsvCache.SaveQueueToCache(SortedQueue);
            IsDirty = false;
        }

        /// <summary>
        /// Caches the queue and changes the default name.
        /// </summary>
        /// <param name="cachePath"></param>
        public void CacheQueue(string cachePath)
        {
            CsvCache.SetCacheLocation(cachePath);
            LCMSSettings.SetParameter(LCMSSettings.PARAM_CACHEFILENAME, CsvCache.CacheName);
            CacheQueue();
        }

        /// <summary>
        /// Loads samples stored in the cache back into the user interface and queue.
        /// </summary>
        public void RetrieveQueueFromCache()
        {
            // Loads the samples and creates unique sequence ID's and unique id's
            var waitingSamples = CsvCache.GetQueueFromCache().ToList();

            // Update the Waiting Sample queue with the right LC-Methods.  This makes sure
            // that we always have valid LC-methods.  Otherwise when we go to run the
            // scheduler and thus program will crash hard.
            foreach (var sample in waitingSamples) // m_waitingQueue)
            {
                if (LcmsNetSDK.Method.LCMethodManager.Manager.TryGetLCMethod(sample.LCMethodName, out var method))
                {
                    // reset the column data.
                    var column = CartConfiguration.Columns[method.Column];
                    sample.ColumnIndex = column.ID;
                }

                if (sample.UniqueID >= m_sampleIndex)
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID);

                if (sample.SequenceID >= m_sequenceIndex)
                    m_sequenceIndex = Convert.ToInt32(sample.SequenceID + 1);
            }

            //ResetColumnData(true);

            // Put them on the waiting queue - we do it this way so that
            // we push stuff on the undo/redo stack.  That logic is already in place
            // in the queue samples method.  We also see if no samples were added (updated == false)
            // and completeQueue.length > 0 that we force an update.  Otherwise if no samples
            // were left in the cache on the waiting queue but were on the complete queue, then
            // we wouldn't see the completed samples...big bug.
            var updated = QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
            if (updated == false && CompletedQueue.Any())
            {
                if (SamplesAdded != null)
                {
                    var args = new SampleQueueArgs(GetAllSamples());
                    SamplesAdded?.Invoke(this, args, true);
                }
            }

            IsDirty = false;
        }

        /// <summary>
        /// Saves the queue to the appropriate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writer"></param>
        public void SaveQueue(string path, ISampleQueueWriter writer)
        {
            writer.WriteSamples(path, SortedQueue.Where(x => x.RunningStatus != SampleRunningStatus.Complete).ToList());
        }

        /// <summary>
        /// Saves the queue to the appropriate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reader"></param>
        public void LoadQueue(string path, ISampleQueueReader reader)
        {
            var waitingSamples = reader.ReadSamples(path);

            // Make sure the method references are created
            foreach (var sample in waitingSamples)
            {
                if (sample.UniqueID >= m_sampleIndex)
                {
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID);
                }
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Saves the queue to the appropriate object.
        /// </summary>
        /// <param name="newSamples"></param>
        public void LoadQueue(List<SampleData> newSamples)
        {
            var waitingSamples = newSamples;

            // Make sure the method references are created
            foreach (var sample in waitingSamples)
            {
                if (sample.UniqueID >= m_sampleIndex)
                {
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID);
                }
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Saves the queue to the appropriate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        public void LoadQueue(string path, ISampleQueueReader reader, ColumnData column)
        {
            var waitingSamples = reader.ReadSamples(path);

            // Here we need to assign the column data information to the samples
            // if one has not already been assigned.
            foreach (var data in waitingSamples)
            {
                data.ColumnIndex = column.ID;
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }

        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}