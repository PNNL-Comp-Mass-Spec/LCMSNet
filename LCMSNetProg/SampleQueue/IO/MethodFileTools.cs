﻿using System;
using System.IO;
using System.Threading.Tasks;
using LcmsNet.Data;
using LcmsNet.Method;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;

namespace LcmsNet.SampleQueue.IO
{
    /// <summary>
    /// Methods for copying method files and other data to DMS.
    /// </summary>
    public class MethodFileTools : IMethodWriter
    {
        #region Constants

        private const string METHOD_FOLDER_NAME = "MethodFiles";
        private const string LOCAL_METHOD_FOLDER_NAME = "CompletedMethods";

        #endregion

        #region Methods

        /// <summary>
        /// Writes information generated by an incomplete sample run.
        /// </summary>
        /// <param name="sample"></param>
        public static async void WriteIncompleteMethodFilesAsync(SampleData sample)
        {
            await Task.Run(() => WriteIncompleteMethodFiles(sample));
        }

        /// <summary>
        /// Writes information generated by an incomplete sample run.
        /// </summary>
        /// <param name="sample"></param>
        public static void WriteIncompleteMethodFiles(SampleData sample)
        {
            if (sample == null)
            {
                ApplicationLogger.LogError(0, "WriteIncompleteMethodFiles: No sample specified");
                return;
            }

            // Exit if method folder creation disabled
            if (!bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_CREATEMETHODFOLDERS)))
            {
                var msg = "WriteMethodFiles: Sample " + sample.Name +
                             ", Method folder creation disabled";
                ApplicationLogger.LogMessage(0, msg);
                return;
            }

            // Make the method transfer folder
            var localFolder = MakeLocalMethodFolder(sample);
            if (localFolder == "")
            {
                var message = "Could not create a folder for the output method data for: " + sample.ActualLCMethod.Name;
                ApplicationLogger.LogError(0, message);
                return;
            }

            // ----------------------------------------------------------------------------------------------------
            // Write the LC Method file
            // ----------------------------------------------------------------------------------------------------
            var methodFileName = GetMethodFileName(sample, ".incompleteLcmethod");
            var lcMethodFileNamePath = Path.Combine(localFolder, methodFileName);
            var lcWriter = new LCMethodWriter();
            try
            {
                lcWriter.WriteMethod(lcMethodFileNamePath, sample.ActualLCMethod);
            }
            catch (Exception ex)
            {
                var msg = "Could not write the LC Method file for: " + sample.ActualLCMethod.Name;
                ApplicationLogger.LogError(0, msg, ex, sample);
            }
            finally
            {
                lcWriter = null;
            }
        }

        /// <summary>
        /// Creates a folder for the methods associated with a sample, then copies it to a bionet transfer folder
        /// </summary>
        /// <param name="sample"></param>
        public static async void WriteMethodFilesAsync(SampleData sample)
        {
            await Task.Run(() => WriteMethodFiles(sample));
        }

        /// <summary>
        /// Creates a folder for the methods associated with a sample, then copies it to a bionet transfer folder
        /// </summary>
        /// <param name="sample"></param>
        public static void WriteMethodFiles(SampleData sample)
        {
            if (sample == null)
            {
                ApplicationLogger.LogError(0, "WriteMethodFiles: No sample specified");
                return;
            }

            // Exit if method folder creation disabled
            if (!bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_CREATEMETHODFOLDERS)))
            {
                var msg = "WriteMethodFiles: Sample " + sample.Name +
                             ", Method folder creation disabled";
                ApplicationLogger.LogMessage(0, msg);
                return;
            }

            // Make the method transfer folder
            var localFolder = MakeLocalMethodFolder(sample);
            if (localFolder == "")
            {
                var message = "Could not create a folder for the output method data for: " + sample.ActualLCMethod.Name;
                ApplicationLogger.LogError(0, message);
                return;
            }

            // ----------------------------------------------------------------------------------------------------
            // Write the LC Method file
            // ----------------------------------------------------------------------------------------------------
            var methodWriter = new LCMethodWriter();
            var methodFileName = GetMethodFileName(sample, ".lcmethod");
            var lcMethodFileNamePath = Path.Combine(localFolder, methodFileName);
            var lcWriter = new LCMethodWriter();
            try
            {
                lcWriter.WriteMethod(lcMethodFileNamePath, sample.ActualLCMethod);
            }
            catch (Exception ex)
            {
                var msg = "Could not write the LC Method file for: " + sample.ActualLCMethod.Name;
                ApplicationLogger.LogError(0, msg, ex, sample);
                return;
            }
            finally
            {
                methodWriter = null;
            }

            // ----------------------------------------------------------------------------------------------------
            // Now try to write all of the event performance data (e.g. pump methods etc.) to the server
            // ----------------------------------------------------------------------------------------------------
            //string performanceFile =
            //try
            //{
            //    sample.ActualLCMethod.WritePerformanceData(localFolder);
            //}
            //catch (Exception ex)
            //{
            //    string message = "Could not write the performance data files.";
            //    ApplicationLogger.LogError(0, message, ex, sample);
            //    return;
            //}

            // ----------------------------------------------------------------------------------------------------
            // Exit if method folder copy disabled
            // ----------------------------------------------------------------------------------------------------
            var shouldCopyFolder = bool.Parse(LCMSSettings.GetParameter(LCMSSettings.PARAM_COPYMETHODFOLDERS));
            if (!shouldCopyFolder)
            {
                var msg = "The method data was not copied to the server for: " + sample.Name +
                             ".  the method folder copy is disabled";
                ApplicationLogger.LogMessage(0, msg);
                return;
            }

            // Move the collected method data to the xfer server
            var remoteTargetFolder = CreateRemoteFolderPath();
            try
            {
                var remoteFolderExists = Directory.Exists(remoteTargetFolder);
                if (!remoteFolderExists)
                {
                    Directory.CreateDirectory(remoteTargetFolder);
                }

                var localName = Path.GetFileName(lcMethodFileNamePath);
                File.Copy(lcMethodFileNamePath, Path.Combine(remoteTargetFolder, localName), true);
                File.Delete(lcMethodFileNamePath);

                var message = string.Format("Method information for {0} was copied to {1}",
                    sample.Name,
                    remoteTargetFolder);
                ApplicationLogger.LogMessage(0, message, sample);
            }
            catch (Exception ex)
            {
                var message = string.Format("Method information for {0} was NOT copied to {1}",
                    sample.Name,
                    remoteTargetFolder);
                ApplicationLogger.LogError(0, message, ex, sample);
            }
        }

        /// <summary>
        /// Get the method file name for the provided dataset
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetMethodFileName(SampleData sample, string extension)
        {
            var datasetName = sample.Name;
            var outFileName =
                string.Format("{0}_{1:MM.dd.yyyy_hh.mm.ss}_{2}{3}",
                    LCMSSettings.GetParameter(LCMSSettings.PARAM_CARTNAME),
                    sample.RunStart,
                    datasetName,
                    extension);
            return outFileName;
        }

        /// <summary>
        /// Makes a local method folder named the same as the dataset folder
        /// </summary>
        /// <param name="sample">Sample we're making a request for</param>
        /// <returns>On success, string containing folder name/path; empty string on failure</returns>
        private static string MakeLocalMethodFolder(ISampleInfo sample)
        {
            // TODO: this line is here for upgrade compatibility - if the folder does not exist in ProgramData, but does in ProgramFiles, this will copy all existing contents.
            var loader = PersistDataPaths.GetDirectorySavePath(LOCAL_METHOD_FOLDER_NAME);

            // Verify local method folder exists. Otherwise, create the folder.
            var localFolder = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH),
                LOCAL_METHOD_FOLDER_NAME);
            if (!Directory.Exists(localFolder))
            {
                try
                {
                    Directory.CreateDirectory(localFolder);
                }
                catch (Exception ex)
                {
                    var message = "Could not create the method folder " + localFolder;
                    ApplicationLogger.LogError(0, message, ex, sample);
                    return string.Empty;
                }
            }
            return localFolder;
        }

        /// <summary>
        /// Test for presence of completed sample method folders that need to be moved to DMS
        /// </summary>
        /// <returns>TRUE if files found; FALSE otherwise</returns>
        public static bool CheckLocalMethodFolders()
        {
            // Check for presence of local method directory
            var localMethodXferFolder = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH),
                LOCAL_METHOD_FOLDER_NAME);
            if (!Directory.Exists(localMethodXferFolder))
                return false; // If no directory, there are no folders needing transfer

            // Get a list of the folders in the transfer folder
            var methodFolders = Directory.GetDirectories(localMethodXferFolder);

            // Check for method folders in the transfer folder
            if (methodFolders.Length < 1)
            {
                return false; // No method folders to copy
            }
            return true; // There are folders to copy
        }

        /// <summary>
        /// Moves local sample method files to the DMS transfer folder
        /// </summary>
        public static void MoveLocalMethodFiles()
        {
            var localFolder = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH),
                LOCAL_METHOD_FOLDER_NAME);

            if (!Directory.Exists(localFolder))
            {
                ApplicationLogger.LogError(0, "The local methods folder does not exist.");
                return;
            }

            var remoteFolder = CreateRemoteFolderPath();
            var exists = Directory.Exists(remoteFolder);
            if (!exists)
            {
                ApplicationLogger.LogError(0, "Creating the remote folder: " + remoteFolder);
                try
                {
                    Directory.CreateDirectory(remoteFolder);
                }
                catch
                {
                    //TODO: Chris/Brian Fix this!  Or Log that the directory is not available!
                }
            }

            // Get a list of the folders in the transfer folder
            var files = Directory.GetFiles(localFolder, "*.lcmethod");

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var remotePath = CreateRemoteFolderPath();

                    File.Copy(file, Path.Combine(remotePath, fileName), true);
                    File.Delete(file);
                    ApplicationLogger.LogMessage(1,
                        string.Format("Copying method file data.  Copied {0} to {1}.", file, remotePath));
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Could not copy the file " + file, ex);
                }
            }
        }

        /// <summary>
        /// Creates the remote system path.
        /// </summary>
        /// <returns></returns>
        public static string CreateRemoteFolderPath()
        {
            var path = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_TRIGGERFILEFOLDER), METHOD_FOLDER_NAME);
            var now = DateTime.Now;
            var month = now.Month;
            var year = now.Year;
            var quarter = (month / 4) + 1;
            path = Path.Combine(path, string.Format("{0}_{1}", year, quarter));
            return path;
        }

        #endregion

        #region IMethodWriter Members

        void IMethodWriter.WriteMethodFiles(ISampleInfo sample)
        {
            if (sample is SampleData sampleData)
                WriteMethodFiles(sampleData);
        }

        bool IMethodWriter.CheckLocalMethodFolders()
        {
            return CheckLocalMethodFolders();
        }

        void IMethodWriter.MoveLocalMethodFiles()
        {
            MoveLocalMethodFiles();
        }

        string IMethodWriter.CreateRemoteFolderPath()
        {
            return CreateRemoteFolderPath();
        }

        #endregion
    }
}