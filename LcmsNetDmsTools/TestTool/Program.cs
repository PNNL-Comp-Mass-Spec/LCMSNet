using System;
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

                dbt.LoadCacheFromDMS();

            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void classApplicationLogger_Error(int errorLevel, classErrorLoggerArgs args)
        {
            Console.WriteLine(@"=== Exception ===");
            Console.WriteLine(args.Message);

            if (!string.Equals(args.Message, args.Exception.Message))
                Console.WriteLine(args.Exception.Message);
        }

        static void classApplicationLogger_Message(int messageLevel, classMessageLoggerArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
