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
                RecentExperimentsMonthsToLoad = 18,
                RecentDatasetsMonthsToLoad = 12
            };

            dbt.LoadCacheFromDMS();
        }
    }
}
