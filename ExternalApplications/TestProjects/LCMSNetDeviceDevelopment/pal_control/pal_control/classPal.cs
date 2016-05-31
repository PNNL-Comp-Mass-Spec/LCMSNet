using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using P;

namespace pal_control
{
    
    //TODO: create checkerror helper
    //      number, string for message, init status (mbool_accessible) -> all in obj
    //TODO: status event (make helper for each time status queried)/status helper
    //      return enum: timeout, error, success
    //TODO: Implement IDeviceControl <-no? this is for controls not classes?
    //TODO: Exceptions. Heh.
    
    public class classPal
    {
        #region Members

        private P.PalClass mobj_PALDrvr;
        private String mstr_methodsFolder;
        private bool mbool_accessible;

        private string mstr_method;
        private string mstr_tray;
        private string mstr_vial;
        private string mstr_volume;

        private System.IO.Ports.SerialPort mobj_serialPort;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classPal()
        {
            //Init routine
            mobj_PALDrvr = new P.PalClass();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the folder containing the PAL method files.
        /// </summary>
        public string MethodsFolder
        {
            get
            {
                return mstr_methodsFolder;
            }
            set
            {
                mstr_methodsFolder = value;
                mobj_PALDrvr.SelectMethodFolder(value); //Bad idea?
            }
        }

        /// <summary>
        /// Gets the accessibility status of the PAL. 0 = Not accessible.
        /// </summary>
        public bool Accessible
        {
            get
            {
                return mbool_accessible;
            }
        }

        /// <summary>
        /// Gets or sets the method for the PAL to run.
        /// </summary>
        public string Method
        {
            get
            {
                return mstr_method;
            }
            set
            {
                mstr_method = value;
            }
        }

        /// <summary>
        /// Gets or sets the tray for the PAL to use.
        /// </summary>
        public string Tray
        {
            get
            {
                return mstr_tray;
            }
            set
            {
                mstr_tray = value;
            }
        }

        /// <summary>
        /// Gets or sets the vial for the PAL to use.
        /// </summary>
        public string Vial
        {
            get
            {
                return mstr_vial;
            }
            set
            {
                mstr_vial = value;
            }
        }

        /// <summary>
        /// Gets or sets the volume (in uL).
        /// </summary>
        public string Volume
        {
            get
            {
                return mstr_volume;
            }
            set
            {
                mstr_volume = value;
            }
        }

        /// <summary>
        /// Gets or sets the serial port which the PAL is connected to.
        /// </summary>
        public System.IO.Ports.SerialPort Port
        {
            get
            {
                return mobj_serialPort;
            }
            set
            {
                mobj_serialPort = value;
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Internal error handler.
        /// </summary>
        private void HandleError(string message)
        {
            //TODO: error event
            Console.WriteLine(message);
        }

        /// <summary>
        /// Initializes the PAL. 
        /// This is done by starting paldriv.exe, resetting the PAL, 
        /// and loading the PAL's configuration.
        /// </summary>
        public void Initialize()
        {
            if (mbool_accessible == false)
            {
                if( mobj_PALDrvr == null )
                {
                    mobj_PALDrvr = new PalClass();
                }

                //System.Threading.Thread.Sleep(100);

                //Start paldriv.exe
                int error = mobj_PALDrvr.StartDriver("1", mobj_serialPort.PortName);
                //TODO: Error check
                switch (error)
                {
                    case 2:
                        HandleError("Port not available");
                        break;
                    case 3:
                        HandleError("Port in use");
                        break;
                }

                if (error > 0)
                {
                    string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    return;
                }
                
                int status = WaitUntilReady(10000);
                //TODO: Error check

                //Engage awesome drive
                if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal"))
                {
                    System.IO.File.Delete(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                }
                System.IO.TextWriter writer = System.IO.File.CreateText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\init.pal");
                writer.Close();

                //Reset the pal
                error = mobj_PALDrvr.ResetPAL();
                //TODO: Error check
                if (error > 0)
                {
                    string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    return;
                }

                status = WaitUntilReady(10000);
                //TODO: Error check

                //Load configuration
                error = mobj_PALDrvr.LoadConfiguration();
                //TODO: Error check
                if (error > 0)
                {
                    string tempStatus = "";
                    mobj_PALDrvr.GetStatus(ref tempStatus);
                    HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                    return;
                }

                status = WaitUntilReady(10000);
                //TODO: Error check

                DateTime start = DateTime.Now;
                DateTime end = start;
                while (end.Subtract(start).TotalMilliseconds < 10000)
                {
                    System.Threading.Thread.Sleep(10);
                    System.Windows.Forms.Application.DoEvents();
                    end = DateTime.Now;                    
                }

                //If we made it this far, success! We can now access the PAL.
                mbool_accessible = true;
            }
        }

        /// <summary>
        /// Sets the folder where the PAL methods are found.
        /// </summary>
        /// <param name="newFolderPath">The path to the new folder.</param>
        public void SetMethodFolder(string newFolderPath)
        {
            int error = mobj_PALDrvr.SelectMethodFolder(newFolderPath);
            //Error Checking
            if (error > 0)
            {
                string tempStatus = "";
                mobj_PALDrvr.GetStatus(ref tempStatus);
                HandleError("Unable to connect to PAL. Return value " + error + ". Error status " + tempStatus);
                return;
            }
        }

        /// <summary>
        /// Disables access to the PAL. This does not physically shutdown the PAL.
        /// </summary>
        public void Shutdown()
        {
            if (mbool_accessible == true)
            {
                mobj_PALDrvr = null;
                mbool_accessible = false;
            }
        }

        /// <summary>
        /// Lists the available methods for use with the PAL.
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public string ListMethods()
        {
            string methods = "";
            int error = mobj_PALDrvr.GetMethodNames(ref methods);
            return methods;
        }

        /// <summary>
        /// Lists the available trays known to the PAL. 
        /// </summary>
        /// <returns>A string containing the methods</returns>
        public string ListTrays()
        {
            string trays = "";
            int error = mobj_PALDrvr.GetTrayNames(ref trays);
            return trays;
        }

        /// <summary>
        /// Queries the PAL's status.
        /// </summary>
        /// <returns>A string containing the status</returns>
        public string GetStatus()
        {
            string tempString = "";
            int error = mobj_PALDrvr.GetStatus(ref tempString);

            return tempString;  //TODO: Process this into its components?
        }

        /// <summary>
        /// Resets the PAL. This takes a bit.
        /// </summary>
        public void ResetPAL()
        {
            int error = mobj_PALDrvr.ResetPAL();
            //TODO: Error check
        }

        /// <summary>
        /// Sets the variables to use the next time StartMethod is called
        /// </summary>
        /// <param name="method">The method name (string)</param>
        /// <param name="tray">The tray (string)</param>
        /// <param name="vial">The vial (string)</param>
        /// <param name="volume">The volume (string)</param>
        public void LoadMethod(string method, string tray, string vial, string volume)
        {
            mstr_method = method;
            mstr_tray = tray;
            mstr_vial = vial;
            mstr_volume = volume;
        }

        /// <summary>
        /// Runs a method as defined by the LoadMethod command.
        /// </summary>
        public void StartMethod(int timeout)
        {
            string tempArgs = "Tray=" + mstr_tray + "; Index=" + mstr_vial + "; Volume=" + mstr_volume;
            string errorMessage = "";
            int error = mobj_PALDrvr.StartMethod(mstr_method,ref tempArgs, ref errorMessage);
            //TODO: Error check


            DateTime start  = DateTime.Now;
            DateTime end    = start;
            string status   = "";
            bool isError    = true; 
            while (end.Subtract(start).TotalMilliseconds < timeout)
            {
                int statusCheckError = mobj_PALDrvr.GetStatus(ref status);
                if (status.Contains("ERROR"))
                {
                    HandleError(status);
                    break;
                }
                else if (status.Contains("READY"))
                {
                    isError = false;
                    break;
                }
                System.Threading.Thread.Sleep(500);
                Console.WriteLine(status);
                end = DateTime.Now;
            }

            if (isError == true)
            {
                HandleError("Holy SAASH!@#!@");
            }
            else
            {
                HandleError("John Elton is the best!");
            }
        }

        /// <summary>
        /// Pauses the currently running method.
        /// </summary>
        public void PauseMethod()
        {
            mobj_PALDrvr.PauseMethod();
        }

        /// <summary>
        /// Resumes the method.
        /// </summary>
        public void ResumeMethod()
        {
            mobj_PALDrvr.ResumeMethod();
        }

        /// <summary>
        /// Continues the method. This is way different than ResumeMethod.
        /// </summary>
        public void ContinueMethod()
        {
            mobj_PALDrvr.ContinueMethod();
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        public void StopMethod()
        {
            mobj_PALDrvr.StopMethod();
        }

        public int WaitUntilReady(int timeoutms)
        {
            DateTime endTime = DateTime.Now + TimeSpan.FromMilliseconds(timeoutms);
            string status = GetStatus();
            DateTime currentTime = DateTime.Now;
            while(currentTime < endTime && !status.Contains("READY"))
            {
                System.Threading.Thread.Sleep(500);
                status = GetStatus();
                currentTime = DateTime.Now;
            }

            if (currentTime < endTime && status.Contains("READY"))
            {
                //Trigger 'ready' event
                return 0;    //Great success!
            }

            else if (currentTime > endTime)
            {
                return 1;   //Timed out
            }

            else
            {
                return 2;   //Not ready
            }

        }

        public void WaitForDataSystem()
        {
            //wut
        }

        #endregion
    }
}
