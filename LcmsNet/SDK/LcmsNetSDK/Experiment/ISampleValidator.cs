using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Experiment
{
    public interface ISampleValidator
    {
        List<classSampleData> ValidateBlocks(
            List<classSampleData> samples);

        List<classSampleValidationError> ValidateSamples(
            classSampleData sample);
    }
}