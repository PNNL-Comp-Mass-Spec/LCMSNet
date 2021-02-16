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
        IDmsData DmsBasicData { get; }

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
        /// Name of operator (as entered in DMS). Can be just userID, just user's name, or "user's name (userID)"
        /// </summary>
        string Operator { get; }

        /// <summary>
        /// Name of the shared directory used to access the dataset. If empty, the instrument default (in DMS) is used.
        /// </summary>
        string CaptureShareName { get; }

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
        IPalData PAL { get; }
    }
}
