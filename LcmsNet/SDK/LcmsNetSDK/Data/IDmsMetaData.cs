using System;
using System.ComponentModel;

namespace LcmsNetDataClasses
{
    [Obsolete("classDMSToolsManager is deprecated")]
    public interface IDmsMetaData
    {
        string Name { get; }

        [DefaultValue("1.0")]
        string Version { get; }
    }
}