using System.ComponentModel;

namespace FluidicsSDK.Base
{
    /// <summary>
    /// state enum for two position valves
    /// </summary>
    public enum TwoPositionState
    {
        [Description("A")] PositionA = 0,
        [Description("B")] PositionB = 1,
        [Description("Unknown")] Unknown = -1
    };
}
