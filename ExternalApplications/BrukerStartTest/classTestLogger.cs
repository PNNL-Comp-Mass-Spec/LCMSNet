
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 07/09/2010
//
// Last modified 07/09/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

// Configure log4net using the .log4net file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Logging.config", Watch = true)]

namespace BrukerStartTest
{
    /// <summary>
    /// Logging class for test program
    /// </summary>
    class classTestLogger
    {
        #region "Constants"
        #endregion

        #region "Class variables"
            private static readonly ILog m_FileLogger = LogManager.GetLogger("FileLogger");
        #endregion

        #region "Delegates"
        #endregion

        #region "Events"
        #endregion

        #region "Properties"
        #endregion

        #region "Constructors"
        #endregion

        #region "Methods"
            /// <summary>
            /// Logs a debug message
            /// </summary>
            /// <param name="msg">Message to log</param>
            public static void LogDebugMessage(string msg)
            {
                StringBuilder outStrBld = new StringBuilder();
                outStrBld.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + ", ");
                outStrBld.Append(msg);
                outStrBld.Append(Environment.NewLine);
                m_FileLogger.Debug(outStrBld.ToString());
            }
        #endregion
    }
}
