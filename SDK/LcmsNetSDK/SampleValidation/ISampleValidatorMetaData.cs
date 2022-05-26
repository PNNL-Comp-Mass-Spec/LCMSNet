using System.ComponentModel;

namespace LcmsNetSDK.SampleValidation
{
    public interface ISampleValidatorMetaData
    {
        string Name { get; }

        [DefaultValue("1.0")]
        string Version { get; }
    }
}