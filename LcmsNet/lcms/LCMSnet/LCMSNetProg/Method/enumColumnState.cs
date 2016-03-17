using System;
using System.Threading;
using System.Collections.Generic;
using LcmsNet.Method;
using LcmsNet.SampleQueue;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Configuration;

namespace LcmsNet.Method
{
    /// <summary>
    /// State enumeration data.
    /// </summary>
    public enum enumColumnState
    {
        Idle,
        Running,
        Error,
        DisabledProgram,
        DisabledUser
    };
}