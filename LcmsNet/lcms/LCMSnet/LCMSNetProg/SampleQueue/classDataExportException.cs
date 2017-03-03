//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/11/2009
//
// Last modified 03/11/2009
//
//*********************************************************************************************************

using System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Custom exception for reporting problems during data export
    /// </summary>
    class classDataExportException : Exception
    {
        public classDataExportException(string message, Exception Ex) :
            base(message, Ex)
        {
        }
    }
}