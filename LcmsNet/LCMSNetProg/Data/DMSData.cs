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
    public class DMSData : INotifyPropertyChangedExt, ICloneable
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
        public object Clone()
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

        #region Property Backing Variables

        private int requestId;
        private string requestName;
        private int block;
        private int runOrder;
        private int batch;
        private bool selectedToRun;
        private string cartName;
        private string comment;

        #endregion

        #region "Properties"

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
        public string RequestName
        {
            get => requestName;
            set => this.RaiseAndSetIfChanged(ref requestName, value);
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
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
        //TODO: DMS_Download_Only? Probably should just drop it
        public string CartName
        {
            get => cartName;
            set => this.RaiseAndSetIfChanged(ref cartName, value);
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

        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
