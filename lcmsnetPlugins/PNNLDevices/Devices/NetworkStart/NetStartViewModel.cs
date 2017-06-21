using System.Collections.Generic;
using System.Linq;
using LcmsNet.Devices.NetworkStart.Socket;
using LcmsNetCommonControls.Devices.NetworkStart;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices.NetworkStart
{
    /// <summary>
    /// Control for detector triggered by network start signal (presently just a stub)
    /// </summary>
    public class NetStartViewModel : NetStartViewModelBase
    {
        #region "Constructors"

        public NetStartViewModel()
        {
        }

        private void RegisterDevice(IDevice device)
        {
            m_netStart = device as classNetStartSocket;
            if (m_netStart != null)
            {
                m_netStart.MethodNames += m_netStart_MethodNames;
                m_netStart.Error += m_netStart_Error;
                UpdateUserInterface();
            }

            SetBaseDevice(m_netStart);
        }

        #endregion

        #region Members

        /// <summary>
        /// A NesStart object to use
        /// </summary>
        private classNetStartSocket m_netStart;

        /// <summary>
        /// Fired when the instrument methods are updated.
        /// </summary>
        public event DelegateNameListReceived InstrumentMethodListReceived;

        #endregion

        #region "Properties"

        /// <summary>
        /// Device associated with this control
        /// </summary>
        public override IDevice Device
        {
            get { return m_netStart; }
            set { RegisterDevice(value); }
        }

        /// <summary>
        /// Gets or sets object emulation mode
        /// </summary>
        public bool Emulation
        {
            get { return m_netStart.Emulation; }
            set { m_netStart.Emulation = value; }
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
            SetStatus(m_netStart.Status, e.Error);
        }

        private void m_netStart_MethodNames(object sender, List<object> data)
        {
            var methodNames = new List<string>(data.Select(x => x.ToString()));

            using (methodComboBoxOptions.SuppressChangeNotifications())
            {
                methodComboBoxOptions.Clear();
                methodComboBoxOptions.AddRange(methodNames);
            }

            InstrumentMethodListReceived?.Invoke(methodNames);
        }

        #endregion

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        private void UpdateUserInterface()
        {
            IPAddress = m_netStart.Address;
            Port = m_netStart.Port;
            SetStatus(m_netStart.Status, "");
        }

        /// <summary>
        /// Updates the status of the device.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        private void SetStatus(enumDeviceStatus status, string message)
        {
            Status = "Status: " + m_netStart.Status + " - " + message;
        }

        #region Form Event Handlers

        /// <summary>
        /// Manually starts the acquisition.
        /// </summary>
        protected override void StartAcquisition()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                SetStatus(m_netStart.Status, "No method selected.");
                return;
            }
            var methodName = SelectedMethod;

            var sample = new classSampleData
            {
                DmsData = { DatasetName = SampleName },
                InstrumentData = {MethodName = methodName}
            };

            m_netStart.StartAcquisition(20, sample);
        }

        /// <summary>
        /// Manually stops the acquisition.
        /// </summary>
        protected override void StopAcquisition()
        {
            m_netStart.StopAcquisition(1000);
        }

        protected override void RefreshMethods()
        {
            m_netStart.GetMethods();
        }

        protected override void IPAddressUpdated()
        {
            OnSaveRequired();
            m_netStart.Address = IPAddress;
        }

        protected override void PortUpdated()
        {
            m_netStart.Port = Port;
            OnSaveRequired();
        }

        #endregion
    }
}
