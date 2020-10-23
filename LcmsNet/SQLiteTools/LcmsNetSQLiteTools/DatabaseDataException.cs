using System;

// ReSharper disable UnusedMember.Global

namespace LcmsNetSQLiteTools
{
    /// <summary>
    ///  Custom exception for reporting problems during a database query
    /// </summary>
    public class DatabaseDataException : Exception
    {
        public DatabaseDataException(string message, Exception ex) :
            base(message, ex)
        {
        }

        public DatabaseDataException()
        {
        }

        public DatabaseDataException(string message) : base(message)
        {
        }
    }
}
