
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 0219/2009
//
// Last modified 02/19/2009
//						- 02/24/2008 (DAC) - Added additional enums for DMS data types
//						- 12/08/2009 (DAC) - Added enum for caching selected separation type
//
//*********************************************************************************************************
				
namespace LcmsNetSQLiteTools
{
	/// <summary>
	/// Describes the available data table names
	/// </summary>
	public enum enumTableTypes
	{
		WaitingQueue,
		RunningQueue,
		CompletedQueue,
		UserList,
		CartList,
		SeparationTypeList,
		SeparationTypeSelected,
		DatasetTypeList,
		InstrumentList,
		ColumnList,
		LCColumnList,	// Superset of ColumnList that spans active and inactive LC Columns and includes their state
		ExperimentList,
		PUserList,			// A User that's in a Proposal
		PReferenceList,		// A cross reference of the PUser and a Proposal
	}	
}	
