using System;
using LcmsNetDataClasses.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices.Pumps;
using Waters.ACQUITY;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Waters.Devices.Pumps
{
    public class WatersEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="status"></param>
        /// <param name="ledState"></param>
        public WatersEventArgs(List<string> data, string status, string ledState)
        {
            Data        = data;
            Status      = status;
            LedState    = ledState;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public WatersEventArgs(List<string> data)
        {
            Data = data;
        }

        public List<string> Data
        {
            get;
            private set;
        }
        public string Status
        {
            get;
            private set;
        }
        public string LedState
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Class that handles communicating with the Waters Nano BSM pumps.
    /// </summary>
    
    public class WatersPump :  IDevice, IPump, IDisposable
    {

        private string[] m_notificationStrings;

        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;

        #region Status Constants
        /// <summary>
        /// Notification string for flow changes
        /// </summary>
        private const string CONST_FLOW_CHANGE = "Flow % below set point";
        /// <summary>
        /// Notification string for pressure values.
        /// </summary>
        private const string CONST_PRESSURE_VALUE = "Pressure Value";
        private const string CONST_ERROR_ABOVE_PRESSURE = "Pressure Above Limit";
        private const string CONST_ERROR_BELOW_PRESSURE = "Pressure Below Limit";
        private const string CONST_ERROR_FLOW_EXCEEDS = "Flow Exceeds limit while pressure control";
        private const string CONST_ERROR_FLOW_UNSTABLE = "Column flow is unstable";
        #endregion

        /// <summary>
        /// Fired when the status of the instrument changes.
        /// </summary>
        public event EventHandler<WatersEventArgs> PumpStatus;
        private const int CONST_MONITORING_MINUTES         = 10;
        private const int CONST_MONITORING_SECONDS_ELAPSED = 1000;
        public event EventHandler<WatersEventArgs> InstrumentsFound;
        public event EventHandler<WatersEventArgs> MethodsFound;
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;
        private Dictionary<string, string> m_methodNameMap;
        Waters.ACQUITY.InstrumentSystemClient m_instrumentSystem = new Waters.ACQUITY.InstrumentSystemClient();

        /// <summary>
        /// Thread for monitoring the instrument settings.
        /// </summary>
        private Thread m_monitoringThread;
        /// <summary>
        /// Object for synchronizing the threads for monitoring and control.
        /// </summary>
        private object   m_syncObject;
        /// <summary>
        /// This is a flag that is used to synchronize between what should be monitored and what should not be.
        /// </summary>
        private volatile bool m_shouldMonitor;
        /// <summary>
        /// Handles synchronizing between two threads for shutdown (fast).
        /// </summary>
        private ManualResetEvent m_syncEvent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WatersPump()
        {            
            m_syncObject            = new object();
            m_syncEvent             = new ManualResetEvent(false);
            StopPumpOnShutdown      = false;
            m_shouldMonitor         = false;
            Name                    = "Waters Pump";
            MachineName             = "192.168.0.1";
            SystemName              = Environment.MachineName;
            MethodsFolderPath       = @"C:\Program Files\Waters Instruments\Methods";
            m_instrumentSystem      = new Waters.ACQUITY.InstrumentSystemClient();
            Instruments             = new List<string>();
            Methods                 = new List<string>();
            m_methodNameMap         = new Dictionary<string, string>();
            Status                  = enumDeviceStatus.NotInitialized;
            m_times                 = new List<DateTime>();
            m_pressures             = new List<double>();
            m_flows                 = new List<double>();
            m_compositions          = new List<double>();
            m_flowRate              = 0;
            
            TotalMonitoringMinutesDataToKeep = CONST_MONITORING_MINUTES;
            TotalMonitoringSecondElapsed     = CONST_MONITORING_SECONDS_ELAPSED;

            m_notificationStrings = new string[] 
            {
                CONST_FLOW_CHANGE,
                CONST_PRESSURE_VALUE
            };

            MobilePhases = new List<LcmsNetSDK.Data.MobilePhase>();

            StartMonitorThread();
        }
        ~WatersPump()
        {
            Dispose();
        }

        /// <summary>
        /// Method that runs on a separate thread for watching the status detail
        /// </summary>
        private void MonitorThread()
        {
            WaitHandle[] events = new WaitHandle[] { m_syncEvent };

            while (m_shouldMonitor)
            {
                int eventId = WaitHandle.WaitAny(events, TotalMonitoringSecondElapsed);
                // This is the user told me to stop event
                if (eventId == 0)
                {

                }
                else
                {
                    string statusXML = "";
                    // Monitor
                    lock (m_syncObject)
                    {
                        try
                        {                            
                            statusXML = m_instrumentSystem.StatusDetail;
                        }
                        catch (Exception) 
                        {

                        }
                    }
                    ParseAndProcessStatus(statusXML);

                }
            }
        }

        private List<DateTime> m_times;
        private List<double> m_pressures;
        private List<double> m_flows;
        private List<double> m_compositions;
        private double m_flowRate;

        private void ParseAndProcessStatus(string xmlData)
        {
            DateTime time = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            
            PumpStatusDetailParser parser = new PumpStatusDetailParser();
            PumpStatusInfo info = parser.ParseData(xmlData);
            double pressure     = info.Pressure;
            double flowrate     = info.FlowrateA + info.FlowrateB;
            double compositionB = info.CompositionB;

            // notifications
            if (m_flowRate != 0)
            {
                double percentFlowChange = (m_flowRate - flowrate) / m_flowRate;
                UpdateNotificationStatus(percentFlowChange.ToString(), CONST_FLOW_CHANGE);               
            }
            m_flowRate = flowrate;

            UpdateNotificationStatus(pressure.ToString(), CONST_PRESSURE_VALUE);

            // Update log collections.
            m_times.Add(time);
            m_pressures.Add(pressure);
            m_flows.Add(flowrate);
            m_compositions.Add(compositionB);

            /// 
            /// Find old data to remove -- needs to be updated (or could be) using LINQ
            /// 
            int count = m_times.Count;
            int total = (TotalMonitoringMinutesDataToKeep * 60) / TotalMonitoringSecondElapsed;
            if (count >= total)
            {
                int i = 0;
                while (time.Subtract(m_times[i]).TotalMinutes > TotalMonitoringMinutesDataToKeep && i < m_times.Count)
                {
                    i++;
                }

                if (i > 0)
                {
                    i = Math.Min(i, m_times.Count - 1);
                    m_times.RemoveRange(0, i);
                    m_flows.RemoveRange(0, i);
                    m_pressures.RemoveRange(0, i);
                    m_compositions.RemoveRange(0, i);
                }
            }

            // Alert the user data is ready
            try
            {
                if (MonitoringDataReceived != null)
                {
                    MonitoringDataReceived(this,
                            new PumpDataEventArgs(this,
                                                    m_times,
                                                    m_pressures,
                                                    m_flows,
                                                    m_compositions));
                }
            }
            catch
            {
            }

            try
            {
                if (PumpStatus != null)
                {
                    WatersEventArgs args = new WatersEventArgs(new List<string>(), info.Status, info.LedStatus);
                    PumpStatus(this, args);
                }
            }
            catch
            {
            }
        }

        private void UpdateNotificationStatus(string message, string type)
        {
            if (StatusUpdate != null)
            {
                classDeviceStatusEventArgs args = new classDeviceStatusEventArgs(Status, type, message, this);
                StatusUpdate(this, args);
            }
        }

        [classPersistenceAttribute("TotalMonitoringMinutes")]
        public int TotalMonitoringMinutesDataToKeep
        {
            get;
            set;
        }
        [classPersistenceAttribute("TotalMonitoringSecondsElapsed")]
        public int TotalMonitoringSecondElapsed
        {
            get;
            set;
        }

        /// <summary>
        /// Starts the monitoring thread.
        /// </summary>
        private void StartMonitorThread()
        {
            AbortMonitorThread();            
            
            m_shouldMonitor     = true;
            ThreadStart start   = new ThreadStart(MonitorThread);
            m_monitoringThread  = new Thread(start);

            m_monitoringThread.Start();
        }
        /// <summary>
        /// Kills the monitoring thread.
        /// </summary>
        private void AbortMonitorThread()
        {
            if (m_monitoringThread != null)
            {
                if (m_monitoringThread.IsAlive)
                {
                    try
                    {
                        m_shouldMonitor = false;
                        m_syncEvent.Set();
                        Thread.Sleep(200);
                        m_monitoringThread.Abort();
                        m_monitoringThread.Join(1000);
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                    finally
                    {
                        m_monitoringThread = null;
                    }
                }
            }
        }
                        
        #region Waters Members
        [classPersistence("MachineName")]
        public string MachineName
        {
            get;
            set;
        }
        [classPersistence("SystemName")]
        public string SystemName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the lists of available instruments.
        /// </summary>
        public List<string> Instruments
        {
            get;
            private set;
        }
        #endregion

        #region IDevice Members
        public string Name
        {
            get;
            set;
        }
        public string Version
        {
            get;
            set;
        }
        public enumDeviceStatus Status
        {
            get;
            set;
        }
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the error type.
        /// </summary>
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets what type of device it is.
        /// </summary>
        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Component; }
        }
        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        public bool Emulation
        {
            get;
            set;
        }

        public bool Initialize(ref string errorMessage)
        {
            Instruments              = GetInstrumentList();
            GetMethodList();

            Dictionary<string, string> instrumentCheck = new Dictionary<string,string>();
            Instruments.ForEach(x => instrumentCheck.Add(x, x));

            if (Instrument != null)
            {
                if (instrumentCheck.ContainsKey(Instrument))
                {
                    XDocument doc = new XDocument
                    (
                        new XElement("Options",
                            new XElement("RunTimeInMethod", "false"),
                            new XElement("HideAdditionalDataTabs", "false"),
                            new XElement("MachineName", MachineName),
                            new XElement("SystemName", SystemName),
                            new XElement("Instruments", Instrument)
                            )
                    );
                    string options = doc.ToString();


                    lock (m_syncObject)
                    {
                        m_instrumentSystem.Initialize(options);
                    }

                    Status = enumDeviceStatus.Initialized;
                    return true;
                }
                else
                {
                    Status = enumDeviceStatus.NotInitialized;
                    errorMessage = "The instrument name does not match any instruments on the pump network.  Did you change pumps?";
                    return false;
                }
            }
            else
            {
                Status       = enumDeviceStatus.NotInitialized;
                errorMessage = "The instrument has not been previously configured.";
                return false;
            }
            return false;
        }
        /// <summary>
        /// Gets the list of methods that are available to the system.
        /// </summary>
        public List<string> Methods
        {
            get;
            private set;
        }
        [classPersistence("Instrument")]
        public string Instrument
        {
            get;
            set;
        }

        [classPersistenceAttribute("MethodsFolderPath")]
        public string MethodsFolderPath { get; set; }
        /// <summary>
        /// Gets or sets whether the system should be shutdown on startup
        /// </summary>        
        [classPersistenceAttribute("StopPumpOnShutdown")]
        public bool StopPumpOnShutdown
        {
            get;
            set;
        }
        public bool Shutdown()
        {
            try
            {
                if (StopPumpOnShutdown)
                {
                    lock (m_syncObject)
                    {
                        m_instrumentSystem.Stop();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                Dispose();  
            }
            return true;
        }
        private void ListMethods()
        {
            if (MethodNames != null)
            {
                List<object> data = new List<object>();
                data.AddRange(Methods);

                MethodNames(this, (List<object>)data);
            }
        }
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames += remoteMethod;
                    ListMethods();
                    break;
            }
        }
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames -= remoteMethod;
                    break;
            }
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
            List<string> notifications = new List<string>() { "Status"
                                                            };

            notifications.AddRange(m_notificationStrings);            
            return notifications;
        }
        public List<string> GetErrorNotificationList()
        {
            return new List<string>(); 
        }        
        #endregion
        
        #region Utility Methods
        /// <summary>
        /// Gets a list of instruments from the local area network.
        /// </summary>
        /// <returns></returns>
        public List<string> GetInstrumentList()
        {
            string[] instruments = null;

            lock (m_syncObject)
            {
                instruments = m_instrumentSystem.GetInstruments(MachineName);
            }
            if (instruments != null)
            {
                if (InstrumentsFound != null)
                {
                    InstrumentsFound(this, new WatersEventArgs(instruments.ToList()));
                }
                Instruments = instruments.ToList();
            }
            return Instruments;
        }
        /// <summary>
        /// Retrieves the list of methods from the file folder.
        /// </summary>
        /// <returns></returns>
        public List<string> GetMethodList()
        {
            string[] methods = Directory.GetFiles(MethodsFolderPath, "*.method");
            
            m_methodNameMap.Clear();
            Methods.Clear();

            foreach (string method in methods)
            {
                string methodName = Path.GetFileNameWithoutExtension(method);
                m_methodNameMap.Add(methodName, method);
                Methods.Add(methodName);
            }

            if (MethodsFound != null)
            {
                MethodsFound(this, new WatersEventArgs(Methods));
            }
            ListMethods();
            return methods.ToList();
        }
        #endregion

        /// <summary>
        /// Gets a list of instruments from the network.       
        /// </summary>
        /// <returns></returns>
        public List<string> ScanForInstruments()
        {
            WDHCPServerSvcLib.EthernetScanClass scan = new WDHCPServerSvcLib.EthernetScanClass();
            
            string instruments  = scan.GetInstrumentList();
            XmlDocument doc     = new XmlDocument();
            doc.LoadXml(instruments);
            string xpath        = @"//Instrument/ID";
            XmlNodeList nodes   = doc.SelectNodes(xpath);

            List<string> instrumentNames = new List<string>();
            foreach (XmlNode node in nodes)
            {
                instrumentNames.Add(node.InnerText);                
            }
            return instrumentNames;
        }

        /// <summary>
        /// Returns the name of the pump.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// Displays the Waters Console.
        /// </summary>
        public void ShowConsole()
        {
            lock (m_syncObject)
            {
                m_instrumentSystem.LaunchConsole();
            }
        }
        /// <summary>
        /// Starts the method provided for the given runtime.  
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="method"></param>
        /// <param name="runTime"></param>
        [classLCMethodAttribute("Start Method", enumMethodOperationTime.Parameter, "MethodNames", 1, true)]        
        public void StartMethod(double timeout, string method)
        {
            try
            {
                string methodPath = m_methodNameMap[method];
                string methodData = File.ReadAllText(methodPath);

                lock (m_syncObject)
                {
                    m_instrumentSystem.SetupMethod(methodData);
                }
                XDocument doc =
                    new XDocument(
                        new XElement("RunInformation",
                            new XElement("Injection",
                                new XElement("Location"),
                                new XElement("SampleName"),
                                new XElement("Volume"),
                                new XElement("SampleType", "Condition Column"),
                                new XElement("Injections"),
                                new XElement("Method", methodPath),
                                new XElement("RunTime", timeout.ToString()),
                                new XElement("RowNumber", "1")
                            )
                        )
                    );

                string injectionParameters          = doc.ToString();
                Dictionary<string, string> methods  = new Dictionary<string, string>();
                methods.Add(methodPath, methodData);

                lock (m_syncObject)
                {
                    Waters.ACQUITY.IInternalInstrumentSystem privateInterface = m_instrumentSystem as Waters.ACQUITY.IInternalInstrumentSystem;
                    if (privateInterface != null)
                    {
                        try
                        {
                            privateInterface.RunSampleSet(methods, injectionParameters);
                        }
                        catch (Exception ex)
                        {
                            //TODO: Log the exception!
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not start Waters pump method.", ex);
            }
        }       

        #region IPump Members
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived;
        #endregion

        public void StopFlow()
        {
            // Stops the flow to the pump
        }

        public void StopMethod()
        {
            try
            {
                lock (m_syncObject)
                {
                    m_instrumentSystem.Stop();
                }
            }
            catch
            {
                // Stops the method
            }
        }

        #region IFinchComponent Members

        /*public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }*/

        #endregion

        /// <summary>
        /// Cleans up any resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                AbortMonitorThread();
            }
            catch
            {
            }
        }

        #region IPump Members


        public List<LcmsNetSDK.Data.MobilePhase> MobilePhases
        {
            get;
            set;
        }

        #endregion
    }    
}
