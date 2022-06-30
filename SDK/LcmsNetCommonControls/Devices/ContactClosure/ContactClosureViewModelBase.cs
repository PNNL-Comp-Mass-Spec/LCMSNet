using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;
using ReactiveUI;

namespace LcmsNetCommonControls.Devices.ContactClosure
{
    /// <summary>
    /// Base view model for a contact closure
    /// </summary>
    /// <typeparam name="T">Enum, with the output port options</typeparam>
    public abstract class ContactClosureViewModelBase<T> : BaseDeviceControlViewModelReactive, IDeviceControl where T : struct
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContactClosureViewModelBase()
        {
            // Use List<T>.AsReadOnly and ReadOnlyCollection<T> to get a non-modifiable list of the options; there is no reason for this to be modifiable.
            OutputPortComboBoxOptions = Enum.GetValues(typeof(T)).Cast<T>().ToList().AsReadOnly();
            SendPulseCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () => await Task.Run(SendPulse));
        }

        private int pulseLength;
        private double voltage;
        private double normalVoltage;
        private T selectedPort;

        /// <summary>
        /// The Port options to show in the ComboBox
        /// </summary>
        public ReadOnlyCollection<T> OutputPortComboBoxOptions { get; }

        /// <summary>
        /// The minimum voltage allowed for the contact closure
        /// </summary>
        public abstract double MinimumVoltage { get; }

        /// <summary>
        /// The maximum voltage allowed by the contact closure
        /// </summary>
        public abstract double MaximumVoltage { get; }

        /// <summary>
        /// The minimum pulse length allowed by the contact closure
        /// </summary>
        public abstract int MinimumPulseLength { get; }

        /// <summary>
        /// The pulse length to run
        /// </summary>
        public virtual int PulseLength
        {
            get => pulseLength;
            set => this.RaiseAndSetIfChanged(ref pulseLength, value);
        }

        /// <summary>
        /// The voltage of the pulse
        /// </summary>
        public virtual double Voltage
        {
            get => voltage;
            set => this.RaiseAndSetIfChanged(ref voltage, value);
        }

        /// <summary>
        /// The normal/default voltage of the output
        /// </summary>
        public virtual double NormalVoltage
        {
            get => normalVoltage;
            set => this.RaiseAndSetIfChanged(ref normalVoltage, value);
        }

        /// <summary>
        /// Command to trigger a pulse
        /// </summary>
        public ReactiveUI.ReactiveCommand<Unit, Unit> SendPulseCommand { get; }

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public virtual T Port
        {
            get => selectedPort;
            set => this.RaiseAndSetIfChanged(ref selectedPort, value);
        }

        /// <summary>
        /// Get the Default view for this view model
        /// </summary>
        /// <returns></returns>
        public override UserControl GetDefaultView()
        {
            return new ContactClosureView();
        }

        /// <summary>
        /// Handles sending a pulse to the Contact Closure when the user presses the button to do so.
        /// </summary>
        protected abstract void SendPulse();
    }
}
