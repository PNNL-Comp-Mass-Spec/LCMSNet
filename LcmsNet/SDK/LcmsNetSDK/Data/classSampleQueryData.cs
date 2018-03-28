//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 01/01/2010
//
// Updates
// - 03/09/2010 (DAC) - Added field for wellplate name
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace LcmsNetSDK.Data
{
    /// <summary>
    /// Class for holding data used to query DMS for samples to run
    /// </summary>
    public class classSampleQueryData
    {
        #region "Constants"

        readonly string CMD_BASE = "SELECT * FROM V_Scheduled_Run_Export";

        #endregion

        #region "Class variables"

        /// <summary>
        /// Dictionary of filters to apply when finding requested runs
        /// </summary>
        /// <remarks>Keys are RequestName, MinRequestNum, MaxRequestNum, BatchID, Block, Cart, or Wellplate</remarks>
        readonly Dictionary<string, string> m_QueryParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region "Properties"

        /// <summary>
        /// Requested run name
        /// </summary>
        public string RequestName
        {
            get => GetValueIfFound("RequestName");
            set => m_QueryParams["RequestName"] = value;
        }

        /// <summary>
        /// Minimum request ID (integer)
        /// </summary>
        public string MinRequestNum
        {
            get => GetValueIfFound("MinRequestNum");
            set => m_QueryParams["MinRequestNum"] = value;
        }

        /// <summary>
        /// Maximum request ID (integer)
        /// </summary>
        public string MaxRequestNum
        {
            get => GetValueIfFound("MaxRequestNum");
            set => m_QueryParams["MaxRequestNum"] = value;
        }

        /// <summary>
        /// Batch ID to use
        /// </summary>
        public string BatchID
        {
            get => GetValueIfFound("BatchID");
            set => m_QueryParams["BatchID"] = value;
        }

        /// <summary>
        /// Block to use
        /// </summary>
        public string Block
        {
            get => GetValueIfFound("Block");
            set => m_QueryParams["Block"] = value;
        }

        /// <summary>
        /// Cart name (supports partial match)
        /// </summary>
        public string Cart
        {
            get => GetValueIfFound("Cart");
            set => m_QueryParams["Cart"] = value;
        }

        public string Wellplate
        {
            get => GetValueIfFound("Wellplate");
            set => m_QueryParams["Wellplate"] = value;
        }

        [Obsolete("This property is unused")]
        public bool UnassignedOnly { get; set; }

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor
        /// </summary>
        public classSampleQueryData()
        {
            // Define a minimum request ID by default, but do not define a maximum
            MinRequestNum = "0";
        }

        /// <summary>
        /// If a specified filter is defined, append the appropriate SQL to the query builder
        /// </summary>
        /// <param name="queryBldr"></param>
        /// <param name="sqlFilter"></param>
        /// <param name="filterName"></param>
        private void AddQueryFilter(StringBuilder queryBldr, string sqlFilter, string filterName)
        {
            var filterValue = GetValueIfFound(filterName);
            if (string.IsNullOrWhiteSpace(filterValue))
                return;

            if (queryBldr.Length > 0)
                queryBldr.Append(" AND ");

            queryBldr.Append(string.Format(sqlFilter, filterValue));
        }

        /// <summary>
        /// Tests for existence of spcified key in dictionary
        /// </summary>
        /// <param name="dictKey">Key name</param>
        /// <returns>Key value if found, otherwise empty string</returns>
        private string GetValueIfFound(string dictKey)
        {
            if (m_QueryParams.ContainsKey(dictKey))
            {
                return m_QueryParams[dictKey];
            }
            return string.Empty;
        }

        /// <summary>
        /// Build the query string for retrieving data from V_Scheduled_Run_Export
        /// </summary>
        /// <returns></returns>
        public string BuildSqlString()
        {
            var queryBldr = new StringBuilder();

            // Note that minimum request ID is auto-defined as 0 in the constructor
            // Add it now only if the calling class changed it from 0
            if (MinRequestNum != "0")
                AddQueryFilter(queryBldr, "Request >= {0}", "MinRequestNum");

            // Add additional filters if they are defined
            AddQueryFilter(queryBldr, "Request <= {0}", "MaxRequestNum");

            AddQueryFilter(queryBldr, "Name LIKE '%{0}%'", "RequestName");
            AddQueryFilter(queryBldr, "Cart LIKE '%{0}%'", "Cart");

            AddQueryFilter(queryBldr, "Batch = {0}", "BatchID");
            AddQueryFilter(queryBldr, "Block = {0}", "Block");

            AddQueryFilter(queryBldr, "[Wellplate Number] LIKE '%{0}%'", "Wellplate");

            if (queryBldr.Length == 0)
            {
                // No filters, just order the results
                return CMD_BASE + " ORDER BY Name";
            }

            // Filters are defined
            return CMD_BASE + " WHERE " + queryBldr + " ORDER BY Name";

        }

        //public bool OneParamHasValue()
        //{
        //   foreach (string testStr in m_QueryParams.Values)
        //   {
        //      if (testStr.Length > 0)
        //      { return true; }
        //   }
        //   return false;
        //}

        public override string ToString()
        {
            return BuildSqlString();
        }

        #endregion
    }
}
