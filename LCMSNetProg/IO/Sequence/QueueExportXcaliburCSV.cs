using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LcmsNet.Data;
using LcmsNet.SampleQueue;
using LcmsNetSDK.Logging;

namespace LcmsNet.IO.Sequence
{
    /// <summary>
    /// Exports specified queue to CSV file specifically tailored to Xcalibur import
    /// </summary>
    class QueueExportXcaliburCSV : ISampleQueueWriter
    {
        /// <summary>
        /// Saves the specified sample list to the specified file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteSamples(string path, List<SampleData> data)
        {
            var outString = BuildOutputString(data);
            WriteOutputFile(outString, path);
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
            strBld.Append("Bracket Type=4" + Environment.NewLine);
            strBld.Append("File Name" + Environment.NewLine);

            // Add the data for each sample
            foreach (var sample in samples)
            {
                strBld.Append(sample.Name + Environment.NewLine);
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
                var errMsg = ex.Message;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DataExportException(errMsg, ex);
            }
        }
    }
}