using DynamicData;
using System;
using LcmsNetSDK.Logging;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage
{
    internal class ZaberManager
    {
        public static ZaberManager Instance { get; } = new ZaberManager();

        private ZaberManager()
        {
            StageConnectionManager.Instance.AvailableDevicesUpdated += OnAvailableDevicesUpdated;
        }

        public SourceList<ConnectionStageID> ConnectionSerials { get; } = new SourceList<ConnectionStageID>();

        public void ReadStagesForConnection(string portName)
        {
            StageConnectionManager.Instance.ReadStagesForConnection(portName, (message, ex) => ApplicationLogger.LogError(LogLevel.Warning, message, ex));
        }

        private void OnAvailableDevicesUpdated(object sender, EventArgs e)
        {
            ConnectionSerials.Edit(list =>
            {
                list.Clear();
                list.AddRange(StageConnectionManager.Instance.ConnectionSerials);
            });
        }
    }
}
