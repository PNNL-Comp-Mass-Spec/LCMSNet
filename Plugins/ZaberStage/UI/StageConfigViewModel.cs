using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using LcmsNetCommonControls.Controls;
using ReactiveUI;

namespace LcmsNetPlugins.ZaberStage.UI
{
    public class StageConfigViewModel : ReactiveObject
    {
        private StageBase stage;
        private string selectedPortName = "";
        private IReadOnlyList<StageSettingsViewModel> stageAxes;

        public StageBase Stage
        {
            get => stage;
            set
            {
                stage = value;

                if (stage != null)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(stage.PortName))
                        {
                            StageConnectionManager.Instance.ReadStagesForConnection(stage.PortName);
                        }

                        SelectedPortName = stage.PortName;
                        StageAxes = stage.StagesUsed.Select(x => new StageSettingsViewModel(x)).ToList();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        public ReadOnlyObservableCollection<SerialPortData> PortNamesComboBoxOptions => SerialPortGenericData.SerialPorts;
        public ReadOnlyObservableCollection<ConnectionStageID> PortDevices { get; }

        public string SelectedPortName
        {
            get => selectedPortName;
            set => this.RaiseAndSetIfChanged(ref selectedPortName, value);
        }

        public IReadOnlyList<StageSettingsViewModel> StageAxes
        {
            get => stageAxes;
            set => this.RaiseAndSetIfChanged(ref stageAxes, value);
        }
        public ReactiveCommand<Unit, Unit> ApplyPortNameCommand { get; }

        public StageConfigViewModel()
        {
            var filter = this.WhenValueChanged(x => x.SelectedPortName).Select(x => new Func<ConnectionStageID, bool>(y => !string.IsNullOrWhiteSpace(x) && x.Equals(y.PortName)));
            StageConnectionManager.Instance.ConnectionSerials.Connect().Filter(filter).ObserveOn(RxApp.MainThreadScheduler).Bind(out var portDevices).Subscribe();
            PortDevices = portDevices;

            ApplyPortNameCommand = ReactiveCommand.Create(ApplyPortName);
        }

        private void ApplyPortName()
        {
            Stage.PortName = SelectedPortName;

            if (!string.IsNullOrWhiteSpace(SelectedPortName))
            {
                StageConnectionManager.Instance.ReadStagesForConnection(SelectedPortName);
            }

            //StatusText = "Port name changed to " + pal.PortName;
        }
    }
}
