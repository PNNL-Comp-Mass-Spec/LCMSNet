namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Enum for ISCO pump problem reporting constants
    /// </summary>
    public enum IscoProblemStatus
    {
        None,
        CylinderEmpty,
        CylinderBottom,
        OverPressure,
        UnderPressure,
        MotorFailure,
        ComError,
        InitializationError,
        MessageParseError,
        DeviceNotInitialized
    }
}
