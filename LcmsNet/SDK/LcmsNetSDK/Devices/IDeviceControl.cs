/*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 06/30/2009
//
// Last modified 06/30/2009
 *      BLL - 9-8-09
 *          Added the Name property to the list
 *      BLL - 9-14-09
 *          Added XML settings file save and load methods.
/*********************************************************************************************************/

using System;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Describes the interface for user control's that interface a given hardware device or object.
    /// </summary>
    public interface IDeviceControl
    {
        #region "Events"

        event DelegateNameChanged NameChanged;
        event DelegateSaveRequired SaveRequired;

        #endregion

        #region "Properties"

        /// <summary>
        /// Indicates control state
        /// </summary>
        bool Running { get; set; }

        /// <summary>
        /// Gets the device to be controlled that's associated with this interface
        /// </summary>
        IDevice Device { get; set; }

        /// <summary>
        /// Gets or sets the name of the device control.
        /// </summary>
        string Name { get; set; }

        #endregion
    }
}