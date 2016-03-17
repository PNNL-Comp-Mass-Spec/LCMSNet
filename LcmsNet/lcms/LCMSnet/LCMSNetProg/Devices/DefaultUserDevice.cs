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

        public event DelegateNameChanged NameChanged;

        public bool Running
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event DelegateSaveRequired SaveRequired;

        #endregion
    }
}