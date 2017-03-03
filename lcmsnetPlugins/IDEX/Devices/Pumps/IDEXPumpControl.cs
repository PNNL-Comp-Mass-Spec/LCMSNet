using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace ASUTGen.Devices.Pumps
{
    public partial class IDEXPumpControl : UserControl, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXPump m_valve;

        public IDEXPumpControl()
        {
            InitializeComponent();
        }
        
        public void RegisterDevice(IDevice device)
        {
            m_valve = device as IDEXPump;
        }

        #region IDeviceControl Members

        public event DelegateNameChanged  NameChanged;
        public event DelegateSaveRequired SaveRequired;

        public bool Running
        {
            get;
            set;
        }

        public IDevice Device
        {
            get
            {
                return m_valve;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        public void ShowProps()
        {
            
        }
        #endregion
    }
}
