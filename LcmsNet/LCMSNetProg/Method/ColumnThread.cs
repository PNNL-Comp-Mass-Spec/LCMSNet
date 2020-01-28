using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using LcmsNetData.Logging;
using LcmsNetData.System;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that handles execution of an LC method.
    /// </summary>
    public class ColumnThread
    {
        #region Members and Constants

        /// <summary>
        /// The ID of the column this thread is working on.
        /// </summary>
        private readonly int m_columnId;

        /// <summary>
        /// Object that contains method to run with parameters.
        /// </summary>
        private SampleData m_sampleData;

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

        public SampleData Sample => m_sampleData;

        public bool IsErrored { get; private set; }

        /// <summary>
        /// Gets or sets how verbose to make the debug output.
        /// </summary>
        public int VerboseLevel { get; set; }

        #endregion

        #region Class Methods

        public ColumnThread(int id, BackgroundWorker worker)
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
                Trace.WriteLine(message);
                Trace.Flush();
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
        /// <param name="lcEvent">Event to execute</param>
        private bool ExecuteEvent(LCEvent lcEvent)
        {
            lcEvent.Device.AbortEvent = m_abortEvent;
            try
            {
                var returnValue = lcEvent.Method.Invoke(lcEvent.Device, lcEvent.Parameters);
                if (returnValue is bool)
                    return (bool) returnValue;

                return true;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "EVENT ERROR: " + ex.Message, ex);
                throw;
            }
        }

        public void ExecuteSample(object sender, DoWorkEventArgs e)
        {
            //Initialization
            m_abortEvent.Reset();

            var args = e.Argument as ColumnArgs;
            if (args != null)
                m_sampleData = args.Sample;

            var method = m_sampleData.ActualLCMethod;
            var methodEvents = m_sampleData.ActualLCMethod.Events;
            Exception ex = null;

            //We return the columnId as the "result" so the scheduler callback can determine which column events
            // are coming from.
            e.Result = m_columnId;

            try
            {
                // Main operation
                for (var eventNumber = 0; eventNumber < methodEvents.Count; eventNumber++)
                {
                    var start = TimeKeeper.Instance.Now;
                    var finished = TimeKeeper.Instance.Now;

                    // before every event, check to see if we need to cancel operations
                    if (mthread_worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    //here we report progress, by notifying at the start of every new event.
                    var totalTimeInTicks = Convert.ToDecimal(method.End.Ticks);
                    var elapsedTimeInTicks = TimeKeeper.Instance.Now.Ticks - method.Start.Ticks;
                    var percentComplete =
                        (int)Math.Round(elapsedTimeInTicks / totalTimeInTicks * 100, MidpointRounding.AwayFromZero);

                    //We send percentage(of time) complete and state of the column, currently consisting
                    //of columnID, event number, and end time of the next event to the event handler
                    var state = new List<object> {
                        m_columnId,
                        eventNumber
                    };

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
                    var actualEvent = methodEvents[eventNumber].Clone() as LCEvent;
                    actualEvent.Start = start;
                    actualEvent.Duration = new TimeSpan(0, 0, 0);
                    method.ActualEvents.Add(actualEvent);
                    var lcEvent = methodEvents[eventNumber];

                    // This determines when the next event should start.  Since the events are laid out in time
                    // we know it should start by the lcEvent.End date time value.  This helps correct for any
                    // straying events that may be a ms or two off.
                    var next = lcEvent.End;
                    var success = true;
                    try
                    {
                        var tempStatus = lcEvent.Device.Status;

                        lcEvent.Device.Status = DeviceStatus.InUseByMethod;
                        if (lcEvent.MethodAttribute.RequiresSampleInput)
                            lcEvent.Parameters[lcEvent.MethodAttribute.SampleParameterIndex] = m_sampleData;

                        // Try to execute the event, if it doesn't work, then we capture
                        // all relevant information and propagate it back out...At this time
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
                        finished = TimeKeeper.Instance.Now;
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
                        ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
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
                        // we don't run the next event before its scheduled start.  This is flow control.
                        //
                        var timer = new TimerDevice();
                        var span = next.Subtract(TimeKeeper.Instance.Now);
                        var totalMilliseconds = 0;
                        try
                        {
                            totalMilliseconds = Convert.ToInt32(span.TotalMilliseconds);
                        }
                        catch (OverflowException ex2)
                        {
                            ApplicationLogger.LogError(0, "TIMEROVERFLOW: " + ex2.Message, ex2);
                        }
                        if (totalMilliseconds > 2)
                        {
                            Print(string.Format("\t\t{0} COLUMN-{1} WAITING:{2}",
                                                finished,
                                                m_columnId, // 1  COL ID
                                                span.TotalMilliseconds),
                                  CONST_VERBOSE_EVENTS);
                            var timerStart = TimeKeeper.Instance.Now;
                            //System.Threading.Tasks.Task.Run(() => WriteTimeoutLog("timeout start: " + timerStart.ToString("h")));
                            timer.WaitMilliseconds(totalMilliseconds, m_abortEvent);
                            var timerEnd = TimeKeeper.Instance.Now.Ticks;
                            //System.Threading.Tasks.Task.Run(() => WriteTimeoutLog("waitTimer end: " + timerEnd.ToString("h")));
                            // Calculate the statistics of how long it took to run.
                        }
                        finished = TimeKeeper.Instance.Now;
                    }
                    else
                    {
                        //
                        // Well, we had an error (exception or expected) and we don't care why, we just want to
                        // gracefully notify people in charge, and exit.
                        //

                        lcEvent.Device.Status = DeviceStatus.Error;
                        m_sampleData = null;
                        if (ex == null)
                        {
                            ex = new Exception(string.Format("{0}.{1} failed.",
                                                             lcEvent.Device.Name,
                                                             lcEvent.Name));
                        }
                        // Calculate the statistics of how long it took to run.
                        finished = TimeKeeper.Instance.Now;
                        throw ex;
                    }
                    //method.ActualEvents.Add(actualEvent);
                    actualEvent.Duration = finished.Subtract(start);
                }
            }
            catch (Exception columnEx)
            {
                throw new ColumnException(m_columnId, columnEx);
            }

            if (method.ActualEnd > TimeKeeper.Instance.Now)
            {
                // Set the actual end time
                method.ActualEnd = TimeKeeper.Instance.Now;
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

        private void WriteTimeoutLog(string message)
        {
            ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, message);
        }

        #endregion
    }
}