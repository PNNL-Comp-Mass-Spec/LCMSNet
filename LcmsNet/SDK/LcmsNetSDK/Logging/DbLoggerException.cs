using System;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Custom exception for database logging problems
    /// </summary>
    class DbLoggerException : Exception
    {
        #region "Constructors"

        public DbLoggerException(string message, Exception ex) :
            base(message, ex)
        {
        }

        #endregion
    }
}