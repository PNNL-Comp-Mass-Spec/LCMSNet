//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/27/2009
//
// Updates:
// - 02/19/2009 (DAC) - Renamed for use with both DMS and SQLite databases
//
//*********************************************************************************************************

using System;

namespace LcmsNetSQLiteTools
{
    /// <summary>
    ///  Custom exception for reporting problems during a database query
    /// </summary>
    public class DatabaseDataException : Exception
    {
        public DatabaseDataException(string message, Exception ex) :
            base(message, ex)
        {
        }
    }
}
