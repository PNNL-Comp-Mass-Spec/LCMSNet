using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Logging;
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

            classApplicationLogger.Error += classApplicationLogger_Error;
            classApplicationLogger.ErrorLevel = 3;

            classApplicationLogger.Message += classApplicationLogger_Message;
            classApplicationLogger.MessageLevel = 2;

            var dbt = new classDBTools
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

        private static void classApplicationLogger_Error(int errorLevel, classErrorLoggerArgs args)
        {
            Console.WriteLine(@"=== Exception ===");
            Console.WriteLine(args.Message);

            if (!string.Equals(args.Message, args.Exception.Message))
                Console.WriteLine(args.Exception.Message);
        }

        private static void classApplicationLogger_Message(int messageLevel, classMessageLoggerArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private static void Dbt_ProgressEvent(object sender, LcmsNetSDK.ProgressEventArgs e)
        {
            Console.WriteLine(e.CurrentTask);
        }

    }
}
