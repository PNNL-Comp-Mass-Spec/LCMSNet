
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

namespace ActiveMQTools
{
    public class classResposeSender
    {
        //*********************************************************************************************************
        // An object instantiated from this class ia intended to
        // send a response to a command that was received which had
        // a correlation id and a reply to queue
        //
        //  This code was liberally plagarized from sample code developed by GR Kiebel, PNNL.
        //**********************************************************************************************************

        #region "Class variables"
            private IConnection mobject_Connection;
            private string mstring_CorrelationId;
            private IDestination mobject_ResponseQueue;
        #endregion

        #region "Constructors"
            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="connection">Active MQ broker connection</param>
            /// <param name="correleationID">Unique ID for message that was received</param>
            /// <param name="responseQueue">Name of queue to handle response</param>
            public classResposeSender(IConnection connection, string correleationID, IDestination responseQueue)
            {
                mobject_Connection = connection;
                mstring_CorrelationId = correleationID;
                mobject_ResponseQueue = responseQueue;
            }   // End sub
        #endregion

        #region "Methods"
            /// <summary>
            /// Sends the response to a received message
            /// </summary>
            /// <param name="responseText">Response message text</param>
            public void SendResponse(string responseText)
            {
                if (mobject_Connection == null) return;

                try
                {
                    ISession session = mobject_Connection.CreateSession();
                    ITextMessage responseMessage = session.CreateTextMessage(responseText);
                    responseMessage.NMSCorrelationID = mstring_CorrelationId;

                    IMessageProducer producer = session.CreateProducer(mobject_ResponseQueue);
                    producer.Persistent = false;
                    producer.Send(responseMessage);
                }
                catch (Exception ex)
                {
                    //TODO: Figure out something intelligent to do here
                }
            }
        #endregion
    }   // Dnd class
}   // End namespace
