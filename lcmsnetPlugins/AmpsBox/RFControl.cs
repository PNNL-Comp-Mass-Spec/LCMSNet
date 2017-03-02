using System;
using System.Windows.Forms;
using AmpsBoxSdk.Devices;

namespace AmpsBox
{
    public partial class RFControl : UserControl
    {
        private AmpsBoxRfData m_rfData;
        public event EventHandler<AmpsBoxCommandEventArgs> SetRfFrequency;
        public event EventHandler<AmpsBoxCommandEventArgs> SetDriveLevel;
        public event EventHandler<AmpsBoxCommandEventArgs> SetOutputVoltage;
        public event EventHandler<AmpsBoxCommandEventArgs> GetRfFrequency;
        public event EventHandler<AmpsBoxCommandEventArgs> GetDriveLevel;
        public event EventHandler<AmpsBoxCommandEventArgs> GetOutputVoltage;

        public RFControl()
        {
            InitializeComponent();

            m_rfData = new AmpsBoxRfData();

            m_rfControl.SetDataCommand              += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetDataCommand);
            m_outputVoltageControl.SetDataCommand   += new EventHandler<AmpsBoxCommandEventArgs>(m_outputVoltageControl_SetDataCommand);
            m_driveLevelControl.SetDataCommand      += new EventHandler<AmpsBoxCommandEventArgs>(m_driveLevelControl_SetDataCommand);
            m_rfControl.GetDataCommand              += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetDataCommand);
            m_outputVoltageControl.GetDataCommand   += new EventHandler<AmpsBoxCommandEventArgs>(m_outputVoltageControl_GetDataCommand);
            m_driveLevelControl.GetDataCommand      += new EventHandler<AmpsBoxCommandEventArgs>(m_driveLevelControl_GetDataCommand);
        }

        #region Command Event Handlers
        void m_driveLevelControl_GetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            GetDriveLevel(this, e);
        }
        void m_outputVoltageControl_GetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            GetOutputVoltage(this, e);
        }
        void m_rfControl_GetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            GetRfFrequency(this, e);
        }
        void m_driveLevelControl_SetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            SetDriveLevel(this, e);
        }
        void m_outputVoltageControl_SetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            SetOutputVoltage(this, e);
        }
        void m_rfControl_SetDataCommand(object sender, AmpsBoxCommandEventArgs e)
        {
            e.Data.Channel = m_rfData.Channel;
            SetRfFrequency(this, e);
        }
        #endregion

        /// <summary>
        /// Sets the RF data and updates displays.
        /// </summary>
        /// <param name="rfData"></param>
        public void SetData(AmpsBoxRfData rfData)
        {
            m_rfData                    = rfData;
            m_rfControl.Data            = rfData.RfFrequency;
            m_outputVoltageControl.Data = rfData.OutputVoltage;
            m_driveLevelControl.Data    = rfData.DriveLevel;

            
        }
    }

}
