namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Logging levels enum
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// Errors causing program shutdown
        /// </summary>
        Fatal = 0,

        /// <summary>
        /// Errors that don't cause program shutdown, but are undesireable and probably should be shown to the user
        /// </summary>
        Error = 1,

        /// <summary>
        /// Warnings or anamolies in program execution that might be of interest
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Informational messages
        /// </summary>
        Info = 3,

        /// <summary>
        /// Info that will be useful when trying to debug an issue
        /// </summary>
        Debug = 4,

        /// <summary>
        /// Verbose info that may be useful in debugging
        /// </summary>
        Trace = 5,

        /// <summary>
        /// Doesn't really need to be logged
        /// </summary>
        ExcessivelyVerbose = 6,
    }
}