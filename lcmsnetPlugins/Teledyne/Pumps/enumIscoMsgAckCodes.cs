//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 02/08/2011
//
//*********************************************************************************************************

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Enum for ISCO message acknowledge codes
    /// </summary>
    public enum enumIscoMsgAckCodes
    {
        Recvd = 0,
        PumpBusy = 1,
        PumpComError = 2
    }
}
