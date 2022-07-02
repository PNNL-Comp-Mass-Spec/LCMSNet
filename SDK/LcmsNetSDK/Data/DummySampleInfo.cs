using System.Collections.Generic;
using LcmsNetSDK.Method;

namespace LcmsNetSDK.Data
{
    public class DummySampleInfo : ISampleInfo
    {
        public DummySampleInfo()
        {
            Name = "DummySample";
            ColumnName = "";
            PAL = new DummyPalData();
            ActualLCMethod = new LCMethodDummy();
            InstrumentMethod = "";
            LCMethodName = "";
        }

        public string Name { get; set; }
        public int ColumnIndex { get; set; }
        public string ColumnName { get; set; }
        public DummyPalData PAL { get; }
        IPalData ISampleInfo.PAL => PAL;
        public double Volume { get; set; }
        public string InstrumentMethod { get; set; }
        public string LCMethodName { get; }
        public LCMethodDummy ActualLCMethod { get; }
        ILCMethod ISampleInfo.ActualLCMethod => ActualLCMethod;

        public List<string[]> GetExportValuePairs()
        {
            return new List<string[]>();
        }
    }
}
