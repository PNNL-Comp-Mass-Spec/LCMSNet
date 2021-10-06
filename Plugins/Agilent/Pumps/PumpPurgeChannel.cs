namespace LcmsNetPlugins.Agilent.Pumps
{
    /// <summary>
    /// Channels available for purging. Set them to flag-like values, for facilitating use when sending commands to the pump
    /// </summary>
    public enum PumpPurgeChannel
    {
        A1 = 0b0001,
        A2 = 0b0010,
        B1 = 0b0100,
        B2 = 0b1000,
    }
}