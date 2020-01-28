namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Enumeration describing the status of a sample.
    /// </summary>
    public enum SampleRunningStatus
    {
        /// <summary>
        /// Queued but not told to execute.
        /// </summary>
        Queued,

        /// <summary>
        /// Stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Waiting to run.
        /// </summary>
        WaitingToRun,

        /// <summary>
        /// Sample is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// Sample successfully finished running.
        /// </summary>
        Complete,

        /// <summary>
        /// Error occurred during the run.
        /// </summary>
        Error
    }
}
