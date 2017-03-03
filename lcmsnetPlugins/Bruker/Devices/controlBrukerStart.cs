
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/19/2010
//
// Last modified 02/19/2010
//*********************************************************************************************************
using System;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.BrukerStart
{
    /// <summary>
    /// Control for detector triggered by Bruker start commands
    /// </summary>
    public partial class controlBrukerStart : controlBaseDeviceControl, IDeviceControl
    {

        #region "Class variables"
            /// <summary>
            /// BrukerStart object to use
            /// </summary>
            private classBrukerStart m_BrukerStart;
        #endregion

        #region "Private delegates"
            private delegate void UpdateStatus(enumDeviceStatus status, string message);
        #endregion

        #region "Events"
            /// <summary>
            /// Fired when the instrument methods are updated.
            /// </summary>
            public event DelegateNameListReceived InstrumentMethodListReceived;
        #endregion

        #region "Constructors"
        public controlBrukerStart()
        {
            InitializeComponent();
        }
        public void RegisterDevice(IDevice device)
        {
            m_BrukerStart                 = device as classBrukerStart;
            m_BrukerStart.MethodNames    += m_BrukerStart_MethodNames;
            m_BrukerStart.Error          += m_BrukerStart_Error;
            
            mtextbox_ipAddress.Text = m_BrukerStart.IPAddress;
            mnum_port.Value         = Convert.ToDecimal(m_BrukerStart.Port);

            SetBaseDevice(m_BrukerStart);
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
                     return m_BrukerStart;
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
                     return m_BrukerStart.Emulation;
                }
                set
                {
                     m_BrukerStart.Emulation = value;
                }
            }
        #endregion

        #region "Methods"
        /// <summary>
        /// Updates the status of the device.
        /// </summary>
        /// <param name="status"></param>
        private void SetStatus(enumDeviceStatus status, string message)
        {
            mlabel_status.Text = "Status: " + m_BrukerStart.Status + " - " + message;
        }
        /// <summary>
        /// Updates the user interface.
        /// </summary>
        private void UpdateUserInterface()
        {
            mtextbox_ipAddress.Text = m_BrukerStart.IPAddress;
            mnum_port.Value = Convert.ToDecimal(m_BrukerStart.Port);
            SetStatus(m_BrukerStart.Status, "");
        }

        void m_BrukerStart_Error(object sender, classDeviceErrorEventArgs e)
        {

            if (InvokeRequired)
            {
                mlabel_status.BeginInvoke(new UpdateStatus(SetStatus), new object[] { m_BrukerStart.Status, e.Error });
            }
            else
            {
                SetStatus(m_BrukerStart.Status, e.Error);
            }
        }

        void m_BrukerStart_MethodNames(object sender, List<object> data)
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

        #region Form Event Handlers
            private void mbutton_startAcquisition_Click(object sender, EventArgs e)
            {
                var methodName = "";
                if (mcomboBox_methods.SelectedIndex < 0)
                {
                    SetStatus(m_BrukerStart.Status, "No method selected.");
                    return;
                }
                methodName = mcomboBox_methods.SelectedItem.ToString();

                var sample           = new classSampleData();
                sample.DmsData.DatasetName       = mtextbox_sampleName.Text;
                sample.InstrumentData.MethodName = methodName;

                m_BrukerStart.StartAcquisition(20, sample);
            }   

            private void mbutton_stopAcquisition_Click(object sender, EventArgs e)
            {
                m_BrukerStart.StopAcquisition(20);
            }   

            private void mbutton_getMethods_Click(object sender, EventArgs e)
            {
                m_BrukerStart.GetMethods();
            }   

            private void mtextbox_ipAddress_TextChanged(object sender, EventArgs e)
            {
                OnSaveRequired();
                m_BrukerStart.IPAddress = mtextbox_ipAddress.Text;
            }   

            private void mnum_port_ValueChanged(object sender, EventArgs e)
            {
                m_BrukerStart.Port = Convert.ToInt32(mnum_port.Value);
                OnSaveRequired();
            }   
        #endregion
    }   
}
