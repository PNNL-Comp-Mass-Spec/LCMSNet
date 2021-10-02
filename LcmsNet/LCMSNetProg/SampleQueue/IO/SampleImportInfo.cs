using LcmsNet.IO.DMS.Data;
using LcmsNetData.Data;
using LcmsNetSDK.Data;

namespace LcmsNet.SampleQueue.IO
{
    public class SampleImportInfo
    {
        public string DatasetName { get; set; }
        public int RequestId { get; set; } = -1;
        public int RunOrder { get; set; } = -1;
        public string PalTray { get; set; } = "";
        public string PalVial { get; set; } = "";
        public double Volume { get; set; } = 0;
        public string LcMethod { get; set; } = "";

        public SampleData GetSampleData()
        {
            var sampleData = new SampleData(false);
            sampleData.DmsData.DatasetName = DatasetName;

            if (RequestId > 0)
            {
                sampleData.DmsData.RequestID = RequestId;
            }

            if (RunOrder >= 0)
            {
                sampleData.DmsData.RunOrder = RunOrder;
            }

            if (!string.IsNullOrWhiteSpace(PalTray))
            {
                sampleData.PAL.PALTray = PalTray;
            }

            if (!string.IsNullOrWhiteSpace(PalVial))
            {
                if (int.TryParse(PalVial, out var vialNum))
                {
                    sampleData.PAL.Well = vialNum;
                }
                else
                {
                    // TODO: Hazard - currently will only work properly for 96-well plates
                    sampleData.PAL.Well = ConvertVialPosition.ConvertVialToInt(PalVial);
                }
            }

            if (Volume > 0)
            {
                sampleData.Volume = Volume;
            }

            if (!string.IsNullOrWhiteSpace(LcMethod))
            {
                // TODO: Maybe verify that the name is valid first? Even do some case-insensitive matching?
                sampleData.LCMethodName = LcMethod;
            }

            return sampleData;
        }
    }
}