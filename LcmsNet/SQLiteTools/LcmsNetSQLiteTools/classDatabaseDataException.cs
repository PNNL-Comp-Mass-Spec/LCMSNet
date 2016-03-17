//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/27/2009
//
// Last modified 01/27/2009
//						02/19/2009 (DAC) - Renamed for use with both DMS and SQLite databases
//
//*********************************************************************************************************

using System;

namespace LcmsNetSQLiteTools
{
    public class classDatabaseDataException : Exception
    {
        //*********************************************************************************************************
        // Custom exception for reporting problems during a database query
        //**********************************************************************************************************
        public classDatabaseDataException(string message, Exception Ex) :
            base(message, Ex)
        {
        }
    }
} // End namespace