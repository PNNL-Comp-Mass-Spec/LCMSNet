
//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 10/21/2009
//
// Last modified 10/21/2009
//                      - 11/30/2009 (DAC) - Modified to remove use of command/response queue
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActiveMQTools
{
    public class classMsgControl
    {
        //*********************************************************************************************************
        // Main DLL entry point. This class provides a wrapper around the detailed methods used for communication
        //      with other devices via ActiveMQ/NMS
        //
        //  This class can be either instantiated, or accessed via static methods and properties
        //**********************************************************************************************************

        #region "Events"
            /// <summary>
            /// Event fired to indicate a control command has been received
            /// </summary>
            public static event DelegateControlCmdReceived ControlCmdReceived;
        #endregion

        #region "Class variables"
            private static classCmdReceiver mobject_CmdReceiver;
            private static classCmdSender mobject_CmdSender;
            private static string mstring_BrokerURI;
            private static string mstring_CommandQueueName;
            private static string mstring_StatusTopicName;
            private static string mstring_InstName;
            private static bool mbool_RunningOnCart = true;
        #endregion

        #region "Properties"
            public static string InstName
            {
                get { return mstring_InstName; }
                set { mstring_InstName = value; }
            }

            public static string BrokerURI
            {
                get { return mstring_BrokerURI; }
                set { mstring_BrokerURI = value; }
            }

            public static string CommandQueueName
            {
                get { return mstring_CommandQueueName; }
                set { mstring_CommandQueueName = value; }
            }

            public static string StatusTopicName
            {
                get { return mstring_StatusTopicName; }
                set { mstring_StatusTopicName = value; }
            }

            public static bool RunningOnCart
            {
                get { return mbool_RunningOnCart; }
                set { mbool_RunningOnCart = value; }
            }
        #endregion

        #region "Constructors"
        #endregion

        #region "Methods"
            /// <summary>
            /// Initializes classes called by this class. Assumes properties have already been set.
            /// </summary>
            public static void InitMsgControl()
            {
                string cmdQueueName = cmdQueueName = mstring_CommandQueueName + "." + mstring_InstName + ".Cmd";
                string replyQueueName = replyQueueName = mstring_CommandQueueName + "." + mstring_InstName + ".Reply";

                if (mbool_RunningOnCart)
                {
                    // Class is being used on a cart, so use command queue for sender and reply queue for receiver
                    InitCommandSender(mstring_BrokerURI, cmdQueueName, mstring_StatusTopicName);
                    InitCommandReceiver(mstring_BrokerURI, replyQueueName);
                }
                else
                {
                    // Class is being used on an instrument, so use reply queue for sender and command queue for receiver
                    InitCommandSender(mstring_BrokerURI, replyQueueName, mstring_StatusTopicName);
                    InitCommandReceiver(mstring_BrokerURI, cmdQueueName);
                }
            }

            /// <summary>
            /// Event handler for commands received via the command queue.
            /// </summary>
            /// <param name="cmdText">Text of received command</param>
            /// <param name="respSender">Object containing response to return to sender, command/response is used</param>
            private static void HandleCmd(string cmdText, classResposeSender respSender)
            {
                // DAC Debugging
                string msg = DateTime.Now.ToString("hh:mm:ss.ff") + ", Message received by ActiveMQTools.HandleCmd: ";
                System.Diagnostics.Debug.WriteLine(msg);
                // End DAC Debugging

                if (ControlCmdReceived != null)
                {
                    // DAC Debugging
                    string msg3 = DateTime.Now.ToString("hh:mm:ss.ff") + ", HandleCmd triggering ControlCmdReceived event";
                    System.Diagnostics.Debug.WriteLine(msg3);
                    // End DAC Debugging

                    ControlCmdReceived(null, cmdText);
                }
            }

            /// <summary>
            /// Inititializes the CommandReceiver object
            /// </summary>
            /// <param name="brokerURI">Address of ActiveMQ broker</param>
            /// <param name="queueName">Queue name for command queue</param>
            private static void InitCommandReceiver(string brokerURI, string queueName)
            {
                mobject_CmdReceiver = new classCmdReceiver(brokerURI, queueName);
                mobject_CmdReceiver.commandReceived += new DelegateCmdProcessorMsgReceived(HandleCmd);
            }

            /// <summary>
            /// Disconnects event handlers and closes command receiver
            /// </summary>
            public static void Dispose()
            {
                if (mobject_CmdReceiver != null)
                {
                    mobject_CmdReceiver.commandReceived -= HandleCmd;
                    mobject_CmdReceiver.Dispose();
                    mobject_CmdReceiver = null;
                }

                if (mobject_CmdSender != null)
                {
                    mobject_CmdSender.Dispose();
                    mobject_CmdSender = null;
                }
            }

            /// <summary>
            /// Initializes the CommandSender object
            /// </summary>
            /// <param name="brokerURI">Address of ActiveMQ broker</param>
            /// <param name="queueName">Queue name for command queue</param></param>
            /// <param name="statusTopicName">Topic name for status broadcast topic</param>
            private static void InitCommandSender(string brokerURI, string queueName, string statusTopicName)
            {
                mobject_CmdSender = new classCmdSender(brokerURI, queueName, statusTopicName);
            }

            /// <summary>
            /// Sends a command to the command queue
            /// </summary>
            /// <param name="cmdText">Text of command to send</param>
            public static void SendCommand(string cmdText)
            {
                mobject_CmdSender.SendCmd(cmdText);
            }

            /// <summary>
            /// Sends a status message to a status topic. This method doesn't support command/response queues
            /// </summary>
            /// <param name="statusMsg">Message to be sent</param>
            public static void SendStatusMsg(string statusMsg)
            {
                string msgToSend = statusMsg.Replace("InsertCartHere", mstring_InstName);   // Cheezy way to ensure cart name
                                                                                                                    // is in status message
                mobject_CmdSender.SendStatus(mstring_InstName, msgToSend);
            }
        #endregion
    }
}
