
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/21/2009
//
// Last modified 02/21/2009
//						 - 02/23/2009 (DAC) - Corrected bug in interface syntax
//
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace LcmsNetDataClasses
{
	public interface ICacheInterface
	{
		//*********************************************************************************************************
		// Interface for data classes that use the cache database
		//**********************************************************************************************************

		#region "Methods"
		StringDictionary GetPropertyValues();

		void LoadPropertyValues(StringDictionary PropValues);
		#endregion
	} // End interface
}	// End namespace
