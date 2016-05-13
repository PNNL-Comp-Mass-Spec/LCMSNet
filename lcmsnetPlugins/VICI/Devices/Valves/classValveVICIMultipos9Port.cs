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
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;

namespace LcmsNet.Devices.Valves
{
    [Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]	
    [classDeviceControlAttribute(typeof(controlValveVICIMultiPos),
                                 "Nine-Port",
                                 "Valves Multi-Position")
    ]
    public class classValveVICIMultipos9Port:classValveVICIMultiPos, INinePortValve
    {
        private const int numPositions = 8;

        public classValveVICIMultipos9Port()
            : base(numPositions)
        {
        }

        public classValveVICIMultipos9Port(SerialPort port)
            : base(numPositions, port)
        {
        }

        [classLCMethodAttribute("Set Position", LC_EVENT_SET_POSITION_TIME_SECONDS, true, "", -1, false)]
        public void SetPosition(EightPositionState position)
        {
           enumValveErrors err = base.SetPosition((int)position);
        }

        public override Type GetStateType()
        {
            return typeof(EightPositionState);
        }
    }
}
