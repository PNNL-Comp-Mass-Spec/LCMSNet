using System;
using System.Collections.Generic;
using LcmsNet.IO.DMS;

namespace LcmsNet.Data
{
    /// <summary>
    /// Dataset information supplied by or required by DMS; includes run request information
    /// </summary>
    [Serializable]
    public class DMSData
    {
        private DMSData()
        {
            Batch = -1;
            Block = -1;
            CartName = "";
            Comment = "";
            RequestID = 0;
            RequestName = "";
            RunOrder = -1;
        }

        public DMSData(DmsDownloadData source)
        {
            RequestName = source.RequestName;
            RequestID = source.RequestID;
            Comment = source.Comment;
            Block = source.Block;
            RunOrder = source.RunOrder;
            Batch = source.Batch;
        }

        internal DMSData(string requestName, int block) : this()
        {
            RequestName = requestName;
            Block = block;
        }

        /// <summary>
        /// Name of request in DMS. Becomes sample name in LCMS and forms part
        /// of dataset name sample after run
        /// </summary>
        public string RequestName { get; }

        /// <summary>
        /// Numeric ID of request in DMS
        /// </summary>
        public int RequestID { get; }

        /// <summary>
        /// Name of cart set for request
        /// </summary>
        public string CartName { get; }

        /// <summary>
        /// Comment field
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Block ID for blocking
        /// </summary>
        public int Block { get; }

        /// <summary>
        /// Run order for blocking
        /// </summary>
        public int RunOrder { get; }

        /// <summary>
        /// Batch number for blocking
        /// </summary>
        public int Batch { get; }

        public List<string[]> GetExportValuePairs()
        {
            var exportData = new List<string[]>
            {
                new[] { "Request:", RequestName },
                new[] { "Request Id:", RequestID.ToString() },
                new[] { "Batch:", Batch.ToString() },
                new[] { "Block:", Block.ToString() },
                new[] { "Run Order:", RunOrder.ToString() },
                new[] { "Comment:", Comment }
            };

            return exportData;
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(RequestName))
                return "Request " + RequestName;

            return "RequestID " + RequestID;
        }
    }
}
