//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/22/2010
//
// Last modified 07/22/2010
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.SampleQueue.IO
{
    class classQueueExportExcalCSV : ISampleQueueWriter
    {
        //*********************************************************************************************************
        //Exports specified queue to CSV file specifically tailored to Excalibur import
        //**********************************************************************************************************

        #region "Methods"

        /// <summary>
        /// Saves the specified sample list to the specified file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteSamples(string path, List<classSampleData> data)
        {
            var outString = BuildOutputString(data);
            WriteOutputFile(outString, path);
        }

        /// <summary>
        /// Compares column ID's for two samples
        /// </summary>
        /// <param name="sample1">First sample to be compared</param>
        /// <param name="sample2">2nd sample to be compared</param>
        /// <returns>0 if equal, 1 if 1st sample's column ID > 2nd, otherwise -1</returns>
        private static int CompareSamplesByRunOrder(classSampleData sample1, classSampleData sample2)
        {
            if (sample1 == null)
            {
                if (sample2 == null)
                {
                    // if sample1 is null and sample2 is null, they're equal)
                    return 0;
                }
                else
                {
                    // If sample1 is null and sample2 is not null, sample2 is greater
                    return -1;
                }
            }
            else
            {
                if (sample2 == null)
                {
                    // If sample1 is not null and sample2 is null, sample1 is greater
                    return 1;
                }
                else
                {
                    // Neither input is null, so compare the column indices
                    if (sample1.DmsData.RunOrder == sample2.DmsData.RunOrder)
                    {
                        // Run orders are equal
                        return 0;
                    }
                    else if (sample1.DmsData.RunOrder > sample2.DmsData.RunOrder)
                    {
                        // sample 1 run order is > sample2 run order
                        return 1;
                    }
                    else
                    {
                        //  sample 1 run order < run order
                        return -1;
                    }
                }
            }
        }

        /// <summary>
        /// Builds the string to save
        /// </summary>
        /// <param name="samples">List of samples</param>
        /// <returns>String representing entire file contents</returns>
        private string BuildOutputString(List<classSampleData> samples)
        {
            var strBld = new StringBuilder();

            // Build header
            strBld.Append("Bracket Type=4" + Environment.NewLine);
            strBld.Append("File Name" + Environment.NewLine);

            // Add the data for each sample
            foreach (var sample in samples)
            {
                strBld.Append(sample.DmsData.DatasetName + Environment.NewLine);
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
                classApplicationLogger.LogMessage(0, "Queue exported to CSV file " + fileNamePath);
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
                classApplicationLogger.LogError(0, errMsg, ex);
                throw new classDataExportException(errMsg, ex);
            }
        }

        #endregion
    }
} // End namespace