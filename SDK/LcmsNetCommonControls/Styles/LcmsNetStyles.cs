using System;
using System.Windows;

namespace LcmsNetCommonControls.Styles
{
    /// <summary>
    /// Class that is reference-able from XAML for referring to styles defined in this assembly
    /// </summary>
    public class LcmsNetStyles : StyleRefExtension
    {
        static LcmsNetStyles()
        {
            ResDict = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/LcmsNetCommonControls;component/Styles/CombinedStyles.xaml")
            };
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LcmsNetStyles()
        {
        }

        /// <summary>
        /// Constructor that takes a resource key
        /// </summary>
        /// <param name="resourceKey"></param>
        public LcmsNetStyles(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// Get the given resource as a style
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns>style if the resource key refers to a style; an empty style if it doesn't.</returns>
        public static Style GetStyle(string resourceKey)
        {
            return GetStyleBase(resourceKey);
        }
    }
}
