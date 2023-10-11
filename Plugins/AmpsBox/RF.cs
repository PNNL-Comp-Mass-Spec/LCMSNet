using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmpsBoxSdk.Devices;
using ReactiveUI;

namespace AmpsBox
{
    public class RF : ReactiveObject
    {
        public RF()
        {
            DriveLevel = new SingleElement() { DisplayName = "Drive Level" };
            OutputVoltage = new SingleElement() { DisplayName = "Output Voltage" };
            RfFrequency = new SingleElement() { DisplayName = "RF Frequency" };

            RfFrequency.SetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_SetDataCommand);
            OutputVoltage.SetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_outputVoltageControl_SetDataCommand);
            DriveLevel.SetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_driveLevelControl_SetDataCommand);
            RfFrequency.GetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_rfControl_GetDataCommand);
            OutputVoltage.GetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_outputVoltageControl_GetDataCommand);
            DriveLevel.GetDataCommand += new EventHandler<AmpsBoxCommandEventArgs>(m_driveLevelControl_GetDataCommand);
        }

        private AmpsBoxRfData m_rfData;
        public event EventHandler<AmpsBoxCommandEventArgs> SetRfFrequency;
        public event EventHandler<AmpsBoxCommandEventArgs> SetDriveLevel;
        public event EventHandler<AmpsBoxCommandEventArgs> SetOutputVoltage;
        public event EventHandler<AmpsBoxCommandEventArgs> GetRfFrequency;
        public event EventHandler<AmpsBoxCommandEventArgs> GetDriveLevel;
        public event EventHandler<AmpsBoxCommandEventArgs> GetOutputVoltage;

        public SingleElement DriveLevel { get; }
        public SingleElement OutputVoltage { get; }
        public SingleElement RfFrequency { get; }

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
            m_rfData = rfData;
            RfFrequency.Data = rfData.RfFrequency;
            OutputVoltage.Data = rfData.OutputVoltage;
            DriveLevel.Data = rfData.DriveLevel;
        }
    }
}
