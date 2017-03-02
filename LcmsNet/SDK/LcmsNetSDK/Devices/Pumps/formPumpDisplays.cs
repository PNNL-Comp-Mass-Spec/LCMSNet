using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetSDK.Devices.Pumps;

namespace LcmsNetDataClasses.Devices.Pumps
{
    public partial class formPumpDisplays : Form
    {
        /// <summary>
        /// Pixel padding for the height at the bottom of the control.
        /// </summary>
        private const int CONST_HEIGHT_PADDING = 15;

        private readonly List<IPump> m_pumps;

        /// <summary>
        /// Dictionary to
        /// </summary>
        readonly Dictionary<IPump, controlPumpDisplay> m_controlList;

        private readonly Dictionary<IPump, GroupBox> m_mobilePhases;

        private int m_pointer;


        public formPumpDisplays()
        {
            InitializeComponent();
            m_mobilePhases = new Dictionary<IPump, GroupBox>();
            m_pumps = new List<IPump>();
            m_controlList = new Dictionary<IPump, controlPumpDisplay>();
            DeviceManagerBridge.DeviceAdded += Manager_DeviceAdded;
            DeviceManagerBridge.DeviceRemoved += Manager_DeviceRemoved;
            FormClosing += formPumpDisplays_FormClosing;
            Resize += formPumpDisplays_Resize;
            m_pointer = 0;
        }

        /// <summary>
        /// Gets or sets whether the window is tacked.
        /// </summary>
        public bool IsTacked { get; set; }

        public event EventHandler Tack;
        public event EventHandler UnTack;

        /// <summary>
        /// Handles resizing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formPumpDisplays_Resize(object sender, EventArgs e)
        {
            OnResize();
        }

        /// <summary>
        /// Resizes all pump displays equally.
        /// </summary>
        void OnResize()
        {
            // 
            // dw = difference width
            // N  = number of controls.
            // 
            var N = mpanel_pumps.Controls.Count;
            if (mpanel_pumps.Controls.Count == 0)
                return;

            var dw = Width / N;
            var left = 0;
            foreach (Control c in mpanel_pumps.Controls)
            {
                c.Top = 0;
                var trueWidth = dw;
                var display = c as controlPumpDisplay;
                if (display != null)
                {
                    if (display.Tacked)
                    {
                        trueWidth = display.TackWidth;
                    }
                }
                c.Width = trueWidth;
                c.Left = left;
                left += trueWidth;
                c.Height = mpanel_pumps.Height - CONST_HEIGHT_PADDING;
            }
        }

        /// <summary>
        /// Prevents the form from being disposed and instead hides the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formPumpDisplays_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            IsTacked = (IsTacked == false);
            if (IsTacked)
            {
                Tack?.Invoke(this, e);
            }
            else
            {
                UnTack?.Invoke(this, e);
            }
        }

        private void mbutton_left_Click(object sender, EventArgs e)
        {
            MoveRight();
        }


        private void mbutton_right_Click(object sender, EventArgs e)
        {
            MoveLeft();
        }

        private void MoveLeft()
        {
            m_pointer = Math.Max(0, --m_pointer);

            if (m_pumps.Count < 1)
            {
                mlabel_pump.Text = "";
                return;
            }

            UpdateLabel();
        }

        private void MoveRight()
        {
            m_pointer = Math.Min(m_pumps.Count - 1, ++m_pointer);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            var pump = m_pumps[m_pointer];

            var box = m_mobilePhases[pump];
            box.Focus();
            box.Select();


            mlabel_pump.Text = pump.Name;
        }

        #region Device Manager Event Handlers

        /// <summary>
        /// Handles when the device manager removes a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, IDevice device)
        {
            // Is this a pump?
            var pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have the pump
            if (m_controlList.ContainsKey(pump) == false)
                return;

            // If it's a pump and we have it we are safe to remove it from the list of controls
            // and the mapping dictionary.
            mpanel_pumps.Controls.Remove(m_controlList[pump]);
            m_controlList.Remove(pump);

            if (m_mobilePhases.ContainsKey(pump))
            {
                var box = m_mobilePhases[pump];
                mpanel_mobilePhase.Controls.Remove(box);

                m_mobilePhases.Remove(pump);
            }

            if (m_pumps.Contains(pump))
                m_pumps.Remove(pump);

            MoveLeft();
            OnResize();
        }

        /// <summary>
        /// Checks to update the name of the device.
        /// </summary>
        /// <param name="sender"></param>
        void pump_DeviceSaveRequired(object sender, EventArgs e)
        {
            var pump = sender as IPump;
            if (pump == null)
                return;

            if (m_controlList.ContainsKey(pump) == false)
                return;

            m_controlList[pump].SetPumpName(pump.Name);
        }

        /// <summary>
        /// Handles when the device manager adds a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, IDevice device)
        {
            // Make sure the device is a pump.
            var pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have a reference to the pump
            if (m_controlList.ContainsKey(pump))
                return;

            // Hook into the pumps display calls
            var display = new controlPumpDisplay();
            display.SetPumpName(pump.Name);

            pump.MonitoringDataReceived += DisplayPumpData;
            pump.DeviceSaveRequired += pump_DeviceSaveRequired;
            display.Tack += display_Tack;
            display.UnTack += display_UnTack;

            // Make sure we reference this pump
            m_controlList.Add(pump, display);
            mpanel_pumps.Controls.Add(display);


            if (pump.MobilePhases != null)
            {
                // Create a groupbox that we can associate with this pump
                var box = new GroupBox();
                box.Text = pump.Name;
                box.Margin = new Padding(5);

                var height = 0;
                foreach (var phase in pump.MobilePhases)
                {
                    var editor = new controlMobilePhaseEditor(phase);
                    editor.Margin = new Padding(5);
                    editor.Dock = DockStyle.Top;
                    height += editor.Height + 15;
                    box.Controls.Add(editor);
                    box.ForeColor = Color.DarkRed;
                    editor.Size = new Size(editor.Width - 5, editor.Height);
                }
                box.Size = new Size(box.Width, height);
                box.Dock = DockStyle.Top;
                mpanel_mobilePhase.Controls.Add(box);
                m_mobilePhases.Add(pump, box);


                m_pumps.Add(pump);

                m_pointer = m_pumps.Count - 1;
                UpdateLabel();

                box.Select();
                box.Focus();
            }

            OnResize();
        }

        void display_UnTack(object sender, EventArgs e)
        {
            OnResize();
        }

        void display_Tack(object sender, EventArgs e)
        {
            OnResize();
        }

        /// <summary>
        /// Updates the appropriate control
        /// </summary>
        void DisplayPumpData(object sender, PumpDataEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;
            if (Visible == false)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<PumpDataEventArgs>(m_controlList[e.Pump].DisplayMonitoringData), sender, e);
            }
            else
            {
                m_controlList[e.Pump].DisplayMonitoringData(sender, e);
            }
        }

        #endregion
    }
}