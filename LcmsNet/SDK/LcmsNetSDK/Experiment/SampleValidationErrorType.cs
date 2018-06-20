namespace LcmsNetSDK.Experiment
{
    /// <summary>
    /// Enumeration detailing how severe the validation error is.
    /// </summary>
    public enum SampleValidationErrorType
    {
        /// <summary>
        /// The dataset name is erroneous.  E.g. (Unused)
        /// </summary>
        DatasetNameError,

        /// <summary>
        /// The device does not exist in configuration.
        /// </summary>
        DeviceDoesNotExist,

        /// <summary>
        /// A device was in error.
        /// </summary>
        DeviceInError,

        /// <summary>
        /// The injection volume was out of range.
        /// </summary>
        InjectionVolumeOutOfRange,

        /// <summary>
        /// The LC Method was not selected.
        /// </summary>
        LCMethodNotSelected,

        /// <summary>
        /// The LC Method has no events defined.
        /// </summary>
        LCMethodHasNoEvents,

        /// <summary>
        /// The LC Method was not built correctly.
        /// </summary>
        LCMethodIncorrect,

        /// <summary>
        /// The PAL Method was not specified.
        /// </summary>
        PalMethodNotSpecified,

        /// <summary>
        /// The PAL Tray was not specified.
        /// </summary>
        PalTrayNotSpecified,

        /// <summary>
        /// The PAL well or vial was not specified.
        /// </summary>
        PalVialNotSpecified
    }
}