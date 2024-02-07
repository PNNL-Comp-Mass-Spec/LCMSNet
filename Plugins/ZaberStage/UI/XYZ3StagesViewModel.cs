using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetPlugins.ZaberStage.UI
{
    public class XYZ3StagesViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        private readonly bool isInDesignMode = false;
        private ZaberXYZAxis3Stage stage;
        private StageControlViewModel xStageVm;
        private StageControlViewModel yStageVm;
        private StageControlViewModel zStageVm;
        private bool controlTabSelected = true;

        public override IDevice Device
        {
            get => stage;
            set
            {
                stage = value as ZaberXYZAxis3Stage;
                if (stage != null && !isInDesignMode)
                {
                    RegisterDevice(value);
                }
            }
        }

        public StageConfigViewModel ConfigVm { get; } = new StageConfigViewModel();

        public StageControlViewModel XStageVM { get => xStageVm; set => this.RaiseAndSetIfChanged(ref xStageVm, value); }
        public StageControlViewModel YStageVM { get => yStageVm; set => this.RaiseAndSetIfChanged(ref yStageVm, value); }
        public StageControlViewModel ZStageVM { get => zStageVm; set => this.RaiseAndSetIfChanged(ref zStageVm, value); }
        public bool ControlTabSelected { get => controlTabSelected; set => this.RaiseAndSetIfChanged(ref controlTabSelected, value); }

        public event EventHandler DeviceControlsOutOfView;

        public XYZ3StagesViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            this.WhenAnyValue(x => x.ControlTabSelected, x => x.DeviceTabSelected).Where(x => !x.Item1 || !x.Item2).Subscribe(x => DeviceControlsOutOfView?.Invoke(this, EventArgs.Empty));
        }

        public override UserControl GetDefaultView()
        {
            return new XYZ3StagesView();
        }

        /// <summary>
        /// Used to disable conditional control without clearing selection states
        /// </summary>
        public override void OutOfView()
        {
            DeviceControlsOutOfView?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Registers the device events and user interface.
        /// </summary>
        /// <param name="device"></param>
        private void RegisterDevice(IDevice device)
        {
            stage = device as ZaberXYZAxis3Stage;

            if (stage != null)
            {
                //Pal.DeviceSaveRequired += Pal_DeviceSaveRequired;
                //Pal.Free += OnFree;

                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    ConfigVm.Stage = stage;
                    XStageVM = new StageControlViewModel(stage.XAxis);
                    YStageVM = new StageControlViewModel(stage.YAxis);
                    ZStageVM = new StageControlViewModel(stage.ZAxis);
                    //if (Pal.TrayNames.Count == 0)
                    //{
                    //    Pal.ListTrays();
                    //    Pal.ListMethods();
                    //    Pal.SetMaxVialsForTrays();
                    //}
                    //
                    //ProcessTrays(Pal.TrayNames);
                    //ProcessMethods(Pal.MethodNames);
                    //ProcessTraysAndMaxVials(Pal.TrayNamesAndMaxVials);
                });
            }

            SetBaseDevice(stage);
        }
    }
}
