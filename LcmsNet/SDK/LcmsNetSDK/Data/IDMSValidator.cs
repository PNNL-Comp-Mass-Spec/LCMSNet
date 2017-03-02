using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNetDataClasses.Data
{
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