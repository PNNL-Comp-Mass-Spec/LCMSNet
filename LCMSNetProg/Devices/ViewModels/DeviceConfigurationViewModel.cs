using System;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNet.Devices.ViewModels
{
    public class DeviceConfigurationViewModel : ReactiveObject, IDisposable
    {
        public DeviceConfigurationViewModel(IDevice device, IDeviceControl viewModel)
        {
            Device = device;
            ViewModel = viewModel;
            //name = ViewModel.WhenAnyValue(x => x.Name).ToProperty(this, x => x.Name);
            name = Device.WhenAnyValue(x => x.Name).ToProperty(this, x => x.Name);
            Device.WhenAnyValue(x => x.Name).Subscribe(x => NameEdit = x);

            status = viewModel.WhenAnyValue(x => x.DeviceStatus).ToProperty(this, x => x.Status);
            viewModel.WhenAnyValue(x => x.DeviceTabSelected)
                .Subscribe(x => this.RaisePropertyChanged(nameof(DeviceSelected)));
        }

        private System.Windows.Controls.UserControl view = null;
        private readonly ObservableAsPropertyHelper<string> name;
        private readonly ObservableAsPropertyHelper<string> status;
        private string nameEdit = "";

        public IDevice Device { get; }
        public IDeviceControl ViewModel { get; }

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

        public bool DeviceSelected
        {
            get => ViewModel.DeviceTabSelected;
            set => ViewModel.DeviceTabSelected = value;
        }

        public void OutOfView()
        {
            ViewModel.OutOfView();
        }

        ~DeviceConfigurationViewModel()
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
}
