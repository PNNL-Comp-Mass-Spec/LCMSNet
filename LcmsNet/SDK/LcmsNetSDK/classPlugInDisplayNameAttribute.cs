//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/03/2009
//
// Last modified 03/03/2009
//*********************************************************************************************************

using System;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Custom attribute class for display of plugin name
    /// </summary>
    public class classPlugInDisplayNameAttribute : Attribute
    {
        #region "Class variables"

        readonly string m_DisplayName;

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DisplayName">Plugin name to be used for display purposes</param>
        public classPlugInDisplayNameAttribute(string DisplayName)
        {
            m_DisplayName = DisplayName;
        }

        /// <summary>
        /// Overrides base class ToString method
        /// </summary>
        /// <returns>String representing display name</returns>
        public override string ToString()
        {
            return m_DisplayName;
        }

        #endregion
    }
}