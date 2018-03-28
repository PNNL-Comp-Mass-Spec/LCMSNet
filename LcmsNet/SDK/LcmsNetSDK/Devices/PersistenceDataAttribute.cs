using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Attribute used to save a device's settings
    /// </summary>
    public class PersistenceDataAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingName"></param>
        public PersistenceDataAttribute(string settingName)
        {
            SettingName = settingName;
        }

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string SettingName { get; private set; }
    }
}