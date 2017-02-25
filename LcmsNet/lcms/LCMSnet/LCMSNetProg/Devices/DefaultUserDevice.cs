using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{
    public partial class DefaultUserDevice : UserControl, IDeviceControl
    {
        public DefaultUserDevice()
        {
            InitializeComponent();
        }

        #region IDeviceControl Members

        public IDevice Device { get; set; }

        /// <summary>
        /// An event that indicates the name of the column has changed.
        /// </summary>
        /// <remarks>This event is required by IDeviceControl but this class does not use it</remarks>
        public event DelegateNameChanged NameChanged
        {
            add { }
            remove { }
        }

        public bool Running
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        /// <remarks>This event is required by IDeviceControl but this class does not use it</remarks>
        public event DelegateSaveRequired SaveRequired
        {
            add { }
            remove { }
        }

        #endregion
    }
}