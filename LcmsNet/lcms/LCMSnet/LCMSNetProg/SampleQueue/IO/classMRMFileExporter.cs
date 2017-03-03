//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 04/02/2009
//
// Last modified 09/11/2014
//                      - 04/08/2009 (DAC) - Added output to status screen when export completes
//                      - 04/09/2009 (DAC) - Added exception log messages
//                      - 09/11/2014 (CJW) - Modiifed to use new classDmsToolsManager
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Class for retrieving MRM files from DMS and exporting them to a folder
    /// </summary>
    class classMRMFileExporter : ISampleQueueWriter
    {

        #region "Methods"

        /// <summary>
        /// Implements base class method to export MRM files to a folder
        /// </summary>
        /// <param name="path">File path where data will be stored</param>
        /// <param name="data">List<classSampleData> containing samples which may need MRM files</param>
        public void WriteSamples(string path, List<classSampleData> data)
        {
            // Get a list of sample IDs that may have associated MRM files
            var sampleIDs = GetSampleIDList(data);
            if (sampleIDs.Count < 1)
            {
                // There are no samples with request IDs, therefore no MRM files are needed
                classApplicationLogger.LogMessage(0, "No MRM files to write");
                return;
            }

            // Get a list of MRM file ID's to retrieve
            var filesToGet = GetMRMFileIDs(sampleIDs);
            if (filesToGet.Count < 1)
            {
                // There are no files to retrieve
                classApplicationLogger.LogMessage(0, "No MRM files to write");
                return;
            }

            // Get the file data from DMS
            var fileDataList = GetMRMFileData(filesToGet);

            // Write the data files
            foreach (var fileData in fileDataList)
            {
                try
                {
                    WriteToMRMFile(path, fileData);
                }
                catch (Exception ex)
                {
                    var ErrMsg = "Exception writing MRM files";
                    classApplicationLogger.LogError(0, ErrMsg, ex);
                }
            }

            // Report success
            classApplicationLogger.LogMessage(0, "MRM file write complete");
        }

        /// <summary>
        /// Gets a list of MRM file IDs to download
        /// </summary>
        /// <param name="SampleIDList">List<int> of sample ID's that may have associated MRM files</param>
        /// <returns>List<string> of MRM file ID's representing files to download</returns>
        private List<string> GetMRMFileIDs(List<int> SampleIDList)
        {
            var retList = new List<string>();

            // Get DMS entries for requests having MRM files. Rather than use repeated DMS calls,
            //      start with a range of request ID's that are contained in the current queue

            var minRequest = SampleIDList.Min(); // Minimum request number
            var maxRequest = SampleIDList.Max(); // Maximum request number

            // Get a dictionary of all requests having MRM files with IDs between minRequest and maxRequest
            Dictionary<int, int> idDict;
            try
            {
                idDict = classDMSToolsManager.Instance.SelectedTool.GetMRMFileListFromDMS(minRequest, maxRequest);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception getting list of MRM file ID's", ex);
                return retList;
            }

            if (idDict.Count() < 1)
            {
                // There are no requests with associated MRM files in this range
                return retList;
            }

            // Stuff the return list with IDs and return
            int fileID;
            bool success;
            foreach (var currSampleID in SampleIDList)
            {
                // If current sample has an MRM file associated with it, get the ID
                success = idDict.TryGetValue(currSampleID, out fileID);
                if (success)
                {
                    if (!retList.Contains(fileID.ToString()))
                    {
                        // File ID not already in list, so insert it
                        retList.Add(fileID.ToString());
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// Gets specified MRM filea
        /// </summary>
        /// <param name="filesToGet">Dictionary<int, string> containing IDs of files to get</param>
        /// <returns>List<classMRMFileData> with file names and data</returns>
        private List<classMRMFileData> GetMRMFileData(List<string> filesToGet)
        {
            var retData = new List<classMRMFileData>();

            // Download the file data
            var fileCount = 1;
            var sqlStrBld = new StringBuilder();
            var firstID = true;
            foreach (var fileID in filesToGet)
            {
                //NOTE: For present, assume all file data will fit in memory without problem
                //  If the size or quantity of files gets large, may need to write more frequently
                //  so that memory can be flushed

                // Calls for data will be into 25 file blocks to prevent problems with SQL command length limits
                if ((fileCount % 25) != 0)
                {
                    // Add the next file ID to the existing query string
                    if (!firstID)
                    {
                        sqlStrBld.Append("," + fileID);
                    }
                    else
                    {
                        sqlStrBld.Append(fileID);
                        firstID = false;
                    }
                    fileCount += 1;
                }
                else
                {
                    // Add the last ID
                    sqlStrBld.Append("," + fileID);
                    // Execute the query
                    try
                    {
                        classDMSToolsManager.Instance.SelectedTool.GetMRMFilesFromDMS(sqlStrBld.ToString(), ref retData);
                    }
                    catch (Exception ex)
                    {
                        var ErrMsg = "Exception getting MRM files from DMS";
                        classApplicationLogger.LogError(0, ErrMsg, ex);
                        return retData;
                    }

                    // Initialize a new query string
                    sqlStrBld = new StringBuilder();
                    firstID = true;
                    fileCount += 1;
                }
            } // End foreach

            // Run query for any remaining files, if any
            if (sqlStrBld.Length > 0)
            {
                // The number of files was not a multiple of 25, so process the remainder
                try
                {
                    classDMSToolsManager.Instance.SelectedTool.GetMRMFilesFromDMS(sqlStrBld.ToString(), ref retData);
                }
                catch (Exception ex)
                {
                    var ErrMsg = "Exception getting MRM files from DMS";
                    classApplicationLogger.LogError(0, ErrMsg, ex);
                    return retData;
                }
            }
            return retData;
        }

        /// <summary>
        /// Retrieves a list of request ID's for all samples in the input list
        /// </summary>
        /// <param name="InpList">List<classSampleData> containing an input queue</param>
        /// <returns>Sorted List<int> of DMS request ID's</returns>
        private List<int> GetSampleIDList(List<classSampleData> InpList)
        {
            var retList = new List<int>();

            foreach (var sample in InpList)
            {
                if (sample.DmsData.RequestID != 0)
                {
                    retList.Add(sample.DmsData.RequestID);
                }
            }
            retList.Sort();
            return retList;
        }

        ///// <summary>
        ///// Creates a dictionary containing all of the file keys for MRM files to download
        ///// </summary>
        ///// <param name="InpList">List<classSampleData> of samples to check for file downloads</param>
        ///// <returns></returns>
        //private Dictionary<int, string> GetMRMFileIDs(List<classSampleData> InpList)
        //{
        //   Dictionary<int, string> returnList = new Dictionary<int, string>();
        //   foreach (classSampleData sample in InpList)
        //   {
        //      int tmpIndx = sample.DmsData.MRMFileID;
        //      // Check to see if the sample has a file download required
        //      if (tmpIndx == 0)
        //      {
        //         // No file download required
        //         continue;
        //      }

        //      // Check to see if this file is already in the list of files to download
        //      if (!returnList.ContainsKey(tmpIndx))
        //      {
        //         // File is not already in list, so add it
        //         returnList.Add(tmpIndx, tmpIndx.ToString());
        //      }
        //   }
        //   // Return the list of files to download
        //   return returnList;
        //}

        /// <summary>
        /// Writes a single MRM file to specified location
        /// </summary>
        /// <param name="FilePath">Location for MRM file</param>
        /// <param name="InpData">classMRMFileData object containing MRM file name and contents</param>
        private void WriteToMRMFile(string FilePath, classMRMFileData InpData)
        {
            var mrmFileNamePath = Path.Combine(FilePath, InpData.FileName);

            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(mrmFileNamePath, false);
                fileWriter.Write(InpData.FileContents);
                classApplicationLogger.LogMessage(0, "Completed writing MRM file " + mrmFileNamePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception writing MRM file" + mrmFileNamePath, ex);
            }
            finally
            {
                fileWriter.Close();
            }
        }

        #endregion
    }
}