using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LcmsNetSDK;
using LcmsNetSDK.Logging;

namespace XcaliburControl
{
    public class InstrumentMethodFiles : INotifyPropertyChangedExt, IDisposable
    {
        public InstrumentMethodFiles(string methodsPath = DefaultMethodsPath)
        {
            availableMethods = new Dictionary<string, string>();

            XcaliburMethodsDirectoryPath = methodsPath;
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PATH: " + path);
            methodFileWatcher = new FileSystemWatcher(XcaliburMethodsDirectoryPath, "*.meth");
            methodFileWatcher.Created += MethodWatcherFileCreated;
            methodFileWatcher.Changed += MethodWatcherFileChanged;
            methodFileWatcher.Deleted += MethodWatcherFileDeleted;
            methodFileWatcher.IncludeSubdirectories = false;
            // Only report filename changes; we don't read the methods prior to the run.
            methodFileWatcher.NotifyFilter = NotifyFilters.FileName;
            methodFileWatcher.EnableRaisingEvents = true;

            ReadMethodDirectory();
        }

        public void Dispose()
        {
            methodFileWatcher.Dispose();
        }

        public const string DefaultMethodsPath = @"C:\Xcalibur\methods";
        private static readonly Regex MethodNameExclusionRegex = new Regex("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly Dictionary<string, string> availableMethods;
        private readonly FileSystemWatcher methodFileWatcher;
        private string xcaliburMethodsDirectoryPath = DefaultMethodsPath;

        public string XcaliburMethodsDirectoryPath
        {
            get => xcaliburMethodsDirectoryPath;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref xcaliburMethodsDirectoryPath, value))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        methodFileWatcher.EnableRaisingEvents = false;
                        ClearMethods();
                    }
                    else
                    {
                        // Path changed, replace the methodFileWatcher
                        methodFileWatcher.Path = value;
                        methodFileWatcher.EnableRaisingEvents = false;

                        ReadMethodDirectory();
                    }
                }
            }
        }

        /// <summary>
        /// Keys are method names,
        /// </summary>
        public IReadOnlyDictionary<string, string> MethodFiles => availableMethods;

        public event EventHandler MethodsUpdated;

        private void MethodWatcherFileChanged(object sender, FileSystemEventArgs e)
        {
            // Filter out method names with GUIDs (these are temporary sample-run-time methods)
            var name = Path.GetFileNameWithoutExtension(e.FullPath);
            if (MethodNameExclusionRegex.IsMatch(name))
            {
                return;
            }

            var methodLoaded = false;
            do
            {
                try
                {
                    // TODO: use e.ChangeType to conditionalize behavior
                    AddMethod(name, e.FullPath);
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + $" {e.ChangeType.ToString().ToLowerInvariant()}.");
                    methodLoaded = true;
                }
                catch (IOException)
                {
                    //probably caught the file being opened for writing.
                }
            } while (!methodLoaded);
        }

        private void MethodWatcherFileDeleted(object sender, FileSystemEventArgs e)
        {
            var methodLoaded = false;
            do
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(e.FullPath);
                    if (string.IsNullOrWhiteSpace(name) || MethodNameExclusionRegex.IsMatch(name))
                    {
                        return;
                    }

                    RemoveMethod(name);
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " removed.");
                    methodLoaded = true;
                }
                catch (IOException)
                {
                    //probably caught the file being opened for writing.
                }
            } while (!methodLoaded);
        }

        private void MethodWatcherFileCreated(object sender, FileSystemEventArgs e)
        {
            //AddMethod(Path.GetFileNameWithoutExtension(e.FullPath), File.ReadAllText(e.FullPath));
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " created.");
        }

        /// <summary>
        /// Reads the pump method directory and alerts the pumps of new methods to run.
        /// </summary>
        public void ReadMethodDirectory()
        {
            try
            {
                var path = XcaliburMethodsDirectoryPath;
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The directory " + path + " does not exist.");
                }

                var filenames = Directory.GetFiles(path, "*.meth");

                var methods = new Dictionary<string, string>();
                foreach (var filename in filenames)
                {
                    // Filter out method names with GUIDs (these are temporary sample-run-time methods)
                    var name = Path.GetFileNameWithoutExtension(filename);
                    if (MethodNameExclusionRegex.IsMatch(name))
                    {
                        continue;
                    }

                    methods[name] = filename;
                }

                // Clear any existing pump methods
                if (methods.Count > 0)
                {
                    ClearMethods();
                    AddMethods(methods);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        /// <summary>
        /// Clears all of the listed pump methods.
        /// </summary>
        public void ClearMethods()
        {
            availableMethods.Clear();
        }

        /// <summary>
        /// Removes a given method.
        /// </summary>
        /// <param name="methodName">Name of method to remove</param>
        public void RemoveMethod(string methodName)
        {
            if (availableMethods.ContainsKey(methodName))
            {
                availableMethods.Remove(methodName);

                // Fire MethodsUpdated since we removed one.
                MethodsUpdated?.Invoke(this, new EventArgs());
            }
        }

        public void AddMethods(Dictionary<string, string> methods)
        {
            foreach (var m in methods)
            {
                availableMethods.Add(m.Key, m.Value);
            }

            MethodsUpdated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Adds a given method.
        /// </summary>
        /// <param name="methodName">Name of method to track.</param>
        /// <param name="methodPath">Method data to store.</param>
        public void AddMethod(string methodName, string methodPath)
        {
            if (availableMethods.ContainsKey(methodName))
            {
                availableMethods[methodName] = methodPath;

                // Don't need to fire ListMethods() - the name didn't change.
            }
            else
            {
                availableMethods.Add(methodName, methodPath);

                MethodsUpdated?.Invoke(this, new EventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
