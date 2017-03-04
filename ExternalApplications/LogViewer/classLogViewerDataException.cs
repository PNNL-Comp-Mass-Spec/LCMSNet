//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/08/2010
//
//*********************************************************************************************************
using System;

namespace LogViewer
{
    /// <summary>
    /// Log viewer custom exception class
    /// </summary>
    class classLogViewerDataException : Exception
    {
        #region "Constructors"
            public classLogViewerDataException(string message, Exception ex) :
                base(message,ex)
            {
            }
        #endregion
    }   
}
