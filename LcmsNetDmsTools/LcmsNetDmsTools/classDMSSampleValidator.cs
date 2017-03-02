using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Experiment;

namespace LcmsNetDmsTools
{
    /// <summary>
    /// Validates a sample.
    /// </summary>
    [Export(typeof(IDMSValidator))]
    [ExportMetadata("RelatedToolName", "PrismDMSTools")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("RequiredDMSToolVersion", "1.0")]
    public class classDMSSampleValidator:IDMSValidator
    {
        /// <summary>
        /// Indicates not request number is tied to a EMSL user proposal in DMS.
        /// </summary>
        public const int CONST_EMSL_REQUEST_CHECK = 0;
        /// <summary>
        /// Indicates this item is tied to a EMSL user proposal that is not tied to a request in DMS.
        /// </summary>
        public const string CONST_EMSL_USAGE_TYPE = "USER";

        /// <summary>
        /// Validates a sample based on DMS criteria.
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public bool IsSampleValid(classSampleData sample)
        {
            var errors = new List<classSampleValidationError>();

            var data = sample.DmsData;

            // If the request is zero, then we have to perform some checks
            if (data.RequestID != CONST_EMSL_REQUEST_CHECK)
            {
                return errors.Count > 0;
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
            return errors.Count > 0;
        }        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLProposalIDValid(classSampleData sample)
        {            
            var data = sample.DmsData;
            
            if (data.RequestID == CONST_EMSL_REQUEST_CHECK)
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
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLUserValid(classSampleData sample)
        {            
            var data = sample.DmsData;
            
            if (data.RequestID == CONST_EMSL_REQUEST_CHECK)
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
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsEMSLUsageTypeValid(classSampleData sample)
        {            
            var data = sample.DmsData;
            
            if (data.RequestID == CONST_EMSL_REQUEST_CHECK)
            {                
                if (string.IsNullOrEmpty(data.UsageType.ToUpper()))
                {
                    return false;
                }
            }   
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static bool IsExperimentNameValid(classSampleData sample)
        {            
            var data = sample.DmsData;
            
            if (data.RequestID == CONST_EMSL_REQUEST_CHECK)
            {                
                if (string.IsNullOrEmpty(data.Experiment.ToUpper()))
                {
                    return false;
                }
            }   
            return true;
        }
     
        public Type DMSValidatorControl
        {
            get
            {
                return typeof(controlDMSValidator);
            }
        }    
    }
}
