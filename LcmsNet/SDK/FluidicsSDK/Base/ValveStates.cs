using System;
using System.ComponentModel;
using System.Linq;

namespace FluidicsSDK.Base
{
    /// <summary>
    /// state enum for sixteen port multiposition valves
    /// </summary>
    public enum FifteenPositionState
    {
        [Description("1")] P1 = 1,
        [Description("2")] P2 = 2,
        [Description("3")] P3 = 3,
        [Description("4")] P4 = 4,
        [Description("5")] P5 = 5,
        [Description("6")] P6 = 6,
        [Description("7")] P7 = 7,
        [Description("8")] P8 = 8,
        [Description("9")] P9 = 9,
        [Description("10")] P10 = 10,
        [Description("11")] P11 = 11,
        [Description("12")] P12 = 12,
        [Description("13")] P13 = 13,
        [Description("14")] P14 = 14,
        [Description("15")] P15 = 15,
        [Description("Unknown")] Unknown = -1
    };


    public enum EightPositionState
    {
        [Description("1")] P1 = 1,
        [Description("2")] P2 = 2,
        [Description("3")] P3 = 3,
        [Description("4")] P4 = 4,
        [Description("5")] P5 = 5,
        [Description("6")] P6 = 6,
        [Description("7")] P7 = 7,
        [Description("8")] P8 = 8,
        [Description("Unknown")] Unknown = -1
    };

    public enum TenPositionState
    {
        [Description("1")] P1 = 1,
        [Description("2")] P2 = 2,
        [Description("3")] P3 = 3,
        [Description("4")] P4 = 4,
        [Description("5")] P5 = 5,
        [Description("6")] P6 = 6,
        [Description("7")] P7 = 7,
        [Description("8")] P8 = 8,
        [Description("9")] P9 = 9,
        [Description("10")] P10 = 10,
        [Description("Unknown")] Unknown = -1
    };

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
