//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.Valves
{
    /// <summary>
    /// Unauthorized access exception - the port is probably used by something else.
    /// </summary>
    public class ValveExceptionUnauthorizedAccess : Exception
    {
        public ValveExceptionUnauthorizedAccess()
            : base()
        {
        }
        public ValveExceptionUnauthorizedAccess(string message)
            : base(message)
        {
        }
        public ValveExceptionUnauthorizedAccess(string message, Exception ex)
            : base(message, ex)
        {
        }

    }

    /// <summary>
    /// Read timeout exception - the device is probably disconnected
    /// </summary>
    public class ValveExceptionReadTimeout : Exception
    {
        public ValveExceptionReadTimeout()
            : base()
        {
        }
        public ValveExceptionReadTimeout(string message)
            : base(message)
        {
        }
        public ValveExceptionReadTimeout(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    /// <summary>
    /// Write timeout exception - the device is probably disconnected
    /// </summary>
    public class ValveExceptionWriteTimeout : Exception
    {
        public ValveExceptionWriteTimeout()
            : base()
        {
        }
        public ValveExceptionWriteTimeout(string message)
            : base(message)
        {
        }
        public ValveExceptionWriteTimeout(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    /// <summary>
    /// Position mismatch exception - the setPosition command was unsuccessful
    /// </summary>
    public class ValveExceptionPositionMismatch : Exception
    {
        public ValveExceptionPositionMismatch()
            : base()
        {
        }
        public ValveExceptionPositionMismatch(string message)
            : base(message)
        {
        }
        public ValveExceptionPositionMismatch(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
