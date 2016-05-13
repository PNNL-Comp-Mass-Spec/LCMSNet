using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace ASIpump
{
    public partial class AsiUI : UserControl, IDeviceControl
    {

        private AsiPump _Pump = null;

        public AsiPump Pump
        {
            get { return _Pump; }
        }

        public AsiUI()
        {
            InitializeComponent();
        }

        public void Pump_MessageStreamed(string message)
        {
            txtPump.SafeThreadAction(d => d.Text += message + "\r\n");
        }

        public void Pump_MessageSent(string message)
        {
            txtPump.SafeThreadAction(d => d.Text += message + "\r\n");
        }

        public void RefreshUi()
        {
            gridASI.SelectedObject = Pump;
        }

        private void gridASI_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            RefreshUi();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string errMsg = "";

            Pump.Initialize(ref errMsg);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Pump.StartProgram();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Pump.Escape();
        }

        private void btnGetPos_Click(object sender, EventArgs e)
        {
            Pump.Send("Apr p");
        }

        private void btnGetPositionB_Click(object sender, EventArgs e)
        {
            Pump.Send("Bpr p");
        }


        public event DelegateSaveRequired SaveRequired;
        public event DelegateNameChanged NameChanged;

        public IDevice Device
        {
            get { return _Pump; }
            set { RegisterDevice(value); }
        }

        private void RegisterDevice(IDevice value)
        {
            _Pump = value as AsiPump;
            if (_Pump != null)
            {
                RefreshUi();
                Pump.MessageStreamed += Pump_MessageStreamed;
                Pump.MessageSent += Pump_MessageSent;
            }
        }


        public bool Running { get; set; }

    }
}
