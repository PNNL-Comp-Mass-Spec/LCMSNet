using System;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using LcmsNetDataClasses.Devices;
using LcmsNetSDK;

namespace LcmsNetCommonControls.Devices.ContactClosure
{
    /// <summary>
    /// Base view model for a contact closure
    /// </summary>
    /// <typeparam name="T">Enum, with the output port options</typeparam>
    public abstract class ContactClosureViewModelBase<T> : BaseDeviceControlViewModel, IDeviceControlWpf where T : struct
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContactClosureViewModelBase()
        {
            outputPortComboBoxOptions = new ReactiveUI.ReactiveList<T>(Enum.GetValues(typeof(T)).Cast<T>());
            SendPulseCommand = ReactiveUI.ReactiveCommand.Create(() => SendPulse());
        }

        #endregion

        #region Members

        private int pulseLength;
        private double voltage;
        private readonly ReactiveUI.ReactiveList<T> outputPortComboBoxOptions;
        private T selectedPort;

        #endregion

        #region Properties

        public ReactiveUI.ReactiveList<T> OutputPortComboBoxOptions => outputPortComboBoxOptions;
        public abstract double MinimumVoltage { get; }
        public abstract double MaximumVoltage { get; }
        public abstract int MinimumPulseLength { get; }

        public int PulseLength
        {
            get { return pulseLength; }
            set { this.RaiseAndSetIfChanged(ref pulseLength, value); }
        }

        public double Voltage
        {
            get { return voltage; }
            set { this.RaiseAndSetIfChanged(ref voltage, value); }
        }

        public ReactiveUI.ReactiveCommand<Unit, Unit> SendPulseCommand { get; private set; }

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public virtual T Port
        {
            get { return selectedPort; }
            set { this.RaiseAndSetIfChanged(ref selectedPort, value); }
        }

        public abstract IDevice Device { get; set; }

        #endregion

        #region Methods

        public virtual UserControl GetDefaultView()
        {
            return new ContactClosureView();
        }

        /// <summary>
        /// Handles sending a pulse to the Contact Closure when the user presses the button to do so.
        /// </summary>
        protected abstract void SendPulse();

        #endregion
    }
}
