//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/04/2011
//
//*********************************************************************************************************
using System.Collections.Generic;
using System;

namespace LcmsNet.Devices.Pumps
{
    /// <summary>
    /// This class provides a wrapper around a dictionary containing ISCO pump status notification strings
    /// </summary>
    public static class classIscoStatusNotifications
    {
        #region "Class variables"
        static readonly Dictionary<string, string> m_NotifyList = new Dictionary<string, string>();
        #endregion

        #region "Constructor"
        static classIscoStatusNotifications()
        {
            //NOTE: This method may need updating if the enums it's based on change!!!
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.InitSucceeded),
                "Initialization succeeded");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Initializing),
                "Initializing");
            m_NotifyList.Add(GetControlModeEnumName(enumIscoControlMode.Local),
                "Control: Local");
            m_NotifyList.Add(GetControlModeEnumName(enumIscoControlMode.Remote),
                "Control: Remote");
            m_NotifyList.Add(GetControlModeEnumName(enumIscoControlMode.External),
                "Control: External");
            m_NotifyList.Add(GetOperationModeEnumName(enumIscoOperationMode.ConstantFlow),
                "Operation: Constant Flow");
            m_NotifyList.Add(GetOperationModeEnumName(enumIscoOperationMode.ConstantPressure),
                "Operation: Constant Pressure");

            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Stopped) + "A",
                "Pump A: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Stopped) + "B",
                "Pump B: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Stopped) + "C",
                "Pump C: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Running) + "A",
                "Pump A: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Running) + "B",
                "Pump B: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Running) + "C",
                "Pump C: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Hold) + "A",
                "Pump A: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Hold) + "B",
                "Pump B: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Hold) + "C",
                "Pump C: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Equilibrating) + "A",
                "Pump A: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Equilibrating) + "B",
                "Pump B: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Equilibrating) + "C",
                "Pump C: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Refilling) + "A",
                "Pump A: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Refilling) + "B",
                "Pump B: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.Refilling) + "C",
                "Pump C: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.FlowSet) + "A",
                "Pump A: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.FlowSet) + "B",
                "Pump B: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.FlowSet) + "C",
                "Pump C: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.PressSet) + "A",
                "Pump A: Pressure Set");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.PressSet) + "B",
                "Pump B: Pressure Set");
            m_NotifyList.Add(GetOperationStatusEnumName(enumIscoOperationStatus.PressSet) + "C",
                "Pump C: Pressure Set");
        }
       
        private static string GetControlModeEnumName(enumIscoControlMode controlMode)
        {
            var name = Enum.GetName(typeof(enumIscoControlMode), controlMode);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)controlMode;

            return name;
        }

        private static string GetOperationModeEnumName(enumIscoOperationMode operationMode)
        {
            var name = Enum.GetName(typeof(enumIscoOperationMode), operationMode);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)operationMode;

            return name;
        }

        private static string GetOperationStatusEnumName(enumIscoOperationStatus operationStatus)
        {
            var name = Enum.GetName(typeof(enumIscoOperationStatus), operationStatus);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)operationStatus;

            return name;
        }

        #endregion

        #region "Methods"
        /// <summary>
        /// Gets a notification string for a given input
        /// </summary>
        /// <param name="inp">Input string based on an enum name</param>
        /// <returns>Notification string</returns>
        public static string GetNotificationString(string inp)
        {
            if (m_NotifyList.ContainsKey(inp))
            {
                return m_NotifyList[inp];
            }

            return "";
        }

        /// <summary>
        /// Gets a list of all the notification strings stored
        /// </summary>
        /// <returns>List of notification strings</returns>
        public static List<string> GetNotificationListStrings()
        {
            var returnList = new List<string>();

            foreach (var curItem in m_NotifyList)
            {
                returnList.Add(curItem.Value);
            }

            return returnList;
        }
        #endregion
    }
}
