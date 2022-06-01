using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LcmsNet.Data;
using LcmsNet.SampleQueue;
using LcmsNetSDK.Logging;

namespace LcmsNet.IO.Sequence
{
    public class QueueExportCSV : ISampleQueueWriter
    {
        //*********************************************************************************************************
        //Exports specified queue to CSV file
        //**********************************************************************************************************

        /// <summary>
        /// Saves the specified sample list to the specified file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteSamples(string path, List<SampleData> data)
        {
            // Make a copy of the data list, so that the rest of the app doesn't get screwed up by the sort
            var sortedData = new List<SampleData>();
            foreach (var currentSample in data)
            {
                var tmpSample = currentSample.Clone(true);
                sortedData.Add(tmpSample);
            }

            var outString = BuildOutputString(sortedData);

            try
            {
                WriteOutputFile(outString, path);
            }
            catch (Exception ex)
            {
                // Just re-throw the exception to let the user know something bad happened
                throw ex;
            }
        }

        /// <summary>
        /// Builds the string to save
        /// </summary>
        /// <param name="samples">List of samples</param>
        /// <returns>String representing entire file contents</returns>
        private string BuildOutputString(List<SampleData> samples)
        {
            var strBld = new StringBuilder();

            // Build header
            strBld.Append("\"Request Name\",");
            strBld.Append("\"Request ID\",");
            strBld.Append("\"Column\",");
            strBld.Append("\"Run Order\"");
            strBld.Append(Environment.NewLine);

            // Add the data for each sample
            foreach (var sample in samples)
            {
                var colNum = sample.ColumnIndex + 1;
                strBld.Append("\"" + sample.Name + "\",");
                strBld.Append("\"" + sample.DmsRequestId + "\",");
                strBld.Append("\"" + colNum + "\",");
                strBld.Append("\"" + (sample.DmsData?.RunOrder ?? 0) + "\",");
                strBld.Append(Environment.NewLine);
            }

            return strBld.ToString();
        }

        /// <summary>
        /// Writes the output file
        /// </summary>
        /// <param name="outputStr">String to write</param>
        /// <param name="fileNamePath">Fully qualified file name and path</param>
        private void WriteOutputFile(string outputStr, string fileNamePath)
        {
            try
            {
                File.WriteAllText(fileNamePath, outputStr);
                ApplicationLogger.LogMessage(0, "Queue exported to CSV file " + fileNamePath);
            }
            catch (Exception ex)
            {
                var errMsg = "Could not write samples to file " + fileNamePath;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DataExportException(errMsg, ex);
            }
        }
    }
}