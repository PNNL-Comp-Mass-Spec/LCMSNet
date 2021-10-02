using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Dataset information supplied by or required by DMS; includes run request information
    /// </summary>
    [Serializable]
    public class DMSData : IDmsDataForSampleRun, INotifyPropertyChangedExt, ICloneable
    {
        /// <summary>
        /// The matching string to ensure only valid characters exist in a dataset name
        /// </summary>
        public const string ValidDatasetNameRegexString = @"^[a-zA-Z0-9_\-]+$";

        /// <summary>
        /// The list of characters allowed in a dataset name
        /// </summary>
        public const string ValidDatasetNameCharacters = @"Valid characters are: 'A-Z', 'a-z', '0-9', '-', '_' (no spaces)";

        /// <summary>
        /// Regex to use to test if a dataset name only contains valid characters
        /// </summary>
        public static readonly Regex NameValidationRegex = new Regex(ValidDatasetNameRegexString, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public DMSData()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            CartConfigName = "";
            Comment = "";
            DatasetName = "";
            DatasetType = "";
            Experiment = "";
            MRMFileID = -1;
            EMSLProposalID = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
            EMSLUsageType = "";
            EMSLProposalUser = "";
            WorkPackage = "";
        }

        /// <summary>
        /// Unlock the object and reset all properties to default values.
        /// </summary>
        public void Reset()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            CartConfigName = "";
            Comment = "";
            DatasetName = "";
            DatasetType = "";
            Experiment = "";
            MRMFileID = -1;
            EMSLProposalID = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
            EMSLUsageType = "";
            EMSLProposalUser = "";
            WorkPackage = "";
        }

        /// <summary>
        /// Clone - get a deep copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var newDmsData = new DMSData();

            newDmsData.Batch = Batch;
            newDmsData.Block = Block;
            newDmsData.CartName = CartName;
            newDmsData.CartConfigName = CartConfigName;
            newDmsData.Comment = Comment;
            newDmsData.DatasetName = DatasetName;
            newDmsData.DatasetType = DatasetType;
            newDmsData.Experiment = Experiment;
            newDmsData.MRMFileID = MRMFileID;
            newDmsData.EMSLProposalID = EMSLProposalID;
            newDmsData.RequestID = RequestID;
            newDmsData.RequestName = RequestName;
            newDmsData.RunOrder = RunOrder;
            newDmsData.SelectedToRun = SelectedToRun;
            newDmsData.EMSLUsageType = EMSLUsageType;
            newDmsData.EMSLProposalUser = EMSLProposalUser;
            newDmsData.WorkPackage = WorkPackage;

            return newDmsData;
        }

        #region Property Backing Variables

        private int requestId;
        private string requestName;
        private string datasetName;
        private string datasetType;
        private string cartConfigName;
        private string workPackage;
        private string emslUsageType;
        private string emslProposalId;
        private string emslProposalUser;
        private string experiment;
        private int block;
        private int runOrder;
        private int batch;
        private bool selectedToRun;
        private string cartName;
        private string comment;
        private int mrmFileId;

        #endregion

        #region "Properties"

        /// <summary>
        /// Flag for determining if request from DMS has been selected for running
        /// </summary>
        public bool SelectedToRun
        {
            get => selectedToRun;
            set => this.RaiseAndSetIfChanged(ref selectedToRun, value);
        }

        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        [PersistenceSetting(IsUniqueColumn = true)]
        public string RequestName
        {
            get => requestName;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref requestName, value))
                {
                    if (string.IsNullOrWhiteSpace(DatasetName))
                    {
                        DatasetName = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the dataset after editing the request name.
        /// </summary>
        [PersistenceSetting(IsUniqueColumn = true)]
        public string DatasetName
        {
            get => datasetName;
            set => this.RaiseAndSetIfChanged(ref datasetName, value);
        }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        [PersistenceSetting(IsUniqueColumn = true)]
        public int RequestID
        {
            get => requestId;
            set => this.RaiseAndSetIfChanged(ref requestId, value);
        }

        /// <summary>
        /// Instrument group specified in DMS for the request (only used when <see cref="RequestID"/> &gt; 0)
        /// </summary>
        public string InstrumentGroup { get; set; }

        /// <summary>
        /// Experiment name
        /// </summary>
        public string Experiment
        {
            get => experiment;
            set => this.RaiseAndSetIfChanged(ref experiment, value);
        }

        /// <summary>
        /// Dataset type (ie, HMS-MSn, HMS, etc)
        /// </summary>
        public string DatasetType
        {
            get => datasetType;
            set => this.RaiseAndSetIfChanged(ref datasetType, value);
        }

        /// <summary>
        /// Work Package/charge code
        /// </summary>
        public string WorkPackage
        {
            get => workPackage;
            set => this.RaiseAndSetIfChanged(ref workPackage, value);
        }

        /// <summary>
        /// EMSL usage type
        /// </summary>
        public string EMSLUsageType
        {
            get => emslUsageType;
            set => this.RaiseAndSetIfChanged(ref emslUsageType, value);
        }

        /// <summary>
        /// EUS user proposal ID
        /// </summary>
        public string EMSLProposalID
        {
            get => emslProposalId;
            set => this.RaiseAndSetIfChanged(ref emslProposalId, value);
        }

        /// <summary>
        /// EUS user list
        /// </summary>
        public string EMSLProposalUser
        {
            get => emslProposalUser;
            set => this.RaiseAndSetIfChanged(ref emslProposalUser, value);
        }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        public string CartName
        {
            get => cartName;
            set => this.RaiseAndSetIfChanged(ref cartName, value);
        }

        /// <summary>
        /// Name of cart configuration for the current cart
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        public string CartConfigName
        {
            get => cartConfigName;
            set => this.RaiseAndSetIfChanged(ref cartConfigName, value);
        }

        /// <summary>
        /// Comment field
        /// </summary>
        public string Comment
        {
            get => comment;
            set => this.RaiseAndSetIfChanged(ref comment, value);
        }

        /// <summary>
        /// File ID for locating MRM file to download
        /// </summary>
        public int MRMFileID
        {
            get => mrmFileId;
            set => this.RaiseAndSetIfChanged(ref mrmFileId, value);
        }

        /// <summary>
        /// Block ID for blocking
        /// </summary>
        public int Block
        {
            get => block;
            set => this.RaiseAndSetIfChanged(ref block, value);
        }

        /// <summary>
        /// Run order for blocking
        /// </summary>
        public int RunOrder
        {
            get => runOrder;
            set => this.RaiseAndSetIfChanged(ref runOrder, value);
        }

        /// <summary>
        /// Batch number for blocking
        /// </summary>
        public int Batch
        {
            get => batch;
            set => this.RaiseAndSetIfChanged(ref batch, value);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DatasetName))
            {
                if (string.Equals(RequestName, DatasetName))
                {
                    return "Request " + RequestName;
                }

                return "Dataset " + DatasetName;
            }

            if (!string.IsNullOrWhiteSpace(Experiment))
                return "Experiment " + Experiment;

            if (!string.IsNullOrWhiteSpace(RequestName))
                return "Request " + RequestName;

            return "RequestID " + RequestID;
        }

        public bool DatasetNameCharactersValid()
        {
            var name = DatasetName;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = RequestName;
            }

            return NameValidationRegex.IsMatch(name);
        }

        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
