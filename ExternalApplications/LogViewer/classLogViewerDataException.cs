//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/08/2010
//
// Last modified 02/08/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewer
{
	class classLogViewerDataException : Exception
	{
		//*********************************************************************************************************
		// Log viewer custom exception class
		//**********************************************************************************************************

		#region "Constructors"
			public classLogViewerDataException(string message, Exception ex) :
				base(message,ex)
			{
			}
		#endregion
	}	
}	// End namespace
