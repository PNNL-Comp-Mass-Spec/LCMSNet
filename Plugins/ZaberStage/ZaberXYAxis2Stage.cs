using System;
using LcmsNetPlugins.ZaberStage.UI;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    [Serializable]
    [DeviceControl(typeof(XY2StagesViewModel),
        "XY w/ 2 stages",
        "Stages")]
    public class ZaberXYAxis2Stage : ZaberStageBase<XYAxis2Stage>
    {
        public ZaberXYAxis2Stage() : base(new XYAxis2Stage())
        {
        }

        internal StageControl XAxis => StageDevice.XAxis;
        internal StageControl YAxis => StageDevice.YAxis;

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
    }
}
