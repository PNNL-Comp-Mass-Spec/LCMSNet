using System;
using LcmsNet.IO.DMS;
using LcmsNetData;
using LcmsNetData.Logging;

namespace LcmsNet.Configuration
{
    public static class DMSDataContainer
    {
        public static DMSDBTools DBTools { get; }

        /// <summary>
        /// When true, progress events from DBTools are logged using ApplicationLogger
        /// </summary>
        /// <remarks>Set to false if another class is logging the events</remarks>
        public static bool LogDBToolsEvents { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        static DMSDataContainer()
        {
            DBTools = new DMSDBTools
            {
                // LCMSNet does not directly use experiments. Don't load them from DMS.
                LoadExperiments = false,
                // LCMSNet does not use DMS dataset names. Don't load them from DMS.
                LoadDatasets = false,
                RecentExperimentsMonthsToLoad = 18,
                RecentDatasetsMonthsToLoad = 12
            };

            LogDBToolsEvents = true;
            DBTools.ProgressEvent += DBTools_ProgressEvent;
        }

        private static void DBTools_ProgressEvent(object sender, ProgressEventArgs e)
        {
            if (!LogDBToolsEvents)
                return;

            ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_USER, e.CurrentTask);
            Console.WriteLine(e.CurrentTask);
        }
    }
}
