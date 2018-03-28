using System;
using System.Collections.Generic;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;
using LcmsNetSQLiteTools;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Reads a sample queue from the sql-lite database sample queue file.
    /// </summary>
    public class classQueueImportSQLite : ISampleQueueReader
    {
        /// <summary>
        /// Reads a queue from the database file and returns a list of stored samples.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<SampleData> ReadSamples(string path)
        {
            //This if statement is a workaround to access network addresses in SQLite library 1.0.93
            if (path.Substring(0, 1) == "\\")
            {
                path = "\\" + path;
            }
            var connStr = "data source=" + path;

            // Get a list of samples from the SQLite file
            var sampleList = new List<SampleData>();
            try
            {
                sampleList = SQLiteTools.GetQueueFromCache(DatabaseTableTypes.WaitingQueue, connStr);
                var msg = "Successfully read input queue file " + path;
                ApplicationLogger.LogMessage(0, msg);
            }
            catch (Exception ex)
            {
                var errMsg = "Exception reading queue from " + path;
                ApplicationLogger.LogError(0, errMsg, ex);
                return sampleList;
            }

            if (sampleList == null)
            {
                var msg = "Returned sample list is null. Additional information may be in log.";
                ApplicationLogger.LogError(0, msg);
                return sampleList;
            }

            return sampleList;
        }
    }
}