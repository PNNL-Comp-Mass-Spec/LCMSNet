using System;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Unauthorized access exception - serial port is probably being used by something else
    /// </summary>
    public class IscoExceptionUnauthroizedAccess : Exception
    {
        public IscoExceptionUnauthroizedAccess()
            : base()
        {
        }

        public IscoExceptionUnauthroizedAccess(string msg)
            : base(msg)
        {
        }

        public IscoExceptionUnauthroizedAccess(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }

    /// <summary>
    /// Read timeout exception - the device is probably disconnected
    /// </summary>
    public class IscoExceptionReadTimeout : Exception
    {
        public IscoExceptionReadTimeout()
            : base()
        {
        }

        public IscoExceptionReadTimeout(string msg)
            : base(msg)
        {
        }
        public IscoExceptionReadTimeout(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }

    /// <summary>
    /// Write timeout exception - the device is probably disconnected
    /// </summary>
    public class IscoExceptionWriteTimeout : Exception
    {
        public IscoExceptionWriteTimeout()
            : base()
        {
        }

        public IscoExceptionWriteTimeout(string msg)
            : base(msg)
        {
        }
        public IscoExceptionWriteTimeout(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }

    /// <summary>
    /// Error message received from pump
    /// </summary>
    public class IscoExceptionMessageError : Exception
    {
        public IscoExceptionMessageError()
            : base()
        {
        }

        public IscoExceptionMessageError(string msg)
            : base(msg)
        {
        }

        public IscoExceptionMessageError(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }

    /// <summary>
    /// Generic ISCO pump exception
    /// </summary>
    public class IscoException : Exception
    {
        public IscoException()
            : base()
        {
        }

        public IscoException(string msg)
            : base(msg)
        {
        }

        public IscoException(string msg, Exception ex)
            : base(msg, ex)
        {
        }
    }
}
