//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/8/2014 by Christopher Walters
//						12/01/2009 (DAC) - Modified to accomodate change of vial from string to int
//						12/02/2009 (DAC) - Added retrieving tray list and firing event to pass list to other objects
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P;
using System.Xml;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNet.Devices;
using LcmsNetDataClasses.Logging;
using System.IO.Ports;
using FluidicsSDK.Devices;

namespace LcmsNet.Devices.Pal
{
    /// <summary>
    /// Interface to the LEAP PAL robotic sampler.
    /// </summary>
	[Serializable]	
    
    [classDeviceControlAttribute(typeof(controlPal),
                                 "PAL Autosampler",
                                 "Auto-Samplers")]
    public class classPal : IDevice, IAutoSampler, IFluidicsSampler
    {
        #region Members

        /// <summary>
        /// An object which can use the PAL dll functions.
        /// </summary>
        private P.PalClass mobj_PALDrvr;

        /// <summary>
        /// The location of the directory containing the PAL methods.
        /// </summary>
        private String mstring_methodsFolder;

        /// <summary>
        /// A flag indicating whether or not the PAL has been initialized.
        /// </summary>
        private bool mbool_accessible;

        /// <summary>
        /// The current method to execute.
        /// </summary>
        private string mstring_method = "";

        /// <summary>
        /// The current tray to use.
        /// </summary>
        private string mstring_tray = "";

        /// <summary>
        /// The current vial to use.
        /// </summary>
        private int mint_vial = 0;

        /// <summary>
        /// The valid range of vials.
        /// </summary>
        private enumVialRanges mobj_vialRange;// = enumVialRanges._96Well;

        /// <summary>
        /// The current volume setting.
        /// </summary>
        private string mstring_volume = "";

        /// <summary>
        /// Name for fluidics designer.
        /// </summary>
        private string mstring_name;

        /// <summary>
        /// The PAL's version information.
        /// </summary>
        private string mstring_version;

        /// <summary>
        /// The current status of the PAL.
        /// </summary>
        private enumDeviceStatus menum_status;

        /// <summary>
        /// Indicates whether or not the PAL is in emulation mode.
        /// </summary>
        private bool mbool_emulation;

        private const int CONST_PALERR_PORTUNAVAILABLE = 2;
        private const int CONST_PALERR_PORTINUSE = 3;
        private const int CONST_WAITTIMEOUT = 10000; //milliseconds

        #endregion

        #region Events
        /// <summary>
        /// Indicates that a change requiring saving in the Fluidics designer has occured.
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Indicates that the device is not busy and can accept commands.
        /// </summary>
        public event DelegateDeviceFree Free;    
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        /// <summary>
        /// Fired when new tray names are available.
        /// </summary>
        public event EventHandler<classAutoSampleEventArgs> TrayNames;
        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        public event EventHandler<classAutoSampleEventArgs> MethodNames;
        /// <summary>
        /// Fired to the method editor handler with a List of method names 
        /// </summary>
        public event DelegateDeviceHasData Methods;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classPal()
        {
            mobj_vialRange  = enumVialRanges.Well96;
            mstring_name    = "pal";
            AbortEvent      = new System.Threading.ManualResetEvent(false);
            StatusPollDelay = 10;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }
        /// <summary>
        /// Gets or sets whether the device is emulation mode.
        /// </summary>
        //[classPersistenceAttribute("Emulated")]
        public bool Emulation
        {
            get
            {
                return mbool_emulation;
            }
            set
            {
                mbool_emulation = value;              
            }
        }
        /// <summary>
        /// The current status of the PAL.
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return menum_status;
            }
            set
			{
				if (StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "Status", this));
				menum_status = value;
            }
        }
        /// <summary>
        /// The PAL's version information.
        /// </summary>
        public string Version
        {
            get
            {
                return mstring_version;
            }
            set
            {
                mstring_version = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets a uniquely identifiable name.
        /// </summary>
        public string Name
        {
            get
            {
                return mstring_name;
            }
            set
            {
                mstring_name = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the folder containing the PAL method files.
        /// </summary>
        [classPersistenceAttribute("MethodsFolder")]
        public string MethodsFolder
        {
            get
            {
                return mstring_methodsFolder;
            }
            set
            {
                mstring_methodsFolder = value;
				if (mbool_emulation == false)
				{
					if (value == null)
					{
						throw new NullReferenceException("The methods folder for the PAL provided by the user was null.");
					}
					if (!System.IO.Directory.Exists(value))
					{
						throw new System.IO.DirectoryNotFoundException("The directory does not exist: " + value + "");
					}
					mobj_PALDrvr.SelectMethodFolder(value); 
				}
            }
        }
        /// <summary>
        /// Gets or sets the method for the PAL to run.
        /// </summary>
        [classPersistenceAttribute("Method")]
        public string Method
        {
            get
            {
                return mstring_method;
            }
            set
            {
                mstring_method = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the tray for the PAL to use.
        /// </summary>
        [classPersistenceAttribute("Tray")]
        public string Tray
        {
            get
            {
                return mstring_tray;
            }
            set
            {
                mstring_tray = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// The range of valid vial numbers
        /// </summary>
        public enumVialRanges VialRange
        {
            get
            {
                return mobj_vialRange;
            }
            set
            {
                mobj_vialRange = value;
            }
        }
        /// <summary>
        /// Gets or sets the vial for the PAL to use.
        /// </summary>
        [classPersistenceAttribute("Vial")]
        public int Vial
        {
            get
            {
                return mint_vial;
            }
            set
            {
                if (ValidateVial(value))
                {
                    mint_vial = value;
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
        [classPersistenceAttribute("Volume")]
        public string Volume
        {
            get
            {
                return mstring_volume;
            }
            set
            {
                mstring_volume = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the serial port which the PAL is connected to.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public string PortName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the delay when polling for system status in seconds
        /// </summary>
        [classPersistenceAttribute("StatusPollDelay")]
        public int StatusPollDelay
        {
            get;
            set;
        }
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
        #endregion

        #region Methods

        /// <summary>
        /// Indicates that the device is not busy and can accept commands.
        /// </summary>
        public virtual void OnFree()
        {
            if (Free != null)
            {
                Free(this);
            }
        }


        /// <summary>
        /// Indicates that a change requiring saving in the Fluidics designer has occured.
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
        }

        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
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
            return (vial >= 1 && vial <= (int)mobj_vialRange);
        }

        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, Exception ex)
        {
            if (Error != null)
            {
					Error(this, new classDeviceErrorEventArgs(message, ex, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, message));							
            }            
        }
        /// <summary>
        /// Initializes the PAL. 
        /// This is done by starting paldriv.exe, resetting the PAL, 
        /// and loading the PAL's configuration.
        /// </summary>
        public bool Initialize(ref string errorMessage)
        {
            if (mbool_emulation == true)
			{
                mbool_accessible = true;
                ListMethods();
                ListTrays();
                return true;                
            }

            if (mbool_accessible == false)
            {
                if( mobj_PALDrvr == null )
                {
                    mobj_PALDrvr = new P.PalClass();
                }

                if (PortName == null)
                {
                    errorMessage = "COM Port not set.  Please select a COM port name.";
                    return false;
                }

                //Start paldriv.exe
                int error = mobj_PALDrvr.StartDriver("1", PortName);
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
					string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }
                
                int status = WaitUntilReady(CONST_WAITTIMEOUT);                                
                if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal"))
                {
                    System.IO.File.Delete(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                }
                System.IO.TextWriter writer = System.IO.File.CreateText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                writer.Close();

                //Reset the pal
                error = mobj_PALDrvr.ResetPAL();                
                if (error > 0)
				{
                    string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }

                status = WaitUntilReady(CONST_WAITTIMEOUT);
                
                //Load configuration
                error = mobj_PALDrvr.LoadConfiguration();
                                
                if (error > 0)
				{
                    string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    errorMessage = "Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus;
                    OnFree();
                    return false;
                }

                status = WaitUntilReady(CONST_WAITTIMEOUT);
                
                //10 second delay (this kind of sucks)
                DateTime start = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
                DateTime end = start;
                while (end.Subtract(start).TotalMilliseconds < CONST_WAITTIMEOUT)
                {
                    System.Threading.Thread.Sleep(StatusPollDelay);
                    System.Windows.Forms.Application.DoEvents();
                    end = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));                    
                }

                string methodsFolder = classLCMSSettings.GetParameter("PalMethodsFolder");
                bool exists = System.IO.Directory.Exists(methodsFolder);
                if (!exists)
                {
                    string newMethodsFolder = methodsFolder.Replace("Program Files (x86)", "Program Files");
                    LcmsNetDataClasses.Logging.classApplicationLogger.LogError(0,
                                        string.Format("Could not find the PAL Methods folder {0}.  Looking for folder path: {1}",
                                            methodsFolder,
                                            newMethodsFolder));
                    methodsFolder = newMethodsFolder;
                }
                MethodsFolder = methodsFolder;
                WaitUntilReady(CONST_WAITTIMEOUT);
                //If we made it this far, success! We can now access the PAL.
                mbool_accessible = true;                
                //list methods
                ListMethods();
                status = WaitUntilReady(CONST_WAITTIMEOUT);
                // list trays
				ListTrays();		
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
            if (mbool_emulation == true)
            {
                return;
            }

            int error = mobj_PALDrvr.SelectMethodFolder(newFolderPath);
            //Error Checking
            if (error > 0)
            {
                string tempStatus = "";
                mobj_PALDrvr.GetStatus(ref tempStatus);
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
            if (mbool_emulation == true)
            {
                mbool_accessible = false;
                return true;
            }

            if (mbool_accessible == true)
            {
                mobj_PALDrvr = null;
                mbool_accessible = false;
            }            
            OnDeviceSaveRequired();
            return true;
        }

        /// <summary>
        /// Lists the available methods for use with the PAL.
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public string ListMethods()
        {
            string methods = "";
            /// 
            /// Find the methods from the device (or emulated one)
            /// 
            if (mbool_emulation == false)
            {
                int error = mobj_PALDrvr.GetMethodNames(ref methods);
                //TODO: Handle error.
            }
            else
            {
                methods = "Example;Dance;Headstand;Self-Destruct";
            }            
            /// 
            /// Now to update the user interface, get the method names
            /// and send them to any listeners.
            /// 
            if (methods != null)
            {
                string[] methodNames = methods.Split(new string [] {";"}, StringSplitOptions.RemoveEmptyEntries);
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PAL METHODS LENGTH: " + methodNames.Length);
                List<string> data    = new List<string>();

                if (methodNames.Length > 0)
                    data.AddRange(methodNames);

                // For UI and other.
                if (data.Count > 0 && MethodNames != null)
                {
                    MethodNames(this, new classAutoSampleEventArgs(new List<string>(), data));
                }
                // For method editor.  Needs to be refactored to use event args like above.
                if (data.Count > 0 && Methods != null)
                {
                    List<object> objects = new List<object>();
                    foreach (string name in data)
                    {
                        objects.Add(name);
                    }
                    Methods(this, objects);
                }
            }
            return methods;
        }

        /// <summary>
        /// Lists the available trays known to the PAL. 
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public List<string> ListTrays()
        {
            
            string trays = "";
            int tries = 0;
            int MAX_TRIES = 50;
            if (mbool_emulation == false)
            {
                int error = int.MaxValue;
                while (error != 0 && string.IsNullOrEmpty(trays) && tries < MAX_TRIES)
                {
                    error = mobj_PALDrvr.GetTrayNames(ref trays);
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

            /// 
            /// Do the parsing for us !
            /// 
            List<string> trayList = new List<string>();
            if (!string.IsNullOrEmpty(trays))
            {
                string[] names = trays.Split(new string []{";"}, StringSplitOptions.RemoveEmptyEntries);
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PAL TRAYS LIST: " + names.Length);
                trayList.AddRange(names);

                if (TrayNames != null)
                {
                    TrayNames(this, new classAutoSampleEventArgs(trayList, new List<string>()));
                }
                else
                {
                    HandleError("No traylist listeners.");
                }
            }
            else
            {
                classApplicationLogger.LogError(0, "Empty traylist returned by PAL");
            }
            return trayList;
        }

        /// <summary>
        /// Queries the PAL's status.
        /// </summary>
        /// <returns>A string containing the status</returns>
        public string GetStatus()
        {
            if (mbool_emulation == true)
            {
                return "Emulated";
            }
            string tempString = "";
            int error = mobj_PALDrvr.GetStatus(ref tempString);
            //TODO: Handle error.
            OnFree();
            return tempString;  
        }

        /// <summary>
        /// Resets the PAL. This takes a bit.
        /// </summary>
        public void ResetPAL()
        {
            if (mbool_emulation == true)
            {
                return;
            }

            int error = mobj_PALDrvr.ResetPAL();
            //TODO: Handle error.
            OnDeviceSaveRequired();
            OnFree();
        }               
        /// 
        /// Loads the method 
        /// 
        [classLCMethodAttribute("Start Method", enumMethodOperationTime.Parameter, true, 1, "MethodNames", 2, false)]       
        public bool LoadMethod(double timeout, classSampleData sample, string methodName)
        {
            if(mbool_emulation == true)
            {
                return true;
            }

            /// 
            /// We let start method run 
            /// 
            DateTime start = LcmsNetSDK.TimeKeeper.Instance.Now; 
  
            /// 
            /// Load the method...
            /// 
            sample.PAL.Method = methodName;
            LoadMethod(sample.PAL.Method,
                        sample.PAL.PALTray,
                        sample.PAL.Well,
                        string.Format("{0}",sample.Volume));

            StartMethod(timeout);     

            /// 
            /// We see if we ran over or not...if so then return failure, otherwise let it continue.
            /// 
            TimeSpan span = LcmsNetSDK.TimeKeeper.Instance.Now.Subtract(start);
            if (timeout > span.TotalSeconds)
                ContinueMethod(timeout - span.TotalSeconds);
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
            if(mbool_emulation)
            {
                return true;
            }

            mstring_method = method;
            mstring_tray   = tray;
            if (ValidateVial(vial))
            {
                mint_vial = vial;
            }
            else
            {
                HandleError("Vial number out of range");
                return false;
            }
            mstring_volume = volume;
            return true;
        }
        /// <summary>
        /// Runs a method as defined by the LoadMethod command.
        /// </summary>
        public bool StartMethod(double waitTimeout)
        {  
            int timeout = Convert.ToInt32(waitTimeout);

            if (mbool_emulation == true)
            {
                return true;
            }
            string tempArgs     = "Tray=" + mstring_tray + "; Index=" + mint_vial.ToString() + "; Volume=" + mstring_volume;
            string errorMessage = "";
            int error           = mobj_PALDrvr.StartMethod(mstring_method, ref tempArgs, ref errorMessage);
                        
            /// 
            /// Check for an error!
            /// 
            string status   = "";
            if (error == 1)
            {
                if (errorMessage.Contains("syringe"))
                {
                    HandleError("The syringe in the PAL Method does not match the physical syringe loaded in the PAL Loader Arm. " + errorMessage);
                }
                else
                {                    
                    HandleError(errorMessage);
                }
                return false;
            }

            DateTime start = LcmsNetSDK.TimeKeeper.Instance.Now; 
            DateTime end    = start;

            int delayTime = StatusPollDelay * 1000;
            while (end.Subtract(start).TotalSeconds < timeout)
            {
                int statusCheckError = mobj_PALDrvr.GetStatus(ref status);
                if (this.StatusUpdate != null)
                {
                    this.StatusUpdate(this, new classDeviceStatusEventArgs(enumDeviceStatus.InUseByMethod,
                        "PAL: " + status + " " + statusCheckError.ToString(),
                        this));                         
                }
                if (status.Contains("ERROR"))
                {
                    HandleError(status);
                    return false;                    
                }
                else if (status.Contains("READY"))
                {
                    break;
                }
                else if (status.Contains("WAITING FOR DS 1;Resetting PAL;"))
                {
                    break;
                }
                System.Threading.Thread.Sleep(delayTime);
                end = LcmsNetSDK.TimeKeeper.Instance.Now; 
            }

            if (this.StatusUpdate != null)
            {
                this.StatusUpdate(this, new classDeviceStatusEventArgs(enumDeviceStatus.InUseByMethod,
                    "Done Injecting start method",
                    this));
            }
            OnFree();
            return true;
        }


        [classLCMethodAttribute("Throwup", enumMethodOperationTime.Parameter, "", -1, false)]        
        public void ThrowError(int timeToThrowup)
        {
            if (Error != null)
            {
					Error(this, new classDeviceErrorEventArgs("AHHH!", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, "None"));
            }
        }
        /// <summary>
        /// Pauses the currently running method.
        /// </summary>
        /// 
        [classLCMethodAttribute("Pause Method", .5, "", -1, false)]
        public void PauseMethod()
        {
            if (mbool_emulation == true)
            {
                return;
            }
            mobj_PALDrvr.PauseMethod();
            OnDeviceSaveRequired();
        }

        /// <summary>
        /// Resumes the method.
        /// </summary>
        /// 
        [classLCMethodAttribute("Resume Method", 500, "", -1, false)]
        public void ResumeMethod()
        {
            if (mbool_emulation == true)
            {
                return;
            }
            mobj_PALDrvr.ResumeMethod();
            OnDeviceSaveRequired();
        }

        /// <summary>
        /// Continues the method. This is way different than ResumeMethod.
        /// </summary>
        [classLCMethodAttribute("Continue Method", enumMethodOperationTime.Parameter, "", -1, false)]
        public void ContinueMethod(double timeout)
        {
            if (mbool_emulation == true)
            {
                return;
            }

            if (this.StatusUpdate != null)
            {
                this.StatusUpdate(this, new classDeviceStatusEventArgs(enumDeviceStatus.InUseByMethod,
                    "continue method",
                    this));
            }
            mobj_PALDrvr.ContinueMethod();
            
            string statusMessage = "";
            int errorCode = mobj_PALDrvr.GetStatus(ref statusMessage);
            if (this.StatusUpdate != null)
            {
                this.StatusUpdate(this, new classDeviceStatusEventArgs(enumDeviceStatus.InUseByMethod,
                    "continue method end: " + statusMessage + " " + errorCode.ToString(),
                    this));
            }
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [classLCMethodAttribute("Stop Method", .5, "", -1, false)]
        public void StopMethod()
        {
            if (mbool_emulation == true)
            {
                return;
            }
            mobj_PALDrvr.StopMethod();            
        }

        /// <summary>
        /// Causes the software to wait for a "READY" response from the PAL before proceeding.
        /// </summary>
        /// <param name="timeoutms">The timeout value, in milliseconds.</param>
        /// <returns>Integer error code.</returns>
        [classLCMethodAttribute("Wait Until Ready", enumMethodOperationTime.Parameter, "", -1, false)]
        public int WaitUntilReady(double waitTimeoutms)
        {
            int timeoutms = Convert.ToInt32(waitTimeoutms);

            if (mbool_emulation == true)
            {
                return 0;
            }
            DateTime endTime = LcmsNetSDK.TimeKeeper.Instance.Now + TimeSpan.FromMilliseconds(timeoutms - 100);
            string status    = GetStatus();
            DateTime currentTime = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            while(currentTime < endTime && !status.Contains("READY"))
            {
                System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[1];
                handles[0]  = this.AbortEvent;
                bool done   = System.Threading.WaitHandle.WaitAll(handles, 500);
                if (!done)
                {
                    return 1;
                }
                
                status = GetStatus();
                currentTime = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            }

            if (currentTime < endTime && status.Contains("READY"))
            {
                //Trigger 'ready' event
                OnFree();
                return 0;    //Great success!
            }

            else if (currentTime > endTime)
            {
                //TODO: OnFree()?
                return 1;   //Timed out
            }

            else
            {
                OnFree();
                return 2;   //Not ready
            }

        }
        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mstring_name;
        }

        #endregion
      
        #region INotifer Methods

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status", "Done Injecting start method", "continue method", "continue method end:" };
	  }

	  public List<string> GetErrorNotificationList()
	  {
          return new List<string>();
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
        /// <summary>
        /// Returns the health of this device in a component data structure.
        /// </summary>
        /// <returns></returns>
        /*public FinchComponentData GetData()
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
    }
}
