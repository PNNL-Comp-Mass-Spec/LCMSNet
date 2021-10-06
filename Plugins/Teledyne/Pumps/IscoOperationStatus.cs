namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Enum for ISCO operation status
    /// </summary>
    public enum IscoOperationStatus
    {
        Stopped,
        Running,
        Refilling,
        Hold,
        Equilibrating,
        InitSucceeded,
        Initializing,
        FlowSet,
        PressSet,
        ConstFlow,
        ConstPress
    }
}
