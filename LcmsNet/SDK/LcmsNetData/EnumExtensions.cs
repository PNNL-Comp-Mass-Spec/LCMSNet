using System;
using System.ComponentModel;
using System.Linq;

namespace LcmsNetData
{
    /// <summary>
    /// Extensions for enum types
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the description from the description attribute of the enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Description from description attribute; if it doesn't exist, returns <paramref name="value"/>.ToString()</returns>
        public static string GetEnumDescription(this Enum value)
        {
            //https://stackoverflow.com/questions/20290842/converter-to-show-description-of-an-enum-and-convert-back-to-enum-value-on-sele
            var description = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;

            return description?.Description ?? value.ToString();
        }
    }
}
