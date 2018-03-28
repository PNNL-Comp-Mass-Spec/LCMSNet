using System;

namespace LcmsNetSDK.Data
{
    [Obsolete("Interface deprecated. Use a direct reference to classDMSSampleValidator.cs")]
    public interface IDMSValidator
    {
        /// <summary>
        /// The Type of a classDMSBaseControl derived control for DMS information for a single sample.
        /// </summary>
        Type DMSValidatorControl { get; }

        /// <summary>
        ///  Determines if sample is valid for DMS
        /// </summary>
        /// <returns>true/false</returns>
        bool IsSampleValid(classSampleData sample);
    }
}