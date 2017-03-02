using System.ComponentModel;

namespace LcmsNetDataClasses.Experiment
{
    public interface ISampleValidatorMetaData
    {
        string Name { get; }

        [DefaultValue("1.0")]
        string Version { get; }
    }
}