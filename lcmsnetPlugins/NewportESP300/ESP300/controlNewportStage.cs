using System;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;

namespace Newport.ESP300
{
    public partial class controlNewportStage : controlBaseDeviceControl, IDeviceControl
    {
        private classNewportStage m_obj;
        const string units = "mm";
        const string positionNotDefined = "NoPosition";
        public controlNewportStage()
        {
            InitializeComponent();
        }


        public void RegisterDevice(IDevice device)
        {
            m_obj = device as classNewportStage;
            SetBaseDevice(m_obj);
            m_obj.PositionsLoaded +=new EventHandler(PositionLoadHandler);
            m_obj.PositionChanged += new EventHandler(PositionChangedHandler);
            m_obj.StatusUpdate += m_obj_StatusUpdate;
            UpdatePositionListBox();
            UpdatePositionLabel(m_obj.CurrentPos);
            UpdateMotorStatus();
            m_serialPropertyGrid.SelectedObject = m_obj.Port;
        }

        private void m_obj_StatusUpdate(object sender, classDeviceStatusEventArgs e)
        {
            var tokens = e.Message.Split(new char[' ']);
            if(e.Notification == "Motor")
            {
                switch(tokens[0])
                {
                    case "1":
                        lblAxis1MotorStatus.Text = tokens[1];
                        break;
                    case "2":
                        lblAxis2MotorStatus.Text = tokens[1];
                        break;
                    case "3":
                        lblAxis3MotorStatus.Text = tokens[1];
                        break;
                    default:
                        // we don't care about other updates...for now.
                        break;
                }
            }
            else if (e.Notification == "Initialized")
            {
                UpdateMotorStatus();
            }
        }

        public IDevice Device
        {
            get
            {
                return m_obj;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        private void btnRefreshPosition_Click(object sender, EventArgs e)
        {
            UpdateAxisPositions();
        }

        private void UpdateMotorStatus()
        {
            lblAxis1MotorStatus.Text = m_obj.GetMotorStatus(1) ? "On": "Off";
            lblAxis2MotorStatus.Text = m_obj.GetMotorStatus(2) ? "On" : "Off";
            lblAxis3MotorStatus.Text = m_obj.GetMotorStatus(3) ? "On" : "Off";
        }
        private void UpdateAxisPositions()
        {
            try
            {
                lblAxis1.Text = m_obj.QueryPosition(1).ToString() + units;
                lblAxis2.Text = m_obj.QueryPosition(2).ToString() + units;
                lblAxis3.Text = m_obj.QueryPosition(3).ToString() + units;
            }
            catch (Exception)
            {
                // do something with the error.
            }
        }


        private void UpdatePositionLabel(string pos)
        {
            lblCurrentPos.Text = pos;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_obj.FindHome(1);
            m_obj.FindHome(2);
            m_obj.FindHome(3);
        }

        private void btnAxis1Back_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(1, true);
            lblCurrentPos.Text = positionNotDefined;
        }

        private void btnAxis1Back_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(1);
            lblAxis1.Text = m_obj.QueryPosition(1).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;

        }

        private void btnAxis1Fwd_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(1, false);
        }

        private void btnAxis1Fwd_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(1);
            lblAxis1.Text = m_obj.QueryPosition(1).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;
        }

        private void btnAxis2Back_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(2, true);
        }

        private void btnAxis2Back_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(2);
            lblAxis2.Text = m_obj.QueryPosition(2).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;
        }

        private void btnAxis2Fwd_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(2, false);
        }

        private void btnAxis2Fwd_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(2);
            lblAxis2.Text = m_obj.QueryPosition(2).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;
        }

        private void btnAxis3Fwd_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(3, false);
        }

        private void btnAxis3Fwd_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(3);
            lblAxis3.Text = m_obj.QueryPosition(3).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;
        }


        private void btnAxis3Back_MouseDown(object sender, MouseEventArgs e)
        {
            m_obj.MoveAxis(3, true);
        }

        private void btnAxis3Back_MouseUp(object sender, MouseEventArgs e)
        {
            m_obj.StopMotion(3);
            lblAxis3.Text = m_obj.QueryPosition(3).ToString() + units;
            lblCurrentPos.Text = positionNotDefined;
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstPositions.SelectedItem != null)
                {
                    var pos = lstPositions.SelectedItem.ToString();
                    m_obj.GoToPosition(5000, pos);
                    UpdateAxisPositions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void btnSetPos_Click(object sender, EventArgs e)
        {
            var pos = txtPosName.Text;
            if (pos != string.Empty)
            {
                var axis1Pos = m_obj.QueryPosition(1);
                var axis2Pos = m_obj.QueryPosition(2);
                var axis3Pos = m_obj.QueryPosition(3);
                m_obj.SetPositionCoordinates(pos, Convert.ToSingle(axis1Pos), Convert.ToSingle(axis2Pos), Convert.ToSingle(axis3Pos));
                UpdatePositionListBox();
                lblCurrentPos.Text = pos;
            }
            else
            {
                MessageBox.Show("A position name is required.", "Error", MessageBoxButtons.OK);
            }
        }

        private void btnGetErrors_Click(object sender, EventArgs e)
        {      
            MessageBox.Show(m_obj.GetErrors(), "ESP300 Errors", MessageBoxButtons.OK);
        }

        private void UpdatePositionListBox()
        {
            lstPositions.Items.Clear();
            foreach (var Key in m_obj.Positions.Keys)
            {
                lstPositions.Items.Add(Key);
            }            
        }

        private void PositionLoadHandler(object sender, EventArgs e)
        {
            UpdatePositionListBox();
        }

        private void PositionChangedHandler(object sender, EventArgs e)
        {
            var stage = sender as classNewportStage;
            UpdatePositionLabel(stage.CurrentPos);
        }

        private void tabControl1_VisibleChanged(object sender, EventArgs e)
        {
            UpdateAxisPositions();
        }

        private void btnRemovePosition_Click(object sender, EventArgs e)
        {
            if (lstPositions.SelectedItem != null)
            {
                var pos = lstPositions.SelectedItem.ToString();
                m_obj.RemovePosition(pos);
                UpdatePositionListBox();
                if (lblCurrentPos.Text == pos)
                {
                    lblCurrentPos.Text = positionNotDefined;
                }
            }
        }

        private void btnClearErrors_Click(object sender, EventArgs e)
        {
            m_obj.ClearErrors();
        }

        private void m_serialPropertyGrid_Click(object sender, EventArgs e)
        {

        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            m_obj.OpenPort();
        }

        private void btnClosePort_Click(object sender, EventArgs e)
        {
            m_obj.ClosePort();
        }

        private bool ToggleMotor(int axis)
        {
            var motorOn = m_obj.GetMotorStatus(axis);
            if (motorOn)
            {
                m_obj.MotorOff(axis);
            }
            else
            {
                m_obj.MotorOn(axis);
            }
            return m_obj.GetMotorStatus(axis);
        }

        private void btnAxis1Motor_Click(object sender, EventArgs e)
        {
            switch(ToggleMotor(1))
            {
                case true:
                    lblAxis1MotorStatus.Text = "On";
                    break;
                case false:
                    lblAxis1MotorStatus.Text = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis1 Motor of Newport stage");
            }
        }

        private void btnAxis2Motor_Click(object sender, EventArgs e)
        {
            switch (ToggleMotor(2))
            {
                case true:
                    lblAxis2MotorStatus.Text = "On";
                    break;
                case false:
                    lblAxis2MotorStatus.Text = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis2 Motor of Newport stage");
            }
        }

        private void btnAxis3Motor_Click(object sender, EventArgs e)
        {
            switch (ToggleMotor(3))
            {
                case true:
                    lblAxis3MotorStatus.Text = "On";
                    break;
                case false:
                    lblAxis3MotorStatus.Text = "Off";
                    break;
                default:
                    throw new Exception("Something went wrong when toggling Axis3 Motor of Newport stage");
            }
        }

    }
}
