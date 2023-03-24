using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Logging;

// ReSharper disable UnusedMember.Global

namespace LcmsNet.IO.DMS
{
    /// <summary>
    /// Class for interacting with DMS database
    /// </summary>
    public class DMSDBTools
    {
        private readonly DMSDBReader dbReader;

        public string ErrMsg { get; set; } = "";

        public string DMSDatabase => dbReader.DMSDatabase;

        /// <summary>
        /// Constructor
        /// </summary>
        public DMSDBTools()
        {
            dbReader = new DMSDBReader();
        }

        /// <summary>
        /// Gets a list of instrument carts from DMS and stores it in cache
        /// </summary>
        public List<string> GetCartListFromDMS()
        {
            return dbReader.ReadCartList().ToList();
        }

        /// <summary>
        /// Test if we can query each of the needed DMS tables/views.
        /// </summary>
        /// <returns></returns>
        public bool CheckDMSConnection()
        {
            return dbReader.CheckDMSConnection();
        }

        /// <summary>
        /// Gets a list of samples (essentially requested runs) from DMS
        /// </summary>
        /// <remarks>Retrieves data from view V_Requested_Run_Active_Export</remarks>
        public IEnumerable<DmsDownloadData> GetRequestedRunsFromDMS(SampleQueryData queryData)
        {
            try
            {
                dbReader.RefreshConnectionConfiguration();
                return dbReader.ReadRequestedRuns(queryData);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting run request list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return Enumerable.Empty<DmsDownloadData>();
            }
        }
    }
}
