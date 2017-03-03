//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy, Modified version of controlContactClosureU12
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/10/2014
//
// Last modified 01/10/2014
//*********************************************************************************************************

using System;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.ContactClosure
{
    /// <summary>
    /// Triggers a mass spectrometer or other device using a labjack TTL pulse.
    /// </summary>
    public partial class controlContactClosureU3 : controlBaseDeviceControl, IDeviceControl
    {
        #region Members

        /// <summary>
        /// The contact closure class used for triggering a pulse.
        /// </summary>
        private classContactClosureU3 m_contactClosure;

        /// <summary>
        /// An event listener to watch for events from the contact closure class.
        /// </summary>
        //private CCEventListener m_CCEventListener;

        private const int CONST_MINIMUMVOLTAGE = -5;
        private const int CONST_MAXIMUMVOLTAGE = 5;
        private const int CONST_MINIMUMPULSELENGTH = 0;
        private bool m_loading;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlContactClosureU3()
        {
            InitializeComponent();

        }

        private void RegisterDevice(IDevice device)
        {

            m_loading = true;
            m_contactClosure = device as classContactClosureU3;

            if (m_contactClosure != null)
            {
                mcomboBox_Ports.SelectedItem = m_contactClosure.Port;
            }
            mcomboBox_Ports.SelectedValueChanged   += new EventHandler(mcomboBox_Ports_SelectedValueChanged);
            m_contactClosure.DeviceSaveRequired += new EventHandler(CC_DeviceSaveRequired);
            SetBaseDevice(m_contactClosure);
            m_loading = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the output port of the device.
        /// </summary>
        public enumLabjackU3OutputPorts Port
        {
            get
            {
                return m_contactClosure.Port;
            }
            set
            {
                mcomboBox_Ports.SelectedItem = value;
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
        public IDevice Device
        {
            get
            {
                
                return m_contactClosure;
            }
            set
            {
                if (value != null)
                {
                    RegisterDevice(value);
                }
            }
        }
        #endregion

        #region Form Event Handlers/Methods
        public virtual void CC_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propogate this event
            OnSaveRequired();
        }
        private void mcomboBox_Ports_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_loading == false)
                m_contactClosure.Port = (enumLabjackU3OutputPorts)Enum.Parse(typeof(enumLabjackU3OutputPorts), this.mcomboBox_Ports.Text);
        }
        /// <summary>
        /// Handles sending a pulse to the Contact Closure when the user presses the button to do so.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_SendPulse_Click(object sender, EventArgs e)
        {
            double pulse = Convert.ToInt32(mnum_pulseLength.Value);
            var voltage = Convert.ToDouble(mnum_voltage.Value);
            if ( CONST_MINIMUMVOLTAGE <= voltage  && voltage <= CONST_MAXIMUMVOLTAGE &&  CONST_MINIMUMPULSELENGTH <=  pulse)
            {
                try
                {
                    m_contactClosure.Trigger(pulse, (enumLabjackU3OutputPorts)Enum.Parse(typeof(enumLabjackU3OutputPorts), this.mcomboBox_Ports.Text), voltage);
                    
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0, "Could not manually send a pulse in the contact closure.", ex);
                }
            }
        }

        public void Initialize()
        {

        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_initializeDevice_Click(object sender, EventArgs e)
        {
            Initialize();
        }
        #endregion
    }   
}
