using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Devices.Pumps;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices.Pumps;
using System.Drawing;

namespace LcmsNetDataClasses.Devices.Pumps
{
    public partial class formPumpDisplays : Form
    {
        /// <summary>
        /// Pixel padding for the height at the bottom of the control.
        /// </summary>
        private const int CONST_HEIGHT_PADDING = 15;
        /// <summary>
        /// Dictionary to 
        /// </summary>
        Dictionary<IPump, controlPumpDisplay> mdict_controlList;
        private Dictionary<IPump, GroupBox> mdict_mobilePhases;
        private List<IPump> m_pumps;

        public event EventHandler Tack;
        public event EventHandler UnTack;


        public formPumpDisplays()
        {
            InitializeComponent();
            mdict_mobilePhases = new Dictionary<IPump, GroupBox>();
            m_pumps = new List<IPump>();
            mdict_controlList  = new Dictionary<IPump, controlPumpDisplay>();            
            DeviceManagerBridge.DeviceAdded     += new DelegateDeviceUpdated(Manager_DeviceAdded);
            DeviceManagerBridge.DeviceRemoved   += new DelegateDeviceUpdated(Manager_DeviceRemoved);
            FormClosing                         += new FormClosingEventHandler(formPumpDisplays_FormClosing);
            Resize                              += new EventHandler(formPumpDisplays_Resize);
            m_pointer = 0;
        }

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
        /// Gets or sets whether the window is tacked.
        /// </summary>
        public bool IsTacked
        {
            get;
            set;
        }
        /// <summary>
        /// Resizes all pump displays equally.
        /// </summary>
        void OnResize()
        {
            /// 
            /// dw = difference width             
            /// N  = number of controls.
            /// 
            int N = mpanel_pumps.Controls.Count;
            if (mpanel_pumps.Controls.Count == 0)
                return;

            int dw   = Width / N;            
            int left = 0;
            foreach (Control c in mpanel_pumps.Controls)
            {
                c.Top    = 0;
                int trueWidth = dw;
                controlPumpDisplay display = c as controlPumpDisplay;
                if (display != null)
                {
                    if (display.Tacked)
                    {
                        trueWidth = display.TackWidth;                        
                    }
                }
                c.Width  = trueWidth;
                c.Left   = left;
                left    += trueWidth;
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

        #region Device Manager Event Handlers 
        /// <summary>
        /// Handles when the device manager removes a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceRemoved(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            // Is this a pump?
            IPump pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have the pump            
            if (mdict_controlList.ContainsKey(pump) == false)
                return;

            // If it's a pump and we have it we are safe to remove it from the list of controls 
            // and the mapping dictionary.                    
            mpanel_pumps.Controls.Remove(mdict_controlList[pump]);
            mdict_controlList.Remove(pump);

            if (mdict_mobilePhases.ContainsKey(pump))
            {
                GroupBox box = mdict_mobilePhases[pump];
                mpanel_mobilePhase.Controls.Remove(box);

                mdict_mobilePhases.Remove(pump);
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
            IPump pump = sender as IPump;
            if (pump == null)
                return;

            if (mdict_controlList.ContainsKey(pump) == false)
                return;

            mdict_controlList[pump].SetPumpName(pump.Name);
        }
        /// <summary>
        /// Handles when the device manager adds a device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        void Manager_DeviceAdded(object sender, LcmsNetDataClasses.Devices.IDevice device)
        {
            // Make sure the device is a pump.
            IPump pump = device as IPump;
            if (pump == null)
                return;

            // Make sure we have a reference to the pump
            if (mdict_controlList.ContainsKey(pump) == true)
                return;
             
            // Hook into the pumps display calls            
            controlPumpDisplay display = new controlPumpDisplay();
            display.SetPumpName(pump.Name);

            pump.MonitoringDataReceived += new EventHandler<PumpDataEventArgs>(DisplayPumpData);
            pump.DeviceSaveRequired     += new EventHandler(pump_DeviceSaveRequired);
            display.Tack                += new EventHandler(display_Tack);
            display.UnTack              += new EventHandler(display_UnTack);

            // Make sure we reference this pump 
            mdict_controlList.Add(pump, display);
            mpanel_pumps.Controls.Add(display);


            if (pump.MobilePhases != null)
            {
                // Create a groupbox that we can associate with this pump
                GroupBox box = new GroupBox();
                box.Text     = pump.Name;
                box.Margin   = new Padding(5);

                int height   = 0;
                foreach (MobilePhase phase in pump.MobilePhases)
                {
                    controlMobilePhaseEditor editor = new controlMobilePhaseEditor(phase);
                    editor.Margin                   = new Padding(5);
                    editor.Dock                     = DockStyle.Top;                    
                    height                          += editor.Height + 15;
                    box.Controls.Add(editor);
                    box.ForeColor = Color.DarkRed;
                    editor.Size   = new System.Drawing.Size(editor.Width - 5, editor.Height);
                }
                box.Size = new System.Drawing.Size(box.Width, height);
                box.Dock = DockStyle.Top;
                mpanel_mobilePhase.Controls.Add(box);
                mdict_mobilePhases.Add(pump, box);


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
        /// <param name="time"></param>
        /// <param name="pressure"></param>
        /// <param name="flowrate"></param>
        /// <param name="percentB"></param>
        void DisplayPumpData(object sender, PumpDataEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;
            if (Visible == false)
                return;

            if (InvokeRequired == true)
            {
                BeginInvoke(new EventHandler<PumpDataEventArgs> (mdict_controlList[e.Pump].DisplayMonitoringData), new object[]
                                                                            {
                                                                                sender, e
                                                                            }
                            );                            
            }
            else
            {
                mdict_controlList[e.Pump].DisplayMonitoringData(sender, e);
            }
        }
        #endregion

        private void mbutton_expand_Click(object sender, EventArgs e)
        {
            IsTacked = (IsTacked == false);
            if (IsTacked)
            {
                if (Tack != null)
                {
                    Tack(this, e);
                }
            }
            else
            {
                if (UnTack != null)
                {
                    UnTack(this, e);
                }
            }
        }

        private int m_pointer;

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
            IPump pump = m_pumps[m_pointer];

            GroupBox box = mdict_mobilePhases[pump];
            box.Focus();
            box.Select();


            mlabel_pump.Text = pump.Name;
        }
    }
}
