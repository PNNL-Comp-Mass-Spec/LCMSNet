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
using System.Collections.Generic;

namespace LcmsNetDataClasses
{
    public class SettingChangedEventArgs : EventArgs
    {
        public SettingChangedEventArgs(string name, string value)
        {
            SettingName = name;
            SettingValue = value;
        }

        public string SettingName { get; }

        public string SettingValue { get; }
    }

    /// <summary>
    /// Class to handle program settings data
    /// </summary>
    public class classLCMSSettings
    {
        public const string CONST_UNASSIGNED_CART_NAME = "(none)";

        public const string PARAM_APPLICATIONPATH = "ApplicationPath";
        public const string PARAM_CACHEFILENAME = "CacheFileName";
        public const string PARAM_CARTCONFIGNAME = "CartConfigName";
        public const string PARAM_CARTNAME = "CartName";
        public const string PARAM_COLUMNNAME = "ColumnName";
        public const string PARAM_COLUMNNAME0 = "ColumnName0";
        public const string PARAM_COLUMNNAME1 = "ColumnName1";
        public const string PARAM_COLUMNNAME2 = "ColumnName2";
        public const string PARAM_COLUMNNAME3 = "ColumnName3";
        public const string PARAM_COPYMETHODFOLDERS = "CopyMethodFolders";
        public const string PARAM_COPYTRIGGERFILES = "CopyTriggerFiles";
        public const string PARAM_CREATEMETHODFOLDERS = "CreateMethodFolders";
        public const string PARAM_CREATETRIGGERFILES = "CreateTriggerFiles";
        public const string PARAM_DMSPWD = "DMSPwd";
        public const string PARAM_DMSTOOL = "DMSTool";
        public const string PARAM_DMSVERSION = "DMSVersion";
        public const string PARAM_EMULATIONENABLED = "EmulationEnabled";
        public const string PARAM_ERRORPATH = "ErrorPath";
        public const string PARAM_FIRSTTIME_LOOKUP_SELECTED_SEP_TYPE = "FirstTime";
        public const string PARAM_INITIALIZEHARDWAREONSTARTUP = "InitializeHardwareOnStartup";
        public const string PARAM_INSTNAME = "InstName";
        public const string PARAM_LOGGINGERRORLEVEL = "LoggingErrorLevel";
        public const string PARAM_LOGGINGMSGLEVEL = "LoggingMsgLevel";
        public const string PARAM_MINIMUMVOLUME = "MinimumVolume";
        public const string PARAM_OPERATOR = "Operator";
        public const string PARAM_PALMETHODSFOLDER = "PalMethodsFolder";
        public const string PARAM_PDFPATH = "PdfPath";
        public const string PARAM_PLUGINFOLDER = "PluginFolder";
        public const string PARAM_SEPARATIONTYPE = "SeparationType";
        public const string PARAM_TIMEZONE = "TimeZone";
        public const string PARAM_TRIGGERFILEFOLDER = "TriggerFileFolder";
        public const string PARAM_VALIDATESAMPLESFORDMS = "ValidateSamplesForDMS";

        #region "Class variables"

        /// <summary>
        /// String dictionary to hold settings data
        /// </summary>
        static readonly Dictionary<string, string> m_Settings;

        #endregion

        public static event EventHandler<SettingChangedEventArgs> SettingChanged;

        #region "Methods"

        /// <summary>
        /// Constructor to initialize static members
        /// </summary>
        static classLCMSSettings()
        {
            m_Settings = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds to or changes a parameter in the string dictionary
        /// </summary>
        /// <param name="ItemKey">Key for item</param>
        /// <param name="ItemValue">Value of item</param>
        public static void SetParameter(string ItemKey, string ItemValue)
        {
            SettingChanged?.Invoke(null, new SettingChangedEventArgs(ItemKey, ItemValue));

            if (m_Settings.ContainsKey(ItemKey))
                m_Settings[ItemKey] = ItemValue;
            else
                m_Settings.Add(ItemKey, ItemValue);
        }

        /// <summary>
        /// Retrieves specified item from string dictionary
        /// </summary>
        /// <param name="ItemKey">Key for item to be retrieved</param>
        /// <returns></returns>
        public static string GetParameter(string ItemKey)
        {
            if (m_Settings.ContainsKey(ItemKey))
                return m_Settings[ItemKey];

            return string.Empty;
        }

        #endregion
    }
} // End namespace