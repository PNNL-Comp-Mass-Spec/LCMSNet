
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/04/2009
//
// Last modified 02/04/2009
//						- 02/24/2009 (DAC) - Now inherits from classDataClassBase
//
//*********************************************************************************************************

namespace LcmsNetDataClasses
{
	public class classUserInfo : classDataClassBase
	{
		//*********************************************************************************************************
		// Class to hold data about LcmsNet users
		//**********************************************************************************************************

		#region "Properties"
			/// <summary>
			/// Name of user
			/// </summary>
			public string UserName
			{
				get;
				set;
			}	// End property

			/// <summary>
			/// Payroll number (network login) of user
			/// </summary>
			public string PayrollNum
			{
				get;
				set;
			}	// End property
		#endregion

	}	
}	  // End namespace
