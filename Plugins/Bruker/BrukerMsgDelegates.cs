namespace LcmsNetPlugins.Bruker
{
    #region "Delegates used within Bruker Start classes"

    public delegate void delegateBrukerMsgReceived(BrukerComConstants.SxcReplies sXcReply);

    public delegate void delegatelNetworkDataReceived();

    public delegate void delegateDeviceError(object sender, string message);

    #endregion
}
