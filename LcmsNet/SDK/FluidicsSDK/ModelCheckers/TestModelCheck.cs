/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * 
 * Last Modified 6/4/2014 By Christopher Walters 
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses;
using LcmsNetSDK;

namespace FluidicsSDK.ModelCheckers
{
    //Test model check that will always be available  to the simulator in order to ensure the system is working.
    public class TestModelCheck:IFluidicsModelChecker
    {

        public TestModelCheck()
        {
            Name = "Test Model Check";
            IsEnabled = true;
            Category = ModelStatusCategory.Error;
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public ModelStatusCategory Category
        {
            get;
            set;
        }

        IEnumerable<ModelStatus> IFluidicsModelChecker.CheckModel()
        {
            List<ModelStatus> testList = new List<ModelStatus>();
            testList.Add(new ModelStatus("Test Check", "This status was created by the Test Model Check. The Model Checks are being run.", Category));
            if( StatusUpdate != null)
            {
                StatusUpdate(this, new LcmsNetDataClasses.Devices.classDeviceStatusEventArgs(LcmsNetDataClasses.Devices.enumDeviceStatus.NotInitialized, "Test Model Check Run.", null));
            }
            return testList;
        }

        public List<string> GetStatusNotificationList()
        {
            List<string> statusNotifications = new List<string>();
            statusNotifications.Add("Test Check Run");
            return statusNotifications;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<LcmsNetDataClasses.Devices.classDeviceErrorEventArgs> Error;
    }
}
