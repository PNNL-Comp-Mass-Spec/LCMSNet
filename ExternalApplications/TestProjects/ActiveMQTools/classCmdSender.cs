
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 10/21/2009
//
// Last modified 10/21/2009
//						- 11/30/2009 (DAC) - Modified to remove use of command/response queue
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ;

namespace ActiveMQTools
{
	class classCmdSender : IDisposable
	{
		//*********************************************************************************************************
		// An object of this class establishes a connection
		// with an ActiveMQ message broker and allows client
		// code to send a command via a message queue
		//
		//	This code was liberally plagarized from sample code developed by GR Kiebel, PNNL.
		//**********************************************************************************************************

		#region "Class variables"
			private string mstring_BrokerURI = "";
			private string mstring_QueueName = "";
			private string mstring_StatusTopicName = "";
			private IConnection mobject_Connection;
			private bool mbool_IsDisposed = false;
			private bool mbool_HasConnection = false;
		#endregion

		#region "Constructors"
			/// <summary>
			/// Object constructor
			/// </summary>
			/// <param name="brokerURI">URI for ActiveMQ broker</param>
			/// <param name="queueName">Name of command queue</param>
			/// <param name="statusTopicName">Name of status topic</param>
			public classCmdSender(string brokerURI, string queueName, string statusTopicName)
			{
				mstring_BrokerURI = brokerURI;
				mstring_QueueName = queueName;
				mstring_StatusTopicName = statusTopicName;
			}	// End sub
		#endregion

		#region "Methods"
			/// <summary>
			/// Sends a command to the command queue
			/// </summary>
			/// <param name="instName">Name of instrument program is running on (appended to queue name)</param>
			/// <param name="cmdText">Text of command to send</param>
			public void SendCmd(string cmdText)
			{
				if (!mbool_HasConnection) CreateConnection();
				if (!mbool_HasConnection) return;
				try
				{
					using (ISession session = mobject_Connection.CreateSession())
					{
						using (IMessageProducer producer = session.CreateProducer(new ActiveMQQueue(mstring_QueueName)))
						{
							ITextMessage message = session.CreateTextMessage(cmdText);
							producer.Send(message);
						}
					}
				}
				catch (Exception ex)
				{
					//TODO: Handle exception intelligently.
					// Since connection is probably dead, kill it to make sure
					DestroyConnection();
				}
			}	// End sub

			/// <summary>
			/// Sends a status message to the ActiveMQ broker
			/// </summary>
			/// <param name="cartName">Name of the cart program is running on</param>
			/// <param name="statusText">Message text to send</param>
			public void SendStatus(string instName, string statusText)
			{
				if (!mbool_HasConnection) CreateConnection();
				if (!mbool_HasConnection) return;
				try
				{
					using (ISession statusSession = mobject_Connection.CreateSession())
					{
						using (IMessageProducer statusSender = statusSession.CreateProducer(new ActiveMQTopic(mstring_StatusTopicName)))
						{
							ITextMessage textMessage = statusSession.CreateTextMessage(statusText);
							textMessage.Properties.SetString("Instrument", instName);
							statusSender.Send(textMessage);
						}
					}	// End using
				}
				catch (Exception ex)
				{
					//TODO: Handle exception
				}
			}	// End sub

			/// <summary>
			/// Creates a connection to the ActiveMQ broker
			/// </summary>
			private void CreateConnection()
			{
				if (mbool_HasConnection) return;
				try
				{
					IConnectionFactory connectionFactory = new ConnectionFactory(mstring_BrokerURI);
					mobject_Connection = connectionFactory.CreateConnection();
					mobject_Connection.Start();
					mbool_HasConnection = true;
				}
				catch (Exception ex)
				{
					//TODO: Figure out what to do here
				}
			}	// End sub

			/// <summary>
			/// Removes the ActiveMQ broker connectiion
			/// </summary>
			private void DestroyConnection()
			{
				if (mbool_HasConnection)
				{
					mobject_Connection.Dispose();
					mbool_HasConnection = false;
				}
			}	// End sub

			/// <summary>
			/// Implements IDisposable.Dispose
			/// </summary>
			public void Dispose()
			{
				if (!mbool_IsDisposed)
				{
					DestroyConnection();
					mbool_IsDisposed = true;
				}
			}	// End sub
		#endregion
	}	// End class
}	// End namespace
