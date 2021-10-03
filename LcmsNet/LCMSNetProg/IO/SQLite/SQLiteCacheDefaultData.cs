using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace LcmsNet.IO.SQLite
{
    public class SQLiteCacheDefaultData
    {
        public SQLiteCacheDefaultData()
        {
            CartNames.Add("default");
        }

        public List<string> CartNames { get; } = new List<string>();
    }
}
