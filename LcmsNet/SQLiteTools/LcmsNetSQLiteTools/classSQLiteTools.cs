
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/10/2009
//
// Last modified 02/10/2009
//						02/12/2009 (DAC) - Added methods for retrieving cached queue
//						02/19/2009 (DAC) - Incorporated renamed exceptions
//						02/23/2009 (DAC) - Reworked queue saving to reduce future coding
//						02/24/2009 (DAC) - Added storage and retrieval of DMS parameters
//						03/03/2009 (DAC) - Modified constructor to fix form designer issue, added method overloads
//													for queue ops to specify a database file other than the cache file
//						03/10/2009 (DAC) - Added function to replace SQLite-incompatible characters
//						04/01/2009 (DAC) - Added file logging for exceptions
//						05/18/2010 (DAC) - Added error logging; Modified for queue import/export using SQLite
//						04/17/2013 (FCT) - Added Proposal Users list with a a cross reference list of their UID to the PIDs of proposals they've worked.
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Reflection;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Data;


namespace LcmsNetSQLiteTools
{
	public class classSQLiteTools
	{
		#region Class Variables
		private static string mstring_connectionString = "";
        //private static string mstring_errroString = "";

		private static List<string>			m_cartNames;
		private static List<string>			m_columnNames;
		private static List<string>			m_separationNames;
		private static List<string>			m_datasetNames;

		private static List<classUserInfo>			m_userInfo;
		private static List<classInstrumentInfo>	m_instrumentInfo;
		private static List<classExperimentData>	m_experimentsData;
		private static List<classLCColumn>			m_lcColumns;

		private static List<classProposalUser>										m_proposalUsers;
		private static Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>	m_pidIndexedReferenceList;
		#endregion

		#region Initialize
		/// <summary>
		/// Constructor
		/// </summary>
		static classSQLiteTools()
		{
            AppDataFolderName   = "LCMSNet";
		    CacheName           = "LCMSCache.que";

			BuildConnectionString(false);
		}
        
	    public static string CacheName { get; set; }
        public static string AppDataFolderName { get; set; }

		public static void BuildConnectionString(bool newCache)
		{
			try
			{
				string name = classLCMSSettings.GetParameter("CacheFileName");
				bool exists = File.Exists(name);
				if (!exists && !newCache)
				{
                    
                    string appPath      = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                    name                = Path.Combine(appPath, AppDataFolderName);
                                        
                    if (!Directory.Exists(name))
                    {
                        Directory.CreateDirectory(name);
                    }
                    name = Path.Combine(name, CacheName);
					classLCMSSettings.SetParameter("CacheFileName", name);
				}
                //workaround for SQLite library version 1.0.93 for network addresses
                if(name.Substring(0,1) == "\\")
                {
                    name = "\\" + name;
                }
				mstring_connectionString = "data source=" + name;
			}
			catch (Exception ex)
			{
				classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Could not load the sample queue cache.", ex);
			}
		}
		#endregion

		#region Properties

		public static string ConnString
		{
			get { return mstring_connectionString; }
			set { mstring_connectionString = value; }
		}	// End property
		#endregion

		#region Methods
		/// <summary>
		/// Sets the cache location to the path provided.  
		/// </summary>
		/// <param name="location">New path to location of queue.</param>
		public static void SetCacheLocation(string location)
		{
		    classLCMSSettings.SetParameter("CacheFileName", location);
            
            BuildConnectionString(!File.Exists(location));
		    //mstring_connectionString = "data source=" + location;
		}
		
		/// <summary>
		/// Retrieves a sample queue from cache database
		/// Connection string and database name are defined by defaults
		/// </summary>
		/// <param name="TableType">TableType enum specifying type of queue to retrieve</param>
		/// <returns>List<classSampleData> containing queue data</returns>
		public static List<classSampleData> GetQueueFromCache(enumTableTypes TableType)
		{
			return GetQueueFromCache(TableType, mstring_connectionString);
		}	
		
		/// <summary>
		/// Retrieves a sample queue from a SQLite database
		/// Overload requires connection string to be specified
		/// </summary>
		/// <param name="TableType">TableType enum specifying type of queue to retrieve</param>
		/// <param name="ConnStr">Cache connection string</param>
		/// <returns>List<classSampleData> containing queue data</returns>
		public static List<classSampleData> GetQueueFromCache(enumTableTypes tableType, string connectionString)
		{
			List<classSampleData> returnData = new List<classSampleData>();
		
			// Convert type of queue into a data table name
			string tableName = GetTableName(tableType);
		
			// Get a list of string dictionaries containing properties for each sample
			List<StringDictionary> allSampleProps = GetPropertiesFromCache(tableName, connectionString);
		
			// For each row (representing one sample), create a sample data object 
			//		and load the property values
			foreach (StringDictionary sampleProps in allSampleProps)
			{
				// Create a classSampleData object
				classSampleData sampleData = new classSampleData();
		
				// Load the sample data object from the string dictionary
				sampleData.LoadPropertyValues(sampleProps);
		
				// Add the sample data object to the return list
				returnData.Add(sampleData);
			}
		
			// All finished, so return
			return returnData;
		}	
		
		/// <summary>
		/// Gets a list of string dictionary objects containing properties for each item in the cache
		/// </summary>
		/// <param name="tableName">Name of table containing the properties</param>
		/// <returns>List&lt;StringDictionary&gt; with properties for each item in cache</returns>
		private static List<StringDictionary> GetPropertiesFromCache(string tableName, string ConnStr)
		{
			List<StringDictionary> returnData = new List<StringDictionary>();
		
			// Verify table exists in database
			if (!VerifyTableExists(tableName, ConnStr))
			{
				// No table, so return an empty list
				return returnData;
			}
		
			// Get table containing cached data
			string sqlStr = "SELECT * FROM " + tableName;
			DataTable cacheData = GetSQLiteDataTable(sqlStr,ConnStr);
			if (cacheData.Rows.Count < 1)
			{
				// No cahced data found, so return an empty list
				return returnData;
			}
		
			// For each row (representing properties and values for one sample), create a string dictionary
			//			with the object's properties from the table columns, and add it to the return list
			foreach (DataRow currentRow in cacheData.Rows)
			{
				// Create a string dictionary containing the properties and values for this sample
				StringDictionary sampleProps = GetPropertyDictionaryForSample(currentRow, cacheData.Columns);
				// Add the string dictionary to the return list
				returnData.Add(sampleProps);
			}
		
			// Return the list
			return returnData;
		}	
		
		/// <summary>
		/// Gets a string dictionary containing property names and values contained in
		///	a row of the cache data table
		/// </summary>
		/// <param name="RowOfValues">DataRow containing property values from table</param>
		/// <param name="TableColumns">Collection of data columns in table</param>
		/// <returns>StringDictionary in <property name, property value> format</returns>
		private static StringDictionary GetPropertyDictionaryForSample(DataRow RowOfValues, DataColumnCollection TableColumns)
		{
			StringDictionary returnDict = new StringDictionary();
			string colName;
			string colData;
		
			// Build the string dictionary
			foreach (DataColumn column in TableColumns)
			{
				colName = column.ColumnName;
				colData = (string)RowOfValues[TableColumns[colName]];
				returnDict.Add(colName, colData);
			}
			
			// Return the dictionary
			return returnDict;
		}	
		
		/// <summary>
		/// Determines if a particular table exists in the SQLite database
		/// </summary>
		/// <param name="TableName">Name of the table to search for</param>
		/// <param name="ConnStr">Connection string for database</param>
		/// <returns>TRUE if table found; FALSE if not found or error</returns>
		private static bool VerifyTableExists(string TableName, string ConnStr)
		{
			string sqlString = "SELECT * FROM sqlite_master WHERE name ='" + TableName + "'";
			DataTable tableList = new DataTable();
			try
			{
				// Get a list of database tables matching the specified table name
				tableList = GetSQLiteDataTable(sqlString, ConnStr);
			}
			catch (Exception Ex)
			{
				string ErrMsg = "SQLite exception verifying table " + TableName + " exists";
				// throw new classDatabaseDataException(ErrMsg, Ex);
				classApplicationLogger.LogError(0, ErrMsg, Ex);
				return false;
			}
		
			// If exactly 1 row returned, then table exists
			if (tableList.Rows.Count == 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}	
		
		/// <summary>
		/// Saves a list of properties for an object to the cache database
		/// </summary>
		/// <param name="dataToCache">List<ICacheInterface> ojects to save properites for</param>
		/// <param name="tableName">Name of the table to save data in</param>
		private static void SavePropertiesToCache(List<ICacheInterface> dataToCache, string tableName, 
																		string ConnStr)
		{
			bool dataExists = (dataToCache.Count > 0);
			string sqlCmd;
		
			// If there is no data, then just exit
			if (!dataExists)
			{
				return;
			}
		
			// Create a string dictionary holding the property names and values for object,
			//		using the first object in the input list
			ICacheInterface firstItem = dataToCache[0];
			StringDictionary FieldNames = firstItem.GetPropertyValues();
		
			// Verify table exists; if not, create it; Otherwise, clear it
			if (!VerifyTableExists(tableName, ConnStr))
			{
				// Table doesn't exist, so create it
				sqlCmd = BuildCreatePropTableCmd(FieldNames, tableName);
				try
				{
					ExecuteSQLiteCommand(sqlCmd, ConnStr);
				}
				catch (Exception Ex)
				{
					string ErrMsg = "SQLite exception creating table " + tableName;
					classApplicationLogger.LogError(0, ErrMsg, Ex);
					return;
				}
			}
		
			// Copy the field data to the data table
			StringDictionary itemProps = new StringDictionary();
			string sqlInsertCmd;
			List<string> cmdList = new List<string>();
			foreach (ICacheInterface tempItem in dataToCache)
			{
				itemProps = tempItem.GetPropertyValues();
				sqlInsertCmd = BuildInsertPropValueCmd(itemProps, tableName);
				cmdList.Add(sqlInsertCmd);
			}
			
			// Execute the list of commands
			try
			{
				ExecuteSQLiteCmdsWithTransaction(cmdList, ConnStr);
			}
			catch (Exception Ex)
			{
				string ErrMsg = "Exception inserting values into table " + tableName;
				// throw new classDatabaseDataException(ErrMsg, Ex);
				classApplicationLogger.LogError(0, ErrMsg, Ex);
				return;
			}
		}	
		
		/// <summary>
		/// Saves the contents of specified sample queue to the SQLite cache file
		/// Connection string and database name are defined by defaults 
		/// </summary>
		/// <param name="QueueData">List<classSampleData> containing the sample data to save</param>
		/// <param name="TableType">TableTypes enum specifying which queue is being saved</param>
		public static void SaveQueueToCache(List<classSampleData> QueueData, enumTableTypes TableType)
		{
			SaveQueueToCache(QueueData, TableType, mstring_connectionString);
		}	
		
		/// <summary>
		/// Saves the contents of specified sample queue to an SQLite database file
		/// Overload requires database connection string be specified
		/// </summary>
		/// <param name="QueueData">List&lt;classSampleData&gt; containing the sample data to save</param>
		/// <param name="TableType">TableTypes enum specifying which queue is being saved</param>
		/// <param name="ConnStr">Connection string for database file</param>
		public static void SaveQueueToCache(List<classSampleData> QueueData, enumTableTypes TableType,
															string ConnStr)
		{
			bool DataInList = (QueueData.Count > 0);
			string tableName = GetTableName(TableType);
		
			// Clear the cache table
			ClearCacheTable(tableName, ConnStr);
		
			//If no data in list, just exit
			if (!DataInList)
			{
				return;
			}
		
			// Convert input data for caching and call cache routine
			List<ICacheInterface> dataList = new List<ICacheInterface>();
			foreach (classSampleData currentSample in QueueData)
			{
				dataList.Add(currentSample);
			}
			SavePropertiesToCache(dataList,tableName, ConnStr);
		}	
		
		/// <summary>
		/// Saves a list of users to cache
		/// </summary>
		/// <param name="UserList">List&lt;classUserInfo&gt; containing user data</param>
		public static void SaveUserListToCache(List<classUserInfo> UserList)
		{
			bool dataInList = (UserList.Count > 0);
			string tableName = GetTableName(enumTableTypes.UserList);
		
			// Clear the cache table
			ClearCacheTable(tableName, mstring_connectionString);
		
			m_userInfo = new List<classUserInfo>(UserList);
			//If no data in list, exit
			if (!dataInList)
			{
				return;
			}
		
			// Convert input data for caching and call cache routine
			List<ICacheInterface> dataList = new List<ICacheInterface>();
			foreach (classUserInfo currentUser in UserList)
			{
				dataList.Add(currentUser);      
			}
			SavePropertiesToCache(dataList, tableName, mstring_connectionString);
		}
		
		public static void SaveExperimentListToCache(List<classExperimentData> expList)
		{
            if (expList == null || expList.Count < 1)
                return;

			bool listHasData = expList.Count != 0;
			string tableName = GetTableName(enumTableTypes.ExperimentList);
		
			// Clear the cache table
			ClearCacheTable(tableName, mstring_connectionString);
		
			m_experimentsData = new List<classExperimentData>(expList);
			// Exit if there's nothing to cache
			if (!listHasData)
				return;
		
			// Convert input data for caching and call cache routine
			List<ICacheInterface> dataList = new List<ICacheInterface>();

            if (!VerifyTableExists(tableName, mstring_connectionString))
            {
                
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(mstring_connectionString))
                    {

                        connection.Open();


                        using (SQLiteCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "CREATE TABLE T_ExperimentList ('Created', 'Experiment', 'ID', 'Organism', 'Reason', 'Request', 'Researcher')";
                            command.ExecuteNonQuery();
                        }

                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                
                                foreach (classExperimentData datum in expList)
                                {
                                    string commandText = string.Format("INSERT INTO T_ExperimentList VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                                                                                        datum.ID,
                                                                                        datum.Organism,
                                                                                        datum.Researcher,
                                                                                        datum.Reason == null ? "" : datum.Reason.Replace("'", ""),
                                                                                        datum.Request,
                                                                                        datum.Experiment,
                                                                                        datum.Created.HasValue ? DateTime.MinValue : datum.Created.Value);
                                    //dataList.Add(datum);
                                    command.CommandText = commandText;
                                    command.ExecuteNonQuery();
                                    
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, string.Format("Could not insert all of the experiment data into the experiment table. {0}", ex.Message));
                }
            }		

            //foreach (classExperimentData datum in expList)
            //{
            //    //INSERT INTO T_ExperimentList VALUES('15', 'None', 'Kiebel, Gary R (d3j410)', '', '0', 'Placeholder', '4/19/2000 12:00:00 AM')

            //    dataList.Add(datum);
            //}
		
			//SavePropertiesToCache(dataList, tableName, mstring_connectionString);
		}

		/// <summary>
		/// Saves the Proposal Users list and a Proposal ID to Proposal User ID cross-reference
		/// list to the cache.
		/// </summary>
		/// <param name="users">A list of the Proposal Users to cache.</param>
		/// <param name="crossReferenceList">A list of cross references to cache.</param>
		/// <param name="pidIndexedReferenceList">
		/// A dictionary of cross reference lists that have been grouped by Proposal ID.
		/// </param>
		public static void SaveProposalUsers(List<classProposalUser> users, 
			List<classUserIDPIDCrossReferenceEntry> crossReferenceList, 
			Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
		{
			string userTableName		= GetTableName(enumTableTypes.PUserList);
			string referenceTableName	= GetTableName(enumTableTypes.PReferenceList);
		
			ClearCacheTable(userTableName, mstring_connectionString);
			ClearCacheTable(referenceTableName, mstring_connectionString);
		
			List<ICacheInterface> userCacheList			= new List<ICacheInterface>();
			List<ICacheInterface> referenceCacheList	= new List<ICacheInterface>();
				
			userCacheList.AddRange(users);
			referenceCacheList.AddRange(crossReferenceList);
		
			SavePropertiesToCache(userCacheList, userTableName, mstring_connectionString);

			SavePropertiesToCache(referenceCacheList, referenceTableName, mstring_connectionString);

			m_proposalUsers				= users;
			m_pidIndexedReferenceList	= pidIndexedReferenceList;
		}

		public static void SaveEntireLCColumnListToCache(List<classLCColumn> lcColumnList)
		{
			bool listHasData = lcColumnList.Count != 0;
			string tableName = GetTableName(enumTableTypes.LCColumnList);
		
			// Clear the cache table
			ClearCacheTable(tableName, mstring_connectionString);
		
			m_lcColumns = new List<classLCColumn>(lcColumnList);
			// Exit if there's nothing to cache
			if (!listHasData)
				return;
		
			// Convert input data for caching and call cache routine
			List<ICacheInterface> dataList = new List<ICacheInterface>();
			foreach (classLCColumn datum in lcColumnList)
				dataList.Add(datum);
		
			SavePropertiesToCache(dataList, tableName, mstring_connectionString);
		}
		
		/// <summary>
		/// Saves a list of instruments to cache
		/// </summary>
		/// <param name="UserList">List<classInstrumentInfo> containing instrument data</param>
		public static void SaveInstListToCache(List<classInstrumentInfo> InstList)
		{
			bool dataInList = (InstList.Count > 0);
			string tableName = GetTableName(enumTableTypes.InstrumentList);
		
			// Clear the cache table
			ClearCacheTable(tableName, mstring_connectionString);
		
			m_instrumentInfo = new List<classInstrumentInfo>(InstList);
			//If no data in list, just exit
			if (!dataInList)
			{
				return;
			}
		
			// Convert input data for caching and call cache routine
			List<ICacheInterface> dataList = new List<ICacheInterface>();
			foreach (classInstrumentInfo currentInst in InstList)
			{
				dataList.Add(currentInst);
			}
			SavePropertiesToCache(dataList, tableName, mstring_connectionString);
		}	
		
		/// <summary>
		/// Executes specified SQLite command
		/// </summary>
		/// <param name="CmdStr">SQL statement to execute</param>
		/// <param name="ConnStr">Connection string for SQL database file</param>
		private static void ExecuteSQLiteCommand(string CmdStr, string ConnStr)
		{
			int AffectedRows;
			using (SQLiteConnection Cn = new SQLiteConnection(ConnStr))
			{
				using (SQLiteCommand myCmd = new SQLiteCommand(Cn))
				{
					myCmd.CommandType = System.Data.CommandType.Text;
					myCmd.CommandText = CmdStr;
					try
					{
						myCmd.Connection.Open();
						AffectedRows = myCmd.ExecuteNonQuery();
						return;
					}
					catch (Exception Ex)
					{
						string ErrMsg = "SQLite exception executing command " + CmdStr;
						classApplicationLogger.LogError(0, ErrMsg, Ex);
						throw new classDatabaseDataException(ErrMsg, Ex);
					}
					finally
					{
						myCmd.Connection.Close();
					}
				}
			}
		}	
		
		/// <summary>
		/// Executes a collection of SQL commands wrapped in a transaction to improve performance
		/// </summary>
		/// <param name="CmdList">List&lt;string&gt; containing the commands to execute</param>
		/// <param name="ConnStr">Connection string</param>
		private static void ExecuteSQLiteCmdsWithTransaction(List<string> CmdList, string ConnStr)
		{
			
			using (SQLiteConnection connection = new SQLiteConnection(ConnStr))
			{
                
				using (SQLiteCommand command = new SQLiteCommand(connection))
				{
                    
					command.CommandType = System.Data.CommandType.Text;
					try
					{
						command.Connection.Open();

                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {                            
                            // Turn off journal, which speeds up transaction
                            command.CommandText = "PRAGMA journal_mode = OFF";
                            command.ExecuteNonQuery();

                            
                            // Send each of the commands
                            foreach (string currCmd in CmdList)
                            {
                                command.CommandText = currCmd;
                                command.ExecuteNonQuery();                                
                            }
                            // End transaction                                                        
                            transaction.Commit();
                        }
						return;
					}
					catch (Exception Ex)
					{
						string ErrMsg = "SQLite exception adding data";
						classApplicationLogger.LogError(0, ErrMsg, Ex);
						throw new classDatabaseDataException(ErrMsg, Ex);
					}
					finally
					{
						command.Connection.Close();
					}
				}
			}
		}	
		
		/// <summary>
		/// Retrieves a data table from a SQLite database
		/// </summary>
		/// <param name="CmdStr">SQL command to execute</param>
		/// <param name="ConnStr">Connection string for SQLite database file</param>
		/// <returns>A DataTable containing data specfied by CmdStr</returns>
		private static DataTable GetSQLiteDataTable(string CmdStr, string ConnStr)
		{
			DataTable returnTable = new DataTable();
			int FilledRows = 0;
			using (SQLiteConnection Cn = new SQLiteConnection(ConnStr))
			{
				using (SQLiteDataAdapter Da = new SQLiteDataAdapter())
				{
					using (SQLiteCommand Cmd = new SQLiteCommand(CmdStr, Cn))
					{
						Cmd.CommandType = CommandType.Text;
						Da.SelectCommand = Cmd;

                        
						try
						{
							FilledRows = Da.Fill(returnTable);
						}
						catch (Exception Ex)
						{
							string ErrMsg = "SQLite exception getting data table via query " + CmdStr + " : " + ConnStr;
							classApplicationLogger.LogError(0, ErrMsg, Ex);
							throw new classDatabaseDataException(ErrMsg, Ex);
						}
					}
				}
			}
			// Everything worked, so return the table
			return returnTable;
		}	
		
		/// <summary>
		/// Replaces characters in a string that are incompatible with SQLite 
		/// </summary>
		/// <param name="InpString">String to clean</param>
		/// <returns>String compatible with SQLite</returns>
		private static string ScrubField(string InpString)
		{
			// Check for empty string
			if (InpString == "")
			{
				return InpString;
			}
		
			// Escape single quotes
			return InpString.Replace("'", "''");
		}	
		
		/// <summary>
		/// Builds a INSERT command from the input string dictionary
		/// </summary>
		/// <param name="InpData">String dictionary containing property names and values</param>
		/// <param name="TableName">Name of table to insert values into</param>
		/// <returns>String consisting of a complete INSERT SQL statement</returns>
		private static string BuildInsertPropValueCmd(StringDictionary InpData, string TableName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("INSERT INTO ");
			sb.Append(TableName + " VALUES(");
			// Add the property values to the string
			foreach (string colName in InpData.Keys)
			{
				sb.Append("'" + ScrubField(InpData[colName]) + "', ");
			}
			// Remove the last "', "
			sb.Remove(sb.Length - 2, 2);
			// Terminate the string and return
			sb.Append(")");
			return sb.ToString();
		}	
		
		/// <summary>
		/// Builds a CREATE TABLE command from the input string dictionary
		/// </summary>
		/// <param name="InpData">String dictionary containing property names and values</param>
		/// <param name="TableName">Name of table to create</param>
		/// <returns>String consisting of a complete CREATE TABLE SQL statement</returns>
		private static string BuildCreatePropTableCmd(StringDictionary InpData, string TableName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CREATE TABLE ");
			sb.Append(TableName + "(");
			// Create column names for each key, which is same as property name in queue being saved
			foreach (string colName in InpData.Keys)
			{
				sb.Append("'" + colName + "', ");
			}
			// Remove the last "', "
			sb.Remove(sb.Length - 2, 2);
			// Terminate the string and return
			sb.Append(")");
			return sb.ToString();
		}	
		
		/// <summary>
		/// Converts a type of table to the corresponding cache db table name
		/// </summary>
		/// <param name="TypeOfQueue">enumTableTypes specifying table to get name for</param>
		/// <returns>Name of db table</returns>
		private static string GetTableName(enumTableTypes TableType)
		{
			return "T_" + (string)Enum.GetName(typeof(enumTableTypes), TableType);
		}

		/// <summary>
		/// Wrapper around generic retrieval method specifically for cart lists
		/// </summary>
		/// <returns>List&lt;string&gt; containing cart names</returns>
		public static List<string> GetCartNameList(bool force)
		{
		    if (m_cartNames == null)
		    {
		        m_cartNames = GetSingleColumnListFromCache(enumTableTypes.CartList);
		    }
		    return m_cartNames;
		}	
		/// <summary>
		/// Wrapper around generic retrieval method specifically for LC column lists
		/// </summary>
		/// <returns>List&lt;string&gt; containing cart names</returns>
		public static List<string> GetColumnList(bool force)
		{
		    if (m_columnNames == null || force)
		    {
		        m_columnNames = GetSingleColumnListFromCache(enumTableTypes.ColumnList);
		    }
		    return m_columnNames;
		}	
		/// <summary>
		/// Wrapper around generic retrieval method specifically for separation type lists
		/// </summary>
		/// <returns>List&lt;string&gt;containing separation types</returns>
		public static List<string> GetSepTypeList(bool force)
		{
		    if (m_separationNames == null)
		    {
		        m_separationNames = GetSingleColumnListFromCache(enumTableTypes.SeparationTypeList);
		    }
			return m_separationNames;
		}	
		/// <summary>
		/// Wrapper around generic retrieval method specifically for dataset type lists
		/// </summary>
		/// <returns>List<string> containing dataset types</returns>
		public static List<string> GetDatasetTypeList(bool force)
		{
		    if (m_datasetNames == null)
		    {
		        m_datasetNames = GetSingleColumnListFromCache(enumTableTypes.DatasetTypeList);
		    }
		    return m_datasetNames;
		}	
		/// <summary>
		/// Gets user list from cache
		/// </summary>
		/// <returns>List&lt;classUserInfo&gt; of user data</returns>
		public static List<classUserInfo> GetUserList(bool force)
		{
		    if (m_userInfo == null || force)
		    {
		        List<classUserInfo> returnData = new List<classUserInfo>();
		
		        // Get data table name
		        string tableName = GetTableName(enumTableTypes.UserList);
		
		        // Get a list of string dictionaries containing properties for each item
		        List<StringDictionary> allUserProps = new List<StringDictionary>();
		        allUserProps = GetPropertiesFromCache(tableName, mstring_connectionString);
		
		        // For each row (representing one user), create a user data object 
		        //		and load the property values
		        foreach (StringDictionary userProps in allUserProps)
		        {
		            // Create a classUserInfo object
		            classUserInfo userData = new classUserInfo();
		
		            // Load the user data object from the string dictionary
		            userData.LoadPropertyValues(userProps);
		
		            // Add the user data object to the return list
		            returnData.Add(userData);
		        }
		        m_userInfo = returnData;
		    }
			// All finished, so return
			return m_userInfo;
		}	
		/// <summary>
		/// Gets a list of instruments from the cache
		/// </summary>
		/// <returns>List&lt;classInstrumentInfo&gt; of instruments</returns>
		public static List<classInstrumentInfo> GetInstrumentList(bool force)
		{
		    if (m_instrumentInfo == null)
		    {
		        List<classInstrumentInfo> returnData = new List<classInstrumentInfo>();
		
		        // Convert type of list into a data table name
		        string tableName = GetTableName(enumTableTypes.InstrumentList);
		
		        // Get a list of string dictionaries containing properties for each instrument
		        List<StringDictionary> allInstProps = new List<StringDictionary>();
		        allInstProps = GetPropertiesFromCache(tableName, mstring_connectionString);
		
		        // For each row (representing one instrument), create an instrument data object 
		        //		and load the property values
		        foreach (StringDictionary instProps in allInstProps)
		        {
		            // Create a classInstrumentInfo object
		            classInstrumentInfo instData = new classInstrumentInfo();
		
		            // Load the instrument data object from the string dictionary
		            instData.LoadPropertyValues(instProps);
		
		            // Add the instrument data object to the return list
		            returnData.Add(instData);
		        }
		
		        // All finished, so return
		        m_instrumentInfo = returnData;
		    }
		    return m_instrumentInfo;
		}
		
		public static List<classExperimentData> GetExperimentList()
		{
			if (m_experimentsData == null)
			{
				List<classExperimentData> returnData = new List<classExperimentData>();
		
				string tableName = GetTableName(enumTableTypes.ExperimentList);
		
				List<StringDictionary> allExpProperties = 
					GetPropertiesFromCache(tableName, mstring_connectionString);
		
				foreach (StringDictionary props in allExpProperties)
				{
					classExperimentData expDatum = new classExperimentData();
		
					expDatum.LoadPropertyValues(props);
		
					returnData.Add(expDatum);
				}
		
				m_experimentsData = returnData;
			}
		
			return m_experimentsData;
		}
		
		public static void GetProposalUsers(
			out List<classProposalUser> users,
			out Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
		{
			if (m_proposalUsers != null && m_proposalUsers.Count > 0 && m_pidIndexedReferenceList != null && m_pidIndexedReferenceList.Count > 0)
			{
				users					= m_proposalUsers;
				pidIndexedReferenceList = m_pidIndexedReferenceList;
			}
			else
			{
				List<classUserIDPIDCrossReferenceEntry> crossReferenceList = new List<classUserIDPIDCrossReferenceEntry>();
				users = new List<classProposalUser>();
				pidIndexedReferenceList = new Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>();

				users = new List<classProposalUser>();
				string userTableName = GetTableName(enumTableTypes.PUserList);
				string referenceTableName = GetTableName(enumTableTypes.PReferenceList);

				List<StringDictionary> userExpProperties =
				GetPropertiesFromCache(userTableName, mstring_connectionString);
				List<StringDictionary> referenceExpProperties =
				GetPropertiesFromCache(referenceTableName, mstring_connectionString);

				foreach (StringDictionary props in userExpProperties)
				{
					classProposalUser datum = new classProposalUser();
					datum.LoadPropertyValues(props);
					users.Add(datum);
				}

				foreach (StringDictionary props in referenceExpProperties)
				{
					classUserIDPIDCrossReferenceEntry datum = new classUserIDPIDCrossReferenceEntry();
					datum.LoadPropertyValues(props);
					crossReferenceList.Add(datum);
				}

				foreach (var crossReference in crossReferenceList)
				{
					if (!pidIndexedReferenceList.ContainsKey(crossReference.PID))
					{
						pidIndexedReferenceList.Add(
							crossReference.PID,
							new List<classUserIDPIDCrossReferenceEntry>());
					}

					pidIndexedReferenceList[crossReference.PID].Add(crossReference);
				}

				m_pidIndexedReferenceList	= pidIndexedReferenceList;
				m_proposalUsers				= users;
			}
		}
		
		public static List<classLCColumn> GetEntireLCColumnList()
		{
			if (m_experimentsData == null)
			{
				List<classLCColumn> returnData = new List<classLCColumn>();
		
				string tablename = GetTableName(enumTableTypes.LCColumnList);
		
				List<StringDictionary> allLCColumnProperties =
					GetPropertiesFromCache(tablename, mstring_connectionString);
		
				foreach (StringDictionary props in allLCColumnProperties)
				{
					classLCColumn datum = new classLCColumn();
					datum.LoadPropertyValues(props);
					returnData.Add(datum);
				}
		
				m_lcColumns = returnData;
			}
		
			return m_lcColumns;
		}
		
		/// <summary>
		/// Caches the separation type that is currently selected for this cart
		/// </summary>
		/// <param name="separatonType">Separation type</param>
		public static void SaveSelectedSeparationType(string separatonType)
		{
			// Create a list for the Save call to use
			List<string> sepTypes = new List<string>();
			sepTypes.Add(separatonType);
		
			SaveSingleColumnListToCache(sepTypes, enumTableTypes.SeparationTypeSelected);
		}	
		
		/// <summary>
		/// Retrieves the cached separation type
		/// </summary>
		/// <returns>Separation type</returns>
		public static string GetDefaultSeparationType()
		{
			List<string> sepType = null;
			try
			{
		        
				sepType = GetSingleColumnListFromCache(enumTableTypes.SeparationTypeSelected);
			}
			catch (Exception ex)
			{
		        string firstTime = classLCMSSettings.GetParameter("FirstTime");
		
		        bool isFirstTime = true;
		        if (firstTime != null)
		        {
		            isFirstTime = Convert.ToBoolean(firstTime);
		        }
		        
		        if (!isFirstTime)
		        {
		
		            //ErrMsg = "Exception getting default separation type. (NOTE: This is normal if a new cache is being used)";
		            string errorMessage = "Exception getting default separation type. (NOTE: This is normal if a new cache is being used)";
		            classApplicationLogger.LogError(0, errorMessage, ex);
		        }
		        else
		        {
		            isFirstTime = false;
		            classLCMSSettings.SetParameter("FirstTime", isFirstTime.ToString());
		        }
				return "";
			}
		
			if (sepType.Count != 1)
			{
				return "";
			}
		
			return sepType[0];
		}	
		
		/// <summary>
		/// Generic method for saving a single column list to the cache db
		/// </summary>
		/// <param name="TableType">enumTableNames specifying table name suffix</param>
		/// <param name="ColumnName">Name for column in database</param>
		/// <param name="ListData">List&lt;string&gt; of data for storing in table</param>
		public static void SaveSingleColumnListToCache(List<string> ListData, enumTableTypes TableType)
		{
			// Set up table name
			string tableName = GetTableName(TableType);

			// SQL statement for table clear command
			string sqlClearCmd = "DELETE FROM " + tableName;

			// Build SQL statement for creating table
			string[] colNames = { "Column1" };
			string sqlCreateCmd = BuildGenericCreateTableCmd(tableName, colNames);
            
			// If table exists, clear it. Otherwise create one
			if (VerifyTableExists(tableName, mstring_connectionString))
			{
				// Clear table
				try
				{
					ExecuteSQLiteCommand(sqlClearCmd, mstring_connectionString);
				}
				catch (Exception Ex)
				{
					string ErrMsg = "SQLite exception clearing table via command " + sqlClearCmd;
					// throw new classDatabaseDataException(ErrMsg, Ex);
					classApplicationLogger.LogError(0, ErrMsg, Ex);
					return;
				}
			}
			else
			{
				// Create table
				try
				{
					ExecuteSQLiteCommand(sqlCreateCmd, mstring_connectionString);
				}
				catch (Exception Ex)
				{
					string ErrMsg = "SQLite exception creating table " + tableName;
					// throw new classDatabaseDataException(ErrMsg, Ex);
					classApplicationLogger.LogError(0, ErrMsg, Ex);
					return;
				}
			}

			// Fill the data table
			string sqlInsertCmd;
			List<string> cmdList = new List<string>();
			foreach (string itemName in ListData)
			{
				sqlInsertCmd = "INSERT INTO " + tableName + " values('" + itemName + "')";
				cmdList.Add(sqlInsertCmd);
			}

			// Execute the command list to store data in database
			try
			{
				ExecuteSQLiteCmdsWithTransaction(cmdList, mstring_connectionString);
			}
			catch (Exception Ex)
			{
				string ErrMsg = "SQLite exception filling single-column table";
				// throw new classDatabaseDataException(ErrMsg, Ex);
				classApplicationLogger.LogError(0, ErrMsg, Ex);
				return;
			}
		}	

		/// <summary>
		/// Generic method for retrieving data from a single column table
		/// </summary>
		/// <param name="TableType">enumTableTypes specifying type of table to retrieve</param>
		/// <returns>List&lt;string&gt; containing cached data</returns>
		private static List<string> GetSingleColumnListFromCache(enumTableTypes TableType)
		{
			List<string> returnList = new List<string>();
		
			// Set up table name
			string tableName = GetTableName(TableType);
		
			// Verify specified table exists
			if (!VerifyTableExists(tableName, mstring_connectionString))
			{
				string ErrMsg = "Data table " + tableName + " not found in cache";
				throw new classDatabaseDataException(ErrMsg, new Exception());
			}
		
			// SQL statement for query commanda
			string sqlQueryCmd = "SELECT * FROM " + tableName;
		
			// Get a table from the cache db
			DataTable resultTable = new DataTable();
			try
			{
				resultTable = GetSQLiteDataTable(sqlQueryCmd, mstring_connectionString);
			}
			catch (Exception Ex)
			{
				string ErrMsg = "SQLite exception getting data table via query " + sqlQueryCmd;
				throw new classDatabaseDataException(ErrMsg, Ex);
			}
		
			// Return empty list if no data in table
			if (resultTable.Rows.Count < 1)
			{
				return returnList;
			}
		
			// Fill the return list
			foreach (DataRow currentRow in resultTable.Rows)
			{
				returnList.Add((string)currentRow[resultTable.Columns[0]]);
			}
		
			// All finished, so return
			return returnList;
		}	
		
		/// <summary>
		/// Generic method to build a CREATE TABLE command
		/// </summary>
		/// <param name="TableName">Name of table to create</param>
		/// <param name="ColNames">String array containing column names</param>
		/// <returns>Complete CREATE TABLE command</returns>
		private static string BuildGenericCreateTableCmd(string TableName, string[] ColNames)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CREATE TABLE ");
			sb.Append(TableName + "(");
			// Create column names for each key, which is same as property name in queue being saved
			foreach (string colName in ColNames)
			{
				sb.Append("'" + colName + "', ");
			}
			// Remove the last "', "
			sb.Remove(sb.Length - 2, 2);
			// Terminate the string and return
			sb.Append(")");
			return sb.ToString();
		}	
		
		/// <summary>
		/// Clears a cache table
		/// </summary>
		/// <param name="TableName">Name of table to clear</param>
		/// <param name="ConnStr">Connection string</param>
		private static void ClearCacheTable(string TableName, string ConnStr)
		{
			// Clear the table, if it exists
			if (VerifyTableExists(TableName, ConnStr))
			{
				// Clear the table
				string sqlStr = "DELETE FROM " + TableName;
				try
				{
					ExecuteSQLiteCommand(sqlStr, ConnStr);
				}
				catch (Exception Ex)
				{
					string errorMessage = "Exception clearing table " + TableName;
					classApplicationLogger.LogError(0, errorMessage, Ex);
					throw new classDatabaseDataException("Exception clearing table " + TableName, Ex);
				}
			}
		}	
		#endregion
	}	
}	// End namespace
