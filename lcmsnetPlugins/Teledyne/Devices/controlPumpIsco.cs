using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LcmsNetDataClasses.Devices;
using System.Xml;
using System.Xml.Linq;

using ZedGraph;

using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices.Pumps
{
    public partial class controlPumpIsco : controlBaseDeviceControl, IDeviceControl
    {
        #region "Class variables"
            classPumpIsco m_Pump;
            classPumpIscoDisplayArray m_PumpDisplays;
            int m_PumpCount = 3;
            double[] mdouble_refillRates;
            double[] mdouble_MaxRefillRates;
        #endregion

        #region "Properties"
            public IDevice Device
            {
                get { return m_Pump; }
                set { RegisterDevice(value); }
            }

            public bool Emulation
            {
                get { return m_Pump.Emulation; }
                set { m_Pump.Emulation = value; }
            }
        #endregion

        #region "Constructors"
            public controlPumpIsco()
            {
                InitializeComponent();
            }
        #endregion

        #region "Methods"
            /// <summary>
            /// Initializes the controls and displays
            /// </summary>
            private void InitControl()
            {
                // Add a list of available com ports to the selection combo box
                mcomboBox_Ports.Items.Clear();
                mcomboBox_Ports.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                if (mcomboBox_Ports.Items.Count > 0) mcomboBox_Ports.SelectedIndex = 0;

                // Add a list of available unit addresses to the unit address combo box
                mcomboBox_UnitAddress.Items.Clear();
                for (var indx = m_Pump.UnitAddressMin; indx < m_Pump.UnitAddressMax + 1; indx++)
                {
                    mcomboBox_UnitAddress.Items.Add(indx);
                }
                // Set the default to 6, if allowed; otherwise use minimum value
                var itemIndx = mcomboBox_UnitAddress.Items.IndexOf(6);
                if (itemIndx != -1)
                {
                    mcomboBox_UnitAddress.SelectedIndex = itemIndx;
                }
                else mcomboBox_UnitAddress.SelectedIndex = 0;

                // Fill in the Notes text box
                var noteStr = "1) Also max allowed pressure in Const Flow mode" + Environment.NewLine;
                noteStr += "2) Max allowed flow in Const Press mode";
                mtextBox_Notes.Text = noteStr;

                // Initialize the pump display controls
                m_PumpDisplays = new classPumpIscoDisplayArray();
                m_PumpDisplays.AddDisplayControl(mcontrol_PumpA);
                m_PumpDisplays.AddDisplayControl(mcontrol_PumpB);
                m_PumpDisplays.AddDisplayControl(mcontrol_PumpC);
                for (var indx = 0; indx < m_PumpDisplays.Count; indx++)
                {
                    m_PumpDisplays[indx].InitControl(indx);
                }

                // Assign pump display control event handlers
                m_PumpDisplays.SetpointChanged += new DelegateIscoPumpDisplaySetpointHandler(m_PumpDisplays_SetpointChanged);
                m_PumpDisplays.StartPump += new DelegateIscoPumpDisplayHandler(m_PumpDisplays_StartPump);
                m_PumpDisplays.StopPump += new DelegateIscoPumpDisplayHandler(m_PumpDisplays_StopPump);
                m_PumpDisplays.StartRefill += new DelegateIscoPumpDisplayHandler(m_PumpDisplays_StartRefill);

                // Assign pump class event handlers
                m_Pump.RefreshComplete += new DelegateIscoPumpRefreshCompleteHandler(m_Pump_RefreshComplete);
                m_Pump.InitializationComplete += new DelegateIscoPumpInitializationCompleteHandler(m_Pump_InitializationComplete);
                m_Pump.ControlModeSet += new DelegateIscoPumpControlModeSetHandler(m_Pump_ControlModeSet);
                m_Pump.OperationModeSet += new DelegateIscoPumpOpModeSetHandler(m_Pump_OperationModeSet);
                m_Pump.Disconnected += new DelegateIscoPumpDisconnected(m_Pump_Disconnected);

#if DACTEST
                m_Pump.StatusUpdate += new EventHandler<classDeviceStatusEventArgs>(m_Pump_StatusUpdate);
                m_Pump.Error += new EventHandler<classDeviceErrorEventArgs>(m_Pump_Error);
#endif
                // Initial control mode display
                mcomboBox_ControlMode.SelectedIndex = 0;

                // Set initial number of pumps to max
                mcomboBox_PumpCount.SelectedIndex = m_Pump.PumpCount - 1;

                // Set initial operation mode display
                mcomboBox_OperationMode.SelectedIndex = 1;

                // Initialize refill rate array
                mdouble_refillRates = new double[] { double.Parse(mtextBox_RefillSpA.Text),
                    double.Parse(mtextBox_RefillSpB.Text),  double.Parse(mtextBox_RefillSpC.Text) };

                // Initialize max refill rate array
                mdouble_MaxRefillRates = new double[] { 30D, 30D, 30D };

                var index = mcomboBox_Ports.Items.IndexOf(m_Pump.PortName);
                if (index >= 0)
                {
                    mcomboBox_Ports.SelectedIndex = index;
                }
            }   

            private void RegisterDevice(IDevice device)
            {
                m_Pump = device as classPumpIsco;
                InitControl();

                // Add to the device manager
                SetBaseDevice(m_Pump);
            }   

            //public override bool RemoveDevice()
            //{
            //    try
            //    {
            //        // Shutdown the object's connections
            //        m_Pump.Shutdown();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new IscoException("Exception shutting down ISCO pump", m_Pump.GetBaseException(ex));
            //    }
            //    return classDeviceManager.Manager.RemoveDevice(this.Device);
            //} 

#if DACTEST
            /// <summary>
            /// Logs an ISCO message to a file (testing only)
            /// </summary>
            /// <param name="msg"></param>
            public void LogDebugMessage(string msg)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath);
                string outFile = System.IO.Path.Combine(fi.DirectoryName, "MsgLog.txt");

                string logTxt = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).ToString("MM/dd/yyyy HH:mm:ss.ff") + ", " + msg;

                using (System.IO.StreamWriter w = System.IO.File.AppendText(outFile))
                {
                    w.WriteLine(logTxt);
                    w.Flush();
                    w.Close();
                }
            }   
#endif

            /// <summary>
            /// Updates the display of limits values
            /// </summary>
            private void UpdateLimitDisplay()
            {
                ListViewItem[] itemArray = {new ListViewItem("Flow Units"), new ListViewItem("Pressure Units"),
                                                        new ListViewItem("Level Units"), new ListViewItem("Min Pressure SP"),
                                                        new ListViewItem("Max Pressure SP (Note 1)"),
                                                        new ListViewItem("Min Flow SP"), new ListViewItem("Max Flow SP"),
                                                        new ListViewItem("Max Flow Value (Note 2)"),
                                                        new ListViewItem("Min Refill Rate SP"), new ListViewItem("Max Refill Rate SP")};

                for (var indx = 0; indx < m_PumpDisplays.Count; indx++)
                {
                    // Flow units
                    itemArray[0].SubItems.Add(classIscoConversions.GetFlowUnitsString());

                    // Pressure units
                    itemArray[1].SubItems.Add(classIscoConversions.GetPressUnitsString());

                    // Level units
                    itemArray[2].SubItems.Add("mL");

                    // Min press SP
                    itemArray[3].SubItems.Add(m_PumpDisplays[indx].MinPressSp.ToString());

                    // Max press SP
                    itemArray[4].SubItems.Add(m_PumpDisplays[indx].MaxPressSp.ToString());

                    // Min flow SP
                    itemArray[5].SubItems.Add(m_PumpDisplays[indx].MinFlowSp.ToString());

                    // Max flow SP
                    itemArray[6].SubItems.Add(m_PumpDisplays[indx].MaxFlowSp.ToString());

                    // Max flow limit
                    itemArray[7].SubItems.Add(m_PumpDisplays[indx].MaxFlowLimit.ToString());

                    // Min refill rate SP
                    itemArray[8].SubItems.Add("0.0");

                    // Max refill rate SP
                    itemArray[9].SubItems.Add(mdouble_MaxRefillRates[indx].ToString());
                }

                mlistView_Limits.BeginUpdate();
                mlistView_Limits.Items.Clear();
                mlistView_Limits.Items.AddRange(itemArray);
                mlistView_Limits.EndUpdate();
            }   
        #endregion

        #region "Form event handlers"
            /// <summary>
            /// Starts refill for a single pump
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="pumpIndx"></param>
            void m_PumpDisplays_StartRefill(object sender, int pumpIndx)
            {
                if (m_Pump.StartRefill(pumpIndx, mdouble_refillRates[pumpIndx]))
                {
                    UpdateStatusDisplay("Refill started");
                }
                else UpdateStatusDisplay("Refill error");
            }   

            /// <summary>
            /// Stops specified pump
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="pumpIndx"></param>
            void m_PumpDisplays_StopPump(object sender, int pumpIndx)
            {
                if (m_Pump.StopPump(0, pumpIndx))
                {
                    UpdateStatusDisplay("Pump stopped");
                }
                else UpdateStatusDisplay("Problem stopping pump");
            }   

            /// <summary>
            /// Starts specified pump
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="pumpIndx"></param>
            void m_PumpDisplays_StartPump(object sender, int pumpIndx)
            {
                if (m_Pump.StartPump(0, pumpIndx))
                {
                    UpdateStatusDisplay("Pump started");
                    
                }
                else UpdateStatusDisplay("Problem starting pump");
            }   

            /// <summary>
            /// Changes the setpoint for one pump
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="pumpIndex">Pump to change</param>
            /// <param name="newValue">new setpoint</param>
            void m_PumpDisplays_SetpointChanged(object sender, int pumpIndex, double newValue)
            {
                var success = true;

                if (m_Pump.OperationMode == enumIscoOperationMode.ConstantFlow)
                {
                    success = m_Pump.SetFlow(pumpIndex, newValue);
                }
                else success = m_Pump.SetPressure(pumpIndex, newValue);

                if (success)
                {
                    UpdateStatusDisplay("Setpoint changed");
                }
                else UpdateStatusDisplay("Problem making setpoint change");
            }   

            /// <summary>
            /// Refreshes the pump displays
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_Refresh_Click(object sender, EventArgs e)
            {
                if (!m_Pump.Refresh())
                {
                    UpdateStatusDisplay("Problem refreshing pump status");
                }
                
            }   

            /// <summary>
            /// Starts all pumps
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_StartAll_Click(object sender, EventArgs e)
            {
                var success = true;

                for (var pumpIndx = 0; pumpIndx < m_PumpCount; pumpIndx++)
                {
                    if (!m_Pump.StartPump(0, pumpIndx)) success = false;
                }

                if (success)
                {
                    UpdateStatusDisplay("All pumps started");
                }
                else UpdateStatusDisplay("Problem starting one or more pumps");
            }   

            /// <summary>
            /// Stops all pumps
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbuttonStopAll_Click(object sender, EventArgs e)
            {
                var success = true;

                for (var pumpIndx = 0; pumpIndx < m_PumpCount; pumpIndx++)
                {
                    if (!m_Pump.StopPump(0, pumpIndx)) success = false;
                }

                if (success)
                {
                    UpdateStatusDisplay("All pumps stopped");
                }
                else UpdateStatusDisplay("Problem stopping one or more pumps");
            }   

            /// <summary>
            /// Sets the flow setpoint for all pumps
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_SetAllFlow_Click(object sender, EventArgs e)
            {
                if (!(m_Pump.OperationMode == enumIscoOperationMode.ConstantFlow))
                {
                    UpdateStatusDisplay("Pump must be in constant flow mode");
                    return;
                }

                var success = true;
                for (var indx = 0; indx < m_PumpCount; indx++)
                {
                    if (!m_Pump.SetFlow(indx, m_PumpDisplays[indx].Setpoint)) success = false;
                }

                if (success)
                {
                    UpdateStatusDisplay("Flow setpoints changed");
                }
                else UpdateStatusDisplay("Problem changing one or more flow setpoints");
            }   

            /// <summary>
            /// Sets pressure setpoint for all pumps
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbuttonSetAllPress_Click(object sender, EventArgs e)
            {
                if (!(m_Pump.OperationMode == enumIscoOperationMode.ConstantPressure))
                {
                    UpdateStatusDisplay("Pump must be in constant pressure mode");
                    return;
                }

                var success = true;
                for (var indx = 0; indx < m_PumpCount; indx++)
                {
                    if (!m_Pump.SetPressure(indx, m_PumpDisplays[indx].Setpoint)) success = false;
                }

                if (success)
                {
                    UpdateStatusDisplay("Pressure setpoints changed");
                }
                else UpdateStatusDisplay("Problem changing one or more pressure setpoints");
            }   

            /// <summary>
            /// Starts a refill operation on all pumps
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_RefillAll_Click(object sender, EventArgs e)
            {
                var success = true;
                for (var pumpIndx = 0; pumpIndx < m_PumpCount; pumpIndx++)
                {
                    if (!m_Pump.StartRefill(pumpIndx, mdouble_refillRates[pumpIndx])) success = false;
                }

                if (success)
                {
                    UpdateStatusDisplay("Refill started all pumps");
                }
                else UpdateStatusDisplay("Problem starting refill on one or more pumps");
            }   

            /// <summary>
            /// Sets the control mode
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_SetControlMode_Click(object sender, EventArgs e)
            {
                var newMode = enumIscoControlMode.External;

                if ((string)mcomboBox_ControlMode.SelectedItem == "Local")
                {
                    newMode = enumIscoControlMode.Local;
                }
                else newMode = enumIscoControlMode.Remote;

                if (m_Pump.SetControlMode(newMode))
                {
                    UpdateStatusDisplay("Control mode changed");
                }
                else UpdateStatusDisplay("Problem setting control mode");
            }   

            /// <summary>
            /// Combo box for choosing number of pumps in use
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mcomboBox_PumpCount_SelectedIndexChanged(object sender, EventArgs e)
            {
                m_PumpCount = int.Parse(mcomboBox_PumpCount.SelectedItem.ToString());
                m_Pump.PumpCount = m_PumpCount;
            }   

            /// <summary>
            /// Sets the operation mode
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_SetOpMode_Click(object sender, EventArgs e)
            {
                var newMode = enumIscoOperationMode.ConstantPressure;

                if ((string)mcomboBox_OperationMode.SelectedItem == "Const Flow")
                {
                    newMode = enumIscoOperationMode.ConstantFlow;
                }
                else newMode = enumIscoOperationMode.ConstantPressure;

                if (m_Pump.SetOperationMode(newMode))
                {
                    for (var indx = 0; indx < m_PumpDisplays.Count; indx++)
                    {
                        m_PumpDisplays[indx].OperationMode = newMode;
                    }
                    UpdateStatusDisplay("Operation mode changed");
                }
                else UpdateStatusDisplay("Problem setting control mode");
            }   

            /// <summary>
            /// Sets the serial port properties object contents
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_SetPortProperties_Click(object sender, EventArgs e)
            {
                if (m_Pump.IsOpen())
                {                    
                }
                m_Pump.PortName = (string)mcomboBox_Ports.SelectedItem;
                m_Pump.BaudRate = int.Parse(mtextBox_BaudRate.Text);
                m_Pump.ReadTimeout = int.Parse(mtextBox_ReadTimeout.Text);
                m_Pump.WriteTimeout = int.Parse(mtextBox_WriteTimeout.Text);
                m_Pump.UnitAddress = (int)mcomboBox_UnitAddress.SelectedItem;
            }   

            /// <summary>
            /// Sets the refill rate SP's
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void mbutton_SetRefillRate_Click(object sender, EventArgs e)
            {
                // Warn if pump isn't initialized, which means range limits aren't available
                if (!m_Pump.Initialized)
                {
                    UpdateStatusDisplay("Pump not initialized");
                    return;
                }

                // For ease of manipulation, put the setpoint text boxes into an array
                var spTextBoxes = new TextBox[] {mtextBox_RefillSpA, mtextBox_RefillSpB, mtextBox_RefillSpC};

                for (var indx = 0; indx < mdouble_refillRates.Length; indx++)
                {
                    double newSp;
                    try
                    {
                        newSp = double.Parse(spTextBoxes[indx].Text);
                    }
                    catch
                    {
                        UpdateStatusDisplay("Unable to parse setpoint string" + spTextBoxes[indx].Text);
                        return;
                    }

                    // Check to see if new SP is within allowed range
                    if ((newSp >= 0) && (newSp <= mdouble_MaxRefillRates[indx]))
                    {
                        mdouble_refillRates[indx] = newSp;
                    }
                    else
                    {
                        UpdateStatusDisplay("New setpoint " + newSp.ToString() + " out of range");
                        return;
                    }
                }
            }   
        #endregion

        #region "Misc event handlers"
            /// <summary>
            /// Refresh complete handler
            /// </summary>
            void m_Pump_RefreshComplete()
            {
                try
                {
                    for (var pumpIndx = 0; pumpIndx < m_PumpDisplays.Count; pumpIndx++)
                    {
                        var pumpData = m_Pump.GetPumpData(pumpIndx);
                        if (pumpData != null)
                        {
                            m_PumpDisplays[pumpIndx].FlowRate = pumpData.Flow;
                            m_PumpDisplays[pumpIndx].Pressure = pumpData.Pressure;
                            m_PumpDisplays[pumpIndx].Volume = pumpData.Volume;
                            //m_PumpDisplays[pumpIndx].Setpoint = pumpData.SetPoint;
                            m_PumpDisplays[pumpIndx].ProblemStatus = pumpData.ProblemStatus;
                        }

                        //classPumpIscoSetpointLimits setpointLimits = m_Pump.GetSetpointLimits(pumpIndx);
                        //if (setpointLimits != null)
                        //{
                        //   m_PumpDisplays[pumpIndx].MinFlowSp = setpointLimits.MinFlowSp;
                        //   m_PumpDisplays[pumpIndx].MaxFlowSp = setpointLimits.MaxFlowSp;
                        //   m_PumpDisplays[pumpIndx].MinPressSp = setpointLimits.MinPressSp;
                        //   m_PumpDisplays[pumpIndx].MaxPressSp = setpointLimits.MaxPressSp;
                        //   m_PumpDisplays[pumpIndx].MaxFlowLimit = setpointLimits.MaxFlowLimit;
                        //}
                    }

                }
                catch(Exception ex)
                {
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "Exception occured trying to refresh pump data " + ex.StackTrace);
                }
                //mcontrol_IscoGraphs.UpdateAllPlots(m_Pump.PumpData);
            }   
            
            /// <summary>
            /// Initialization complete handler
            /// </summary>
            void m_Pump_InitializationComplete()
            {
                // Set the operation mode displays
                var currOpMode = m_Pump.OperationMode;
                if (currOpMode == enumIscoOperationMode.ConstantFlow)
                {
                    mcomboBox_OperationMode.SelectedIndex = 0;
                }
                else mcomboBox_OperationMode.SelectedIndex = 1;
                for (var indx = 0; indx < m_PumpDisplays.Count; indx++)
                {
                    m_PumpDisplays[indx].OperationMode = currOpMode;
                }

                // Get data for limits display
                for (var indx = 0; indx < m_PumpCount; indx++)
                {
                    var rangeData = m_Pump.GetPumpRanges(indx);
                    var setpointLimits = m_Pump.GetSetpointLimits(indx);

                    if (rangeData == null) rangeData = new classPumpIscoRangeData(); // Use the defaults in the class
                    if (setpointLimits == null) setpointLimits = new classPumpIscoSetpointLimits(); // Use the defaults in the class

                    m_PumpDisplays[indx].MinFlowSp = setpointLimits.MinFlowSp;
                    m_PumpDisplays[indx].MaxFlowSp = setpointLimits.MaxFlowSp;
                    m_PumpDisplays[indx].MinPressSp = setpointLimits.MinPressSp;
                    m_PumpDisplays[indx].MaxPressSp = setpointLimits.MaxPressSp;
                    m_PumpDisplays[indx].MaxFlowLimit = setpointLimits.MaxFlowLimit;

                    mdouble_MaxRefillRates[indx] = rangeData.MaxRefillRate;

                    mdouble_refillRates[indx] = m_Pump.GetPumpData(indx).RefillRate;
                }

                // Fill in limits display
                UpdateLimitDisplay();
    
                // Init refill rate displays
                mtextBox_RefillSpA.Text = mdouble_refillRates[0].ToString();
                mtextBox_RefillSpB.Text = mdouble_refillRates[1].ToString();
                mtextBox_RefillSpC.Text = mdouble_refillRates[2].ToString();

                //// Clear the pump displays
                //mcontrol_IscoGraphs.ClearGraphs();
            }   

            /// <summary>
            /// OperationModeSet handler
            /// </summary>
            /// <param name="newMode"></param>
            void m_Pump_OperationModeSet(enumIscoOperationMode newMode)
            {
                if (newMode == enumIscoOperationMode.ConstantFlow)
                {
                    mcomboBox_OperationMode.SelectedIndex = 0;
                }
                else mcomboBox_OperationMode.SelectedIndex = 1;
            }   

            /// <summary>
            /// ControlModeSet handler
            /// </summary>
            /// <param name="newMode"></param>
            void m_Pump_ControlModeSet(enumIscoControlMode newMode)
            {
                if (newMode == enumIscoControlMode.Local)
                {
                    mcomboBox_ControlMode.SelectedIndex = 0;
                }
                else mcomboBox_ControlMode.SelectedIndex = 1;
            }   

            /// <summary>
            /// Handles pump class disconnect
            /// </summary>
            void m_Pump_Disconnected()
            {
                //Included for future use
            }        

#if DACTEST
            void m_Pump_Error(object sender, classDeviceErrorEventArgs e)
            {
                string msg = "ERROR: " + e.Notification;
                LogDebugMessage(msg);
            }

            void m_Pump_StatusUpdate(object sender, classDeviceStatusEventArgs e)
            {
                string msg = "STATUS: " + e.Notification;
                LogDebugMessage(msg);
            }   
#endif

        #endregion

        #region "IDevice methods"
            ///// <summary>
            ///// Saves device settings
            ///// </summary>
            ///// <returns>XML element containing device settings</returns>
            //public System.Xml.XmlElement SaveDeviceSettings()
            //{
            //   XmlDocument doc = new XmlDocument();
            //   XmlElement element = doc.CreateElement("PumpSettings");
            //   m_device.SaveDeviceSettings(element);
            //   return element;
            //} 

            ///// <summary>
            ///// Loads device settings
            ///// </summary>
            ///// <param name="deviceNode">XML element containing device settings</param>
            //public void LoadDeviceSettings(System.Xml.XmlElement deviceNode)
            //{
            //   // Get the settings
            //   m_device.LoadDeviceSettings(deviceNode);

            //   // Serial port
            //   if (mcomboBox_Ports.Items.Contains(m_Pump.PortName))
            //   {
            //      int index = mcomboBox_Ports.Items.IndexOf(m_Pump.PortName);
            //      mcomboBox_Ports.SelectedIndex = index;
            //   }

            //   // Unit address
            //   if (mcomboBox_UnitAddress.Items.Contains(m_Pump.UnitAddress))
            //   {
            //      int index = mcomboBox_UnitAddress.Items.IndexOf(m_Pump.UnitAddress);
            //      mcomboBox_UnitAddress.SelectedIndex = index;
            //   }

            //   // Pump count
            //   if (mcomboBox_PumpCount.Items.Contains(m_Pump.PumpCount))
            //   {
            //      int index = mcomboBox_PumpCount.Items.IndexOf(m_Pump.PumpCount);
            //      mcomboBox_PumpCount.SelectedIndex = index;
            //   }

            //   // Baud rate
            //   mtextBox_BaudRate.Text = m_Pump.BaudRate.ToString();

            //   // Write timeout
            //   mtextBox_WriteTimeout.Text = m_Pump.WriteTimeout.ToString();

            //   // Read timeout
            //   mtextBox_ReadTimeout.Text = m_Pump.ReadTimeout.ToString();

            //   OnNameChanged(m_device.Name);
            //}
        #endregion

    }   
}
