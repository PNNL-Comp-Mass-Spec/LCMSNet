using System;

// ReSharper disable UnusedMember.Global

namespace LcmsNetSQLiteTools
{
    /// <summary>
    /// Custom exception for reporting invalid database connection string
    /// </summary>
    public class DatabaseConnectionStringException : Exception
    {
        public DatabaseConnectionStringException(string message) : base(message)
        {
        }

        public DatabaseConnectionStringException()
        {
        }

        public DatabaseConnectionStringException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
