using System;
using System.Collections.Generic;
using System.Text;

namespace LcmsNet.IO.DMS
{
    /// <summary>
    /// Class for holding data used to query DMS for samples to run
    /// </summary>
    public class SampleQueryData
    {
        // Ignore Spelling: Wellplate

        /// <summary>
        /// Dictionary of filters to apply when finding requested runs
        /// </summary>
        /// <remarks>Keys are RequestName, MinRequestNum, MaxRequestNum, BatchID, Block, Cart, or Wellplate</remarks>
        readonly Dictionary<string, string> m_QueryParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

        // ReSharper disable once UnusedMember.Global
        [Obsolete("This property is unused")]
        public bool UnassignedOnly { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SampleQueryData()
        {
            // Define a minimum request ID by default, but do not define a maximum
            MinRequestNum = "0";
        }

        /// <summary>
        /// If a specified filter is defined, append the appropriate SQL to the query builder
        /// </summary>
        /// <param name="queryBuilder"></param>
        /// <param name="sqlFilter"></param>
        /// <param name="filterName"></param>
        private void AddQueryFilter(StringBuilder queryBuilder, string sqlFilter, string filterName)
        {
            var filterValue = GetValueIfFound(filterName);
            if (string.IsNullOrWhiteSpace(filterValue))
                return;

            if (queryBuilder.Length > 0)
                queryBuilder.Append(" AND ");

            queryBuilder.Append(string.Format(sqlFilter, filterValue));
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
        /// Build the query string for retrieving data from V_Requested_Run_Active_Export
        /// </summary>
        /// <returns></returns>
        public string BuildSqlString()
        {
            const string cmdBase = "SELECT Request, Name, Cart, Comment, Usage_Type, EUS_Users, Block, Run_Order, Batch, Well, Wellplate FROM V_Requested_Run_Active_Export";
            var queryBuilder = new StringBuilder();

            // Note that minimum request ID is auto-defined as 0 in the constructor
            // Add it now only if the calling class changed it from 0
            if (MinRequestNum != "0")
                AddQueryFilter(queryBuilder, "Request >= {0}", "MinRequestNum");

            // Add additional filters if they are defined
            AddQueryFilter(queryBuilder, "Request <= {0}", "MaxRequestNum");

            AddQueryFilter(queryBuilder, "Name LIKE '%{0}%'", "RequestName");
            AddQueryFilter(queryBuilder, "Cart LIKE '%{0}%'", "Cart");

            AddQueryFilter(queryBuilder, "Batch = {0}", "BatchID");
            AddQueryFilter(queryBuilder, "Block = {0}", "Block");

            AddQueryFilter(queryBuilder, "Wellplate LIKE '%{0}%'", "Wellplate");

            if (queryBuilder.Length == 0)
            {
                // No filters, just order the results
                return cmdBase + " ORDER BY Name";
            }

            // Filters are defined
            return cmdBase + " WHERE " + queryBuilder + " ORDER BY Name";
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
    }
}
