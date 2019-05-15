using System;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Provides data for creating trigger files
    /// </summary>
    public interface ITriggerFileData
    {
        /// <summary>
        /// DMS Data: Request ID, Dataset Name, etc.
        /// </summary>
        DMSData DmsData { get; }

        /// <summary>
        /// Time when the Acquisition started
        /// </summary>
        DateTime RunStart { get; }

        /// <summary>
        /// Time when the Acquisition ended
        /// </summary>
        DateTime RunFinish { get; }

        /// <summary>
        /// Name of the column used (as entered in DMS)
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// Name of the instrument (as entered in DMS)
        /// </summary>
        string InstrumentName { get; }

        /// <summary>
        /// Separation type used (as entered in DMS)
        /// </summary>
        string SeparationType { get; }

        /// <summary>
        /// Name of operator (as entered in DMS)
        /// </summary>
        string Operator { get; }

        /// <summary>
        /// Subdirectory containing the dataset
        /// </summary>
        string CaptureSubdirectoryPath { get; }

        /// <summary>
        /// DMS interest rating (or 'Unreviewed')
        /// </summary>
        string InterestRating { get; }
    }

    /// <summary>
    /// Provides data for creating trigger files, including Well Plate data
    /// </summary>
    public interface ITriggerFilePalData : ITriggerFileData
    {
        /// <summary>
        /// Autosampler information, specifically Well Plate Number and Well Number
        /// </summary>
        PalData PAL { get; }
    }
}
