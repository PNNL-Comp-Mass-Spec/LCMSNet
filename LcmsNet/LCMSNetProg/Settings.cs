//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 08/20/2010
//
//*********************************************************************************************************

using System.ComponentModel;
using System.Reflection;

namespace LcmsNet.Properties
{
    /// <summary>
    /// Modifies framework settings behavior to prevent loss of user settings when app revision is changed.
    /// </summary>
    /// <remarks>Original code developed by Nathan Trimble, PNNL</remarks>
    internal sealed partial class Settings
    {

        #region "Constructors"

        public Settings()
        {
            PropertyChanged += Settings_PropertyChanged;

            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (applicationVersion != appVersion.ToString())
            {
                Upgrade(); // Copies previous version's user settings to current version's user settings
                applicationVersion = appVersion.ToString();
            }
        }

        #endregion

        #region "EventHandlers"

        /// <summary>
        /// Handler for PropertyChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        #endregion
    }
}