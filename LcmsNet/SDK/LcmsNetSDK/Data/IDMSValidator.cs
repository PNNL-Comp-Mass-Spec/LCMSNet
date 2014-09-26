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
        ///  Determines if sample is valid for DMS
        /// </summary>
        /// <returns>true/false</returns>
        bool IsSampleValid(classSampleData sample);
        /// <summary>
        /// The Type of a System.Windows.Forms UserControl derived control for DMS information for a single sample.
        /// </summary>
        Type DMSValidatorControl { get; } 
    }
}
