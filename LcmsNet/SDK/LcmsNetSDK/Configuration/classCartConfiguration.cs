using System;
using System.Collections.Generic;
using System.Text;
using LcmsNetDataClasses;

namespace LcmsNetDataClasses.Configuration
{
    /// <summary>
    /// Class that encapsulates the configuration of the cart from
    /// systems to columns.
    /// </summary>
    public static class classCartConfiguration
    {
        /// <summary>
        /// Gets the number of enabled columns.
        /// </summary>
        public static int NumberOfEnabledColumns
        {
            get
            {
                var n = 0;

                // 
                // Figure out what columns are enabled or disabled.
                //             
                foreach (var col in Columns)
                {
                    if (col.Status != enumColumnStatus.Disabled)
                    {
                        n++;
                    }
                }
                return n;
            }
        }

        /// <summary>
        /// Gets or sets the list of columns available.
        /// </summary>
        public static List<classColumnData> Columns { get; set; }

        /// <summary>
        /// Builds a list of columns from the cart configuration object.
        /// </summary>
        /// <param name="config">Cart configuration.</param>
        /// <param name="orderByFirst">Orders the list by the first selected column. e.g. list[0] = column3 iff column3.First = True</param>
        /// <returns>List of columns stored in cart configuration.</returns>
        public static List<classColumnData> BuildColumnList(bool orderByFirst)
        {
            var orderList = new List<classColumnData>();
            foreach (var column in classCartConfiguration.Columns)
            {
                orderList.Add(column);
            }

            if (orderByFirst == false)
                return orderList;

            // 
            // For every column add it to the build list,
            // if it is not disabled to run
            // 
            var tempList1 = new List<classColumnData>();
            var tempList2 = new List<classColumnData>();
            var ptrList = tempList1;
            var copyNew = false;
            foreach (var data in orderList)
            {
                if (data.Status != enumColumnStatus.Disabled)
                {
                    if (data.First == true)
                    {
                        ptrList = tempList2;
                        copyNew = true;
                    }
                    ptrList.Add(data);
                }
            }
            if (copyNew == true)
            {
                ptrList.AddRange(tempList1);
            }

            return ptrList;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the name of the cart.
        /// </summary>
        public static string CartName
        {
            get { return classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTNAME); }
            set { classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CARTNAME, value); }
        }

        /// <summary>
        /// Gets or sets the name of the cart configuration in use
        /// </summary>
        public static string CartConfigName
        {
            get { return classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME); }
            set { classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CARTCONFIGNAME, value); }
        }

        public static double MinimumVolume
        {
            get
            {
                var volumeSetting = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_MINIMUMVOLUME);
                double volume;
                if (double.TryParse(volumeSetting, out volume))
                    return volume;

                return classSampleData.CONST_MIN_SAMPLE_VOLUME;
            }
            set { classLCMSSettings.SetParameter(classLCMSSettings.PARAM_MINIMUMVOLUME, value.ToString()); }
        }

        #endregion
    }
}