
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 10/21/2009
//
// Last modified 10/21/2009
//*********************************************************************************************************
using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.ActiveMQ;

namespace ActiveMQTools
{
	class classCmdReceiver : IDisposable
	{
		//*********************************************************************************************************
		// Yhis class establishes a connection to an ActiveMQ server
		// and sets up a listener for a queue through which it expects 
		// to receive commands.  Upon receiving a command, any registered 
		// processor delegate will be called with the command and an object
		// that allows the delegate to send a response back to the issuer
		// of that command, provided the issuer included a "ReplyTo" and 
		// "CorrelationID" parameters.
		//
		//	This code was liberally plagarized from sample code developed by GR Kiebel, PNNL.
		//**********************************************************************************************************

		#region "Class variables"
			private string mstring_QueueName = "";
			private string mstring_BrokerURI = "";
			private IConnection mobject_Connection;
			private bool mbool_IsDisposed = false;
			private bool mbool_HasConnection = false;
		#endregion

		#region "Events"
			/// <summary>
			/// Event fired to indicate a control command has been received
			/// </summary>
			public event DelegateCmdProcessorMsgReceived commandReceived;
		#endregion

		#region "Constructors"
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="brokerURI">URI for ActiveMQ borker</param>
			/// <param name="queueName">Name for command queue</param>
			public classCmdReceiver(string brokerURI, string queueName)
			{
				mstring_BrokerURI = brokerURI;
				mstring_QueueName = queueName;

				InitCmdReceiver();
			}
		#endregion

		#region "Methods"
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
					//TODO: Figure out what to do besides eat the exception
				}
			}	// End sub

			/// <summary>
			/// Hendles a command/response message received from the broker
			/// </summary>
			/// <param name="message">The message object that was received</param>
			private void OnCommandReceived(IMessage message)
			{
				ITextMessage textMsg = message as ITextMessage;

				// DAC Debugging
				string msg = DateTime.Now.ToString("hh:mm:ss.ff") + ", Message received by ActiveMQTools.OnCommandReceived: ";
				System.Diagnostics.Debug.WriteLine(msg);
				// End DAC Debugging

				if (commandReceived != null)
				{
					// Set up an object to allow the delegate to send response if requested
					classResposeSender respSender = null;
					if (message.NMSReplyTo != null)
					{
						respSender = new classResposeSender(mobject_Connection, message.NMSCorrelationID, message.NMSReplyTo);
						IDestination destination = message.NMSReplyTo;
					}
					// Trigger the event
					commandReceived(textMsg.Text, respSender);
				}
			}	// End sub

			/// <summary>
			/// Initializes the CommandReceiver object
			/// </summary>
			private void InitCmdReceiver()
			{
				try
				{
					if (!mbool_HasConnection) CreateConnection();
					if (!mbool_HasConnection) return;
					ISession session = mobject_Connection.CreateSession();
					IMessageConsumer consumer = session.CreateConsumer(new ActiveMQQueue(mstring_QueueName));
					consumer.Listener += new MessageListener(OnCommandReceived);
				}
				catch (Exception ex)
				{
					DestroyConnection();
				}
			}	// End sub

			/// <summary>
			/// Disconnects the object from the broker
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
				DestroyConnection();
				mbool_IsDisposed = true;
			}	// End sub
		#endregion
	}	// End class
}	// End namespace
