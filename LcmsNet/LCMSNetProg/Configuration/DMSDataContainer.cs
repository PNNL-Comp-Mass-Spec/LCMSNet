﻿using System;
using LcmsNetDmsTools;
using LcmsNetSDK;
using LcmsNetSDK.Logging;

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

            ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_USER, e.CurrentTask);
            Console.WriteLine(e.CurrentTask);
        }
    }
}