//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/21/2009
//
// Last modified 01/21/2009
//						- 03/10/2010 (DAC) - Added wellplate field, changed vial field to well
//*********************************************************************************************************
				
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;

namespace LcmsNetDataClasses.Data
{
	[Serializable]
    public class classPalData: classDataClassBase, ICloneable
	 {
		 //*********************************************************************************************************
		 //Class that encapsulates the PAL data.
		 //**********************************************************************************************************

		 #region "Constants"
		 private const string CONST_METHOD_NAME = "std_01";
        /// <summary>
        /// Default sample vial number.  This should be invalid and force the user to update the sample information before running.
        /// </summary>
	     public const int     CONST_DEFAULT_VIAL_NUMBER = 0;
		 #endregion

		 #region "Class variables"
			/// <summary>
			/// Name of the PAL method to run.
			/// </summary>
			private string mstring_palMethod = "";

			/// <summary>
			/// Name of the PAL tray to use.
			/// </summary>
			private string mstring_PalTray = "";

			/// <summary>
			/// Vial index to use.
			/// </summary>
			private int mint_Well = 0;

			/// <summary>
			/// Wellplate name
			/// </summary>
			private string mstring_WellPlate = "";			
		 #endregion

		 #region "Properties"
			/// <summary>
			/// Gets or sets the vial number to pull sample from.
			/// </summary>
			public int Well
			{
			 get { return mint_Well; }
			 set { mint_Well = value; }
			}

			/// <summary>
			/// Gets or sets the tray name to use.
			/// </summary>
			public string PALTray
			{
			 get { return mstring_PalTray; }
			 set { mstring_PalTray = value; }
			}

			/// <summary>
			/// Gets or sets the PAL method to use.
			/// </summary>
			public string Method
			{
			 get { return mstring_palMethod; }
			 set { mstring_palMethod = value; }
			}

			/// <summary>
			/// Gets or sets the Wellplate name that is stored in DMS.
			/// </summary>
			public string WellPlate
			{
				get { return mstring_WellPlate; }
				set { mstring_WellPlate = value; }
			}
		 #endregion

		 #region "Constructors"
			 /// <summary>
			 /// Default constructor.
			 /// </summary>
			 public classPalData()
			 {
				 mstring_palMethod  = CONST_METHOD_NAME;
				 mstring_PalTray    = "";
				 mint_Well          = CONST_DEFAULT_VIAL_NUMBER;
			 }
		 #endregion

       #region ICloneable Members
			/// <summary>
			/// Returns a new object reference to a cloned copy of this PAL data.
			/// </summary>
			/// <returns>A new object reference as a copy of this.</returns>
			public object Clone()
			{
				classPalData newData = new classPalData();
				newData.PALTray    = mstring_PalTray;
				newData.Method  = mstring_palMethod;
				newData.Well   = mint_Well;
				newData.WellPlate = mstring_WellPlate;

				return newData;
			}
       #endregion

		//#region Well Plate - Vial Conversions
		//  /// <summary>
		//  /// Converts a given vial number to a well plate index.
		//  /// </summary>
		//  /// <param name="vialNumber">Number to convert to a well plate location.</param>
		//  /// <returns></returns>
		//  public static string ConvertVialToWellPlateLocation(int vialNumber)
		//  {
		//      return vialNumber.ToString();
		//  }
		//  #endregion
    }
}
