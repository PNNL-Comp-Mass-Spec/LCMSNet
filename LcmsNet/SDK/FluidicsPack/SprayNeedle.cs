/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 1/6/2013
 * 
 * Last Modified 1/6/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using System.Timers;
using FluidicsSDK.Base;

namespace FluidicsPack
{
    [classDeviceControlAttribute(null,
                                 typeof(FluidicsSprayNeedle),
                                 "Spray Needle",
                                 "Fluidics Components")]
    public class SprayNeedle:FluidicsComponentBase
    {
           public SprayNeedle()
           {
               Name = "Spray Needle";
               Version = "infinity.";
               Position = 1;
               AbortEvent = new System.Threading.ManualResetEvent(false);
           }          

            public int Position { get; set; }     
    }
}
