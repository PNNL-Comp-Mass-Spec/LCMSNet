
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

namespace LcmsNetDmsTools
{
	public class classDatabaseStoredProcException : Exception
	{
		//*********************************************************************************************************
		// Custom exception for reporting errors during stored procedure execution
		//**********************************************************************************************************

		#region "Class variables"
			int mint_ReturnCode = -1;
			string mstring_ProcName = "";
			string mstring_ErrMessage = "";
		#endregion

		#region "Properties"
			/// <summary>
			/// Stored procedure return code
			/// </summary>
			public int ReturnCode
			{
				get
				{
					return mint_ReturnCode;
				}
				set
				{
					mint_ReturnCode = value;
				}
			}	// End property

			/// <summary>
			/// Name of stored procedure that returned error
			/// </summary>
			public string ProcName
			{
				get
				{
					return mstring_ProcName;
				}
				set
				{
					mstring_ProcName = value;
				}
			}	// End property

			/// <summary>
			/// Error message returned by stored procedure
			/// </summary>
			public string ErrMessage
			{
				get
				{
					return mstring_ErrMessage;
				}
				set
				{
					mstring_ErrMessage = value;
				}
			}	// End property
		#endregion

		#region "Methods"
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="SPName">Name of stored procedure that returned error</param>
			/// <param name="RetCode">Stored procedure return code</param>
			/// <param name="ErrMsg">Error message returned by stored procedure</param>
			public classDatabaseStoredProcException(string SPName, int RetCode, string ErrMsg)
			{
				mstring_ProcName = SPName;
				mint_ReturnCode = RetCode;
				mstring_ErrMessage = ErrMsg;
			}	
		#endregion

	}	
}	// End namespace
