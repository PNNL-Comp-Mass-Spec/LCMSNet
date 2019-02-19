using System;
using System.Globalization;
using System.IO;
using LcmsNetData.Logging;

namespace LcmsNetData.Data
{
    public class TriggerFileUtils
    {
        /// <summary>
        /// Tests for presence of local trigger files
        /// </summary>
        /// <returns>TRUE if trigger files present, FALSE otherwise</returns>
        public static bool CheckLocalTriggerFiles()
        {
            // Check for presence of local trigger file directory
            var localFolderPath = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), "TriggerFiles");

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

            // Verfiy remote folder connection exists
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
