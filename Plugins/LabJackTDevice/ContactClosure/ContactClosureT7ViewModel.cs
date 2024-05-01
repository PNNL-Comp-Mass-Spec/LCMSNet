using System;
using LabJackTSeries;
using LcmsNetCommonControls.Devices;
using LcmsNetCommonControls.Devices.ContactClosure;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNetPlugins.LabJackTDevice.ContactClosure
{
    /// <summary>
    /// Triggers a mass spectrometer or other device using a LabJack TTL pulse.
    /// </summary>
    public class ContactClosureT7ViewModel : ContactClosureViewModelBase<LabJackT7Outputs>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContactClosureT7ViewModel()
        {
        }

        /// <summary>
        /// The contact closure class used for triggering a pulse.
        /// </summary>
        private ContactClosureT7 contactClosure;

        private const int CONST_MINIMUMVOLTAGE = -5;
        private const int CONST_MAXIMUMVOLTAGE = 5;
        private const int CONST_MINIMUMPULSELENGTH = 0;
        private bool isLoading;
        private int selectedPulseLength;
        private double selectedVoltage;
        private double selectedNormalVoltage;
        private LabJackT7Outputs selectedOutputPort;

        public override double MinimumVoltage => CONST_MINIMUMVOLTAGE;
        public override double MaximumVoltage => CONST_MAXIMUMVOLTAGE;
        public override int MinimumPulseLength => CONST_MINIMUMPULSELENGTH;

        /// <summary>
        /// The pulse length to run
        /// </summary>
        public override int PulseLength
        {
            get => contactClosure.PulseLength;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedPulseLength, value) && isLoading == false)
                {
                    contactClosure.PulseLength = value;
                }
            }
        }

        /// <summary>
        /// The voltage of the pulse
        /// </summary>
        public override double Voltage
        {
            get => contactClosure.PulseVoltage;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedVoltage, value) && isLoading == false)
                {
                    contactClosure.PulseVoltage = value;
                }
            }
        }

        /// <summary>
        /// The normal/default voltage of the output
        /// </summary>
        public override double NormalVoltage
        {
            get => contactClosure.NormalVoltage;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedNormalVoltage, value) && isLoading == false)
                {
                    contactClosure.NormalVoltage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public override LabJackT7Outputs Port
        {
            get => contactClosure.Port;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedOutputPort, value) && isLoading == false)
                {
                    contactClosure.Port = value;
                }
            }
        }

        /// <summary>
        /// Determines if the device is in emulation mode or not.
        /// </summary>
        public bool Emulation
        {
            get
            {
                var emulated = true;
                if (contactClosure != null)
                {
                    emulated = contactClosure.Emulation;
                }
                return emulated;
            }
            set
            {
                if (contactClosure != null)
                {
                    contactClosure.Emulation = value;
                }
            }
        }

        /// <summary>
        /// The associated device (contact closure)
        /// </summary>
        public override IDevice Device
        {
            get => contactClosure;
            set
            {
                if (value != null)
                {
                    RegisterDevice(value);

                    this.RaisePropertyChanged(nameof(Port));
                }
            }
        }

        private void RegisterDevice(IDevice device)
        {
            isLoading = true;
            contactClosure = device as ContactClosureT7;

            if (contactClosure != null)
            {
                Port = contactClosure.Port;

                contactClosure.DeviceSaveRequired += CC_DeviceSaveRequired;
            }
            SetBaseDevice(contactClosure);
            isLoading = false;
        }

        public virtual void CC_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propagate this event
            OnSaveRequired();
        }

        /// <summary>
        /// Handles sending a pulse to the Contact Closure when the user presses the button to do so.
        /// </summary>
        protected override void SendPulse()
        {
            if (Voltage >= CONST_MINIMUMVOLTAGE && Voltage <= CONST_MAXIMUMVOLTAGE && PulseLength > CONST_MINIMUMPULSELENGTH)
            {
                try
                {
                    contactClosure.Trigger();
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Could not manually send a pulse in the contact closure.", ex);
                }
            }
        }
    }
}
