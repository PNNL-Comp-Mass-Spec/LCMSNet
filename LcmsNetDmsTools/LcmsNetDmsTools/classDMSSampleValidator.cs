using System;
// Deprecated: using System.ComponentModel.Composition;
using LcmsNetDataClasses;
// Deprecated: using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;

namespace LcmsNetDmsTools
{
    /// <summary>
    /// Validates a sample.
    /// </summary>
    // Deprecated export: [Export(typeof(IDMSValidator))]
    // Deprecated export: [ExportMetadata("RelatedToolName", "PrismDMSTools")]
    // Deprecated export: [ExportMetadata("Version", "1.0")]
    // Deprecated export: [ExportMetadata("RequiredDMSToolVersion", "1.0")]
    public class classDMSSampleValidator
    {

        /// <summary>
        /// Indicates this item is tied to a EMSL user proposal that is not tied to a request in DMS.
        /// </summary>
        public const string CONST_EMSL_USAGE_TYPE = "USER";

        /// <summary>
        /// Validates a sample based on DMS criteria.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns>True if valid, false if invalid</returns>
        public bool IsSampleValid(classSampleData sample)
        {

            var data = sample.DmsData;

            if (data.RequestID != 0)
            {
                // The request is non-zero, no need to perform additional checks
                return true;
            }

            if (data.UsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
            {
                if (string.IsNullOrEmpty(data.ProposalID.Replace(" ","")))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(data.UserList.Replace(" ","")))
                {
                    return false;
                }
            }
            else if (string.IsNullOrEmpty(data.UsageType.ToUpper()))
            {
                return false;
            }

            // No errors; sample is valid
            return true;
        }

        /// <summary>
        /// Validates EUS Proposal ID
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLProposalIDValid(classSampleData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (data.UsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
                {
                    if (string.IsNullOrEmpty(data.ProposalID.Replace(" ","")))
                    {
                     return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Validate EUS Users
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLUserValid(classSampleData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (data.UsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
                {
                    if (string.IsNullOrEmpty(data.UserList.Replace(" ","")))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Validate EUS Usage
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLUsageTypeValid(classSampleData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (string.IsNullOrEmpty(data.UsageType.ToUpper()))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validate experiment name
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsExperimentNameValid(classSampleData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (string.IsNullOrEmpty(data.Experiment.ToUpper()))
                {
                    return false;
                }
            }
            return true;
        }

        public Type DMSValidatorControl => typeof(controlDMSValidator);
    }
}
