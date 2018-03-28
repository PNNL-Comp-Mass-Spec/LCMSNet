//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 06/29/2010
//
//*********************************************************************************************************

namespace LcmsNetPlugins.Bruker.Devices
{
    #region "Delegates used within Bruker Start classes"

    public delegate void delegateBrukerMsgReceived(classBrukerComConstants.SxcReplies sXcReply);

    public delegate void delegatelNetworkDataReceived();

    public delegate void delegateDeviceError(object sender, string message);
    
    #endregion
}
