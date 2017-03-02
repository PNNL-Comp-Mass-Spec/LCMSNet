using System;
using System.Collections.Generic;
using LcmsNet.Method;
using LcmsNetSQLiteTools;
using LcmsNetDmsTools;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Reads a sample queue from the sql-lite database sample queue file.
    /// </summary>
    public class classQueueExportSQLite : ISampleQueueWriter
    {
        /// <summary>
        /// Reads a queue from the database file and returns a list of stored samples.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void WriteSamples(string path, List<classSampleData> data)
        {
            string connStr = "data source=" + path;

            try
            {
                classSQLiteTools.SaveQueueToCache(data, enumTableTypes.WaitingQueue, connStr);
            }
            catch (Exception ex)
            {
                string errMsg = "Exception exporting queue to " + path;
                classApplicationLogger.LogError(0, errMsg, ex);
            }
        }
    }
}