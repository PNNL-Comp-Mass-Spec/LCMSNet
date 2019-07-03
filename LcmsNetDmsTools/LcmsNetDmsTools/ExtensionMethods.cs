using System;

namespace LcmsNetDmsTools
{
    /// <summary>
    /// Extension methods static class for LcmsNetDmsTools
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Simple cast that handles DBNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>If value is DBNull, then returns default(t); otherwise casts value to T</returns>
        public static T CastDBValTo<T>(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }

            return (T) value;
        }

        /// <summary>
        /// Conversion helper that handles DBNull
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="conversionFunction">Function to convert the value to T (should handle null)</param>
        /// <returns>Converted value</returns>
        public static T ConvertDBNull<T>(object value, Func<object, T> conversionFunction)
        {
            return conversionFunction(value == DBNull.Value ? null : value);
        }
    }
}
