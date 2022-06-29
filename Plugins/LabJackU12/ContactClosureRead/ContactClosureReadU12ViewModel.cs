﻿using System;
using LcmsNetCommonControls.Devices;
using LcmsNetCommonControls.Devices.ContactClosureRead;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using ReactiveUI;

namespace LcmsNetPlugins.LabJackU12.ContactClosureRead
{
    /// <summary>
    /// Reads/waits for a ready signal from a mass spectrometer or other device using a labjack TTL I/O.
    /// </summary>
    public class ContactClosureReadU12ViewModel : ContactClosureReadViewModelBase<LabjackU12InputPorts>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContactClosureReadU12ViewModel()
        {
            MinimumAnalogVoltage = -10;
            MaximumAnalogVoltage = 10;
            AnalogVoltageThreshold = 2.5;
            Port = LabjackU12InputPorts.AI0;
            IsAnalog = true;
        }

        /// <summary>
        /// The contact closure read class used for reading a ready signal.
        /// </summary>
        private ContactClosureReadU12 contactClosureRead;

        private bool loading;
        private LabjackU12InputPorts selectedInputPort;

        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public override LabjackU12InputPorts Port
        {
            get => contactClosureRead.Port;
            set
            {
                if (ReactiveObjectExtensions.RaiseAndSetIfChangedRetBool(this, ref selectedInputPort, value) && loading == false)
                {
                    contactClosureRead.Port = value;
                    IsAnalog = value.ToString().ToUpper().StartsWith("A");
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
            get => contactClosureRead;
            set
            {
                if (value != null)
                {
                    RegisterDevice(value);

                    this.RaisePropertyChanged(nameof(IsAnalog));
                    this.RaisePropertyChanged(nameof(Port));
                }
            }
        }

        private void RegisterDevice(IDevice device)
        {
            loading = true;
            contactClosureRead = device as ContactClosureReadU12;

            if (contactClosureRead != null)
            {
                Port = contactClosureRead.Port;
                IsAnalog = contactClosureRead.Port.ToString().ToUpper().StartsWith("A");

                contactClosureRead.DeviceSaveRequired += CC_DeviceSaveRequired;
            }
            SetBaseDevice(contactClosureRead);
            loading = false;
        }

        public virtual void CC_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propagate this event
            OnSaveRequired();
        }

        /// <summary>
        /// Handles reading the status of the signal when the user presses the button to do so.
        /// </summary>
        protected override void ReadStatus()
        {
            try
            {
                var state = ContactClosureReadU12.ContactClosureState.Open;
                if (IsAnalog)
                {
                    // Analog port: use analog read
                    state = contactClosureRead.ReadStateAnalog(100, AnalogVoltageThreshold);
                }
                else
                {
                    // Digital port: use digital read
                    state = contactClosureRead.ReadStateDigital(100);
                }
                if (state.HasFlag(ContactClosureReadU12.ContactClosureState.Closed))
                {
                    Status = ContactClosureState.Closed;
                }
                else
                {
                    Status = ContactClosureState.Open;
                }
                Voltage = contactClosureRead.ReadVoltage();
                ReadReport = $"Input state read at {DateTime.Now:HH:m:s tt}";
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not manually read a voltage or state in the contact closure read.", ex);
            }
        }
    }
}