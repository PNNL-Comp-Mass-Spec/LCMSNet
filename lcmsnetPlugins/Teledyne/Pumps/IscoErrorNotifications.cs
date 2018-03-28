//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/07/2011
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// This class provides a wrapper around a dictionary containing ISCO pump error notification strings
    /// </summary>
    public static class IscoErrorNotifications
    {
        #region "Class variables"
        static readonly Dictionary<string, string> m_NotifyList = new Dictionary<string, string>();
        #endregion

        #region "Constructor"
        static IscoErrorNotifications()
        {
            //NOTE: This method may need updating if the enums it's based on change!!!
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.DeviceNotInitialized),
                "Device Not Initialized");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.ComError),
                "Communication Error");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.InitializationError),
                "Initialization Error");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.MessageParseError),
                "Message Parse Error");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderBottom) + "A",
                "Pump A: Cylinder at bottom");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderBottom) + "B",
                "Pump B: Cylinder at bottom");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderBottom) + "C",
                "Pump C: Cylinder at bottom");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderEmpty) + "A",
                "Pump A: Cylinder empty");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderEmpty) + "B",
                "Pump B: Cylinder empty");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.CylinderEmpty) + "C",
                "Pump C: Cylinder empty");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.MotorFailure) + "A",
                "Pump A: Motor Failure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.MotorFailure) + "B",
                "Pump B: Motor Failure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.MotorFailure) + "C",
                "Pump C: Motor Failure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.OverPressure) + "A",
                "Pump A: Over pressure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.OverPressure) + "B",
                "Pump B: Over pressure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.OverPressure) + "C",
                "Pump C: Over pressure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.UnderPressure) + "A",
                "Pump A: Under pressure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.UnderPressure) + "B",
                "Pump B: Under pressure");
            m_NotifyList.Add(GetEnumName(IscoProblemStatus.UnderPressure) + "C",
                "Pump C: Under pressure");
        }

        private static string GetEnumName(IscoProblemStatus iscoProblemStatus)
        {
            var name = Enum.GetName(typeof(IscoProblemStatus), iscoProblemStatus);
            if (string.IsNullOrWhiteSpace(name))
                return "InvalidEnum_" + (int)iscoProblemStatus;

            return name;
        }

        #endregion

        #region "Methods"
        /// <summary>
        /// Gets specified notification string
        /// </summary>
        /// <param name="key">Notification key</param>
        /// <returns>Notification string</returns>
        public static string GetNotificationString(string key)
        {
            if (m_NotifyList.ContainsKey(key))
            {
                return m_NotifyList[key];
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a list of all the notifications stored
        /// </summary>
        /// <returns>List of notifications</returns>
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
