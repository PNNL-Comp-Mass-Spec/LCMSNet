
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

namespace LcmsNetDmsTools
{
	public class classSampleQueryData
	{
		//*********************************************************************************************************
		// Class for holding data used to query DMS for samples to run
		//**********************************************************************************************************

		#region "Constants"
			string CMD_BASE = "SELECT * FROM V_Scheduled_Run_Export WHERE ";
		#endregion

		#region "Class variables"
			StringDictionary mlist_QueryParams = new StringDictionary();
			bool mbool_UnassignedOnly = false;
		#endregion

		#region "Properties"
			public string RequestName
			{
				get { return GetValueIfFound("requestname"); }
				set { mlist_QueryParams["requestname"] = value; }
			}

			public string MinRequestNum
			{
				get { return GetValueIfFound("minrequestnum"); }
				set { mlist_QueryParams["minrequestnum"] = value; }
			}

			public string MaxRequestNum
			{
				get { return GetValueIfFound("maxrequestnum"); }
				set { mlist_QueryParams["maxrequestnum"] = value; }
			}

			public string BatchID
			{
				get { return GetValueIfFound("batchid"); }
				set { mlist_QueryParams["batchid"] = value; }
			}

			public string Block
			{
				get { return GetValueIfFound("block"); }
				set { mlist_QueryParams["block"] = value; }
			}

			public string Cart
			{
				get { return GetValueIfFound("cart"); }
				set { mlist_QueryParams["cart"] = value; }
			}

			public string Wellplate
			{
				get { return GetValueIfFound("wellplate"); }
				set { mlist_QueryParams["wellplate"] = value; }
			}

			public bool UnassignedOnly
			{
				get { return mbool_UnassignedOnly; }
				set { mbool_UnassignedOnly = value; }
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

			public string BuildSqlString()
			{
				StringBuilder queryBldr = new StringBuilder(CMD_BASE);

				// Add min/max request numbers
				queryBldr.Append("Request >= '" + mlist_QueryParams["minrequestnum"] + "'");
				queryBldr.Append(" AND Request <= '" + mlist_QueryParams["maxrequestnum"] + "'");

				// Add request name, if applicable
				if (mlist_QueryParams["requestname"].Length > 0)
				{
					queryBldr.Append(" AND Name LIKE '%" + mlist_QueryParams["requestname"] + "%'");
				}

				// Add cart, if applicable
				if (mlist_QueryParams["cart"].Length > 0)
				{
					queryBldr.Append(" AND Cart LIKE '%" + mlist_QueryParams["cart"] + "%'");
				}

				// Add batch ID, if applicable
				if (mlist_QueryParams["batchid"].Length > 0)
				{
					queryBldr.Append(" AND Batch='" + mlist_QueryParams["batchid"] + "'");
				}

				// Add block, if applicable
				if (mlist_QueryParams["block"].Length > 0)
				{
					queryBldr.Append(" AND Block='" + mlist_QueryParams["block"] + "'");
				}

				// Addwellplate, if applicable
				if (mlist_QueryParams["wellplate"].Length > 0)
				{
					queryBldr.Append(" AND [Wellplate Number] LIKE '%" + mlist_QueryParams["wellplate"] + "%'");
				}

				// Add Order clause
				queryBldr.Append(" ORDER BY Name");

				return queryBldr.ToString();
			}	

			//public bool OneParamHasValue()
			//{
			//   foreach (string testStr in mlist_QueryParams.Values)
			//   {
			//      if (testStr.Length > 0)
			//      { return true; }
			//   }
			//   return false;
			//}	
		#endregion
	}	
}	// End namespace
