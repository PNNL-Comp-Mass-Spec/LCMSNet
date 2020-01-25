namespace LcmsNetData.Data
{
    /// <summary>
    /// Interface for use by SQLiteTools to limit object types stored to a "Queue" cache
    /// </summary>
    public interface ISampleData : ITriggerFilePalData, IRequestedRunDataWithPalData
    {
        /// <summary>
        /// Name of operator (as entered in DMS)
        /// </summary>
        new string Operator { get; set; }

        /// <summary>
        /// Gets or sets the instrument method.
        /// </summary>
        string InstrumentMethod { get; set; }

        /// <summary>
        /// Gets or sets the volume of the sample to inject.
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        /// Get a new non-dummy instance
        /// </summary>
        /// <returns></returns>
        ISampleData GetNewNonDummy();
    }
}
