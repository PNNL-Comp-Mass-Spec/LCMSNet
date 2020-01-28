using System.Collections.Generic;
using LcmsNetData.Data;

namespace LcmsNetSQLiteTools
{
    public class SQLiteCacheDefaultData
    {
        public SQLiteCacheDefaultData()
        {
            CartNames.Add("default");
            SeparationTypes.Add("default");
            DatasetTypes.Add("default");
            ColumnNames.AddRange(new [] { "0", "1", "2", "3", "4" });
            Experiments.Add(new ExperimentData());
        }

        public List<string> CartNames { get; } = new List<string>();
        public List<string> SeparationTypes { get; } = new List<string>();
        public List<string> DatasetTypes { get; } = new List<string>();
        public List<InstrumentInfo> InstrumentInfo { get; } = new List<InstrumentInfo>();
        public List<UserInfo> Users { get; } = new List<UserInfo>();
        public List<string> ColumnNames { get; } = new List<string>();
        public List<ExperimentData> Experiments { get; } = new List<ExperimentData>();
    }
}
