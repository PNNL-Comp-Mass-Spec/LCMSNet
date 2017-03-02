//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2014, Battelle Memorial Institute
// Created 3/13/2014
//
// Last modified 04/18/2014
//*********************************************************************************************************

using System.Text;

namespace LcmsNetDataClasses
{
    public enum ModelStatusCategory
    {
        Error,
        Warning,
        Information
    }

    /// <summary>
    /// class to hold information on the Fluidics Model's status.
    /// </summary>
    public class ModelStatus
    {
        private static long m_availableID;
        private string m_timeOfOccurance;

        public ModelStatus()
        {
            UID = m_availableID++;
            Name = "Generic";
            Category = ModelStatusCategory.Information;
            Description = "Generic ModelStatus";
            EventDevice = null;
            Event = string.Empty;
            Time = LcmsNetSDK.TimeKeeper.Instance.Now.ToString();
                // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString();
        }

        public ModelStatus(string name = "", string description = "",
            ModelStatusCategory category = ModelStatusCategory.Information, string eventName = "", string time = "",
            Devices.IDevice device = null, Devices.IDevice problemDevice = null)
        {
            UID = m_availableID++;
            Name = name;
            Description = description;
            Category = category;
            Event = eventName;
            Time = time != string.Empty ? time : LcmsNetSDK.TimeKeeper.Instance.Now.ToString();
            EventDevice = device;
            ProblemDevice = problemDevice;
        }

        /// <summary>
        /// Unique ID representing this status change.
        /// </summary>
        public long UID { get; set; }

        /// <summary>
        /// The name/topic of the status change
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the status change
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///  The status category of Error, Information, or Warning.
        /// </summary>
        public ModelStatusCategory Category { get; set; }

        /// <summary>
        /// returns The IDevice that triggered this status change, if known.
        /// </summary>
        public Devices.IDevice EventDevice { get; set; }


        public Devices.IDevice ProblemDevice { get; set; }

        /// <summary>
        /// returns the event that triggered the status change, if set.
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// returns the time that the status change was recorded.
        /// </summary>
        public string Time
        {
            get { return m_timeOfOccurance; }
            set { m_timeOfOccurance = value; }
        }

        /// <summary>
        /// provides a string representation of what happened on this mode status change.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var temp = new StringBuilder();
            temp.Append(Description);
            if (Event != null)
            {
                temp.Append(" On Event:");
                temp.Append(Event);
            }
            if (Time != null)
            {
                temp.Append(" at ");
                temp.Append(Time);
            }
            if (EventDevice != null)
            {
                temp.Append(" using Device:");
                temp.Append(EventDevice.Name);
            }

            return temp.ToString();
        }
    }
}