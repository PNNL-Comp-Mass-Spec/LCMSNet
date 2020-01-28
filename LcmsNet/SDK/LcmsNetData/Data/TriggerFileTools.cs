using System;
using System.Globalization;
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
        /// <summary>
        /// Generates a trigger file for a sample
        /// </summary>
        /// <param name="sample"></param>
        public static string GenerateTriggerFile(ITriggerFileData sample)
        {
            /*
             * NOTE: Disabled because the 'CopyTriggerFiles' setting allows us to create the trigger file locally, but not copy it to the server.
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
            return SaveFile(triggerFileContents, sample);
        }

        /// <summary>
        /// Generates the XML-formatted trigger file contents
        /// </summary>
        /// <param name="sample">sample object for sample that was run</param>
        protected static XmlDocument GenerateXmlDoc(ITriggerFileData sample)
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
            AddParam(rootElement, "Work Package", TrimWhitespace(sample.DmsData.WorkPackage));
            AddParam(rootElement, "Comment", TrimWhitespace(sample.DmsData.CommentComplete));
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
                proposal = sample.DmsData.EMSLProposalID;
                usage = sample.DmsData.EMSLUsageType;
                userList = sample.DmsData.UserList;
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
        protected static string SaveFile(XmlDocument doc, ITriggerFileData sample)
        {
            var sampleName = sample.DmsData.DatasetName;
            var outFileName = GetTriggerFileName(sample, ".xml");

            try
            {
                var copyTriggerFiles = LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYTRIGGERFILES, false);
                if (copyTriggerFiles)
                {
                    var remoteTriggerFolderPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER);
                    var remoteTriggerFilePath = Path.Combine(remoteTriggerFolderPath, outFileName);

                    // Attempt to write the trigger file directly to the remote server
                    var outputFile = new FileStream(remoteTriggerFilePath, FileMode.Create, FileAccess.Write);
                    doc.Save(outputFile);
                    outputFile.Close();
                    ApplicationLogger.LogMessage(0, "Remote trigger file created for dataset " + sample.DmsData.DatasetName);

                    // File successfully created remotely, so exit the procedure
                    return remoteTriggerFilePath;
                }

                // Skip remote file creation since CopyTriggerFiles is false
                var msg = "Generate Trigger File: Dataset " + sample.DmsData.DatasetName + ", Remote Trigger file creation disabled";
                ApplicationLogger.LogMessage(0, msg);
            }
            catch (Exception ex)
            {
                // If remote write failed or disabled, log and try to write locally
                var msg = "Remote trigger file creation failed, dataset " + sample.DmsData.DatasetName + ". Creating file locally.";
                ApplicationLogger.LogError(0, msg, ex);
            }

            // Write trigger file to local folder
            var appPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH);
            var localTriggerFolderPath = Path.Combine(appPath, "TriggerFiles");

            // If local folder doesn't exist, then create it
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
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Exception creating local trigger file folder", ex);
                    return string.Empty;
                }
            }

            // Create the trigger file on the local computer
            var localTriggerFilePath = Path.Combine(localTriggerFolderPath, outFileName);
            try
            {
                var outputFile = new FileStream(localTriggerFilePath, FileMode.Create, FileAccess.Write);
                doc.Save(outputFile);
                outputFile.Close();
                ApplicationLogger.LogMessage(0, "Local trigger file created for dataset " + sampleName);
                return localTriggerFilePath;
            }
            catch (Exception ex)
            {
                // If local write failed, log it
                var msg = "Error creating local trigger file for dataset " + sampleName;
                ApplicationLogger.LogError(0, msg, ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Tests for presence of local trigger files
        /// </summary>
        /// <returns>TRUE if trigger files present, FALSE otherwise</returns>
        public static bool CheckLocalTriggerFiles()
        {
            // Check for presence of local trigger file directory
            var localFolderPath = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), "TriggerFiles");

            // If local folder doesn't exist, then there are no local trigger files
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
            var localFolderPath = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), "TriggerFiles");

            // Verify local trigger file directory exists
            if (!Directory.Exists(localFolderPath))
            {
                ApplicationLogger.LogMessage(0, "Local trigger file directory not found");
                return;
            }

            // Get a list of local trigger files
            var triggerFiles = Directory.GetFiles(localFolderPath);
            if (triggerFiles.Length < 1)
            {
                // No files found
                ApplicationLogger.LogMessage(0, "No files in local trigger file directory");
                return;
            }

            // Move the local files to the remote server
            var remoteFolderPath = LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER);

            // Verify remote folder connection exists
            if (!Directory.Exists(remoteFolderPath))
            {
                var msg = "MoveLocalTriggerFiles: Unable to connect to remote folder " + remoteFolderPath;
                ApplicationLogger.LogError(0, msg);
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
                        ApplicationLogger.LogError(0, "Exception archiving local trigger file " + fi.FullName, ex);
                    }
                }
            }
        }

        public static bool MoveLocalFile(string sourceFile, string targetFile)
        {
            try
            {
                if (!File.Exists(targetFile))
                {
                    File.Move(sourceFile, targetFile);
                    ApplicationLogger.LogMessage(0, "Trigger file " + sourceFile + " moved");
                    return true;
                }

                ApplicationLogger.LogMessage(0, "Trigger file " + targetFile + " already exists remotely; not overwriting.");
                return true;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception moving trigger file " + sourceFile, ex);
                return false;
            }
        }
    }
}
