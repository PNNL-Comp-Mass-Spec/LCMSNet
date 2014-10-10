﻿//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
//						- 12/01/09 (DAC) - Modified to accomodate change of vial from string to int
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;

using LcmsNet.Devices;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses;

namespace LcmsNet.Devices.Pal
{    
    public partial class controlPal : controlBaseDeviceControl, IDeviceControl
    {
        #region Members

        /// <summary>
        /// The class which controls the PAL itself.
        /// </summary>
        private classPal mobj_Pal;

        /// <summary>
        /// An event listener to watch for events from the PAL class
        /// </summary>
        //public PalEventListener mobj_PalEventListener;

        #endregion

        #region Constructors 

        /// <summary>
        /// The main constructor. Creates the PAL class and initializes the port.
        /// </summary>
        public controlPal()
        {
            InitializeComponent(); 
            string[] names = System.IO.Ports.SerialPort.GetPortNames();
            mcombo_portNames.Items.AddRange(names);
            mcombo_portNames.SelectedIndexChanged += new EventHandler(mcombo_portNames_SelectedIndexChanged);
        }

        void mcombo_portNames_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// Registers the device events and user interface.
        /// </summary>
        /// <param name="device"></param>
        private void RegisterDevice(IDevice device)
        {
          
            mobj_Pal = device as classPal;            
           
            mobj_Pal.DeviceSaveRequired += new EventHandler(Pal_DeviceSaveRequired);
            mobj_Pal.Free               += new DelegateDeviceFree(OnFree);
            mobj_Pal.TrayNames          += new EventHandler<classAutoSampleEventArgs>(mobj_Pal_PalTrayListReceived);
            mobj_Pal.MethodNames        += new EventHandler<classAutoSampleEventArgs>((object sender, classAutoSampleEventArgs e) => {ProcessMethods(e.MethodList); });     
     
            mcomboBox_VialRange.DataSource      = System.Enum.GetNames(typeof(enumVialRanges));
            mcomboBox_VialRange.SelectedItem    = mobj_Pal.VialRange.ToString();
            
            SetBaseDevice(mobj_Pal);
        }

        #endregion

        #region Events
        /// <summary>
        /// Indicates that the device is available to take commands
        /// </summary>
        public event DelegateFree Free;        
        #endregion

        #region Properties
        //TODO: This. There are wait/free events, do I still need this?
        /// <summary>
        /// Keeps track of whether or not the PAL is occupied.
        /// </summary>
        public override bool Running
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// Decides whether or not the PAL is emulated.
        /// </summary>
        public bool Emulation
        {
            get
            {
                return mobj_Pal.Emulation;
            }
            set
            {
                mobj_Pal.Emulation = value;
            }
        }

        /// <summary>
        /// The associated device (PAL).
        /// </summary>
        public IDevice Device
        {
            get
            {
                return mobj_Pal;
            }
            set
            {
                mobj_Pal = value as classPal;
                if (mobj_Pal != null && !DesignMode)
                {
                    try
                    {
                        mcombo_portNames.SelectedText = mobj_Pal.PortName;
                    }
                    catch
                    { 
                    }
                    RegisterDevice(value);
                }
            }
        }
        #endregion

        #region Methods
        
        /// <summary>
        /// Indicates that the device is available to take commands
        /// </summary>
        public virtual void OnFree(object sender)
        {
            if (Free != null)
            {
                Free(this);
            }
           // mbool_runningMethodManually = false;
            //mButton_RunMethod.Text = "Run Method";
        }

        public virtual void Pal_DeviceSaveRequired(object sender, EventArgs e)
        {
            //Propogate this
            //TODO: Figure out if this actually worked or not
            //System.Windows.Forms.MessageBox.Show("OH SNAP WE NEED TO SAVE");
            OnSaveRequired();
        }

        /// <summary>
        /// Converts the raw method list string into a list of methods.
        /// </summary>
        /// <param name="rawMethodList">The string which the PAL class returns after GetMethodList()</param>
        public void ProcessMethods(List<string> rawMethodList)
        {
            //LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "PAL ADVANCED CONTROL PROCESS METHODS:" + rawMethodList.Count);
            if (rawMethodList != null)
            {
                mcomboBox_MethodList.BeginUpdate();
                //Clear out the previous list
                mcomboBox_MethodList.Items.Clear();

                //That was cool, so now fill up the combobox
                foreach (string s in rawMethodList)
                {
                    mcomboBox_MethodList.Items.Add(s);
                }
                mcomboBox_MethodList.EndUpdate();
            }
        }

        /// <summary>
        /// Handles when the PAL says it has tray data.
        /// </summary>
        /// <param name="trayList">List of detected tray names.</param>
        void mobj_Pal_PalTrayListReceived(object sender, classAutoSampleEventArgs args)
        {
            ProcessTrays(args.TrayList);
        }

        /// <summary>
        /// Converts the raw tray list string into a list of trays.
        /// </summary>
        /// <param name="rawMethodList">The string which the PAL class returns after GetTrayList()</param>
        public void ProcessTrays(List<string> trayList) 
        {
            /*LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(
                                                               LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                                                               "ADVANCED CONTROL PROCESS TRAYS:" + trayList.Count);*/
            if (trayList != null)
            {
                //Clear out the previous list
                mcomboBox_tray.Items.Clear();
                mcomboBox_tray.BeginUpdate();               

                //That was cool, so now fill up the combobox
                foreach (string s in trayList)
                {
                    mcomboBox_tray.Items.Add(s);
                }
                mcomboBox_tray.EndUpdate();
            }          
        }

        private void mButton_RefreshMethods_Click(object sender, EventArgs e)
        {
             mobj_Pal.ListMethods();
             mobj_Pal.ListTrays();                     
        }

        private void mButton_RunMethod_Click(object sender, EventArgs e)
        {         
            if (mcomboBox_tray.SelectedItem == null)
            {
                MessageBox.Show("No tray selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (mcomboBox_MethodList.SelectedItem == null)
            {
                MessageBox.Show("No method selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (mobj_Pal.GetStatus().Contains("READY"))
                {
                    mobj_Pal.LoadMethod(mcomboBox_MethodList.SelectedItem.ToString(), mcomboBox_tray.SelectedItem.ToString(), Convert.ToInt32(mnum_vial.Value), Convert.ToString(mnum_volume.Value));
                    mobj_Pal.StartMethod(1000);
                }
                else
                {
                    mobj_Pal.ContinueMethod(0);
                }
            }
        }

        private void mButton_Initialize_Click(object sender, EventArgs e)
        {
            try
            {
                string errorMessage = "";
                mobj_Pal.Initialize(ref errorMessage);
            }
            catch
            {
                mTextBox_Status.Text = "Could not initialize.";
            }
        }

        private void mButton_StatusRefresh_Click(object sender, EventArgs e)
        {
            mTextBox_Status.Text = mobj_Pal.GetStatus();
        }

        #endregion        
        private void mbutton_stopMethod_Click(object sender, EventArgs e)
        {
            mobj_Pal.StopMethod();
        }
        private void mcomboBox_VialRange_SelectionChangeCommitted(object sender, EventArgs e)
        {
            mobj_Pal.VialRange = (enumVialRanges)System.Enum.Parse(typeof(enumVialRanges), mcomboBox_VialRange.Text);
        }

        private void mbutton_apply_Click(object sender, EventArgs e)
        {
            mobj_Pal.PortName       = mcombo_portNames.SelectedItem as string;
            mTextBox_Status.Text    = "Port name changed to " + mobj_Pal.PortName;
        }
    }
}
