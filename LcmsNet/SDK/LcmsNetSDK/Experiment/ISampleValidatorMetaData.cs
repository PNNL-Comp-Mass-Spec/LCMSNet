using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace LcmsNetDataClasses.Experiment
{
    public interface ISampleValidatorMetaData
    {
        string Name { get; }

        [DefaultValue("1.0")]
        string Version { get; }
    }
}