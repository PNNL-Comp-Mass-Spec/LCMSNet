using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using System.IO;
using System.Threading;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Data;

namespace ExternalFileWatcher.Watchers
{


   /* [classDeviceControlAttribute(typeof(controlExternalFileWatcher),
                                    "Data file watcher",
                                    "External")
    ]*/
    public class ExternalFileWatcher: IDevice
    {
        /// <summary>
        /// The directory is being searched.
        /// </summary>
        private const int FIND_DIRECTORY_MODE    = 0;
        /// <summary>
        /// The file is being searched for.
        /// </summary>
        private const int FIND_FILE_MODE         = 1;
        /// <summary>
        /// The file is being monitored
        /// </summary>
        private const int MONITOR_MSFILE_MODE    = 2;
        private const string FOUND_FILE          = "Found File";
        private const string FOUND_FILE_COMPLETE = "File Complete";
        private string mstring_name;
        public event EventHandler<FileWriteArgs> FileFound;
        public event EventHandler<FileWriteArgs> FileComplete;
        private enumDeviceStatus m_status;

        public ExternalFileWatcher()
        {
            m_status            = enumDeviceStatus.NotInitialized;            
            AbortEvent          = new ManualResetEvent(false);
            Name                = "Data file watcher";
            DirectoryExtension  = ".d";
            FileExtension       = ".ms";
        }

        /// <summary>
        /// Gets or sets how long to wait before we say the file is not being written to.
        /// </summary>
        [classPersistence("SecondsToWait")]
        public double SecondsToWait
        {
            get;
            set;
        }
        /// <summary>
        /// Path to watch for data in.
        /// </summary>        
        [classPersistence("WatchPath")]
        public string WatchPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the file extension to watch for.
        /// </summary>        
        [classPersistence("FileExtension")]
        public string FileExtension
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the directory extension if the external software appends to a directory name.
        /// </summary>
        [classPersistence("DirectoryExtension")]
        public string DirectoryExtension
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
      //  [classLCMethodAttribute("Trigger when sample file created", enumMethodOperationTime.Indeterminate, true, 0, "", -1, false)]
        public bool WatchForFileCreated(classSampleData sample)
        {
            bool successful      = false;
            bool shouldWatch     = true;
            WaitHandle[] handles = new WaitHandle[] { AbortEvent };

            string path = sample.DmsData.DatasetName;
            path = Path.Combine(WatchPath, path + FileExtension);

            while (shouldWatch)
            {
                int dwEvent = WaitHandle.WaitAny(handles, 100);

                // Told to stop.
                if (dwEvent == 0)
                {
                    shouldWatch = false;
                }

                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    successful    = true;
                    classTriggerFileTools.GenerateTriggerFile(sample);
                    shouldWatch   = false;

                    if (FileComplete != null)
                    {
                        FileComplete(this, new FileWriteArgs(path, 
                            info.LastWriteTime, 
                            info.LastWriteTime.AddSeconds(SecondsToWait)));
                        shouldWatch = false;
                    }
                }
            }
            return successful;
        }
        /// <summary>
        /// Watches for a file and creates a trigger file when completed.
        /// </summary>
        /// <param name="sample"></param>
      //  [classLCMethodAttribute("Trigger when done collecting", enumMethodOperationTime.Indeterminate, true, 0, "", -1, false)]
        public bool WatchForFile(classSampleData sample)
        {
            bool successful      = false;
            bool shouldWatch     = true;
            WaitHandle[] handles = new WaitHandle[] { AbortEvent };

            string path = sample.DmsData.DatasetName;
            path = Path.Combine(WatchPath, path + FileExtension);

            while (shouldWatch)
            {
                int dwEvent = WaitHandle.WaitAny(handles, 500);

                // Told to stop.
                if (dwEvent == 0)
                {
                    shouldWatch = false;
                }

                if (File.Exists(path))
                {
                    FileInfo info       = new FileInfo(path);
                    DateTime lastWrite  = info.LastWriteTime;

                    if (FileFound != null)
                    {
                        FileFound(this, new FileWriteArgs(path, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                    }

                    double totalTime = Math.Abs(DateTime.Now.Subtract(lastWrite).TotalSeconds);

                    if (SecondsToWait < Convert.ToInt32(totalTime))
                    {
                        LcmsNetDataClasses.Data.classTriggerFileTools.GenerateTriggerFile(sample);
                        successful = true;
                        if (FileComplete != null)
                        {
                            FileComplete(this, new FileWriteArgs(path, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                            shouldWatch = false;
                        }
                    }
                }
            }
            return successful;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="minutes"></param>
        [classLCMethodAttribute("Watch for File N seconds", enumMethodOperationTime.Parameter, true, 1, "", -1, false)]
        public bool WatchDirectoryForNMins(double timeout, classSampleData sample)
        {
            bool shouldWatch = true;
            WaitHandle[] handles = new WaitHandle[] { AbortEvent };

            string path = sample.DmsData.DatasetName;
            path        = Path.Combine(WatchPath, path + FileExtension);

            DateTime startTime  = DateTime.Now;
            bool found          = false; 
            while (shouldWatch)
            {
                int dwEvent = WaitHandle.WaitAny(handles, 100);

                // Told to stop.
                if (dwEvent == 0)
                {
                    shouldWatch = false;
                }
                else
                {
                    double total = DateTime.Now.Subtract(startTime).TotalSeconds;
                    if (total >= timeout - .5)
                    {                        
                        break;
                    }
                }
                
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    DateTime lastWrite = info.LastWriteTime;


                    if (FileFound != null)
                    {
                        FileFound(this, new FileWriteArgs(path, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                    }

                    double totalTime = Math.Abs(DateTime.Now.Subtract(lastWrite).TotalSeconds);

                    if (SecondsToWait <= Convert.ToInt32(totalTime))
                    {
                        LcmsNetDataClasses.Data.classTriggerFileTools.GenerateTriggerFile(sample);
                        if (FileComplete != null)
                        {
                            FileComplete(this, new FileWriteArgs(path, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                            shouldWatch = false;
                            found = true;
                        }
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="minutes"></param>
        [classLCMethodAttribute("Watch for Directory N seconds", enumMethodOperationTime.Parameter, true, 1, "", -1, false)]
        public bool WatchDirectoryFileForNMins(double timeout, classSampleData sample)
        {
            bool shouldWatch = true;
            WaitHandle[] handles = new WaitHandle[] { AbortEvent };

            string searchPath   = sample.DmsData.DatasetName;
            searchPath          = Path.Combine(WatchPath, searchPath);
            searchPath          = searchPath + DirectoryExtension;
            DateTime startTime  = DateTime.Now;
            bool found          = false;
            int flag            = FIND_DIRECTORY_MODE;

            string filePath     = "";  
            while (shouldWatch)
            {
                int dwEvent = WaitHandle.WaitAny(handles, 100);

                // Told to stop.
                if (dwEvent == 0)
                {
                    shouldWatch = false;
                    break;
                }
                else
                {
                    double total = DateTime.Now.Subtract(startTime).TotalSeconds;
                    if (total >= timeout - .5)
                    {
                        break;
                    }

                    switch(flag)
                    {
                        default:
                            break;
                        case FIND_DIRECTORY_MODE:
                            // Here we check for the directory to see if it was created.
                            if (Directory.Exists(searchPath))
                            {                                
                                flag        = FIND_FILE_MODE;
                            }
                            break;
                        case FIND_FILE_MODE:
                            /// Here we look for the latest file to be written and assume that's the guy we want.
                            string [] files = Directory.GetFiles(searchPath, "*" + FileExtension, SearchOption.TopDirectoryOnly);                            
                            FileInfo info   = null;
                            foreach (string file in files)
                            {
                                FileInfo tempInfo = new FileInfo(file);
                                if (tempInfo != null)
                                {
                                    if (info != null)
                                    {
                                        if (info.LastWriteTime.CompareTo(tempInfo.LastWriteTime) < 0)
                                        {
                                            info = tempInfo;
                                        }
                                    }
                                    else
                                    {
                                        info = tempInfo;
                                    }
                                }
                            }                            
                            if (info != null)
                            {
                                // We found something so move the path and tell the state machine to transfer to monitor the file size.
                                filePath    = info.FullName;
                                flag        = MONITOR_MSFILE_MODE;
                            }
                            break;
                        case MONITOR_MSFILE_MODE:
                            if (File.Exists(filePath))
                            {
                                
                                info = new FileInfo(filePath);
                                DateTime lastWrite = info.LastWriteTime;

                                if (FileFound != null)
                                {
                                    FileFound(this, new FileWriteArgs(filePath, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                                }

                                double totalTime = Math.Abs(DateTime.Now.Subtract(lastWrite).TotalSeconds);

                                if (SecondsToWait <= Convert.ToInt32(totalTime))
                                {
                                    LcmsNetDataClasses.Data.classTriggerFileTools.GenerateTriggerFile(sample);
                                    if (FileComplete != null)
                                    {
                                        FileComplete(this, new FileWriteArgs(filePath, lastWrite, lastWrite.AddSeconds(SecondsToWait)));
                                        shouldWatch = false;
                                        found = true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return found;
        }

        #region IDevice Members
        public string Name
        {
            get
            {
                return mstring_name;
            }
            set
            {
                mstring_name = value;
                if (DeviceSaveRequired != null)
                {
                    DeviceSaveRequired(this, null);
                }
            }
        }
        public string Version
        {
            get;
            set;
        }
        public enumDeviceStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
                if (StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(m_status, "Status", this));
                }
            }
        }
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        public bool Initialize(ref string errorMessage)
        {
            return true;
        }
        public bool Shutdown()
        {         
            return true;
        }
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            
        }
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            
        }
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
         
        }
        public classMonitoringComponent GetHealthData()
        {
            return null;    
        }

        public List<string> GetStatusNotificationList()
        {
            List<string> notifications = new List<string>() { "Status",
                                                            FOUND_FILE,
                                                            FOUND_FILE_COMPLETE
                                                            };

            return notifications;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() ;
        }

        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }
        public bool Emulation
        {
            get;
            set;
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region IFinchComponent Members

        public Finch.Data.FinchComponentData GetData()
        {
            return new Finch.Data.FinchComponentData();
        }

        #endregion
    }
}
