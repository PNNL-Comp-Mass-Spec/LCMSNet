using LcmsNetDmsTools;

namespace TestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbt = new classDBTools
            {
                LoadExperiments = true,
                LoadDatasets = true,
                RecentExperimentsMonthsToLoad = 0,
                RecentDatasetsMonthsToLoad = 12
            };

            dbt.LoadCacheFromDMS();
        }
    }
}
