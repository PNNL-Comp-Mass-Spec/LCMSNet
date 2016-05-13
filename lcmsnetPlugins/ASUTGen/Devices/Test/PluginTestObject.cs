using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PluginTestObjectLibrary
{
    public class PluginTestObject
    {


        #region Members

        /// <summary>
        /// Flag used to check if the device has been opened.
        /// </summary>
        protected bool mOpened = false;

        /// <summary>
        /// Integer data member for testing plugin
        /// </summary>
        protected int mIntDataMember = 0x00;

        #endregion Members


        #region Methods

        /// <summary>
        /// Opens the test object 
        /// </summary>
        public virtual void Open()
        {
            // Create slight time delay to simulate opening
            Thread.Sleep(1000);
            mOpened = true;

        }// End  Open() method

        /// <summary>
        /// Checks to see if the device has already been opened. If it has not
        /// an exception is thrown
        /// </summary>
        public void CheckOpened()
        {
            if (mOpened != true) throw new Exception("Device has not been opened yet");

        }// End  CheckOpened() method

        /// <summary>
        /// Closes the test object
        /// </summary>
        public virtual void Close()
        {
            try
            {
                // Create slight time delay to simulate closing
                Thread.Sleep(1000);              
            }
            finally
            {
                mOpened = false;
            }

        }// End Close() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Displays a dialog box with the user supplied message,
        /// that blocks execution until acknowledgement, to verify
        /// visually that the method has been called.
        /// </summary>
        public void ShowTest(string message)
        {
            CheckOpened();

            MessageBox.Show(message, "Plugin Test Method Called");

        } // End ShowTest() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Function that blocks execution for the user supplied delay time(Sec) to verify
        /// that proper sequencing is occuring in the LCMSNet method that's being tested.
        /// </summary>
        public void WaitTest(int delayTime)
        {
            CheckOpened();

            Thread.Sleep(delayTime * 1000);

        } // End WaitTest() method

        /// <summary>
        /// Test method for the plugin test object:
        /// Function used to validate proper plugin data processing, by returning to
        /// the calling LCMSNet method the sum of the current value of the IntTestProperty and 
        /// the argument that was passed to the method.
        /// </summary>
        public int FunctionTest(int argument)
        {
            CheckOpened();

            return argument + mIntDataMember;

        } // End WaitTest() method

        /// <summary>
        /// Test query for the plugin test object:
        /// Query used to test that an LCMSNet method returns the proper real number value
        /// from a plugin query function
        /// </summary>
        public double TestQuery()
        {
            CheckOpened();

            return 2.71828;

        } // End Query1() method

        public override string ToString()
        {
            // TODO: Add device specific ToString() code
            return String.Format("Plugin Test Object: Property={0} | Opened= {1}", mIntDataMember , mOpened);

        } // End ToString() method

        #endregion Methods


        #region Properties

        /// <summary>
        /// Flag used to see if the device has been opened.
        /// </summary>
        public bool Opened
        {
            get
            {
                return mOpened;
            }
        }// End Opened Property

        /// <summary>
        ///Integer property for testing the data processing of the plugin
        /// </summary>
        public int IntTestProperty
        {
            set
            {
                mIntDataMember = value;
            }
            get
            {
                return mIntDataMember;
            }
        }// End IntTestProperty Property

        #endregion Properties


    } // End PluginTestObject class


} // End PluginTestObjectLibrary
