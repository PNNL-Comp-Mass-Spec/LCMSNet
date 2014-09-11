using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;
using global::PluginTestObjectLibrary;






namespace LCMSNetPlugins
{

    /// <summary>
    /// Plugin template for the device being controlled.
    /// </summary>
    // TODO: Change Control and Glyph types to the proper type, and set the proper 
    // strings, for the device being controlled, in the following attribute code
    [classDeviceControlAttribute(typeof(PluginTestObjectControl),
                                 typeof(PluginTestObjectGlyph),
                                 "Plugin Test Object",
                                 "Testing Objects")
    ]
    public class PluginTestObjectDevice : IDevice
    {


        #region Members

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string mName;

        /// <summary>
        /// Device Object used to access the device.
        /// </summary>
        // TODO: Replace with class type being controlled
        // Use Refactor to rename member to 'real' name
        private PluginTestObject mTestObject = null;

        // Playing around, trying to see what this does
        enumDeviceStatus mStatus = enumDeviceStatus.NotInitialized;

        #endregion Members


        /// <summary>
        /// Default constructor - set default name and create Item object
        /// </summary>
        public PluginTestObjectDevice()
        {
            // TODO: Change 'boilerplate' code below: change Name to proper name
            // and 'create' code below to use the proper type and member name

            // Boilerplate

            // Name of item plugin as will be used by LCMSNet SW
            Name = "Test Object";
            
            // Create and intialize object being controlled here
            mTestObject = new PluginTestObject();         

        } // End constructor


        #region Events

        // This is all boilerplate from Brain M., Don't really know anything about how 
        // it works or how to use it

        /// <summary>
        /// Used to notify the system on status events/changes.
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Used to notify the system on errors.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;

        /// <summary>
        /// Used to notify others that properties have changed.  
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        #endregion


        // Item Device interface implementation
        #region IDevice Implementation

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }

        } // End Name property

        /// <summary>
        /// Gets or sets the version of the software interface.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status of the device.
        /// </summary>
        public enumDeviceStatus Status
        {
            // Playing around, trying to see what this does
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                //MessageBox.Show(mStatus.ToString(), "Status Set");
            }
        }

        /// <summary>
        /// Gets or sets the abort event of the device to be used during
        /// wait handles.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the error type the device could be in.
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
            get
            {
                // Most devices need to be a component.  The others are internal.
                return enumDeviceType.Component;
            }

        }// End DeviceType property

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        public bool Emulation
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the object being controlled
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Initialize(ref string errorMessage)
        {
            try
            {
                // TODO: Add any custom intialization code that extends beyond
                // the object's Open() method

                if ( !mTestObject.Opened )
                {

                    if (!mTestObject.Opened) mTestObject.Open(); // Don't open if already opened
                    MessageBox.Show(mTestObject.ToString()); // For testing only
                    classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMessageText(mName + " Opened"));
                    
                } // End if NOT opened
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;

        } // End Initialize() method

        /// <summary>
        /// Closes the object being controlled, ignores any errors that occur
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {
            try
            {
                // TODO: Add any custom shutdown code that extends beyond
                // the objects Close() method 
                if ( mTestObject.Opened )
                {
                    if (mTestObject.Opened) mTestObject.Close(); // Don't close if it's not opened
                    MessageBox.Show(mTestObject.ToString()); // For testing only
                    classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMessageText( mName + " Closed" ));

                } // End if opened
            }

            // Ignore any errors that occur during valve shutdown
            catch
            {
            }

            return true;

        } // End Shutdown() method

        /// <summary>
        /// Registers data providers
        /// </summary>
        /// <param name="key"></param>
        /// <param name="remoteMethod"></param>
        public void RegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            // This is done so that we can provide custom lists of strings
            // to the user interface if a particular method needs them in the method editor.
        }

        /// <summary>
        /// Unregisters data providers.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="remoteMethod"></param>
        public void UnRegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {

        }

        /// <summary>
        /// Gets or sets any performance data.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        /// <summary>
        /// Gets or sets any monitoring data needed.
        /// </summary>
        /// <returns></returns>
        public classMonitoringComponent GetHealthData()
        {
            return null;
        }
        
        public List<string> GetStatusNotificationList()
        {
            //TODO: ??? Add any custom code 
            return new List<string>() { "Empty String1", "Empty String 2" };
        }
        
        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Empty Failure 1", "Empty Failure 2" };
        }

        #endregion IDevice Implementation


        #region Properties

        /// <summary>
        /// Gets whether the item has been successfully opened
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return mTestObject.Opened;
            }
        }// End IsOpen property      

        /// <summary>
        ///Integer property for testing the data processing of the plugin
        /// </summary>
        [classPersistenceAttribute("TestObjectIntProperty")]
        public int IntTestProperty
        {
            get
            {

               return mTestObject.IntTestProperty;
            }

            set
            {
                mTestObject.IntTestProperty = value;
            }

        } // End IntTestProperty property
        
        #endregion  Properties


        #region Methods

        #region Operations and Commands

        /// <summary>
        /// Test method for the plugin test object:
        /// The simplest of tests, no arguments, no resturn value:
        /// displays a dialog box with the object name,
        /// that blocks execution until acknowledgement, to verify
        /// visually that the method has been called.
        /// </summary>        
        [classLCMethodAttribute("SimpleShowTest", enumMethodOperationTime.Parameter, "", -1, false)]
        public void SimpleShowTest(string timeSec)
        {
            Exception savedException = null;

            try
            {
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMethodCalledMessage(mName, "SimpleShowTest"));
                mTestObject.ShowTest(mName + " Says Hello !");

            } // End try block

            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, PluginHelperFunctions.FormatExceptionText(mName, ex));
                savedException = ex;

            } // End catch block

        } // End ShowTest() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Displays a dialog box with the user supplied message,
        /// that blocks execution until acknowledgement, to verify
        /// visually that the method has been called.
        /// </summary>        
        [classLCMethodAttribute("ShowTest", enumMethodOperationTime.Parameter, "", -1, false)]
        public void ShowTest(string timeSec, string message)
        {
            Exception savedException = null;
           
            try
            {
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMethodCalledMessage(mName, "ShowTest with " + message));
                mTestObject.ShowTest(message);

            } // End try block

            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, PluginHelperFunctions.FormatExceptionText(mName, ex));
                savedException = ex;

            } // End catch block

        } // End ShowTest() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Function that blocks execution for the user supplied delay time(Sec) to verify
        /// that proper sequencing is occuring in the LCMSNet method that's being tested.
        /// </summary>        
        [classLCMethodAttribute("WaitTest", enumMethodOperationTime.Parameter, "", -1, false)]
        public void WaitTest(string timeSec, string delayTime)
        {
            Exception savedException = null;
            
            try
            {
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMethodCalledMessage(mName, "WaitTest with " + delayTime));
                mTestObject.WaitTest( Convert.ToInt32( delayTime ));
               
            } // End try block

            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, PluginHelperFunctions.FormatExceptionText(mName, ex));
                savedException = ex;               

            } // End catch block

        } // End WaitTest() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Function used to validate proper plugin data processing, by returning to
        /// the calling LCMSNet method the sum of the current value of the IntTestProperty and 
        /// the argument that was passed to the method.
        /// </summary>        
        [classLCMethodAttribute("FunctionTest", enumMethodOperationTime.Parameter, "", -1, false)]
        public int FunctionTest(string timeSec, string argumentIn)
        {
            Exception savedException = null;
            int Result = 0;

            try
            {
                int argument = Convert.ToInt32(argumentIn);
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMethodCalledMessage(mName, "FunctionTest with " + argument));
                Result = mTestObject.FunctionTest( argument );
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatResultMessage(mName, Result));

            } // End try block

            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, PluginHelperFunctions.FormatExceptionText(mName, ex));
                savedException = ex;
                return 0;

            } // End catch block

            return Result;

        } // End FunctionTest() method

        #endregion Operations and Commands

        #region Queries
        
        /// <summary>
        /// Test query for the plugin test object:
        /// Query used to test that an LCMSNet method returns the proper real number value
        /// from a plugin query function
        /// </summary>  
        [classLCMethodAttribute("TestQuery", enumMethodOperationTime.Parameter, "", -1, false)]
        public double TestQuery(string timeSec)
        {
            Exception savedException = null;
            double QueryResult = 0;

            try
            {
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatMethodCalledMessage(mName, "TestQuery"));
                QueryResult = mTestObject.TestQuery();
                classApplicationLogger.LogMessage(0, PluginHelperFunctions.FormatResultMessage(mName, QueryResult));

            } // End try block

            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, PluginHelperFunctions.FormatExceptionText(mName, ex));
                savedException = ex;
                return 0;

            } // End catch block

            return QueryResult;

        } // End TestQuery() method

        /// <summary>
        /// Exposes the Item's ToString method so it can be accessed by
        /// users of the this class, e.g. the Control and Glyph classes, and LCMSNet SW
        /// </summary> 
        public string DeviceToString()
        {
            return mTestObject.ToString();
        }

        /// <summary>
        /// Overides ToString() method to return the item name, this is required for the
        /// LCMSNetwork application to properly use the device name
        /// </summary>        
        public override string ToString()
        {
            return Name;
        }

        #endregion Queries

        #endregion Methods


    } // End ItemDevice class

} // End LCMSNetPlugins NS
