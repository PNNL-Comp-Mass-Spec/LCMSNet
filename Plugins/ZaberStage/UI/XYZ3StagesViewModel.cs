using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
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
        private XYZAxis3Stage stage;
        private StageControlViewModel xStageVm;
        private StageControlViewModel yStageVm;
        private StageControlViewModel zStageVm;

        public override IDevice Device
        {
            get => stage;
            set
            {
                stage = value as XYZAxis3Stage;
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

        public XYZ3StagesViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public override UserControl GetDefaultView()
        {
            return new XYZ3StagesView();
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
            stage = device as XYZAxis3Stage;

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
