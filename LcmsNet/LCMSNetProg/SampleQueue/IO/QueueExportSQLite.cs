using System;
using System.Collections.Generic;
using LcmsNet.Data;
using LcmsNet.IO.SQLite;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Reads a sample queue from the sql-lite database sample queue file.
    /// </summary>
    public class QueueExportSQLite : ISampleQueueWriter
    {
        /// <summary>
        /// Reads a queue from the database file and returns a list of stored samples.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void WriteSamples(string path, List<SampleData> data)
        {
            var connStr = "data source=" + path;

            try
            {
                SQLiteTools.SaveQueueToCache(data, DatabaseTableTypes.WaitingQueue, connStr);
            }
            catch (Exception ex)
            {
                var errMsg = "Exception exporting queue to " + path;
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }
    }
}