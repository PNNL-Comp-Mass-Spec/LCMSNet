//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/28/2009
//
// Last modified 01/28/2009
//						02/19/2009 (DAC) - Renamed for use with both DMS and SQLite databases
//
//*********************************************************************************************************

using System;

namespace LcmsNetSQLiteTools
{
    public class classDatabaseStoredProcException : Exception
    {
        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SPName">Name of stored procedure that returned error</param>
        /// <param name="RetCode">Stored procedure return code</param>
        /// <param name="ErrMsg">Error message returned by stored procedure</param>
        public classDatabaseStoredProcException(string SPName, int RetCode, string ErrMsg)
        {
            ProcName = SPName;
            ReturnCode = RetCode;
            ErrMessage = ErrMsg;
        }

        #endregion

        //*********************************************************************************************************
        // Custom exception for reporting errors during stored procedure execution
        //**********************************************************************************************************

        #region "Class variables"

        #endregion

        #region "Properties"

        /// <summary>
        /// Stored procedure return code
        /// </summary>
        public int ReturnCode { get; set; } = -1;

        // End property

        /// <summary>
        /// Name of stored procedure that returned error
        /// </summary>
        public string ProcName { get; set; } = "";

        // End property

        /// <summary>
        /// Error message returned by stored procedure
        /// </summary>
        public string ErrMessage { get; set; } = "";

        // End property

        #endregion
    }
} // End namespace