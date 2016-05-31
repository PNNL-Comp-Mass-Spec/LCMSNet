
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/09/2010
//
// Last modified 02/09/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace LogViewer
{
	class classLogQueryData
	{
		//*********************************************************************************************************
		// Class for holding data used to query log db
		//**********************************************************************************************************

		#region "Class variables"
			StringDictionary mlist_QueryParams = new StringDictionary();
		#endregion

		#region "Properties"
			public string StartTime
			{
				get { return GetValueIfFound("starttime"); }
				set { mlist_QueryParams["starttime"] = value; }
			}

			public string StopTime
			{
				get { return GetValueIfFound("stoptime"); }
				set { mlist_QueryParams["stoptime"] = value; }
			}

			public string Type
			{
				get { return GetValueIfFound("type"); }
				set { mlist_QueryParams["type"] = value; }
			}

			public string Sample
			{
				get { return GetValueIfFound("sample"); }
				set { mlist_QueryParams["sample"] = value; }
			}

			public string Column
			{
				get { return GetValueIfFound("column"); }
				set { mlist_QueryParams["column"] = value; }
			}

			public string Device
			{
				get { return GetValueIfFound("device"); }
				set { mlist_QueryParams["device"] = value; }
			}

			public string Message
			{
				get { return GetValueIfFound("message"); }
				set { mlist_QueryParams["message"] = value; }
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
				if (mlist_QueryParams.ContainsKey(dictKey))
				{
					return mlist_QueryParams[dictKey];
				}
				else
				{
					return "";
				}
			}	

			public bool OneParamHasValue()
			{
				foreach (string testStr in mlist_QueryParams.Values)
				{
					if (testStr.Length > 0)
					{ return true; }
				}
				return false;
			}	
		#endregion
	}	
}	// End namespace
