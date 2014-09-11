
//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/13/2009
//
// Last modified 03/24/2014
//                      03/24/2014 (Christopher Walters) -Added static event for when a setting is changd.
//						02/04/2009 (DAC) - Converted methods to static
//						08/31/2010 (DAC) - Changes resulting from move part of configuraton to LcmsNet namespace
//
//*********************************************************************************************************
using System.Collections.Specialized;
using System;

namespace LcmsNetDataClasses
{
    public class SettingChangedEventArgs:EventArgs
    {
        private string m_settingName;
        private string m_settingValue;
        
        public SettingChangedEventArgs(string name, string value)
        {
            m_settingName = name;
            m_settingValue = value;
        }

        public string SettingName
        {         
            get
            {
                return m_settingName;
            }
        }

        public string SettingValue
        {
            get
            {
                return m_settingValue;
            }
        }
    }
    
	public class classLCMSSettings
	{
        public const string CONST_UNASSIGNED_CART_NAME = "(none)";
        public static event EventHandler<SettingChangedEventArgs> SettingChanged;
		//*********************************************************************************************************
		// Class to handle program settings data
		//**********************************************************************************************************
		#region "Class variables"
			/// <summary>
			/// String dictionary to hold settings data
			/// </summary>
			static readonly StringDictionary mstringdict_SettingsDict;
		#endregion
		
		#region "Methods"
			/// <summary>
			/// Constructor to initialize static members
			/// </summary>
			static classLCMSSettings()
			{
				mstringdict_SettingsDict = new StringDictionary();
			}	

			/// <summary>
			/// Adds to or changes a parameter in the string dictionary
			/// </summary>
			/// <param name="ItemKey">Key for item</param>
			/// <param name="ItemValue">Value of item</param>
			public static void SetParameter(string ItemKey, string ItemValue)
			{
                if(SettingChanged != null)
                {
                    SettingChanged(null, new SettingChangedEventArgs(ItemKey, ItemValue));
                }
				mstringdict_SettingsDict[ItemKey] = ItemValue;
			}	

			/// <summary>
			/// Retrieves specified item from string dictionary
			/// </summary>
			/// <param name="ItemKey">Key for item to be retrieved</param>
			/// <returns></returns>
			public static string GetParameter(string ItemKey)
			{
				return mstringdict_SettingsDict[ItemKey];
			}	
		#endregion
	}	
}	// End namespace
