using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices;

namespace ASUTGen.Devices.Modules
{
    public partial class IDEXValvePumpModuleControl : UserControl, IDeviceControl
    {
        /// <summary>
        /// Notification driver object.
        /// </summary>
        private IDEXValvePumpModule mobj_valve;

        public IDEXValvePumpModuleControl()
        {
            InitializeComponent();
        }

        public void RegisterDevice(IDevice device)
        {
            mobj_valve = device as IDEXValvePumpModule;
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
                return mobj_valve;
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
