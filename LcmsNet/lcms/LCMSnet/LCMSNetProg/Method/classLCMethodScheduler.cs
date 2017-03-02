/*********************************************************************************************************
 * Written by Dave Clark?, Brian LaMarche, Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2009 Battle Memorial Institute
 *
 * Last Modified 9/30/2014 By Christopher Walters
 *********************************************************************************************************/

using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNet.SampleQueue;
using LcmsNet.SampleQueue.IO;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetSDK.Notifications;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that schedules and executes LC methods for given columns.
    /// </summary>
    public class classLCMethodScheduler : INotifier
    {
        private const string CONST_ERROR_STOPPED = "Samples Stopped Due to Error";


        private bool m_notifyOnKill;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classLCMethodScheduler(classSampleQueue sampleQueue)
        {
            m_columnThreads = new List<classColumnThread>();
            m_columnWorkers = new List<BackgroundWorker>();
            m_threadLocks = new List<object>();
            sampleEndTime = new DateTime[CONST_NUMBER_THREADS + 1];
            currentEvent = new int[CONST_NUMBER_THREADS + 1];
            samples = new classSampleData[CONST_NUMBER_THREADS + 1];

            for (var i = 0; i < CONST_NUMBER_THREADS + 1; i++)
            {
                m_columnWorkers.Add(new BackgroundWorker());
                m_columnThreads.Add(new classColumnThread(i, m_columnWorkers[i]));
                m_threadLocks.Add(new Object());
                m_columnWorkers[i].DoWork += m_columnThreads[i].ExecuteSample;
                m_columnWorkers[i].WorkerReportsProgress = true;
                m_columnWorkers[i].WorkerSupportsCancellation = true;
                m_columnWorkers[i].ProgressChanged += ColumnProgressChanged_Handler;
                m_columnWorkers[i].RunWorkerCompleted += ColumnWorkerComplete_Handler;
                if (i < CONST_NUMBER_THREADS)
                {
                    m_columnThreads[i].Name = "Column " + i;
                }
                else if (i == CONST_NUMBER_THREADS)
                {
                    m_columnThreads[i].Name = "Special";
                }
            }
            Name = "Scheduler";

            //
            // worker thread.
            //
            m_thread = null;
            m_stopping = false;

            m_notifyOnKill = false;

            //
            // Reference to the sample queue so that we can grab samples
            // when we need them.
            //
            m_sampleQueue = sampleQueue;

            // Register yourself with the notification system.
            Notification.NotificationBroadcaster.Manager.AddNotifier(this);

            //
            // This is the event to tell the scheduler to shut everything down!
            //
            m_abortWaiting = new ManualResetEvent(false);
            m_stoppedSamples = new ManualResetEvent(false);
            m_stopSamples = new ManualResetEvent(false);
        }

        #region Constants

        /// <summary>
        /// The number of events the scheduler uses to synchronize with other objects besides the column threads.
        /// </summary>
        private const int CONST_NUMBER_OF_SCHEDULER_EVENTS = 3;

        /// <summary>
        /// Flag indicating the column is IDLE
        /// </summary>
        private const int CONST_IDLE_FLAG = -1;

        /// <summary>
        /// Flag indicating the column has been told to cancel current operations.
        /// </summary>
        private const int CONST_CANCELLING_FLAG = -2;

        /// <summary>
        /// How long to wait for an event trigger before waking to perform default activities.
        /// </summary>
        private const int CONST_THREAD_WAITTIMEOUT_MS = 100;

        /// <summary>
        /// The number of threads that are available.
        /// </summary>
        private const int CONST_NUMBER_THREADS = 4;

        /// <summary>
        /// Event number when a sample is queued.
        /// </summary>
        private const int CONST_EVENT_NUM_STOP_SAMPLES = 1;

        /// <summary>
        /// Total seconds an event is allowed to go over before terminating the associated method.
        /// </summary>
        private const int CONST_OVER_EVENT_TIME_LIMIT_SECONDS = 1;

        /// <summary>
        /// Prints all events to the trace.
        /// </summary>
        private const int CONST_VERBOSE_EVENTS = 1;

        /// <summary>
        /// Prints only the most important messages to the trace.
        /// </summary>
        private const int CONST_VERBOSE_LEAST = 0;

        private const int CONST_COLUMN_DISPLAY_ADJUSTMENT = 1;

        #endregion

        #region Members

        private bool m_stopping;

        /// <summary>
        /// Flag indicating if methods are running.
        /// </summary>
        private volatile bool m_isRunning;

        /// <summary>
        /// Scheduling thread reference.
        /// </summary>
        private Thread m_thread;

        /// <summary>
        /// Sample queue that handles sorting of the samples to run.
        /// </summary>
        private readonly classSampleQueue m_sampleQueue;

        /// <summary>
        /// Event that tells the background worker thread that its time to stop
        /// waiting and shutdown.
        /// </summary>
        private readonly ManualResetEvent m_abortWaiting;

        /// <summary>
        /// Event that tells the background worker thread that its time to stop running.
        /// </summary>
        private readonly ManualResetEvent m_stopSamples;

        /// <summary>
        /// Event that tells the main thread things are ok and stopped.
        /// </summary>
        private readonly ManualResetEvent m_stoppedSamples;

        //
        // Here we hold a copy of the samples that are running, so we can do things like
        // report status and interrogate when things should be done, without having
        // to use locks or critical sections that will chew up time
        // spent by the column thread actually running the methods.
        //
        // In a sense we hide latency of running the experiment here, in the scheduler.
        // We do all the administrative tasks, and let the column thread just set
        // threaded events.
        //
        // A note: The above is no longer strictly true. The code for handling the administrative tasks does indeed
        // reside here in the scheduler, but it is in the form of callbacks to events that are run on the column
        // threads themselves. This is a more reliable way to synchronize the scheduler with the columns, since
        // state updates to the scheduler occur as they happen on the columns, instead of waiting for the
        // scheduler thread to get around to dealing with the report.
        //
        classSampleData[] samples;
        //
        // These two arrays tell us when the sample should finish
        // and what event of the LC method the current column thread is on.
        //
        DateTime[] sampleEndTime;
        int[] currentEvent;

        /// <summary>
        /// List of backgroundworker objects that will asynchronously run the columnThreads
        /// </summary>
        private readonly List<BackgroundWorker> m_columnWorkers;

        /// <summary>
        /// List of column threads that are to be used.
        /// </summary>
        private readonly List<classColumnThread> m_columnThreads;

        /// <summary>
        /// List of objects used as locks used for mutual exclusion
        /// </summary>
        private readonly List<object> m_threadLocks;

        #endregion

        #region Events

        /// <summary>
        /// Fired when an error occurs on a column.
        /// </summary>
        public event DelegateError SchedulerError;

        /// <summary>
        /// Fired when there is sample progress on a column, such as a new event is executed.
        /// </summary>
        public event DelegateSampleProgress SampleProgress;

        #endregion

        #region Methods

        /// <summary>
        /// Kills the scheduler thread.
        /// </summary>
        private void Abort()
        {
            if (m_thread != null)
            {
                try
                {
                    //
                    // Turn off the flag so the worker thread knows
                    // to stop doing what its doing when the flag is set to false
                    //
                    m_isRunning = false;
                    m_thread.Abort();
                }
                catch (ThreadAbortException)
                {
                }
                finally
                {
                    m_thread = null;
                }
            }
        }

        /// <summary>
        /// Starts the scheduler, by first aborting any previously running instances of its child threads.
        /// </summary>
        public void Initialize()
        {
            Shutdown();
            var start = new ThreadStart(RunThreaded);
            m_isRunning = true;
            m_thread = new Thread(start);
            m_thread.Start();
        }

        /// <summary>
        /// Shuts down the scheduler and stops any LC methods.
        /// </summary>
        public void Shutdown()
        {
            StopSamples();
            Abort();
        }

        /// <summary>
        /// Signals the scheduler to stop running samples.
        /// </summary>
        public void Stop()
        {
            m_stopping = true;

            // Tell the scheduler thread to stop
            m_stopSamples.Set();

            // Wait for it to tell us its done
            m_stoppedSamples.WaitOne();

            m_sampleQueue.StopRunningQueue();

            // Then reset the event so it can tell us again later it's done.
            m_stoppedSamples.Reset();

            m_stopping = false;
        }

        private void StopAllOnOverdue()
        {
            m_stopping = true;
            StopSamples();
            m_sampleQueue.StopRunningQueue();
            m_stopping = false;
        }

        /// <summary>
        /// Stops any running samples on all column threads but does not terminate them.
        /// </summary>
        private void StopSamples()
        {
            for (var i = 0; i < m_columnThreads.Count; i++)
            {
                try
                {
                    Monitor.Enter(m_threadLocks[i]); // since we are shutting down, we want to block here.
                    if (samples[i] != null)
                    {
                        //
                        // Tell the column thread to die..
                        //

                        Print("Killing column: " + i.ToString(), CONST_VERBOSE_EVENTS);
                        if (m_columnWorkers[i].IsBusy)
                        {
                            sampleEndTime[i] = DateTime.MinValue;
                            currentEvent[i] = CONST_CANCELLING_FLAG;
                            m_columnThreads[i].Abort();
                            m_columnWorkers[i].CancelAsync();
                        }

                        //
                        // Tell the sample queue to stop doing what it is doing
                        //
                        m_sampleQueue.CancelRunningSample(samples[i], false);

                        //
                        // Don't hold a reference to the sample if we dont need it.
                        //
                        samples[i] = null;
                    }
                }
                catch (Exception)
                {
                    Print("Error occured while shutting down column" + i.ToString(), CONST_VERBOSE_LEAST);
                }
                finally
                {
                    Monitor.Exit(m_threadLocks[i]);
                }
            }
        }


        /// <summary>
        /// Prints the debug message to the diagnostics debug output stream.
        /// </summary>
        /// <param name="message">Message to print.</param>
        /// <param name="level">Level of message</param>
        private void Print(string message, int level, Exception ex = null, classSampleData sample = null)
        {
            if (level <= VerboseLevel)
            {
                System.Diagnostics.Trace.WriteLine(message);
                System.Diagnostics.Trace.Flush();
                Exception exVal = null;
                var error = ex != null;
                classSampleData sampVal = null;
                if (Logger != null)
                {
                    if (ex != null)
                    {
                        exVal = ex;
                    }
                    if (sample != null)
                    {
                        sampVal = sample;
                    }
                    if (!error)
                    {
                        Logger.LogMessage(level, message, sampVal);
                    }
                    else
                    {
                        Logger.LogError(level, message, exVal, sampVal);
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets how verbose to make the debug output.
        /// </summary>
        public int VerboseLevel { get; set; }

        /// <summary>
        /// Gets or sets the logging object.
        /// </summary>
        public LcmsNetDataClasses.Logging.ILogger Logger { get; set; }

        #endregion

        #region Main Thread and Background worker

        /// <summary>
        /// Writes the method data for a completed sample run.
        /// </summary>
        /// <param name="sample">Sample containing data to write.</param>
        private static void WriteCompletedSampleInformation(object sample)
        {
            //Write methods files
            classMethodFileTools.WriteMethodFiles((classSampleData) sample);
        }

        private static void WriteIncompleteSampleInformation(object sample)
        {
            classMethodFileTools.WriteIncompleteMethodFiles((classSampleData) sample);
        }

        /// <summary>
        /// Alert listeners that an error has occurred.
        /// </summary>
        /// <param name="errorMessage">Error message to report.</param>
        private void HandleError(classSampleData sample, string errorMessage)
        {
            SchedulerError?.Invoke(this, sample, errorMessage);
        }

        /// <summary>
        /// Gets the next sample and runs it on the column specified
        /// </summary>
        private void RunNextSample(ref classSampleData[] samples, ref DateTime[] sampleEndTime,
            ref int[] currentEvent)
        {
            var sampleColumnID = 0;
            classSampleData data = null;

            data = m_sampleQueue.NextSampleQuery();

            //
            // See if the sample is legit, then if we have an idle column.
            // We also need to check if the column is not disabled.
            //
            if (data == null)
            {
                return;
            }

            sampleColumnID = data.ColumnData.ID;

            // Make sure we don't need to put this on the last column.
            if (data.LCMethod.IsSpecialMethod)
            {
                sampleColumnID = CONST_NUMBER_THREADS;
            }
            if (!m_columnWorkers[sampleColumnID].IsBusy)
            {
                //
                // Can we start this guy? make sure he starts after some time.
                //
                //TimeSpan startSpan = LcmsNetSDK.TimeKeeper.Instance.Now.Subtract(data.LCMethod.Start);

                //
                // If we have a positive integer, then that means the lcmethod's start time is good enough
                // basically, we want to ensure that a method doesn't start before it's expected start time
                // has occurred.
                //
                if (LcmsNetSDK.TimeKeeper.Instance.Now.CompareTo(data.LCMethod.Start) >= 0)
                    //(startSpan.Milliseconds >= 0)
                {
                    data = m_sampleQueue.NextSampleStart();
                    Print(string.Format("START SAMPLE = {0} \t COLUMN = {1}, EXPECTED START = {2}",
                        data.DmsData.DatasetName,
                        data.ColumnData.ID + 1, data.LCMethod.Start),
                        CONST_VERBOSE_LEAST, null, data);
                    //
                    // Hold on to a copy of the sample
                    //

                    if (data.LCMethod.Events.Count > 0)
                        sampleEndTime[sampleColumnID] = data.LCMethod.Events[0].End;
                    else
#if DEBUG
                        sampleEndTime[sampleColumnID] = LcmsNetSDK.TimeKeeper.Instance.Now.AddSeconds(5);
#else
                        sampleEndTime[sampleColumnID] = LcmsNetSDK.TimeKeeper.Instance.Now;
#endif

                    currentEvent[sampleColumnID] = 0;
                    data.LCMethod.CurrentEventNumber = 0;
                    data.LCMethod.ActualEnd = DateTime.MaxValue;
                    data.LCMethod.ActualStart = LcmsNetSDK.TimeKeeper.Instance.Now;
                    samples[sampleColumnID] = data.Clone() as classSampleData;
                    samples[sampleColumnID].LCMethod = data.LCMethod.Clone() as classLCMethod;
                    m_columnWorkers[sampleColumnID].RunWorkerAsync(new classColumnArgs(data));
                }
            }
        }

        /// <summary>
        /// Method that queries all of the schedulers for status and schedules
        /// work to be performed.
        /// </summary>
        private void RunThreaded()
        {
            Thread.CurrentThread.Name = "Scheduler Thread";
            //
            // A list of events to listen for
            var events = new WaitHandle[CONST_NUMBER_OF_SCHEDULER_EVENTS];

            //
            // Setup the event table
            //
            var j = 0;
            events[j++] = m_abortWaiting;
            events[j++] = m_stopSamples;
            events[j++] = m_sampleQueue.SampleQueuedEvent;

            for (var i = 0; i < m_columnThreads.Count; i++)
            {
                sampleEndTime[i] = DateTime.MinValue;
                currentEvent[i] = CONST_IDLE_FLAG;
                m_columnThreads[i].Initialize();
            }

            //
            // This is the meat of it, here is where we manage the column threads and send them
            // samples to run.  Here is also where we do things like monitor the thread to make
            // sure its not going to timeout or run over its allocated time.
            //
            while (m_isRunning == true)
            {
                //
                // Wait to be told to stop, if not told to stop within 100ms,
                // then check to see if a method is overdue or a column is ready for another sample.
                //
                var eventNumber = WaitHandle.WaitAny(events, CONST_THREAD_WAITTIMEOUT_MS);


                //
                // Temporary variables for setting up a new sample on a column thread.
                //
                var columnID = 0;
                var timeElapsedOverdue = 0.0; // variable to see if a sample's event has gone past due.

                switch (eventNumber)
                {
                    //
                    // Stops samples from being run.
                    //
                    case CONST_EVENT_NUM_STOP_SAMPLES:
                        m_stopSamples.Reset();
                        Print("Killing all samples on all columns.", CONST_VERBOSE_EVENTS);
                        StopSamples();
                        if (m_notifyOnKill)
                        {
                            m_sampleQueue.StopRunningQueue();
                            m_notifyOnKill = false;
                            StatusUpdate?.Invoke(this,
    new LcmsNetDataClasses.Devices.classDeviceStatusEventArgs(
        LcmsNetDataClasses.Devices.enumDeviceStatus.Error, CONST_ERROR_STOPPED, this));
                        }
                        Print("Done killing columns.  ", CONST_VERBOSE_EVENTS);
                        m_stoppedSamples.Set();
                        break;
                    default:
                        //
                        // Figure out if it's worth looking at the next sample.  This is better
                        // than acquiring locks and being turned down later because the state of a
                        // column is running or disabled.
                        //
                        var now = LcmsNetSDK.TimeKeeper.Instance.Now;
                        for (columnID = 0; columnID < currentEvent.Length; columnID++)
                        {
                            var lockTaken = false;
                            Monitor.TryEnter(m_threadLocks[columnID], 1, ref lockTaken);
                                //if we can't get the lock, it's because the column is changing state, so just move on to the next column and check again next time the scheduler takes the default action.
                            if (lockTaken)
                            {
                                try
                                {
                                    //
                                    // Test to see if the sample has gone past due
                                    //
                                    if (sampleEndTime[columnID] != DateTime.MinValue)
                                    {
                                        var overdueSpan = now.Subtract(sampleEndTime[columnID]);
                                        timeElapsedOverdue = overdueSpan.TotalSeconds;
                                        if (Convert.ToInt32(Math.Floor(timeElapsedOverdue)) >=
                                            CONST_OVER_EVENT_TIME_LIMIT_SECONDS)
                                        {
                                            //
                                            // Here we shut down the method killing any execution of it.
                                            // We do our state-cleanup here because the BackgroundWorker
                                            // doesn't allow us to return the necessary information to
                                            // do so to the ColumnWorkerComplete_Handler event handler.
                                            //
                                            m_columnThreads[columnID].Abort();
                                            m_columnWorkers[columnID].CancelAsync();

                                            String deviceName;
                                            String eventName;
                                            TimeSpan eventDuration;
                                            if (currentEvent[columnID] < samples[columnID].LCMethod.Events.Count)
                                            {
                                                var lcEvent =
                                                    samples[columnID].LCMethod.Events[currentEvent[columnID]];
                                                deviceName = lcEvent.Device.Name;
                                                eventName = lcEvent.Name;
                                                eventDuration = lcEvent.Duration;
                                            }
                                            else
                                            {
                                                deviceName = "Unknown Device";
                                                eventName = "Unknown Event";
                                                eventDuration = new TimeSpan(0, 0, 0);
                                            }
                                            var message = string.Format(
                                                "\tCOLUMN-{0} did not finish. Device: {2}, Event: {3}, Expected End Time: {1}, Late by {4:F2} seconds; Stopping all samples",
                                                columnID + CONST_COLUMN_DISPLAY_ADJUSTMENT,
                                                sampleEndTime[columnID],
                                                deviceName,
                                                eventName,
                                                timeElapsedOverdue);
                                            //Print(message, CONST_VERBOSE_LEAST, null, samples[columnID]);
                                            HandleError(samples[columnID], message);
                                            sampleEndTime[columnID] = DateTime.MinValue;

                                            if (currentEvent[columnID] <
                                                m_columnThreads[columnID].Sample.LCMethod.ActualEvents.Count)
                                            {
                                                var evt =
                                                    m_columnThreads[columnID].Sample.LCMethod.ActualEvents[
                                                        currentEvent[columnID]];
                                                evt.Duration =
                                                    evt.Start.Add(eventDuration).Add(overdueSpan).Subtract(evt.Start);
                                            }

                                            currentEvent[columnID] = CONST_CANCELLING_FLAG;
                                            m_sampleQueue.CancelRunningSample(samples[columnID], true);
                                            samples[columnID] = null;
                                            StopAllOnOverdue();
                                            ThreadPool.QueueUserWorkItem(WriteIncompleteSampleInformation,
                                                m_columnThreads[columnID].Sample);
                                        }
                                    }
                                    //
                                    // We dont only look for == IDLE so that we pull off the
                                    // dead samples if the column is disabled.
                                    //
                                    else if (currentEvent[columnID] == CONST_IDLE_FLAG && !m_stopping)
                                    {
                                        RunNextSample(ref samples, ref sampleEndTime, ref currentEvent);

                                        if (samples[columnID] != null && SampleProgress != null)
                                        {
                                            SampleProgress(this,
                                                new classSampleProgressEventArgs(
                                                    "LC-Method Started",
                                                    samples[columnID],
                                                    enumSampleProgress.Started));
                                        }
                                    }
                                }
                                finally
                                {
                                    if (lockTaken)
                                    {
                                        Monitor.Exit(m_threadLocks[columnID]);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        #endregion

        #region INotifier Members

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() {CONST_ERROR_STOPPED};
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public string Name { get; set; }

        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        /// <remarks>This event is required by IDevice but this class does not use it</remarks>
        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }

        #endregion

        #region Column BackgroundWorker Event Handlers

        public void ColumnProgressChanged_Handler(object sender, ProgressChangedEventArgs e)
        {
            var columnState = e.UserState as List<Object>;
            var columnID = (int) columnState[0];
            var columnCurrentEvent = (int) columnState[1];
            var columnEndOfCurrentEvent = (DateTime) columnState[2];
            lock (m_threadLocks[columnID]) //We want to block, so as to make sure this is done.
            {
                if (samples[columnID] != null && columnCurrentEvent >= 0)
                {
                    //
                    // Make sure we have another event to process.
                    //
                    if (samples[columnID].LCMethod.Events.Count > columnCurrentEvent)
                    {
                        //
                        // Now update the expected end time for the current event.
                        //
                        sampleEndTime[columnID] = columnEndOfCurrentEvent;
                        samples[columnID].LCMethod.CurrentEventNumber = columnCurrentEvent;

                        SampleProgress?.Invoke(this,
    new classSampleProgressEventArgs(
        "LC-Event Started",
        samples[columnID],
        enumSampleProgress.RunningNextEvent));
                        currentEvent[columnID] = columnCurrentEvent;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the backgrounder worker running the column thread is done,
        /// via error, cancellation, or completing a method.
        /// </summary>
        /// <param name="sender">the background worker</param>
        /// <param name="e">runworkercompletedevent args</param>
        public void ColumnWorkerComplete_Handler(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                var workerIndex = m_columnWorkers.FindIndex(x => x.Equals(sender));
                if (workerIndex == -1)
                {
                    //Note: This shouldn't be possible, this is here just in case something funky happens.
                    Stop();
                    HandleError(new classSampleData(), "Report by Unknown column worker");
                }
                lock (m_threadLocks[workerIndex])
                {
                    //let the scheduler know the backgroundworker has finished cancelling and is ready to go!
                    currentEvent[workerIndex] = CONST_IDLE_FLAG;
                }
            }
            else if (e.Error != null)
            {
                var ex = e.Error as classColumnException;
                var columnID = ex.ColumnID;
                lock (m_threadLocks[columnID]) //We want to block, so as to make sure this is done.
                {
                    var EVENT_ADJUST = 1;
                    var currentEventNumber = currentEvent[columnID] + EVENT_ADJUST <
                                             samples[columnID].LCMethod.Events.Count
                        ? currentEvent[columnID] + EVENT_ADJUST
                        : currentEvent[columnID];
                    // we use currentEvent[columnID] + EVENT_ADJUST here as the scheduler still thinks we're on the old event.
                    var stackTrace = UnwrapException(ex);
                    HandleError(samples[columnID],
                        string.Format(
                            "Column {0}: Method {1} of sample {2} had an error running event {3} on device {4} Stack Trace: {5}",
                            columnID + CONST_COLUMN_DISPLAY_ADJUSTMENT,
                            samples[columnID].LCMethod.Name,
                            samples[columnID].DmsData.DatasetName,
                            samples[columnID].LCMethod.Events[currentEventNumber].Name,
                            samples[columnID].LCMethod.Events[currentEventNumber].Device.Name,
                            stackTrace));

                    m_sampleQueue.CancelRunningSample(samples[columnID], true);
                    samples[columnID] = null;
                    sampleEndTime[columnID] = DateTime.MinValue;
                    currentEvent[columnID] = CONST_IDLE_FLAG;
                    //Print("ERROR", CONST_VERBOSE_LEAST);
                    ThreadPool.QueueUserWorkItem(WriteIncompleteSampleInformation, m_columnThreads[columnID].Sample);
                }
                Stop();
            }
            else
            {
                var columnID = (int) e.Result;
                //Separation completed.
                lock (m_threadLocks[columnID]) //We want to block, so as to make sure this is done.
                {
                    var noww = LcmsNetSDK.TimeKeeper.Instance.Now;

                    String datasetName;
                    if (samples[columnID] != null && samples[columnID].DmsData != null)
                        datasetName = samples[columnID].DmsData.DatasetName;
                    else
                        datasetName = "Unknown_Dataset since samples[columnID] is null for columnID = " + columnID;

                    var mm = string.Format("END SAMPLE {2} on COLUMN-{0} COMPLETED at {1} ",
                        columnID + CONST_COLUMN_DISPLAY_ADJUSTMENT,
                        sampleEndTime[columnID],
                        datasetName);
                    Print(mm, CONST_VERBOSE_LEAST, null, samples[columnID]);
                    sampleEndTime[columnID] = DateTime.MinValue;
                    currentEvent[columnID] = CONST_IDLE_FLAG;
                    SampleProgress?.Invoke(this,
    new classSampleProgressEventArgs(
        "LC-Method Completed",
        samples[columnID],
        enumSampleProgress.Complete));
                    m_sampleQueue.FinishSampleRun(samples[columnID]); // Then tell the sample queue that we are done!
                    //
                    // Write the trigger file and other data in a separate thread. I/O is expensive and we don't
                    // want to bog down time critical functions waiting on it. So lets toss it in a threadpool thread.
                    //
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WriteCompletedSampleInformation),
                        m_columnThreads[columnID].Sample);
                    samples[columnID] = null;
                }
            }
        }

        private string UnwrapException(Exception ex)
        {
            var returnVal = ex.Message + " " + ex.StackTrace;
            if (ex.InnerException != null)
            {
                returnVal += UnwrapException(ex.InnerException);
            }
            return returnVal;
        }

        #endregion
    }
}