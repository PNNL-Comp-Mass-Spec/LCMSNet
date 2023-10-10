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
    [DeviceControl(typeof(XYZ3StagesViewModel),
        "XYZ w/ 3 stages",
        "Stages")]
    public class XYZAxis3Stage : StageBase
    {
        public XYZAxis3Stage() : base(new string[] { "X", "Y", "Z" }, "XYZ_Stage")
        {
        }

        internal StageControl XAxis => StagesUsed[0];
        internal StageControl YAxis => StagesUsed[1];
        internal StageControl ZAxis => StagesUsed[2];

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

        [DeviceSavedSetting("ZAxisConfig")]
        public string ZAxisConfig
        {
            get => ZAxis.GetAxisConfigString();
            set => ZAxis.ParseAxisConfigString(value);
        }
    }
}
