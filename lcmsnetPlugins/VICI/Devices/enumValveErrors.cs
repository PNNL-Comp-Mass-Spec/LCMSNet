﻿//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.Valves
{
    /// <summary>
    /// Errors generated by the valve
    /// </summary>
    public enum enumValveErrors
    {
        Success = 0,
        UnauthorizedAccess,
        TimeoutDuringRead,
        TimeoutDuringWrite,
        ValvePositionMismatch,
        BadArgument,
        UndefinedPort
    }
}
