//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
/* Last modified 01/16/2009
 *
 *      1/16/2009:  Brian LaMarche
 *          Added starting, cancelling queue operations with associated events.
 *
 *          2/12/2009: Dave Clark
 *              Added method for saving queue to SQLite database
 *          3/17/2009: BLL
 *              Generates unique ID's and sample ID's
 *          5/18/2010: DAC
 *                Modified for queue export to SQLite file and CSV file
 */
//*********************************************************************************************************

using System;
using System.Threading;
using System.Collections.Generic;
using LcmsNetSQLiteTools;
using LcmsNet.Method;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Configuration;
using LcmsNetDataClasses.Experiment;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Defines what to do with the sample queue
    /// </summary>
    public enum enumColumnDataHandling
    {
        /// <summary>
        /// Resorts the samples to the appropiate columns.
        /// </summary>
        Resort,

        /// <summary>
        /// Distributes samples across columns appropiately.
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
    public class classSampleQueue
    {
        #region Utility Methods

        /// <summary>
        /// Creates a new unique ID number not used and stores it.  This always generates the largest unique ID.
        /// </summary>
        /// <returns>Unique ID number.</returns>
        private long GenerateUniqueID()
        {
            // Get the largest unique ID in the set.
            var count = m_uniqueID.Count;
            long uniqueID = 1;
            if (count > 0)
                uniqueID = m_uniqueID[count - 1] + 1;
            m_uniqueID.Add(uniqueID);
            return uniqueID;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines if the sample is valid
        /// </summary>
        /// <param name="requestname"></param>
        /// <returns></returns>
        public enumSampleValidResult IsSampleDataValid(classSampleData sample)
        {
            var result = enumSampleValidResult.Valid;

            //
            // Determine if the sample has a duplicate request name.
            // If it has one match, then it should be itself.
            //
            var data = FindSample(sample.DmsData.DatasetName);
            if (data.Count > 1)
            {
                result = enumSampleValidResult.DuplicateRequestName;
                return result;
            }

            return result;
        }

        #endregion

        #region Delegate Definitions

        /// <summary>
        /// Delegate definition for when a sample is modified.
        /// </summary>
        /// <param name="sender">Sample Queue that made the call.</param>
        /// <param name="data">Data associated with the addition.</param>
        public delegate void DelegateSamplesModifiedHandler(object sender, classSampleQueueArgs data);

        /// <summary>
        /// Definition for when a sample is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void DelegateSampleStarted(object sender, classSampleQueueArgs data);

        #endregion

        #region Constants

        /// <summary>
        /// Defualt new sample name.
        /// </summary>
        private const string CONST_DEFAULT_SAMPLENAME = "blank";

        /// <summary>
        /// Name used when distributed samples across columns.
        /// </summary>
        private const string CONST_DEFAULT_INTEGRATE_SAMPLENAME = "(unused)";

        #endregion

        #region Members

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
        /// Name of samples that are added through distribution across columns.
        /// </summary>
        private readonly string m_integrateName;

        /// <summary>
        /// List of columns that are enabled or disabled.
        /// </summary>
        //private List<classColumnData> m_columnOrders;
        /// <summary>
        /// Queue of sample data to be run.  Index 0 should be the next sample to run.
        /// </summary>
        private List<classSampleData> m_waitingQueue;

        /// <summary>
        /// Queue of samples that have already ran.  Index 0 being the oldest run sample. Index N (size of queue
        /// being the most recently run sample).
        /// </summary>
        private List<classSampleData> m_completeQueue;

        /// <summary>
        /// List of samples that are currently running.
        /// </summary>
        private readonly List<classSampleData> m_runningQueue;

        /// <summary>
        /// List of unique ID's used in the sample queue.
        /// </summary>
        private readonly List<long> m_uniqueID;

        /// <summary>
        /// Flag indicating whether to re-set the column data when a queue operation is performed.
        /// </summary>
        private bool m_resetColumns;

        /// <summary>
        /// Default name of the sample to add when distributing across columns.
        /// </summary>
        private string m_defaultSampleName;

        /// <summary>
        /// Stack of waiting queues for undo operations.
        /// </summary>
        private readonly Stack<List<classSampleData>> mstack_undoBackWaitingQueue;

        /// <summary>
        /// Stack of samples for redo operations
        /// </summary>
        private readonly Stack<List<classSampleData>> mstack_undoForwardWaitingQueue;

        /// <summary>
        ///
        /// </summary>
        private List<classColumnData> m_columnOrders;

        /// <summary>
        /// Flag indicating whether the samples were started explicitly by the caller.
        /// If false the queries for next samples will return null indicating no samples can be started.
        /// If true, and samples finish or are cancelled (i.e. errors) then the flag will reset to false.
        /// </summary>
        private bool m_startedSamples;

        #endregion

        #region Delegated Events and Threading Events

        /// <summary>
        /// Fired when a sample is added to a queue.
        /// </summary>
        public event DelegateSamplesModifiedHandler SamplesAdded;

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
        /// Fired when a sample is reordered or randomized.
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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for a sample queue.
        /// </summary>
        public classSampleQueue()
        {
            m_completeQueue = new List<classSampleData>();
            m_waitingQueue = new List<classSampleData>();
            m_runningQueue = new List<classSampleData>();
            m_uniqueID = new List<long>();

            m_defaultSampleName = CONST_DEFAULT_SAMPLENAME;
            m_integrateName = CONST_DEFAULT_INTEGRATE_SAMPLENAME;
            m_resetColumns = true;
            m_sampleIndex = 1;
            m_sampleWaitingEvent = new AutoResetEvent(false);
            m_columnOrders = new List<classColumnData>();

            // Undo - redo operations
            mstack_undoBackWaitingQueue = new Stack<List<classSampleData>>();
            mstack_undoForwardWaitingQueue = new Stack<List<classSampleData>>();


            // Pointer to the next available sample that is queued for running.
            m_nextAvailableSample = 0;

            // Tracks what is the current sequence number that has been run previously.
            m_sequenceIndex = 1;


            UpdateColumnList();
            foreach (var column in classCartConfiguration.Columns)
            {
                column.FirstChanged += new classColumnData.DelegateFirstChanged(column_FirstChanged);
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
            m_columnOrders = classCartConfiguration.BuildColumnList(true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the next column a sample will be added on.
        /// </summary>
        public classColumnData NextColumnData
        {
            get
            {
                //
                // Make sure that we have enough columns to add data on.
                //
                if (m_columnOrders.Count < 1)
                    return null;


                var index = -1;

                //
                // Get the index of the last sample that is queued...
                //

                //
                // if the waiting queue is empty. that means no samples are queued
                // TODO:  Add to see what the last sample in the running or complete queue is
                //
                if (m_waitingQueue.Count > 0)
                {
                    var data = m_waitingQueue[m_waitingQueue.Count - 1];
                    index = m_columnOrders.IndexOf(data.ColumnData);
                }
                else
                {
                    if (m_runningQueue.Count > 0)
                    {
                        index = m_columnOrders.IndexOf(m_runningQueue[m_runningQueue.Count - 1].ColumnData);
                    }
                    else if (m_completeQueue.Count > 0)
                    {
                        index = m_columnOrders.IndexOf(m_completeQueue[m_completeQueue.Count - 1].ColumnData);
                    }
                }
                //
                // Then return the column data available at the end of queue
                //
                return m_columnOrders[(index + 1) % m_columnOrders.Count];
            }
        }

        /// <summary>
        /// Gets the name of the un-used sample.
        /// </summary>
        public string UnusedSampleName
        {
            get { return m_integrateName; }
        }

        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public List<classColumnData> ColumnOrder
        {
            get { return m_columnOrders; }
        }

        /// <summary>
        /// Gets or sets whether to reset the column data.
        /// </summary>
        public bool AutoColumnData
        {
            get { return m_resetColumns; }
            set { m_resetColumns = value; }
        }

        /// <summary>
        /// Gets or sets the default sample name.
        /// </summary>
        public string DefaultSampleName
        {
            get { return m_defaultSampleName; }
            set { m_defaultSampleName = value; }
        }

        /// <summary>
        /// Gets or sets the running sample index of samples that have been
        /// added to the queue.
        /// </summary>
        public int RunningSampleIndex
        {
            get { return m_sampleIndex; }
            set { m_sampleIndex = value; }
        }

        /// <summary>
        /// Gets the threading event when a sample is queued.
        /// </summary>
        public AutoResetEvent SampleQueuedEvent
        {
            get { return m_sampleWaitingEvent; }
        }

        #endregion

        #region Queue Searching

        /// <summary>
        /// Determines if the waiting sample queue has any unused samples.
        /// </summary>
        /// <returns></returns>
        public bool HasUnusedSamples()
        {
            //
            // Find the first sample that has an unused name
            //
            foreach (var sample in m_waitingQueue)
            {
                if (sample.DmsData.DatasetName == m_integrateName)
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
        public bool HasUnusedSamples(classColumnData column)
        {
            //
            // Find the first sample that has an unused name
            //
            foreach (var sample in m_waitingQueue)
            {
                if (sample.DmsData.DatasetName == m_integrateName && column.ID == sample.ColumnData.ID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the sample object based on its sample request name.
        /// </summary>
        /// <param name="sampleName">Name of sample to search for.</param>
        /// <returns>List of samples founds.</returns>
        public List<classSampleData> FindSample(string sampleName)
        {
            var samples = new List<classSampleData>();

            //
            // Search all of the queues and add any instance of a copy
            //
            lock (m_waitingQueue)
            {
                foreach (var data in m_waitingQueue)
                    if (data.DmsData.DatasetName == sampleName)
                        samples.Add(data);
            }
            lock (m_runningQueue)
            {
                foreach (var data in m_runningQueue)
                    if (data.DmsData.DatasetName == sampleName)
                        samples.Add(data);
            }
            lock (m_completeQueue)
            {
                foreach (var data in m_completeQueue)
                    if (data.DmsData.DatasetName == sampleName)
                        samples.Add(data);
            }

            return samples;
        }

        /// <summary>
        /// Finds the sample in any of the queues based on a unique ID.
        /// </summary>
        /// <param name="uniqueID">ID to search for in queue.</param>
        /// <returns>Single instance of a found sample.  Null if no sample
        /// contains the unique ID provided.</returns>
        public classSampleData FindSample(long uniqueID)
        {
            lock (m_completeQueue)
            {
                foreach (var sample in m_completeQueue)
                    if (sample.UniqueID == uniqueID)
                        return sample;
            }
            lock (m_runningQueue)
            {
                foreach (var sample in m_runningQueue)
                    if (sample.UniqueID == uniqueID)
                        return sample;
            }
            lock (m_waitingQueue)
            {
                foreach (var sample in m_waitingQueue)
                    if (sample.UniqueID == uniqueID)
                        return sample;
            }
            return null;
        }

        /// <summary>
        /// Finds the sample in any of the queues based on a unique ID.
        /// </summary>
        /// <param name="uniqueID">ID to search for in queue.</param>
        /// <returns>Single instance of a found sample.  Null if no sample
        /// contains the unique ID provided.</returns>
        public classSampleData FindSample(List<classSampleData> queue, long uniqueID)
        {
            foreach (var sample in queue)
                if (sample.UniqueID == uniqueID)
                    return sample;
            return null;
        }

        /// <summary>
        /// Finds the list of unused samples.
        /// </summary>
        /// <param name="queue">Queue to search on.</param>
        /// <returns>List of samples whose request names are unused.</returns>
        private List<classSampleData> FindUnusedSamples(List<classSampleData> queue)
        {
            var unusedSamples = new List<classSampleData>();
            foreach (var sample in queue)
            {
                if (sample.DmsData.DatasetName == m_integrateName)
                {
                    unusedSamples.Add(sample);
                }
            }
            return unusedSamples;
        }

        /// <summary>
        /// Finds the list of unused samples.
        /// </summary>
        /// <param name="queue">Queue to search on.</param>
        /// <param name="column">Column to search on.</param>
        /// <returns>List of samples whose request names are unused.</returns>
        private List<classSampleData> FindUnusedSamples(List<classSampleData> queue, classColumnData column)
        {
            var unusedSamples = new List<classSampleData>();
            foreach (var sample in queue)
            {
                if (sample.DmsData.DatasetName == m_integrateName && sample.ColumnData == column)
                {
                    unusedSamples.Add(sample);
                }
            }
            return unusedSamples;
        }

        /// <summary>
        /// Retrives the next sample to run.
        /// </summary>
        /// <returns>A reference to the next sample to run.
        /// Null if there are no samples to run.</returns>
        public classSampleData GetNextSample()
        {
            classSampleData data = null;

            if (m_waitingQueue.Count > 0)
            {
                //
                // Remove from the run queue
                //
                data = m_waitingQueue[0];
                m_waitingQueue.RemoveAt(0);

                //
                // Put the data on the queue.
                //
                m_runningQueue.Add(data);
            }
            return data;
        }

        #endregion

        #region Non-Queue Specific Operation Methods

        /// <summary>
        /// Builds a histogram keyed on column that contains a list of samples.
        /// </summary>
        /// <param name="queue">Queue to build histogram from</param>
        /// <returns>Histogram.  Empty if no samples exist.</returns>
        private Dictionary<classColumnData, List<classSampleData>> BuildSampleHistogram(List<classSampleData> queue)
        {
            //
            // Create a queue histogram.
            //
            var sampleHistogram =
                new Dictionary<classColumnData, List<classSampleData>>();
            foreach (var col in m_columnOrders)
            {
                sampleHistogram.Add(col, new List<classSampleData>());
            }
            //
            // Calculate the Histogram
            //
            for (var i = 0; i < queue.Count; i++)
            {
                var data = queue[i];
                var col = data.ColumnData;
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
        /// <param name="column">Column the data was added on.</param>
        /// <param name="addSamples"></param>
        private void DistributeSamplesAcrossColumns(Dictionary<classColumnData, List<classSampleData>> histogram,
            List<classSampleData> queue)
        {
            //
            // Figure out how many items we have on this column.
            //
            var maxCount = -1;
            foreach (var data in histogram.Values)
            {
                maxCount = Math.Max(data.Count, maxCount);
            }

            //
            // Now we have a histogram of columns whose entries are a List of samples
            // For every column whose List Count is less than the maximum, add unused samples to it
            // At this point the waiting list does not mean anything.  We will re-construct
            // the waiting queue after this point.
            //
            foreach (var col in histogram.Keys)
            {
                var data = histogram[col];
                while (data.Count < maxCount)
                {
                    classSampleData sample;
                    if (queue.Count > 0)
                    {
                        sample = queue[queue.Count - 1].Clone() as classSampleData;
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
                    }
                    else
                    {
                        sample = new classSampleData();
                    }

                    //
                    // We make the request an "Unused sample"
                    //
                    sample.DmsData.RequestName = m_integrateName;
                    sample.DmsData.DatasetName = m_integrateName;
                    sample.DmsData.Block = 0; // It's an unused sample. so dont copy this information.
                    sample.DmsData.Batch = 0;
                    sample.ColumnData = col;
                    sample.UniqueID = GenerateUniqueID();
                    data.Add(sample);
                }
            }
            queue.Clear();
            //
            // Here we reconstruct the waiting queue.  We use the column orders list (tells us
            // what column is the first one to use) to add samples in order.
            //
            var firstList = histogram[m_columnOrders[0]];
            for (var i = 0; i < firstList.Count; i++)
            {
                var firstSample = firstList[i];
                queue.Add(firstSample);

                //
                // Now add each sample found on another column
                //
                for (var j = 1; j < m_columnOrders.Count; j++)
                {
                    var col = m_columnOrders[j];
                    var sample = histogram[col][i];
                    queue.Add(sample);
                }
            }
        }

        /// <summary>
        /// Removes excess samples (names who are just placeholders. From the ends of the queue.
        /// </summary>
        /// <param name="queue">Queue to remove excess samples from.</param>
        private void RemoveExcessSamples(List<classSampleData> queue)
        {
            //
            // Remove excess items from the end of the list.
            //
            while (queue.Count > 0 && queue[queue.Count - 1].DmsData.DatasetName == m_integrateName)
                queue.RemoveAt(queue.Count - 1);
        }

        /// <summary>
        /// Re-sequences the waiting queue starting from the smallest sequence number.  It always starts the sequence number from 1
        /// </summary>
        public void ResequenceQueuedSamples(List<classSampleData> queue)
        {
            if (queue.Count > 0)
            {
                long sequence = 0;
                foreach (var sample in m_completeQueue)
                {
                    sequence = Math.Max(sample.SequenceID, sequence);
                }
                foreach (var sample in m_runningQueue)
                {
                    sequence = Math.Max(sample.SequenceID, sequence);
                }
                sequence++;

                m_sequenceIndex = Convert.ToInt32(sequence);

                ResequenceQueuedSamples(queue, sequence);
            }
        }

        /// <summary>
        /// Resequences the samples in the queued samples.
        /// </summary>
        /// <param name="start">Start offset to resequence from.</param>
        private void ResequenceQueuedSamples(List<classSampleData> queue, long startSequence)
        {
            foreach (var data in queue)
            {
                data.SequenceID = startSequence++;
            }
            m_sequenceIndex = Convert.ToInt32(startSequence);
        }

        #endregion

        #region Undo

        /// <summary>
        /// Pushes the new queue onto the stack for undo operations.
        /// </summary>
        /// <param name="stack">Stack to push queue onto.</param>
        /// <param name="queue">Queue to push onto stack.</param>
        private void PushQueue(Stack<List<classSampleData>> backStack, Stack<List<classSampleData>> forwardStack,
            List<classSampleData> queue)
        {
            PushQueue(backStack, forwardStack, queue, true);
        }

        /// <summary>
        /// Pushes the queue onto the backstack and the
        /// </summary>
        /// <param name="backStack"></param>
        /// <param name="queue"></param>
        /// <param name="clearForward"></param>
        private void PushQueue(Stack<List<classSampleData>> backStack,
            Stack<List<classSampleData>> forwardStack,
            List<classSampleData> queue,
            bool clearForward)
        {
            //
            // If the user wants us to clear the forward stack then we will,
            // otherwise we ignore it.
            //
            if (clearForward == true && forwardStack != null)
                forwardStack.Clear();


            var pushQueue = new List<classSampleData>();
            foreach (var data in queue)
            {
                var sample = data.Clone() as classSampleData;
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
                pushQueue.Add(sample);
            }
            backStack.Push(pushQueue);
            IsDirty = true;
        }

        public bool IsDirty { get; set; }

        /// <summary>
        /// Pops the queue from the stack if available
        /// </summary>
        /// <param name="backStack">Stack to pop queue from.</param>
        /// <param name="forwardStack">In case the user wants to "redo".  If null no forward operation will be handled.</param>
        /// <param name="queue">Queue to push most recent operation into.</param>
        /// <returns>A new queue if it can be popped.  Otherwise null if the back stack is empty.</returns>
        private List<classSampleData> PopQueue(Stack<List<classSampleData>> backStack)
        {
            List<classSampleData> newQueue = null;


            if (backStack.Count < 1)
                return newQueue;

            IsDirty = true;
            newQueue = backStack.Pop();

            return newQueue;
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        public void Undo()
        {
            //
            // Pop the next item
            //
            var queue = PopQueue(mstack_undoBackWaitingQueue);

            // Then if popping
            if (queue != null && queue.Count > 0)
            {
                //
                // Save the current waiting queue onto the forward stack, thus saving it for a redo
                //
                PushQueue(mstack_undoForwardWaitingQueue, null, m_waitingQueue);

                // Transfer the new queue to our waiting queue.
                m_waitingQueue.Clear();
                m_waitingQueue = queue;
                //ResetColumnData();

                if (SamplesAdded != null)
                {
                    SamplesAdded(this, new classSampleQueueArgs(GetAllSamples()));
                }
            }
        }

        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        public void Redo()
        {
            //
            // Pull the queue off the forward stack if one exists
            //
            var queue = PopQueue(mstack_undoForwardWaitingQueue);

            if (queue != null && queue.Count > 0)
            {
                //
                // Push the current queue onto the back stack, thus saving our waiting queue.
                //
                PushQueue(mstack_undoBackWaitingQueue, null, m_waitingQueue);

                m_waitingQueue.Clear();
                m_waitingQueue = queue;
                //ResetColumnData();

                if (SamplesAdded != null)
                {
                    SamplesAdded(this, new classSampleQueueArgs(GetAllSamples()));
                }
            }
        }

        #endregion

        #region Adding, Removing, Updating Samples from Queue

        /// <summary>
        /// Inserts samples into the
        /// </summary>
        /// <param name="samples"></param>
        public void InsertIntoUnusedSamples(List<classSampleData> samples, enumColumnDataHandling handling)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);
            //
            // Overwrite all of the samples that are unused first.
            //
            // Then add to the queue if the number of unused samples are less
            // than the number of provided ones.
            //
            var i = 0;
            while (i < m_waitingQueue.Count && samples.Count > 0)
            {
                var unusedSample = m_waitingQueue[i];
                if (unusedSample.DmsData.DatasetName.Contains(m_integrateName))
                {
                    var sample = samples[0];
                    samples.RemoveAt(0);
                    m_waitingQueue[i] = sample;
                    m_waitingQueue[i].UniqueID = GenerateUniqueID();
                }
                i++;
            }

            //
            // ask if there are any leftovers
            //
            if (samples.Count > 0)
            {
                //
                // Dont need to re-sequence or notify
                // because queue samples does it for us.
                //
                QueueSamples(samples, handling);
            }
            else
            {
                //
                // Always re-sequence, and notify
                //
                ResequenceQueuedSamples(m_waitingQueue);

                if (handling == enumColumnDataHandling.Resort)
                {
                    ResetColumnData();
                }

                if (SamplesAdded != null)
                {
                    SamplesAdded(this, new classSampleQueueArgs(GetAllSamples()));
                }
            }
        }

        /// <summary>
        /// Inserts samples into the
        /// </summary>
        /// <param name="samples"></param>
        public void InsertIntoUnusedSamples(List<classSampleData> samples, classColumnData column,
            enumColumnDataHandling handling)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // Overwrite all of the samples that are unused first.
            //
            // Then add to the queue if the number of unused samples are less
            // than the number of provided ones.
            //
            var i = 0;
            while (i < m_waitingQueue.Count && samples.Count > 0)
            {
                var unusedSample = m_waitingQueue[i];
                if (unusedSample.DmsData.DatasetName.Contains(m_integrateName) &&
                    unusedSample.ColumnData == column)
                {
                    var sample = samples[0];
                    samples.RemoveAt(0);
                    m_waitingQueue[i] = sample;
                    m_waitingQueue[i].UniqueID = GenerateUniqueID();
                }
                i++;
            }

            //
            // ask if there are any leftovers
            //
            if (samples.Count > 0)
            {
                //
                // Don't need to re-sequence or notify
                // because queue samples does it for us.
                //
                QueueSamples(samples, handling);
            }
            else
            {
                //
                // Always re-sequence, and notify
                //
                ResequenceQueuedSamples(m_waitingQueue);
                if (SamplesAdded != null)
                {
                    SamplesAdded(this, new classSampleQueueArgs(GetAllSamples()));
                }
            }
        }

        /// <summary>
        /// Moves the samples provided in the list by offset in the waiting queue.
        /// Order of input samples should be current order in queue.
        /// </summary>
        /// <param name="samples">Samples to move.</param>
        /// <param name="offset">Number of samples to move by.</param>
        /// <param name="offset">How to move the samples.</param>
        public void MoveQueuedSamples(List<classSampleData> samples, int baseOffset, int offset,
            enumMoveSampleType moveType)
        {
            SwapQueuedSamplesColumn(m_waitingQueue, samples, baseOffset, offset, enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Adds a sample to the list of waiting samples.
        /// </summary>
        /// <param name="sampleList">List of samples to add.</param>
        /// <param name="forceColumns">Tells the queue operation whether to distribute samples on other columnds</param>
        /// <returns>True if addition was a success.  False if addition failed.</returns>
        public bool QueueSamples(IEnumerable<classSampleData> sampleList,
            enumColumnDataHandling distributeSamplesEvenlyAcrossColumns)
        {
            var added = false;

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // Here we add to the waiting queue.
            //     1. Distribute the samples across the waiting queue using a histogram
            //         to figure out the distribution of samples.  This version will
            //         add unused samples that never get run but help balance the
            //         queue building.
            //     2. Add the samples directly back to the waiting queue - append.
            //         This just puts samples on the queue.
            //

            if (distributeSamplesEvenlyAcrossColumns == enumColumnDataHandling.CreateUnused)
            {
                var tempQueue = new List<classSampleData>();
                var sampleHistogram =
                    BuildSampleHistogram(m_waitingQueue);
                //
                // Add samples to their respective columns.
                //
                foreach (var sample in sampleList)
                {
                    //
                    // Check for object references.
                    // We allow for the same request name to be had in the
                    // sample list.
                    //
                    if (m_waitingQueue.Contains(sample) == false)
                    {
                        added = true;
                        sample.UniqueID = GenerateUniqueID();
                        sampleHistogram[sample.ColumnData].Add(sample);
                        tempQueue.Add(sample);
                    }
                }
                DistributeSamplesAcrossColumns(sampleHistogram, m_waitingQueue);
            }
            else
            {
                foreach (var data in sampleList)
                {
                    data.UniqueID = GenerateUniqueID();
                    m_waitingQueue.Add(data);
                    added = true;
                }
            }

            //
            // Now make sure we alert everyone that we added a sample
            //
            if (added == true)
            {
                //
                // Only re-sequence and remove if we changed the queue to not waste time
                // doing something if we dont have to.
                //
                RemoveExcessSamples(m_waitingQueue);
                ResequenceQueuedSamples(m_waitingQueue);

                if (SamplesAdded != null)
                {
                    var args = new classSampleQueueArgs(GetAllSamples());
                    SamplesAdded(this, args);
                }
            }
            return added;
        }

        /// <summary>
        /// Updates all listeners that the sample data has been updated.
        /// </summary>
        public void UpdateAllSamples()
        {
            if (SamplesAdded != null)
            {
                var args = new classSampleQueueArgs(GetAllSamples());
                SamplesAdded(this, args);
            }
        }

        /// <summary>
        /// Updates all listeners that the sample data has been updated.
        /// </summary>
        public void UpdateWaitingSamples()
        {
            if (SamplesAdded != null)
            {
                var args = new classSampleQueueArgs(GetWaitingQueue());
                SamplesAdded(this, args);
            }
        }

        /// <summary>
        /// Compiles all of the samples from the sample queue.
        /// </summary>
        /// <returns>List of all run, running, and waiting samples.</returns>
        private List<classSampleData> GetAllSamples()
        {
            var allSamples = new List<classSampleData>();
            allSamples.AddRange(m_completeQueue);
            allSamples.AddRange(m_runningQueue);
            allSamples.AddRange(m_waitingQueue);
            return allSamples;
        }

        /// <summary>
        /// Gets a list of all the samples in the waiting queue
        /// </summary>
        /// <returns>List of samples in waiting queue</returns>
        public List<classSampleData> GetWaitingQueue()
        {
            var waitingQueue = new List<classSampleData>();
            waitingQueue.AddRange(m_waitingQueue);
            return waitingQueue;
        }

        /// <summary>
        /// Gets a list of all the samples in the waiting queue
        /// </summary>
        /// <returns>List of samples in waiting queue</returns>
        public List<classSampleData> GetRunningQueue()
        {
            var runningQueue = new List<classSampleData>();
            runningQueue.AddRange(m_runningQueue);
            return runningQueue;
        }

        /// <summary>
        /// Class that adds a sample to the list of managed samples.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        private bool RemoveSample(classSampleData sample)
        {
            var removed = false;
            if (m_waitingQueue.Contains(sample) == true)
            {
                m_waitingQueue.Remove(sample);
                removed = true;
            }
            return removed;
        }

        /// <summary>
        /// Remove each id from the list of potential samples.
        /// </summary>
        /// <param name="uniqueIDs">Unique ID's found on the samples.</param>
        private bool RemoveSample(List<classSampleData> queue, List<long> uniqueIDs)
        {
            var removed = false;
            //
            // Find each sample if it exists,
            //
            foreach (var id in uniqueIDs)
            {
                var sample = FindSample(queue, id);
                if (sample != null)
                {
                    queue.Remove(sample);
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Removes samples from the waiting queue found in the list of unique ids.
        /// </summary>
        /// <param name="uniqueIDs">List of unique id's.</param>
        /// <returns>True if removed, false if not.</returns>
        public bool RemoveSample(List<long> uniqueIDs, enumColumnDataHandling resortColumns)
        {
            var removed = false;
            //
            // Remove the sample from the complete queue.
            //
            removed = RemoveSample(m_completeQueue, uniqueIDs);

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // First we figure out what samples we need to remove based on their
            // Unique ID's.  Then we remove them.
            //
            var removedSamples = new List<classSampleData>();
            foreach (var i in uniqueIDs)
            {
                var data = FindSample(i);
                if (data != null)
                {
                    //
                    // Check for duplicate object references
                    // Also, if we are to distribute across columns that means we remove it
                    // from the queue.  Then we allow the program to reseat the samples on new
                    // columns.
                    //
                    // Otherwise, we just make the sample name unused.
                    //
                    if (m_waitingQueue.Contains(data) == true)
                    {
                        m_waitingQueue.Remove(data);
                        removedSamples.Add(data);
                        removed = true;
                    }
                }
            }


            //
            // If we removed then we want to tell someone what samples were removed.
            // Later we re-sequence them.
            //
            // Also, make sure we reseat samples on new columns.  We do this because
            // removed can only be true if we removed some samples instead of renaming.
            //
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
                        //
                        // Then, if some were removed we build up the distribution list and fill back into the queue
                        // allowing for the correct spacing on other columns, but inserting unused samples at the end of this column list
                        // so we dont disturb other column Data.  We do this using the same mechanism that adding samples uses
                        // but here we are not adding new samples to the queue.
                        //
                        var histogram =
                            BuildSampleHistogram(m_waitingQueue);
                        DistributeSamplesAcrossColumns(histogram, m_waitingQueue);
                        RemoveExcessSamples(m_waitingQueue);
                        break;
                }

                ResequenceQueuedSamples(m_waitingQueue);

                if (SamplesRemoved != null)
                {
                    SamplesRemoved(this, new classSampleQueueArgs(GetAllSamples()));
                }

                if (SamplesUpdated != null)
                    SamplesUpdated(this, new classSampleQueueArgs(m_waitingQueue));
            }
            return removed;
        }

        /// <summary>
        /// Remove all of the samples that are in the queue that are unused.  Columns will
        /// be resorted since no column data was specified.
        /// </summary>
        /// <returns>True if any samples were removed.  False if not.</returns>
        public bool RemoveUnusedSamples(enumColumnDataHandling resortColumns)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // Find the unique id's of all the samples that are unused.
            //
            var uniqueList = new List<long>();
            foreach (var sample in m_waitingQueue)
            {
                if (sample.DmsData.DatasetName == m_integrateName)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            //
            // Removes the samples from this list, AND! resorts the columns.
            // This is important for this method because it means that
            // we dont care about preserving column information.
            //
            if (uniqueList.Count > 0)
            {
                return RemoveSample(uniqueList, resortColumns);
            }
            return false;
        }

        /// <summary>
        /// Removes unused samples on the given column.
        /// </summary>
        /// <param name="column">Column to remove samples from.</param>
        /// <returns>True if any samples were removed.  False if not.</returns>
        public bool RemoveUnusedSamples(classColumnData column, enumColumnDataHandling resortColumns)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // Find the unique id's of all the samples that are unused.
            //
            var uniqueList = new List<long>();
            foreach (var sample in m_waitingQueue)
            {
                if (sample.DmsData.DatasetName == m_integrateName && sample.ColumnData == column)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            //
            // Removes the samples from this list, AND DOES NOT! Resort the columns.
            // This is important for this method because it means that
            // we DO care about preserving column information.
            //
            if (uniqueList.Count > 0)
            {
                return RemoveSample(uniqueList, resortColumns);
            }
            return false;
        }

        /// <summary>
        /// Reorders the samples provided as the argument by inserting the items in the queue.  Re-orders in place.
        /// </summary>
        /// <param name="newOrders">List of samples that contain the new ordering.</param>
        public void ReorderSamples(List<classSampleData> newOrders, enumColumnDataHandling handling)
        {
            if (newOrders.Count < 1)
                return;

            if (m_waitingQueue.Count < 1)
                return;

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            var baseOffset = m_waitingQueue[0].SequenceID;
            foreach (var sample in newOrders)
            {
                var newIndex = sample.SequenceID - baseOffset;
                if (m_waitingQueue.Count > newIndex)
                {
                    m_waitingQueue[Convert.ToInt32(newIndex)] = sample;
                }
            }

            if (handling == enumColumnDataHandling.Resort)
            {
                ResetColumnData();
            }

            //TODO: Re-order by only sending the ones that changed.
            if (SamplesReordered != null)
            {
                SamplesReordered(this, new classSampleQueueArgs(GetAllSamples()));
            }
        }

        /// <summary>
        /// Resets the column data after a move, delete operation.
        /// </summary>
        public void ResetColumnData(bool updateUsers)
        {
            ResetColumnData();
            if (SamplesUpdated != null)
            {
                SamplesUpdated(this, new classSampleQueueArgs(m_waitingQueue));
            }
        }

        /// <summary>
        /// Resets the column data after a move, delete operation.
        /// </summary>
        public void ResetColumnData()
        {
            var index = 0;

            if (m_runningQueue.Count > 0)
            {
                index = m_columnOrders.IndexOf(m_runningQueue[m_runningQueue.Count - 1].ColumnData) + 1;
            }
            else if (m_completeQueue.Count > 0)
            {
                index = m_columnOrders.IndexOf(m_completeQueue[m_completeQueue.Count - 1].ColumnData) + 1;
            }

            foreach (var data in m_waitingQueue)
            {
                var columnData = m_columnOrders[index % m_columnOrders.Count];
                data.ColumnData = columnData;
                index++;
            }
        }

        /// <summary>
        /// Moves samples specified by the offset, swapping i with j instead of moving up and down.
        /// </summary>
        /// <param name="samples">List of samples to swap.</param>
        /// <param name="baseOffset">Base offset to swap by.</param>
        /// <param name="offset">Spacing between columns.</param>
        private void SwapQueuedSamplesColumn(List<classSampleData> queue,
            List<classSampleData> samples,
            int baseOffset,
            int offset,
            enumColumnDataHandling handling)
        {
            if (samples.Count < 1)
                return;

            var firstSequenceNumber = m_waitingQueue[0].SequenceID;
            var swapped = false;

            //
            // If the offset is positive then move the samples down in the list - towards end of queue
            //
            if (offset > 0)
            {
                var lastIndex = queue.Count - 1;
                for (var i = samples.Count - 1; i >= 0; i--)
                {
                    //
                    // If the first guy could not be swapped, then we dont want the other items
                    // to be swapped either.  So continue if done so.
                    //
                    if (i < samples.Count - 1 && swapped == false)
                        continue;

                    // find the location of the sample to swap in the waiting queue.
                    var index = queue.IndexOf(samples[i]);

                    //
                    // Calculate its new location to swap if the offset is one,
                    // otherwise we dont want to swap by the last index because this would
                    // move items to another column.
                    //
                    var newIndex = Math.Min(index + offset, lastIndex);
                    if (Math.Abs(offset) > 1 && index + offset > lastIndex)
                        newIndex = index;

                    if (newIndex != index)
                    {
                        var tempSample = queue[newIndex];
                        queue[newIndex] = m_waitingQueue[index];
                        queue[index] = tempSample;
                        lastIndex = newIndex - offset;
                        swapped = true;
                    }
                    else
                    {
                        lastIndex = index - offset;
                    }
                }
            }
            // Otherwise move the samples up in the queue order.
            else if (offset < 0)
            {
                var lastIndex = baseOffset;
                for (var i = 0; i < samples.Count; i++)
                {
                    //
                    // If the first guy could not be swapped, then we dont want the other items
                    // to be swapped either.  So continue if done so.
                    //
                    if (i > 0 && swapped == false)
                        continue;

                    // find the location of the sample to swap in the waiting queue.
                    var index = queue.IndexOf(samples[i]);

                    //
                    // Calculate its new location to swap if the offset is one,
                    // otherwise we dont want to swap by the last index because this would
                    // move items to another column.
                    //
                    var newIndex = Math.Max(index + offset, lastIndex);
                    if (Math.Abs(offset) > 1 && index + offset < lastIndex)
                        newIndex = index;


                    if (newIndex != index)
                    {
                        // Swap
                        var tempSample = queue[newIndex];
                        queue[newIndex] = queue[index];
                        queue[index] = tempSample;
                        lastIndex = newIndex - offset;
                        swapped = true;
                    }
                    else
                    {
                        lastIndex = index - offset;
                    }
                }
            }
            //
            // Update the sequence numbers
            //
            ResequenceQueuedSamples(queue, firstSequenceNumber);

            //
            // Update the column data
            //
            //if (handling == enumColumnDataHandling.Resort)
            //{
            //    ResetColumnData();
            //}

            //
            // Tell listeners that we have re-sequenced the queue.
            //
            if (SamplesReordered != null)
            {
                SamplesReordered(this, new classSampleQueueArgs(GetAllSamples()));
            }
        }

        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="data"></param>
        public void UpdateSample(classSampleData data)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            //
            // Find the sample
            //
            var index = m_waitingQueue.IndexOf(data);
            if (index >= 0)
            {
                //
                // Update...although the reference should be updated.
                //
                m_waitingQueue[index] = data;

                //
                // Alert listening objects.
                //
                if (SamplesUpdated != null)
                {
                    SamplesUpdated(this, new classSampleQueueArgs(m_waitingQueue));
                }
            }
        }

        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="data"></param>
        public void UpdateSamples(List<classSampleData> samples)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, m_waitingQueue);

            var updated = false;
            foreach (var sample in samples)
            {
                var existingSample = FindSample(m_waitingQueue, sample.UniqueID);
                if (existingSample != null)
                {
                    var index = m_waitingQueue.IndexOf(existingSample);
                    if (index >= 0)
                    {
                        updated = true;
                        //if (sample.ColumnData == null)
                        //{
                        //    sample.ColumnData = existingSample.ColumnData;
                        //}
                        m_waitingQueue[index] = sample;
                    }
                }
            }
            //
            // Alert listening objects.
            //
            if (updated == true && SamplesUpdated != null)
            {
                SamplesUpdated(this, new classSampleQueueArgs(m_waitingQueue));
            }
        }

        #endregion

        #region Notification Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        public void AppendNotification(classLCMethod method)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        public void RunNext(classSampleData sample)
        {
            var i = 0;
            while (i < m_runningQueue.Count &&
                   m_runningQueue[i].RunningStatus == enumSampleRunningStatus.Running)
            {
                i++;
            }

            if (m_runningQueue.Count == i)
            {
                m_runningQueue.Insert(i, sample);
            }
            else
            {
                m_runningQueue.Add(sample);
            }


            UpdateAllSamples();

            if (!IsRunning)
            {
                StartSamples();
            }
        }

        #endregion

        #region Running Samples and Queue Operation

        /// <summary>
        /// Gets a value indicating if samples are read to be run.
        /// </summary>
        public bool AreSamplesAvailableToRun
        {
            get { return m_runningQueue.Count > 0; }
        }

        /// <summary>
        /// Gets whether there are samples currently set with running status.
        /// </summary>
        public bool IsRunning
        {
            get { return m_nextAvailableSample > 0; }
        }

        /// <summary>
        /// Starts the samples
        /// </summary>
        public void StartSamples()
        {
            // Don't run if nothing is there to run.
            if (m_runningQueue.Count == 0)
            {
                return;
            }

            if (m_nextAvailableSample > 0)
            {
                return;
            }

            // Setup the queue to be optimized.
            var validSamples = new List<classSampleData>();
            DateTime next;
            foreach (var sample in m_runningQueue)
            {
                //DateTime next       = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).Add(new TimeSpan(0, 0, 10));
                next = LcmsNetSDK.TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10));
                var containsMethod = classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name);
                if (containsMethod)
                {
                    sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                }

                sample.LCMethod = sample.LCMethod.Clone() as classLCMethod;
                validSamples.Add(sample);
                sample.LCMethod.SetStartTime(next);
                // We need to look for Daylight Savings Time Transitions and adjust for them here.
                if (LcmsNetSDK.TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(sample.LCMethod.Start,
                    sample.LCMethod.End))
                {
                    LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(
                        LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                        "QUEUE: some samples have been moved forward 1 hour due to a Daylight Savings Transition, this will avoid odd behavior while running the queue.");
                    next.Add(new TimeSpan(1, 0, 0));
                    sample.LCMethod.SetStartTime(next);
                }
            }

            // We have not started to run so optimize this way.
            var optimizer = new classLCMethodOptimizer();
            System.Diagnostics.Debug.WriteLine("Optimizing samples that are queued to run before starting the queue");
            optimizer.AlignSamples(m_runningQueue);

            // Set the listening event so that time sensitive items will know that
            // a sample is waiting on the running queue.
            m_sampleWaitingEvent.Set();
            m_startedSamples = true;
        }

        /// <summary>
        /// Puts the sample on the running queue and starts if the operator has appened this sample to an
        /// already running queue.
        /// </summary>
        /// <param name="sample">Sample to run.</param>
        public void MoveSamplesToRunningQueue(classSampleData sample)
        {
            MoveSamplesToRunningQueue(new List<classSampleData>() {sample});
        }

        /// <summary>
        /// Tells the scheduler to run these samples, putting them on the waiting (running) queue.
        /// </summary>
        /// <param name="samples"></param>
        public void MoveSamplesToRunningQueue(List<classSampleData> samples)
        {
            // For each sample to run, set the status and delay the run for 10 seconds
            var validSamples = new List<classSampleData>();

            var optimizer = new classLCMethodOptimizer();
            foreach (var sample in samples)
            {
                //DateTime next = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).Add(new TimeSpan(0, 0, 10));
                var next = LcmsNetSDK.TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10));
                var realSample = sample;
                if (sample.IsDummySample)
                {
                    realSample = FindSample(sample.UniqueID);
                }
                if (realSample == null)
                {
                    //
                    // this sample does not exist on the sample queue!
                    //
                    throw new NullReferenceException("This sample does not exist in the waiting queue! " +
                                                     sample.DmsData.DatasetName);
                }
                m_waitingQueue.Remove(realSample);

                var containsMethod = classLCMethodManager.Manager.Methods.ContainsKey(realSample.LCMethod.Name);
                if (containsMethod)
                {
                    realSample.LCMethod = classLCMethodManager.Manager.Methods[realSample.LCMethod.Name];
                }
                realSample.LCMethod = realSample.LCMethod.Clone() as classLCMethod;
                validSamples.Add(realSample);

                realSample.RunningStatus = enumSampleRunningStatus.WaitingToRun;

                if (m_runningQueue.Count > 0 && m_nextAvailableSample > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Optimizing sample against running queue.");
                    // We arent the first ones on the queue, but we are running,
                    // so we need to hurry up and go!
                    realSample.LCMethod.SetStartTime(next);
                    optimizer.AlignSamples(m_runningQueue, realSample);
                }
                else if (m_runningQueue.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Setting sample start time as it is first in running queue.");
                    // Otherwise we are the first ones on the queue, but we dont need to do anything
                    // for alignment.
                    realSample.LCMethod.SetStartTime(next);
                }
                m_runningQueue.Add(realSample);
            }

            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this,
                    new classSampleQueueArgs(validSamples,
                        m_nextAvailableSample,
                        m_runningQueue.Count,
                        m_completeQueue.Count,
                        m_waitingQueue.Count));
            }
        }

        /// <summary>
        /// Dequeues the samples from the running queue back onto the waiting queue.
        /// </summary>
        /// <param name="sample">Sample to dequeue.</param>
        public void DequeueSampleFromRunningQueue(classSampleData sample)
        {
            DequeueSampleFromRunningQueue(new List<classSampleData>() {sample});
        }

        /// <summary>
        /// Moves a sample from the running queue back onto the waiting queue.
        /// </summary>
        /// <param name="samples">Samples to put back on the waiting queue.</param>
        public void DequeueSampleFromRunningQueue(List<classSampleData> samples)
        {
            //
            // For each sample to run, set the status and delay the run for 10 seconds
            //
            var validSamples = new List<classSampleData>();
            foreach (var sample in samples)
            {
                var realSample = sample;
                if (sample.IsDummySample)
                {
                    realSample = FindSample(sample.UniqueID);
                }
                if (realSample == null)
                {
                    //
                    // this sample does not exist on the sample queue!
                    //
                    throw new NullReferenceException("This sample does not exist in the waiting queue! " +
                                                     sample.DmsData.DatasetName);
                }
                m_runningQueue.Remove(realSample);
                m_waitingQueue.Insert(0, realSample);

                realSample.LCMethod = realSample.LCMethod.Clone() as classLCMethod;
                realSample.RunningStatus = enumSampleRunningStatus.Queued;
                validSamples.Add(realSample);
            }

            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this,
                    new classSampleQueueArgs(validSamples,
                        m_nextAvailableSample,
                        m_runningQueue.Count,
                        m_completeQueue.Count,
                        m_waitingQueue.Count));
            }
        }

        /// <summary>
        /// Cancel the run provided and appends to the waiting queue.
        /// </summary>
        /// <param name="data">Sample to cancel.</param>
        public void CancelRunningSample(classSampleData sampleData, bool error)
        {
            if (sampleData == null)
                throw new NullReferenceException("The sample provided to cancel was null.");

            //
            // Find the sample provided to cancel.
            //
            var sample = FindSample(m_runningQueue, sampleData.UniqueID);
            if (sample == null)
            {
                var errorMessage = string.Format("The sample {0} was not found on the running Queue.",
                    sampleData);
                //throw new classSampleNotRunningException(errorMessage);
                return;
            }

            //
            // Remove the sample from the running queue
            //
            lock (m_runningQueue)
            {
                m_runningQueue.Remove(sample);
            }
            //enumSampleRunningStatus status = enumSampleRunningStatus.Stopped;
            //if (error)
            //{
            //    status = enumSampleRunningStatus.Error;
            //}
            sample.RunningStatus = enumSampleRunningStatus.Stopped;
            m_nextAvailableSample = Math.Max(m_nextAvailableSample - 1, 0);

            //
            // Requeue the sample putting it back on the queue it came from.
            //
            lock (m_completeQueue)
            {
                m_completeQueue.Add(sample);
            }

            var samples = new classSampleData[] {sample};
            var args = new classSampleQueueArgs(samples,
                m_nextAvailableSample,
                m_runningQueue.Count,
                m_completeQueue.Count,
                m_waitingQueue.Count);

            if ((m_nextAvailableSample == 0) && (m_runningQueue.Count == 0))
            {
                m_startedSamples = false;
            }

            if (SamplesCancelled != null)
            {
                SamplesCancelled(this, args);
            }
            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this, args);
            }
        }

        /// <summary>
        /// Stops all of the queue jobs and puts anything on the running queue back into the regular queue to be run later.
        /// </summary>
        public void StopRunningQueue()
        {
            foreach (var sample in m_runningQueue)
                sample.RunningStatus = enumSampleRunningStatus.Queued;

            lock (m_waitingQueue)
            {
                m_waitingQueue.InsertRange(0, m_runningQueue);
            }
            lock (m_runningQueue)
            {
                m_runningQueue.Clear();
            }
            m_nextAvailableSample = 0;

            var args = new classSampleQueueArgs(m_waitingQueue,
                m_nextAvailableSample,
                m_runningQueue.Count,
                m_completeQueue.Count,
                m_waitingQueue.Count);
            m_startedSamples = false;
            if (SamplesUpdated != null)
            {
                SamplesStopped(this, args);
            }
            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this, args);
            }
        }

        /// <summary>
        /// Moves the sample from the running queue to the finished queue.
        /// </summary>
        /// <param name="sample"></param>
        public void FinishSampleRun(classSampleData sampleData)
        {
            // uhhh this sample was null
            if (sampleData == null)
            {
                //TODO: Add error display.
                //throw new NullReferenceException("The sample that is said done was not an actual sample");
                return;
            }
            //
            // Find the sample provided to complete.
            //
            var sample = FindSample(m_runningQueue, sampleData.UniqueID);
            if (sample == null)
            {
                var errorMessage = string.Format("The sample {0} was not found on the running Queue.",
                    sampleData);
                //TODO: BLL Removed because of the notification system.   throw new classSampleNotRunningException(errorMessage);
                return;
            }

            lock (m_runningQueue)
            {
                m_runningQueue.Remove(sample);
            }
            lock (m_completeQueue)
            {
                m_completeQueue.Add(sample);
            }

            //
            // Moves the sample pointer backward to the front of the running queue.
            //
            m_nextAvailableSample = Math.Max(m_nextAvailableSample - 1, 0);
            sample.RunningStatus = enumSampleRunningStatus.Complete;


            var args = new classSampleQueueArgs(
                new classSampleData[] {sample},
                m_nextAvailableSample,
                m_runningQueue.Count,
                m_completeQueue.Count,
                m_waitingQueue.Count);

            if ((m_nextAvailableSample == 0) && (m_runningQueue.Count == 0))
            {
                m_startedSamples = false;
            }

            if (SamplesFinished != null)
            {
                SamplesFinished(this, args);
            }
            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this, args);
            }
        }

        /// <summary>
        /// Moves the waiting sample onto the running queue.
        /// </summary>
        /// <param name="sample">Sample to start</param>
        public classSampleData NextSampleStart()
        {
            classSampleData sample = null;

            //
            // Dequeue the sample, and start it.
            //
            lock (m_runningQueue)
            {
                if (m_nextAvailableSample < m_runningQueue.Count && m_startedSamples)
                {
                    sample = m_runningQueue[m_nextAvailableSample++];
                    sample.RunningStatus = enumSampleRunningStatus.Running;

                    if (SamplesStarted != null)
                    {
                        SamplesStarted(this, new classSampleQueueArgs(new classSampleData[] {sample}));
                    }
                }
            }

            var args = new classSampleQueueArgs(
                new classSampleData[] {sample},
                m_nextAvailableSample,
                m_runningQueue.Count,
                m_completeQueue.Count,
                m_waitingQueue.Count);
            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this, args);
            }
            return sample;
        }

        /// <summary>
        /// Gets the next sample waiting to be run.  Similar to a peek method for file reading,
        /// i.e. does not remove from queue or change running status.
        /// </summary>
        /// <returns></returns>
        public classSampleData NextSampleQuery()
        {
            classSampleData sample = null;
            lock (m_runningQueue)
            {
                if (m_nextAvailableSample < m_runningQueue.Count && m_startedSamples)
                {
                    sample = m_runningQueue[m_nextAvailableSample];
                }
            }
            return sample;
        }

        #endregion

        #region Cache and Save Operations

        /// <summary>
        /// Writes each of the queue lists to a SQLite cache file
        /// </summary>
        public void CacheQueue(bool buildconnectionString)
        {
            // This means that we have to recompile the data ...
            if (buildconnectionString)
            {
                var cartNames = classSQLiteTools.GetCartNameList();
                var columnNames = classSQLiteTools.GetColumnList(false);
                var datasetNames = classSQLiteTools.GetDatasetTypeList(false);
                var instrumentList = classSQLiteTools.GetInstrumentList(false);
                var separationTypes = classSQLiteTools.GetSepTypeList(false);
                var userNames = classSQLiteTools.GetUserList(false);
                var separationDefault = classSQLiteTools.GetDefaultSeparationType();

                classSQLiteTools.BuildConnectionString(true);

                classSQLiteTools.SaveInstListToCache(instrumentList);
                classSQLiteTools.SaveSelectedSeparationType(separationDefault);
                classSQLiteTools.SaveSingleColumnListToCache(columnNames, enumTableTypes.ColumnList);
                classSQLiteTools.SaveSingleColumnListToCache(datasetNames, enumTableTypes.DatasetTypeList);
                classSQLiteTools.SaveSingleColumnListToCache(separationTypes, enumTableTypes.SeparationTypeList);
                classSQLiteTools.SaveSingleColumnListToCache(cartNames, enumTableTypes.CartList);
                classSQLiteTools.SaveUserListToCache(userNames);
            }
            classSQLiteTools.SaveQueueToCache(m_waitingQueue, enumTableTypes.WaitingQueue);
            classSQLiteTools.SaveQueueToCache(m_runningQueue, enumTableTypes.RunningQueue);
            classSQLiteTools.SaveQueueToCache(m_completeQueue, enumTableTypes.CompletedQueue);
            IsDirty = false;
        }

        /// <summary>
        /// Caches the queue and changes the default name.
        /// </summary>
        /// <param name="cachePath"></param>
        public void CacheQueue(string cachePath)
        {
            classSQLiteTools.SetCacheLocation(cachePath);
            CacheQueue(true);
        }

        /// <summary>
        /// Loads samples stored in the cache back into the user interface and queue.
        /// </summary>
        public void RetrieveQueueFromCache()
        {
            RetrieveQueueFromCache(true);
        }

        public void RetrieveQueueFromCache(bool buildConnectionString)
        {
            m_completeQueue = classSQLiteTools.GetQueueFromCache(enumTableTypes.CompletedQueue);
            foreach (var sample in m_completeQueue)
            {
                sample.RunningStatus = enumSampleRunningStatus.Complete;
                m_uniqueID.Add(sample.UniqueID);

                if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                {
                    sample.LCMethod =
                        classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
                }
                else
                {
                    sample.LCMethod = null;
                }

                if (sample.UniqueID >= m_sampleIndex)
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID + 1);

                if (sample.SequenceID >= m_sequenceIndex)
                    m_sequenceIndex = Convert.ToInt32(sample.SequenceID + 1);
            }

            //
            // Loads the samples and creates unique sequence ID's and unique id's
            //
            List<classSampleData> waitingSamples;
            waitingSamples = classSQLiteTools.GetQueueFromCache(enumTableTypes.WaitingQueue);

            //
            // Update the Waiting Sample queue with the right LC-Methods.  This makes sure
            // that we always have valid LC-methods.  Otherwise when we go to run the
            // scheduler and thus program will crash hard.
            //
            foreach (var sample in waitingSamples) // m_waitingQueue)
            {
                if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                {
                    sample.LCMethod =
                        classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
                    if (sample.LCMethod.Column >= 0)
                    {
                        // reset the column data.
                        var column = classCartConfiguration.Columns[sample.LCMethod.Column];
                        sample.ColumnData = column;
                    }
                }
                else
                {
                    sample.LCMethod = null;
                }

                if (sample.UniqueID >= m_sampleIndex)
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID);

                if (sample.SequenceID >= m_sequenceIndex)
                    m_sequenceIndex = Convert.ToInt32(sample.SequenceID + 1);
            }

            //ResetColumnData(true);

            //
            // Put them on the waiting queue - we do it this way so that
            // we push stuff on the undo/redo stack.  That logic is already in place
            // in the queue samples method.  We also see if no samples were added (updated == false)
            // and completeQueue.length > 0 that we force an update.  Otherwise if no samples
            // were left in the cache on the waiting queue but were on the complete queue, then
            // we wouldnt see the completed samples...big bug.
            //
            var updated = QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
            if (updated == false && m_completeQueue.Count > 0)
            {
                if (SamplesAdded != null)
                {
                    var args = new classSampleQueueArgs(GetAllSamples());
                    SamplesAdded(this, args);
                }
            }
        }

        /// <summary>
        /// Saves the queue to the appropiate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writer"></param>
        public void SaveQueue(string path, ISampleQueueWriter writer)
        {
            SaveQueue(path, writer, false);
        }

        /// <summary>
        /// Saves the queue to the appropiate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writer"></param>
        public void SaveQueue(string path, ISampleQueueWriter writer, bool includeRunning)
        {
            var samples = new List<classSampleData>();
            if (includeRunning && m_runningQueue.Count > 0)
            {
                samples.AddRange(m_runningQueue);
            }
            samples.AddRange(m_waitingQueue);
            writer.WriteSamples(path, samples);
        }

        /// <summary>
        /// Saves the queue to the appropiate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writer"></param>
        public void LoadQueue(string path, ISampleQueueReader reader)
        {
            List<classSampleData> waitingSamples;
            waitingSamples = reader.ReadSamples(path);

            //
            // We need to assign the column data information to the samples
            // since columns have not been assigned.
            //
            var index = 0;
            if (m_waitingQueue.Count > 0)
            {
                index = m_columnOrders.IndexOf(m_waitingQueue[m_waitingQueue.Count - 1].ColumnData) + 1;
            }
            else
            {
                index = m_columnOrders.IndexOf(m_columnOrders[0]);
            }

            //
            // For all the entries in the new list of samples...add some column information back into it.
            //
            //foreach (classSampleData data in waitingSamples)
            //{
            //    classColumnData columnData = m_columnOrders[index % m_columnOrders.Count];
            //    data.ColumnData            = columnData;
            //    index++;
            //}

            //
            // Make sure the method references are created
            //
            foreach (var sample in waitingSamples)
            {
                if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                {
                    sample.LCMethod =
                        classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
                    var columnID = sample.LCMethod.Column;
                    if (columnID > 0)
                    {
                        sample.ColumnData = classCartConfiguration.Columns[columnID];
                    }
                }
                sample.DmsData.CartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
                if (sample.UniqueID >= m_sampleIndex)
                {
                    m_sampleIndex = Convert.ToInt32(sample.UniqueID);
                }
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }

        /// <summary>
        /// Saves the queue to the appropiate object.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writer"></param>
        public void LoadQueue(string path, ISampleQueueReader reader, classColumnData column)
        {
            List<classSampleData> waitingSamples;
            waitingSamples = reader.ReadSamples(path);

            //
            // Here we need to assign the column data information to the samples
            // if one has not already been assigned.
            //
            foreach (var data in waitingSamples)
            {
                data.DmsData.CartName = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME);
                data.ColumnData = column;
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }

        #endregion
    }
}