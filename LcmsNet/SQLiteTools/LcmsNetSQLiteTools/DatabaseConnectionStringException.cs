using System;

namespace LcmsNetSQLiteTools
{
    /// <summary>
    /// Custom exception for reporting invalid database connection string
    /// </summary>
    public class DatabaseConnectionStringException : Exception
    {        public DatabaseConnectionStringException(string message) :
            base(message)
        {
        }
    }
}
