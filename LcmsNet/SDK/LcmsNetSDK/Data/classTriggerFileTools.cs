//*********************************************************************************************************
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
using LcmsNetDataClasses.Logging;

namespace LcmsNetDataClasses.Data
{
    /// <summary>
    /// Tools for generation of trigger files
    /// </summary>
    public class classTriggerFileTools
    {
        #region "Class variables"

        private static XmlDocument m_TriggerFileContents;

        #endregion

        #region "Methods"

        /// <summary>
        /// Generates a trigger file for a sample
        /// </summary>
        /// <param name="sample"></param>
        public static void GenerateTriggerFile(classSampleData sample)
        {
            /*
                var createTriggerFiles = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CREATETRIGGERFILES, false);
                if (!createTriggerFiles)
                {
                    string msg = "Generate Trigger File: Sample " + sample.DmsData.DatasetName + ", Trigger file creation disabled";
                    classApplicationLogger.LogMessage(0, msg);
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
        private static void GenerateXmlDoc(classSampleData sample)
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
            AddParam(rootElement, "Instrument Name", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_INSTNAME));
            AddParam(rootElement, "Separation Type", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_SEPARATIONTYPE));
            AddParam(rootElement, "LC Cart Name", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME));
            AddParam(rootElement, "LC Cart Config", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME));
            AddParam(rootElement, "LC Column", sample.ColumnData.Name);
            AddParam(rootElement, "Wellplate Number", sample.PAL.WellPlate);
            AddParam(rootElement, "Well Number", sample.PAL.Well.ToString());
            AddParam(rootElement, "Dataset Type", sample.DmsData.DatasetType);
            // AddParam(rootElement, "Operator (PRN)", sample.Operator);
            AddParam(rootElement, "Operator (PRN)", classLCMSSettings.GetParameter(classLCMSSettings.PARAM_OPERATOR));
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
                usage = sample.DmsData.UsageType;
                userList = sample.DmsData.UserList;
                proposal = sample.DmsData.ProposalID;
            }

            AddParam(rootElement, "Request", sample.DmsData.RequestID.ToString());
            AddParam(rootElement, "EMSL Proposal ID", proposal);
            AddParam(rootElement, "EMSL Usage Type", usage);
            AddParam(rootElement, "EMSL Users List", userList);
            AddParam(rootElement, "Run Start", sample.LCMethod.ActualStart.ToString("MM/dd/yyyy HH:mm:ss"));
            AddParam(rootElement, "Run Finish", sample.LCMethod.ActualEnd.ToString("MM/dd/yyyy HH:mm:ss"));
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
                classApplicationLogger.LogError(0, "Exception creating trigger file", ex);
            }
        }

        /// <summary>
        /// Write the trigger file
        /// </summary>
        /// <param name="doc">XML document to be written</param>
        /// <param name="sample">Name of the sample this trigger file is for</param>
        /// <param name="datasetName">Dataset name</param>
        private static void SaveFile(XmlDocument doc, classSampleData sample, string datasetName)
        {
            var sampleName = sample.DmsData.DatasetName;
            var outFileName = classSampleData.GetTriggerFileName(sample, ".xml");

            // Write trigger file to local folder
            var appPath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH);
            var outFilePath = Path.Combine(appPath, "TriggerFiles");
            var wasLocalFileCreated = false;

            // If local folder doen't exist, then create it
            if (!Directory.Exists(outFilePath))
            {
                try
                {
                    Directory.CreateDirectory(outFilePath);
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Exception creating local trigger file folder", ex);
                    return;
                }
            }

            var outFileNamePath = Path.Combine(outFilePath, outFileName);

            // Copy the file path
            try
            {
                var outputFile = new FileStream(outFileNamePath, FileMode.Create, FileAccess.Write);
                doc.Save(outputFile);
                outputFile.Close();
                classApplicationLogger.LogMessage(0, "Local trigger file created for sample " + sampleName);
                wasLocalFileCreated = true;
            }
            catch (Exception ex)
            {
                // If local write failed, log it
                var msg = "Error creating local trigger file for sample " + sampleName;
                classApplicationLogger.LogError(0, msg, ex);
            }

            try
            {
                if (bool.Parse(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_COPYTRIGGERFILES)))
                {
                    // If the file was created locally...copy it
                    if (wasLocalFileCreated)
                    {
                        outFilePath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TRIGGERFILEFOLDER);
                        MoveLocalFile(outFileNamePath, Path.Combine(outFilePath, outFileName));
                    }
                    else
                    {
                        outFileNamePath = Path.Combine(outFilePath, outFileName);
                        outFilePath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TRIGGERFILEFOLDER);

                        // Attempt to write the trigger file to a remote server
                        var outputFile = new FileStream(outFileNamePath, FileMode.Create, FileAccess.Write);
                        doc.Save(outputFile);
                        outputFile.Close();
                        classApplicationLogger.LogMessage(0,
                            "Remote trigger file created for sample " + sample.DmsData.DatasetName);
                    }
                }
                else
                {
                    var msg = "Generate Trigger File: Sample " + datasetName + ", Trigger file copy disabled";
                    classApplicationLogger.LogMessage(0, msg);
                }
            }
            catch (Exception ex)
            {
                // If remote write failed or disabled, log and try to write locally
                var msg = "Remote trigger file copy failed or disabled, sample " + sample.DmsData.DatasetName +
                          ". Creating file locally.";
                classApplicationLogger.LogError(0, msg, ex);
            }
        }

        /// <summary>
        /// Tests for presence of local trigger files
        /// </summary>
        /// <returns>TRUE if trigger files present, FALSE otherwise</returns>
        public static bool CheckLocalTriggerFiles()
        {
            // Check for presence of local trigger file directory
            var localFolderPath = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), "TriggerFiles");

            // If local folder doen't exist, then there are no local trigger files
            if (!Directory.Exists(localFolderPath)) return false;

            var triggerFiles = Directory.GetFiles(localFolderPath);
            if (triggerFiles.Length < 1)
            {
                // No files found
                return false;
            }
            // At least one file found
            return true;
        }

        /// <summary>
        /// Moves local trigger files to a remote server
        /// </summary>
        public static void MoveLocalTriggerFiles()
        {
            var localFolderPath = Path.Combine(classLCMSSettings.GetParameter(classLCMSSettings.PARAM_APPLICATIONPATH), "TriggerFiles");

            // Verify local trigger file directory exists
            if (!Directory.Exists(localFolderPath))
            {
                classApplicationLogger.LogMessage(0, "Local trigger file directory not found");
                return;
            }

            // Get a list of local trigger files
            var triggerFiles = Directory.GetFiles(localFolderPath);
            if (triggerFiles.Length < 1)
            {
                // No files found
                classApplicationLogger.LogMessage(0, "No files in local trigger file directory");
                return;
            }

            // Move the local files to the remote server
            var remoteFolderPath = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_TRIGGERFILEFOLDER);

            // Verfiy remote folder connection exists
            if (!Directory.Exists(remoteFolderPath))
            {
                var msg = "MoveLocalTriggerFiles: Unable to connect to remote folder " + remoteFolderPath;
                classApplicationLogger.LogError(0, msg);
                return;
            }

            foreach (var localFile in triggerFiles)
            {
                var fi = new FileInfo(localFile);
                var targetFilePath = Path.Combine(remoteFolderPath, fi.Name);

                var success = MoveLocalFile(fi.FullName, targetFilePath);
                fi.Refresh();

                if (success && fi.Exists)
                {
                    // Move the file into a subfolder so that it doesn't get processed the next time the program starts
                    try
                    {
                        var diLocalArchiveFolder =
                            new DirectoryInfo(Path.Combine(fi.Directory.FullName,
                                DateTime.Now.Year.ToString(CultureInfo.InvariantCulture)));
                        if (!diLocalArchiveFolder.Exists)
                            diLocalArchiveFolder.Create();

                        targetFilePath = Path.Combine(diLocalArchiveFolder.FullName, fi.Name);
                        success = MoveLocalFile(fi.FullName, targetFilePath);

                        fi.Refresh();

                        if (success && fi.Exists)
                            fi.Delete();
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogError(0, "Exception archiving local trigger file " + fi.FullName, ex);
                    }
                }
            }
        }

        private static bool MoveLocalFile(string sourceFile, string targetFile)
        {
            try
            {
                if (!File.Exists(targetFile))
                {
                    File.Move(sourceFile, targetFile);
                    classApplicationLogger.LogMessage(0, "Trigger file " + sourceFile + " moved");
                    return true;
                }

                classApplicationLogger.LogMessage(0, "Trigger file " + targetFile + " exists remotely.");
                return true;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Exception moving trigger file " + sourceFile, ex);
                return false;
            }
        }

        #endregion
    }
}
