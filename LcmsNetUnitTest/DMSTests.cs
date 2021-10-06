using System;
using LcmsNet.IO.DMS;
using LcmsNetSDK.Logging;
using NUnit.Framework;

namespace LcmsnetUnitTest
{
    public class DMSTests
    {
        [Test]
        public void TestDMSDataLoading()
        {
            ApplicationLogger.Error += ApplicationLogger_Error;
            logErrorLevel = 3;

            ApplicationLogger.Message += ApplicationLogger_Message;
            logMessageLevel = 2;

            var dbt = new DMSDBTools();
            var carts = dbt.GetCartListFromDMS();

            Console.WriteLine("Data loaded");
        }

        private int logErrorLevel = 3;
        private int logMessageLevel = 2;

        private void ApplicationLogger_Error(int errorLevel, ErrorLoggerArgs args)
        {
            if (errorLevel > logErrorLevel)
            {
                return;
            }

            Console.WriteLine(@"=== Exception ===");
            Console.WriteLine(args.Message);

            if (!string.Equals(args.Message, args.Exception.Message))
                Console.WriteLine(args.Exception.Message);
        }

        private void ApplicationLogger_Message(int messageLevel, MessageLoggerArgs args)
        {
            if (messageLevel > logMessageLevel)
            {
                return;
            }

            Console.WriteLine(args.Message);
        }
    }
}
