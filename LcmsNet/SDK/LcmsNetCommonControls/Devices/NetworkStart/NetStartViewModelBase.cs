using System.ComponentModel;
using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace LcmsNetCommonControls.Devices.NetworkStart
{
    /// <summary>
    /// Control for detector triggered by network start signal (presently just a stub)
    /// </summary>
    public abstract class NetStartViewModelBase : BaseDeviceControlViewModel, IDeviceControlWpf
    {
        #region "Constructors"

        public NetStartViewModelBase()
        {
            SetupCommands();
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Members

        private readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();
        private string selectedMethod = "";
        private string sampleName = "";
        private string status = "";
        private string ipAddress = "localhost";
        private int port = 4771;

        #endregion

        #region "Properties"

        public ReactiveUI.ReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;

        public string SelectedMethod
        {
            get { return selectedMethod; }
            set { this.RaiseAndSetIfChanged(ref selectedMethod, value); }
        }

        public string SampleName
        {
            get { return sampleName; }
            set { this.RaiseAndSetIfChanged(ref sampleName, value); }
        }

        public string Status
        {
            get { return status; }
            protected set { this.RaiseAndSetIfChanged(ref status, value); }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set { this.RaiseAndSetIfChanged(ref ipAddress, value); }
        }

        public int Port
        {
            get { return port; }
            set { this.RaiseAndSetIfChanged(ref port, value); }
        }

        /// <summary>
        /// Device associated with this control
        /// </summary>
        public abstract IDevice Device { get; set; }

        #endregion

        #region Commands

        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshMethodsCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StartAcquisitionCommand { get; private set; }
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopAcquisitionCommand { get; private set; }

        private void SetupCommands()
        {
            RefreshMethodsCommand = ReactiveUI.ReactiveCommand.Create(() => RefreshMethods());
            StartAcquisitionCommand = ReactiveUI.ReactiveCommand.Create(() => StartAcquisition());
            StopAcquisitionCommand = ReactiveUI.ReactiveCommand.Create(() => StopAcquisition());
        }

        #endregion

        #region Methods

        public UserControl GetDefaultView()
        {
            return new NetStartView();
        }

        /// <summary>
        /// Manually starts the acquisition.
        /// </summary>
        protected abstract void StartAcquisition();

        /// <summary>
        /// Manually stops the acquisition.
        /// </summary>
        protected abstract void StopAcquisition();

        protected abstract void RefreshMethods();

        protected abstract void IPAddressUpdated();

        protected abstract void PortUpdated();

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IPAddress)))
            {
                IPAddressUpdated();
            }
            if (e.PropertyName.Equals(nameof(Port)))
            {
                PortUpdated();
            }
        }

        #endregion
    }
}
