//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/04/2010
//
//*********************************************************************************************************

using System;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Custom exception for database logging problems
    /// </summary>
    class classDbLoggerException : Exception
    {
        #region "Constructors"

        public classDbLoggerException(string message, Exception ex) :
            base(message, ex)
        {
        }

        #endregion
    }
}