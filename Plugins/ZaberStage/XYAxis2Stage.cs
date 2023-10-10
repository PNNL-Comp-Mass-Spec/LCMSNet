using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcmsNetPlugins.ZaberStage.UI;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.ZaberStage
{
    [Serializable]
    [DeviceControl(typeof(XY2StagesViewModel),
        "XY w/ 2 stages",
        "Stages")]
    public class XYAxis2Stage : StageBase
    {
        public XYAxis2Stage() : base(new string[] { "X", "Y" }, "XY_Stage")
        {
        }

        internal StageControl XAxis => StagesUsed[0];
        internal StageControl YAxis => StagesUsed[1];

        [DeviceSavedSetting("XAxisConfig")]
        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value);
        }

        [DeviceSavedSetting("YAxisConfig")]
        public string YAxisConfig
        {
            get => YAxis.GetAxisConfigString();
            set => YAxis.ParseAxisConfigString(value);
        }
    }
}
