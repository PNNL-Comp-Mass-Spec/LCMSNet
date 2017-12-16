using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace LcmsNetCommonControls.Devices.ContactClosureRead
{
    /// <summary>
    /// Base view model for a contact closure read
    /// </summary>
    /// <typeparam name="T">Enum, with the output port options</typeparam>
    public abstract class ContactClosureReadViewModelBase<T> : BaseDeviceControlViewModel, IDeviceControl where T : struct
    {
        /// <summary>
        /// State for a contact closure
        /// </summary>
        public enum ContactClosureState
        {
            /// <summary>
            /// State is unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// State is open
            /// </summary>
            Open = 1,

            /// <summary>
            /// State is closed
            /// </summary>
            Closed = 2
        }

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContactClosureReadViewModelBase()
        {
            inputPortComboBoxOptions = new ReactiveUI.ReactiveList<T>(Enum.GetValues(typeof(T)).Cast<T>());
            ReadStatusCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(() => ReadStatus()));
            MinimumAnalogVoltage = 0;
            MaximumAnalogVoltage = 5;
            AnalogVoltageThreshold = 2.5;
        }

        #endregion

        #region Members

        private double voltage;
        private double analogVoltageThreshold;
        private double minimumAnalogVoltage;
        private double maximumAnalogVoltage;
        private string readReport;
        private T selectedPort;
        private bool isAnalog;
        private ContactClosureState status = ContactClosureState.Unknown;

        /// <summary>
        /// InputPortComboBoxOptions backing field
        /// </summary>
        protected readonly ReactiveUI.ReactiveList<T> inputPortComboBoxOptions;

        #endregion

        #region Properties

        /// <summary>
        /// The Port options to show in the ComboBox
        /// </summary>
        public ReactiveUI.IReadOnlyReactiveList<T> InputPortComboBoxOptions => inputPortComboBoxOptions;

        /// <summary>
        /// Command to read the signal
        /// </summary>
        public ReactiveUI.ReactiveCommand<Unit, Unit> ReadStatusCommand { get; private set; }

        /// <summary>
        /// The voltage at the input
        /// </summary>
        public double Voltage
        {
            get { return voltage; }
            protected set { this.RaiseAndSetIfChanged(ref voltage, value); }
        }

        /// <summary>
        /// The minimum voltage for an analog input to be considered "closed"
        /// </summary>
        public double AnalogVoltageThreshold
        {
            get { return analogVoltageThreshold; }
            set { this.RaiseAndSetIfChanged(ref analogVoltageThreshold, value); }
        }

        /// <summary>
        /// The minimum voltage for an analog input
        /// </summary>
        public double MinimumAnalogVoltage
        {
            get { return minimumAnalogVoltage; }
            protected set { this.RaiseAndSetIfChanged(ref minimumAnalogVoltage, value); }
        }

        /// <summary>
        /// The maximum voltage for an analog input
        /// </summary>
        public double MaximumAnalogVoltage
        {
            get { return maximumAnalogVoltage; }
            protected set { this.RaiseAndSetIfChanged(ref maximumAnalogVoltage, value); }
        }

        /// <summary>
        /// If the selected port is an analog port
        /// </summary>
        public bool IsAnalog
        {
            get { return isAnalog; }
            protected set { this.RaiseAndSetIfChanged(ref isAnalog, value); }
        }

        /// <summary>
        /// A report status message that may be shown to the user
        /// </summary>
        public string ReadReport
        {
            get { return readReport; }
            protected set { this.RaiseAndSetIfChanged(ref readReport, value); }
        }

        /// <summary>
        /// Gets or sets the intput port of the device.
        /// </summary>
        public virtual T Port
        {
            get { return selectedPort; }
            set { this.RaiseAndSetIfChanged(ref selectedPort, value); }
        }

        /// <summary>
        /// Signal status
        /// </summary>
        public ContactClosureState Status
        {
            get { return status; }
            set { this.RaiseAndSetIfChanged(ref status, value); }
        }

        /// <summary>
        /// The device this view model is associated with
        /// </summary>
        public abstract IDevice Device { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the Default view for this view model
        /// </summary>
        /// <returns></returns>
        public virtual UserControl GetDefaultView()
        {
            return new ContactClosureReadView();
        }

        /// <summary>
        /// Handles reading the status of the signal when the user presses the button to do so.
        /// </summary>
        protected abstract void ReadStatus();

        #endregion
    }
}
