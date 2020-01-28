using System;
using System.Collections.Generic;

namespace LcmsNetData
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
    public class LCMSSettings
    {
        public const string CONST_UNASSIGNED_CART_NAME = "(none)";

        public const string PARAM_APPLICATIONPATH = "ApplicationPath";
        public const string PARAM_APPLICATIONDATAPATH = "ApplicationDataPath";
        public const string PARAM_CACHEFILENAME = "CacheFileName";
        public const string PARAM_CARTCONFIGNAME = "CartConfigName";
        public const string PARAM_CARTNAME = "CartName";
        public const string PARAM_COLUMNNAME = "ColumnName";
        public const string PARAM_COLUMNNAME0 = "ColumnName0";
        public const string PARAM_COLUMNNAME1 = "ColumnName1";
        public const string PARAM_COLUMNNAME2 = "ColumnName2";
        public const string PARAM_COLUMNNAME3 = "ColumnName3";
        public const string PARAM_COLUMNDISABLED = "ColumnDisabled";
        public const string PARAM_COLUMNDISABLED0 = "ColumnDisabled0";
        public const string PARAM_COLUMNDISABLED1 = "ColumnDisabled1";
        public const string PARAM_COLUMNDISABLED2 = "ColumnDisabled2";
        public const string PARAM_COLUMNDISABLED3 = "ColumnDisabled3";
        public const string PARAM_COLUMNDISABLEDSPECIAL = "ColumnDisabledSpecial";
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
        static LCMSSettings()
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
        /// <param name="itemKey">Key for item to be retrieved</param>
        /// <returns>The value for the setting, or an empty string if the itemKey is not defined</returns>
        public static string GetParameter(string itemKey)
        {
            if (m_Settings.ContainsKey(itemKey))
                return m_Settings[itemKey];

            return string.Empty;
        }

        /// <summary>
        /// Retrieves specified item from string dictionary, converting it to a boolean
        /// </summary>
        /// <param name="itemKey">Key for item to be retrieved</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>
        /// The value for the setting, or defaultValue if the itemKey
        /// is not defined or if it cannot be converted to a boolean
        /// </returns>
        /// <remarks>If the value is an integer, will return false if 0 or true if non-zero</remarks>
        public static bool GetParameter(string itemKey, bool defaultValue)
        {
            if (m_Settings.ContainsKey(itemKey))
            {
                var valueText = m_Settings[itemKey];
                if (valueText != null)
                {
                    bool value;
                    if (bool.TryParse(valueText, out value))
                        return value;

                    int valueInt;
                    if (int.TryParse(valueText, out valueInt))
                    {
                        return valueInt != 0;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves specified item from string dictionary, converting it to an integer
        /// </summary>
        /// <param name="itemKey">Key for item to be retrieved</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>
        /// The value for the setting, or defaultValue if the itemKey
        /// is not defined or if it cannot be converted to an integer
        /// </returns>
        public static int GetParameter(string itemKey, int defaultValue)
        {
            if (m_Settings.ContainsKey(itemKey))
            {
                var valueText = m_Settings[itemKey];
                int value;
                if (valueText != null && int.TryParse(valueText, out value))
                {
                    return value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Retrieves specified item from string dictionary, converting it to a double
        /// </summary>
        /// <param name="itemKey">Key for item to be retrieved</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>
        /// The value for the setting, or defaultValue if the itemKey
        /// is not defined or if it cannot be converted to a double
        /// </returns>
        public static double GetParameter(string itemKey, double defaultValue)
        {
            if (m_Settings.ContainsKey(itemKey))
            {
                var valueText = m_Settings[itemKey];
                double value;
                if (valueText != null && double.TryParse(valueText, out value))
                {
                    return value;
                }
            }

            return defaultValue;
        }
        #endregion
    }
}
