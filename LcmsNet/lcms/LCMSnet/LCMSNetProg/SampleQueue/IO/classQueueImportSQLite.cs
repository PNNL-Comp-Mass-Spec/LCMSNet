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
    public class classQueueImportSQLite: ISampleQueueReader
    {
        /// <summary>
        /// Reads a queue from the database file and returns a list of stored samples.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<classSampleData> ReadSamples(string path)
        {
            //This if statement is a workaround to access network addresses in SQLite library 1.0.93           
            if(path.Substring(0,1) == "\\")
            {
                path = "\\" + path;
            }
            string connStr = "data source=" + path;

            // Get a list of samples from the SQLite file
            List<classSampleData> sampleList = new List<classSampleData>();
            try
            {
                sampleList = classSQLiteTools.GetQueueFromCache(enumTableTypes.WaitingQueue, connStr);
                string msg = "Successfully read input queue file " + path; 
                classApplicationLogger.LogMessage(0, msg);
            }
            catch (Exception ex)
            {
                string errMsg = "Exception reading queue from " + path; 
                classApplicationLogger.LogError(0, errMsg, ex);                
                return sampleList;
            }

            if (sampleList == null)
            {
                string msg = "Returned sample list is null. Additional information may be in log.";
                classApplicationLogger.LogError(0, msg);                
                return sampleList;
            }

            return sampleList;
        }
    }
}
