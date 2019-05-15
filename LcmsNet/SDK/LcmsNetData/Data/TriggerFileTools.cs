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
using System.IO;
using System.Xml;
using LcmsNetData.Logging;
using LcmsNetData.System;

namespace LcmsNetData.Data
{
    /// <summary>
    /// Tools for generation of trigger files
    /// </summary>
    public class TriggerFileTools
    {
        #region "Methods"

        /// <summary>
        /// Generates a trigger file for a sample
        /// </summary>
        /// <param name="sample"></param>
        public static void GenerateTriggerFile(ITriggerFileData sample)
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
            var triggerFileContents = GenerateXmlDoc(sample);

            // Write the document to the file
            SaveFile(triggerFileContents, sample, sample.DmsData.DatasetName);
        }

        /// <summary>
        /// Generates the XML-formatted trigger file contents
        /// </summary>
        /// <param name="sample">sample object for sample that was run</param>
        private static XmlDocument GenerateXmlDoc(ITriggerFileData sample)
        {
            // Create and initialize the document
            var triggerFileContents = new XmlDocument();
            var docDeclaration = triggerFileContents.CreateXmlDeclaration("1.0", null, null);
            triggerFileContents.AppendChild(docDeclaration);

            // Add dataset (Root) element
            var rootElement = triggerFileContents.CreateElement("Dataset");
            triggerFileContents.AppendChild(rootElement);

            // Add the parameters
            AddParam(rootElement, "Dataset Name", sample.DmsData.DatasetName);
            AddParam(rootElement, "Experiment Name", TrimWhitespace(sample.DmsData.Experiment));
            AddParam(rootElement, "Instrument Name", TrimWhitespace(sample.InstrumentName));
            AddParam(rootElement, "Capture Subfolder", TrimWhitespace(sample.CaptureSubdirectoryPath));
            AddParam(rootElement, "Separation Type", TrimWhitespace(sample.SeparationType));
            AddParam(rootElement, "LC Cart Name", TrimWhitespace(sample.DmsData.CartName));
            AddParam(rootElement, "LC Cart Config", TrimWhitespace(sample.DmsData.CartConfigName));
            AddParam(rootElement, "LC Column", TrimWhitespace(sample.ColumnName));

            if (sample is ITriggerFilePalData lcSample)
            {
                AddParam(rootElement, "Wellplate Number", TrimWhitespace(lcSample.PAL.WellPlate));
                AddParam(rootElement, "Well Number", TrimWhitespace(lcSample.PAL.Well.ToString()));
            }

            AddParam(rootElement, "Dataset Type", TrimWhitespace(sample.DmsData.DatasetType));

            AddParam(rootElement, "Operator (PRN)", TrimWhitespace(sample.Operator));
            AddParam(rootElement, "Comment", TrimWhitespace(sample.DmsData.Comment));
            AddParam(rootElement, "Interest Rating", TrimWhitespace(sample.InterestRating ?? "Unreviewed"));

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
            AddParam(rootElement, "Run Start", sample.RunStart.ToString("MM/dd/yyyy HH:mm:ss"));
            AddParam(rootElement, "Run Finish", sample.RunFinish.ToString("MM/dd/yyyy HH:mm:ss"));
            // Removed to fix date comparison problems during DMS data import
            //AddParam(rootElement, "Run Finish UTC",   ConvertTimeLocalToUtc(sample.LCMethod.ActualEnd));

            return triggerFileContents;
        }

        private static string TrimWhitespace(string metadata)
        {
            if (string.IsNullOrWhiteSpace(metadata))
                return string.Empty;

            return metadata.Trim();
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
        /// <param name="parent">Parent element to add the parameter to</param>
        /// <param name="paramName">Name of the parameter to add</param>
        /// <param name="paramValue">Value of the parameter</param>
        private static void AddParam(XmlElement parent, string paramName, string paramValue)
        {
            try
            {
                var newElement = parent.OwnerDocument.CreateElement("Parameter");
                var nameAttr = parent.OwnerDocument.CreateAttribute("Name");
                nameAttr.Value = paramName;
                newElement.Attributes.Append(nameAttr);
                var valueAttr = parent.OwnerDocument.CreateAttribute("Value");
                valueAttr.Value = paramValue;
                newElement.Attributes.Append(valueAttr);
                parent.AppendChild(newElement);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception creating trigger file", ex);
            }
        }

        /// <summary>
        /// Get the trigger file name for the provided dataset
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetTriggerFileName(ITriggerFileData sample, string extension)
        {
            var datasetName = sample.DmsData.DatasetName;
            var outFileName =
                string.Format("{0}_{1:MM.dd.yyyy_hh.mm.ss}_{2}{3}",
                    sample.DmsData.CartName,
                    //DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)),
                    sample.RunStart,
                    datasetName,
                    extension);
            return outFileName;
        }

        /// <summary>
        /// Write the trigger file
        /// </summary>
        /// <param name="doc">XML document to be written</param>
        /// <param name="sample">Name of the sample this trigger file is for</param>
        /// <param name="datasetName">Dataset name</param>
        private static void SaveFile(XmlDocument doc, ITriggerFileData sample, string datasetName)
        {
            var sampleName = sample.DmsData.DatasetName;
            var outFileName = GetTriggerFileName(sample, ".xml");

            // Write trigger file to local folder
            var appPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH);
            var localTriggerFolderPath = Path.Combine(appPath, "TriggerFiles");
            var wasLocalFileCreated = false;

            var localTriggerFilePath = Path.Combine(localTriggerFolderPath, outFileName);

            // If local folder doen't exist, then create it
            if (!Directory.Exists(localTriggerFolderPath))
            {
                try
                {
                    /**/
                    // TODO: this line is here for upgrade compatibility - if the folder does not exist in ProgramData, but does in ProgramFiles, this will copy all existing contents.
                    var loader = PersistDataPaths.GetDirectorySavePath("TriggerFiles");
                    /*/
                    // TODO: This line needs to be uncommented when the above is removed.
                    Directory.CreateDirectory(localTriggerFolderPath);
                    /**/

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
