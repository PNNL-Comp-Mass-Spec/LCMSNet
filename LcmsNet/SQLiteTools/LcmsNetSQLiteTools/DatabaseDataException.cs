using System;

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
    }
}
