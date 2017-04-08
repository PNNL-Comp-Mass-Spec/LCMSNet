//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
//*********************************************************************************************************

using System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Thrown if a sample is cancelled but is not running.
    /// </summary>
    public class classSampleNotRunningException : Exception
    {
        /// <summary>
        /// Constructor to elaborate on why the exception was thrown.
        /// </summary>
        /// <param name="message"></param>
        public classSampleNotRunningException(string message) :
            base(message)
        {
        }
    }
}