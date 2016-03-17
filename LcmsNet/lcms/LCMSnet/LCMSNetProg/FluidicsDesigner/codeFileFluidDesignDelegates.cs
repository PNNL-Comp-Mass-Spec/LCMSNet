
//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 06/30/2009
//
// Last modified 06/30/2009
//                      12/02/2009 (DAC) - Added delegate for handling PAL tray list
//                      12/11/2009 (DAC) - Added delegate for handling Save Required event from devices
//*********************************************************************************************************

using LcmsNetDataClasses;

namespace LcmsNet.FluidicsDesigner
{
    //*********************************************************************************************************
    // Delegates for use with Fluidics Designer
    //**********************************************************************************************************

    public delegate void DelegateOperationError(string message);
    public delegate void DelegateSaveRequired(LcmsNetDataClasses.Devices.IDeviceControl device);
}   // End namespace