
//*********************************************************************************************************
/*/ Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/25/2009
//
 *      12-11-2009 - BLL - Added device status and data provider methods for examining the status in a list view
// Last modified 12/11/2009
/*///*********************************************************************************************************
using System;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNet.Devices.NetworkStart.Socket;

namespace LcmsNet.Devices.NetworkStart
{
    /// <summary>
    /// Control for detector triggered by network start signal (presently just a stub)
    /// </summary>
    public partial class controlNetStart : controlBaseDeviceControl, IDeviceControl
    {

        #region Members
        /// <summary>
        /// A NesStart object to use
        /// </summary>
        private classNetStartSocket m_netStart;
        private delegate void UpdateStatus(enumDeviceStatus status, string message);
        /// <summary>
        /// Fired when the instrument methods are updated.
        /// </summary>
        public event DelegateNameListReceived InstrumentMethodListReceived;
        #endregion


        #region "Constructors"
        public controlNetStart()
        {
            InitializeComponent();
        }
        private void RegisterDevice(IDevice device)
        {
            m_netStart              = device as classNetStartSocket;
            if (m_netStart != null)
            {
                m_netStart.MethodNames += m_netStart_MethodNames;
                m_netStart.Error       += m_netStart_Error;
                UpdateUserInterface();                           
            }

            SetBaseDevice(m_netStart);
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// Device associated with this control
        /// </summary>
        public IDevice Device
        {
            get
            {
                return m_netStart;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        /// <summary>
        /// Gets or sets object emulation mode
        /// </summary>
        public bool Emulation
        {
            get
            {
                return m_netStart.Emulation;
            }
            set
            {
                m_netStart.Emulation = value;
            }
        }
        #endregion

        #region Network Start Events
        /// <summary>
        /// Handles when the device has an error!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_netStart_Error(object sender, classDeviceErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                mlabel_status.BeginInvoke(new UpdateStatus(SetStatus), m_netStart.Status, e.Error);
            }
            else
            {
                SetStatus(m_netStart.Status, e.Error);
            }
        }
        void m_netStart_MethodNames(object sender, List<object> data)
        {
            var methodNames = new List<string>();

            mcomboBox_methods.Items.Clear();
            mcomboBox_methods.BeginUpdate();
            foreach (var o in data)
            {
                methodNames.Add(o.ToString());
                mcomboBox_methods.Items.Add(o);
            }
            mcomboBox_methods.EndUpdate();


            InstrumentMethodListReceived?.Invoke(methodNames);
        }

        #endregion
        
        /// <summary>
        /// Updates the user interface.
        /// </summary>
        private void UpdateUserInterface()
        {
            mtextbox_ipAddress.Text = m_netStart.Address;
            mnum_port.Value         = Convert.ToDecimal(m_netStart.Port);
            SetStatus(m_netStart.Status, "");
        }

        /// <summary>
        /// Updates the status of the device.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        private void SetStatus(enumDeviceStatus status, string message)
        {
            mlabel_status.Text = "Status: " + m_netStart.Status + " - " + message;
        }

        #region Form Event Handlers
        /// <summary>
        /// Manually starts the acquisition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_startAcquisition_Click(object sender, EventArgs e)
        {
            if (mcomboBox_methods.SelectedIndex < 0)
            {
                SetStatus(m_netStart.Status, "No method selected.");
                return;
            }
            var methodName = mcomboBox_methods.SelectedItem.ToString();

            var sample = new classSampleData
            {
                DmsData = {DatasetName = mtextbox_sampleName.Text},
                InstrumentData = {MethodName = methodName}
            };

            m_netStart.StartAcquisition(20, sample);
        }
        /// <summary>
        /// Manually stops the acquisition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_stopAcquisition_Click(object sender, EventArgs e)
        {
            m_netStart.StopAcquisition(1000);
        }
        private void mbutton_getMethods_Click(object sender, EventArgs e)
        {
            m_netStart.GetMethods();
        }
        #endregion

        private void mtextbox_ipAddress_TextChanged(object sender, EventArgs e)
        {
            OnSaveRequired();
            m_netStart.Address = mtextbox_ipAddress.Text;
        }

        private void mnum_port_ValueChanged(object sender, EventArgs e)
        {
            m_netStart.Port = Convert.ToInt32(mnum_port.Value);
            OnSaveRequired();
        }
    }   
}
