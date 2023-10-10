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
    public class X1StageViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        private readonly bool isInDesignMode = false;
        private XAxis1Stage stage;
        private StageControlViewModel xStageVm;

        public override IDevice Device
        {
            get => stage;
            set
            {
                stage = value as XAxis1Stage;
                if (stage != null && !isInDesignMode)
                {
                    RegisterDevice(value);
                }
            }
        }

        public StageConfigViewModel ConfigVm { get; } = new StageConfigViewModel();

        public StageControlViewModel XStageVM { get => xStageVm; set => this.RaiseAndSetIfChanged(ref xStageVm, value); }

        public X1StageViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public override UserControl GetDefaultView()
        {
            return new X1StageView();
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
            stage = device as XAxis1Stage;

            if (stage != null)
            {
                //Pal.DeviceSaveRequired += Pal_DeviceSaveRequired;
                //Pal.Free += OnFree;

                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    ConfigVm.Stage = stage;
                    XStageVM = new StageControlViewModel(stage.XAxis);
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
