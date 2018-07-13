using System;
using LcmsNetCommonControls.Devices.ContactClosure;
using LcmsNetData;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.LabJackU3
{
    /// <summary>
    /// Triggers a mass spectrometer or other device using a labjack TTL pulse.
    /// </summary>
    public class ContactClosureU3ViewModel : ContactClosureViewModelBase<LabjackU3OutputPorts>
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContactClosureU3ViewModel()
        {
        }

        #endregion

        #region Members

        /// <summary>
        /// The contact closure class used for triggering a pulse.
        /// </summary>
        private ContactClosureU3 m_contactClosure;

        private const int CONST_MINIMUMVOLTAGE = -5;
        private const int CONST_MAXIMUMVOLTAGE = 5;
        private const int CONST_MINIMUMPULSELENGTH = 0;
        private bool m_loading;
        private LabjackU3OutputPorts selectedOutputPort;

        #endregion

        #region Properties

        public override double MinimumVoltage => CONST_MINIMUMVOLTAGE;
        public override double MaximumVoltage => CONST_MAXIMUMVOLTAGE;
        public override int MinimumPulseLength => CONST_MINIMUMPULSELENGTH;

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public override LabjackU3OutputPorts Port
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
                }
            }
        }

        #endregion

        #region Methods

        private void RegisterDevice(IDevice device)
        {
            m_loading = true;
            m_contactClosure = device as ContactClosureU3;

            if (m_contactClosure != null)
            {
                Port = m_contactClosure.Port;
            }
            if (m_contactClosure != null)
            {
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
            if (CONST_MINIMUMVOLTAGE <= Voltage && Voltage <= CONST_MAXIMUMVOLTAGE && CONST_MINIMUMPULSELENGTH <= PulseLength)
            {
                try
                {
                    m_contactClosure.Trigger(PulseLength, Port, Voltage);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Could not manually send a pulse in the contact closure.", ex);
                }
            }
        }

        #endregion
    }
}
