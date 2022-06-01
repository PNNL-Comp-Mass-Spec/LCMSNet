using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Attribute used to save a device's settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DeviceSavedSettingAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingName"></param>
        public DeviceSavedSettingAttribute(string settingName)
        {
            SettingName = settingName;
        }

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string SettingName { get; }
    }
}
