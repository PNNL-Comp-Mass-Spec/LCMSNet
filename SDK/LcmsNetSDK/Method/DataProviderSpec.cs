using System;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.Method
{
    /// <summary>
    /// Specification for a Data Provider for an LC Method Event
    /// </summary>
    public readonly struct DataProviderSpec
    {
        /// <summary>
        /// True if this instance was created with valid parameters
        /// </summary>
        public bool IsSet { get; }

        /// <summary>
        /// The name/key of the data provider that provides data for a given control. If not empty string, method's class must implement <see cref="IHasDataProvider"/>
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Index of the method parameter that expects a value supplied by the data provider
        /// </summary>
        public int ParamIndex { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">The name/key of the data provider that provides data for a given control. If not empty string, method's class must implement <see cref="IHasDataProvider"/></param>
        /// <param name="paramIndex">The data provider parameter index, for the method the attribute is applied to</param>
        public DataProviderSpec(string key, int paramIndex)
        {
            if (string.IsNullOrWhiteSpace(key) || paramIndex < 0)
            {
                throw new Exception("Data provider key must be not null and not blank, and paramIndex must not be negative!");
            }

            IsSet = true;
            Key = key;
            ParamIndex = paramIndex;
        }
    }
}
