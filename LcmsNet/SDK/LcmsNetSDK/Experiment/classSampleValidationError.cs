using System;

namespace LcmsNetDataClasses.Experiment
{
    /// <summary>
    /// Holds information regarding the type of error that 
    /// </summary>
    public class classSampleValidationError
    {
        public classSampleValidationError(string error, enumSampleValidationError validationError)
        {
            Error = error;
            ValidationErrorType = validationError;
        }

        public string Error { get; set; }
        public enumSampleValidationError ValidationErrorType { get; set; }
    }
}