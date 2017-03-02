/*********************************************************************************************************
 * Written by Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2014 Battle Memorial Institute
 *
 * Last Modified 6/4/2014 By Christopher Walters
 *********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Method;

namespace FluidicsSimulator
{
    /// <summary>
    /// This list is meant to hold concurrent events.
    /// </summary>
    public class SimEventList : List<classLCEvent>, IComparable
    {

        #region methods
        /// <summary>
        /// Just to make sure that this list can't be instantiated without a time...this is private
        /// </summary>
        private SimEventList()
        {
            Time = new DateTime();
        }

        /// <summary>
        /// Constructor that takes a timespan as parameter, this is so we can sort this list in a SortedSet
        /// all events in the list should happen "concurrently".
        /// </summary>
        /// <param name="time">A datetime representing when the events following those in this list should start</param>
        public SimEventList(DateTime time)
        {
            this.Time = time;
        }

        /// <summary>
        /// IComparable interface required, used to define the lists position in the SortedSet
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        int IComparable.CompareTo(Object other)
        {
            SimEventList otherLst = (SimEventList)other;
            return this.Time.CompareTo(otherLst.Time);
        }

        #endregion

        #region properties
        public DateTime Time
        {
            get;
            set;
        }

        #endregion
    }
}
