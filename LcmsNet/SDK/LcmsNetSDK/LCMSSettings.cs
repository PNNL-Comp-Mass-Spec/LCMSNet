using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using LcmsNetSDK.System;

namespace LcmsNetSDK
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
        public const string PARAM_DMSPWD = "DMSPwd";
        public const string PARAM_DMSTOOL = "DMSTool";
        public const string PARAM_DMSVERSION = "DMSVersion";
        public const string PARAM_EMULATIONENABLED = "EmulationEnabled";
        public const string PARAM_ERRORPATH = "ErrorPath";
        public const string PARAM_INITIALIZEHARDWAREONSTARTUP = "InitializeHardwareOnStartup";
        public const string PARAM_LOGGINGERRORLEVEL = "LoggingErrorLevel";
        public const string PARAM_LOGGINGMSGLEVEL = "LoggingMsgLevel";
        public const string PARAM_MINIMUMVOLUME = "MinimumVolume";
        public const string PARAM_OPERATOR = "Operator";
        public const string PARAM_PALMETHODSFOLDER = "PalMethodsFolder";
        public const string PARAM_PDFPATH = "PdfPath";
        public const string PARAM_SEPARATIONTYPE = "SeparationType";
        public const string PARAM_TIMEZONE = "TimeZone";
        public const string PARAM_TRIGGERFILEFOLDER = "TriggerFileFolder";
        public const string PARAM_LocalDataPath = "LocalDataPath";

        /// <summary>
        /// String dictionary to hold settings data
        /// </summary>
        static readonly Dictionary<string, string> m_Settings;

        public static event EventHandler<SettingChangedEventArgs> SettingChanged;

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

        /// <summary>
        /// Loads settings into <see cref="LCMSSettings"/>, with some settings allowed to override others. All logging is to the returned list.
        /// </summary>
        /// <param name="settings">Collection of settings for the application, usually the Application default-style 'Settings' class</param>
        /// <param name="overridingSettings">Collection of settings that override the settings in <paramref name="settings"/>, usually from something like 'ConfigurationManager.AppSettings' (this can come from a file specified in 'app.config')</param>
        /// <param name="allowOverridingUserSettings">If false, <paramref name="overridingSettings"/> entries that override user-scope settings will be ignored and reported via the returned list. If true, they will override user-scope settings.</param>
        /// <returns>Errors/log entries that occurred while loading settings</returns>
        /// <remarks>
        /// Some examples of sources for <paramref name="overridingSettings"/>:<br />
        /// in <c>app.config/configuration/configSections</c>:<br />
        /// <code>
        /// &lt;section name="developerAppSettings" type="System.Configuration.NameValueFileSectionHandler, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/&gt;<br />
        /// </code>
        /// <br />
        /// in <c>app.config/configuration</c>:<br />
        /// <code>
        /// &lt;appSettings file="LcmsNet_PersistentSettings.config"/&gt;<br />
        /// &lt;developerAppSettings file="LcmsNet_DeveloperSettings.config"/&gt;<br />
        /// </code>
        ///<br />
        /// <c>LcmsNet_PersistentSettings.config</c>:<br />
        /// <code>
        /// &lt;appSettings&gt;<br />
        ///   &lt;add key="CartName" value="No_Cart" /&gt;<br />
        /// &lt;/appSettings&gt;<br />
        /// </code>
        ///<br />
        /// <c>LcmsNet_DeveloperSettings.config</c>:<br />
        /// <code>
        /// &lt;developerAppSettings&gt;<br />
        ///   &lt;add key="CartName" value="(none)" /&gt;<br />
        /// &lt;/developerAppSettings&gt;<br />
        /// </code>
        ///<br />
        /// To load them into the program:<br />
        /// <code>
        /// // System.Configuration.ConfigurationManager: Will need to add an assembly reference<br />
        /// var persistentSettings = System.Configuration.ConfigurationManager.AppSettings;<br />
        /// var devSettings = (NameValueCollection)(ConfigurationManager.GetSection("developerAppSettings"));<br />
        /// // ... (Choosing logic for using persistentSettings or devSettings)<br />
        /// </code>
        /// </remarks>
        public static List<Tuple<string, Exception>> LoadSettings(ApplicationSettingsBase settings, NameValueCollection overridingSettings = null, bool allowOverridingUserSettings = false)
        {
            var loadErrors = new List<Tuple<string, Exception>>();

            if (overridingSettings != null)
            {
                // Settings: There are the default *.exe.config application config and [user\appdata\local\...\user.config] user config files,
                // but some settings apply to the whole system (application config, *.exe.config), yet shouldn't be replaced when we install
                // a new version of the program (but the application config file has other important configuration information that should be replaced).
                // To properly handle this, these settings are specified in the application configuration, but we use a different file (specified in
                // app.config, the appSettings file 'LcmsNet_PersistentSettings.config') to overrule the matching settings in the application config file.

                var mainSettings = settings.Properties.Cast<SettingsProperty>().ToList();
                foreach (var setting in overridingSettings.AllKeys)
                {
                    // Can't add new settings, and we don't want to override user-scope settings
                    var settingValue = overridingSettings[setting];
                    var match = mainSettings.First(x => x.Name.Equals(setting));
                    if (match == null)
                    {
                        // ApplicationLogger.LogError(0, $"Could not apply persistent setting '{setting}' with value '{settingValue}': Setting does not exist.");
                        loadErrors.Add(new Tuple<string, Exception>($"Could not apply persistent setting '{setting}' with value '{settingValue}': Setting does not exist.", null));
                    }
                    else if (match.Attributes.Contains(typeof(ApplicationScopedSettingAttribute)) || allowOverridingUserSettings)
                    {
                        // By default only overriding of application-scope settings is allowed, but allowing overriding of user-scope settings is useful in certain instances (during development is an example)
                        try
                        {
                            var changedTypeValue = Convert.ChangeType(settingValue, match.PropertyType);
                            settings[setting] = changedTypeValue;
                        }
                        catch (Exception e)
                        {
                            // Don't use ApplicationLogger yet - load the settings first to load the possibly user-modified local logging path.
                            //ApplicationLogger.LogError(0, $"Could not apply persistent setting '{setting}' with value '{settingValue}' to type '{match.PropertyType.FullName}'", e);
                            loadErrors.Add(new Tuple<string, Exception>($"Could not apply persistent setting '{setting}' with value '{settingValue}' to type '{match.PropertyType.FullName}'", e));
                        }
                    }
                    else
                    {
                        // Don't use ApplicationLogger yet - load the settings first to load the possibly user-modified local logging path.
                        // ApplicationLogger.LogError(0, $"Could not apply persistent setting '{setting}' with value '{settingValue}': Cannot override a user-scope setting.");
                        loadErrors.Add(new Tuple<string, Exception>($"Could not apply persistent setting '{setting}' with value '{settingValue}': Cannot override a user-scope setting.", null));
                    }
                }
            }

            var propColl = settings.Properties;
            foreach (SettingsProperty currProperty in propColl)
            {
                var propertyName = currProperty.Name;
                var propertyValue = settings[propertyName].ToString();
                SetParameter(propertyName, propertyValue);
            }

            // Add path to executable as a saved setting
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                loadErrors.Add(new Tuple<string, Exception>("Settings loading: Could not determine the entry assembly; cannot store path to application", null));
            }
            else
            {
                var exeInfo = new FileInfo(entryAssembly.Location);
                SetParameter(PARAM_APPLICATIONPATH, exeInfo.DirectoryName);
            }

            // Assure that parameter ApplicationDataPath is defined
            SetParameter(PARAM_APPLICATIONDATAPATH, PersistDataPaths.LocalDataPath);

            return loadErrors;
        }
    }
}
