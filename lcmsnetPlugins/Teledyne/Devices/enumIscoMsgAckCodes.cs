//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 02/08/2011
//
// Last modified 02/08/2011
//*********************************************************************************************************

namespace LcmsNet.Devices.Pumps
{
    //*********************************************************************************************************
    // Enum for ISCO message acknowledge codes
    //**********************************************************************************************************

    public enum enumIscoMsgAckCodes
    {
        Recvd = 0,
        PumpBusy = 1,
        PumpComError = 2
    }
}
