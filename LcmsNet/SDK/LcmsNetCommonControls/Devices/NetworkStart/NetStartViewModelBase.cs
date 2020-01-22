using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices.NetworkStart
{
    /// <summary>
    /// Control for detector triggered by network start signal (presently just a stub)
    /// </summary>
    public abstract class NetStartViewModelBase : BaseDeviceControlViewModelReactive, IDeviceControl
    {
        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public NetStartViewModelBase()
        {
            RefreshMethodsCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(RefreshMethods));
            StartAcquisitionCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(StartAcquisition));
            StopAcquisitionCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(StopAcquisition));

            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Members

        /// <summary>
        /// MethodComboBoxOptions backing field
        /// </summary>
        protected readonly ReactiveUI.ReactiveList<string> methodComboBoxOptions = new ReactiveUI.ReactiveList<string>();

        private string selectedMethod = "";
        private string sampleName = "";
        private string status = "";
        private string ipAddress = "localhost";
        private int port = 4771;

        #endregion

        #region "Properties"

        /// <summary>
        /// The methods to show in the ComboBox
        /// </summary>
        public ReactiveUI.IReadOnlyReactiveList<string> MethodComboBoxOptions => methodComboBoxOptions;

        /// <summary>
        /// The currently selected method
        /// </summary>
        public string SelectedMethod
        {
            get => selectedMethod;
            set => this.RaiseAndSetIfChanged(ref selectedMethod, value);
        }

        /// <summary>
        /// The Sample name
        /// </summary>
        public string SampleName
        {
            get => sampleName;
            set => this.RaiseAndSetIfChanged(ref sampleName, value);
        }

        /// <summary>
        /// The device status
        /// </summary>
        public string Status
        {
            get => status;
            protected set => this.RaiseAndSetIfChanged(ref status, value);
        }

        /// <summary>
        /// Device IP Address
        /// </summary>
        public string IPAddress
        {
            get => ipAddress;
            set => this.RaiseAndSetIfChanged(ref ipAddress, value);
        }

        /// <summary>
        /// Device network port
        /// </summary>
        public int Port
        {
            get => port;
            set => this.RaiseAndSetIfChanged(ref port, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to refresh the methods list
        /// </summary>
        public ReactiveUI.ReactiveCommand<Unit, Unit> RefreshMethodsCommand { get; }

        /// <summary>
        /// Command to start acquisition
        /// </summary>
        public ReactiveUI.ReactiveCommand<Unit, Unit> StartAcquisitionCommand { get; }

        /// <summary>
        /// Command to stop acquisition
        /// </summary>
        public ReactiveUI.ReactiveCommand<Unit, Unit> StopAcquisitionCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// The default view to use with this view model
        /// </summary>
        /// <returns></returns>
        public override UserControl GetDefaultView()
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

        /// <summary>
        /// Reload the methods
        /// </summary>
        protected abstract void RefreshMethods();

        /// <summary>
        /// Called when the IP Address is updated
        /// </summary>
        protected abstract void IPAddressUpdated();

        /// <summary>
        /// Called when the network port is updated
        /// </summary>
        protected abstract void PortUpdated();

        /// <summary>
        /// PropertyChanged event handler to monitor certain properties and call methods when they are changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
