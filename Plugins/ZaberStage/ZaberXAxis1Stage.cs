using System;
using LcmsNetPlugins.ZaberStage.UI;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    [Serializable]
    [DeviceControl(typeof(X1StageViewModel),
        "X w/ 1 stage",
        "Stages")]
    public class ZaberXAxis1Stage : ZaberStageBase<XAxis1Stage>
    {
        public ZaberXAxis1Stage() : base(new XAxis1Stage())
        {
        }

        internal StageControl XAxis => StageDevice.XAxis;

        [DeviceSavedSetting("XAxisConfig")]
        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value, (message, ex) => ApplicationLogger.LogError(LogLevel.Error, message, ex));
        }
    }
}
