using System;

namespace LcmsNetData.Data
{
    public interface IDmsData : INotifyPropertyChangedExt
    {
        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        string RequestName { get; set; }

        /// <summary>
        /// Gets or sets the name of the dataset after editing the request name.
        /// </summary>
        string DatasetName { get; set; }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        int RequestID { get; set; }

        /// <summary>
        /// Instrument group specified in DMS for the request (only used when <see cref="RequestID"/> &gt; 0)
        /// </summary>
        string InstrumentGroup { get; set; }

        /// <summary>
        /// Experiment name
        /// </summary>
        string Experiment { get; set; }

        /// <summary>
        /// Dataset type (ie, HMS-MSn, HMS, etc)
        /// </summary>
        string DatasetType { get; set; }

        /// <summary>
        /// Work Package/charge code
        /// </summary>
        string WorkPackage { get; set; }

        /// <summary>
        /// EMSL usage type
        /// </summary>
        string EMSLUsageType { get; set; }

        /// <summary>
        /// EUS user proposal ID
        /// </summary>
        string EMSLProposalID { get; set; }

        /// <summary>
        /// EUS user list
        /// </summary>
        string EMSLProposalUser { get; set; }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        string CartName { get; set; }

        /// <summary>
        /// Name of cart configuration for the current cart
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        string CartConfigName { get; set; }

        /// <summary>
        /// Comment field
        /// </summary>
        string Comment { get; set; }
    }

    /// <summary>
    /// IDmsData interface with additional data about the sample source and run instructions
    /// </summary>
    public interface IDmsDataForSampleRun : IDmsData, ICloneable
    {
        /// <summary>
        /// Flag for determining if request from DMS has been selected for running
        /// </summary>
        bool SelectedToRun { get; set; }

        /// <summary>
        /// File ID for locating MRM file to download
        /// </summary>
        int MRMFileID { get; set; }

        /// <summary>
        /// Block ID for blocking/randomizing
        /// </summary>
        int Block { get; set; }

        /// <summary>
        /// Run order for blocking/randomizing
        /// </summary>
        int RunOrder { get; set; }

        /// <summary>
        /// Batch number for blocking/randomizing
        /// </summary>
        int Batch { get; set; }
    }
}
