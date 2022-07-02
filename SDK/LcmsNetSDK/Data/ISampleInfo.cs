using System.Collections.Generic;
using LcmsNetSDK.Method;

namespace LcmsNetSDK.Data
{
    public interface ISampleInfo
    {
        string Name { get; }
        int ColumnIndex { get; }
        string ColumnName { get; }
        IPalData PAL { get; }
        double Volume { get; }
        string InstrumentMethod { get; }
        string LCMethodName { get; }
        ILCMethod ActualLCMethod { get; }
        List<string[]> GetExportValuePairs();
    }
}
