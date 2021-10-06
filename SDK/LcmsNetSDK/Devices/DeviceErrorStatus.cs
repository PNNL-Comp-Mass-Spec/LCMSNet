namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Enumeration describing errors that can occur on the carts.
    /// </summary>
    public enum DeviceErrorStatus
    {
        /// <summary>
        /// No error occurs
        /// </summary>
        NoError,

        /// <summary>
        /// There was an error but it only affects this sample.
        /// </summary>
        ErrorSampleOnly,

        /// <summary>
        /// There was an error and it affects all samples on all columns.
        /// </summary>
        ErrorAffectsAllColumns,

        /// <summary>
        /// There was an error and it affects only this column.
        /// </summary>
        ErrorAffectsThisColumn
    }
}