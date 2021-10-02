using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    public class QueueCacheCSV
    {


        public sealed class SampleCacheMap : ClassMap<SampleData>
        {
            public SampleCacheMap()
            {
                var index = 0;
                // SequenceID
                // Column ID
                // Unique ID?
                Map(x => x.DmsData.DatasetName).Name("Sample", "Request Name", "Sample Name", "Dataset Name", "Dataset", "Name").Index(index++);
                Map(x => x.PAL.Well).Name("PAL Vial", "Vial", "Well").Index(index++).Default("");
                Map(x => x.PAL.PALTray).Name("PAL Tray", "Tray").Index(index++).Default("");
                Map(x => x.Volume).Name("Volume").Index(index++).Default(0);
                Map(x => x.LCMethodName).Name("Method", "LC Method", "LCMethod").Index(index++).Default("");
                //Map(x => x.DmsData.RunOrder).Name("Run Order").Index(index++).Default(-1);
                //Map(x => x.DmsData.RequestID).Name("Request ID").Index(index++).Default(0);
                // RunningStatus?
            }
        }
    }
}
