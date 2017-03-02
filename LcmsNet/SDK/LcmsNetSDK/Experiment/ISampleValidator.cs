namespace LcmsNetDataClasses.Experiment
{
    public interface ISampleValidator
    {
        System.Collections.Generic.List<classSampleData> ValidateBlocks(
            System.Collections.Generic.List<classSampleData> samples);

        System.Collections.Generic.List<classSampleValidationError> ValidateSamples(
            classSampleData sample);
    }
}