using System;

namespace LcmsNetPlugins.VICI.Valves
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
