/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 * 
 * Last Modified 6/5/2014 By Christopher Walters 
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;
using System.IO.Ports;

namespace LcmsNet.Devices.Valves
{
    [Serializable]    
    [classDeviceControlAttribute(typeof(controlValveVICI2Pos),                            
                                 "Four-Port",
                                 "Valves Two-Position")
    ]    
    public class classValveVICI2pos4port: classValveVICI2Pos, IFourPortValve
    {

        public classValveVICI2pos4port()
            : base()
        {
        }

        public classValveVICI2pos4port(SerialPort port)
            : base(port)
        {

        }
    }
}
