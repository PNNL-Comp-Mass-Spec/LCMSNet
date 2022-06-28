namespace LcmsNetPlugins.Teledyne.Pumps
{
    public delegate void DelegateIscoPumpDisplayHandler(object sender, int pumpIndx);

    public delegate void DelegateIscoPumpDisplaySetpointHandler(object sender, int pumpIndex, double newValue);

    public delegate void DelegateIscoPumpRefreshCompleteHandler();

    public delegate void DelegateIscoPumpInitializationCompleteHandler();

    public delegate void DelegateIscoPumpInitializingHandler();

    public delegate void DelegateIscoPumpOpModeSetHandler(IscoOperationMode newMode);

    public delegate void DelegateIscoPumpControlModeSetHandler(IscoControlMode newMode);

    public delegate void DelegateIscoPumpDisconnected();
}
