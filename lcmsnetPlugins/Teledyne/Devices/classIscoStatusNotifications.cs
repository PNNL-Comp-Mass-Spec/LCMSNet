//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2011, Battelle Memorial Institute
// Created 03/04/2011
//
// Last modified 03/4/2011
//*********************************************************************************************************
using System.Collections.Generic;
using System;

namespace LcmsNet.Devices.Pumps
{
    public static class classIscoStatusNotifications
    {
        //*********************************************************************************************************
        // This class provides a wrapper around a dictionary containing ISCO pump status notification strings
        //**********************************************************************************************************

        #region "Class variables"
            static Dictionary<string, string> mobj_NotifyList = new Dictionary<string, string>();
        #endregion

        #region "Constructor"
            static classIscoStatusNotifications()
            {
                //NOTE: This method may need updating if the enums it's based on change!!!
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.InitSucceeded),
                    "Initialization succeeded");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Initializing),
                    "Initializing");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoControlMode), enumIscoControlMode.Local),
                    "Control: Local");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoControlMode), enumIscoControlMode.Remote),
                    "Control: Remote");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoControlMode), enumIscoControlMode.External),
                    "Control: External");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationMode), enumIscoOperationMode.ConstantFlow),
                    "Operation: Constant Flow");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationMode), enumIscoOperationMode.ConstantPressure),
                    "Operation: Constant Pressure");

                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Stopped) + "A",
                    "Pump A: Stopped");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Stopped) + "B",
                    "Pump B: Stopped");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Stopped) + "C",
                    "Pump C: Stopped");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Running) + "A",
                    "Pump A: Running");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Running) + "B",
                    "Pump B: Running");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Running) + "C",
                    "Pump C: Running");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Hold) + "A",
                    "Pump A: Hold");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Hold) + "B",
                    "Pump B: Hold");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Hold) + "C",
                    "Pump C: Hold");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Equilibrating) + "A",
                    "Pump A: Equilibrating");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Equilibrating) + "B",
                    "Pump B: Equilibrating");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Equilibrating) + "C",
                    "Pump C: Equilibrating");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Refilling) + "A",
                    "Pump A: Refilling");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Refilling) + "B",
                    "Pump B: Refilling");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.Refilling) + "C",
                    "Pump C: Refilling");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.FlowSet) + "A",
                    "Pump A: Flow Set");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.FlowSet) + "B",
                    "Pump B: Flow Set");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.FlowSet) + "C",
                    "Pump C: Flow Set");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.PressSet) + "A",
                    "Pump A: Pressure Set");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.PressSet) + "B",
                    "Pump B: Pressure Set");
                mobj_NotifyList.Add(Enum.GetName(typeof(enumIscoOperationStatus), enumIscoOperationStatus.PressSet) + "C",
                    "Pump C: Pressure Set");
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
                if (mobj_NotifyList.ContainsKey(inp))
                {
                    return mobj_NotifyList[inp];
                }
                else return "";
            }   

            /// <summary>
            /// Gets a list of all the notification strings stored
            /// </summary>
            /// <returns>List of notification strings</returns>
            public static List<string> GetNotificationListStrings()
            {
                List<string> returnList = new List<string>();

                foreach (KeyValuePair<string, string> curItem in mobj_NotifyList)
                {
                    returnList.Add(curItem.Value);
                }

                return returnList;
            }   
        #endregion
    }
}
