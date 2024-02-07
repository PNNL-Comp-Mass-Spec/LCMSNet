using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using LcmsNetCommonControls.Devices;
using LcmsNetPlugins.ZaberStage.UI;
using LcmsNetSDK.Devices;
using ReactiveUI;
using ZaberStageControl;

namespace LcmsNetPlugins.ZaberStage.Devices
{
    internal class TwoColumnTipPositionerViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {
        private readonly bool isInDesignMode = false;
        private TwoColumnTipPositioner stage;
        private StageControlViewModel xStageVm;
        private StageControlViewModel yStageVm;
        private StageControlViewModel zStageVm;
        private bool controlTabSelected = true;
        private ObservableAsPropertyHelper<double> yAxisMoveOffsetMM;
        private double newYAxisMoveOffsetMM = 0;

        public override IDevice Device
        {
            get => stage;
            set
            {
                Stage = value as TwoColumnTipPositioner;
                if (Stage != null && !isInDesignMode)
                {
                    RegisterDevice(value);
                }
            }
        }

        public StageConfigViewModel ConfigVm { get; } = new StageConfigViewModel();

        public TwoColumnTipPositioner Stage { get => stage; set => this.RaiseAndSetIfChanged(ref stage, value); }
        public StageControlViewModel XStageVM { get => xStageVm; set => this.RaiseAndSetIfChanged(ref xStageVm, value); }
        public StageControlViewModel YStageVM { get => yStageVm; set => this.RaiseAndSetIfChanged(ref yStageVm, value); }
        public StageControlViewModel ZStageVM { get => zStageVm; set => this.RaiseAndSetIfChanged(ref zStageVm, value); }
        public bool ControlTabSelected { get => controlTabSelected; set => this.RaiseAndSetIfChanged(ref controlTabSelected, value); }
        public double YAxisMoveOffsetMM => yAxisMoveOffsetMM.Value;
        public double NewYAxisMoveOffsetMM { get => newYAxisMoveOffsetMM; set => this.RaiseAndSetIfChanged(ref newYAxisMoveOffsetMM, value); }
        public ReactiveCommand<Unit, Unit> SetPosition1Command { get; }
        public ReactiveCommand<Unit, Unit> GotoPosition1Command { get; }
        public ReactiveCommand<Unit, Unit> SetPosition2Command { get; }
        public ReactiveCommand<Unit, Unit> GotoPosition2Command { get; }
        public ReactiveCommand<Unit, Unit> SetYAxisMoveOffsetCommand { get; }

        public event EventHandler DeviceControlsOutOfView;

        public TwoColumnTipPositionerViewModel()
        {
            isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

            this.WhenAnyValue(x => x.ControlTabSelected, x => x.DeviceTabSelected).Where(x => !x.Item1 || !x.Item2).Subscribe(x => DeviceControlsOutOfView?.Invoke(this, EventArgs.Empty));
            yAxisMoveOffsetMM = this.WhenAnyValue(x => x.Stage, x => x.Stage.YAxisMoveOffsetMM).Select(x => x.Item2).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.YAxisMoveOffsetMM);

            SetPosition1Command = ReactiveCommand.Create(SetPosition1);
            GotoPosition1Command = ReactiveCommand.Create(GotoPosition1);
            SetPosition2Command = ReactiveCommand.Create(SetPosition2);
            GotoPosition2Command = ReactiveCommand.Create(GotoPosition2);
            SetYAxisMoveOffsetCommand = ReactiveCommand.Create(SetYAxisMoveOffset);
        }

        public override UserControl GetDefaultView()
        {
            return new TwoColumnTipPositionerView();
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
            stage = device as TwoColumnTipPositioner;

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

                    NewYAxisMoveOffsetMM = stage.YAxisMoveOffsetMM;

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

        private void ReadAllPositions()
        {
            XStageVM.ReadPosition();
            YStageVM.ReadPosition();
            ZStageVM.ReadPosition();
        }

        private void SetPosition1()
        {
            Stage.Position1 = new Position("1", XStageVM.GetPositionMM(), YStageVM.GetPositionMM(), ZStageVM.GetPositionMM());
        }

        private void GotoPosition1()
        {
            Stage.MoveToPosition1();
            ReadAllPositions();
        }

        private void SetPosition2()
        {
            Stage.Position2 = new Position("2", XStageVM.GetPositionMM(), YStageVM.GetPositionMM(), ZStageVM.GetPositionMM());
        }

        private void GotoPosition2()
        {
            Stage.MoveToPosition2();
            ReadAllPositions();
        }

        private void SetYAxisMoveOffset()
        {
            Stage.YAxisMoveOffsetMM = NewYAxisMoveOffsetMM;
        }
    }
}
