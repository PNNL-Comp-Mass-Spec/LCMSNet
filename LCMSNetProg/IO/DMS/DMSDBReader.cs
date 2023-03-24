using System.Collections.Generic;
using LcmsNet.IO.Sequence;

namespace LcmsNet.IO.DMS
{
    internal class DMSDBReader
    {
        // ReSharper disable CommentTypo

        // Ignore Spelling: na, wellplate

        // ReSharper restore CommentTypo

        private readonly DMSDBConnection db;

        public string DMSDatabase => db.DMSDatabase;

        /// <summary>
        /// Constructor
        /// </summary>
        public DMSDBReader()
        {
            db = new DMSDBConnection();
        }

        /// <summary>
        /// Checks for updates to the connection configuration if it hasn't been done recently
        /// </summary>
        /// <returns>True if the connection configuration was updated and is different from the previous configuration</returns>
        public bool RefreshConnectionConfiguration()
        {
            return db.RefreshConnectionConfiguration();
        }

        /// <summary>
        /// Gets DMS connection string from config file, excluding the password
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return db.GetCleanConnectionString();
        }

        /// <summary>
        /// Gets a list of instrument carts
        /// </summary>
        public IEnumerable<string> ReadCartList()
        {
            // Get a List containing all the carts
            var sqlCmd = $"SELECT DISTINCT cart_name FROM {db.SchemaPrefix}v_lc_cart_active_export " +
                         "ORDER BY cart_name";
            return db.GetSingleColumnTable(sqlCmd);
        }

        public IEnumerable<DmsDownloadData> ReadRequestedRuns(SampleQueryData queryData)
        {
            // Retrieve run requests from V_Requested_Run_Active_Export, filtering based on settings in queryData
            var sqlCmd = queryData.BuildSqlString(db.SchemaPrefix);

            var deDupDictionary = new Dictionary<string, string>();

            return db.ExecuteReader(sqlCmd, reader =>
            {
                var data = new DmsDownloadData
                {
                    RequestID = reader["request"].CastDBValTo<int>(),
                    RequestName = reader["name"].CastDBValTo<string>(),
                    CartName = reader["cart"].CastDBValTo<string>().LimitStringDuplication(deDupDictionary),
                    Comment = reader["comment"].CastDBValTo<string>().LimitStringDuplication(deDupDictionary),
                    EMSLUsageType = reader["usage_type"].CastDBValTo<string>().LimitStringDuplication(deDupDictionary),
                    EMSLProposalUser = reader["eus_users"].CastDBValTo<string>().LimitStringDuplication(deDupDictionary),
                    Block = reader["block"].CastDBValTo<int>(),
                    RunOrder = reader["run_order"].CastDBValTo<int>(),
                    Batch = reader["batch"].CastDBValTo<int>(),
                    SelectedToRun = false
                };

                var wellNumber = reader["well"].CastDBValTo<string>();
                data.PalWell = ConvertWellStringToInt(wellNumber);

                data.PalWellPlate = reader["wellplate"].CastDBValTo<string>();

                if (string.IsNullOrWhiteSpace(data.PalWellPlate) || data.PalWellPlate == "na")
                    data.PalWellPlate = string.Empty;

                return data;
            });
        }

        /// <summary>
        /// Converts a letter/number or just number string representing a well/vial into an integer
        /// </summary>
        /// <param name="vialPosition">Input string</param>
        /// <returns>Integer position</returns>
        private int ConvertWellStringToInt(string vialPosition)
        {
            if (string.IsNullOrWhiteSpace(vialPosition) || vialPosition == "na")
            {
                return 0;
            }

            // First, we'll see if it's a simple number
            if (int.TryParse(vialPosition, out var parsed))
            {
                // vialPosition is simply an integer
                return parsed;
            }

            try
            {
                // vialPosition is in the form A1 or B10
                // Convert it using ConvertVialToInt
                return ConvertVialPosition.ConvertVialToInt(vialPosition);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Test if we can query each of the needed DMS tables/views.
        /// </summary>
        /// <returns></returns>
        public bool CheckDMSConnection()
        {
            db.RefreshConnectionConfiguration();

            // Keys in this dictionary are view names, values are the column to use when ranking rows using Row_number()
            var viewInfo = new Dictionary<string, string>
            {
                { $"{db.SchemaPrefix}v_lc_cart_config_export", "id" },
                { $"{db.SchemaPrefix}v_requested_run_active_export", "request" }
            };

            return db.CheckDMSConnection(viewInfo);
        }
    }
}
