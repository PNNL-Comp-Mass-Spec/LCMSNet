using System;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Markup;
namespace LcmsNetCommonControls.Styles
{
    /// <summary>
    /// Base class for styles that need to be used in other assemblies
    /// </summary>
    /// <remarks>https://www.codeproject.com/Tips/542018/Simply-Using-WPF-Styles-in-Other-Assemblies-or-Oth</remarks>
    public abstract class StyleRefExtension : MarkupExtension
    {
        /// <summary>
        /// Property for specific resource dictionary
        /// </summary>
        protected static ResourceDictionary ResDict;

        /// <summary>
        /// Resource key which we want to extract
        /// </summary>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Overriding base function which will return key from RD
        /// </summary>
        /// <param name="serviceProvider">Not used</param>
        /// <returns>Object from RD</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ResDict == null)
            {
                throw new Exception(@"You should define ResDict before usage.\nPlease make it in static constructor of extending class!");
            }

            return ResDict[ResourceKey];
        }

        /// <summary>
        /// Get the given resource as a style
        /// </summary>
        /// <returns>style if the resource key refers to a style; an empty style if it doesn't.</returns>
        public Style GetStyle()
        {
            var obj = ProvideValue(new ServiceContainer());
            if (obj is Style s)
            {
                return s;
            }

            return new Style();
        }

        /// <summary>
        /// Get the given resource as a style
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns>style if the resource key refers to a style; an empty style if it doesn't.</returns>
        protected static Style GetStyleBase(string resourceKey)
        {
            if (ResDict == null)
            {
                throw new Exception(@"You should define ResDict before usage.\nPlease make it in static constructor of extending class!");
            }

            var obj = ResDict[resourceKey];

            if (obj is Style s)
            {
                return s;
            }

            return new Style();
        }
    }
}
