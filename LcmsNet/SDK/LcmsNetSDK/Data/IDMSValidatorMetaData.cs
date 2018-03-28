using System;
using System.ComponentModel;

namespace LcmsNetSDK.Data
{
    [Obsolete("Interface deprecated. Use a direct reference to classDMSSampleValidator.cs")]
    public interface IDMSValidatorMetaData
    {
        /// <summary>
        /// Gives the name of the DMS tool this validator is related to.
        /// </summary>
        string RelatedToolName { get; }

        /// <summary>
        /// Version of the Validator.
        /// </summary>
        [DefaultValue("1.0")]
        string Version { get; }

        /// <summary>
        /// The minimum version of the DMS tool that this validator works with.
        /// </summary>
        [DefaultValue("1.0")]
        string RequiredDMSToolVersion { get; }
    }
}