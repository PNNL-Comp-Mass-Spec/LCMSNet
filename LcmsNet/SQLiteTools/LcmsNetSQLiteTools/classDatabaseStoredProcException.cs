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
            m_ProcName = SPName;
            m_ReturnCode = RetCode;
            m_ErrMessage = ErrMsg;
        }

        #endregion

        //*********************************************************************************************************
        // Custom exception for reporting errors during stored procedure execution
        //**********************************************************************************************************

        #region "Class variables"

        int m_ReturnCode = -1;
        string m_ProcName = "";
        string m_ErrMessage = "";

        #endregion

        #region "Properties"

        /// <summary>
        /// Stored procedure return code
        /// </summary>
        public int ReturnCode
        {
            get { return m_ReturnCode; }
            set { m_ReturnCode = value; }
        } // End property

        /// <summary>
        /// Name of stored procedure that returned error
        /// </summary>
        public string ProcName
        {
            get { return m_ProcName; }
            set { m_ProcName = value; }
        } // End property

        /// <summary>
        /// Error message returned by stored procedure
        /// </summary>
        public string ErrMessage
        {
            get { return m_ErrMessage; }
            set { m_ErrMessage = value; }
        } // End property

        #endregion
    }
} // End namespace