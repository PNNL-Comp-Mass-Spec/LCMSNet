using System;
using System.Collections.Generic;

namespace LcmsNetSDK.Method
{
    public interface ILCMethod : ICloneable
    {
        string Name { get; set; }
        IReadOnlyList<ILCEvent> Events { get; }
        DateTime Start { get; }
        DateTime End { get; }
        int Column { get; }
    }
}
