using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace LcmsNet.Data
{
    /// <summary>
    /// Dataset information supplied by or required by DMS; includes run request information
    /// </summary>
    [Serializable]
    public class DMSData : INotifyPropertyChangedExt
    {
        public DMSData()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            Comment = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
        }

        /// <summary>
        /// Unlock the object and reset all properties to default values.
        /// </summary>
        public void Reset()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            Comment = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
        }

        /// <summary>
        /// Clone - get a deep copy
        /// </summary>
        /// <returns></returns>
        public DMSData Clone()
        {
            var newData = new DMSData();

            newData.Batch = Batch;
            newData.Block = Block;
            newData.CartName = CartName;
            newData.Comment = Comment;
            newData.RequestID = RequestID;
            newData.RequestName = RequestName;
            newData.RunOrder = RunOrder;
            newData.SelectedToRun = SelectedToRun;

            return newData;
        }

        private bool selectedToRun;

        /// <summary>
        /// Flag for determining if request from DMS has been selected for running
        /// </summary>
        //TODO: DMS_Download_Only
        public bool SelectedToRun
        {
            get => selectedToRun;
            set => this.RaiseAndSetIfChanged(ref selectedToRun, value);
        }

        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        //TODO: DMS_Download_Only?
        [PersistenceSetting(IsUniqueColumn = true)]
        public string RequestName { get; set; }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        [PersistenceSetting(IsUniqueColumn = true)]
        public int RequestID { get; set; }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        //TODO: DMS_Download_Only
        public string CartName { get; set; }

        /// <summary>
        /// Comment field
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Block ID for blocking
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        /// Run order for blocking
        /// </summary>
        public int RunOrder { get; set; }

        /// <summary>
        /// Batch number for blocking
        /// </summary>
        public int Batch { get; set; }

        /// <summary>
        /// EMSL usage type
        /// </summary>
        public string EMSLUsageType { get; set; }

        /// <summary>
        /// EUS user list
        /// </summary>
        public string EMSLProposalUser { get; set; }

        public List<string[]> GetExportValuePairs()
        {
            var exportData = new List<string[]>();

            exportData.Add(new[] { "Request:", RequestName });
            exportData.Add(new[] { "Request Id:", RequestID.ToString() });
            exportData.Add(new[] { "Batch:", Batch.ToString() });
            exportData.Add(new[] { "Block:", Block.ToString() });
            exportData.Add(new[] { "Run Order:", RunOrder.ToString() });
            exportData.Add(new[] { "Comment:", Comment });

            return exportData;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(RequestName))
                return "Request " + RequestName;

            return "RequestID " + RequestID;
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
