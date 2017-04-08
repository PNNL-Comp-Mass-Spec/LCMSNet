//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/21/2009
//
//*********************************************************************************************************

using System.Collections;
using System.Windows.Forms;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Class for comparison of listview items in DMS view form listviews
    /// </summary>
    class classListViewItemComparer : IComparer
    {
        #region "Class variables"

        int m_Column;

        #endregion

        #region "Properties"

        /// <summary>
        /// Determines sort order. TRUE for ascending, FALSE for descending
        /// </summary>
        public bool sortOrderAsc { get; set; } = true;

        // End property

        /// <summary>
        /// Sort mode from SortModeConstants enum
        /// </summary>
        public enumListViewComparerMode.SortModeConstants sortMode { get; set; } = enumListViewComparerMode.SortModeConstants.text;

        // End property

        #endregion

        #region "Methods"

        /// <summary>
        /// Constructor overload requiring no parameters
        /// Assumes column 0, ascending sort order, string data type
        /// </summary>
        public classListViewItemComparer()
        {
            DoConstructorTask(0, true, enumListViewComparerMode.SortModeConstants.text);
        }

        /// <summary>
        /// Constructor overload requiring one parameter
        /// Assumes ascending sort order, string data type
        /// </summary>
        /// <param name="column">Index of listview column being sorted</param>
        public classListViewItemComparer(int column)
        {
            DoConstructorTask(column, true, enumListViewComparerMode.SortModeConstants.text);
        }

        /// <summary>
        /// Constructor overload requiring two parameters
        /// Assumes string data type
        /// </summary>
        /// <param name="column">Index of listview column being sorted</param>
        /// <param name="sortOrderAsc">Sort order. TRUE for ascending; FALSE for descending</param>
        public classListViewItemComparer(int column, bool sortOrderAsc)
        {
            DoConstructorTask(column, sortOrderAsc, enumListViewComparerMode.SortModeConstants.text);
        }

        /// <summary>
        /// Constructor overload requiring three parameters
        /// </summary>
        /// <param name="column">Index of listview column being sorted</param>
        /// <param name="sortOrderAsc">Sort order. TRUE for ascending; FALSE for descending</param>
        /// <param name="sortMode">Enum specifying numeric or string data in column</param>
        public classListViewItemComparer(int column, bool sortOrderAsc,
            enumListViewComparerMode.SortModeConstants sortMode)
        {
            DoConstructorTask(column, sortOrderAsc, sortMode);
        }

        /// <summary>
        /// Common method to be called by all constructor overloads
        /// </summary>
        /// <param name="column">Index of listview column being sorted</param>
        /// <param name="sortOrderAsc">Sort order. TRUE for ascending; FALSE for descending</param>
        /// <param name="sortMode">Enum specifying numeric or string data in column</param>
        private void DoConstructorTask(int column, bool sortOrderAsc,
            enumListViewComparerMode.SortModeConstants sortMode)
        {
            m_Column = column;
            this.sortOrderAsc = sortOrderAsc;
            this.sortMode = sortMode;
        }

        /// <summary>
        /// Performs comparison of two listview items
        /// Requires sort order and data type to be set before calling, either in constructor or properties
        /// </summary>
        /// <param name="x">First ListviewItem object for comparison</param>
        /// <param name="y">Second ListviewItem object for comparison</param>
        /// <returns>If sortorder set to ascending, returns 1 if x > y, -1 if x<y.
        /// If sortorder set to descending, returns -1 if x>y, 1 if x<y</returns>
        public int Compare(object x, object y)
        {
            var tempResult = 0;

            // Convert input objects to listview items
            var item1 = x as ListViewItem;
            var item2 = y as ListViewItem;

            if (sortMode == enumListViewComparerMode.SortModeConstants.numeric)
            {
                // Column is numeric, so convert text in listviewitem to numeric value
                double value1;
                double value2;
                double.TryParse(item1.SubItems[m_Column].Text, out value1);
                double.TryParse(item2.SubItems[m_Column].Text, out value2);
                // Set temp output value
                if (value1 > value2)
                {
                    tempResult = 1;
                }
                else if (value1 < value2)
                {
                    tempResult = -1;
                }
            }
            else
            {
                // Column is text
                tempResult = string.Compare(item1.SubItems[m_Column].Text, item2.SubItems[m_Column].Text);
            }

            // If sort order is ascending, then return tempResult. Otherwise, negate tempResult before returning
            if (sortOrderAsc)
            {
                return tempResult;
            }
            return -1 * tempResult;
        }

        #endregion
    }
}