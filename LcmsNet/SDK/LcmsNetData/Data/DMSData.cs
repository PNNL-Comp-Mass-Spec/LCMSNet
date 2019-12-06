﻿using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Dataset information supplied by or required by DMS; includes run request information
    /// </summary>
    [Serializable]
    public class DMSData : INotifyPropertyChangedExt, ICloneable
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
            LockData = false;
            Batch = -1;
            Block = -1;
            CartName = "";
            CartConfigName = "";
            Comment = "";
            CommentAddition = "";
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
            UserList = "";
            WorkPackage = "";
        }

        /// <summary>
        /// Unlock the object and reset all properties to default values.
        /// </summary>
        public void Reset()
        {
            LockData = false;
            Batch = -1;
            Block = -1;
            CartName = "";
            CartConfigName = "";
            Comment = "";
            CommentAddition = "";
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
            UserList = "";
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
            newDmsData.UserList = UserList;
            newDmsData.WorkPackage = WorkPackage;
            newDmsData.LockData = LockData;

            return newDmsData;
        }

        public void CopyValuesAndLock(DMSData other)
        {
            LockData = false;
            Batch = other.Batch;
            Block = other.Block;
            CartName = other.CartName;
            CartConfigName = other.CartConfigName;
            Comment = other.Comment;
            DatasetName = other.DatasetName;
            DatasetType = other.DatasetType;
            Experiment = other.Experiment;
            MRMFileID = other.MRMFileID;
            EMSLProposalID = other.EMSLProposalID;
            RequestID = other.RequestID;
            RequestName = other.RequestName;
            RunOrder = other.RunOrder;
            SelectedToRun = other.SelectedToRun;
            EMSLUsageType = other.EMSLUsageType;
            UserList = other.UserList;
            WorkPackage = other.WorkPackage;
            LockData = other.LockData;
        }

        /// <summary>
        /// Copies the data and locks this object, and sets the dataset name to the filename in <paramref name="filePath"/>
        /// Used in Buzzard
        /// </summary>
        /// <param name="other"></param>
        /// <param name="filePath"></param>
        public void CopyValuesAndLockWithNewPath(DMSData other, string filePath)
        {
            CopyValuesAndLock(other);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                LockData = false;
                DatasetName = fileName;
                LockData = true;
            }
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
        private string userList;
        private string experiment;
        private int block;
        private int runOrder;
        private int batch;
        private bool selectedToRun;
        private bool lockData = false;
        private string cartName;
        private string comment;
        private int mrmFileId;
        private string commentAddition;

        #endregion

        #region "Properties"

        /// <summary>
        /// When the data comes from DMS, it will be locked. This is meant to stop the user
        /// from altering it. (this is not used in LCMSNet; it is used in Buzzard)
        /// </summary>
        public bool LockData
        {
            get => lockData;
            private set => this.RaiseAndSetIfChanged(ref lockData, value, nameof(LockData));
        }

        /// <summary>
        /// Flag for determining if request from DMS has been selected for running
        /// </summary>
        public bool SelectedToRun
        {
            get => selectedToRun;
            set => this.RaiseAndSetIfChangedLockCheck(ref selectedToRun, value, LockData, nameof(SelectedToRun));
        }

        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        public string RequestName
        {
            get => requestName;
            set
            {
                if (this.RaiseAndSetIfChangedLockCheckRetBool(ref requestName, value, LockData, nameof(RequestName)))
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
        public string DatasetName
        {
            get => datasetName;
            set => this.RaiseAndSetIfChangedLockCheck(ref datasetName, value, LockData, nameof(DatasetName));
        }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        public int RequestID
        {
            get => requestId;
            set => this.RaiseAndSetIfChangedLockCheck(ref requestId, value, LockData, nameof(RequestID));
        }

        /// <summary>
        /// Experiment name
        /// </summary>
        public string Experiment
        {
            get => experiment;
            set
            {
                this.RaiseAndSetIfChangedLockCheck(ref experiment, value, LockData, nameof(Experiment));
            }
        }

        /// <summary>
        /// Dataset type (ie, HMS-MSn, HMS, etc)
        /// </summary>
        public string DatasetType
        {
            get => datasetType;
            set => this.RaiseAndSetIfChangedLockCheck(ref datasetType, value, LockData, nameof(DatasetType));
        }

        /// <summary>
        /// Work Package/charge code
        /// </summary>
        public string WorkPackage
        {
            get => workPackage;
            set => this.RaiseAndSetIfChangedLockCheck(ref workPackage, value, LockData, nameof(WorkPackage));
        }

        /// <summary>
        /// EMSL usage type
        /// </summary>
        public string EMSLUsageType
        {
            get => emslUsageType;
            set => this.RaiseAndSetIfChangedLockCheck(ref emslUsageType, value, LockData, nameof(EMSLUsageType));
        }

        /// <summary>
        /// EUS user proposal ID
        /// </summary>
        public string EMSLProposalID
        {
            get => emslProposalId;
            set => this.RaiseAndSetIfChangedLockCheck(ref emslProposalId, value, LockData, nameof(EMSLProposalID));
        }

        /// <summary>
        /// EUS user list
        /// </summary>
        public string UserList
        {
            get => userList;
            set => this.RaiseAndSetIfChangedLockCheck(ref userList, value, LockData, nameof(UserList));
        }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        public string CartName
        {
            get => cartName;
            set => this.RaiseAndSetIfChanged(ref cartName, value, nameof(CartName));
        }

        /// <summary>
        /// Name of cart configuration for the current cart
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        public string CartConfigName
        {
            get => cartConfigName;
            set => this.RaiseAndSetIfChanged(ref cartConfigName, value, nameof(CartConfigName));
        }

        /// <summary>
        /// Comment field
        /// </summary>
        public string Comment
        {
            get => comment;
            set => this.RaiseAndSetIfChangedLockCheck(ref comment, value, LockData, nameof(Comment));
        }

        /// <summary>
        /// The full comment, including the addition and addition prefix
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public string CommentComplete => $"{Comment} {(string.IsNullOrWhiteSpace(CommentAdditionPrefix) ? string.Empty : CommentAdditionPrefix.Trim() + " ")}{CommentAddition}".Trim();

        /// <summary>
        /// Additional comment. Used by Buzzard to add comment information to datasets matched to run requests.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public string CommentAddition
        {
            get => commentAddition;
            set => this.RaiseAndSetIfChanged(ref commentAddition, value);
        }

        /// <summary>
        /// Additional comment prefix. Used by Buzzard, output before CommentAddition.
        /// </summary>
        [PersistenceSetting(IgnoreProperty = true)]
        public string CommentAdditionPrefix { get; set; }

        /// <summary>
        /// File ID for locating MRM file to download
        /// </summary>
        public int MRMFileID
        {
            get => mrmFileId;
            set => this.RaiseAndSetIfChangedLockCheck(ref mrmFileId, value, LockData, nameof(MRMFileID));
        }

        /// <summary>
        /// Block ID for blocking/randomizing
        /// </summary>
        public int Block
        {
            get => block;
            set => this.RaiseAndSetIfChangedLockCheck(ref block, value, LockData, nameof(Block));
        }

        /// <summary>
        /// Run order for blocking/randomizing
        /// </summary>
        public int RunOrder
        {
            get => runOrder;
            set => this.RaiseAndSetIfChangedLockCheck(ref runOrder, value, LockData, nameof(RunOrder));
        }

        /// <summary>
        /// Batch number for blocking/randomizing
        /// </summary>
        public int Batch
        {
            get => batch;
            set => this.RaiseAndSetIfChangedLockCheck(ref batch, value, LockData, nameof(Batch));
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
