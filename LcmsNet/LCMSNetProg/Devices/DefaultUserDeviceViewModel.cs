using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LcmsNetSDK.Devices;

namespace LcmsNet.Devices
{
    public class DefaultUserDeviceViewModel : IDeviceControl
    {
        public DefaultUserDeviceViewModel()
        {
        }

        #region IDeviceControl Members

        public IDevice Device { get; set; }
        public string Name { get; set; }
        public UserControl GetDefaultView()
        {
            return new DefaultUserDeviceView();
        }

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
