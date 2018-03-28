//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/17/2011
//
//*********************************************************************************************************

namespace LcmsNetPlugins.Teledyne.Pumps
{
    #region "Delegates for use with ISCO pump control"

    public delegate void DelegateIscoPumpDisplayHandler(object sender, int pumpIndx);

    public delegate void DelegateIscoPumpDisplaySetpointHandler(object sender, int pumpIndex, double newValue);

    public delegate void DelegateIscoPumpRefreshCompleteHandler();

    public delegate void DelegateIscoPumpInitializationCompleteHandler();

    public delegate void DelegateIscoPumpInitializingHandler();

    public delegate void DelegateIscoPumpOpModeSetHandler(IscoOperationMode newMode);

    public delegate void DelegateIscoPumpControlModeSetHandler(IscoControlMode newMode);

    public delegate void DelegateIscoPumpDisconnected();

    #endregion

}
