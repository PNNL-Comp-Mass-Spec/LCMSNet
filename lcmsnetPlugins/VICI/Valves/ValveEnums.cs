using System.ComponentModel;

namespace LcmsNetPlugins.VICI.Valves
{
    /// <summary>
    /// state enum for a four position multiposition valve, for example a 9-port valve with a 4-position isolated continuous flow plate
    /// </summary>
    public enum FourPositionState
    {
        [Description("1")] P1 = 1,
        [Description("2")] P2 = 2,
        [Description("3")] P3 = 3,
        [Description("4")] P4 = 4,
        [Description("Unknown")] Unknown = -1
    }

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
    }

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
    }

    /// <summary>
    /// state enum for 15-position valves
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
    }

    /// <summary>
    /// state enum for a sixteen position multiposition valve, for example a 33-port valve with a 16-position isolated continuous flow plate
    /// </summary>
    public enum SixteenPositionState
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
        [Description("16")] P16 = 16,
        [Description("Unknown")] Unknown = -1
    }
}
