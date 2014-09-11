using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
