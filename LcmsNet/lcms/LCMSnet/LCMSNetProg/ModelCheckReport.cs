using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses;

namespace LcmsNet
{
    public partial class ModelCheckReport : UserControl
    {
        public ModelCheckReport(ModelStatus status)
        {
            InitializeComponent();
            Time = status.Time;
            Type = status.Name;
            MessageType = status.Category.ToString();
            Event = status.Event;
            Device = status.EventDevice != null ? status.EventDevice.Name : string.Empty;
            ProblemDevice = status.ProblemDevice != null ? status.ProblemDevice.Name : string.Empty;
        }

        public string Time
        {
            get { return lblTime.Text; }
            set { lblTime.Text = value; }
        }


        public string MessageType
        {
            get { return lblMsgTypeConst.Text; }
            set { lblMsgType.Text = value; }
        }

        public string Type
        {
            get { return lblType.Text; }
            set { lblType.Text = value; }
        }

        public string Event
        {
            get { return lblEvent.Text; }
            set { lblEvent.Text = value; }
        }


        public string ProblemDevice
        {
            get { return lblProblemDevice.Text; }
            set { lblProblemDevice.Text = value; }
        }

        public string Device
        {
            get { return lblDevice.Text; }
            set { lblDevice.Text = value; }
        }
    }
}