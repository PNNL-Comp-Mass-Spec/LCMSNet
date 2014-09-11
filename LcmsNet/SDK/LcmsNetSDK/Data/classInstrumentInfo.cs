
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/04/2009
//
// Last modified 02/04/2009
//						- 02/24/2009 (DAC) - Now inherits from classDataClassBase
//                      - 03/16/2009 (BLL) - Added method names, and methods to the class so we 
//                        what methods are available for use and what method to run during an experiment.
//                      - 03/17/2009 (BLL) - Added Serializable attribute to allow for deep copy
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses
{
    /// <summary>
    /// Class to hold data about the instrument connected to the LC cart
    /// </summary>
    /// 
   [Serializable]
	public class classInstrumentInfo : classDataClassBase
	{
		//*********************************************************************************************************
		// Class to hold data about the instrument connected to the LC cart
		//**********************************************************************************************************

		#region "Properties"
			/// <summary>
			/// Instrument name as used in DMS
			/// </summary>
			public string DMSName
			{
				get;
				set;
			}	// End property
			/// <summary>
			/// User-friendly name used for pick lists
			/// </summary>
			public string CommonName
			{
				get;
				set;
			}	// End property
            /// <summary>
            /// Gets or sets the name of the method to run on the instrument.
            /// </summary>
            public string MethodName
            {
                get;
                set;
            }
		#endregion
	}	
}	// End namespace
