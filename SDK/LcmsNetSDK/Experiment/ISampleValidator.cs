using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Experiment
{
    public interface ISampleValidator
    {
        List<SampleValidationError> ValidateSamples(
            ISampleInfo sample);
    }
}