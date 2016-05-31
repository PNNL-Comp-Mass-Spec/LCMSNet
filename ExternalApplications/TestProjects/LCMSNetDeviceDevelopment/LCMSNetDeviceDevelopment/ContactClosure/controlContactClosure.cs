using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LcmsNet.Devices.ContactClosure
{
    public partial class controlContactClosure : controlBaseDeviceControl
    {
        private classContactClosure mobj_contactClosure;

        public controlContactClosure()
        {
            InitializeComponent();
            mobj_contactClosure = new classContactClosure(enumLabjackU12OutputPorts.AO1);
        }

        public classContactClosure ContactClosure
        {
            get
            {
                return mobj_contactClosure;
            }
            set
            {
                mobj_contactClosure = value;
                //mcomboBox_Ports.SelectedItem = mobj_contactClosure.port;
            }
        }

        private void controlContactClosure_Load(object sender, EventArgs e)
        {
            //Set the groupbox selection to the associated port
            if (mobj_contactClosure != null)
            {
                mcomboBox_Ports.SelectedItem = mobj_contactClosure.port;
            }
        }

        private void mcomboBox_Ports_SelectedValueChanged(object sender, EventArgs e)
        {
            //Wat
            mobj_contactClosure.port = (enumLabjackU12OutputPorts)Enum.Parse(typeof(enumLabjackU12OutputPorts), this.mcomboBox_Ports.Text);
            
        }

        private void mbutton_SendPulse_Click(object sender, EventArgs e)
        {
            mobj_contactClosure.Trigger(Convert.ToInt32(mtextbox_PulseLength.Text), Convert.ToDouble(mtextBox_Voltage.Text));
        }
    }
}
