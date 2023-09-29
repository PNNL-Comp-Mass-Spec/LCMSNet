using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using DynamicData;
using LcmsNetCommonControls.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;
using ReactiveUI;

namespace LcmsNetPlugins.XcaliburLC
{
    public class XcaliburLCViewModel : BaseDeviceControlViewModelReactive, IDeviceControl, IDisposable
    {

        private const string DefaultLcMethodPath = @"C:\Xcalibur\methods";

        /// <summary>
        /// The default Constructor.
        /// </summary>
        public XcaliburLCViewModel()
        {
            methodComboBoxOptions.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var methodComboBoxOptionsBound).Subscribe();
            MethodComboBoxOptions = methodComboBoxOptionsBound;

            LoadMethodsCommand = ReactiveCommand.Create(LoadMethods);
            RefreshInfoCommand = ReactiveCommand.Create(() =>
            {
                var names = Pump.XcaliburCom.GetDevices();
                if (names.Count > 0)
                {
                    XcaliburDevices = string.Join(", ", names);
                }
                else
                {
                    XcaliburDevices = "No devices configured!";
                }
            });
            RefreshStatusCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    Status = Pump.XcaliburCom.GetRunStatus();
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(LogLevel.Error, "Failed call to read status", ex);
                }
                //Pump.GetPumpState();
            });
            GetDeviceStatusCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    XcaliburDeviceStatus = Pump.XcaliburCom.GetDeviceStatus();
                }
                catch (Exception ex)
                {
                    XcaliburDeviceStatus = ex.ToString();
                }
            });
            GetDeviceInfoCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    XcaliburDeviceInfo = Pump.XcaliburCom.GetDeviceInfoString();
                }
                catch (Exception ex)
                {
                    XcaliburDeviceInfo = ex.ToString();
                }
            });
            StartPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(StartPump));
            StopPumpCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => Pump.StopMethod()));
            ExportMethodTextCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(() => WriteMethodToFile()));
        }

        ~XcaliburLCViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public override UserControl GetDefaultView()
        {
            return new XcaliburLCView();
        }

        /// <summary>
        /// A pump object to use.
        /// </summary>
        private XcaliburLCPump pump;

        private readonly SourceList<string> methodComboBoxOptions = new SourceList<string>();
        private string status = "";
        private string xcaliburDevices = "";
        private string selectedMethod = "";
        private string xcaliburDeviceStatus = "";
        private string xcaliburDeviceInfo = "";

        public override IDevice Device
        {
            get => pump;
            set => RegisterDevice(value);
        }

        public XcaliburLCPump Pump
        {
            get => pump;
            private set => this.RaiseAndSetIfChanged(ref pump, value);
        }

        public ReadOnlyObservableCollection<string> MethodComboBoxOptions { get; }

        public string SelectedMethod
        {
            get => selectedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
        }

        public string Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }

        public string XcaliburDevices
        {
            get => xcaliburDevices;
            set => this.RaiseAndSetIfChanged(ref xcaliburDevices, value);
        }

        public string XcaliburDeviceStatus
        {
            get => xcaliburDeviceStatus;
            set => this.RaiseAndSetIfChanged(ref xcaliburDeviceStatus, value);
        }

        public string XcaliburDeviceInfo
        {
            get => xcaliburDeviceInfo;
            set => this.RaiseAndSetIfChanged(ref xcaliburDeviceInfo, value);
        }

        /// <summary>
        /// Determines whether or not pump is in emulation mode
        /// </summary>
        public bool Emulation
        {
            get => Pump.Emulation;
            set => Pump.Emulation = value;
        }

        public ReactiveCommand<Unit, Unit> LoadMethodsCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> GetDeviceStatusCommand { get; }
        public ReactiveCommand<Unit, Unit> GetDeviceInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> StartPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> StopPumpCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportMethodTextCommand { get; }

        private void RegisterDevice(IDevice device)
        {
            Pump = device as XcaliburLCPump;

            // Initialize the underlying device class
            if (Pump != null)
            {
                Pump.MethodNames += PumpOnMethodNames;

                methodComboBoxOptions.Edit(list =>
                {
                    list.Clear();
                    list.AddRange(Pump.GetMethodNames());
                });
            }

            // Add to the device manager.
            SetBaseDevice(Pump);
        }

        /// <summary>
        /// Handles when new pump method names are available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void PumpOnMethodNames(object sender, List<object> data)
        {
            methodComboBoxOptions.Edit(list =>
            {
                list.Clear();
                list.AddRange(data.Select(x => x.ToString()));
            });

            // Make sure one method is selected.
            if (methodComboBoxOptions.Count > 0 && !methodComboBoxOptions.Items.Contains(SelectedMethod))
            {
                SelectedMethod = methodComboBoxOptions.Items.First();
            }
        }

        /// <summary>
        /// Starts the pumps currently loaded time table.
        /// </summary>
        private void StartPump()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                return;
            }

            Pump.StartMethod(SelectedMethod);
        }

        private void LoadMethods()
        {
            try
            {
                // The reason we don't just add stuff straight into the user interface here, is to maintain the
                // design pattern that things propagate events to us, since we are not in charge of managing the
                // data.  We will catch an event from adding a method that one was added...and thus update
                // the user interface intrinsically.
                string methodSelected = null;
                if (MethodComboBoxOptions.Count > 0)
                {
                    methodSelected = SelectedMethod;
                }

                Pump.ReadMethodDirectory();

                if (methodSelected != null)
                {
                    // try to select the last selected method, if it has been loaded back in to the system.
                    SelectedMethod = MethodComboBoxOptions.Contains(methodSelected) ? methodSelected : "";
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                ApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        private void WriteMethodToFile()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                return;
            }

            var methodText = Pump.GetMethodText(SelectedMethod);
            var path = Path.Combine(LCMSSettings.GetParameter(LCMSSettings.PARAM_APPLICATIONDATAPATH), DeviceManager.CONST_PUMP_METHOD_PATH, $"{SelectedMethod}_export.txt");

            File.WriteAllText(path, methodText);
            ApplicationLogger.LogMessage(LogLevel.Info, $"Wrote text for method {SelectedMethod} to file '{path}'");
        }
    }
}
