using System.Collections.Generic;

namespace LcmsNetDataClasses.Experiment
{
    public interface ISampleValidator
    {
        List<classSampleData> ValidateBlocks(
            List<classSampleData> samples);

        List<classSampleValidationError> ValidateSamples(
            classSampleData sample);
    }
}