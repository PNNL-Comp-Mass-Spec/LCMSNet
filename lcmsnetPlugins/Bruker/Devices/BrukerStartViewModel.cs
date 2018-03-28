using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using LcmsNetCommonControls.Devices.NetworkStart;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.Bruker.Devices
{
    /// <summary>
    /// Control for detector triggered by Bruker start commands
    /// </summary>
    public class BrukerStartViewModel : NetStartViewModelBase
    {
        #region "Events"

        /// <summary>
        /// Fired when the instrument methods are updated.
        /// </summary>
        public event DelegateNameListReceived InstrumentMethodListReceived;

        #endregion

        #region "Constructors"

        public BrukerStartViewModel()
        {
        }

        public void RegisterDevice(IDevice device)
        {
            m_BrukerStart = device as classBrukerStart;
            if (m_BrukerStart != null)
            {
                m_BrukerStart.MethodNames += m_BrukerStart_MethodNames;
                m_BrukerStart.Error += m_BrukerStart_Error;

                IPAddress = m_BrukerStart.IPAddress;
                Port = m_BrukerStart.Port;
            }

            SetBaseDevice(m_BrukerStart);
        }

        #endregion

        #region "Class variables"

        /// <summary>
        /// BrukerStart object to use
        /// </summary>
        private classBrukerStart m_BrukerStart;

        #endregion

        #region "Properties"

        /// <summary>
        /// Device associated with this control
        /// </summary>
        public override IDevice Device
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
        /// <param name="message"></param>
        private void SetStatus(DeviceStatus status, string message)
        {
            Status = "Status: " + m_BrukerStart.Status + " - " + message;
        }

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        private void UpdateUserInterface()
        {
            IPAddress = m_BrukerStart.IPAddress;
            Port = m_BrukerStart.Port;
            SetStatus(m_BrukerStart.Status, "");
        }

        private void m_BrukerStart_Error(object sender, DeviceErrorEventArgs e)
        {
            SetStatus(m_BrukerStart.Status, e.Error);
        }

        private void m_BrukerStart_MethodNames(object sender, List<object> data)
        {
            var methodNames = new List<string>(data.Select(x => x.ToString()));

            ReactiveUI.RxApp.MainThreadScheduler.Schedule(() =>
            {
                using (methodComboBoxOptions.SuppressChangeNotifications())
                {
                    methodComboBoxOptions.Clear();
                    methodComboBoxOptions.AddRange(methodNames);
                }
            });

            InstrumentMethodListReceived?.Invoke(methodNames);
        }

        #endregion

        #region Form Event Handlers

        protected override void StartAcquisition()
        {
            if (string.IsNullOrWhiteSpace(SelectedMethod))
            {
                SetStatus(m_BrukerStart.Status, "No method selected.");
                return;
            }
            var methodName = SelectedMethod;

            var sample = new SampleData
            {
                DmsData = { DatasetName = SampleName },
                InstrumentData = { MethodName = methodName }
            };

            m_BrukerStart.StartAcquisition(20, sample);
        }

        protected override void StopAcquisition()
        {
            m_BrukerStart.StopAcquisition(20);
        }

        protected override void RefreshMethods()
        {
            m_BrukerStart.GetMethods();
        }

        protected override void IPAddressUpdated()
        {
            OnSaveRequired();
            m_BrukerStart.IPAddress = IPAddress;
        }

        protected override void PortUpdated()
        {
            m_BrukerStart.Port = Port;
            OnSaveRequired();
        }

        #endregion
    }
}
