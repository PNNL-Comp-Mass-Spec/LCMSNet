using System;
using System.Collections.Generic;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// This class provides a wrapper around a dictionary containing ISCO pump status notification strings
    /// </summary>
    public static class IscoStatusNotifications
    {
        #region "Class variables"
        static readonly Dictionary<string, string> m_NotifyList = new Dictionary<string, string>();
        #endregion

        #region "Constructor"
        static IscoStatusNotifications()
        {
            //NOTE: This method may need updating if the enums it's based on change!!!
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.InitSucceeded),
                "Initialization succeeded");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Initializing),
                "Initializing");
            m_NotifyList.Add(GetControlModeEnumName(IscoControlMode.Local),
                "Control: Local");
            m_NotifyList.Add(GetControlModeEnumName(IscoControlMode.Remote),
                "Control: Remote");
            m_NotifyList.Add(GetControlModeEnumName(IscoControlMode.External),
                "Control: External");
            m_NotifyList.Add(GetOperationModeEnumName(IscoOperationMode.ConstantFlow),
                "Operation: Constant Flow");
            m_NotifyList.Add(GetOperationModeEnumName(IscoOperationMode.ConstantPressure),
                "Operation: Constant Pressure");

            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Stopped) + "A",
                "Pump A: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Stopped) + "B",
                "Pump B: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Stopped) + "C",
                "Pump C: Stopped");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Running) + "A",
                "Pump A: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Running) + "B",
                "Pump B: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Running) + "C",
                "Pump C: Running");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Hold) + "A",
                "Pump A: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Hold) + "B",
                "Pump B: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Hold) + "C",
                "Pump C: Hold");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Equilibrating) + "A",
                "Pump A: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Equilibrating) + "B",
                "Pump B: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Equilibrating) + "C",
                "Pump C: Equilibrating");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Refilling) + "A",
                "Pump A: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Refilling) + "B",
                "Pump B: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.Refilling) + "C",
                "Pump C: Refilling");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.FlowSet) + "A",
                "Pump A: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.FlowSet) + "B",
                "Pump B: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.FlowSet) + "C",
                "Pump C: Flow Set");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.PressSet) + "A",
                "Pump A: Pressure Set");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.PressSet) + "B",
                "Pump B: Pressure Set");
            m_NotifyList.Add(GetOperationStatusEnumName(IscoOperationStatus.PressSet) + "C",
                "Pump C: Pressure Set");
        }

        private static string GetControlModeEnumName(IscoControlMode controlMode)
        {
            var name = Enum.GetName(typeof(IscoControlMode), controlMode);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)controlMode;

            return name;
        }

        private static string GetOperationModeEnumName(IscoOperationMode operationMode)
        {
            var name = Enum.GetName(typeof(IscoOperationMode), operationMode);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)operationMode;

            return name;
        }

        private static string GetOperationStatusEnumName(IscoOperationStatus operationStatus)
        {
            var name = Enum.GetName(typeof(IscoOperationStatus), operationStatus);
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
