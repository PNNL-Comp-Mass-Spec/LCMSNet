using System;
using LcmsNet.IO.DMS;
using LcmsNetData;
using LcmsNetData.Logging;
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

            var dbt = new DMSDBTools
            {
                LoadExperiments = true,
                LoadDatasets = true,
                RecentExperimentsMonthsToLoad = 18,
                RecentDatasetsMonthsToLoad = 12
            };

            dbt.ProgressEvent += Dbt_ProgressEvent;

            dbt.LoadCacheFromDMS();

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

        private void Dbt_ProgressEvent(object sender, ProgressEventArgs e)
        {
            Console.WriteLine(e.CurrentTask);
        }

    }
}
