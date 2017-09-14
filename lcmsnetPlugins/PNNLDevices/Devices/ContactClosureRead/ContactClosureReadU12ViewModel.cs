using System;
using LcmsNet.Devices.ContactClosure;
using LcmsNetCommonControls.Devices.ContactClosureRead;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK;

namespace LcmsNet.Devices.ContactClosureRead
{
    /// <summary>
    /// Reads/waits for a ready signal from a mass spectrometer or other device using a labjack TTL I/O.
    /// </summary>
    public class ContactClosureReadU12ViewModel : ContactClosureReadViewModelBase<enumLabjackU12InputPorts>
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContactClosureReadU12ViewModel()
        {
        }

        #endregion

        #region Members

        /// <summary>
        /// The contact closure read class used for reading a ready signal.
        /// </summary>
        private ContactClosureReadU12 contactClosureRead;

        private bool loading;
        private enumLabjackU12InputPorts selectedInputPort;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public override enumLabjackU12InputPorts Port
        {
            get { return contactClosureRead.Port; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref selectedInputPort, value) && loading == false)
                {
                    contactClosureRead.Port = value;
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
                if (contactClosureRead != null)
                {
                    emulated = contactClosureRead.Emulation;
                }
                return emulated;
            }
            set
            {
                if (contactClosureRead != null)
                {
                    contactClosureRead.Emulation = value;
                }
            }
        }

        /// <summary>
        /// The associated device (contact closure)
        /// </summary>
        public override IDevice Device
        {
            get { return contactClosureRead; }
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

            loading = true;
            contactClosureRead = device as ContactClosureReadU12;

            if (contactClosureRead != null)
            {
                Port = contactClosureRead.Port;
            }
            if (contactClosureRead != null)
            {
                contactClosureRead.DeviceSaveRequired += CC_DeviceSaveRequired;
            }
            SetBaseDevice(contactClosureRead);
            loading = false;
        }

        public virtual void CC_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propogate this event
            OnSaveRequired();
        }

        /// <summary>
        /// Handles sending a pulse to the Contact Closure when the user presses the button to do so.
        /// </summary>
        protected override void ReadStatus()
        {
            try
            {
                var state = contactClosureRead.ReadStatus();
                if (state.HasFlag(ContactClosureReadU12.ContactClosureState.Closed))
                {
                    Status = ContactClosureState.Closed;
                }
                else
                {
                    Status = ContactClosureState.Open;
                }
                Voltage = contactClosureRead.ReadVoltage();
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not manually send a pulse in the contact closure.", ex);
            }
        }

        #endregion
    }
}
