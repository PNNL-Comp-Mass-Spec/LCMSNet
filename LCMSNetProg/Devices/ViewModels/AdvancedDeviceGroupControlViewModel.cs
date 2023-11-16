using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DynamicData;
using FluidicsSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class AdvancedDeviceGroupControlViewModel : ReactiveObject, IDisposable, IEquatable<AdvancedDeviceGroupControlViewModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AdvancedDeviceGroupControlViewModel(string name)
        {
            Name = name;
            deviceToViewModelMap = new Dictionary<IDevice, DeviceConfigurationViewModel>();

            SelectedDevice = null;

            SelectedColor = Colors.Red;
            NotSelectedColor = Colors.Black;

            RenameDeviceCommand = ReactiveCommand.Create(RenameSelectedDevice, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            InitializeDeviceCommand = ReactiveCommand.CreateFromTask(InitializeSelectedDevice, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));
            ClearErrorCommand = ReactiveCommand.Create(ClearSelectedDeviceError, this.WhenAnyValue(x => x.SelectedDevice).Select(x => x != null));

            deviceViewModels.Connect().ObserveOn(RxApp.MainThreadScheduler).Bind(out var deviceViewModelsBound).Subscribe();
            DeviceViewModels = deviceViewModelsBound;

            // Once an item is added to the device controls, automatically make the first one the "Selected device" if one hasn't been previously set.
            this.WhenAnyValue(x => x.SelectedDevice).Where(x => deviceViewModels.Count > 0 && x == null)
                .Subscribe(x => SelectedDevice = deviceViewModels.Items.FirstOrDefault());
            deviceViewModels.CountChanged.Where(x => x > 0 && SelectedDevice == null)
                .Subscribe(x => SelectedDevice = deviceViewModels.Items.FirstOrDefault());

            this.WhenAnyValue(x => x.SelectedDevice, x => x.GroupIsSelected).Where(x => x.Item2).Subscribe(x =>
            {
                // TODO: Limit calling this to when this device class is also selected
                FluidicsModerator.Moderator.SetSelectedDevice(SelectedDevice?.Device);
            });
        }

        [Obsolete("WPF Design-time use only.", true)]
        public AdvancedDeviceGroupControlViewModel() : this("DevTest")
        { }

        ~AdvancedDeviceGroupControlViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var deviceControl in DeviceViewModels)
            {
                deviceControl.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        private readonly Dictionary<IDevice, DeviceConfigurationViewModel> deviceToViewModelMap;
        private readonly SourceList<DeviceConfigurationViewModel> deviceViewModels = new SourceList<DeviceConfigurationViewModel>();
        private DeviceConfigurationViewModel selectedDevice = null;
        private bool groupIsSelected = false;

        public string Name { get; }

        public bool GroupIsSelected
        {
            get => groupIsSelected;
            set => this.RaiseAndSetIfChanged(ref groupIsSelected, value);
        }

        public ReadOnlyObservableCollection<DeviceConfigurationViewModel> DeviceViewModels { get; }

        /// <summary>
        /// Selected device
        /// </summary>
        public DeviceConfigurationViewModel SelectedDevice
        {
            get => selectedDevice;
            set
            {
                var changed = selectedDevice != value;
                if (selectedDevice != null && changed)
                {
                    SelectedDevice.DeviceSelected = false;
                    SelectedDevice.OutOfView();
                }

                this.RaiseAndSetIfChanged(ref selectedDevice, value);

                if (changed && selectedDevice != null)
                {
                    SelectedDevice.DeviceSelected = true;
                }
            }
        }

        /// <summary>
        /// Determines if there are any devices in this group.
        /// </summary>
        public bool IsDeviceGroupEmpty => deviceViewModels.Count < 1;

        public Color SelectedColor { get; set; }

        public Color NotSelectedColor { get; set; }

        public ReactiveCommand<Unit, Unit> RenameDeviceCommand { get; }
        public ReactiveCommand<Unit, Unit> InitializeDeviceCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearErrorCommand { get; }

        public void OutOfView()
        {
            SelectedDevice?.OutOfView();
        }

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

        public void RemoveDevice(IDevice device)
        {
            if (deviceToViewModelMap.ContainsKey(device))
            {
                // Remove from maps
                var deviceVm = deviceToViewModelMap[device];
                deviceViewModels.Remove(deviceVm);
                deviceToViewModelMap.Remove(device);
            }
        }

        /// <summary>
        /// Adds the device to the panel for us.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="viewModel"></param>
        public void AddDevice(IDevice device, IDeviceControl viewModel)
        {
            var deviceVm = new DeviceConfigurationViewModel(device, viewModel);
            deviceViewModels.Add(deviceVm);
            deviceToViewModelMap.Add(device, deviceVm);
        }

        public bool Equals(AdvancedDeviceGroupControlViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AdvancedDeviceGroupControlViewModel) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
