using System;
using System.Windows.Forms;
using AmpsBoxSdk.Devices;
using AmpsBoxSdk;

namespace AmpsBox
{
    /// <summary>
    /// Single control for each device item.
    /// </summary>
    public partial class SingleElementControl : UserControl
    {
        /// <summary>
        /// Fired when the user sends data to the device.
        /// </summary>
        public event EventHandler<AmpsBoxCommandEventArgs> SetDataCommand;
        /// <summary>
        /// Fired when the user requests data from the device
        /// </summary>
        public event EventHandler<AmpsBoxCommandEventArgs> GetDataCommand;
        /// <summary>
        /// Data to send or receive.
        /// </summary>
        private AmpsBoxChannelData m_data; 

        /// <summary>
        /// Constructor.
        /// </summary>
        public SingleElementControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the name of the element control.
        /// </summary>
        public string DisplayName 
        {
            get
            {
                return mgroupbox_name.Text;
            }
            set
            {
                if (value == null)
                    return;

                mgroupbox_name.Text = value;
                mgroupbox_name.Refresh();
            }                    
        }

        /// <summary>
        /// Gets or sets the command type.
        /// </summary>        
        public AmpsBoxChannelData Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        /// <summary>
        /// Updates the display with the data provided.
        /// </summary>
        private void UpdateDisplayData()
        {
            mlabel_value.Text = m_data.Actual.ToString();
            mnum_setpointValue.Value = Convert.ToDecimal(m_data.Setpoint);
        }

        /// <summary>
        /// Tells the box to read
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_read_Click(object sender, EventArgs e)
        {
            if (GetDataCommand != null)
            {
                AmpsBoxCommandEventArgs args = new AmpsBoxCommandEventArgs(Data);
                GetDataCommand(this, args);
                UpdateDisplayData();
            }
        }
        /// <summary>
        /// Sets the data to the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_set_Click(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(mnum_setpointValue.Value);
            if (SetDataCommand != null)
            {
                Data.Setpoint = value;
                SetDataCommand(this, new AmpsBoxCommandEventArgs(Data));
            }
        }
        /// <summary>
        /// Sets the HV data
        /// </summary>
        /// <param name="ampsBoxChannelData"></param>
        public void SetData(AmpsBoxChannelData ampsBoxChannelData)
        {
            m_data = ampsBoxChannelData;
            mnum_setpointValue.Maximum = Convert.ToDecimal(m_data.Maximum);
        }

        private void mnum_setpointValue_ValueChanged(object sender, EventArgs e)
        {
            m_data.Setpoint = Convert.ToInt32(mnum_setpointValue.Value);
        }

    }
    /// <summary>
    /// Class that holds data for event arguments.
    /// </summary>
    public class AmpsBoxCommandEventArgs: EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public AmpsBoxCommandEventArgs(AmpsBoxChannelData data)
        {
            Data = data;            
        }
        /// <summary>
        /// Gets or sets the data 
        /// </summary>
        public AmpsBoxChannelData Data { get; set; }
    }
}
