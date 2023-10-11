using System;
using System.Reactive;
using AmpsBoxSdk.Devices;
using ReactiveUI;

namespace AmpsBox
{
    /// <summary>
    /// Single control for each device item.
    /// </summary>
    public class SingleElement : ReactiveObject
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

        private string displayName = "RF Frequency";
        private int setpoint = 0;
        private int setpointMaximum = 2000;
        private int currentValue = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SingleElement()
        {
            this.WhenAnyValue(x => x.Setpoint).Subscribe(x => m_data.Setpoint = x);

            ReadValueCommand = ReactiveCommand.Create(ReadCurrent);
            SetSetpointCommand = ReactiveCommand.Create(SetSetpoint);
        }

        /// <summary>
        /// Gets or sets the name of the element control.
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (value == null)
                    return;

                this.RaiseAndSetIfChanged(ref displayName, value);
            }
        }

        /// <summary>
        /// Gets or sets the command type.
        /// </summary>
        public AmpsBoxChannelData Data
        {
            get => m_data;
            set => m_data = value;
        }

        public int Setpoint
        {
            get => setpoint;
            set => this.RaiseAndSetIfChanged(ref setpoint, value);
        }

        public int SetpointMaximum
        {
            get => setpointMaximum;
            private set => this.RaiseAndSetIfChanged(ref setpointMaximum, value);
        }

        public int CurrentValue
        {
            get => currentValue;
            private set => this.RaiseAndSetIfChanged(ref currentValue, value);
        }

        public ReactiveCommand<Unit, Unit> SetSetpointCommand { get; }
        public ReactiveCommand<Unit, Unit> ReadValueCommand { get; }

        /// <summary>
        /// Updates the display with the data provided.
        /// </summary>
        private void UpdateDisplayData()
        {
            CurrentValue = m_data.Actual;
            Setpoint = m_data.Setpoint;
        }

        /// <summary>
        /// Tells the box to read
        /// </summary>
        private void ReadCurrent()
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
        private void SetSetpoint()
        {
            int value = Setpoint;
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
            SetpointMaximum = m_data.Maximum;
        }
    }

    /// <summary>
    /// Class that holds data for event arguments.
    /// </summary>
    public class AmpsBoxCommandEventArgs : EventArgs
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
