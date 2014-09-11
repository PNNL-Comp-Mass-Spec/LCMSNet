
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/08/2010
//
// Last modified 02/08/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace LogViewer
{
	class classViewerSqlTools
	{
		//*********************************************************************************************************
		// Class for handling SQLite database file access
		//**********************************************************************************************************

		#region "Constants"
		#endregion

		#region "Class variables"
			string mstring_DataFileNamePath;
			string mstring_ConnStr = "";
		#endregion

		#region "Delegates"
		#endregion

		#region "Events"
		#endregion

		#region "Properties"
			public string DataFileNamePath
			{
				get { return mstring_DataFileNamePath; }
				set
				{
					mstring_DataFileNamePath = value;
					mstring_ConnStr = "data source=" + mstring_DataFileNamePath;
				}
			}

			public string ConnString
			{
				get { return mstring_ConnStr; }
			}
		#endregion

		#region "Constructors"
		#endregion

		#region "Methods"
			/// <summary>
			/// Builds a SELECT statement based on data passed from the UI
			/// </summary>
			/// <param name="queryData">Parameters to be used in building query</param>
			/// <returns>SELECT statement as a string</returns>
			private string BuildSelectString(classLogQueryData queryData)
			{
				bool andNeeded = false;

				StringBuilder sqlBldr = new StringBuilder("SELECT * FROM T_LogData");
				
				// Check to see if there are any parameters to add to a WHERE clause
				if (!queryData.OneParamHasValue()) { return sqlBldr.ToString(); }

				// There are one or more parameters for a WHERE clauss, so start adding to the string
				sqlBldr.Append(" WHERE ");

				// Start time
				if (queryData.StartTime.Length > 0)
				{
					sqlBldr.Append("Date >= '" + queryData.StartTime + "'");
					andNeeded = true;
				}

				// Stop time
				if (queryData.StopTime.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Date <= '" + queryData.StopTime + "'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Date <= '" + queryData.StopTime + "'");
						andNeeded = true;
					}
				}

				// Entry type
				if (queryData.Type.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Type LIKE '%" + queryData.Type + "%'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Type LIKE '" + queryData.Type + "%'");
						andNeeded = true;
					}
				}

				// Sample name
				if (queryData.Sample.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Sample LIKE '%" + queryData.Sample + "%'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Sample LIKE '" + queryData.Sample + "%'");
						andNeeded = true;
					}
				}

				// Column
				if (queryData.Column.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Column LIKE '%" + queryData.Column + "%'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Column LIKE '" + queryData.Column + "%'");
						andNeeded = true;
					}
				}

				// Device
				if (queryData.Device.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Device LIKE '%" + queryData.Device + "%'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Device LIKE '" + queryData.Device + "%'");
						andNeeded = true;
					}
				}

				// Message
				if (queryData.Message.Length > 0)
				{
					if (andNeeded)
					{
						sqlBldr.Append(" AND Message LIKE '%" + queryData.Message + "%'");
						andNeeded = true;
					}
					else
					{
						sqlBldr.Append("Message LIKE '" + queryData.Message + "%'");
						andNeeded = true;
					}
				}

				// Return the completed statement
				return sqlBldr.ToString();
			}	

			/// <summary>
			/// Gets all the log entries in the db file
			/// </summary>
			/// <returns>Data table containing log entries</returns>
			public DataTable GetLogEntries(classLogQueryData queryData)
			{
				if (mstring_ConnStr == "")
				{ throw new classLogViewerDataException("Connection string not specified", new Exception()); }

				string sqlStr = BuildSelectString(queryData);

				try
				{
					return GetSQLiteDataTable(sqlStr, mstring_ConnStr);
				}
				catch (Exception ex)
				{
					throw new classLogViewerDataException("Exception getting log data", ex);
				}
			}	

			/// <summary>
			/// Gets the datatable resulting from an SQL query
			/// </summary>
			/// <param name="sqlStr">SQL command string</param>
			/// <param name="connStr">DB connection string</param>
			/// <returns></returns>
			private DataTable GetSQLiteDataTable(string sqlStr, string connStr)
			{
				DataTable returnTable = new DataTable();
				int FilledRows = 0;
				using (SQLiteConnection Cn = new SQLiteConnection(connStr))
				{
					using (SQLiteDataAdapter Da = new SQLiteDataAdapter())
					{
						using (SQLiteCommand Cmd = new SQLiteCommand(sqlStr, Cn))
						{
							Cmd.CommandType = CommandType.Text;
							Da.SelectCommand = Cmd;
							try
							{
								FilledRows = Da.Fill(returnTable);
							}
							catch (Exception Ex)
							{
								string ErrMsg = "SQLite exception getting data table via query " + sqlStr;
								throw new classLogViewerDataException(ErrMsg, Ex);
							}
						}
					}
				}
				// Everything worked, so return the table
				return returnTable;
			}	
		#endregion
	}	
}	// End namespace
