using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using LcmsNetCommonControls.Devices;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class AdvancedDeviceGroupControlViewModel : ReactiveObject, IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AdvancedDeviceGroupControlViewModel()
        {
            deviceToControlMap = new Dictionary<IDevice, IDeviceControl>();
            deviceToWrapperMap = new Dictionary<IDevice, DeviceControlData>();

            SelectedDevice = null;

            SelectedColor = Colors.Red;
            NotSelectedColor = Colors.Black;

            RenameDeviceCommand = ReactiveCommand.Create(RenameSelectedDevice, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            InitializeDeviceCommand = ReactiveCommand.CreateFromTask(InitializeSelectedDevice, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            ClearErrorCommand = ReactiveCommand.Create(ClearSelectedDeviceError, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));

            // Once an item is added to the device controls, automatically make the first one the "Selected device" if one hasn't been previously set.
            this.WhenAnyValue(x => x.deviceControls, x => x.SelectedDevice, x => x.deviceControls.Count).Where(x => x.Item1.Count > 0 && x.Item2 == null)
                .Subscribe(x => SelectedDevice = x.Item1[0]);
        }

        ~AdvancedDeviceGroupControlViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var deviceControl in DeviceControls)
            {
                deviceControl.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public class DeviceControlData : ReactiveObject, IDisposable
        {
            private System.Windows.Controls.UserControl view = null;
            private ObservableAsPropertyHelper<string> name;
            private ObservableAsPropertyHelper<string> status;
            private string nameEdit = "";

            public IDevice Device { get; private set; }
            public IDeviceControl ViewModel { get; private set; }

            public System.Windows.Controls.UserControl View
            {
                get
                {
                    if (view == null)
                    {
                        view = ViewModel.GetDefaultView();
                        view.DataContext = ViewModel;
                    }
                    return view;
                }
            }

            public string Name => name?.Value ?? string.Empty;

            public string Status => status?.Value ?? string.Empty;

            public string NameEdit
            {
                get => nameEdit;
                set => this.RaiseAndSetIfChanged(ref nameEdit, value);
            }

            public DeviceControlData(IDevice device, IDeviceControl viewModel)
            {
                Device = device;
                ViewModel = viewModel;
                //ViewModel.WhenAnyValue(x => x.Name).ToProperty(this, x => x.Name, out name);
                Device.WhenAnyValue(x => x.Name).ToProperty(this, x => x.Name, out name);
                Device.WhenAnyValue(x => x.Name).Subscribe(x => NameEdit = x);

                if (viewModel is BaseDeviceControlViewModel vm)
                {
                    vm.WhenAnyValue(x => x.DeviceStatus).ToProperty(this, x => x.Status, out status);
                }
                else if (viewModel is BaseDeviceControlViewModelReactive vmr)
                {
                    vmr.WhenAnyValue(x => x.DeviceStatus).ToProperty(this, x => x.Status, out status);
                }
            }

            ~DeviceControlData()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (ViewModel is IDisposable dis)
                {
                    dis.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }

        private readonly Dictionary<IDevice, IDeviceControl> deviceToControlMap;
        private readonly Dictionary<IDevice, DeviceControlData> deviceToWrapperMap;
        private readonly ReactiveList<DeviceControlData> deviceControls = new ReactiveList<DeviceControlData>();
        private DeviceControlData selectedDevice = null;

        public IReadOnlyReactiveList<DeviceControlData> DeviceControls => deviceControls;

        /// <summary>
        /// Selected device
        /// </summary>
        public DeviceControlData SelectedDevice
        {
            get => selectedDevice;
            set => this.RaiseAndSetIfChanged(ref selectedDevice, value);
        }

        /// <summary>
        /// Determines if there are any devices in this group.
        /// </summary>
        public bool IsDeviceGroupEmpty => deviceControls.Count < 1;

        public Color SelectedColor { get; set; }

        public Color NotSelectedColor { get; set; }

        public ReactiveCommand<Unit, Unit> RenameDeviceCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> InitializeDeviceCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ClearErrorCommand { get; private set; }

        private async Task InitializeSelectedDevice()
        {
            if (SelectedDevice == null)
                return;

            var message = "";

            try
            {
                await Task.Run(() => DeviceManager.Manager.InitializeDevice(SelectedDevice.Device));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, message, ex);
                ApplicationLogger.LogError(0, ex.Message);
            }
            //bool wasOk = SelectedDevice.Initialize(ref message);
            //if (!wasOk)
            //{
            //    ApplicationLogger.LogError(0, message);
            //}
        }

        private void RenameSelectedDevice()
        {
            if (SelectedDevice == null)
                return;

            RenameDevice(SelectedDevice.Device, SelectedDevice.NameEdit);
        }

        private void RenameDevice(IDevice device, string newName)
        {
            // It's the same name so don't mess with it
            if (SelectedDevice.Name == newName)
                return;

            DeviceManager.Manager.RenameDevice(device, newName);

            // Update the user interface with the new name
            SelectedDevice.NameEdit = device.Name;
        }

        private void ClearSelectedDeviceError()
        {
            if (SelectedDevice != null)
                SelectedDevice.Device.Status = DeviceStatus.Initialized;
        }

        #region Public Methods For Adding and Removing Devices for the UI

        public void RemoveDevice(IDevice device)
        {
            if (deviceToWrapperMap.ContainsKey(device))
            {
                // Remove from maps
                var deviceControl = deviceToWrapperMap[device];
                deviceControls.Remove(deviceControl);

                deviceToControlMap.Remove(device);
                deviceToWrapperMap.Remove(device);
            }
        }

        /// <summary>
        /// Adds the device to the panel for us.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="control"></param>
        public void AddDevice(IDevice device, IDeviceControl control)
        {
            var deviceControl = new DeviceControlData(device, control);
            deviceControls.Add(deviceControl);

            deviceToControlMap.Add(device, control);
            deviceToWrapperMap.Add(device, deviceControl);
        }

        #endregion
    }
}
