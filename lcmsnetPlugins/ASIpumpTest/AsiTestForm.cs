using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ASIpump;

namespace ASIpumpTest
{
    public partial class AsiTestForm : Form
    {
        public AsiTestForm()
        {
            InitializeComponent();

            AsiPump pump = new AsiPump();
            pump.PortName = "COM3";

            asiUI1.Pump = pump;
        }

        private void asiUI1_Load(object sender, EventArgs e)
        {

        }
    }
}
