using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.Valves
{
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
