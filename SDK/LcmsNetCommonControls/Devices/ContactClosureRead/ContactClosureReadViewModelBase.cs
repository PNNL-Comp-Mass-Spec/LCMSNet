using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices.ContactClosureRead
{
    /// <summary>
    /// Base view model for a contact closure read
    /// </summary>
    /// <typeparam name="T">Enum, with the output port options</typeparam>
    public abstract class ContactClosureReadViewModelBase<T> : BaseDeviceControlViewModelReactive, IDeviceControl where T : struct
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContactClosureReadViewModelBase()
        {
            // Use List<T>.AsReadOnly and ReadOnlyCollection<T> to get a non-modifiable list of the options; there is no reason for this to be modifiable.
            InputPortComboBoxOptions = Enum.GetValues(typeof(T)).Cast<T>().ToList().AsReadOnly();
            ReadStatusCommand = ReactiveCommand.CreateFromTask(async () => await Task.Run(ReadStatus));
            MinimumAnalogVoltage = 0;
            MaximumAnalogVoltage = 5;
            AnalogVoltageThreshold = 2.5;
        }

        private double voltage;
        private double analogVoltageThreshold;
        private double minimumAnalogVoltage;
        private double maximumAnalogVoltage;
        private string readReport;
        private T selectedPort;
        private bool isAnalog;
        private ContactClosureState status = ContactClosureState.Unknown;

        /// <summary>
        /// The Port options to show in the ComboBox
        /// </summary>
        public ReadOnlyCollection<T> InputPortComboBoxOptions { get; }

        /// <summary>
        /// Command to read the signal
        /// </summary>
        public ReactiveCommand<Unit, Unit> ReadStatusCommand { get; }

        /// <summary>
        /// The voltage at the input
        /// </summary>
        public double Voltage
        {
            get => voltage;
            protected set => this.RaiseAndSetIfChanged(ref voltage, value);
        }

        /// <summary>
        /// The minimum voltage for an analog input to be considered "closed"
        /// </summary>
        public double AnalogVoltageThreshold
        {
            get => analogVoltageThreshold;
            set => this.RaiseAndSetIfChanged(ref analogVoltageThreshold, value);
        }

        /// <summary>
        /// The minimum voltage for an analog input
        /// </summary>
        public double MinimumAnalogVoltage
        {
            get => minimumAnalogVoltage;
            protected set => this.RaiseAndSetIfChanged(ref minimumAnalogVoltage, value);
        }

        /// <summary>
        /// The maximum voltage for an analog input
        /// </summary>
        public double MaximumAnalogVoltage
        {
            get => maximumAnalogVoltage;
            protected set => this.RaiseAndSetIfChanged(ref maximumAnalogVoltage, value);
        }

        /// <summary>
        /// If the selected port is an analog port
        /// </summary>
        public bool IsAnalog
        {
            get => isAnalog;
            protected set => this.RaiseAndSetIfChanged(ref isAnalog, value);
        }

        /// <summary>
        /// A report status message that may be shown to the user
        /// </summary>
        public string ReadReport
        {
            get => readReport;
            protected set => this.RaiseAndSetIfChanged(ref readReport, value);
        }

        /// <summary>
        /// Gets or sets the input port of the device.
        /// </summary>
        public virtual T Port
        {
            get => selectedPort;
            set => this.RaiseAndSetIfChanged(ref selectedPort, value);
        }

        /// <summary>
        /// Signal status
        /// </summary>
        public ContactClosureState Status
        {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }

        /// <summary>
        /// Get the Default view for this view model
        /// </summary>
        /// <returns></returns>
        public override UserControl GetDefaultView()
        {
            return new ContactClosureReadView();
        }

        /// <summary>
        /// Handles reading the status of the signal when the user presses the button to do so.
        /// </summary>
        protected abstract void ReadStatus();
    }
}
