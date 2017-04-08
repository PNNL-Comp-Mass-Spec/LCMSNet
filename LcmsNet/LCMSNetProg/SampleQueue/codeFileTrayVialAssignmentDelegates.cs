//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 08/03/2010
//
//*********************************************************************************************************

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Delegate for use with Tray/Vial assignment form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="trayNumber"></param>
    public delegate void DelegateRowModified(object sender, int trayNumber);
}