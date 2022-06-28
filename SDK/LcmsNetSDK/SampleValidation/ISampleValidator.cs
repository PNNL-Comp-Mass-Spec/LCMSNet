using System.Collections.Generic;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.SampleValidation
{
    public interface ISampleValidator
    {
        List<SampleValidationError> ValidateSamples(
            ISampleInfo sample);
    }
}
