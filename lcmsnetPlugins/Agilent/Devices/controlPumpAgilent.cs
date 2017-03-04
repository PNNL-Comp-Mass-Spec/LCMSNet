//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices.Pumps;

namespace Agilent.Devices.Pumps
{
    public partial class controlPumpAgilent : controlBaseDeviceControl, IDeviceControl
    {
        private const string CONST_PUMP_METHOD_PATH = "pumpmethods";

        #region Members
        /// <summary>
        /// A pump object to use.
        /// </summary>
        private classPumpAgilent m_pump;
        /// <summary>
        /// Fired when a new method is available from the pumps.
        /// </summary>
        public static event EventHandler NewMethodAvailable;
        formAgilentPumpPurge mform_purges;
        #endregion

        #region Constructors
        /// <summary>
        /// The default Constructor.
        /// </summary>
        public controlPumpAgilent()
        {
            InitializeComponent();

        }
        private void RegisterDevice(IDevice device)
        {
            m_pump = device as classPumpAgilent;

            mform_purges = new formAgilentPumpPurge(m_pump);
            // 
            // Initialize the underlying device class
            // 
            if (m_pump != null)
            {
                m_pump.MethodAdded += m_pump_MethodAdded;
                m_pump.MethodUpdated += m_pump_MethodUpdated;
                m_pump.MonitoringDataReceived += m_pump_MonitoringDataReceived;
                mcomboBox_Mode.DataSource = Enum.GetValues(typeof(enumPumpAgilentModes));

                NewMethodAvailable += controlPumpAgilent_NewMethodAvailable;
            }

            // 
            // Add to the device manager.
            // 
            SetBaseDevice(m_pump);

            // 
            // Add a list of available serial port names to the combo box.

            var portNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var portName in portNames)
            {
                mcomboBox_comPort.Items.Add(portName);
            }

            if (mcomboBox_comPort.Items.Count > 0)
                mcomboBox_comPort.SelectedIndex = 0;
            if (mcomboBox_comPort.Items.Contains(m_pump.PortName))
            {
                var index = mcomboBox_comPort.Items.IndexOf(m_pump.PortName);
                if (index >= 0)
                {
                    mcomboBox_comPort.SelectedIndex = index;
                }
            }

            // Reads the pump method directory.
            try
            {
                ReadMethodDirectory();
            }
            catch
            {
                //TODO: Update errors!
            }

            InitializePlots();
        }
        #endregion

        #region Plotting and Monitoring Data Handling
        /// <summary>
        /// Initialize the plots for monitoring data.
        /// </summary>
        public void InitializePlots()
        {
        }
        /// <summary>
        /// Handles the event when data is received from the pumps.
        /// </summary>
        void m_pump_MonitoringDataReceived(object sender, PumpDataEventArgs args)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<PumpDataEventArgs>(mcontrol_pumpDisplay.DisplayMonitoringData), sender, args);
            }
            else
            {
                mcontrol_pumpDisplay.DisplayMonitoringData(sender, args);
            }
        }
        #endregion

        #region Pump Event Handlers and methods
        void controlPumpAgilent_NewMethodAvailable(object sender, EventArgs e)
        {
            if (sender != null && sender != this)
            {
                //TODO: Error handling
                ReadMethodDirectory();
            }
        }
        /// <summary>
        /// Handles when a pump method is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_pump_MethodUpdated(object sender, classPumpMethodEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, classPumpMethodEventArgs>(m_pump_MethodUpdated), sender, e);
            }
            else if (mcomboBox_methods.Items.Contains(e.MethodName) == false)
            {
                mcomboBox_methods.Items.Add(e.MethodName);
            }
        }
        /// <summary>
        /// Handles when a pump method is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_pump_MethodAdded(object sender, classPumpMethodEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, classPumpMethodEventArgs>(m_pump_MethodAdded), sender, e);
            }
            else if (mcomboBox_methods.Items.Contains(e.MethodName) == false)
            {
                mcomboBox_methods.Items.Add(e.MethodName);

                // 
                // Make sure one method is selected.
                // 
                if (mcomboBox_methods.Items.Count == 1)
                    mcomboBox_methods.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Reads the pump method directory and alerts the pumps of new methods to run.
        /// </summary>
        void ReadMethodDirectory()
        {
            // 
            // The reason we don't just add stuff straight into the user interface here, is to maintain the
            // design pattern that things propogate events to us, since we are not in charge of managing the
            // data.  We will catch an event from adding a method that one was added...and thus update
            // the user interface intrinsically.
            // 
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), CONST_PUMP_METHOD_PATH);
            if (!System.IO.Directory.Exists(path))
            {
                throw new System.IO.DirectoryNotFoundException("The directory " + path + " does not exist.");
            }

            var filenames = System.IO.Directory.GetFiles(path, "*.txt");


            string selectedMethod = null;
            if (mcomboBox_methods.Items.Count > 0)
            {
                selectedMethod = mcomboBox_methods.SelectedItem as string;
            }
            // 
            // Clear any existing pump methods
            // 
            if (filenames.Length > 0)
                m_pump.ClearMethods();
            var methods = new Dictionary<string, string>();
            foreach (var filename in filenames)
            {
                var method = System.IO.File.ReadAllText(filename);
                methods[System.IO.Path.GetFileNameWithoutExtension(filename)] = method;
                m_pump.AddMethod(System.IO.Path.GetFileNameWithoutExtension(filename), method);
            }
            //m_pump.AddMethods(methods);
            if (selectedMethod != null)
            {
                // try to select the last selected method, if it has been loaded back in to the system.
                mcomboBox_methods.SelectedIndex = mcomboBox_methods.Items.Contains(selectedMethod) ? mcomboBox_methods.Items.IndexOf(selectedMethod) : 0;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// The associated device.
        /// </summary>
        public IDevice Device
        {
            get
            {
                return m_pump;
            }
            set
            {
                RegisterDevice(value);
            }
        }

        /// <summary>
        /// Determines whether or not pump is in emulation mode
        /// </summary>
        public bool Emulation
        {
            get
            {
                return m_pump.Emulation;
            }
            set
            {
                m_pump.Emulation = value;
            }
        }

        #endregion

        #region Methods
        ///// <summary>
        ///// Removes the device from the device manager.
        ///// </summary>
        ///// <returns></returns>
        //public override bool RemoveDevice()
        //{
        //    return classDeviceManager.Manager.RemoveDevice(this.Device);
        //}
        #endregion

        #region Control Event Handlers
        private void mbutton_SetFlowRate_Click(object sender, EventArgs e)
        {
            m_pump.SetFlowRate(Convert.ToDouble(mnum_flowRate.Value));
        }
        private void mbutton_GetFlowRate_Click(object sender, EventArgs e)
        {
            mtextBox_ActualFlowRate.Text = m_pump.GetActualFlow().ToString("0.000");
        }
        private void mbutton_SetMixerVol_Click(object sender, EventArgs e)
        {
            m_pump.SetMixerVolume(Convert.ToDouble(mnum_mixerVolume.Value));
        }
        private void mbutton_GetMixerVol_Click(object sender, EventArgs e)
        {
            mtextBox_GetMixerVol.Text = m_pump.GetMixerVolume().ToString("0.000");
        }
        private void mbutton_GetPressure_Click(object sender, EventArgs e)
        {
            mtextBox_Pressure.Text = m_pump.GetPressure().ToString("0.000");
        }
        private void mbutton_SetMode_Click(object sender, EventArgs e)
        {
            m_pump.SetMode((enumPumpAgilentModes)mcomboBox_Mode.SelectedValue);
        }
        private void mbutton_retrieve_Click(object sender, EventArgs e)
        {
            // 
            // Get the pump method from the device
            // 
            var method = m_pump.RetrieveMethod();
            // 
            // Allow the user to edit it.
            // 
            mtextbox_method.Text = method;
        }
        /// <summary>
        /// Saves the Pump method to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_saveMethod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(mtextbox_method.Text))
                return;

            var dialog = new SaveFileDialog
            {
                InitialDirectory = CONST_PUMP_METHOD_PATH,
                FileName = "pumpMethod",
                DefaultExt = ".txt"
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            System.IO.TextWriter writer = System.IO.File.CreateText(dialog.FileName);
            writer.Write(mtextbox_method.Text);
            writer.Close();

            // 
            // Make sure we add it to the list of methods as well.
            // 
            m_pump.AddMethod(System.IO.Path.GetFileNameWithoutExtension(dialog.FileName), mtextbox_method.Text);

            NewMethodAvailable?.Invoke(this, null);
        }
        /// <summary>
        /// Turns the pumps on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_on_Click(object sender, EventArgs e)
        {
            m_pump.PumpOn();
        }
        /// <summary>
        /// Turns the pumps off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_off_Click(object sender, EventArgs e)
        {
            m_pump.PumpOff();
        }
        /// <summary>
        /// Starts the pumps currently loaded time table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_start_Click(object sender, EventArgs e)
        {
            if (mcomboBox_methods.SelectedIndex < 0)
                return;

            var methodName = mcomboBox_methods.Items[mcomboBox_methods.SelectedIndex].ToString();
            m_pump.StartMethod(methodName);
        }
        /// <summary>
        /// Stops the pumps currently running method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_stop_Click(object sender, EventArgs e)
        {
            m_pump.StopMethod();
        }
        /// <summary>
        /// Sets the percent B of the pump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbutton_setPercentB_Click(object sender, EventArgs e)
        {
            m_pump.SetPercentB(Convert.ToDouble(mnum_percentB.Value));
        }
        private void mbutton_getPercentB_Click(object sender, EventArgs e)
        {
            mtextbox_percentB.Text = string.Format("{0:0.0}", m_pump.GetPercentB());
        }
        private void mbutton_loadMethods_Click(object sender, EventArgs e)
        {
            try
            {
                ReadMethodDirectory();
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                classApplicationLogger.LogError(0, ex.Message, ex);
            }
        }
        #endregion

        readonly Random r = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            //#if DEBUG
            if (Emulation)
            {
                timer1.Interval = m_pump.TotalMonitoringSecondElapsed * 1000;
                m_pump.PushData(r.NextDouble(), r.NextDouble(), r.NextDouble());
            }
            //#endif
        }

        private void mbutton_initializeDevice_Click(object sender, EventArgs e)
        {

            var index = mcomboBox_comPort.SelectedIndex;
            if (index < 0)
                return;

            m_pump.PortName = mcomboBox_comPort.Items[index].ToString();
            var errorMessage = "";
            m_pump.Initialize(ref errorMessage);
        }

        private void mbutton_purge_Click(object sender, EventArgs e)
        {
            mform_purges?.ShowDialog();
        }

        private void mcomboBox_comPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void mbutton_setPortName_Click(object sender, EventArgs e)
        {
            var index = mcomboBox_comPort.SelectedIndex;
            if (index < 0)
                return;

            m_pump.PortName = mcomboBox_comPort.Items[index].ToString();
        }
    }
}
