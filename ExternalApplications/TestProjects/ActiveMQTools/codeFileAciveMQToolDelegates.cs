
//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 10/21/2009
//
// Last modified 10/21/2009
//*********************************************************************************************************
namespace ActiveMQTools
{
    #region "Delegates for use with ActiveMQ tools DLL#

    /// <summary>
    /// Received commands are sent to a delegate function with this signature
    /// </summary>
    /// <param name="cmdText">Text of received command</param>
    /// <param name="respSender">Object to be used in returning response</param>
    public delegate void DelegateCmdProcessorMsgReceived(string cmdText, classResposeSender respSender);

    /// <summary>
    /// Used for event that is fired to indicate a command has been received and acknowledged
    /// </summary>
    /// <param name="sender">Object firing event (normally null because static class is used)</param>
    /// <param name="cmdText">Text of received command</param>
    public delegate void DelegateControlCmdReceived(object sender, string cmdText);

    #endregion
}
