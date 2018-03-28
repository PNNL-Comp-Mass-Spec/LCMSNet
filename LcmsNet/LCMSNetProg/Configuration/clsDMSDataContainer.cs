using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDmsTools;
using LcmsNetSDK;
using LcmsNetSDK.Logging;

namespace LcmsNet.Configuration
{
    public static class clsDMSDataContainer
    {

        public static classDBTools DBTools { get; }

        /// <summary>
        /// When true, progress events from DBTools are logged using classApplicationLogger
        /// </summary>
        /// <remarks>Set to false if another class is logging the events</remarks>
        public static bool LogDBToolsEvents { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        static clsDMSDataContainer()
        {
            DBTools = new classDBTools
            {
                LoadExperiments = true,
                LoadDatasets = true,
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

            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_USER, e.CurrentTask);
            Console.WriteLine(e.CurrentTask);
        }
    }
}
