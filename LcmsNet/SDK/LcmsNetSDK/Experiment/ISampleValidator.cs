using System;

namespace LcmsNetDataClasses.Experiment
{
    public interface ISampleValidator
    {
        System.Collections.Generic.List<LcmsNetDataClasses.classSampleData> ValidateBlocks(
            System.Collections.Generic.List<LcmsNetDataClasses.classSampleData> samples);

        System.Collections.Generic.List<classSampleValidationError> ValidateSamples(
            LcmsNetDataClasses.classSampleData sample);
    }
}