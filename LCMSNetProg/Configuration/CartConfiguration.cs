using System.Collections.Generic;
using System.Linq;
using LcmsNetSDK.Data;

namespace LcmsNet.Configuration
{
    /// <summary>
    /// Class that encapsulates the configuration of the cart from
    /// systems to columns.
    /// </summary>
    public static class CartConfiguration
    {
        static CartConfiguration()
        {
            Columns = new List<ColumnData>();
        }

        /// <summary>
        /// Gets the number of enabled columns.
        /// </summary>
        // Figure out what columns are enabled or disabled.
        public static int NumberOfEnabledColumns => Columns.Count(col => col.Status != ColumnStatus.Disabled);

        /// <summary>
        /// Gets or sets the list of columns available.
        /// </summary>
        public static List<ColumnData> Columns { get; set; }

        /// <summary>
        /// Builds a list of columns from the cart configuration object.
        /// </summary>
        /// <param name="orderByFirst">Orders the list by the first selected column. e.g. list[0] = column3 iff column3.First = True</param>
        /// <returns>List of columns stored in cart configuration.</returns>
        public static List<ColumnData> BuildColumnList(bool orderByFirst)
        {
            var orderList = new List<ColumnData>();
            foreach (var column in Columns)
            {
                orderList.Add(column);
            }

            if (orderByFirst == false)
                return orderList;

            // For every column add it to the build list,
            // if it is not disabled to run
            var tempList1 = new List<ColumnData>();
            var tempList2 = new List<ColumnData>();
            var ptrList = tempList1;
            var copyNew = false;
            foreach (var data in orderList)
            {
                if (data.Status != ColumnStatus.Disabled)
                {
                    if (data.First)
                    {
                        ptrList = tempList2;
                        copyNew = true;
                    }
                    ptrList.Add(data);
                }
            }
            if (copyNew)
            {
                ptrList.AddRange(tempList1);
            }

            return ptrList;
        }
    }
}
