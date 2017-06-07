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

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Class for holding data used to query DMS for samples to run
    /// </summary>
    public class classSampleQueryData
    {
        #region "Constants"

        readonly string CMD_BASE = "SELECT * FROM V_Scheduled_Run_Export WHERE ";

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
        /// Tests for existence of spcified key in dictionary
        /// </summary>
        /// <param name="dictKey">Key name</param>
        /// <returns>Key value if found, otherwise empty string</returns>
        private String GetValueIfFound(String dictKey)
        {
            if (m_QueryParams.ContainsKey(dictKey))
            {
                return m_QueryParams[dictKey];
            }
            return string.Empty;
        }

        public string BuildSqlString()
        {
            var queryBldr = new StringBuilder(CMD_BASE);

            // Add min/max request numbers
            queryBldr.Append("Request >= '" + m_QueryParams["minrequestnum"] + "'");
            queryBldr.Append(" AND Request <= '" + m_QueryParams["maxrequestnum"] + "'");

            // Add request name, if applicable
            if (m_QueryParams["requestname"].Length > 0)
            {
                queryBldr.Append(" AND Name LIKE '%" + m_QueryParams["requestname"] + "%'");
            }

            // Add cart, if applicable
            if (m_QueryParams["cart"].Length > 0)
            {
                queryBldr.Append(" AND Cart LIKE '%" + m_QueryParams["cart"] + "%'");
            }

            // Add batch ID, if applicable
            if (m_QueryParams["batchid"].Length > 0)
            {
                queryBldr.Append(" AND Batch='" + m_QueryParams["batchid"] + "'");
            }

            // Add block, if applicable
            if (m_QueryParams["block"].Length > 0)
            {
                queryBldr.Append(" AND Block='" + m_QueryParams["block"] + "'");
            }

            // Addwellplate, if applicable
            if (m_QueryParams["wellplate"].Length > 0)
            {
                queryBldr.Append(" AND [Wellplate Number] LIKE '%" + m_QueryParams["wellplate"] + "%'");
            }

            // Add Order clause
            queryBldr.Append(" ORDER BY Name");

            return queryBldr.ToString();
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
