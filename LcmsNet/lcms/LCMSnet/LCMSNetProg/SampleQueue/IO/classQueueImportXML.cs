//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 04/14/2009
//
// Last modified 04/14/2009
//                      - 12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Method;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Imports an XML file from LCMS
    /// </summary>
    class classQueueImportXML : ISampleQueueReader
    {
        //*********************************************************************************************************
        // 
        //**********************************************************************************************************

        #region "Methods"

        /// <summary>
        /// Reads the XML file into a list
        /// </summary>
        /// <param name="path">Name and path of file to import</param>
        /// <returns>List<classSampleData> containing samples read from XML file</returns>
        public List<classSampleData> ReadSamples(string path)
        {
            var returnList = new List<classSampleData>();

            // Verify input file exists
            if (!File.Exists(path))
            {
                var ErrMsg = "Import file " + path + " not found";
                classApplicationLogger.LogMessage(0, ErrMsg);
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
                classApplicationLogger.LogError(0, ErrMsg, ex);
                throw new classDataImportException(ErrMsg, ex);
            }

            // Get all the nodes under QueueSettings node
            var nodeList = doc.SelectNodes("//QueueSettings/*");

            // If no nodes found, report and exit
            if (nodeList.Count < 1)
            {
                var ErrMsg = "No data found for import in file " + path;
                classApplicationLogger.LogMessage(0, ErrMsg);
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
                        classApplicationLogger.LogError(0, ErrMsg, ex);
                        throw new classDataImportException(ErrMsg, ex);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Converts an individual XML node into a sampledata object
        /// </summary>
        /// <param name="ItemNode">XML node containing data for 1 sample</param>
        /// <returns>classSampleData object containing data from the XML node</returns>
        private classSampleData ConvertXMLNodeToSample(XmlNode ItemNode)
        {
            var retData = new classSampleData(false);
            string tempStr;

            // Description (DMS.Name)
            tempStr = ConvertNullToString(ItemNode.SelectSingleNode("Description").InnerText);
            // Value is mandatory for this field, so check for it
            if (tempStr != "")
            {
                retData.DmsData.RequestName = tempStr;
            }
            else
            {
                classApplicationLogger.LogMessage(0, "Description field empty or missing. Import cannot be performed");
                return null;
            }

            // Selection Method (PAL.Method)
            retData.PAL.Method = ConvertNullToString(ItemNode.SelectSingleNode("Selection/Method").InnerText);

            // Tray (PAL.Tray) (aka wellplate)
            retData.PAL.PALTray = ConvertNullToString(ItemNode.SelectSingleNode("Selection/Tray").InnerText);

            // Vial (PAL.Vial) (aka well)
            var tmpStr = ConvertNullToString(ItemNode.SelectSingleNode("Selection/Vial").InnerText);
            if (tmpStr == "")
            {
                retData.PAL.Well = 0;
            }
            else
            {
                retData.PAL.Well = int.Parse(tmpStr);
            }

            // Volume (Volume)
            retData.Volume = ConvertNullToDouble(ItemNode.SelectSingleNode("Selection/Volume").InnerText);

            // Separation Method (Experiment.ExperimentName)
            var methodName = ConvertNullToString(ItemNode.SelectSingleNode("Separation/Method").InnerText);
            retData.LCMethod = new classLCMethod();
            retData.LCMethod.Name = methodName;

            // Acquisition Method (InstrumentData.MethodName)
            retData.InstrumentData.MethodName =
                ConvertNullToString(ItemNode.SelectSingleNode("Acquisition/Method").InnerText);

            // DMS RequestNumber (DMSData.RequestID)
            retData.DmsData.RequestID = ConvertNullToInt(ItemNode.SelectSingleNode("DMS/RequestNumber").InnerText);

            // DMS Comment (DMSData.Comment)
            retData.DmsData.Comment = ConvertNullToString(ItemNode.SelectSingleNode("DMS/Comment").InnerText);

            // DMS DatasetType (DMSData.DatasetType)
            retData.DmsData.DatasetType = ConvertNullToString(ItemNode.SelectSingleNode("DMS/DatasetType").InnerText);

            // DMS Experiment (DMSData.Experiment)
            retData.DmsData.Experiment = ConvertNullToString(ItemNode.SelectSingleNode("DMS/Experiment").InnerText);

            // DMS EMSLProposalID (DMSData.ProposalID)
            retData.DmsData.ProposalID = ConvertNullToString(ItemNode.SelectSingleNode("DMS/EMSLProposalID").InnerText);

            // DMS EMSLUsageType (DMSData.UsageType)
            retData.DmsData.UsageType = ConvertNullToString(ItemNode.SelectSingleNode("DMS/EMSLUsageType").InnerText);

            // DMS EMSLUser (DMSData.UserList)
            retData.DmsData.UserList = ConvertNullToString(ItemNode.SelectSingleNode("DMS/EMSLUser").InnerText);

            // It's all in, so return
            return retData;
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to a string
        /// </summary>
        /// <param name="InpVal">String from XML parser</param>
        /// <returns>If input string is empty or null, returns empty string. Otherwise returns input string</returns>
        private string ConvertNullToString(string InpVal)
        {
            if (string.IsNullOrEmpty(InpVal))
            {
                return string.Empty;
            }
            return InpVal;
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to an int
        /// </summary>
        /// <param name="InpVal">String from XML parser</param>
        /// <returns>If input string is empty or null, returns 0. Otherwise returns input string converted to int</returns>
        private int ConvertNullToInt(string InpVal)
        {
            if (string.IsNullOrEmpty(InpVal))
            {
                return 0;
            }
            return int.Parse(InpVal);
        }

        /// <summary>
        /// Utility method to convert a null or empty string value in the XML file to a double
        /// </summary>
        /// <param name="InpVal">String from XML parser</param>
        /// <returns>If input string is empty or null, returns 0.0. Otherwise returns input string converted to double</returns>
        private double ConvertNullToDouble(string InpVal)
        {
            if (string.IsNullOrEmpty(InpVal))
            {
                return 0.0;
            }
            return double.Parse(InpVal);
        }

        #endregion
    }
}