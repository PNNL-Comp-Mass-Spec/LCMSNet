using System.Collections.Generic;
using LcmsNet.IO.DMS.Data;

// ReSharper disable UnusedMember.Global

namespace LcmsNet.IO.SQLite
{
    public class SQLiteCacheDefaultData
    {
        public SQLiteCacheDefaultData()
        {
            CartNames.Add("default");
            SeparationTypes.Add("default");
            DatasetTypes.Add("default");
            ColumnNames.AddRange(new [] { "0", "1", "2", "3", "4" });
        }

        public List<string> CartNames { get; } = new List<string>();
        public List<string> SeparationTypes { get; } = new List<string>();
        public List<string> DatasetTypes { get; } = new List<string>();
        public List<string> ColumnNames { get; } = new List<string>();
    }
}
