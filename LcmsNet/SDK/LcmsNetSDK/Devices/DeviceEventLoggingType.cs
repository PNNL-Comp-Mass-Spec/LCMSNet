namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Enum to specify how a status or error event should be logged
    /// </summary>
    public enum DeviceEventLoggingType
    {
        /// <summary>
        /// Do not log the event
        /// </summary>
        None = 0,

        /// <summary>
        /// Log the event to the default log for the event type
        /// </summary>
        Default = 1,

        /// <summary>
        /// Log the event to the message log
        /// </summary>
        Message = 2,

        /// <summary>
        /// Log the event to the error log
        /// </summary>
        Error = 3,
    }
}
