using System;
using System.Windows.Forms;
using System.Collections.Generic;

using LcmsNetDataClasses.Devices;

namespace LcmsNetMethodSimulator.Method
{
    public partial class controlMethodEditor : UserControl
    {

        public controlMethodEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reflects the given device adding it to the user interface.
        /// </summary>
        /// <param name="device"></param>
        public void ReflectDevice(IDevice device)
        {

        }
    }
}
