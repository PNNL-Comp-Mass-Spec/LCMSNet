using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNet.Data;
using LcmsNetSDK;
using ReactiveUI;

namespace LcmsNet.Configuration
{
    /// <summary>
    /// Class that encapsulates the configuration of the cart from
    /// systems to columns.
    /// </summary>
    public class CartConfiguration : ReactiveObject
    {
        static CartConfiguration()
        {
            Instance = null;
        }

        private CartConfiguration()
        {
            var columns = new List<ColumnData>();
            var columnStatus1 = ColumnStatus.Idle;
            var columnStatus2 = ColumnStatus.Idle;
            var columnStatus3 = ColumnStatus.Idle;
            var columnStatus4 = ColumnStatus.Idle;
            var disabledCount = 0;
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED0, false))
            {
                columnStatus1 = ColumnStatus.Disabled;
                disabledCount++;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED1, false))
            {
                columnStatus2 = ColumnStatus.Disabled;
                disabledCount++;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED2, false))
            {
                columnStatus3 = ColumnStatus.Disabled;
                disabledCount++;
            }
            if (LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNDISABLED3, false))
            {
                columnStatus4 = ColumnStatus.Disabled;
                disabledCount++;
            }

            // Business logic: One column must be enabled; if all columns are disabled in settings, force-enable column one
            if (disabledCount == 4)
            {
                columnStatus1 = ColumnStatus.Idle;
                LCMSSettings.SetParameter(LCMSSettings.PARAM_COLUMNDISABLED0, false.ToString());
            }

            // Create system data with columns.
            column1 = new ColumnData(0, LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME0), columnStatus1, Colors.Tomato);
            column2 = new ColumnData(1, LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME1), columnStatus2, Colors.Lime);
            column3 = new ColumnData(2, LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME2), columnStatus3, Colors.LightSteelBlue);
            column4 = new ColumnData(3, LCMSSettings.GetParameter(LCMSSettings.PARAM_COLUMNNAME3), columnStatus4, Colors.LightSalmon);

            columns.Add(column1);
            columns.Add(column2);
            columns.Add(column3);
            columns.Add(column4);

            columnList = columns;

            numberOfColumnsEnabled = this.WhenAnyValue(x => x.column1.Status, x => x.column2.Status,
                x => x.column3.Status, x => x.column4.Status).Select(
                x =>
                {
                    var count = 0;
                    if (x.Item1 != ColumnStatus.Disabled)
                        count++;
                    if (x.Item2 != ColumnStatus.Disabled)
                        count++;
                    if (x.Item3 != ColumnStatus.Disabled)
                        count++;
                    if (x.Item4 != ColumnStatus.Disabled)
                        count++;

                    return count;
                }).ToProperty(this, x => x.NumberOfColumnsEnabled);
        }

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new CartConfiguration();
            }
        }

        private readonly ColumnData column1;
        private readonly ColumnData column2;
        private readonly ColumnData column3;
        private readonly ColumnData column4;
        private readonly ObservableAsPropertyHelper<int> numberOfColumnsEnabled;
        private readonly IReadOnlyList<ColumnData> columnList;

        public int NumberOfColumnsEnabled => numberOfColumnsEnabled.Value;

        public static CartConfiguration Instance { get; private set; }

        /// <summary>
        /// Gets the number of enabled columns.
        /// </summary>
        // Figure out what columns are enabled or disabled.
        public static int NumberOfEnabledColumns => Columns.Count(col => col.Status != ColumnStatus.Disabled);

        /// <summary>
        /// Gets or sets the list of columns available.
        /// </summary>
        public static IReadOnlyList<ColumnData> Columns => Instance.columnList;

        /// <summary>
        /// Builds a list of columns from the cart configuration object.
        /// </summary>
        /// <param name="orderByFirst">Orders the list by the first selected column. e.g. list[0] = column3 iff column3.First = True</param>
        /// <returns>List of columns stored in cart configuration.</returns>
        public static List<ColumnData> BuildColumnList(bool orderByFirst)
        {
            if (!orderByFirst)
                return Columns.ToList();

            // For every column add it to the build list, if it is not disabled to run
            return Columns.Where(x => x.Status != ColumnStatus.Disabled).OrderBy(x => x.ID).ToList();
        }
    }
}
