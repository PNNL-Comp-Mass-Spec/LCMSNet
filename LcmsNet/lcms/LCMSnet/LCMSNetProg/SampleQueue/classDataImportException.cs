//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 04/14/2009
//
// Last modified 04/14/2009
//*********************************************************************************************************

using System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Custom exception for reporting problems during data import
    /// </summary>
    class classDataImportException : Exception
    {
        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message to accompany exception</param>
        /// <param name="Ex">Exception to report</param>
        public classDataImportException(string message, Exception Ex) :
            base(message, Ex)
        {
        }

        #endregion

        #region "Constants"

        #endregion

        #region "Class variables"

        #endregion

        #region "Events"

        #endregion

        #region "Properties"

        #endregion
    }
}