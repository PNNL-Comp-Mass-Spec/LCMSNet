using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Logging
{
    public interface ILogger
    {
        void LogError(int errorLevel, string message, Exception ex, classSampleData sample);
        void LogError(int errorLevel, string message, Exception ex);
        void LogError(int errorLevel, string message, classSampleData sample);
        void LogError(int errorLevel, string message);
        void LogMessage(int messageLevel, string message);
        void LogMessage(int messageLevel, string message, classSampleData sample);
    }
}
