﻿//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 12/03/2009
//
// Updates:
// -  02/22/2011 (DAC) - Removed Run Finish UTC param to fix problem with data import in DMS
// -  02/23/2011 (DAC) - Changed Operator output field to use settings value instead of sample value
//*********************************************************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Tools for generation of trigger files
    /// </summary>
    public class TriggerFileTools
    {
        #region "Class variables"

        private static XmlDocument m_TriggerFileContents;

        #endregion

        #region "Methods"

        /// <summary>
        /// Generates a trigger file for a sample
        /// </summary>
        /// <param name="sample"></param>
        public static void GenerateTriggerFile(SampleData sample)
        {
            /*
                var createTriggerFiles = LCMSSettings.GetParameter(LCMSSettings.PARAM_CREATETRIGGERFILES, false);
                if (!createTriggerFiles)
                {
                    string msg = "Generate Trigger File: Sample " + sample.DmsData.DatasetName + ", Trigger file creation disabled";
                    ApplicationLogger.LogMessage(0, msg);
                    return;
                }
            */

            // Create an XML document containing the trigger file's contents
            GenerateXmlDoc(sample);

            // Write the document to the file
            SaveFile(m_TriggerFileContents, sample, sample.DmsData.DatasetName);
        }

        /// <summary>
        /// Generates the XML-formatted trigger file contents
        /// </summary>
        /// <param name="sample">sample object for sample that was run</param>
        private static void GenerateXmlDoc(SampleData sample)
        {
            // Create and initialize the document
            m_TriggerFileContents = new XmlDocument();
            var docDeclaration = m_TriggerFileContents.CreateXmlDeclaration("1.0", null, null);
            m_TriggerFileContents.AppendChild(docDeclaration);

            // Add dataset (Root) element
            var rootElement = m_TriggerFileContents.CreateElement("Dataset");
            m_TriggerFileContents.AppendChild(rootElement);

            // Add the parameters
            AddParam(rootElement, "Dataset Name", sample.DmsData.DatasetName);
            AddParam(rootElement, "Experiment Name", sample.DmsData.Experiment);
            AddParam(rootElement, "Instrument Name", LCMSSettings.GetParameter(LCMSSettings.PARAM_INSTNAME));
            AddParam(rootElement, "Separation Type", LCMSSettings.GetParameter(LCMSSettings.PARAM_SEPARATIONTYPE));
            AddParam(rootElement, "LC Cart Name", LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME));
            AddParam(rootElement, "LC Cart Config", LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTCONFIGNAME));
            AddParam(rootElement, "LC Column", sample.ColumnData.Name);
            AddParam(rootElement, "Wellplate Number", sample.PAL.WellPlate);
            AddParam(rootElement, "Well Number", sample.PAL.Well.ToString());
            AddParam(rootElement, "Dataset Type", sample.DmsData.DatasetType);
            // AddParam(rootElement, "Operator (PRN)", sample.Operator);
            AddParam(rootElement, "Operator (PRN)", LCMSSettings.GetParameter(LCMSSettings.PARAM_OPERATOR));
            AddParam(rootElement, "Comment", "");
            AddParam(rootElement, "Interest Rating", "Unreviewed");

            //
            // BLL: Added to appease the trigger file gods, so that we don't
            // confuse DMS with EMSL related data when the requests are already fulfilled.
            //
            var usage = "";
            var userList = "";
            var proposal = "";
            if (sample.DmsData.RequestID <= 0)
            {
                usage = sample.DmsData.EMSLUsageType;
                userList = sample.DmsData.UserList;
                proposal = sample.DmsData.EMSLProposalID;
            }

            AddParam(rootElement, "Request", sample.DmsData.RequestID.ToString());
            AddParam(rootElement, "EMSL Proposal ID", proposal);
            AddParam(rootElement, "EMSL Usage Type", usage);
            AddParam(rootElement, "EMSL Users List", userList);
            AddParam(rootElement, "Run Start", sample.ActualLCMethod.ActualStart.ToString("MM/dd/yyyy HH:mm:ss"));
            AddParam(rootElement, "Run Finish", sample.ActualLCMethod.ActualEnd.ToString("MM/dd/yyyy HH:mm:ss"));
            // Removed to fix date comparison problems during DMS data import
            //AddParam(rootElement, "Run Finish UTC",   ConvertTimeLocalToUtc(sample.LCMethod.ActualEnd));
        }

        /// <summary>
        /// Converts a string representing a local time to UTC time
        /// </summary>
        /// <param name="localTime">Local time</param>
        /// <returns></returns>
        private static string ConvertTimeLocalToUtc(DateTime localTime)
        {
            // First convert the local time string to a date/time object
            //DateTime localTime = DateTime.Parse(localTime);

            // Convert the local time to UTC time
            var utcTime = localTime.ToUniversalTime();

            return utcTime.ToString("MM/dd/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Adds a trigger file parameter to the XML document defining the file contents
        /// </summary>
        /// <param name="Parent">Parent element to add the parameter to</param>
        /// <param name="paramName">Name of the parameter to add</param>
        /// <param name="paramValue">Value of the parameter</param>
        private static void AddParam(XmlElement Parent, string paramName, string paramValue)
        {
            try
            {
                var newElement = m_TriggerFileContents.CreateElement("Parameter");
                var nameAttr = m_TriggerFileContents.CreateAttribute("Name");
                nameAttr.Value = paramName;
                newElement.Attributes.Append(nameAttr);
                var valueAttr = m_TriggerFileContents.CreateAttribute("Value");
                valueAttr.Value = paramValue;
                newElement.Attributes.Append(valueAttr);
                Parent.AppendChild(newElement);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception creating trigger file", ex);
            }
        }

        /// <summary>
        /// Write the trigger file
        /// </summary>
        /// <param name="doc">XML document to be written</param>
        /// <param name="sample">Name of the sample this trigger file is for</param>
        /// <param name="datasetName">Dataset name</param>
        private static void SaveFile(XmlDocument doc, SampleData sample, string datasetName)
        {
            var sampleName = sample.DmsData.DatasetName;
            var outFileName = SampleData.GetTriggerFileName(sample, ".xml");

            // Write trigger file to local folder
            var appPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONPATH);
            var localTriggerFolderPath = Path.Combine(appPath, "TriggerFiles");
            var wasLocalFileCreated = false;

            var localTriggerFilePath = Path.Combine(localTriggerFolderPath, outFileName);

            // If local folder doen't exist, then create it
            if (!Directory.Exists(localTriggerFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(localTriggerFolderPath);

                    // Create the trigger file on the local computer
                    try
                    {
                        var outputFile = new FileStream(localTriggerFilePath, FileMode.Create, FileAccess.Write);
                        doc.Save(outputFile);
                        outputFile.Close();
                        ApplicationLogger.LogMessage(0, "Local trigger file created for sample " + sampleName);
                        wasLocalFileCreated = true;
                    }
                    catch (Exception ex)
                    {
                        // If local write failed, log it
                        var msg = "Error creating local trigger file for sample " + sampleName;
                        ApplicationLogger.LogError(0, msg, ex);
                    }

                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Exception creating local trigger file folder", ex);
                }
            }

            try
            {
                var copyTriggerFiles = LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYTRIGGERFILES, false);
                if (copyTriggerFiles)
                {
                    var remoteTriggerFolderPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER);

                    // If the file was created locally...copy it
                    if (wasLocalFileCreated)
                    {
                        TriggerFileUtils.MoveLocalFile(localTriggerFilePath, Path.Combine(remoteTriggerFolderPath, outFileName));
                    }
                    else
                    {
                        var remoteTriggerFilePath = Path.Combine(remoteTriggerFolderPath, outFileName);

                        // Attempt to write the trigger file directly to the remote server
                        var outputFile = new FileStream(remoteTriggerFilePath, FileMode.Create, FileAccess.Write);
                        doc.Save(outputFile);
                        outputFile.Close();
                        ApplicationLogger.LogMessage(0,
                            "Remote trigger file created for sample " + sample.DmsData.DatasetName);
                    }
                }
                else
                {
                    var msg = "Generate Trigger File: Sample " + datasetName + ", Trigger file copy disabled";
                    ApplicationLogger.LogMessage(0, msg);
                }
            }
            catch (Exception ex)
            {
                // If remote write failed or disabled, log and try to write locally
                var msg = "Remote trigger file copy failed or disabled, sample " + sample.DmsData.DatasetName;
                ApplicationLogger.LogError(0, msg, ex);
            }
        }

        /// <summary>
        /// Tests for presence of local trigger files
        /// </summary>
        /// <returns>TRUE if trigger files present, FALSE otherwise</returns>
        public static bool CheckLocalTriggerFiles()
        {
            return TriggerFileUtils.CheckLocalTriggerFiles();
        }

        /// <summary>
        /// Moves local trigger files to a remote server
        /// </summary>
        public static void MoveLocalTriggerFiles()
        {
            TriggerFileUtils.MoveLocalTriggerFiles();
        }

        #endregion
    }
}
