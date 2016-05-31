using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
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
