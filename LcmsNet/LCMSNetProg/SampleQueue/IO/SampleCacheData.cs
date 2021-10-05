using CsvHelper.Configuration;
using LcmsNet.Data;

namespace LcmsNet.SampleQueue.IO
{
    public class SampleCacheData
    {
        public SampleCacheData()
        {
            ID = 0;
            Name = "";
            Volume = 0;
            PalWell = 0;
            PalTray = "";
            LcMethod = "";
            RequestId = 0;
        }

        public SampleCacheData(SampleData sample)
        {
            ID = sample.SequenceID;
            Name = sample.Name;
            Volume = sample.Volume;
            PalWell = sample.PAL.Well;
            PalTray = sample.PAL.PALTray;
            LcMethod = sample.LCMethodName;
            RequestId = sample.DmsRequestId;
        }

        public SampleData AsSampleData()
        {
            return new SampleData(false)
            {
                SequenceID = ID,
                Name = Name,
                Volume = Volume,
                PAL = { Well = PalWell, PALTray = PalTray },
                LCMethodName = LcMethod,
                DmsRequestId = RequestId
            };
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public double Volume { get; set; }
        public int PalWell { get; set; }
        public string PalTray { get; set; }
        public string LcMethod { get; set; }
        public int RequestId { get; set; }

        public sealed class SampleCacheMap : ClassMap<SampleCacheData>
        {
            public SampleCacheMap()
            {
                var index = 0;
                // SequenceID
                Map(x => x.Name).Name("Sample", "Request Name", "Sample Name", "Dataset Name", "Dataset", "Name").Index(index++);
                Map(x => x.PalWell).Name("PALWell", "PAL Well", "PAL Vial", "Vial", "Well").Index(index++).Default(0);
                Map(x => x.PalTray).Name("PALTray", "PAL Tray", "Tray").Index(index++).Default("");
                Map(x => x.Volume).Name("Volume").Index(index++).Default(0);
                Map(x => x.LcMethod).Name("Method", "LC Method", "LCMethod").Index(index++).Default("");
                Map(x => x.ID).Name("ID", "SequenceID").Index(index++).Default(0);
                Map(x => x.RequestId).Name("RequestID", "Request ID").Index(index++).Default(0);
            }
        }
    }
}
