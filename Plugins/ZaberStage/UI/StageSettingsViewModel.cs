using System;
using System.Linq;
using ReactiveUI;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage.UI
{
    public class StageSettingsViewModel : ReactiveObject
    {
        [Obsolete("For WPF Design time use only.", true)]
        public StageSettingsViewModel() : this(new StageControl("D"))
        {
        }

        public StageSettingsViewModel(StageControl settings)
        {
            Settings = settings;

            this.WhenAnyValue(x => x.Settings.SerialNumber).Subscribe(x =>
            {
                if (SelectedDevice.SerialNumber != x)
                {
                    var devices = ZaberManager.Instance.ConnectionSerials.Items.ToList();
                    foreach (var device in devices)
                    {
                        if (device.SerialNumber == x)
                        {
                            SelectedDevice = device;
                            break;
                        }
                    }
                }
            });

            this.WhenAnyValue(x => x.SelectedDevice).Subscribe(x =>
            {
                Settings.SerialNumber = x.SerialNumber;
            });
        }

        private ConnectionStageID selectedDevice;

        public StageControl Settings { get; }

        public ConnectionStageID SelectedDevice
        {
            get => selectedDevice;
            set => this.RaiseAndSetIfChanged(ref selectedDevice, value);
        }
    }
}
