using System;
using System.ComponentModel;

namespace LcmsNetDataClasses
{
    public interface IDmsMetaData
    {
        string Name { get; }
        [DefaultValue("1.0")]
        string Version { get; }
    }
}
