//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/03/2011
//
//*********************************************************************************************************

namespace LcmsNetPlugins.Teledyne.Devices
{
    /// <summary>
    /// Enum for ISCO pump problem reporting constants
    /// </summary>
    public enum enumIscoProblemStatus
    {
        None,
        CylinderEmpty,
        CylinderBottom,
        OverPressure,
        UnderPressure,
        MotorFailure,
        ComError,
        InitializationError,
        MessageParseError,
        DeviceNotInitialized
    }
}
