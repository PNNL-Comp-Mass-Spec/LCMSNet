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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.SampleQueue
{
    class classDataExportException : Exception
    {
        //*********************************************************************************************************
        // Custom exception for reporting problems during data export
        //**********************************************************************************************************
        public classDataExportException(string message, Exception Ex) :
            base(message, Ex)
        {
        }
    }
} // End namespace