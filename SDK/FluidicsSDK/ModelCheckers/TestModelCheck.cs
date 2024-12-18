﻿/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute

 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

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

        private string name;
        private bool isEnabled;
        private ModelStatusCategory category;

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }

        public ModelStatusCategory Category
        {
            get => category;
            set => this.RaiseAndSetIfChanged(ref category, value);
        }

        IEnumerable<ModelStatus> IFluidicsModelChecker.CheckModel()
        {
            var testList = new List<ModelStatus>
            {
                new ModelStatus("Test Check", "This status was created by the Test Model Check. The Model Checks are being run.", Category)
            };
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(DeviceStatus.NotInitialized, "Test Model Check Run.", null));
            return testList;
        }

        public List<string> GetStatusNotificationList()
        {
            var statusNotifications = new List<string> {
                "Test Check Run"
            };
            return statusNotifications;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<DeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
