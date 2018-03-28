using System;
using LcmsNetCommonControls.Devices.ContactClosure;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;

namespace LcmsNet.Devices.ContactClosure
{
    /// <summary>
    /// Triggers a mass spectrometer or other device using a labjack TTL pulse.
    /// </summary>
    public class ContactClosureU12ViewModel : ContactClosureViewModelBase<enumLabjackU12OutputPorts>
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContactClosureU12ViewModel()
        {
        }

        #endregion

        #region Members

        /// <summary>
        /// The contact closure class used for triggering a pulse.
        /// </summary>
        private classContactClosureU12 m_contactClosure;

        private const int CONST_MINIMUMVOLTAGE = -5;
        private const int CONST_MAXIMUMVOLTAGE = 5;
        private const int CONST_MINIMUMPULSELENGTH = 0;
        private bool m_loading;
        private enumLabjackU12OutputPorts selectedOutputPort;

        #endregion

        #region Properties

        public override double MinimumVoltage => CONST_MINIMUMVOLTAGE;
        public override double MaximumVoltage => CONST_MAXIMUMVOLTAGE;
        public override int MinimumPulseLength => CONST_MINIMUMPULSELENGTH;

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public override enumLabjackU12OutputPorts Port
        {
            get { return m_contactClosure.Port; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedOutputPort, value) && m_loading == false)
                {
                    m_contactClosure.Port = value;
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
                if (m_contactClosure != null)
                {
                    emulated = m_contactClosure.Emulation;
                }
                return emulated;
            }
            set
            {
                if (m_contactClosure != null)
                {
                    m_contactClosure.Emulation = value;
                }
            }
        }

        /// <summary>
        /// The associated device (contact closure)
        /// </summary>
        public override IDevice Device
        {
            get { return m_contactClosure; }
            set
            {
                if (value != null)
                {
                    RegisterDevice(value);

                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        #endregion

        #region Methods

        private void RegisterDevice(IDevice device)
        {
            m_loading = true;
            m_contactClosure = device as classContactClosureU12;

            if (m_contactClosure != null)
            {
                Port = m_contactClosure.Port;

                m_contactClosure.DeviceSaveRequired += CC_DeviceSaveRequired;
            }
            SetBaseDevice(m_contactClosure);
            m_loading = false;
        }

        public virtual void CC_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propogate this event
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
                    m_contactClosure.Trigger(PulseLength, Voltage);

                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Could not manually send a pulse in the contact closure.", ex);
                }
            }
        }

        #endregion
    }
}
