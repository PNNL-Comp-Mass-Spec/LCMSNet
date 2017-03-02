
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 06/30/2009
//
// Last modified 06/30/2009
//                      09/03/2009 (DAC) - Added group name property for saving/loading config
//                      09/18/2009 (DAC) - Added SaveSymbolSettings method for saving config
//                      12/02/2009 (DAC) - Added event for handling PAL tray list
//                      02/16/2010 (DAC) - Added Dispose method for event handler cleanup
//*********************************************************************************************************
using System;
using Syncfusion.Windows.Forms.Diagram;

using LcmsNetDataClasses.Devices;

namespace LcmsNet.FluidicsDesigner
{
    interface IDeviceSymbol
    {
        //*********************************************************************************************************
        // Defines a device to be added to the designer
        //**********************************************************************************************************

        #region "Events"
        event DelegateOperationError    OperationError;
        event DelegateSaveRequired      SaveRequired;
        #endregion

        #region "Properties"
        /// <summary>
        /// The drawing group that's created for adding to the graphical display
        /// </summary>
        Group DwgGroup { get; }

        /// <summary>
        /// The device associated with this cymbol class
        /// </summary>
        IDeviceControl Device { get; }

        /// <summary>
        /// The name Syncfusion creates for this symbol
        /// </summary>
        string GroupName { get; set; }
        #endregion

        #region "Methods"
        void ShowProps();
        void OnNameChange(object Sender, string NewName);
        void SaveSymbolSettings(string parentName);
        void Dispose();
        #endregion
    }
}
