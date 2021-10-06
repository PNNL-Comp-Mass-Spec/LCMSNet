namespace LcmsNetSDK.Experiment
{
    /// <summary>
    /// Holds information regarding the type of error that
    /// </summary>
    public class SampleValidationError
    {
        public SampleValidationError(string error, SampleValidationErrorType validationError)
        {
            Error = error;
            ValidationErrorType = validationError;
        }

        public string Error { get; set; }
        public SampleValidationErrorType ValidationErrorType { get; set; }

        public override string ToString()
        {
            return $"{ValidationErrorType.ToString()}: {Error}";
        }
    }
}