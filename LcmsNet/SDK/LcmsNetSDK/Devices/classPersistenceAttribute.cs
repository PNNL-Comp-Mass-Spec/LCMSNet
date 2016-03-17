using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Attribute used to save a device's settings
    /// </summary>
    public class classPersistenceAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingName"></param>
        public classPersistenceAttribute(string settingName)
        {
            SettingName = settingName;
        }

        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string SettingName { get; private set; }
    }
}