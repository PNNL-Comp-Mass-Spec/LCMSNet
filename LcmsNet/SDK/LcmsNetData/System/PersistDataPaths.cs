using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LcmsNetData.System
{
    public static class PersistDataPaths
    {
        /// <summary>
        /// Name of the app - used for ProgramData directory name
        /// </summary>
        public static string AppName { get; private set; } = "LCMSNet";

        /// <summary>
        /// Sets the app name to a non-default value
        /// </summary>
        /// <param name="appName"></param>
        public static void SetAppName(string appName)
        {
            AppName = appName;
        }

        /// <summary>
        /// The directory where the exe file is located, and where data may have been persisted before
        /// </summary>
        public static string ProgramExeDirectory { get; }

        static PersistDataPaths()
        {
            ProgramExeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// Gets the local data storage path; if the settings file does not contain a value for "LocalDataPath", defaults to %AppData%\[AppName]
        /// </summary>
        public static string LocalDataPath
        {
            get
            {
                var path = LCMSSettings.GetParameter(LCMSSettings.PARAM_LocalDataPath);

                if (string.IsNullOrWhiteSpace(path))
                {
                    // Use 'Environment.SpecialFolder.CommonApplicationData' to get %ProgramData% instead.
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
                }

                return path;
            }
        }

        /// <summary>
        /// Program data path; here for compatibility
        /// </summary>
        private static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);

        /// <summary>
        /// Get the path where the file should be saved
        /// </summary>
        /// <param name="fileSubPath"></param>
        /// <returns></returns>
        public static string GetFileSavePath(string fileSubPath)
        {
            // Prefer the local data path
            // Transitioning to this, so don't return the programFiles path at all.
            var localDataPath = Path.Combine(LocalDataPath, fileSubPath);
            if (File.Exists(localDataPath))
            {
                return localDataPath;
            }

            // Ensure that the needed directory tree exists
            var parent = Path.GetDirectoryName(localDataPath);
            if (parent != null && !Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            return localDataPath;
        }

        /// <summary>
        /// Get the full path of the provided file subPath that exists
        /// </summary>
        /// <param name="fileSubPath"></param>
        /// <returns></returns>
        public static string GetFileLoadPath(string fileSubPath)
        {
            // Prefer the local data path
            var localDataPath = Path.Combine(LocalDataPath, fileSubPath);
            if (File.Exists(localDataPath))
            {
                return localDataPath;
            }

            // fallback 1: program data path (used for a time)
            var programDataPath = Path.Combine(ProgramDataPath, fileSubPath);
            if (Directory.Exists(programDataPath))
            {
                return programDataPath;
            }

            // fallback 2: program files path (may cause exceptions if not running as administrator)
            var programFilesPath = Path.Combine(ProgramExeDirectory, fileSubPath);
            if (File.Exists(programFilesPath))
            {
                return programFilesPath;
            }

            // fallback 3: working directory (usually ends up being the same as programFilesPath)
            if (File.Exists(fileSubPath))
            {
                return fileSubPath;
            }

            // final: nothing meets the criteria, return the preferred local data path
            return localDataPath;
        }

        /// <summary>
        /// Get the path where the directory should be located
        /// </summary>
        /// <param name="directorySubPath"></param>
        /// <returns></returns>
        public static string GetDirectorySavePath(string directorySubPath)
        {
            if (Path.IsPathRooted(directorySubPath))
            {
                return directorySubPath;
            }

            // Prefer the local data path
            // Transitioning to this, so don't return the programFiles path at all.
            var localDataPath = Path.Combine(LocalDataPath, directorySubPath);
            if (Directory.Exists(localDataPath))
            {
                return localDataPath;
            }

            Directory.CreateDirectory(localDataPath);

            // Ensure that the needed directory tree exists
            var oldDataDir = GetDirectoryLoadPathCheckContents(directorySubPath);

            if (Directory.Exists(oldDataDir))
            {
                // copy files over, since we don't want to read both directories...
                CopyDirectory(oldDataDir, localDataPath);
            }

            return localDataPath;
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.EnumerateFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var targetFilePath = Path.Combine(targetDir, fileName);
                if (!File.Exists(targetFilePath))
                {
                    File.Copy(file, targetFilePath);
                }
            }

            foreach (var dir in Directory.EnumerateDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(targetDir);
                var targetDirPath = Path.Combine(targetDir, dirName);
                CopyDirectory(dir, targetDirPath);
            }
        }

        /// <summary>
        /// Get the full path of the provided directory subPath that exists
        /// </summary>
        /// <param name="directorySubPath"></param>
        /// <returns></returns>
        public static string GetDirectoryLoadPath(string directorySubPath)
        {
            // Prefer the local data path
            var localDataPath = Path.Combine(LocalDataPath, directorySubPath);
            if (Directory.Exists(localDataPath))
            {
                return localDataPath;
            }

            // fallback 1: program data path (used for a time)
            var programDataPath = Path.Combine(ProgramDataPath, directorySubPath);
            if (Directory.Exists(programDataPath))
            {
                return programDataPath;
            }

            // fallback 2: program files path (may cause exceptions if not running as administrator)
            var programFilesPath = Path.Combine(ProgramExeDirectory, directorySubPath);
            if (Directory.Exists(programFilesPath))
            {
                return programFilesPath;
            }

            // fallback 3: working directory (usually ends up being the same as programFilesPath)
            if (Directory.Exists(directorySubPath))
            {
                return directorySubPath;
            }

            // final: nothing meets the criteria, return the preferred local data path
            return localDataPath;
        }

        /// <summary>
        /// Get the full path of the provided directory subPath that exists
        /// </summary>
        /// <param name="directorySubPath"></param>
        /// <returns></returns>
        public static string GetDirectoryLoadPathCheckContents(string directorySubPath)
        {
            // Prefer the local data path
            var localDataPath = Path.Combine(LocalDataPath, directorySubPath);
            if (Directory.Exists(localDataPath) && Directory.EnumerateFileSystemEntries(localDataPath).Any())
            {
                return localDataPath;
            }

            // fallback 1: program data path (used for a time)
            var programDataPath = Path.Combine(ProgramDataPath, directorySubPath);
            if (Directory.Exists(programDataPath) && Directory.EnumerateFileSystemEntries(programDataPath).Any())
            {
                return programDataPath;
            }

            // fallback 2: program files path (may cause exceptions if not running as administrator)
            var programFilesPath = Path.Combine(ProgramExeDirectory, directorySubPath);
            if (Directory.Exists(programFilesPath) && Directory.EnumerateFileSystemEntries(programFilesPath).Any())
            {
                return programFilesPath;
            }

            // fallback 3: working directory (usually ends up being the same as programFilesPath)
            if (Directory.Exists(directorySubPath) && Directory.EnumerateFileSystemEntries(directorySubPath).Any())
            {
                return directorySubPath;
            }

            // final: nothing meets the criteria, return the preferred local data path
            return localDataPath;
        }

        /// <summary>
        /// Get the full path of the provided directory subPath that exists
        /// </summary>
        /// <param name="directorySubPath"></param>
        /// <param name="fileFilter">file filter string</param>
        /// <returns></returns>
        public static string GetDirectoryLoadPathCheckFiles(string directorySubPath, string fileFilter = "*")
        {
            // Prefer the local data path
            var localDataPath = Path.Combine(LocalDataPath, directorySubPath);
            if (Directory.Exists(localDataPath) && Directory.EnumerateFiles(localDataPath, fileFilter).Any())
            {
                return localDataPath;
            }

            // fallback 1: program data path (used for a time)
            var programDataPath = Path.Combine(ProgramDataPath, directorySubPath);
            if (Directory.Exists(programDataPath) && Directory.EnumerateFiles(programDataPath, fileFilter).Any())
            {
                return programDataPath;
            }

            // fallback 2: program files path (may cause exceptions if not running as administrator)
            var programFilesPath = Path.Combine(ProgramExeDirectory, directorySubPath);
            if (Directory.Exists(programFilesPath) && Directory.EnumerateFiles(programFilesPath, fileFilter).Any())
            {
                return programFilesPath;
            }

            // fallback 3: working directory (usually ends up being the same as programFilesPath)
            if (Directory.Exists(directorySubPath) && Directory.EnumerateFiles(directorySubPath, fileFilter).Any())
            {
                return directorySubPath;
            }

            // final: nothing meets the criteria, return the preferred local data path
            return localDataPath;
        }
    }
}
