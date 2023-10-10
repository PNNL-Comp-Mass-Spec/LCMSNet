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
    [DeviceControl(typeof(X1StageViewModel),
        "X w/ 1 stage",
        "Stages")]
    public class XAxis1Stage : StageBase
    {
        public XAxis1Stage() : base(new string[] { "X" }, "X_Stage")
        {
        }

        internal StageControl XAxis => StagesUsed[0];

        [DeviceSavedSetting("XAxisConfig")]
        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value);
        }
    }
}
