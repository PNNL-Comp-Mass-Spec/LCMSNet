//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/03/2011
//
// Last modified 03/03/2011
//*********************************************************************************************************

namespace LcmsNet.Devices.Pumps
{
    //*********************************************************************************************************
    // Enum for ISCO operation status
    //**********************************************************************************************************

    public enum enumIscoOperationStatus
    {
        Stopped,
        Running,
        Refilling,
        Hold,
        Equilibrating,
        InitSucceeded,
        Initializing,
        FlowSet,
        PressSet,
        ConstFlow,
        ConstPress
    }
}
