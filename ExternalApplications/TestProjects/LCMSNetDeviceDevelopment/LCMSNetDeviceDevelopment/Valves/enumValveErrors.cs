using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.Valves
{
    public enum enumValveErrors
    {
        Success = 0,
        UnauthorizedAccess,
        TimeoutDuringRead,
        TimeoutDuringWrite,
        ValvePositionMismatch,
        BadArgument,
        UndefinedPort
    }
}
