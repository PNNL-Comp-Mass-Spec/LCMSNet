//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Updates:
// - 02/04/2009 (DAC) - Removed well plate and well, which were xferred to PAL data class
// - 02/10/2009 (DAC) - Added method for obtaining current values of all properties
// - 02/17/2009 (DAC) - Changed to inherited class; moved cache ops to base class
// - 03/09/2009 (DAC) - Added comment property
// - 04/02/2009 (DAC) - Added field for MRM file index
// - 08/11/2009 (DAC) - Added field for run request batch number
//
//*********************************************************************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LcmsNetSDK;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Class file for handling data used in communication with DMS
    /// </summary>
    [Serializable]
    public class classDMSData : classDataClassBase, INotifyPropertyChangedExt
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

        public classDMSData()
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
            ProposalID = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
            UsageType = "";
            UserList = "";
        }

        #region Property Backing Variables

        private int requestId;
        private string requestName;
        private string datasetName;
        private string datasetType;
        private string cartConfigName;
        private string usageType;
        private string proposalId;
        private string userList;
        private string experiment;
        private int block;
        private int runOrder;
        private int batch;

        #endregion

        #region "Properties"

        /// <summary>
        /// Flag for determining if request from DMS has been selected for running
        /// </summary>
        public bool SelectedToRun { get; set; }

        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        public string RequestName
        {
            get { return requestName; }
            set
            {
#if DotNET4
                if (this.RaiseAndSetIfChangedRetBool(ref requestName, value, nameof(RequestName)))
#else
                if (this.RaiseAndSetIfChangedRetBool(ref requestName, value))
#endif
                {
                    if (string.IsNullOrEmpty(DatasetName))
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
            get { return datasetName; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref datasetName, value, nameof(DatasetName)); }
#else
            set { this.RaiseAndSetIfChanged(ref datasetName, value); }
#endif
        }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        public int RequestID
        {
            get { return requestId; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref requestId, value, nameof(RequestID)); }
#else
            set { this.RaiseAndSetIfChanged(ref requestId, value); }
#endif
        }

        /// <summary>
        /// Experiment name
        /// </summary>
        public string Experiment
        {
            get { return experiment; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref experiment, value, nameof(Experiment)); }
#else
            set { this.RaiseAndSetIfChanged(ref experiment, value); }
#endif
        }

        /// <summary>
        /// Dataset type (ie, HMS-MSn, HMS, etc)
        /// </summary>
        public string DatasetType
        {
            get { return datasetType; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref datasetType, value, nameof(DatasetType)); }
#else
            set { this.RaiseAndSetIfChanged(ref datasetType, value); }
#endif
        }

        /// <summary>
        /// EMSL usage type
        /// </summary>
        public string UsageType
        {
            get { return usageType; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref usageType, value, nameof(UsageType)); }
#else
            set { this.RaiseAndSetIfChanged(ref usageType, value); }
#endif
        }

        /// <summary>
        /// EUS sser proposal ID
        /// </summary>
        public string ProposalID
        {
            get { return proposalId; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref proposalId, value, nameof(ProposalID)); }
#else
            set { this.RaiseAndSetIfChanged(ref proposalId, value); }
#endif
        }

        /// <summary>
        /// EUS user list
        /// </summary>
        public string UserList
        {
            get { return userList; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref userList, value, nameof(UserList)); }
#else
            set { this.RaiseAndSetIfChanged(ref userList, value); }
#endif
        }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        public string CartName { get; set; }

        /// <summary>
        /// Name of cart configuration used for sample run
        /// </summary>
        public string CartConfigName
        {
            get { return cartConfigName; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref cartConfigName, value, nameof(CartConfigName)); }
#else
            set { this.RaiseAndSetIfChanged(ref cartConfigName, value); }
#endif
        }

        /// <summary>
        /// Comment field
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// File ID for locating MRM file to download
        /// </summary>
        public int MRMFileID { get; set; }

        /// <summary>
        /// Block ID for blocking/randomizing
        /// </summary>
        public int Block
        {
            get { return block; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref block, value, nameof(Block)); }
#else
            set { this.RaiseAndSetIfChanged(ref block, value); }
#endif
        }

        /// <summary>
        /// Run order for blocking/randomizing
        /// </summary>
        public int RunOrder
        {
            get { return runOrder; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref runOrder, value, nameof(RunOrder)); }
#else
            set { this.RaiseAndSetIfChanged(ref runOrder, value); }
#endif
        }

        /// <summary>
        /// Batch number for blocking/randomizing
        /// </summary>
        public int Batch
        {
            get { return batch; }
#if DotNET4
            set { this.RaiseAndSetIfChanged(ref batch, value, nameof(Batch)); }
#else
            set { this.RaiseAndSetIfChanged(ref batch, value); }
#endif
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

#if DotNET4
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#else
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
#endif
    }
}
