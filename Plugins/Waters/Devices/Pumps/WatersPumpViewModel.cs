using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Controls;
using DynamicData;
using LcmsNetCommonControls.Devices.Pumps;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace Waters.Devices.Pumps
{
    internal class WatersPumpViewModel : BaseDeviceControlViewModel, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private WatersPump mobj_pump;

        public WatersPumpViewModel()
        {
            instrumentList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var instruments).Subscribe();
            InstrumentList = instruments;
            methodList.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methods).Subscribe();
            MethodList = methods;

            OpenWatersConsoleCommand = ReactiveCommand.Create(() => mobj_pump.ShowConsole());
            GetMethodsListCommand = ReactiveCommand.Create(GetListOfMethods);
            StartMethodCommand = ReactiveCommand.Create(StartMethod);
            StopMethodCommand = ReactiveCommand.Create(() => mobj_pump.StopMethod());
            SetComputerNameCommand = ReactiveCommand.Create(SetComputerName);
            SetSystemNameCommand = ReactiveCommand.Create(SetSystemName);
            SetInstrumentCommand = ReactiveCommand.Create(SetInstrument);
            ScanInstrumentsCommand = ReactiveCommand.Create(ScanInstruments);
        }

        private string systemName;
        private string computerName;
        private string statusLabel;
        private string selectedMethod;
        private double methodLength = 6;
        private string selectedInstrument;
        private readonly SourceList<string> instrumentList = new SourceList<string>();
        private readonly SourceList<string> methodList = new SourceList<string>();

        public PumpDisplayViewModel PumpDisplay { get; } = new PumpDisplayViewModel("Unknown");

        public string SystemName { get => systemName; set => this.RaiseAndSetIfChanged(ref systemName, value); }
        public string ComputerName { get => computerName; set => this.RaiseAndSetIfChanged(ref computerName, value); }
        public string StatusLabel { get => statusLabel; set => this.RaiseAndSetIfChanged(ref statusLabel, value); }
        public string SelectedMethod { get => selectedMethod; set => this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        public double MethodLength { get => methodLength; set => this.RaiseAndSetIfChanged(ref methodLength, value); }
        public string SelectedInstrument { get => selectedInstrument; set => this.RaiseAndSetIfChanged(ref selectedInstrument, value); }

        public ReadOnlyObservableCollection<string> InstrumentList { get; }
        public ReadOnlyObservableCollection<string> MethodList { get; }

        public ReactiveCommand<Unit, Unit> OpenWatersConsoleCommand { get; }
        public ReactiveCommand<Unit, Unit> GetMethodsListCommand { get; }
        public ReactiveCommand<Unit, Unit> StartMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> StopMethodCommand { get; }
        public ReactiveCommand<Unit, Unit> SetComputerNameCommand { get; }
        public ReactiveCommand<Unit, Unit> SetSystemNameCommand { get; }
        public ReactiveCommand<Unit, Unit> SetInstrumentCommand { get; }
        public ReactiveCommand<Unit, Unit> ScanInstrumentsCommand { get; }

        public void RegisterDevice(IDevice device)
        {
            mobj_pump = device as WatersPump;
            SetBaseDevice(mobj_pump);

            if (mobj_pump != null)
            {
                mobj_pump.PropertyChanged += PumpOnPropertyChanged;
                mobj_pump.InstrumentsFound += PumpInstrumentsFound;
                mobj_pump.MethodsFound += PumpMethodsFound;
                mobj_pump.MonitoringDataReceived += PumpMonitoringDataReceived;
                mobj_pump.PumpStatus += PumpStatusUpdated;

                PumpDisplay.SetPumpName(mobj_pump.Name);
                ComputerName = mobj_pump.MachineName;
                SystemName = mobj_pump.SystemName;
            }
        }

        private void PumpOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(mobj_pump.Name)))
            {
                PumpDisplay.SetPumpName(mobj_pump.Name);
            }
        }

        private void PumpStatusUpdated(object sender, WatersEventArgs e)
        {
            RxApp.MainThreadScheduler.Schedule(() => StatusLabel = e.Status);
        }

        /// <summary>
        /// Handles the event when data is received from the pumps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PumpMonitoringDataReceived(object sender, PumpDataEventArgs args)
        {
            PumpDisplay.DisplayMonitoringData(sender, args);
        }

        private void PumpInstrumentsFound(object sender, WatersEventArgs e)
        {
            instrumentList.Edit(list =>
            {
                list.Clear();
                list.AddRange(e.Data);
            });
        }

        private void PumpMethodsFound(object sender, WatersEventArgs e)
        {
            methodList.Edit(list =>
            {
                list.Clear();
                list.AddRange(e.Data);
            });

            if (e.Data.Count > 0)
            {
                RxApp.MainThreadScheduler.Schedule(() => SelectedMethod = e.Data[0]);
            }
        }

        public override UserControl GetDefaultView()
        {
            return new WatersPumpView();
        }

        public override IDevice Device
        {
            get => mobj_pump;
            set => RegisterDevice(value);
        }

        private void StartMethod()
        {
            string methodName = "";
            double timeout = MethodLength;
            if (SelectedMethod != null)
            {
                ApplicationLogger.LogMessage(0, string.Format("Manually starting Waters Method {0} for {1} mins.", methodName, timeout));
                methodName = SelectedMethod;
                mobj_pump.StartMethod(timeout, methodName);
            }
            else
            {
                ApplicationLogger.LogMessage(0, "Cannot start a method when no method is selected.");
            }
        }

        private void GetListOfMethods()
        {
            List<string> methods = mobj_pump.GetMethodList();
        }

        private void SetInstrument()
        {
            if (SelectedInstrument != null)
            {
                mobj_pump.Instrument = SelectedInstrument;
            }
            else
            {
                ApplicationLogger.LogError(0, "You have to select a nano BSM pump first.");
            }
        }

        private void ScanInstruments()
        {
            ApplicationLogger.LogMessage(0, "Scanning network for Waters pump list.");
            List<string> instruments = mobj_pump.GetInstrumentList();
            ApplicationLogger.LogMessage(0, string.Format("Found {0} Water's Nano BSM pumps.", instruments.Count));
        }

        private void SetComputerName()
        {
            mobj_pump.MachineName = ComputerName;
        }

        private void SetSystemName()
        {
            mobj_pump.SystemName = SystemName;
        }
    }
}
