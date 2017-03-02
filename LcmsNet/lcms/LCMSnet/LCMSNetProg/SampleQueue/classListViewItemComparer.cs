//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/21/2009
//
// Last modified 01/22/2009
//*********************************************************************************************************

using System.Collections;

namespace LcmsNet.SampleQueue
{
    class classListViewItemComparer : IComparer
    {
        //*********************************************************************************************************
        // Class for comparison of listview items in DMS view form listviews
        //**********************************************************************************************************

        #region "Class variables"

        bool mbool_SortOrderAsc = true;
        enumListViewComparerMode.SortModeConstants menum_SortMode = enumListViewComparerMode.SortModeConstants.text;
        int mint_Column;

        #endregion

        #region "Properties"

        /// <summary>
        /// Determines sort order. TRUE for ascending, FALSE for descending
        /// </summary>
        public bool sortOrderAsc
        {
            get { return mbool_SortOrderAsc; }
            set { mbool_SortOrderAsc = value; }
        } // End property

        /// <summary>
        /// Sort mode from SortModeConstants enum
        /// </summary>
        public enumListViewComparerMode.SortModeConstants sortMode
        {
            get { return menum_SortMode; }
            set { menum_SortMode = value; }
        } // End property

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
            mint_Column = column;
            mbool_SortOrderAsc = sortOrderAsc;
            menum_SortMode = sortMode;
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
            int tempResult = 0;

            // Convert input objects to listview items
            System.Windows.Forms.ListViewItem item1 = x as System.Windows.Forms.ListViewItem;
            System.Windows.Forms.ListViewItem item2 = y as System.Windows.Forms.ListViewItem;

            if (menum_SortMode == enumListViewComparerMode.SortModeConstants.numeric)
            {
                // Column is numeric, so convert text in listviewitem to numeric value
                double value1;
                double value2;
                double.TryParse(item1.SubItems[mint_Column].Text, out value1);
                double.TryParse(item2.SubItems[mint_Column].Text, out value2);
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
                tempResult = string.Compare(item1.SubItems[mint_Column].Text, item2.SubItems[mint_Column].Text);
            }

            // If sort order is ascending, then return tempResult. Otherwise, negate tempResult before returning
            if (mbool_SortOrderAsc)
            {
                return tempResult;
            }
            else
            {
                return -1 * tempResult;
            }
        }

        #endregion
    }
} // End namespace