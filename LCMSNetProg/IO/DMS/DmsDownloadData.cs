using System;
using System.ComponentModel;
using LcmsNetSDK;
// ReSharper disable CommentTypo

namespace LcmsNet.IO.DMS
{
    /// <summary>
    /// Dataset information supplied by DMS, for creating sample queues
    /// </summary>
    public class DmsDownloadData : INotifyPropertyChangedExt
    {
        public DmsDownloadData()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            Comment = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
            SelectedToRun = false;
            PalWell = 0;
            PalWellPlate = "";
        }

        private bool selectedToRun;

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
        public string RequestName { get; set; }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        public int RequestID { get; set; }

        /// <summary>
        /// Name of cart used for sample run
        /// </summary>
        /// <remarks>This is an editable field even if the DMS Request has been resolved.</remarks>
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

        /// <summary>
        /// Well number for wellplate, can also be a vial number
        /// </summary>
        public int PalWell { get; set; }

        /// <summary>
        /// WellPlate identification
        /// </summary>
        public string PalWellPlate { get; set; }

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
