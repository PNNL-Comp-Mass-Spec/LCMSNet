﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.PALAutoSampler.Pal
{
    /// <summary>
    /// Interface to the LEAP PAL robotic sampler.
    /// </summary>
    [Serializable]

    [DeviceControl(typeof(PalViewModel),
                                 "PAL Autosampler",
                                 "Auto-Samplers")]
    public class Pal : IDevice, IAutoSampler, IFluidicsSampler
    {
        #region Members

        /// <summary>
        /// An object which can use the PAL dll functions.
        /// </summary>
        private P.PalClass m_PALDrvr;

        /// <summary>
        /// The location of the directory containing the PAL methods.
        /// </summary>
        private String m_methodsFolder;

        /// <summary>
        /// A flag indicating whether or not the PAL has been initialized.
        /// </summary>
        private bool m_accessible;

        /// <summary>
        /// The current method to execute.
        /// </summary>
        private string m_method = "";

        /// <summary>
        /// The current tray to use.
        /// </summary>
        private string m_tray = "";

        /// <summary>
        /// The current vial to use.
        /// </summary>
        private int m_vial;

        /// <summary>
        /// The valid range of vials.
        /// </summary>
        private VialRanges m_vialRange;// = enumVialRanges._96Well;

        /// <summary>
        /// The current volume setting.
        /// </summary>
        private string m_volume = "";

        /// <summary>
        /// Name for fluidics designer.
        /// </summary>
        private string m_name;

        /// <summary>
        /// The PAL's version information.
        /// </summary>
        private string m_version;

        /// <summary>
        /// The current status of the PAL.
        /// </summary>
        private DeviceStatus m_status;

        /// <summary>
        /// Indicates whether or not the PAL is in emulation mode.
        /// </summary>
        private bool m_emulation;

        private readonly Dictionary<string, int> trayNamesAndMaxVials = new Dictionary<string, int>();
        private readonly List<string> trayNames = new List<string>();
        private readonly List<string> methodNames = new List<string>();

        private const int CONST_PALERR_PORTUNAVAILABLE = 2;
        private const int CONST_PALERR_PORTINUSE = 3;
        private const int CONST_WAITTIMEOUT = 10000; //milliseconds

        #endregion

        #region Events

        /// <summary>
        /// Indicates that a change requiring saving in the Fluidics designer has occurred.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Indicates that the device is not busy and can accept commands.
        /// </summary>
        public event Action Free;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Fired when new tray names are available.
        /// </summary>
        public event EventHandler<AutoSampleEventArgs> TrayNamesRead;

        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        public event EventHandler<AutoSampleEventArgs> MethodNamesRead;

        /// <summary>
        /// Fired to the method editor handler with a List of method names
        /// </summary>
        public event DelegateDeviceHasData Methods;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Pal()
        {
            m_vialRange = VialRanges.Well96;
            m_name = "pal";
            AbortEvent = new System.Threading.ManualResetEvent(false);
            StatusPollDelay = 1;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The list of trays addressable by the Pal. Populated by <see cref="ListTrays()"/>
        /// </summary>
        public List<string> TrayNames => trayNames;

        /// <summary>
        /// The list of methods in the Pal. Populated by <see cref="ListMethods()"/>
        /// </summary>
        public List<string> MethodNames => methodNames;

        /// <summary>
        /// A mapping of trays and max vials for each tray. Populated by <see cref="SetMaxVialsForTrays"/>
        /// </summary>
        public Dictionary<string, int> TrayNamesAndMaxVials => trayNamesAndMaxVials;

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the device is emulation mode.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation
        {
            get => m_emulation;
            set => m_emulation = value;
        }

        /// <summary>
        /// The current status of the PAL.
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status", this));
                m_status = value;
            }
        }

        /// <summary>
        /// The PAL's version information.
        /// </summary>
        public string Version
        {
            get => m_version;
            set
            {
                m_version = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets a uniquely identifiable name.
        /// </summary>
        public string Name
        {
            get => m_name;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Gets or sets the folder containing the PAL method files.
        /// </summary>
        [PersistenceData("MethodsFolder")]
        public string MethodsFolder
        {
            get => m_methodsFolder;
            set
            {
                m_methodsFolder = value;
                if (m_emulation == false)
                {
                    if (value == null)
                    {
                        throw new NullReferenceException("The methods folder for the PAL provided by the user was null.");
                    }
                    if (!System.IO.Directory.Exists(value))
                    {
                        throw new System.IO.DirectoryNotFoundException("The directory does not exist: " + value + "");
                    }
                    m_PALDrvr.SelectMethodFolder(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the method for the PAL to run.
        /// </summary>
        [PersistenceData("Method")]
        public string Method
        {
            get => m_method;
            set
            {
                m_method = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the tray for the PAL to use.
        /// </summary>
        [PersistenceData("Tray")]
        public string Tray
        {
            get => m_tray;
            set
            {
                m_tray = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// The range of valid vial numbers
        /// </summary>
        public VialRanges VialRange
        {
            get => m_vialRange;
            set => this.RaiseAndSetIfChanged(ref m_vialRange, value);
        }

        /// <summary>
        /// The maximum valid vial number
        /// </summary>
        [PersistenceData("VialRange")]
        public int MaxVial
        {
            get => (int) VialRange;
            set
            {
                var newVal = Enum.GetValues(typeof(VialRanges)).Cast<int>().Where(x => x >= value).DefaultIfEmpty((int) VialRanges.Well1536).Min();
                if (Enum.TryParse(newVal.ToString(), out VialRanges range))
                {
                    VialRange = range;
                }
                else
                {
                    VialRange = VialRanges.Well96;
                }
            }
        }

        /// <summary>
        /// Gets or sets the vial for the PAL to use.
        /// </summary>
        [PersistenceData("Vial")]
        public int Vial
        {
            get => m_vial;
            set
            {
                if (ValidateVial(value))
                {
                    m_vial = value;
                }
                else
                {
                    HandleError("Vial number out of range");
                }
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the volume (in uL).
        /// </summary>
        [PersistenceData("Volume")]
        public string Volume
        {
            get => m_volume;
            set
            {
                m_volume = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the serial port which the PAL is connected to.
        /// </summary>
        [PersistenceData("Port")]
        public string PortName { get; set; }

        /// <summary>
        /// Gets or sets the delay when polling for system status in seconds
        /// </summary>
        [PersistenceData("StatusPollDelay")]
        public int StatusPollDelay { get; set; }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Component;

        #endregion

        #region Methods

        /// <summary>
        /// Indicates that the device is not busy and can accept commands.
        /// </summary>
        public virtual void OnFree()
        {
            Free?.Invoke();
        }

        /// <summary>
        /// Indicates that a change requiring saving in the Fluidics designer has occurred.
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        /// <summary>
        /// Internal error handler that propagates the error message to listening objects.
        /// </summary>
        private void HandleError(string message)
        {
            HandleError(message, null);
        }

        /// <summary>
        /// Validates a particular vial number
        /// </summary>
        /// <param name="vial">The vial number to check</param>
        /// <returns></returns>
        private bool ValidateVial(int vial)
        {
            return (vial >= 1 && vial <= (int)m_vialRange);
        }

        /// <summary>
        /// Internal error handler that propagates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, Exception ex)
        {
            Error?.Invoke(this, new DeviceErrorEventArgs(message, ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, message));
        }

        /// <summary>
        /// Initializes the PAL.
        /// This is done by starting paldriv.exe, resetting the PAL,
        /// and loading the PAL's configuration.
        /// </summary>
        public bool Initialize(ref string errorMessage)
        {
            if (m_emulation)
            {
                m_accessible = true;
                if (!AutoSamplers.ConnectedAutoSamplers.Contains(this))
                {
                    AutoSamplers.ConnectedAutoSamplers.Add(this);
                }
                ListMethods();
                ListTrays();
                return true;
            }

            if (m_accessible == false)
            {
                if (m_PALDrvr == null)
                {
                    m_PALDrvr = new P.PalClass();
                }

                if (PortName == null)
                {
                    errorMessage = "COM Port not set.  Please select a COM port name.";
                    return false;
                }

                //Start paldriv.exe
                var error = m_PALDrvr.StartDriver("1", PortName);
                switch (error)
                {
                    case CONST_PALERR_PORTUNAVAILABLE:
                        HandleError("COM Port not available");
                        errorMessage = "COM Port not available";
                        OnFree();
                        break;
                    case CONST_PALERR_PORTINUSE:
                        HandleError("COM Port in use");
                        errorMessage = "COM Port in use";
                        OnFree();
                        break;
                }

                if (error > 0)
                {
                    var tempStatus = "";
                    m_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }

                var status = WaitUntilReady(CONST_WAITTIMEOUT);
                // Bryson: Removed, because it doesn't appear to be used at all?
                // If re-enabled, should change to use "PersistDataPaths" to not write to the ProgramFiles directory
                //if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal"))
                //{
                //    System.IO.File.Delete(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                //}
                //System.IO.TextWriter writer = System.IO.File.CreateText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                //writer.Close();

                //Reset the pal
                error = m_PALDrvr.ResetPAL();
                if (error > 0)
                {
                    var tempStatus = "";
                    m_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }

                status = WaitUntilReady(CONST_WAITTIMEOUT);

                //Load configuration
                error = m_PALDrvr.LoadConfiguration();

                if (error > 0)
                {
                    var tempStatus = "";
                    m_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }

                status = WaitUntilReady(CONST_WAITTIMEOUT);
                if (status != 0)
                {
                    status = WaitUntilReady(CONST_WAITTIMEOUT);
                }

                var methodsFolder = LCMSSettings.GetParameter(LCMSSettings.PARAM_PALMETHODSFOLDER);
                var exists = System.IO.Directory.Exists(methodsFolder);
                if (!exists)
                {
                    var newMethodsFolder = methodsFolder.Replace("Program Files (x86)", "Program Files");
                    ApplicationLogger.LogError(0,
                                        string.Format("Could not find the PAL Methods folder {0}.  Looking for folder path: {1}",
                                            methodsFolder,
                                            newMethodsFolder));
                    methodsFolder = newMethodsFolder;
                }

                MethodsFolder = methodsFolder;

                WaitUntilReady(CONST_WAITTIMEOUT);
                //If we made it this far, success! We can now access the PAL.
                m_accessible = true;
                // store the pal instance to make it available for, say, validators
                if (!AutoSamplers.ConnectedAutoSamplers.Contains(this))
                {
                    AutoSamplers.ConnectedAutoSamplers.Add(this);
                }

                // Set the methods folder the PAL uses according to the LCMSNet application settings
                m_PALDrvr.SelectMethodFolder(MethodsFolder);

                //list methods
                ListMethods();
                status = WaitUntilReady(CONST_WAITTIMEOUT);
                // list trays
                ListTrays();
                // set the max vial for each tray
                SetMaxVialsForTrays();
            }
            OnFree();
            return true;
        }

        /// <summary>
        /// Sets the folder where the PAL methods are found.
        /// </summary>
        /// <param name="newFolderPath">The path to the new folder.</param>
        public void SetMethodFolder(string newFolderPath)
        {
            if (m_emulation)
            {
                return;
            }

            var error = m_PALDrvr.SelectMethodFolder(newFolderPath);
            //Error Checking
            if (error > 0)
            {
                var tempStatus = "";
                m_PALDrvr.GetStatus(ref tempStatus);
                HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                OnFree();
                return;
            }
            OnDeviceSaveRequired();
            OnFree();
        }

        /// <summary>
        /// Disables access to the PAL. This does not physically shutdown the PAL.
        /// </summary>
        public bool Shutdown()
        {
            if (AutoSamplers.ConnectedAutoSamplers.Contains(this))
            {
                AutoSamplers.ConnectedAutoSamplers.Remove(this);
            }

            if (m_emulation)
            {
                m_accessible = false;
                return true;
            }

            if (m_accessible)
            {
                m_PALDrvr = null;
                m_accessible = false;
            }
            OnDeviceSaveRequired();
            return true;
        }

        /// <summary>
        /// Lists the available methods for use with the PAL. Also populates <see cref="MethodNames"/>
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public List<string> ListMethods()
        {
            var methods = "";
            //
            // Find the methods from the device (or emulated one)
            //
            if (m_emulation == false)
            {
                var error = m_PALDrvr.GetMethodNames(ref methods);
                //TODO: Handle error.
            }
            else
            {
                methods = "Example;Dance;Headstand;Self-Destruct";
            }
            //
            // Now to update the user interface, get the method names
            // and send them to any listeners.
            //
            var methodsList = new List<string>();
            if (methods != null)
            {
                var methodNames = methods.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PAL METHODS LENGTH: " + methodNames.Length);

                if (methodNames.Length > 0)
                {
                    methodsList.AddRange(methodNames);
                }

                // For UI and other.
                if (methodsList.Count > 0)
                {
                    MethodNamesRead?.Invoke(this, new AutoSampleEventArgs(new List<string>(), methodsList));
                }
                // For method editor.  Needs to be refactored to use event args like above.
                if (methodsList.Count > 0 && Methods != null)
                {
                    var objects = new List<object>();
                    foreach (var name in methodsList)
                    {
                        objects.Add(name);
                    }
                    Methods(this, objects);
                }
            }

            MethodNames.Clear();
            MethodNames.AddRange(methodsList);

            return methodsList;
        }

        /// <summary>
        /// Lists the available trays known to the PAL. Also populates <see cref="TrayNames"/>
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public List<string> ListTrays()
        {
            var trays = "";
            var tries = 0;
            var MAX_TRIES = 50;
            if (m_emulation == false)
            {
                var error = 0; //assume success
                while (string.IsNullOrEmpty(trays) && tries <= MAX_TRIES)
                {
                    error = m_PALDrvr.GetTrayNames(ref trays);
                    System.Threading.Thread.Sleep(250);
                    tries++;
                }
                if (error != 0)
                {
                    HandleError("PAL List Trays error: " + error.ToString());
                }
            }
            else
            {
                trays = "emuTray01;emuTray02;emuTray03;emuTray04;emuTray05;emuTray06";
            }

            //
            // Do the parsing for us !
            //
            var trayList = new List<string>();
            if (!string.IsNullOrEmpty(trays))
            {
                var names = trays.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PAL TRAYS LIST: " + names.Length);
                trayList.AddRange(names);

                if (TrayNamesRead != null)
                {
                    TrayNamesRead(this, new AutoSampleEventArgs(trayList, new List<string>()));
                }
                else
                {
                    HandleError("No traylist listeners.");
                }
            }
            else
            {
                ApplicationLogger.LogError(0, "Empty traylist returned by PAL");
            }

            TrayNames.Clear();
            TrayNames.AddRange(trayList);

            return trayList;
        }

        /// <summary>
        /// Populates <see cref="TrayNamesAndMaxVials"/>
        /// </summary>
        public void SetMaxVialsForTrays()
        {
            if (TrayNames.Count == 0)
            {
                return;
            }

            TrayNamesAndMaxVials.Clear();
            foreach (var tray in TrayNames)
            {
                ValidateVial(tray, 0, out int lastVial);
                TrayNamesAndMaxVials.Add(tray, lastVial);
            }
        }

        /// <summary>
        /// Input: Unsure, can be blank. Opens a "Trays" UI for selecting vials from a provided initial tray.
        /// </summary>
        /// <param name="vialTypes"></param>
        /// <param name="tray"></param>
        /// <param name="selectedVials"></param>
        /// <returns></returns>
        public string SelectVials(string vialTypes, string tray, string selectedVials)
        {
            if (m_emulation)
            {
                return "Emulated";
            }
            var error = m_PALDrvr.SelectVials(vialTypes, tray, ref selectedVials);

            return error.ToString() + ": " + selectedVials;
        }

        /// <summary>
        /// Validates the currently set tray and vial
        /// </summary>
        /// <returns></returns>
        public bool ValidateVial()
        {
            return ValidateVial(Tray, Vial, out _);
        }

        /// <summary>
        /// Validates a tray/vial pair
        /// </summary>
        /// <param name="tray">Tray name</param>
        /// <param name="vial">Vial number.</param>
        /// <param name="lastVial">The last vial in the specified tray</param>
        /// <returns>True if the tray and vial combination is valid; otherwise false</returns>
        public bool ValidateVial(string tray, int vial, out int lastVial)
        {
            lastVial = 0;
            if (m_emulation)
            {
                return false;
            }

            var tempStr = "";
            var error = m_PALDrvr.ValidateVial($"{tray}:{vial}", ref tempStr);

            int.TryParse(tempStr, out lastVial);

            return error == 0;
        }

        /// <summary>
        /// Queries the PAL's status.
        /// </summary>
        /// <returns>A string containing the status</returns>
        public string GetStatus()
        {
            if (m_emulation)
            {
                return "Emulated";
            }
            var tempString = "";
            var error = m_PALDrvr.GetStatus(ref tempString);
            //TODO: Handle error.
            OnFree();
            return tempString;
        }

        /// <summary>
        /// Resets the PAL. This takes a bit.
        /// </summary>
        public void ResetPAL()
        {
            if (m_emulation)
            {
                return;
            }

            var error = m_PALDrvr.ResetPAL();
            //TODO: Handle error.
            OnDeviceSaveRequired();
            OnFree();
        }

        ///
        /// Loads the method
        ///
        [LCMethodEvent("Start Method", MethodOperationTimeoutType.Parameter, true, 1, "MethodNames", 2, false)]
        public bool LoadMethod(double timeout, ISampleInfo sample, string methodName)
        {
            if (m_emulation)
            {
                return true;
            }

            //
            // We let start method run
            //
            var start = TimeKeeper.Instance.Now;

            //
            // Load the method...
            //
            sample.PAL.Method = methodName;
            LoadMethod(sample.PAL.Method,
                        sample.PAL.PALTray,
                        sample.PAL.Well,
                        string.Format("{0}", sample.Volume));

            StartMethod(timeout);

            //
            // We see if we ran over or not...if so then return failure, otherwise let it continue.
            //
            var status = "";
            var statusCheckError = m_PALDrvr.GetStatus(ref status);
            var span = TimeKeeper.Instance.Now.Subtract(start);
            if (timeout > span.TotalSeconds)
            {
                // If the PAL is waiting for data station synchronization, DO NOT CONTINUE IN THE START METHOD!
                // Exception: PAL default macros include a "Wait for DS" step in the 'Method Entry' macro. To support this, we add the 'time < 20 seconds', to allow passing it.
                // This will only "continue" once, so it mainly targets the method entry 'Wait for DS'
                if (!status.Contains("WAITING FOR DS") || span.TotalSeconds < 20)
                {
                    ContinueMethod(timeout - span.TotalSeconds);
                }
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Sets the variables to use the next time StartMethod is called
        /// </summary>
        /// <param name="method">The method name (string)</param>
        /// <param name="tray">The tray (string)</param>
        /// <param name="vial">The vial (string)</param>
        /// <param name="volume">The volume (string)</param>
        public bool LoadMethod(string method, string tray, int vial, string volume)
        {
            if (m_emulation)
            {
                return true;
            }

            m_method = method;
            m_tray = tray;
            if (ValidateVial(vial))
            {
                m_vial = vial;
            }
            else
            {
                HandleError("Vial number out of range");
                return false;
            }
            m_volume = volume;
            return true;
        }

        /// <summary>
        /// Runs a method as defined by the LoadMethod command.
        /// </summary>
        public bool StartMethod(double waitTimeout)
        {
            var timeout = Convert.ToInt32(waitTimeout);

            if (m_emulation)
            {
                return true;
            }
            var tempArgs = "Tray=" + m_tray + "; Index=" + m_vial.ToString() + "; Volume=" + m_volume;
            var errorMessage = "";
            var error = m_PALDrvr.StartMethod(m_method, ref tempArgs, ref errorMessage);

            // Check for an error!
            if (error == 1)
            {
                if (errorMessage.Contains("syringe") || errorMessage.ToLower().Contains("ul is not in pal") || errorMessage.ToLower().Contains("ml is not in pal"))
                {
                    HandleError("The syringe in the PAL Method does not match the physical syringe loaded in the PAL Loader Arm. " + errorMessage);
                }
                else
                {
                    HandleError(errorMessage);
                }
                return false;
            }

            if (!WaitUntilStopPoint(timeout))
            {
                return false;
            }

            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(DeviceStatus.InUseByMethod,
                "Done Injecting start method", this));
            OnFree();
            return true;
        }

        /// <summary>
        /// Wait until error, synchronization point, or ready
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [LCMethodEvent("Wait for SyncPoint", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool WaitUntilStopPoint(double timeout)
        {
            var status = "";
            var start = TimeKeeper.Instance.Now;
            var end = start;

            var delayTime = StatusPollDelay * 1000;
            while (end.Subtract(start).TotalSeconds < timeout)
            {
                var statusCheckError = m_PALDrvr.GetStatus(ref status);
                /*if (this.StatusUpdate != null)
                {
                    this.StatusUpdate(this, new DeviceStatusEventArgs(DeviceStatus.InUseByMethod,
                        "PAL: " + status + " " + statusCheckError.ToString(),
                        this));
                }*/
                if (status.Contains("ERROR"))
                {
                    if (status.ToLower().Contains("aspirating"))
                    {
                        HandleError("AspirationError");
                    }
                    else
                    {
                        HandleError(status);
                    }
                    return false;
                }

                if (status.Contains("DISCONNECTED"))
                {
                    // Disconnected is a problem
                    return false;
                }

                if (status.Contains("READY") || status.Contains("WAITING FOR DS"))
                {
                    return true;
                }

                System.Threading.Thread.Sleep(delayTime);
                end = TimeKeeper.Instance.Now;
            }

            // Timed out
            return false;
        }

        [LCMethodEvent("Throwup", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public void ThrowError(int timeToThrowup)
        {
            Error?.Invoke(this, new DeviceErrorEventArgs("AHHH!", null, DeviceErrorStatus.ErrorAffectsAllColumns, this, "None"));
        }

        /// <summary>
        /// Pauses the currently running method.
        /// </summary>
        ///
        [LCMethodEvent("Pause Method", .5, "", -1, false)]
        public void PauseMethod()
        {
            if (m_emulation)
            {
                return;
            }
            m_PALDrvr.PauseMethod();
            OnDeviceSaveRequired();
        }

        /// <summary>
        /// Resumes the method.
        /// </summary>
        ///
        [LCMethodEvent("Resume Method", 500, "", -1, false)]
        public void ResumeMethod()
        {
            if (m_emulation)
            {
                return;
            }
            m_PALDrvr.ResumeMethod();
            OnDeviceSaveRequired();
        }

        /// <summary>
        /// Continues the method. This is way different than ResumeMethod.
        /// </summary>
        [LCMethodEvent("Continue Method", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool ContinueMethod(double timeout, bool waitForComplete = false)
        {
            if (m_emulation)
            {
                return true;
            }

            var result = true;
            var prevStatus = "";
            m_PALDrvr.GetStatus(ref prevStatus);

            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(DeviceStatus.InUseByMethod,
                "continue method", this));
            m_PALDrvr.ContinueMethod();

            if (waitForComplete)
            {
                var status = prevStatus;
                while (status.Equals(prevStatus))
                {
                    System.Threading.Thread.Sleep(StatusPollDelay * 500);
                    m_PALDrvr.GetStatus(ref status);
                }

                result = WaitUntilStopPoint(timeout);
            }

            var statusMessage = "";
            var errorCode = m_PALDrvr.GetStatus(ref statusMessage);
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(DeviceStatus.InUseByMethod,
                "continue method end", this, statusMessage + " " + errorCode.ToString()));

            return result;
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [LCMethodEvent("Stop Method", .5, "", -1, false)]
        public void StopMethod()
        {
            if (m_emulation)
            {
                return;
            }
            m_PALDrvr.StopMethod();
        }

        /// <summary>
        /// Causes the software to wait for a "READY" response from the PAL before proceeding.
        /// </summary>
        /// <param name="waitTimeoutms">The timeout value, in milliseconds.</param>
        /// <returns>Integer error code.</returns>
        [LCMethodEvent("Wait Until Ready", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public int WaitUntilReady(double waitTimeoutms)
        {
            var timeoutms = Convert.ToInt32(waitTimeoutms);

            if (m_emulation)
            {
                return 0;
            }
            var endTime = TimeKeeper.Instance.Now + TimeSpan.FromMilliseconds(timeoutms - 100);
            var status = GetStatus();
            var currentTime = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            while (currentTime < endTime && !status.Contains("READY"))
            {
                var handles = new System.Threading.WaitHandle[1];
                handles[0] = AbortEvent;
                var done = System.Threading.WaitHandle.WaitAll(handles, 500);
                if (!done)
                {
                    return 1;
                }

                status = GetStatus();
                currentTime = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            }

            if (currentTime < endTime && status.Contains("READY"))
            {
                //Trigger 'ready' event
                OnFree();
                return 0;    //Great success!
            }

            if (currentTime > endTime)
            {
                //TODO: OnFree()?
                return 1;   //Timed out
            }

            OnFree();
            return 2;   //Not ready
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        #endregion

        #region INotifer Methods

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status", "Done Injecting start method", "continue method", "continue method end" };
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "AspirationError" };
        }

        #endregion

        #region IDevice Data Provider Methods

        /// <summary>
        /// Registers the method with a data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    Methods += remoteMethod;
                    ListMethods();
                    break;
            }
        }

        /// <summary>
        /// Unregisters the method from the data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    Methods -= remoteMethod;
                    break;
            }
        }

        #endregion

        #region Finch Methods

        /// <summary>
        /// Writes any performance data for the last method used.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        /*
        /// <summary>
        /// Returns the health of this device in a component data structure.
        /// </summary>
        /// <returns></returns>
        public FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            component.Status        = Status.ToString();
            component.Name          = Name;
            component.Type          = "Auto-sampler";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurement = new FinchScalarSignal();
            measurement.Name        = "Last Vial";
            measurement.Type = FinchDataType.String;
            measurement.Units       = "";
            measurement.Value       = Vial.ToString();
            component.Signals.Add(measurement);

            FinchScalarSignal measurementTray = new FinchScalarSignal();
            measurementTray.Name        = "Last Tray";
            measurementTray.Type        = FinchDataType.String;
            measurementTray.Units       = "";
            measurementTray.Value       = Tray;
            component.Signals.Add(measurementTray);

            FinchScalarSignal measurementVolume = new FinchScalarSignal();
            measurementVolume.Name        = "Last Volume";
            measurementVolume.Type        = FinchDataType.Double;
            measurementVolume.Units       = "uL";
            measurementVolume.Value       = Volume.ToString();
            component.Signals.Add(measurementVolume);

            FinchScalarSignal measurementMethod = new FinchScalarSignal();
            measurementMethod.Name        = "Last Method";
            measurementMethod.Type        = FinchDataType.String;
            measurementMethod.Units       = "";
            measurementMethod.Value       = Method;
            component.Signals.Add(measurementMethod);

            FinchScalarSignal measurementPort = new FinchScalarSignal();
            measurementPort.Name        = "Port";
            measurementPort.Type        = FinchDataType.String;
            measurementPort.Units       = "";
            measurementPort.Value       = this.PortName;
            component.Signals.Add(measurementPort);

            return component;
        }*/

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}