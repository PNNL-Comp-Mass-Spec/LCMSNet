using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ActiveMQTools;

namespace ActiveMQModuleTest
{
	public partial class Form1 : Form
	{
		//private ActiveMQTools.classMsgControl msgControl = null;

		public Form1()
		{
			InitializeComponent();

			InitForm();
		}

		private void InitForm()
		{
			//msgControl = new classMsgControl("Elmer", txtBrokerURI.Text, txtQueueName.Text,
			//                  txtStatusTopic.Text);

			// Initialize ActuveMQ handling class
			classMsgControl.BrokerURI = txtBrokerURI.Text;
			classMsgControl.CommandQueueName = txtQueueName.Text;
			classMsgControl.InstName = "Elmer";
			classMsgControl.StatusTopicName = txtStatusTopic.Text;
			classMsgControl.InitMsgControl();
			classMsgControl.ControlCmdReceived += new DelegateControlCmdReceived(OnControlCmdReceived);
		}

		void OnControlCmdReceived(object sender, string cmdText)
		{
			txtCmdReceived.Text = cmdText;
		}

		private void btnSendWithReply_Click(object sender, EventArgs e)
		{
			string response = classMsgControl.SendCommandWithReply(txtQueueMsg.Text);
			//string response = msgControl.SendCommandWithReply(txtQueueMsg.Text);
			txtQueueReply.Text = response;
		}

		private void btnSendStatus_Click(object sender, EventArgs e)
		{
			classMsgControl.SendStatusMsg(txtStatus.Text);
		}
	}
}
