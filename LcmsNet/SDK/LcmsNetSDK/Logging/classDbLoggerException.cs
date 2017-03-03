//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/04/2010
//
// Last modified 02/04/2010
//*********************************************************************************************************

using System;

namespace LcmsNetDataClasses.Logging
{
    class classDbLoggerException : Exception
    {
        //*********************************************************************************************************
        // Custom exception for database logging problems
        //**********************************************************************************************************

        #region "Constructors"

        public classDbLoggerException(string message, Exception ex) :
            base(message, ex)
        {
        }

        #endregion
    }
}