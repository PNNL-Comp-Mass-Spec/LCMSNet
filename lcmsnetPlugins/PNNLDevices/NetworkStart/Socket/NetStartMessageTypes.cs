namespace LcmsNetPlugins.PNNLDevices.NetworkStart.Socket
{
    public enum NetStartMessageTypes
    {
        Unknown = 0,
        Query,
        Post,
        Execute,
        Acknowledge,
        Response,
        Error,
        System,
        SystemError
    }
}
