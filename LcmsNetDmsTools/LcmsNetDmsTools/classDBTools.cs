
//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Last modified 09/17/2014
//						- 01/26/2009 (DAC) - Added error handling for database errors. Removed excess "using" statements
//						- 02/03/2009 (DAC) - Added methods for accessing additional data per Brian request
//						- 02/04/2009 (DAC) - Changed DMS request retrieval to ruturn List<classSampleData>
//						- 02/19/2009 (DAC) - Incorporated renamed exceptions
//						- 02/24/2009 (DAC) - Changed to cache downloaded data in SQLite database
//						- 03/09/2009 (DAC) - Added download of DMS Comment field
//						- 03/27/2009 (DAC) - Converted LINQ queries to ADO.Net due to terrible error handling by LINQ
//						- 03/31/2009 (DAC) - Implemented file logging for exceptions
//						- 04/02/2009 (DAC) - Implemented MRM file download tools
//						- 04/09/2009 (DAC) - Added retrieval and caching of column lists
//						- 05/12/2009 (DAC) - Added download of blocking and run order values
//						- 08/11/2009 (DAC) - Added download of request batch number
//						- 12/01/2009 (DAC) - Modified to accomodate changing vial from string to int
//						- 02/18/2010 (DAC) - Modified to streamline generation of sample request query
//						- 07/28/2010 (DAC) - Added conversion of alpha well/vial to integer
//						- 12/08/2010 (DAC) - Modified DMS connection string to use name/password authentication
//						- 04/30/2013 (FCT) - Modified get carts sql command string to include "No_Cart" in the results.
//						- 05/22/2014 (MEM) - Replace Select * with selection of specific columns
//                      - 09/11/2014 (CJW) - Modified to be an MEF Extension for lcmsnet
//                      - 09/17/2014 (CJW) - Modified to use a stand-alone configuration file instead of the LcmsNet app configuration file.
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Data;
using LcmsNetSQLiteTools;
using System.ComponentModel.Composition;

namespace LcmsNetDmsTools
{
    [Export(typeof(IDmsTools))]
    [ExportMetadata("Name", "PrismDMSTools")]
    [ExportMetadata("Version", "1.0")]
    public class classDBTools : LcmsNetDataClasses.IDmsTools
    {
        //*********************************************************************************************************
        // Class for interacting with DMS database
        //**********************************************************************************************************

        #region "Class variables"
         string mstring_ErrMsg = "";

         /// <summary>
         /// key to access the DMS version string in the configuration dictionary.
         /// </summary>
         private const string CONST_DMS_VERSION_KEY = "DMSVersion";
         /// <summary>
         /// key to access the encoded DMS password string in the configuration dictionary.
         /// </summary>
         private const string CONST_DMS_PASSWORD_KEY = "DMSPwd";
        #endregion

        #region "Properties"
        public  string ErrMsg
        {
            get
            {
                return mstring_ErrMsg;
            }
            set
            {
                mstring_ErrMsg = value;
            }
        }	// End property

        public string DMSVersion
        {
            get
            {
                return configuration[CONST_DMS_VERSION_KEY];
            }
        }

        private StringDictionary configuration;
     
        #endregion

        #region "Constructors"
        /// <summary>
        /// Constructor
        /// </summary>
        public classDBTools()
        {
            configuration = new StringDictionary();
            LoadConfiguration();
        }
        #endregion

        #region "Methods"

        /// <summary>
        /// Loads DMS configuration from file
        /// </summary>
        private void LoadConfiguration()
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema;
            readerSettings.ValidationEventHandler += settings_ValidationEventHandler;

            string configurationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\PrismDMS.config";
            XmlReader reader = XmlReader.Create(configurationPath, readerSettings);
            while(reader.Read())
            {
                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:                         
                        if(reader.GetAttribute("dmssetting") == "true")
                        {
                            string settingName = reader.Name.Remove(0,2);
                            configuration[settingName] = reader.ReadString();
                        }                 
                        break;
                }
            }
        }

        void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {            
            if(e.Severity == XmlSeverityType.Error)
            {
                throw new InvalidOperationException(e.Message, e.Exception);
            }
            else
            {
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "DmsTools Configuration warning: " + e.Message);
            }
        }

        /// <summary>
        /// Loads all DMS data into cache
        /// </summary>
        public void LoadCacheFromDMS()
        {
            LoadCacheFromDMS(true);
        }
        public void LoadCacheFromDMS(bool shouldLoadExperiment)
        {
            GetCartListFromDMS();
            GetSepTypeListFromDMS();
            GetDatasetTypeListFromDMS();
            GetInstrumentListFromDMS();
            GetUserListFromDMS();
            GetColumnListFromDMS();
            if (shouldLoadExperiment)
            {
                GetExperimentListFromDMS();
            }
            GetProposalUsers();
            //GetEntireColumnListListFromDMS();
        }

        /// <summary>
        /// Gets a list of instrument carts from DMS and stores it in cache
        /// </summary>
        public void GetCartListFromDMS()
        {


            List<string> tmpCartList = null;	// Temp list for holding return values
            string connStr = GetConnectionString();


            // Get a List containing all the carts
            const string sqlCmd = "SELECT [Cart Name] FROM V_LC_Cart_List_Report " +
                                  "WHERE State LIKE '%service%' " +
                                  "ORDER BY [Cart Name]";
            try
            {
                tmpCartList = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting cart list";
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            // Store the list of carts in the cache db
            try
            {
                classSQLiteTools.SaveSingleColumnListToCache(tmpCartList, enumTableTypes.CartList);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing LC cart list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        /// <summary>
        /// Gets a list of LC columns from DMS and stores in the cache
        /// </summary>
        public void GetColumnListFromDMS()
        {
            List<string> tmpColList = null;	// Temp list for holding return values
            string connStr = GetConnectionString();

            // Get a List containing all the columns
            const string sqlCmd = "SELECT val FROM V_LC_Column_Picklist ORDER BY val";
            try
            {
                tmpColList = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting column list";
                //				throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            // Store the list of carts in the cache db
            try
            {
                classSQLiteTools.SaveSingleColumnListToCache(tmpColList, enumTableTypes.ColumnList);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing column list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        public void GetEntireColumnListListFromDMS()
        {
            string connStr = GetConnectionString();
            DataTable lcColumnTable;

            const string sqlCmd = "SELECT [State], [Column Number] FROM V_LC_Column_List_Report";
            try
            {
                lcColumnTable = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting experiment list";
                classApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            // Get list of column names in the table
            var columnNames = new List<string>(lcColumnTable.Columns.Count);
            foreach (DataColumn column in lcColumnTable.Columns)
            {
                string s = column.ColumnName;
                columnNames.Add(s);
            }

            int rowCount = lcColumnTable.Rows.Count;
            var lcColumnList = new List<classLCColumn>(rowCount);

            foreach (DataRow currentRow in lcColumnTable.Rows)
            {
                var tempObject = new classLCColumn
                {
                    LCColumn = currentRow["Column Number"] as string,
                    State = currentRow["State"] as string
                };

                lcColumnList.Add(tempObject);
            }

            try
            {
                classSQLiteTools.SaveEntireLCColumnListToCache(lcColumnList);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing LC Column data list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        /// <summary>
        /// Gets a list of separation types from DMS and stores it in cache
        /// </summary>
        public void GetSepTypeListFromDMS()
        {
            List<string> tmpRetVal;	// Temp list for holding separation types
            string connStr = GetConnectionString();

            const string sqlCmd = "SELECT SS_Name FROM T_Secondary_Sep ORDER BY SS_Name";

            try
            {
                tmpRetVal = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting separation type list";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            // Store data in cache
            try
            {
                classSQLiteTools.SaveSingleColumnListToCache(tmpRetVal, enumTableTypes.SeparationTypeList);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing separation type list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
                return;
            }
        }

        /// <summary>
        /// Gets a list of dataset types from DMS ans stores it in cache
        /// </summary>
        public void GetDatasetTypeListFromDMS()
        {
            List<string> tmpRetVal;	// Temp list for holding dataset types
            string connStr = GetConnectionString();

            // Get a list of the dataset types
            const string sqlCmd = "SELECT DST_Name FROM t_DatasetTypeName ORDER BY DST_Name";
            try
            {
                tmpRetVal = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting dataset type list";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            // Store data in cache
            try
            {
                classSQLiteTools.SaveSingleColumnListToCache(tmpRetVal, enumTableTypes.DatasetTypeList);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing dataset type list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        /// <summary>
        /// Gets a list of active users from DMS and stores it in cache
        /// </summary>
        public void GetUserListFromDMS()
        {
            var tmpRetVal = new List<classUserInfo>();	// Temp list for holding user data
            string connStr = GetConnectionString();

            DataTable userTable;

            // Get a data table containing all the users
            const string sqlCmd = "SELECT Name, [Payroll Num] as Payroll FROM V_Active_Users ORDER BY Name";
            try
            {
                userTable = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting user list";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            //Fill out the temp list
            foreach (DataRow currRow in userTable.Rows)
            {
                var tempUser = new classUserInfo
                {
                    UserName = (string)currRow[userTable.Columns["Name"]],
                    PayrollNum = (string)currRow[userTable.Columns["Payroll"]]
                };
                tmpRetVal.Add(tempUser);
            }

            // Store data in cache
            try
            {
                classSQLiteTools.SaveUserListToCache(tmpRetVal);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing user list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
                return;
            }
        }

        public void GetExperimentListFromDMS()
        {
            string connStr = GetConnectionString();
            DataTable expTable;

            const string sqlCmd = "SELECT ID, Experiment, Created, Organism, Reason, Request, Researcher FROM V_Experiment_List_Report_2";
            try
            {
                expTable = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting experiment list";
                classApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            //// Little piece of code that gets me all the column names of the table.
            //List<string> columnNames = new List<string>(expTable.Columns.Count);
            //foreach (DataColumn column in expTable.Columns)
            //{
            //    string s = column.ColumnName;
            //    columnNames.Add(s);
            //}

            int rowCount = expTable.Rows.Count;
            var expermentData = new List<classExperimentData>(rowCount);

            foreach (DataRow currentRow in expTable.Rows)
            {
                var tempObject = new classExperimentData
                {
                    Created = currentRow["Created"] as DateTime?,
                    Experiment = currentRow["Experiment"] as string,
                    ID = currentRow["ID"] as int?,
                    Organism = currentRow["Organism"] as string,
                    Reason = currentRow["Reason"] as string,
                    Request = currentRow["Request"] as int?,
                    Researcher = currentRow["Researcher"] as string
                };

                expermentData.Add(tempObject);
            }

            try
            {
                classSQLiteTools.SaveExperimentListToCache(expermentData);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing experment list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        public void GetProposalUsers()
        {
            string connStr = GetConnectionString();
            DataTable expTable;

            const string sqlCmd = "SELECT [User ID], [User Name], [#Proposal] FROM V_EUS_Proposal_Users";

            try
            {
                expTable = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting EUS Proposal Users list";
                classApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }


            //// Little piece of code that gets me all the column names of the table.
            //List<string> columnNames = new List<string>(expTable.Columns.Count);
            //foreach (DataColumn column in expTable.Columns)
            //{
            //    string s = column.ColumnName;
            //    columnNames.Add(s);
            //}


            // Split the View back into the two tables it was built from.
            // Note: It would be faster if we had the component tables the View was created from.												

            var users = new List<classProposalUser>();
            var referenceList = new List<classUserIDPIDCrossReferenceEntry>();
            var referenceDictionary = new Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>();

            var userMap = new Dictionary<int, classProposalUser>();

            foreach (DataRow row in expTable.Rows)
            {
                var uid = row["User ID"] as int?;
                var pid = row["#Proposal"] as string;
                var userName = row["User Name"] as string;

                if (!uid.HasValue || string.IsNullOrWhiteSpace(pid) || string.IsNullOrWhiteSpace(userName))
                    continue;

                var user = new classProposalUser();
                var crossReference = new classUserIDPIDCrossReferenceEntry();

                user.UserID = uid.Value;
                user.UserName = userName;

                crossReference.PID = pid;
                crossReference.UserID = uid.Value;

                if (!userMap.ContainsKey(user.UserID))
                {
                    userMap.Add(user.UserID, user);
                    users.Add(user);
                }

                if (!referenceDictionary.ContainsKey(crossReference.PID))
                    referenceDictionary.Add(crossReference.PID, new List<classUserIDPIDCrossReferenceEntry>());

                if (!(referenceDictionary[crossReference.PID].Any(cr => cr.UserID == crossReference.UserID)))
                {
                    referenceDictionary[crossReference.PID].Add(crossReference);
                    referenceList.Add(crossReference);
                }
            }

            try
            {
                classSQLiteTools.SaveProposalUsers(users, referenceList, referenceDictionary);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing Proposal Users list in cache";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        /// <summary>
        /// Gets a list of instruments from DMS
        /// </summary>
        public void GetInstrumentListFromDMS()
        {
            var tmpRetVal = new List<classInstrumentInfo>();	// Temp list for holding instrument data
            string connStr = GetConnectionString();

            DataTable instTable;

            // Get a table containing the instrumnet data
            const string sqlCmd = "SELECT ex, val FROM V_Instrument_Picklist ORDER BY ex";
            try
            {
                instTable = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting instrument list";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return;
            }

            //Fill out the temp list
            foreach (DataRow currRow in instTable.Rows)
            {
                var tempInst = new classInstrumentInfo
                {
                    DMSName = (string)currRow[instTable.Columns["ex"]],
                    CommonName = (string)currRow[instTable.Columns["val"]]
                };
                tmpRetVal.Add(tempInst);
            }

            // Store data in cache
            try
            {
                classSQLiteTools.SaveInstListToCache(tmpRetVal);
            }
            catch (Exception Ex)
            {
                const string errMsg = "Exception storing instrument list in cache";
                classApplicationLogger.LogError(0, errMsg, Ex);
            }
        }

        /// <summary>
        /// Gets a list of run requests from DMS
        /// </summary>
        public List<classSampleData> GetSamplesFromDMS(classSampleQueryData queryData)
        {
            var tmpReturnVal = new List<classSampleData>();	// Temp list for holding samples
            string connStr = GetConnectionString();

            DataTable schedRunList;
            // Get a data table containing run requests
            string sqlCmd = queryData.BuildSqlString();

            try
            {
                schedRunList = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting run request list";
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return tmpReturnVal;
            }

            foreach (DataRow currRow in schedRunList.Rows)
            {
                var tmpDMSData = new classSampleData
                {
                    DmsData =
                    {
                        DatasetType = currRow[schedRunList.Columns["Type"]] as string,
                        Experiment = currRow[schedRunList.Columns["Experiment"]] as string,
                        ProposalID = currRow[schedRunList.Columns["Proposal ID"]] as string,
                        RequestID = (int)currRow[schedRunList.Columns["Request"]],
                        RequestName = currRow[schedRunList.Columns["Name"]] as string,
                        UsageType = currRow[schedRunList.Columns["Usage Type"]] as string,
                        UserList = currRow[schedRunList.Columns["EUS Users"]] as string
                    }
                };
                
                var tmpStr = currRow[schedRunList.Columns["Well Number"]] as string;
                if ((tmpStr == null) || tmpStr == "na")
                    tmpStr = "0";                     

                try
                {
                    tmpDMSData.PAL.Well = ConvertWellStringToInt(tmpStr);
                }
                catch
                {
                    tmpDMSData.PAL.Well = 0;
                }
                tmpDMSData.PAL.WellPlate = currRow[schedRunList.Columns["Wellplate Number"]] as string;
                
                if ((tmpDMSData.PAL.WellPlate == null) || (tmpDMSData.PAL.WellPlate == "na")) 
                    tmpDMSData.PAL.WellPlate = "";

                tmpDMSData.DmsData.CartName = currRow[schedRunList.Columns["Cart"]] as string;
                tmpDMSData.DmsData.Comment = currRow[schedRunList.Columns["Comment"]] as string;
                tmpDMSData.DmsData.MRMFileID = DbCint(currRow[schedRunList.Columns["MRMFileID"]]);
                tmpDMSData.DmsData.Block = DbCint(currRow[schedRunList.Columns["Block"]]);
                tmpDMSData.DmsData.RunOrder = DbCint(currRow[schedRunList.Columns["RunOrder"]]);
                tmpDMSData.DmsData.Batch = DbCint(currRow[schedRunList.Columns["Batch"]]);
                tmpDMSData.DmsData.SelectedToRun = false;
                tmpReturnVal.Add(tmpDMSData);
            }

            // Return and close
            return tmpReturnVal;

        }

        /// <summary>
        /// Adds data for block of MRM files to file data list
        /// </summary>
        /// <param name="FileIndxList">Comma-separated list of file indices needing data</param>
        /// <param name="fileData">List of file names and contents</param>
        public void GetMRMFilesFromDMS(string FileIndxList, ref List<classMRMFileData> fileData)
        {
            DataTable dt;
            string sqlCmd = "SELECT File_Name, Contents FROM T_Attachments WHERE ID IN (" + FileIndxList + ")";
            string connStr = GetConnectionString();

            // Get the data from DMS
            try
            {
                dt = GetDataTable(sqlCmd, connStr);

            }
            catch (Exception Ex)
            {
                mstring_ErrMsg = "Exception getting MRM file data from DMS";
                classApplicationLogger.LogError(0, mstring_ErrMsg, Ex);
                throw new classDatabaseDataException(mstring_ErrMsg, Ex);
            }

            if (dt != null)
            {
                foreach (DataRow currRow in dt.Rows)
                {
                    var currData = new classMRMFileData
                    {
                        FileName = currRow[dt.Columns["File_Name"]] as string,
                        FileContents = currRow[dt.Columns["Contents"]] as string
                    };
                    fileData.Add(currData);
                }
            }
        }

        /// <summary>
        /// Gets a list of MRM files to retrieve
        /// </summary>
        /// <param name="MinID">Minimum request ID for MRM file search</param>
        /// <param name="MaxID"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetMRMFileListFromDMS(int MinID, int MaxID)
        {
            var retList = new Dictionary<int, int>();

            DataTable dt;
            string sqlCmd = "SELECT ID, RDS_MRM_Attachment FROM T_Requested_Run WHERE (not RDS_MRM_Attachment is null) " +
                                    "AND (ID BETWEEN " + MinID + " AND " + MaxID + ")";
            string connStr = GetConnectionString();

            // Get the data from DMS
            try
            {
                dt = GetDataTable(sqlCmd, connStr);
            }
            catch (Exception Ex)
            {
                mstring_ErrMsg = "Exception getting MRM file list from DMS";
                classApplicationLogger.LogError(0, mstring_ErrMsg, Ex);
                throw new classDatabaseDataException(mstring_ErrMsg, Ex);
            }

            // Pull the data from the table
            if (dt != null)
            {
                foreach (DataRow currRow in dt.Rows)
                {
                    var tempIndx = (int)currRow[dt.Columns["ID"]];
                    var tempValue = (int)currRow[dt.Columns["RDS_MRM_Attachment"]];
                    retList.Add(tempIndx, tempValue);
                }
            }
            return retList;
        }

        /// <summary>
        /// Gets DMS connection sstring from config file
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            string retStr = "Data Source=gigasax;Initial Catalog="; //Base connection string

            // Get DMS version and append to base string
            string dmsVersion = configuration[CONST_DMS_VERSION_KEY];
            if (dmsVersion != null)
            {
                retStr += dmsVersion + ";User ID=LCMSNetUser;Password=";
            }
            else
            {
                throw new classDatabaseConnectionStringException("DMS version string not found in configuration file");
            }

            // Get password and append to return string
            string dmsPassword = configuration[CONST_DMS_PASSWORD_KEY];
            if (dmsPassword != null)
            {
                retStr += DecodePassword(dmsPassword);
            }
            else
            {
                throw new classDatabaseConnectionStringException("DMS password string not found in configuration file");
            }

            return retStr;
        }

        /// <summary>
        /// Updates the cart assignment in DMS
        /// </summary>
        /// <param name="requestList">Comma-delimited string of request ID's (must be less than 8000 chars long)</param>
        /// <param name="cartName">Name of cart to assign (ignored for removing aasignment)</param>
        /// <param name="updateMode">TRUE for updating assignment; FALSE to clear assignment</param>
        /// <returns>TRUE for success; FALSE for error</returns>
        public bool UpdateDMSCartAssignment(string requestList, string cartName, bool updateMode)
        {
            string connStr = GetConnectionString();
            string returnMsg = "";
            string mode;
            int resultCode;

            // Verify request list is < 8000 chars (stored procedure limitation)
            if (requestList.Length > 8000)
            {
                mstring_ErrMsg = "Too many requests selected for import.\r\nReduce the number of requests being imported.";
                return false;
            }

            // Convert mode to string value
            if (updateMode)
            {
                mode = "Add";
            }
            else
            {
                mode = "Remove";
            }

            // Set up parameters for stored procedure call
            var spCmd = new SqlCommand();
            spCmd.CommandType = CommandType.StoredProcedure;
            spCmd.CommandText = "AddRemoveRequestCartAssignment";
            spCmd.Parameters.Add(new SqlParameter("@Return", SqlDbType.Int));
            spCmd.Parameters["@Return"].Direction = ParameterDirection.ReturnValue;
            spCmd.Parameters.Add(new SqlParameter("@RequestIDList", SqlDbType.VarChar, 8000));
            spCmd.Parameters["@RequestIDList"].Direction = ParameterDirection.Input;
            spCmd.Parameters["@RequestIDList"].Value = requestList;
            spCmd.Parameters.Add(new SqlParameter("@CartName", SqlDbType.VarChar, 128));
            spCmd.Parameters["@CartName"].Direction = ParameterDirection.Input;
            spCmd.Parameters["@CartName"].Value = cartName;
            spCmd.Parameters.Add(new SqlParameter("@Mode", SqlDbType.VarChar, 32));
            spCmd.Parameters["@Mode"].Direction = ParameterDirection.Input;
            spCmd.Parameters["@Mode"].Value = mode;
            spCmd.Parameters.Add(new SqlParameter("@message", SqlDbType.VarChar, 512));
            spCmd.Parameters["@message"].Direction = ParameterDirection.InputOutput;
            spCmd.Parameters["@message"].Value = "";

            // Execute the SP
            try
            {
                resultCode = ExecuteSP(spCmd, connStr);                
            }
            catch (Exception Ex)
            {
                //					throw new classDatabaseDataException("Exception updating DMS cart list", Ex);
                classApplicationLogger.LogError(0, "Exception updating DMS cart list", Ex);
                return false;
            }

            if (resultCode != 0)	// Error occurred
            {
                returnMsg = spCmd.Parameters["@message"].ToString();
                throw new classDatabaseStoredProcException("AddRemoveRequestCartAssignment", resultCode, returnMsg);
            }

            // Success!
            return true;

        }

        /// <summary>
        /// Generic method to retrieve data from a single-column table in DMS
        /// </summary>
        /// <param name="CmdStr">SQL command to execute</param>
        /// <param name="ConnStr">Database connection string</param>
        /// <returns>List containing the table's contents</returns>
        private List<string> GetSingleColumnTableFromDMS(string CmdStr, string ConnStr)
        {
            var retList = new List<string>();
            DataTable dbTable;

            // Get a table from the database
            try
            {
                dbTable = GetDataTable(CmdStr, ConnStr);
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception getting single column table via command: " + CmdStr;
                //					throw new classDatabaseDataException(ErrMsg, Ex);
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                return retList;

            }

            // Copy the table contents into the list
            foreach (DataRow currRow in dbTable.Rows)
            {
                retList.Add((string)currRow[dbTable.Columns[0]]);
            }

            // Return the list
            return retList;
        }

        /// <summary>
        /// Retrieves a data table from DMS
        /// </summary>
        /// <param name="CmdStr">SQL command to retrieve table</param>
        /// <param name="ConnStr">DMS connection string</param>
        /// <returns>DataTable containing requested data</returns>
        private DataTable GetDataTable(string CmdStr, string ConnStr)
        {
            var returnTable = new DataTable();
            using (var cn = new SqlConnection(ConnStr))
            {
                using (var da = new SqlDataAdapter())
                {
                    using (var cmd = new SqlCommand(CmdStr, cn))
                    {
                        cmd.CommandType = CommandType.Text;
                        da.SelectCommand = cmd;
                        try
                        {
                            da.Fill(returnTable);
                        }
                        catch (Exception Ex)
                        {
                            string errMsg = "SQL exception getting data table via query " + CmdStr;
                            classApplicationLogger.LogError(0, errMsg, Ex);
                            throw new classDatabaseDataException(errMsg, Ex);
                        }
                    }
                }
            }
            // Return the output table
            return returnTable;
        }

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="SpCmd">SQL command object containing SP parameters</param>
        /// <param name="ConnStr">Connection string</param>
        /// <returns>SP result code</returns>
        private int ExecuteSP(SqlCommand SpCmd, string ConnStr)
        {
            int resultCode = -9999;
            try
            {
                using (var cn = new SqlConnection(ConnStr))
                {
                    using (var da = new SqlDataAdapter())
                    {
                        using (var ds = new DataSet())
                        {
                            SpCmd.Connection = cn;
                            da.SelectCommand = SpCmd;
                            da.Fill(ds);
                            resultCode = (int)da.SelectCommand.Parameters["@Return"].Value;
                        }	// End using ds
                    }	// End using da
                }	// End using cn
            }
            catch (Exception Ex)
            {
                ErrMsg = "Exception executing stored procedure " + SpCmd.CommandText;
                classApplicationLogger.LogError(0, ErrMsg, Ex);
                throw new classDatabaseStoredProcException(SpCmd.CommandText, resultCode, Ex.Message);
            }
            return resultCode;
        }

        /// <summary>
        /// Converts a letter/number or just number string representing a well/vial into an integer
        /// </summary>
        /// <param name="inpStr">Input string</param>
        /// <returns>Integer position</returns>
        private int ConvertWellStringToInt(string inpStr)
        {
            const string regexAlpha = @"^[a-hA-H]\d{1,2}$";
            const string regexNum = @"^\d{1,2}$";

            // First, we'll see if it's a simple number
            var re = new Regex(regexNum);
            MatchCollection mc = re.Matches(inpStr);
            if (mc.Count == 1) return int.Parse(mc[0].ToString());

            // Didn't find a nubmer, so try an alpha
            re = new Regex(regexAlpha);
            mc = re.Matches(inpStr);
            if (mc.Count != 1) return 0;	// Input string isn't a proper vial number

            // It's an alpha. Pull off the first character
            string tmpStr = mc[0].ToString();
            string firstChar = tmpStr.Substring(0, 1);

            // Strip first char off of tmpStr and convert rest to int
            int colNum = int.Parse(tmpStr.Replace(firstChar, ""));

            // Convert first char to row multiplier (ugly brute force here, but it works)
            int tmpRowMult = 0;
            switch (firstChar.ToLower())
            {
                case "a":
                    tmpRowMult = 0;
                    break;
                case "b":
                    tmpRowMult = 1;
                    break;
                case "c":
                    tmpRowMult = 2;
                    break;
                case "d":
                    tmpRowMult = 3;
                    break;
                case "e":
                    tmpRowMult = 4;
                    break;
                case "f":
                    tmpRowMult = 5;
                    break;
                case "g":
                    tmpRowMult = 6;
                    break;
                case "h":
                    tmpRowMult = 7;
                    break;
            }

            // Now assemble the whole number and return
            return (tmpRowMult * 12) + colNum;
        }

        /// <summary>
        /// Decrypts password received from ini file
        /// </summary>
        /// <param name="enPwd">Encoded password</param>
        /// <returns>Clear text password</returns>
        private string DecodePassword(string enPwd)
        {
            // Decrypts password received from ini file
            // Password was created by alternately subtracting or adding 1 to the ASCII value of each character

            // Convert the password string to a character array
            char[] pwdChars = enPwd.ToCharArray();
            var pwdBytes = new byte[pwdChars.Length];
            var pwdCharsAdj = new char[pwdChars.Length];

            for (int i = 0; i < pwdChars.Length; i++)
            {
                pwdBytes[i] = (byte)pwdChars[i];
            }

            // Modify the byte array by shifting alternating bytes up or down and convert back to char, and add to output string
            string retStr = "";
            for (int byteCntr = 0; byteCntr < pwdBytes.Length; byteCntr++)
            {
                if ((byteCntr % 2) == 0)
                {
                    pwdBytes[byteCntr] += 1;
                }
                else
                {
                    pwdBytes[byteCntr] -= 1;
                }
                pwdCharsAdj[byteCntr] = (char)pwdBytes[byteCntr];
                retStr += pwdCharsAdj[byteCntr].ToString(CultureInfo.InvariantCulture);
            }
            return retStr;
        }

        /// <summary>
        /// Converts to integer while handling null values
        /// </summary>
        /// <param name="InpObj">Object to convert</param>
        /// <returns>0 if null, otherwise integer version of InpObj</returns>
        private int DbCint(object InpObj)
        {            
            if (InpObj is DBNull)
            {
                return 0;
            }

            return (int)InpObj;
        }

        #endregion
    }
}	// End namespace
