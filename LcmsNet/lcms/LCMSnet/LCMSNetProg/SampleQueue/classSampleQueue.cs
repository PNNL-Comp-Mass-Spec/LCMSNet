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
 *			2/12/2009: Dave Clark
 *				Added method for saving queue to SQLite database
 *		    3/17/2009: BLL
 *		        Generates unique ID's and sample ID's
 *		    5/18/2010: DAC
 *				  AModified for queue export to SQLite file and CSV file
 */
//*********************************************************************************************************
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using LcmsNet.Configuration;
using LcmsNetSQLiteTools;
using LcmsNet.Method;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Configuration;

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
        private int mint_sequenceIndex;
        /// <summary>
        /// Index of next available sample in the running queue.
        /// </summary>
        private int mint_nextAvailableSample;
        /// <summary>
        /// Index count of samples
        /// </summary>
        private int mint_sampleIndex;
        /// <summary>
        /// Name of samples that are added through distribution across columns.
        /// </summary>
        private string mstring_integrateName;
        /// <summary>
        /// List of columns that are enabled or disabled.
        /// </summary>
        //private List<classColumnData> mlist_columnOrders;
        /// <summary>
        /// Queue of sample data to be run.  Index 0 should be the next sample to run.
        /// </summary>
        private List<classSampleData> mlist_waitingQueue;
        /// <summary>
        /// Queue of samples that have already ran.  Index 0 being the oldest run sample. Index N (size of queue
        /// being the most recently run sample).
        /// </summary>
        private List<classSampleData> mlist_completeQueue;
        /// <summary>
        /// List of samples that are currently running.
        /// </summary>
        private List<classSampleData> mlist_runningQueue;             
        /// <summary>
        /// List of unique ID's used in the sample queue.
        /// </summary>
        private List<long> mlist_uniqueID;
        /// <summary>
        /// Flag indicating whether to re-set the column data when a queue operation is performed.
        /// </summary>
        private bool mbool_resetColumns;
        /// <summary>
        /// Default name of the sample to add when distributing across columns.        
        /// </summary>
        private string mstring_defaultSampleName;
        /// <summary>
        /// Stack of waiting queues for undo operations.
        /// </summary>
        private Stack<List<classSampleData>> mstack_undoBackWaitingQueue;
        /// <summary>
        /// Stack of samples for redo operations
        /// </summary>
        private Stack<List<classSampleData>> mstack_undoForwardWaitingQueue;
        /// <summary>
        /// 
        /// </summary>
        private List<classColumnData> mlist_columnOrders;
        /// <summary>
        /// Flag indicating whether the samples were started explicitly by the caller.  
        /// If false the queries for next samples will return null indicating no samples can be started.
        /// If true, and samples finish or are cancelled (i.e. errors) then the flag will reset to false.
        /// </summary>
        private bool mbool_startedSamples;
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
        public AutoResetEvent mobj_sampleWaitingEvent;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for a sample queue.
        /// </summary>
        public classSampleQueue()
        {			
            mlist_completeQueue = new List<classSampleData>();
            mlist_waitingQueue  = new List<classSampleData>();
            mlist_runningQueue  = new List<classSampleData>();
            mlist_uniqueID      = new List<long>();

            mstring_defaultSampleName = CONST_DEFAULT_SAMPLENAME;
            mstring_integrateName     = CONST_DEFAULT_INTEGRATE_SAMPLENAME;
            mbool_resetColumns        = true;
            mint_sampleIndex          = 1;
            mobj_sampleWaitingEvent   = new AutoResetEvent(false);
            mlist_columnOrders        = new List<classColumnData>();

            // Undo - redo operations
            mstack_undoBackWaitingQueue     = new Stack<List<classSampleData>>();
            mstack_undoForwardWaitingQueue  = new Stack<List<classSampleData>>();

            
            // Pointer to the next available sample that is queued for running.
            mint_nextAvailableSample = 0;

            // Tracks what is the current sequence number that has been run previously.
            mint_sequenceIndex = 1;


            UpdateColumnList();
            foreach (classColumnData column in classCartConfiguration.Columns)
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
            mlist_columnOrders.Clear();
            mlist_columnOrders = classCartConfiguration.BuildColumnList(true);         
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Creates a new unique ID number not used and stores it.  This always generates the largest unique ID.
        /// </summary>
        /// <returns>Unique ID number.</returns>
        private long GenerateUniqueID()
        {
            /// Get the largest unique ID in the set.
            int count     = mlist_uniqueID.Count;
            long uniqueID = 1;
            if (count > 0)
                uniqueID = mlist_uniqueID[count - 1] + 1;
            mlist_uniqueID.Add(uniqueID);
            return uniqueID;
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
                /// 
                /// Make sure that we have enough columns to add data on.
                /// 
                if (mlist_columnOrders.Count < 1)
                    return null;


                int index = -1;

                /// 
                /// Get the index of the last sample that is queued...
                /// 

                /// 
                /// if the waiting queue is empty. that means no samples are queued
                /// TODO:  Add to see what the last sample in the running or complete queue is                
                /// 
                if (mlist_waitingQueue.Count > 0)
                {

                    classSampleData data = mlist_waitingQueue[mlist_waitingQueue.Count - 1];
                    index = mlist_columnOrders.IndexOf(data.ColumnData);
                }
                else
                {
                    if (mlist_runningQueue.Count > 0)
                    {
                        index = mlist_columnOrders.IndexOf(mlist_runningQueue[mlist_runningQueue.Count - 1].ColumnData);
                    }
                    else if (mlist_completeQueue.Count > 0)
                    {
                        index = mlist_columnOrders.IndexOf(mlist_completeQueue[mlist_completeQueue.Count - 1].ColumnData);
                    }
                }
                /// 
                /// Then return the column data available at the end of queue                
                /// 
                return mlist_columnOrders[(index + 1) % mlist_columnOrders.Count];
            }
        }
        /// <summary>
        /// Gets the name of the un-used sample.
        /// </summary>
        public string UnusedSampleName
        {
            get
            {
                return mstring_integrateName;
            }
        }
        /// <summary>
        /// Gets or sets the column data.
        /// </summary>
        public List<classColumnData> ColumnOrder
        {
            get
            {
                return mlist_columnOrders;
            }
        }
        /// <summary>
        /// Gets or sets whether to reset the column data.        
        /// </summary>
        public bool AutoColumnData
        {
            get
            {
                return mbool_resetColumns;
            }
            set
            {
                mbool_resetColumns = value;
            }
        }
        /// <summary>
        /// Gets or sets the default sample name.
        /// </summary>
        public string DefaultSampleName
        {
            get
            {
                return mstring_defaultSampleName;
            }
            set
            {
                mstring_defaultSampleName = value;
            }
        }
        /// <summary>
        /// Gets or sets the running sample index of samples that have been 
        /// added to the queue.
        /// </summary>
        public int RunningSampleIndex
        {
            get
            {
                return mint_sampleIndex;
            }
            set
            {
                mint_sampleIndex = value;
            }
        }
        /// <summary>
        /// Gets the threading event when a sample is queued.
        /// </summary>
        public AutoResetEvent SampleQueuedEvent
        {
            get
            {
                return mobj_sampleWaitingEvent;
            }
        }
        #endregion

        #region Queue Searching
        /// <summary>
        /// Determines if the waiting sample queue has any unused samples.
        /// </summary>
        /// <returns></returns>
        public bool HasUnusedSamples()
        {
            /// 
            /// Find the first sample that has an unused name
            /// 
            foreach (classSampleData sample in mlist_waitingQueue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName)
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
            /// 
            /// Find the first sample that has an unused name
            /// 
            foreach (classSampleData sample in mlist_waitingQueue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName && column.ID == sample.ColumnData.ID)
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
            List<classSampleData> samples = new List<classSampleData>();
            /// 
            /// Search all of the queues and add any instance of a copy
            /// 
            foreach (classSampleData data in mlist_waitingQueue)
                if (data.DmsData.DatasetName == sampleName)
                    samples.Add(data);
            foreach (classSampleData data in mlist_runningQueue)
                if (data.DmsData.DatasetName == sampleName)
                    samples.Add(data);
            foreach (classSampleData data in mlist_completeQueue)
                if (data.DmsData.DatasetName == sampleName)
                    samples.Add(data);
            
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
            foreach (classSampleData sample in mlist_completeQueue)
                if (sample.UniqueID == uniqueID)
                    return sample;
            foreach (classSampleData sample in mlist_runningQueue)
                if (sample.UniqueID == uniqueID)
                    return sample;
            foreach (classSampleData sample in mlist_waitingQueue)
                if (sample.UniqueID == uniqueID)
                    return sample;
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
            foreach (classSampleData sample in queue)
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
            List<classSampleData> unusedSamples = new List<classSampleData>();
            foreach (classSampleData sample in queue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName)
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
            List<classSampleData> unusedSamples = new List<classSampleData>();
            foreach (classSampleData sample in queue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName && sample.ColumnData == column)
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

            if (mlist_waitingQueue.Count > 0)
            {
                /// 
                /// Remove from the run queue
                /// 
                data = mlist_waitingQueue[0];
                mlist_waitingQueue.RemoveAt(0);

                /// 
                /// Put the data on the queue.
                /// 
                mlist_runningQueue.Add(data);
            }
            return data;
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
            enumSampleValidResult result = enumSampleValidResult.Valid;

            /// 
            /// Determine if the sample has a duplicate request name.
            /// If it has one match, then it should be itself.
            /// 
            List<classSampleData> data = FindSample(sample.DmsData.DatasetName);
            if (data.Count > 1)
            {
                result = enumSampleValidResult.DuplicateRequestName;
                return result;
            }

            return result;
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
            /// 
            /// Create a queue histogram.
            ///
            Dictionary<classColumnData, List<classSampleData>> sampleHistogram = new Dictionary<classColumnData, List<classSampleData>>();
            foreach (classColumnData col in mlist_columnOrders)
            {
                sampleHistogram.Add(col, new List<classSampleData>());
            }
            /// 
            /// Calculate the Histogram
            /// 
            for (int i = 0; i < queue.Count; i++)
            {
                classSampleData data = queue[i];
                classColumnData col = data.ColumnData;
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
            /// 
            /// Figure out how many items we have on this column.
            /// 
            int maxCount = -1;
            foreach (List<classSampleData> data in histogram.Values)
            {
                maxCount = Math.Max(data.Count, maxCount);
            }

            /// 
            /// Now we have a histogram of columns whose entries are a List of samples
            /// For every column whose List Count is less than the maximum, add unused samples to it
            /// At this point the waiting list does not mean anything.  We will re-construct
            /// the waiting queue after this point.
            /// 
            foreach (classColumnData col in histogram.Keys)
            {
                List<classSampleData> data = histogram[col];
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
                                /// 
                                /// Because sample clones are deep copies, we cannot trust that
                                /// every object in the sample is serializable...so...we are stuck
                                /// making sure we re-hash the method using the name which 
                                /// is copied during the serialization.
                                /// 
                                sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                            }
                        }
                    }
                    else
                    {
                        sample = new classSampleData();
                    }

                    /// 
                    /// We make the request an "Unused sample"
                    /// 
                    sample.DmsData.RequestName  = mstring_integrateName;
                    sample.DmsData.DatasetName  = mstring_integrateName;
                    sample.DmsData.Block        = 0;    // It's an unused sample. so dont copy this information.
                    sample.DmsData.Batch        = 0;
                    sample.ColumnData           = col;
                    sample.UniqueID             = GenerateUniqueID();
                    data.Add(sample);
                }
            }
            queue.Clear();
            /// 
            /// Here we reconstruct the waiting queue.  We use the column orders list (tells us 
            /// what column is the first one to use) to add samples in order.  
            /// 
            List<classSampleData> firstList = histogram[mlist_columnOrders[0]];
            for (int i = 0; i < firstList.Count; i++)
            {
                classSampleData firstSample = firstList[i];
                queue.Add(firstSample);

                /// 
                /// Now add each sample found on another column
                /// 
                for (int j = 1; j < mlist_columnOrders.Count; j++)
                {
                    classColumnData col = mlist_columnOrders[j];
                    classSampleData sample = histogram[col][i];
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
            /// 
            /// Remove excess items from the end of the list.
            /// 
            while (queue.Count > 0 && queue[queue.Count - 1].DmsData.DatasetName == mstring_integrateName)
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
                foreach (classSampleData sample in mlist_completeQueue)
                {
                    sequence = Math.Max(sample.SequenceID, sequence);
                }
                foreach (classSampleData sample in mlist_runningQueue)
                {
                    sequence = Math.Max(sample.SequenceID, sequence);
                }
                sequence++;

                mint_sequenceIndex = Convert.ToInt32(sequence);

                ResequenceQueuedSamples(queue, sequence);
            }
        }
        /// <summary>
        /// Resequences the samples in the queued samples.
        /// </summary>
        /// <param name="start">Start offset to resequence from.</param>
        private void ResequenceQueuedSamples(List<classSampleData> queue, long startSequence)
        {
            foreach (classSampleData data in queue)
            {
                data.SequenceID = startSequence++;
            }
            mint_sequenceIndex = Convert.ToInt32(startSequence);
        }
        #endregion

        #region Undo
        /// <summary>
        /// Pushes the new queue onto the stack for undo operations.
        /// </summary>
        /// <param name="stack">Stack to push queue onto.</param>
        /// <param name="queue">Queue to push onto stack.</param>
        private void PushQueue(Stack<List<classSampleData>> backStack, Stack<List<classSampleData>> forwardStack, List<classSampleData> queue)
        {
            PushQueue(backStack, forwardStack, queue, true);
        }
        /// <summary>
        /// Pushes the queue onto the backstack and the 
        /// </summary>
        /// <param name="backStack"></param>
        /// <param name="queue"></param>
        /// <param name="clearForward"></param>
        private void PushQueue( Stack<List<classSampleData>> backStack,
                                Stack<List<classSampleData>> forwardStack, 
                                List<classSampleData> queue, 
                                bool clearForward)
        {
            /// 
            /// If the user wants us to clear the forward stack then we will,
            /// otherwise we ignore it.
            /// 
            if (clearForward == true && forwardStack != null)            
                forwardStack.Clear();
            

            List<classSampleData> pushQueue = new List<classSampleData>();
            foreach (classSampleData data in queue)
            {
                classSampleData sample = data.Clone() as classSampleData;
                if (sample.LCMethod != null && sample.LCMethod.Name != null)
                {
                    if (classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                    {
                        /// 
                        /// Because sample clones are deep copies, we cannot trust that
                        /// every object in the sample is serializable...so...we are stuck
                        /// making sure we re-hash the method using the name which 
                        /// is copied during the serialization.
                        /// 
                        sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                    }
                }
                pushQueue.Add(sample);
            }
            backStack.Push(pushQueue);
            IsDirty = true;
        }
        public bool IsDirty
        {
            get;
            set;
        }
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

            IsDirty  = true;
            newQueue = backStack.Pop();
            
            return newQueue;
        }
        /// <summary>
        /// Undoes the most recent operation on the queue.
        /// </summary>
        public void Undo()
        {
            
            /// 
            /// Pop the 
            /// 
            List<classSampleData> queue = PopQueue(mstack_undoBackWaitingQueue);

            /// Then if popping
            if (queue != null && queue.Count > 0)
            {
                /// 
                /// Save the current waiting queue onto the forward stack, thus saving it for a redo
                /// 
                PushQueue(mstack_undoForwardWaitingQueue, null, mlist_waitingQueue);            

                /// Transfer the new queue to our waiting queue.
                mlist_waitingQueue.Clear();
                mlist_waitingQueue = queue;
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
            /// 
            /// Pull the queue off the forward stack if one exists            
            /// 
            List<classSampleData> queue = PopQueue(mstack_undoForwardWaitingQueue); 
            
            if (queue != null && queue.Count > 0)
            {
                /// 
                /// Push the current queue onto the back stack, thus saving our waiting queue.
                /// 
                PushQueue(mstack_undoBackWaitingQueue, null, mlist_waitingQueue);

                mlist_waitingQueue.Clear();
                mlist_waitingQueue = queue;
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

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue,  mlist_waitingQueue);
            /// 
            /// Overwrite all of the samples that are unused first. 
            /// 
            /// Then add to the queue if the number of unused samples are less
            /// than the number of provided ones.
            /// 
            int i = 0;
            while(i < mlist_waitingQueue.Count && samples.Count > 0)
            {
                classSampleData unusedSample = mlist_waitingQueue[i];
                if (unusedSample.DmsData.DatasetName.Contains(mstring_integrateName))
                {
                    classSampleData sample = samples[0];
                    samples.RemoveAt(0);
                    mlist_waitingQueue[i]           = sample;
                    mlist_waitingQueue[i].UniqueID  = GenerateUniqueID();
                }
                i++;
            }

            /// 
            /// ask if there are any leftovers
            /// 
            if (samples.Count > 0)
            {
                /// 
                /// Dont need to re-sequence or notify
                /// because queue samples does it for us.
                /// 
                QueueSamples(samples, handling);
            }
            else
            {
                /// 
                /// Always re-sequence, and notify
                /// 
                ResequenceQueuedSamples(mlist_waitingQueue);

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
        public void InsertIntoUnusedSamples(List<classSampleData> samples, classColumnData column, enumColumnDataHandling handling)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// Overwrite all of the samples that are unused first. 
            /// 
            /// Then add to the queue if the number of unused samples are less
            /// than the number of provided ones.
            /// 
            int i = 0;
            while (i < mlist_waitingQueue.Count && samples.Count > 0)
            {
                classSampleData unusedSample = mlist_waitingQueue[i];
                if (unusedSample.DmsData.DatasetName.Contains(mstring_integrateName) && unusedSample.ColumnData == column)
                {
                    classSampleData sample = samples[0];
                    samples.RemoveAt(0);
                    mlist_waitingQueue[i]            = sample;
                    mlist_waitingQueue[i].UniqueID   = GenerateUniqueID();
                }
                i++;
            }

            /// 
            /// ask if there are any leftovers
            /// 
            if (samples.Count > 0)
            {
                /// 
                /// Dont need to re-sequence or notify
                /// because queue samples does it for us.
                /// 
                QueueSamples(samples, handling);
            }
            else
            {
                /// 
                /// Always re-sequence, and notify
                /// 
                ResequenceQueuedSamples(mlist_waitingQueue);
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
        public void MoveQueuedSamples(List<classSampleData> samples, int baseOffset, int offset, enumMoveSampleType moveType)
        {
            SwapQueuedSamplesColumn(mlist_waitingQueue, samples, baseOffset, offset, enumColumnDataHandling.LeaveAlone);
        }
        /// <summary>
        /// Adds a sample to the list of waiting samples.
        /// </summary>
        /// <param name="sampleList">List of samples to add.</param>
        /// <param name="forceColumns">Tells the queue operation whether to distribute samples on other columnds</param>
        /// <returns>True if addition was a success.  False if addition failed.</returns>                        
        public bool QueueSamples(IEnumerable<classSampleData> sampleList, enumColumnDataHandling distributeSamplesEvenlyAcrossColumns)
        {          
            bool added = false;

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// Here we add to the waiting queue.
            ///     1. Distribute the samples across the waiting queue using a histogram
            ///         to figure out the distribution of samples.  This version will
            ///         add unused samples that never get run but help balance the 
            ///         queue building.
            ///     2. Add the samples directly back to the waiting queue - append.
            ///         This just puts samples on the queue.
            /// 
                
            if (distributeSamplesEvenlyAcrossColumns == enumColumnDataHandling.CreateUnused)
            {
                List<classSampleData> tempQueue                                    = new List<classSampleData>();
                Dictionary<classColumnData, List<classSampleData>> sampleHistogram = BuildSampleHistogram(mlist_waitingQueue);
                /// 
                /// Add samples to their respective columns.
                /// 
                foreach (classSampleData sample in sampleList)
                {
                    /// 
                    /// Check for object references.
                    /// We allow for the same request name to be had in the 
                    /// sample list.
                    /// 
                    if (mlist_waitingQueue.Contains(sample) == false)
                    {
                        added           = true;
                        sample.UniqueID = GenerateUniqueID();
                        sampleHistogram[sample.ColumnData].Add(sample);
                        tempQueue.Add(sample);
                    }
                }                
                DistributeSamplesAcrossColumns(sampleHistogram, mlist_waitingQueue);             
            }
            else
            {
                foreach (classSampleData data in sampleList)
                {

                    data.UniqueID = GenerateUniqueID();
                    mlist_waitingQueue.Add(data);
                    added = true;
                }             
            }
            
            /// 
            /// Now make sure we alert everyone that we added a sample
            /// 
            if (added == true)
            {
                /// 
                /// Only re-sequence and remove if we changed the queue to not waste time
                /// doing something if we dont have to.
                /// 
                RemoveExcessSamples(mlist_waitingQueue);
                ResequenceQueuedSamples(mlist_waitingQueue);

                if (SamplesAdded != null)
                {                                        
                    classSampleQueueArgs args = new classSampleQueueArgs(GetAllSamples());
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
                classSampleQueueArgs args = new classSampleQueueArgs(GetAllSamples());
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
                classSampleQueueArgs args = new classSampleQueueArgs(GetWaitingQueue());
                SamplesAdded(this, args);
            }
        }
        /// <summary>
        /// Compiles all of the samples from the sample queue.
        /// </summary>
        /// <returns>List of all run, running, and waiting samples.</returns>
        private List<classSampleData> GetAllSamples()
        {
            List<classSampleData> allSamples = new List<classSampleData>();
            allSamples.AddRange(mlist_completeQueue);
            allSamples.AddRange(mlist_runningQueue);
            allSamples.AddRange(mlist_waitingQueue);
            return allSamples;
		}
		/// <summary>
		/// Gets a list of all the samples in the waiting queue
		/// </summary>
		/// <returns>List of samples in waiting queue</returns>
		public List<classSampleData> GetWaitingQueue()
		{
			List<classSampleData> waitingQueue = new List<classSampleData>();
			waitingQueue.AddRange(mlist_waitingQueue);
			return waitingQueue;
		}
		/// <summary>
		/// Gets a list of all the samples in the waiting queue
		/// </summary>
		/// <returns>List of samples in waiting queue</returns>
		public List<classSampleData> GetRunningQueue()
		{
			List<classSampleData> runningQueue = new List<classSampleData>();
			runningQueue.AddRange(mlist_runningQueue);
			return runningQueue;
		}
        /// <summary>
        /// Class that adds a sample to the list of managed samples.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        private bool RemoveSample(classSampleData sample)
        {
            bool removed = false;
            if (mlist_waitingQueue.Contains(sample) == true)
            {
                mlist_waitingQueue.Remove(sample);
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
            bool removed = false;
            /// 
            /// Find each sample if it exists,
            /// 
            foreach (long id in uniqueIDs)
            {
                classSampleData sample = FindSample(queue, id);
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
            bool removed = false;
            /// 
            /// Remove the sample from the complete queue.
            /// 
            removed = RemoveSample(mlist_completeQueue, uniqueIDs);

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// First we figure out what samples we need to remove based on their
            /// Unique ID's.  Then we remove them.
            /// 
            List<classSampleData> removedSamples = new List<classSampleData>();
            foreach (long i in uniqueIDs)
            {
                classSampleData data = FindSample(i);
                if (data != null)
                {
                    /// 
                    /// Check for duplicate object references
                    /// Also, if we are to distribute across columns that means we remove it 
                    /// from the queue.  Then we allow the program to reseat the samples on new
                    /// columns.  
                    /// 
                    /// Otherwise, we just make the sample name unused.
                    /// 
                    if (mlist_waitingQueue.Contains(data) == true)
                    {
                        
                        mlist_waitingQueue.Remove(data);
                        removedSamples.Add(data);
                        removed = true;
                    }
                }
            }


            /// 
            /// If we removed then we want to tell someone what samples were removed.
            /// Later we re-sequence them.
            /// 
            /// Also, make sure we reseat samples on new columns.  We do this because
            /// removed can only be true if we removed some samples instead of renaming.
            /// 
            if (removed)
            {

                switch(resortColumns)
                {
                    case enumColumnDataHandling.Resort:
                        ResetColumnData();
                        break;  
                    case enumColumnDataHandling.LeaveAlone:
                        break;
                    case enumColumnDataHandling.CreateUnused:                                    
                        /// 
                        /// Then, if some were removed we build up the distribution list and fill back into the queue
                        /// allowing for the correct spacing on other columns, but inserting unused samples at the end of this column list
                        /// so we dont disturb other column Data.  We do this using the same mechanism that adding samples uses 
                        /// but here we are not adding new samples to the queue.
                        ///            
                        Dictionary<classColumnData, List<classSampleData>> histogram = BuildSampleHistogram(mlist_waitingQueue);
                        DistributeSamplesAcrossColumns(histogram, mlist_waitingQueue);
                        RemoveExcessSamples(mlist_waitingQueue);
                        break;
                }

                ResequenceQueuedSamples(mlist_waitingQueue);

                if (SamplesRemoved != null)
                {
                    SamplesRemoved(this, new classSampleQueueArgs(GetAllSamples()));
                }

                if (SamplesUpdated != null)
                    SamplesUpdated(this, new classSampleQueueArgs(mlist_waitingQueue));
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
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// Find the unique id's of all the samples that are unused.
            /// 
            List<long> uniqueList = new List<long>();
            foreach (classSampleData sample in mlist_waitingQueue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            /// 
            /// Removes the samples from this list, AND! resorts the columns.
            /// This is important for this method because it means that 
            /// we dont care about preserving column information.
            /// 
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
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// Find the unique id's of all the samples that are unused.
            /// 
            List<long> uniqueList = new List<long>();
            foreach (classSampleData sample in mlist_waitingQueue)
            {
                if (sample.DmsData.DatasetName == mstring_integrateName && sample.ColumnData == column)
                {
                    uniqueList.Add(sample.UniqueID);
                }
            }

            /// 
            /// Removes the samples from this list, AND DOES NOT! Resort the columns.
            /// This is important for this method because it means that 
            /// we DO care about preserving column information.
            /// 
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

            if (mlist_waitingQueue.Count < 1)
                return;

            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            long baseOffset = mlist_waitingQueue[0].SequenceID;
            foreach (classSampleData sample in newOrders)
            {
                long newIndex = sample.SequenceID - baseOffset;
                if (mlist_waitingQueue.Count > newIndex)
                {
                    mlist_waitingQueue[Convert.ToInt32(newIndex)] = sample;
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
                SamplesUpdated(this, new classSampleQueueArgs(mlist_waitingQueue));
            }
        }
        /// <summary>
        /// Resets the column data after a move, delete operation.
        /// </summary>
        public void ResetColumnData()
        {
            int index = 0;

            if (mlist_runningQueue.Count > 0)
            {
                index = mlist_columnOrders.IndexOf(mlist_runningQueue[mlist_runningQueue.Count - 1].ColumnData) + 1;
            }
            else if (mlist_completeQueue.Count > 0)
            {
                index = mlist_columnOrders.IndexOf(mlist_completeQueue[mlist_completeQueue.Count - 1].ColumnData) + 1;
            }

            foreach (classSampleData data in mlist_waitingQueue)
            {
                classColumnData columnData = mlist_columnOrders[index % mlist_columnOrders.Count];
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

            long firstSequenceNumber = mlist_waitingQueue[0].SequenceID;
            bool swapped = false;

            /// 
            /// If the offset is positive then move the samples down in the list - towards end of queue
            /// 
            if (offset > 0)
            {
                int lastIndex = queue.Count - 1;
                for (int i = samples.Count - 1; i >= 0; i--)
                {
                    /// 
                    /// If the first guy could not be swapped, then we dont want the other items
                    /// to be swapped either.  So continue if done so.
                    /// 
                    if (i < samples.Count - 1 && swapped == false)
                        continue;

                    /// find the location of the sample to swap in the waiting queue.                   
                    int index = queue.IndexOf(samples[i]);

                    /// 
                    /// Calculate its new location to swap if the offset is one, 
                    /// otherwise we dont want to swap by the last index because this would
                    /// move items to another column.
                    /// 
                    int newIndex = Math.Min(index + offset, lastIndex);
                    if (Math.Abs(offset) > 1 && index + offset > lastIndex)                    
                        newIndex = index;

                    if (newIndex != index)
                    {
                        classSampleData tempSample = queue[newIndex];
                        queue[newIndex] = mlist_waitingQueue[index];
                        queue[index]    = tempSample;
                        lastIndex       = newIndex - offset;
                        swapped         = true;
                    }
                    else
                    {
                        lastIndex = index - offset;
                    }
                }
            }
            /// Otherwise move the samples up in the queue order.
            else if (offset < 0)
            {
                int lastIndex = baseOffset;
                for (int i = 0; i < samples.Count; i++)
                {

                    /// 
                    /// If the first guy could not be swapped, then we dont want the other items
                    /// to be swapped either.  So continue if done so.
                    /// 
                    if (i > 0 && swapped == false)
                        continue;

                    /// find the location of the sample to swap in the waiting queue.                   
                    int index = queue.IndexOf(samples[i]);

                    /// 
                    /// Calculate its new location to swap if the offset is one, 
                    /// otherwise we dont want to swap by the last index because this would
                    /// move items to another column.
                    /// 
                    int newIndex = Math.Max(index + offset, lastIndex); 
                    if (Math.Abs(offset) > 1 && index + offset < lastIndex)                    
                        newIndex = index;
                    

                    if (newIndex != index)
                    {
                        /// Swap
                        classSampleData tempSample = queue[newIndex];
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
            /// 
            /// Update the sequence numbers
            /// 
            ResequenceQueuedSamples(queue, firstSequenceNumber);

            /// 
            /// Update the column data            
            /// 
            //if (handling == enumColumnDataHandling.Resort)
            //{
            //    ResetColumnData();
            //}

            /// 
            /// Tell listeners that we have re-sequenced the queue.
            /// 
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
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            /// 
            /// Find the sample 
            /// 
            int index = mlist_waitingQueue.IndexOf(data);
            if (index >= 0)
            {
                /// 
                /// Update...although the reference should be updated.
                /// 
                mlist_waitingQueue[index] = data;

                /// 
                /// Alert listening objects.
                /// 
                if (SamplesUpdated != null)
                {
                    SamplesUpdated(this, new classSampleQueueArgs(mlist_waitingQueue));
                }
            }
        }
        /// <summary>
        /// Updates the sample with new data and alerts all listening objects.
        /// </summary>
        /// <param name="data"></param>
        public void UpdateSamples(List<classSampleData> samples)
        {
            PushQueue(mstack_undoBackWaitingQueue, mstack_undoForwardWaitingQueue, mlist_waitingQueue);

            bool updated = false;
            foreach(classSampleData sample in samples)
            {
                classSampleData existingSample = FindSample(mlist_waitingQueue, sample.UniqueID);
                if (existingSample != null)
                {
                    int index = mlist_waitingQueue.IndexOf(existingSample);
                    if (index >= 0)
                    {
                        updated                   = true;
                        //if (sample.ColumnData == null)
                        //{
                        //    sample.ColumnData = existingSample.ColumnData;
                        //}
                        mlist_waitingQueue[index] = sample;
                    }
                }
            }
            /// 
            /// Alert listening objects.
            /// 
            if (updated == true && SamplesUpdated != null)
            {
                SamplesUpdated(this, new classSampleQueueArgs(mlist_waitingQueue));
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
            int i = 0;
            while ( i < mlist_runningQueue.Count && mlist_runningQueue[i].RunningStatus == enumSampleRunningStatus.Running)
            {
                    i++;
            }

            if (mlist_runningQueue.Count == i)
            {
                mlist_runningQueue.Insert(i, sample);
            }
            else
            {
                mlist_runningQueue.Add(sample);
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
            get
            {
                return mlist_runningQueue.Count > 0;
            }
        }
        /// <summary>
        /// Gets whether there are samples currently set with running status.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return mint_nextAvailableSample > 0;
            }
        }        
		/// <summary>
		/// Starts the samples 
		/// </summary>
		public void StartSamples()
		{
			// Don't run if nothing is there to run.
			if (mlist_runningQueue.Count == 0)
			{
				return;
			}

			if (mint_nextAvailableSample > 0)
			{
				return;
			}

			// Setup the queue to be optimized.
			List<classSampleData> validSamples = new List<classSampleData>();
            DateTime next;   
			foreach (classSampleData sample in mlist_runningQueue)
			{
				//DateTime next       = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).Add(new TimeSpan(0, 0, 10));
                next = LcmsNetSDK.TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10)); 
                bool containsMethod = classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name);
                if (containsMethod)
                {
                    sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name];
                }

				sample.LCMethod     = sample.LCMethod.Clone() as classLCMethod;
				validSamples.Add(sample);
				sample.LCMethod.SetStartTime(next);
                // We need to look for Daylight Savings Time Transitions and adjust for them here.
                if(LcmsNetSDK.TimeKeeper.Instance.DoDateTimesSpanDaylightSavingsTransition(sample.LCMethod.Start, sample.LCMethod.End))
                {
                    LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "QUEUE: some samples have been moved forward 1 hour due to a Daylight Savings Transition, this will avoid odd behavior while running the queue.");
                    next.Add(new TimeSpan(1, 0, 0));
                    sample.LCMethod.SetStartTime(next);
                }
			}

			// We have not started to run so optimize this way.
			classLCMethodOptimizer optimizer = new classLCMethodOptimizer();
			optimizer.AlignSamples(mlist_runningQueue);

			// Set the listening event so that time sensitive items will know that
			// a sample is waiting on the running queue.			
            mobj_sampleWaitingEvent.Set();
            mbool_startedSamples = true;		
		}
		/// <summary>
		/// Puts the sample on the running queue and starts if the operator has appened this sample to an
		/// already running queue.
		/// </summary>
		/// <param name="sample">Sample to run.</param>
		public void MoveSamplesToRunningQueue(classSampleData sample)
		{
			MoveSamplesToRunningQueue(new List<classSampleData>() { sample });
		}
        /// <summary>
        /// Tells the scheduler to run these samples, putting them on the waiting (running) queue.
        /// </summary>
        /// <param name="samples"></param>
        public void MoveSamplesToRunningQueue(List<classSampleData> samples)
        {			            
            // For each sample to run, set the status and delay the run for 10 seconds                      
            List<classSampleData> validSamples = new List<classSampleData>();

			classLCMethodOptimizer optimizer = new classLCMethodOptimizer();
            foreach (classSampleData sample in samples)
            {
                //DateTime next = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).Add(new TimeSpan(0, 0, 10));
                DateTime next = LcmsNetSDK.TimeKeeper.Instance.Now.Add(new TimeSpan(0, 0, 10));
                classSampleData realSample = FindSample(sample.UniqueID);
                if (realSample == null)
                {
                    /// 
                    /// this sample does not exist on the sample queue! 
                    /// 
                    throw new NullReferenceException("This sample does not exist in the waiting queue! " + sample.DmsData.DatasetName);
                }                
                mlist_waitingQueue.Remove(realSample);

                bool containsMethod = classLCMethodManager.Manager.Methods.ContainsKey(realSample.LCMethod.Name);
                if (containsMethod)
                {
                    realSample.LCMethod = classLCMethodManager.Manager.Methods[realSample.LCMethod.Name];
                }
                realSample.LCMethod = realSample.LCMethod.Clone() as classLCMethod;
                validSamples.Add(realSample);

                realSample.RunningStatus = enumSampleRunningStatus.WaitingToRun;
                
				if (mlist_runningQueue.Count > 0 && mint_nextAvailableSample > 0)
				{
					// We arent the first ones on the queue, but we are running,
					// so we need to hurry up and go!
					realSample.LCMethod.SetStartTime(next);
					optimizer.AlignSamples(mlist_runningQueue, realSample);
				}
				else if (mlist_runningQueue.Count == 0)
				{
					// Otherwise we are the first ones on the queue, but we dont need to do anything
                    // for alignment.
					realSample.LCMethod.SetStartTime(next);
				}
                mlist_runningQueue.Add(realSample);
            }

            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this, 
                                    new classSampleQueueArgs(validSamples,
                                                                mint_nextAvailableSample, 
                                                                mlist_runningQueue.Count,
                                                                mlist_completeQueue.Count, 
                                                                mlist_waitingQueue.Count));
            }         
        }
		/// <summary>
		/// Dequeues the samples from the running queue back onto the waiting queue.
		/// </summary>
		/// <param name="sample">Sample to dequeue.</param>
		public void DequeueSampleFromRunningQueue(classSampleData sample)
		{
			DequeueSampleFromRunningQueue(new List<classSampleData>() { sample });
		}
		/// <summary>
		/// Moves a sample from the running queue back onto the waiting queue.
		/// </summary>
		/// <param name="samples">Samples to put back on the waiting queue.</param>
		public void DequeueSampleFromRunningQueue(List<classSampleData> samples)
		{
			/// 
			/// For each sample to run, set the status and delay the run for 10 seconds
			///          
			List<classSampleData> validSamples = new List<classSampleData>();			
			foreach (classSampleData sample in samples)
			{
				classSampleData realSample = FindSample(sample.UniqueID);
				if (realSample == null)
				{
					/// 
					/// this sample does not exist on the sample queue! 
					/// 
					throw new NullReferenceException("This sample does not exist in the waiting queue! " + sample.DmsData.DatasetName);
				}
				mlist_runningQueue.Remove(realSample);
				mlist_waitingQueue.Insert(0, realSample);

				realSample.LCMethod		 = realSample.LCMethod.Clone() as classLCMethod;
				realSample.RunningStatus = enumSampleRunningStatus.Queued;								
				validSamples.Add(realSample);
			}

            if (SamplesWaitingToRun != null)
            {
                SamplesWaitingToRun(this,
                                    new classSampleQueueArgs(validSamples,
                                                                mint_nextAvailableSample,
                                                                mlist_runningQueue.Count,
                                                                mlist_completeQueue.Count,
                                                                mlist_waitingQueue.Count));
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

            /// 
            /// Find the sample provided to cancel.
            ///             
            classSampleData sample = FindSample(mlist_runningQueue, sampleData.UniqueID);
            if (sample == null)
            {
                string errorMessage = string.Format("The sample {0} was not found on the running Queue.",
                                                    sampleData);
                //throw new classSampleNotRunningException(errorMessage);
                return;
            }

            /// 
            /// Remove the sample from the running queue
            ///             
            mlist_runningQueue.Remove(sample);
            //enumSampleRunningStatus status = enumSampleRunningStatus.Stopped;
            //if (error)
            //{
            //    status = enumSampleRunningStatus.Error;
            //}
            sample.RunningStatus     = enumSampleRunningStatus.Stopped;  
            mint_nextAvailableSample = Math.Max(mint_nextAvailableSample - 1, 0);

            /// 
            /// Requeue the sample putting it back on the queue it came from.
            ///             
            mlist_completeQueue.Add(sample);

            classSampleData [] samples = new classSampleData[]   {  sample };
            classSampleQueueArgs args  = new classSampleQueueArgs(  samples,
                                                                    mint_nextAvailableSample, 
                                                                    mlist_runningQueue.Count,
                                                                    mlist_completeQueue.Count,
                                                                    mlist_waitingQueue.Count);

            if ((mint_nextAvailableSample == 0) && (mlist_runningQueue.Count == 0))
            {
                mbool_startedSamples = false;
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
            foreach (classSampleData sample in mlist_runningQueue)
                sample.RunningStatus = enumSampleRunningStatus.Queued;

            mlist_waitingQueue.InsertRange(0, mlist_runningQueue);
            mlist_runningQueue.Clear();
            mint_nextAvailableSample = 0;

            classSampleQueueArgs args = new classSampleQueueArgs(mlist_waitingQueue,
                                                                mint_nextAvailableSample,
                                                                mlist_runningQueue.Count,
                                                                mlist_completeQueue.Count,
                                                                mlist_waitingQueue.Count);
            mbool_startedSamples = false;
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
            /// uhhh this sample was null
            if (sampleData == null)
            {
                //TODO: Add error display.
                //throw new NullReferenceException("The sample that is said done was not an actual sample");
                return;
            }
            /// 
            /// Find the sample provided to complete.
            ///          
            classSampleData sample = FindSample(mlist_runningQueue, sampleData.UniqueID);
            if (sample == null)
            {
                string errorMessage = string.Format("The sample {0} was not found on the running Queue.",
                                                    sampleData);
                //TODO: BLL Removed because of the notification system.   throw new classSampleNotRunningException(errorMessage);
                return;
            }

            mlist_runningQueue.Remove(sample);
            mlist_completeQueue.Add(sample);

            /// 
            /// Moves the sample pointer backward to the front of the running queue.
            /// 
            mint_nextAvailableSample = Math.Max(mint_nextAvailableSample - 1, 0);
            sample.RunningStatus     = enumSampleRunningStatus.Complete;


            classSampleQueueArgs args = new classSampleQueueArgs(
                                                            new classSampleData[] { sample },
                                                            mint_nextAvailableSample,
                                                            mlist_runningQueue.Count,
                                                            mlist_completeQueue.Count,
                                                            mlist_waitingQueue.Count);

            if ((mint_nextAvailableSample == 0) && (mlist_runningQueue.Count == 0))
            {
                mbool_startedSamples = false;
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

            /// 
            /// Dequeue the sample, and start it.
            /// 
            if (mint_nextAvailableSample < mlist_runningQueue.Count && mbool_startedSamples)
            {
                sample               = mlist_runningQueue[mint_nextAvailableSample++];                                
                sample.RunningStatus = enumSampleRunningStatus.Running;

                if (SamplesStarted != null)
                {
                    SamplesStarted(this, new classSampleQueueArgs(new classSampleData[] { sample }));
                }
            }

            classSampleQueueArgs args = new classSampleQueueArgs(
                                                            new classSampleData[] { sample },
                                                            mint_nextAvailableSample,
                                                            mlist_runningQueue.Count,
                                                            mlist_completeQueue.Count,
                                                            mlist_waitingQueue.Count);			
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
            if (mint_nextAvailableSample < mlist_runningQueue.Count && mbool_startedSamples) 
            {
                sample = mlist_runningQueue[mint_nextAvailableSample];                
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
                List<string> cartNames          = classSQLiteTools.GetCartNameList(false);
                List<string> columnNames        = classSQLiteTools.GetColumnList(false);
                List<string> datasetNames       = classSQLiteTools.GetDatasetTypeList(false);
                List<classInstrumentInfo> instrumentList = classSQLiteTools.GetInstrumentList(false);
                List<string> separationTypes = classSQLiteTools.GetSepTypeList(false);                
                List<classUserInfo> userNames   = classSQLiteTools.GetUserList(false);
                string  separationDefault       = classSQLiteTools.GetDefaultSeparationType();

                classSQLiteTools.BuildConnectionString(true);

                classSQLiteTools.SaveInstListToCache(instrumentList);
                classSQLiteTools.SaveSelectedSeparationType(separationDefault);
                classSQLiteTools.SaveSingleColumnListToCache(columnNames, enumTableTypes.ColumnList);
                classSQLiteTools.SaveSingleColumnListToCache(datasetNames, enumTableTypes.DatasetTypeList);
                classSQLiteTools.SaveSingleColumnListToCache(separationTypes, enumTableTypes.SeparationTypeList);
                classSQLiteTools.SaveSingleColumnListToCache(cartNames, enumTableTypes.CartList);
                classSQLiteTools.SaveUserListToCache(userNames);    
                

            }
            classSQLiteTools.SaveQueueToCache(mlist_waitingQueue, enumTableTypes.WaitingQueue);
            classSQLiteTools.SaveQueueToCache(mlist_runningQueue, enumTableTypes.RunningQueue);
            classSQLiteTools.SaveQueueToCache(mlist_completeQueue, enumTableTypes.CompletedQueue);
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
          mlist_completeQueue = classSQLiteTools.GetQueueFromCache(enumTableTypes.CompletedQueue);          
          foreach (classSampleData sample in mlist_completeQueue)
          {
              sample.RunningStatus = enumSampleRunningStatus.Complete;
              mlist_uniqueID.Add(sample.UniqueID);

              if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
              {
                  sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
              }
              else
              {
                  sample.LCMethod = null;
              }

              if (sample.UniqueID >= mint_sampleIndex)
                  mint_sampleIndex = Convert.ToInt32(sample.UniqueID + 1);

              if (sample.SequenceID >= mint_sequenceIndex)
                  mint_sequenceIndex = Convert.ToInt32(sample.SequenceID + 1);
          }
          
          /// 
          /// Loads the samples and creates unique sequence ID's and unique id's
          /// 
          List<classSampleData> waitingSamples;
          waitingSamples = classSQLiteTools.GetQueueFromCache(enumTableTypes.WaitingQueue);                     

          /// 
          /// Update the Waiting Sample queue with the right LC-Methods.  This makes sure
          /// that we always have valid LC-methods.  Otherwise when we go to run the 
          /// scheduler and thus program will crash hard.
          /// 
          foreach (classSampleData sample in waitingSamples) // mlist_waitingQueue)
          {
              if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
              {
                  sample.LCMethod = classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
                  if (sample.LCMethod.Column >= 0)
                  {
                      // reset the column data.
                      classColumnData column = classCartConfiguration.Columns[sample.LCMethod.Column];
                      sample.ColumnData      = column;
                  }
              }
              else
              {
                  sample.LCMethod = null;
              }

              if (sample.UniqueID >= mint_sampleIndex)
                  mint_sampleIndex = Convert.ToInt32(sample.UniqueID);

              if (sample.SequenceID >= mint_sequenceIndex)
                  mint_sequenceIndex = Convert.ToInt32(sample.SequenceID + 1);
          }
          
          //ResetColumnData(true);

          /// 
          /// Put them on the waiting queue - we do it this way so that 
          /// we push stuff on the undo/redo stack.  That logic is already in place
          /// in the queue samples method.  We also see if no samples were added (updated == false)
          /// and completeQueue.length > 0 that we force an update.  Otherwise if no samples
          /// were left in the cache on the waiting queue but were on the complete queue, then
          /// we wouldnt see the completed samples...big bug.
          /// 
          bool updated = QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
          if (updated == false && mlist_completeQueue.Count > 0)
          {
              if (SamplesAdded != null)
              {
                  classSampleQueueArgs args = new classSampleQueueArgs(GetAllSamples());
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
            List<classSampleData> samples = new List<classSampleData>();
            if (includeRunning && mlist_runningQueue.Count > 0)
            {
                samples.AddRange(mlist_runningQueue);                
            }
            samples.AddRange(mlist_waitingQueue);
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

            /// 
            /// We need to assign the column data information to the samples
            /// since columns have not been assigned.         
            /// 
            int index = 0;
            if (mlist_waitingQueue.Count > 0)
            {
                index = mlist_columnOrders.IndexOf(mlist_waitingQueue[mlist_waitingQueue.Count - 1].ColumnData) + 1;
            }
            else
            {
                index = mlist_columnOrders.IndexOf(mlist_columnOrders[0]);
            }

            /// 
            /// For all the entries in the new list of samples...add some column information back into it.
            /// 
            //foreach (classSampleData data in waitingSamples)
            //{
            //    classColumnData columnData = mlist_columnOrders[index % mlist_columnOrders.Count];
            //    data.ColumnData            = columnData;
            //    index++;
            //}

            /// 
            /// Make sure the method references are created 
            /// 
            foreach (classSampleData sample in waitingSamples)
            {
                if (sample.LCMethod != null && classLCMethodManager.Manager.Methods.ContainsKey(sample.LCMethod.Name))
                {
                    sample.LCMethod     = classLCMethodManager.Manager.Methods[sample.LCMethod.Name].Clone() as classLCMethod;
                    int columnID        = sample.LCMethod.Column;
                    if (columnID > 0)
                    {
                        sample.ColumnData = classCartConfiguration.Columns[columnID];
                    }
                }
                sample.DmsData.CartName = classLCMSSettings.GetParameter("CartName");
                if (sample.UniqueID >= mint_sampleIndex)
                {
                    mint_sampleIndex = Convert.ToInt32(sample.UniqueID);
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

            /// 
            /// Here we need to assign the column data information to the samples
            /// if one has not already been assigned.
            /// 
            foreach (classSampleData data in waitingSamples)
            {
                data.DmsData.CartName = classLCMSSettings.GetParameter("CartName");
                data.ColumnData = column;
            }

            QueueSamples(waitingSamples, enumColumnDataHandling.LeaveAlone);
        }
 	    #endregion	 
    }
}

