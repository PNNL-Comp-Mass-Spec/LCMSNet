namespace LcmsNetData.Data
{
    /// <summary>
    /// The necessary set of data objects needed to store DMS requested run information
    /// </summary>
    public interface IRequestedRunData
    {
        /// <summary>
        /// Gets the list of data downloaded from DMS for this sample
        /// </summary>
        IDmsData DmsBasicData { get; }
    }

    /// <summary>
    /// The necessary set of data objects needed to store DMS requested run information,
    /// including the wellplate/vial tray and well/vial location of the sample in the requested run
    /// </summary>
    public interface IRequestedRunDataWithPalData : IRequestedRunData
    {
        /// <summary>
        /// Gets the pal data associated with this sample.
        /// </summary>
        IPalData PAL { get; set; }
    }
}
