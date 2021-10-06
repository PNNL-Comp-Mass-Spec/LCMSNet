using System.ComponentModel;

namespace LcmsNetSDK.Experiment
{
    public interface ISampleValidatorMetaData
    {
        string Name { get; }

        [DefaultValue("1.0")]
        string Version { get; }
    }
}