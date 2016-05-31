using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public partial class FluidicsColumnControl : UserControl, IDeviceControl
    {
        FluidicsColumn m_column;

        public FluidicsColumnControl()
        {
            InitializeComponent();
        }

        private void RegisterDevice(IDevice device)
        {
            if (device == null)
            {
                m_column = null;
                return;
            }
            m_column = device as FluidicsColumn;
            if (m_column != null)
            {
                mtextBox_packingMaterial.Text   = m_column.PackingMaterial;
                mnum_innerDiameter.Value        = Convert.ToDecimal(m_column.InnerDiameter);
                mnum_length.Value               = Convert.ToDecimal(m_column.Length);
            }
        }

        #region IDeviceControl Members
        public event DelegateNameChanged NameChanged;
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
                return m_column;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        #endregion
    }
}
