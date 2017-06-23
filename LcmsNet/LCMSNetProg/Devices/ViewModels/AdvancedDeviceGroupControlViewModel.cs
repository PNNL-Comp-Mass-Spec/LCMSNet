using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class AdvancedDeviceGroupControlViewModel : ReactiveObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AdvancedDeviceGroupControlViewModel()
        {
            deviceToControlMap = new Dictionary<IDevice, IDeviceControlWpf>();
            deviceToWrapperMap = new Dictionary<IDevice, DeviceControlData>();

            SelectedDevice = null;

            SelectedColor = Colors.Red;
            NotSelectedColor = Colors.Black;

            SetupCommands();
        }

        public class DeviceControlData : ReactiveObject
        {
            private System.Windows.Controls.UserControl view = null;
            private ObservableAsPropertyHelper<string> name;
            private ObservableAsPropertyHelper<string> status;
            private string nameEdit = "";

            public IDevice Device { get; private set; }
            public IDeviceControlWpf ViewModel { get; private set; }

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

            public string Name
            {
                get { return name?.Value ?? string.Empty; }
            }

            public string Status
            {
                get { return status?.Value ?? string.Empty; }
            }

            public string NameEdit
            {
                get { return nameEdit; }
                set { this.RaiseAndSetIfChanged(ref nameEdit, value); }
            }

            public DeviceControlData(IDevice device, IDeviceControlWpf viewModel)
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
            }
        }

        private readonly Dictionary<IDevice, IDeviceControlWpf> deviceToControlMap;
        private readonly Dictionary<IDevice, DeviceControlData> deviceToWrapperMap;
        private readonly ReactiveList<DeviceControlData> deviceControls = new ReactiveList<DeviceControlData>();
        private DeviceControlData selectedDevice = null;

        public IReadOnlyReactiveList<DeviceControlData> DeviceControls => deviceControls;

        /// <summary>
        /// Selected device
        /// </summary>
        public DeviceControlData SelectedDevice
        {
            get { return selectedDevice; }
            set { this.RaiseAndSetIfChanged(ref selectedDevice, value); }
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

        private void SetupCommands()
        {
            RenameDeviceCommand = ReactiveCommand.Create(() => RenameSelectedDevice(), this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            InitializeDeviceCommand = ReactiveCommand.Create(() => InitializeSelectedDevice(), this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            ClearErrorCommand = ReactiveCommand.Create(() => ClearSelectedDeviceError(), this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
        }

        private void InitializeSelectedDevice()
        {
            if (SelectedDevice == null)
                return;

            var message = "";

            try
            {
                classDeviceManager.Manager.InitializeDevice(SelectedDevice.Device);
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, message, ex);
                classApplicationLogger.LogError(0, ex.Message);
            }
            //bool wasOk = SelectedDevice.Initialize(ref message);
            //if (!wasOk)
            //{
            //    classApplicationLogger.LogError(0, message);
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

            classDeviceManager.Manager.RenameDevice(device, newName);

            // Update the user interface with the new name
            SelectedDevice.NameEdit = device.Name;
        }

        private void ClearSelectedDeviceError()
        {
            if (SelectedDevice != null)
                SelectedDevice.Device.Status = enumDeviceStatus.Initialized;
        }

        #region Public Methods For Adding and Removing Devices for the UI

        public void RemoveDevice(IDevice device)
        {
            if (deviceToWrapperMap.ContainsKey(device))
            {
                // Remove from maps
                deviceToControlMap.Remove(device);
                deviceToWrapperMap.Remove(device);
            }
        }

        /// <summary>
        /// Adds the device to the panel for us.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="control"></param>
        public void AddDevice(IDevice device, IDeviceControlWpf control)
        {
            var deviceControl = new DeviceControlData(device, control);
            deviceControls.Add(deviceControl);

            deviceToControlMap.Add(device, control);
        }

        #endregion
    }
}
