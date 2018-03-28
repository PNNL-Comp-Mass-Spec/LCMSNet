using System;
using LcmsNetSDK.Data;

namespace LcmsNet.Method
{
    /// <summary>
    /// Delegate defining how error information is passed to a listening object.
    /// </summary>
    /// <param name="sender">Scheduler sending data.</param>
    /// <param name="errorMessage">Error message found.</param>
    public delegate void DelegateError(object sender, classSampleData sample, string errorMessage);

    /// <summary>
    /// Delegate defining sample progress information.
    /// </summary>
    /// <param name="sender">Scheduler sending data.</param>
    /// <param name="args"></param>
    public delegate void DelegateSampleProgress(object sender, classSampleProgressEventArgs args);

    /// <summary>
    /// Class that defines the sample event arguments.
    /// </summary>
    public class classSampleProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message from the sender.</param>
        /// <param name="sample">Sample data whose progress is being reported for.</param>
        /// <param name="progressType"></param>
        public classSampleProgressEventArgs(string message, classSampleData sample, enumSampleProgress progressType)
        {
            Message = message;
            Sample = sample;
            ProgressType = progressType;
        }

        /// <summary>
        /// Gets or sets the sample whose progress has been made.
        /// </summary>
        public classSampleData Sample { get; set; }

        /// <summary>
        /// Gets or sets the message about the sample progress.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the progress event type.
        /// </summary>
        public enumSampleProgress ProgressType { get; set; }
    }

    /// <summary>
    /// Enumeration of types of sample progress that has been made.
    /// </summary>
    public enum enumSampleProgress
    {
        /// <summary>
        /// Indicates a sample run has finished, all LC-Events are complete.
        /// </summary>
        Complete,

        /// <summary>
        /// Indicates an error has occurred with an event.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates a LC-Event has completed, and is starting the next event.
        /// </summary>
        RunningNextEvent,

        /// <summary>
        /// Indicates a sample has started.
        /// </summary>
        Started,

        /// <summary>
        /// Indicates a sample was stopped.
        /// </summary>
        Stopped
    }
}