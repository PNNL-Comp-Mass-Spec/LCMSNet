//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 01/01/2010
///
// Last modified 01/01/2010
//						- 03/09/2010 (DAC) - Added field for wellplate name
//
//****************/*****************************************************************************************

using System;
using System.Collections.Specialized;
using System.Text;

namespace LcmsNetDataClasses
{
    public class classSampleQueryData
    {
        //*********************************************************************************************************
        // Class for holding data used to query DMS for samples to run
        //**********************************************************************************************************

        #region "Constants"

        readonly string CMD_BASE = "SELECT * FROM V_Scheduled_Run_Export WHERE ";

        #endregion

        #region "Class variables"

        readonly StringDictionary m_QueryParams = new StringDictionary();
        bool m_UnassignedOnly;

        #endregion

        #region "Properties"

        public string RequestName
        {
            get { return GetValueIfFound("requestname"); }
            set { m_QueryParams["requestname"] = value; }
        }

        public string MinRequestNum
        {
            get { return GetValueIfFound("minrequestnum"); }
            set { m_QueryParams["minrequestnum"] = value; }
        }

        public string MaxRequestNum
        {
            get { return GetValueIfFound("maxrequestnum"); }
            set { m_QueryParams["maxrequestnum"] = value; }
        }

        public string BatchID
        {
            get { return GetValueIfFound("batchid"); }
            set { m_QueryParams["batchid"] = value; }
        }

        public string Block
        {
            get { return GetValueIfFound("block"); }
            set { m_QueryParams["block"] = value; }
        }

        public string Cart
        {
            get { return GetValueIfFound("cart"); }
            set { m_QueryParams["cart"] = value; }
        }

        public string Wellplate
        {
            get { return GetValueIfFound("wellplate"); }
            set { m_QueryParams["wellplate"] = value; }
        }

        public bool UnassignedOnly
        {
            get { return m_UnassignedOnly; }
            set { m_UnassignedOnly = value; }
        }

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
            else
            {
                return string.Empty;
            }
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

        #endregion
    }
} // End namespace