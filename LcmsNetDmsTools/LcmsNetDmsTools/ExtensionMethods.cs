using System;
using System.Collections.Generic;

namespace LcmsNetDmsTools
{
    /// <summary>
    /// Extension methods static class for LcmsNetDmsTools
    /// </summary>
    internal static class ExtensionMethods
    {
        // Ignore Spelling: Lcms, de-duplication

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

        /// <summary>
        /// Performs memory de-duplication of strings, since database reads can give us a large number of duplicated strings
        /// </summary>
        /// <param name="input"></param>
        /// <param name="deDuplicationDictionary"></param>
        /// <returns></returns>
        public static string LimitStringDuplication(this string input,
            Dictionary<string, string> deDuplicationDictionary)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            if (deDuplicationDictionary.TryGetValue(input, out var match))
            {
                return match;
            }

            deDuplicationDictionary.Add(input, input);
            return input;
        }
    }
}
