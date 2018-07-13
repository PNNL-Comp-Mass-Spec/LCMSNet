using System;
using LcmsNetData;
using LcmsNetData.Logging;
using LcmsNetDmsTools;

namespace TestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestDMSDataLoading();

            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("");
            Console.WriteLine("Closing in 3 seconds");
            System.Threading.Thread.Sleep(3000);
        }

        private static void TestDMSDataLoading()
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

        private static int logErrorLevel = 3;
        private static int logMessageLevel = 2;

        private static void ApplicationLogger_Error(int errorLevel, ErrorLoggerArgs args)
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

        private static void ApplicationLogger_Message(int messageLevel, MessageLoggerArgs args)
        {
            if (messageLevel > logMessageLevel)
            {
                return;
            }

            Console.WriteLine(args.Message);
        }

        private static void Dbt_ProgressEvent(object sender, ProgressEventArgs e)
        {
            Console.WriteLine(e.CurrentTask);
        }

    }
}
