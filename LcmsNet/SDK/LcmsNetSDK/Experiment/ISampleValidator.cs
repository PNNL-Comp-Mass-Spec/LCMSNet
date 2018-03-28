using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Experiment
{
    public interface ISampleValidator
    {
        List<SampleData> ValidateBlocks(
            List<SampleData> samples);

        List<SampleValidationError> ValidateSamples(
            SampleData sample);
    }
}