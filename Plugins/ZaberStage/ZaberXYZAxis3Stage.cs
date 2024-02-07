using System;
using LcmsNetPlugins.ZaberStage.UI;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    [Serializable]
    [DeviceControl(typeof(XYZ3StagesViewModel),
        "XYZ w/ 3 stages",
        "Stages")]
    public class ZaberXYZAxis3Stage : ZaberStageBase<XYZAxis3Stage>
    {
        public ZaberXYZAxis3Stage() : base(new XYZAxis3Stage())
        {
        }

        internal StageControl XAxis => StageDevice.XAxis;
        internal StageControl YAxis => StageDevice.YAxis;
        internal StageControl ZAxis => StageDevice.ZAxis;

        [DeviceSavedSetting("XAxisConfig")]
        public string XAxisConfig
        {
            get => XAxis.GetAxisConfigString();
            set => XAxis.ParseAxisConfigString(value, (message, ex) => ApplicationLogger.LogError(LogLevel.Error, message, ex));
        }

        [DeviceSavedSetting("YAxisConfig")]
        public string YAxisConfig
        {
            get => YAxis.GetAxisConfigString();
            set => YAxis.ParseAxisConfigString(value, (message, ex) => ApplicationLogger.LogError(LogLevel.Error, message, ex));
        }

        [DeviceSavedSetting("ZAxisConfig")]
        public string ZAxisConfig
        {
            get => ZAxis.GetAxisConfigString();
            set => ZAxis.ParseAxisConfigString(value, (message, ex) => ApplicationLogger.LogError(LogLevel.Error, message, ex));
        }
    }
}
