
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/09/2010
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;

namespace LogViewer
{
    /// <summary>
    /// Class for holding data used to query log db
    /// </summary>
    class classLogQueryData
    {
        #region "Class variables"

        readonly Dictionary<string, string> m_QueryParams = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region "Properties"
        public string StartTime
        {
            get => GetValueIfFound("starttime");
            set => m_QueryParams["starttime"] = value;
        }

        public string StopTime
        {
            get => GetValueIfFound("stoptime");
            set => m_QueryParams["stoptime"] = value;
        }

        public string Type
        {
            get => GetValueIfFound("type");
            set => m_QueryParams["type"] = value;
        }

        public string Sample
        {
            get => GetValueIfFound("sample");
            set => m_QueryParams["sample"] = value;
        }

        public string Column
        {
            get => GetValueIfFound("column");
            set => m_QueryParams["column"] = value;
        }

        public string Device
        {
            get => GetValueIfFound("device");
            set => m_QueryParams["device"] = value;
        }

        public string Message
        {
            get => GetValueIfFound("message");
            set => m_QueryParams["message"] = value;
        }
        #endregion

        #region "Methods"
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

            return "";
        }

        public bool OneParamHasValue()
        {
            foreach (string testStr in m_QueryParams.Values)
            {
                if (testStr.Length > 0)
                { return true; }
            }
            return false;
        }
        #endregion
    }
}
