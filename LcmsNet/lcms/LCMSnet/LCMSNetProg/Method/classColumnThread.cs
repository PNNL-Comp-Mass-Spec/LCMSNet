/*********************************************************************************************************
 * Written by Dave Clark?, Brian LaMarche for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2009 Battle Memorial Institute
 *
 * Last Modified 9/16/2014 By Christopher Walters
 *********************************************************************************************************/

using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that handles execution of an LC method.
    /// </summary>
    public class classColumnThread
    {
        #region Members and Constants

        /// <summary>
        /// The ID of the column this thread is working on.
        /// </summary>
        private readonly int m_columnId;

        /// <summary>
        /// Object that contains method to run with parameters.
        /// </summary>
        private classSampleData m_sampleData;

        /// <summary>
        /// synchronization event used for classDeviceTimer
        /// </summary>
        private readonly ManualResetEvent m_abortEvent;

        /// <summary>
        /// reference to the background worker this column thread is run on. Needed so we can check for
        /// Cancellations and other such issues.
        /// </summary>
        private readonly BackgroundWorker mthread_worker;

        private const int CONST_VERBOSE_LEAST = 0;
        private const int CONST_VERBOSE_EVENTS = 1;

        #endregion

        #region Properties

        public string Name { get; set; }

        public classSampleData Sample => m_sampleData;

        public bool IsErrored { get; private set; }

        /// <summary>
        /// Gets or sets how verbose to make the debug output.
        /// </summary>
        public int VerboseLevel { get; set; }

        #endregion

        #region Class Methods

        public classColumnThread(int id, BackgroundWorker worker)
        {
            m_columnId = id;
            m_abortEvent = new ManualResetEvent(false);
            mthread_worker = worker;
            VerboseLevel = CONST_VERBOSE_EVENTS;
        }

        public void Initialize()
        {
            IsErrored = false;
        }

        /// <summary>
        /// Prints the debug message to the diagnostics debug output stream.
        /// </summary>
        /// <param name="message">Message to print.</param>
        /// <param name="level">Level of message</param>
        private void Print(string message, int level)
        {
            if (level <= VerboseLevel)
            {
                System.Diagnostics.Trace.WriteLine(message);
                System.Diagnostics.Trace.Flush();
            }
        }

        #endregion

        #region Method/Event Execution

        /// <summary>
        /// aborts current wait timer.
        /// </summary>
        public void Abort()
        {
            m_abortEvent.Set();
        }

        /// <summary>
        /// Executes a method for a given column.
        /// </summary>
        /// <param name="lcMethod">Method to execute</param>
        private bool ExecuteEvent(classLCEvent lcEvent)
        {
            var flag = true;
            lcEvent.Device.AbortEvent = m_abortEvent;
            try
            {
                var returnValue = lcEvent.Method.Invoke(lcEvent.Device, lcEvent.Parameters);
                if (returnValue != null && returnValue.GetType() == typeof (bool))
                    flag = (bool) returnValue;
                return flag;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "EVENT ERROR: " + ex.Message, ex);
                throw;
            }
        }

        public void ExecuteSample(object sender, DoWorkEventArgs e)
        {
            //Initialization
            m_abortEvent.Reset();
            var args = e.Argument as classColumnArgs;
            m_sampleData = args.Sample;
            var method = m_sampleData.LCMethod;
            var methodEvents = m_sampleData.LCMethod.Events;
            Exception ex = null;
            //We return the columnId as the "result" so the scheduler callback can determine which column events
            // are coming from.
            e.Result = m_columnId;

            try
            {
                // Main operation
                for (var eventNumber = 0; eventNumber < methodEvents.Count; eventNumber++)
                {
                    classLCEvent actualEvent;
                    var start = LcmsNetSDK.TimeKeeper.Instance.Now;
                    var finished = LcmsNetSDK.TimeKeeper.Instance.Now;
                    // before every event, check to see if we need to cancel operations
                    if (mthread_worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        //here we report progress, by notifying at the start of every new event.
                        var totalTimeInTicks = Convert.ToDecimal(method.End.Ticks);
                        var elapsedTimeInTicks = LcmsNetSDK.TimeKeeper.Instance.Now.Ticks - method.Start.Ticks;
                        var percentComplete =
                            (int)
                                Math.Round((elapsedTimeInTicks / totalTimeInTicks) * 100, MidpointRounding.AwayFromZero);
                        //We send percentage(of time) complete and state of the column, currently consisting
                        //of columnID, event number, and end time of the next event to the event handler
                        var state = new List<object>();
                        state.Add(m_columnId);
                        state.Add(eventNumber);
                        if (eventNumber <= methodEvents.Count - 1)
                        {
                            state.Add(methodEvents[eventNumber].End);
                        }
                        else
                        {
                            state.Add(DateTime.MinValue);
                        }
                        mthread_worker.ReportProgress(percentComplete, state);
                        method.CurrentEventNumber = eventNumber;
                        // Used for statistics
                        actualEvent = methodEvents[eventNumber].Clone() as classLCEvent;
                        actualEvent.Start = start;
                        actualEvent.Duration = new TimeSpan(0, 0, 0);
                        method.ActualEvents.Add(actualEvent);
                        var lcEvent = methodEvents[eventNumber];

                        // This determines when the next event should start.  Since the events are layed out in time
                        // we know it should start by the lcEvent.End date time value.  This helps correct for any
                        // straying events that may be a ms or two off.
                        var next = lcEvent.End;
                        var success = true;
                        try
                        {
                            var tempStatus = lcEvent.Device.Status;

                            lcEvent.Device.Status = enumDeviceStatus.InUseByMethod;
                            if (lcEvent.MethodAttribute.RequiresSampleInput)
                                lcEvent.Parameters[lcEvent.MethodAttribute.SampleParameterIndex] = m_sampleData;

                            // Try to execute the event, if it doesnt work, then we capture
                            // all relevant information and propogate it back out...At this time
                            // if an error occurs we are done executing on this column.
                            if (lcEvent.Duration.TotalMilliseconds > 1)
                                success = ExecuteEvent(lcEvent);
                            else
                                success = true;
                            lcEvent.Device.Status = tempStatus;
                        }
                        catch (Exception exThrown)
                        {
                            // Set the flags and variables so that we can say HEY!
                            // there was an error!
                            IsErrored = true;
                            ex = exThrown;
                            success = false;
                            finished = LcmsNetSDK.TimeKeeper.Instance.Now;
                            Print(
                                string.Format(
                                    "\t{0} COLUMN-{1} {5}.{4} EVENT TERMINATED an Exception was thrown: {2} Stack Trace:{3}",
                                    finished,
                                    m_columnId, // 1  COL ID
                                    exThrown.Message, // 2  Message
                                    exThrown.StackTrace, // 3  Stack Trace
                                    lcEvent.Name,
                                    lcEvent.Device.Name),
                                CONST_VERBOSE_EVENTS);
                            classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                                string.Format(
                                    "\t{0} COLUMN-{1} {5}.{4} EVENT TERMINATED an Exception was thrown: {2} Stack Trace:{3}",
                                    finished,
                                    m_columnId, // 1  COL ID
                                    exThrown.Message, // 2  Message
                                    exThrown.StackTrace, // 3  Stack Trace
                                    lcEvent.Name,
                                    lcEvent.Device.Name),
                                ex);
                        }

                        if (success)
                        {
                            actualEvent.HadError = false;
                            //
                            // Here we'll wait enough time so that
                            // we dont run the next event before its scheduled start.  This is flow control.
                            //
                            var timer = new classTimerDevice();
                            var span = next.Subtract(LcmsNetSDK.TimeKeeper.Instance.Now);
                            var totalMilliseconds = 0;
                            try
                            {
                                totalMilliseconds = Convert.ToInt32(span.TotalMilliseconds);
                            }
                            catch (OverflowException ex2)
                            {
                                classApplicationLogger.LogError(0, "TIMEROVERFLOW: " + ex2.Message, ex2);
                            }
                            if (totalMilliseconds > 2)
                            {
                                Print(string.Format("\t\t{0} COLUMN-{1} WAITING:{2}",
                                    finished,
                                    m_columnId, // 1  COL ID
                                    span.TotalMilliseconds),
                                    CONST_VERBOSE_EVENTS);
                                var timerStart = LcmsNetSDK.TimeKeeper.Instance.Now;
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WriteTimeoutLog), "timeout start: " + timerStart.ToString("h"));
                                timer.WaitMilliseconds(totalMilliseconds, m_abortEvent);
                                var timerEnd = LcmsNetSDK.TimeKeeper.Instance.Now.Ticks;
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(WriteTimeoutLog), "waitTimer end: " + timerEnd.ToString("h"));
                                // Calculate the statistics of how long it took to run.
                            }
                            finished = LcmsNetSDK.TimeKeeper.Instance.Now;
                        }
                        else if (!success)
                        {
                            //
                            // Well, we had an error (exception or expected) and we dont care why, we just want to
                            // gracefully notify people in charge, and exit.
                            //

                            lcEvent.Device.Status = enumDeviceStatus.Error;
                            m_sampleData = null;
                            if (ex == null)
                            {
                                ex = new Exception(string.Format("{0}.{1} failed.",
                                    lcEvent.Device.Name,
                                    lcEvent.Name));
                            }
                            // Calculate the statistics of how long it took to run.
                            finished = LcmsNetSDK.TimeKeeper.Instance.Now;
                            throw ex;
                        }
                        //method.ActualEvents.Add(actualEvent);
                    }
                    actualEvent.Duration = finished.Subtract(start);
                }
            }
            catch (Exception columnEx)
            {
                throw new classColumnException(m_columnId, columnEx);
            }
            //We may have finished the method, but if we were told to cancel between the time
            //we started the last event and now, we still have to die. Otherwise, we could cause
            //null reference exceptions in the scheduler due to how we have to handle the cancellation
            //process.
            if (mthread_worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void WriteTimeoutLog(object message)
        {
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message as string);
        }

        #endregion
    }
}