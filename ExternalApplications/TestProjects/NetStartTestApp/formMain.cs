using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ActiveMQTools;
using System.Collections.Specialized;
using System.Collections;

namespace NetStartTestApp
{
    public partial class formMain : Form
    {
        //*********************************************************************************************************
        //Insert general class description here
        //**********************************************************************************************************

        #region "Delegates"
            private delegate void DelegateDispRecMsg(StringDictionary receivedData);
        #endregion

        #region "Constants"
        #endregion

        #region "Class variables"
            string m_xmlToSend = "";
            string m_xmlToSendDelayed = "";
        #endregion

        #region "Events"
        #endregion

        #region "Properties"
        #endregion

        #region "Constructors"
            public formMain()
            {
                InitializeComponent();

                Initialize();
            }
        #endregion

        #region "Methods"
            private void Initialize()
            {
                // Get the instrument name
                txtInstName.Text = Properties.Settings.Default.InstName;

                // Get the ActiveMQ parameters
                lblBrokerUri.Text = Properties.Settings.Default.BrokerURI;
                lblTopicName.Text = Properties.Settings.Default.CmdQueueName;

                // Configure the ActiveMQ tools
                classMsgControl.BrokerURI = lblBrokerUri.Text;
                classMsgControl.CommandQueueName = lblTopicName.Text;
                classMsgControl.InstName = txtInstName.Text;
                classMsgControl.StatusTopicName = "NotUsed";
                classMsgControl.RunningOnCart = false;
                classMsgControl.InitMsgControl();

                // Subscribe to event indicating a command was received
                classMsgControl.ControlCmdReceived += new DelegateControlCmdReceived(OnCommandReceived);
            }

            void OnCommandReceived(object sender, string cmdText)
            {
                // Parse the received command
                StringDictionary receivedData = classXmlMsgTools.ParseCartCommand(cmdText);

                // Display the received data
                DisplayReceivedData(receivedData);

                // Perform action based on received command
                switch (receivedData["ResponseType"])
                {
                    case "StartAcquisition":
                        // If simulating a non-response, do nothing
                        if (rbStartNoResponse.Checked) return;

                        // Create a start message based on user form and command data
                        CreateStartAcqMessage(receivedData);

                        // Create a delayed start message in case it's needed
                        CreateDelayedStartAcqMessage(receivedData);

                        if (cbStartSendImm.Checked) SendMessage(m_xmlToSend);   // Send immediately was selected

                        // If the message was not sent immediately, it will be sent when the user specifies
                        break;
                    case "StopAcquisition":
                        // If simulating a non-response, do nothing
                        if (rbStopNoResp.Checked) return;

                        // Create a start message based on user form and command data
                        CreateStopAcqMessage(receivedData);

                        if (cbStopSendImm.Checked) SendMessage(m_xmlToSend);    // Send immediately was selected

                        // If the message was not sent immediately, it will be sent when the user specifies
                        break;
                    case "MethodNamesRequest":
                        // Build a list of methods
                        List<string> methodList = new List<string>();
                        foreach (object item in lbMethods.Items)
                        {
                            methodList.Add(item.ToString());
                        }
                        // Create XML-formatted string for message
                        m_xmlToSend = classXmlMsgTools.CreateMethodRequestResponse(receivedData["CartName"],methodList);

                        if (cbMethReqSendImm.Checked)   SendMessage(m_xmlToSend);   // Send immediately was selected

                        // If the message was not sent immediately, it will be sent when the user specifies
                        break;
                    default:
                        break;
                }   // End switch
            }

            private void CreateStartAcqMessage(StringDictionary receivedData)
            {
                classResponseData responseData = new classResponseData();

                // Cart name
                responseData.Cart = receivedData["CartName"];

                // Sample name
                responseData.SampleName = receivedData["SampleName"];

                // LC Method
                responseData.LcMethod = receivedData["LCMethodName"];

                // Instrument method
                responseData.InstMethod = receivedData["InstMethodName"];

                // Status
                if (rbStartStarted.Checked)
                {
                    responseData.Status = "Started";
                }
                else if (rbStartStarting.Checked)
                {
                    responseData.Status = "Starting";
                }
                else
                {
                    responseData.Status = "Error";
                }

                // Message
                responseData.Message = txtStartMsg.Text;

                // Create the XML-formatted message
                m_xmlToSend = classXmlMsgTools.CreateStartAcqResponse(responseData);
            }

            private void CreateDelayedStartAcqMessage(StringDictionary receivedData)
            {
                classResponseData responseData = new classResponseData();

                // Cart name
                responseData.Cart = receivedData["CartName"];

                // Sample name
                responseData.SampleName = receivedData["SampleName"];

                // LC Method
                responseData.LcMethod = receivedData["LCMethodName"];

                // Instrument method
                responseData.InstMethod = receivedData["InstMethodName"];

                // Status
                responseData.Status = "Started";

                // Message
                responseData.Message = txtStartMsg.Text;

                // Create the XML-formatted message
                m_xmlToSendDelayed = classXmlMsgTools.CreateStartAcqResponse(responseData);
            }

            private void CreateStopAcqMessage(StringDictionary receivedData)
            {
                classResponseData responseData = new classResponseData();

                // Cart name
                responseData.Cart = receivedData["CartName"];

                // Status
                if (rbStopStopped.Checked)
                {
                    responseData.Status = "Stopped";
                }
                else if (rbStopStopping.Checked)
                {
                    responseData.Status = "Stopping";
                }
                else
                {
                    responseData.Status = "Error";
                }

                // Message
                responseData.Message = txtStopMsg.Text;

                // Create the XML-formatted message
                m_xmlToSend = classXmlMsgTools.CreateStopAcqResponse(responseData);
            }

            private void SendMessage(string xmlText)
            {
                classMsgControl.SendCommand(xmlText);
                //string msgReply = classMsgControl.SendCommandWithReply(xmlText);
                //if (msgReply == classMsgControl.CMD_ACK_TIMEOUT)
                //{
                //   MessageBox.Show("Reply not acknowledged", "ERROR");
                //}
                //else
                //{
                //   MessageBox.Show("Reply sent and acknowledged", "NetStart Tester");
                //}
            }

            private void DisplayReceivedData(StringDictionary receivedData)
            {
                if (txtReceivedMsg.InvokeRequired)
                {
                    DelegateDispRecMsg d = new DelegateDispRecMsg(DisplayReceivedData);
                    txtReceivedMsg.Invoke(d, new object[] { receivedData });
                }
                else
                {
                    txtReceivedMsg.Text = "";
                    StringBuilder receivedString = new StringBuilder();

                    foreach (DictionaryEntry de in receivedData)
                    {
                        receivedString.Append(de.Key.ToString() + " +> " + de.Value.ToString() + Environment.NewLine);
                    }

                    txtReceivedMsg.Text = receivedString.ToString();
                    Application.DoEvents();
                }
            }

            private void btnMethReqSendNow_Click(object sender, EventArgs e)
            {
                SendMessage(m_xmlToSend);
            }

            private void btnStartSendNow_Click(object sender, EventArgs e)
            {
                if (rbStartStarting.Checked)
                {
                    SendMessage(m_xmlToSendDelayed);
                }
                else
                {
                    SendMessage(m_xmlToSend);
                }
            }

            private void btnStopSendNow_Click(object sender, EventArgs e)
            {
                SendMessage(m_xmlToSend);
            }

            private void cbStartSendImm_CheckedChanged(object sender, EventArgs e)
            {
                if (cbStartSendImm.Checked)
                {
                    btnStartSendNow.Enabled = false;
                }
                else
                {
                    btnStartSendNow.Enabled = true;
                }
            }

            private void cbStopSendImm_CheckedChanged(object sender, EventArgs e)
            {
                if (cbStopSendImm.Checked)
                {
                    btnStopSendNow.Enabled = false;
                }
                else
                {
                    btnStopSendNow.Enabled = true;
                }
            }

            private void cbMethReqSendImm_CheckedChanged(object sender, EventArgs e)
            {
                if (cbMethReqSendImm.Checked)
                {
                    btnMethReqSendNow.Enabled = false;
                }
                else
                {
                    btnMethReqSendNow.Enabled = true;
                }
            }

            private void btnClear_Click(object sender, EventArgs e)
            {
                txtReceivedMsg.Text = "";
            }
        #endregion
    }
}
