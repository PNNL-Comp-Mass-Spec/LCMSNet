//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 04/14/2009
//
// Updates
// - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Imports an XML file from LCMS
    /// </summary>
    class QueueImportXML : ISampleQueueReader
    {
        //*********************************************************************************************************
        //
        //**********************************************************************************************************

        #region "Methods"

        /// <summary>
        /// Reads the XML file into a list
        /// </summary>
        /// <param name="path">Name and path of file to import</param>
        /// <returns>List containing samples read from XML file</returns>
        public List<SampleData> ReadSamples(string path)
        {
            var returnList = new List<SampleData>();

            // Verify input file exists
            if (!File.Exists(path))
            {
                var ErrMsg = "Import file " + path + " not found";
                ApplicationLogger.LogMessage(0, ErrMsg);
                return returnList;
            }

            // Open the file
            var doc = new XmlDocument();
            try
            {
                doc.Load(path);
            }
            catch (Exception ex)
            {
                var ErrMsg = "Exception loading XML file " + path;
                ApplicationLogger.LogError(0, ErrMsg, ex);
                throw new DataImportException(ErrMsg, ex);
            }

            // Get all the nodes under QueueSettings node
            var nodeList = doc.SelectNodes("//QueueSettings/*");

            // If no nodes found, report and exit
            if (nodeList == null || nodeList.Count < 1)
            {
                var errMsg = "No sample data found for import in file " + path;
                ApplicationLogger.LogMessage(0, errMsg);
                return returnList;
            }

            // Get the data for each sample and add it to the return list
            foreach (XmlNode currentNode in nodeList)
            {
                if (currentNode.Name.StartsWith("Item") && !currentNode.Name.Equals("ItemCount"))
                {
                    try
                    {
                        var newSample = ConvertXMLNodeToSample(currentNode);
                        returnList.Add(newSample);
                    }
                    catch (Exception ex)
                    {
                        var ErrMsg = "Exception converting XML item node to sample " + currentNode.Name;
                        ApplicationLogger.LogError(0, ErrMsg, ex);
                        throw new DataImportException(ErrMsg, ex);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Converts an individual XML node into a sampledata object
        /// </summary>
        /// <param name="itemNode">XML node containing data for 1 sample</param>
        /// <returns>SampleData object containing data from the XML node</returns>
        private SampleData ConvertXMLNodeToSample(XmlNode itemNode)
        {
            var retData = new SampleData(false);

            // Description (DMS.Name)
            var tempStr = GetNodeValue(itemNode, "Description");
            // Value is mandatory for this field, so check for it
            if (tempStr != "")
            {
                retData.DmsData.RequestName = tempStr;
            }
            else
            {
                ApplicationLogger.LogMessage(0, "Description field empty or missing. Import cannot be performed");
                return null;
            }

            // Selection Method (PAL.Method)
            retData.PAL.Method = GetNodeValue(itemNode, "Selection/Method");

            // Tray (PAL.Tray) (aka wellplate)
            retData.PAL.PALTray = GetNodeValue(itemNode, "Selection/Tray");

            // Vial (PAL.Vial) (aka well)
            var tmpStr = GetNodeValue(itemNode, "Selection/Vial");
            if (tmpStr == "")
            {
                retData.PAL.Well = 0;
            }
            else
            {
                retData.PAL.Well = int.Parse(tmpStr);
            }

            // Volume (Volume)
            retData.Volume = ConvertNullToDouble(GetNodeValue(itemNode, "Selection/Volume"));

            // Separation Method (Experiment.ExperimentName)
            var methodName = GetNodeValue(itemNode, "Separation/Method");
            retData.LCMethod = new LCMethod {
                Name = methodName
            };

            // Acquisition Method (InstrumentData.MethodName)
            retData.InstrumentData.MethodName = GetNodeValue(itemNode, "Acquisition/Method");

            // DMS RequestNumber (DMSData.RequestID)
            retData.DmsData.RequestID = ConvertNullToInt(GetNodeValue(itemNode, "DMS/RequestNumber"));

            // DMS Comment (DMSData.Comment)
            retData.DmsData.Comment = GetNodeValue(itemNode, "DMS/Comment");

            // DMS DatasetType (DMSData.DatasetType)
            retData.DmsData.DatasetType = GetNodeValue(itemNode, "DMS/DatasetType");

            // DMS Experiment (DMSData.Experiment)
            retData.DmsData.Experiment = GetNodeValue(itemNode, "DMS/Experiment");

            // DMS EMSLProposalID (DMSData.ProposalID)
            retData.DmsData.EMSLProposalID = GetNodeValue(itemNode, "DMS/EMSLProposalID");

            // DMS EMSLUsageType (DMSData.UsageType)
            retData.DmsData.EMSLUsageType = GetNodeValue(itemNode, "DMS/EMSLUsageType");

            // DMS EMSLUser (DMSData.UserList)
            retData.DmsData.UserList = GetNodeValue(itemNode, "DMS/EMSLUser");

            // It's all in, so return
            return retData;
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to an int
        /// </summary>
        /// <param name="value">String from XML parser</param>
        /// <returns>If input string is empty or null, returns 0. Otherwise returns input string converted to int</returns>
        private int ConvertNullToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            return int.TryParse(value, out int number) ? number : 0;
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to a double
        /// </summary>
        /// <param name="value">String from XML parser</param>
        /// <returns>If input string is empty or null, returns 0.0. Otherwise returns input string converted to double</returns>
        private double ConvertNullToDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            return double.TryParse(value, out double number) ? number : 0;
        }

        private string GetNodeValue(XmlNode itemNode, string nodeName)
        {
            var valueNode = itemNode.SelectSingleNode(nodeName);

            var value = valueNode?.InnerText;

            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value;
        }

        #endregion
    }
}