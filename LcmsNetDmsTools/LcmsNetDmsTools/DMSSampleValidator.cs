using System;
using System.Collections.Generic;
using System.Text;
using LcmsNetData.Data;

namespace LcmsNetDmsTools
{
    [Flags]
    public enum DMSSampleValidatorErrors : int
    {
        /// <summary>
        /// No Error - it's good
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Usage type is not set
        /// </summary>
        UsageTypeNotSet = 1,

        /// <summary>
        /// Usage type is EMSL User, and the Proposal ID is empty
        /// </summary>
        EUSProposalIDEmpty = 2,

        /// <summary>
        /// Usage type is EMSL User, and the user list is empty
        /// </summary>
        EUSUserListEmpty = 4,

        /// <summary>
        /// LC Cart is not set
        /// </summary>
        LCCartNotSet = 8,

        /// <summary>
        /// LC Cart Config is not set
        /// </summary>
        LCCartConfigNotSet = 16,

        /// <summary>
        /// LC Cart Config is not set
        /// </summary>
        LCCartConfigNotValidForCart = 32,
    }

    /// <summary>
    /// Validates a sample.
    /// </summary>
    // Deprecated export: [Export(typeof(IDMSValidator))]
    // Deprecated export: [ExportMetadata("RelatedToolName", "PrismDMSTools")]
    // Deprecated export: [ExportMetadata("Version", "1.0")]
    // Deprecated export: [ExportMetadata("RequiredDMSToolVersion", "1.0")]
    public class DMSSampleValidator
    {
        public List<string> CartConfigNamesValidForCart = new List<string>();

        public DMSSampleValidator()
        {
        }

        public DMSSampleValidator(IEnumerable<string> cartConfigNamesValidForCart)
        {
            CartConfigNamesValidForCart.AddRange(cartConfigNamesValidForCart);
        }

        /// <summary>
        /// Indicates this item is tied to a EMSL user proposal that is not tied to a request in DMS.
        /// </summary>
        public const string CONST_EMSL_USAGE_TYPE = "USER";

        /// <summary>
        /// Validates a sample based on DMS criteria.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns>True if valid, false if invalid</returns>
        [Obsolete("Use IsSampleValidDetailed to report why it failed")]
        public bool IsSampleValid(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID != 0)
            {
                // The request is non-zero, no need to perform additional checks
                return true;
            }

            if (data.EMSLUsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
            {
                if (string.IsNullOrEmpty(data.EMSLProposalID.Replace(" ","")))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(data.EMSLProposalUser.Replace(" ","")))
                {
                    return false;
                }
            }
            else if (string.IsNullOrEmpty(data.EMSLUsageType.ToUpper()))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.CartName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.CartConfigName))
            {
                return false;
            }
            if (!CartConfigNamesValidForCart.Contains(data.CartConfigName))
            {
                return false;
            }

            // No errors; sample is valid
            return true;
        }

        /// <summary>
        /// Validates a sample based on DMS criteria.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns>True if valid, false if invalid</returns>
        public DMSSampleValidatorErrors IsSampleValidDetailed(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            //if (data.RequestID != 0)
            //{
            //    // The request is non-zero, no need to perform additional checks
            //    return DMSSampleValidatorErrors.NoError;
            //}

            var errors = DMSSampleValidatorErrors.NoError;

            if (data.EMSLUsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
            {
                if (string.IsNullOrEmpty(data.EMSLProposalID.Replace(" ", "")))
                {
                    errors |= DMSSampleValidatorErrors.EUSProposalIDEmpty;
                }
                if (string.IsNullOrEmpty(data.EMSLProposalUser.Replace(" ", "")))
                {
                    errors |= DMSSampleValidatorErrors.EUSUserListEmpty;
                }
            }
            else if (string.IsNullOrEmpty(data.EMSLUsageType.ToUpper()))
            {
                errors |= DMSSampleValidatorErrors.UsageTypeNotSet;
            }

            if (string.IsNullOrWhiteSpace(data.CartName))
            {
                errors |= DMSSampleValidatorErrors.LCCartNotSet;
            }
            if (string.IsNullOrWhiteSpace(data.CartConfigName))
            {
                errors |= DMSSampleValidatorErrors.LCCartConfigNotSet;
            }
            else if (!CartConfigNamesValidForCart.Contains(data.CartConfigName))
            {
                // Don't want to add this error if the LCCartConfig is not set at all
                errors |= DMSSampleValidatorErrors.LCCartConfigNotValidForCart;
            }

            // No errors; sample is valid
            return errors;
        }

        public string CreateErrorListFromErrors(DMSSampleValidatorErrors errors)
        {
            var errorDetails = new StringBuilder();
            if (errors == DMSSampleValidatorErrors.NoError)
            {
                return "";
            }

            if (errors.HasFlag(DMSSampleValidatorErrors.UsageTypeNotSet))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("Usage type is not set for sample. Set Usage Type. (DMS Edit)");
            }
            if (errors.HasFlag(DMSSampleValidatorErrors.EUSProposalIDEmpty))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("EUS Proposal ID is empty. Provide a Proposal ID. (DMS Edit)");
            }
            if (errors.HasFlag(DMSSampleValidatorErrors.EUSUserListEmpty))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("EUS User list is empty. Provide a User list. (DMS Edit)");
            }
            if (errors.HasFlag(DMSSampleValidatorErrors.LCCartNotSet))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("LC Cart is not set. Set LC Cart. (Configuration)");
            }
            if (errors.HasFlag(DMSSampleValidatorErrors.LCCartConfigNotSet))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("LC Cart Config is not set. Set LC Cart Config.");
            }
            if (errors.HasFlag(DMSSampleValidatorErrors.LCCartConfigNotValidForCart))
            {
                if (errorDetails.Length > 0)
                {
                    errorDetails.Append('\n');
                }
                errorDetails.Append("LC Cart Config is not valid for this cart. Change LC Cart Config.");
            }

            return errorDetails.ToString();
        }

        /// <summary>
        /// Validates EUS Proposal ID
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLProposalIDValid(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (data.EMSLUsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
                {
                    if (string.IsNullOrEmpty(data.EMSLProposalID.Replace(" ","")))
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
        public static bool IsEMSLUserValid(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (data.EMSLUsageType.ToUpper() == CONST_EMSL_USAGE_TYPE)
                {
                    if (string.IsNullOrEmpty(data.EMSLProposalUser.Replace(" ","")))
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
        public static bool IsEMSLUsageTypeValid(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (string.IsNullOrEmpty(data.EMSLUsageType.ToUpper()))
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
        public static bool IsExperimentNameValid(IRequestedRunData sample)
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

        /// <summary>
        /// Validate cart config setting
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsCartConfigValid(IRequestedRunData sample)
        {
            var data = sample.DmsData;

            if (data.RequestID == 0)
            {
                if (string.IsNullOrWhiteSpace(data.CartConfigName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
