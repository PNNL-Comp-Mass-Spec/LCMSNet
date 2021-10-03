using System.Collections.Generic;
using LcmsNetSDK.Method;

namespace LcmsNetSDK.Data
{
    public interface ISampleInfo
    {
        string Name { get; }
        int ColumnIndex { get; }
        string ColumnName { get; }
        PalData PAL { get; }
        double Volume { get; }
        string InstrumentMethod { get; }
        string LCMethodName { get; }
        LCMethod ActualLCMethod { get; }
        List<string[]> GetExportValuePairs();
    }
}
