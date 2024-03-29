﻿using System;
using System.Globalization;

namespace LcmsNetSDK.System
{
    public class TimeKeeper
    {
        private TimeZoneInfo m_current_timezone;
        private static TimeKeeper m_instance;

        private TimeKeeper()
        {
            var timeZone = LCMSSettings.GetParameter(LCMSSettings.PARAM_TIMEZONE);
            if (string.IsNullOrWhiteSpace(timeZone))
            {
                m_current_timezone = TimeZoneInfo.Local;
                LCMSSettings.SetParameter(LCMSSettings.PARAM_TIMEZONE, m_current_timezone.Id);
            }
            else
            {
                m_current_timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            LCMSSettings.SettingChanged += LCMSSettings_SettingChanged;
        }

        private void LCMSSettings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.SettingName == "TimeZone")
            {
                if (!string.IsNullOrEmpty(e.SettingValue))
                {
                    var newTimeZone = TimeZoneInfo.FindSystemTimeZoneById(e.SettingValue);
                    m_current_timezone = newTimeZone;
                }
            }
        }

        ~TimeKeeper()
        {
            //Persist the timezone setting
            LCMSSettings.SettingChanged -= LCMSSettings_SettingChanged;
            LCMSSettings.SetParameter(LCMSSettings.PARAM_TIMEZONE, m_current_timezone.Id);
        }

        /// <summary>
        /// Convert a DateTime object to the specified time zone
        /// </summary>
        /// <param name="time">a DateTime object containing the date and time to convert</param>
        /// <param name="timeZoneId">a string representing the time zone to convert to</param>
        /// <returns>a DateTime object containing the date and time in the requested timezone</returns>
        public DateTime ConvertToTimeZone(DateTime time, string timeZoneId)
        {
            return TimeZoneInfo.ConvertTime(time, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }

        /// <summary>
        /// Determine if two dates cross over a daylight savings time transition
        /// </summary>
        /// <param name="start">DateTime object containing the starting date and time</param>
        /// <param name="end">DateTime object containing the ending date and time</param>
        /// <returns>true if a DST transition occurs between the two DateTimes, false otherwise</returns>
        public bool DoDateTimesSpanDaylightSavingsTransition(DateTime start, DateTime end)
        {
            //Construct DateTimes representing DST transition
            FindDSTTransitions(start.Year, out var startTransTime, out var endTransTime);
            var springTransition = ConvertToDateTime(startTransTime, start.Year);
            var fallTransition = ConvertToDateTime(endTransTime, end.Year);
            //determine if a DST transition occurs between start and end (inclusive of both)
            if (start.CompareTo(springTransition) <= 0 && end.CompareTo(springTransition) >= 0)
            {
                return true;
            }

            if (start.CompareTo(fallTransition) <= 0 && end.CompareTo(fallTransition) >= 0)
            {
                return true;
            }

            return false;
        }

        private void FindDSTTransitions(int year, out TimeZoneInfo.TransitionTime startTransTime,
            out TimeZoneInfo.TransitionTime endTransTime)
        {
            startTransTime = new TimeZoneInfo.TransitionTime();
            endTransTime = new TimeZoneInfo.TransitionTime();

            var adjustments = m_current_timezone.GetAdjustmentRules();
            // Iterate adjustment rules for time zone
            foreach (var adjustment in adjustments)
            {
                // Determine if this adjustment rule covers year desired
                if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
                {
                    startTransTime = adjustment.DaylightTransitionStart;
                    endTransTime = adjustment.DaylightTransitionEnd;
                }
            }
        }

        /*
        /// <summary>
        /// Determine if a method starts after a DST transition(on the day of that DST transition). Used in
        /// optimization routine.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool AfterDSTTransition(LCMethod method)
        {
            var year = method.Start.Year;
            TimeZoneInfo.TransitionTime springTransition;
            TimeZoneInfo.TransitionTime fallTransition;
            FindDSTTransitions(year, out springTransition, out fallTransition);
            var dstTransitionSpring = ConvertToDateTime(springTransition, year);
            var dstTransitionFall = ConvertToDateTime(fallTransition, year);
            // If the method starts after the spring dst transition and occurs on the DAY of that transition...
            if (method.Start.Subtract(dstTransitionSpring).Milliseconds >= 0 &&
                method.Start.Date == dstTransitionSpring.Date)
            {
                return true;
            }

            // If the method starts after the fall dst transition and occurs on the DAY of that transition...
            if (method.Start.Subtract(dstTransitionFall).Milliseconds >= 0 &&
                method.Start.Date == dstTransitionFall.Date)
            {
                return true;
            }

            // method does not start after a dst transition.
            return false;
        }*/

        /// <summary>
        /// convert a daylight savings transition rule to a date
        /// </summary>
        /// <param name="transition">a TransitionTime struct for a specific TimeZoneInfo(and thus a specific timezone)</param>
        /// <param name="year">An int representing the year to apply the rule to</param>
        /// <returns>A DateTime object containing the exact date and time the transition occurs for the specified year</returns>
        private DateTime ConvertToDateTime(TimeZoneInfo.TransitionTime transition, int year)
        {
            //This is a modified version of MSDN's example code from the TimeZoneInfo.TransitionTime property page.
            var localCalendar = CultureInfo.CurrentCulture.Calendar;
            if (transition.IsFixedDateRule)
            {
                return new DateTime(year, transition.Month, (int) transition.DayOfWeek, transition.TimeOfDay.Hour,
                    transition.TimeOfDay.Minute, transition.TimeOfDay.Second, localCalendar);
            }

            var startOfWeek = transition.Week * 7 - 6;

            // What day of the week does the month start on?
            var firstDayOfWeek = (int) localCalendar.GetDayOfWeek(new DateTime(year, transition.Month, 1));

            // Determine how much start date has to be adjusted
            int transitionDay;
            var changeDayOfWeek = (int) transition.DayOfWeek;

            if (firstDayOfWeek <= changeDayOfWeek)
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
            else
                transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

            // Adjust for months with no fifth week
            if (transitionDay > localCalendar.GetDaysInMonth(year, transition.Month))
                transitionDay -= 7;

            return new DateTime(year, transition.Month, transitionDay, transition.TimeOfDay.Hour,
                                transition.TimeOfDay.Minute, transition.TimeOfDay.Second);
        }

        /// <summary>
        /// Get current time as defined by the currently selected timezone
        /// </summary>
        /// <remarks>The time will be adjusted for daylight savings, as appropriate</remarks>
        public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, m_current_timezone);

        public TimeZoneInfo TimeZone
        {
            get => m_current_timezone;
            set
            {
                m_current_timezone = value;
                LCMSSettings.SetParameter(LCMSSettings.PARAM_TIMEZONE, value.Id);
            }
        }

        public static TimeKeeper Instance => m_instance ?? (m_instance = new TimeKeeper());
    }
}
