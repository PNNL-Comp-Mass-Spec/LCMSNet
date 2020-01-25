using System.Collections.Generic;

namespace LcmsNetSQLiteTools
{
    public static class ExtensionMethods
    {
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
