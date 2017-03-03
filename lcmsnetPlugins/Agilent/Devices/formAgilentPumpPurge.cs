using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Agilent.Devices.Pumps
{
    public partial class formAgilentPumpPurge : Form
    {
        /// <summary>
        /// Pump to purge.
        /// </summary>
        private readonly classPumpAgilent m_pump;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pump"></param>
        public formAgilentPumpPurge(classPumpAgilent pump)
        {
            InitializeComponent();

            m_pump = pump;
            pump.DeviceSaveRequired += new EventHandler(pump_DeviceSaveRequired);
            Text = "Purge Pumps " + m_pump.Name;
        }

        void pump_DeviceSaveRequired(object sender, EventArgs e)
        {
            Text = "Purge Pumps " + m_pump.Name;
        }

        private void mbutton_purgeB2_Click(object sender, EventArgs e)
        {            
            var flow  = Convert.ToDouble(mnum_flowB2.Value);
            var mins  = Convert.ToDouble(mnum_timeB2.Value);
            m_pump.PurgePump(0, enumPurgePumpChannel.B2, flow, mins);
        }

        private void mbutton_purgeB1_Click(object sender, EventArgs e)
        {
            var flow = Convert.ToDouble(mnum_flowB1.Value);
            var mins = Convert.ToDouble(mnum_timeB1.Value);
            m_pump.PurgePump(0, enumPurgePumpChannel.B1, flow, mins);
        }

        private void mbutton_purgeA2_Click(object sender, EventArgs e)
        {
            var flow = Convert.ToDouble(mnum_flowA2.Value);
            var mins = Convert.ToDouble(mnum_timeA2.Value);
            m_pump.PurgePump(0, enumPurgePumpChannel.A2, flow, mins);
        }

        private void mbutton_purgeA1_Click(object sender, EventArgs e)
        {
            var flow = Convert.ToDouble(mnum_flowA1.Value);
            var mins = Convert.ToDouble(mnum_timeA1.Value);
            m_pump.PurgePump(0, enumPurgePumpChannel.A1, flow, mins);
        }

        private void mbutton_abortPurges_Click(object sender, EventArgs e)
        {
            m_pump.AbortPurges(0);
        }     
    }
}
