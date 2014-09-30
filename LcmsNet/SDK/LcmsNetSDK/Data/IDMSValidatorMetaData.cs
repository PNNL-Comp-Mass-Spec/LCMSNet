using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LcmsNetDataClasses.Data
{
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
