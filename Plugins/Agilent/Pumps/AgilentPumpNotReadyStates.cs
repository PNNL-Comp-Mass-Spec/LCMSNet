using System;

namespace LcmsNetPlugins.Agilent.Pumps
{
    [Flags]
    public enum AgilentPumpNotReadyStates
    {
        Ready = 0,
        // Generic
        Startup =            1 << 0, // 0x00000001,
        Reset =              1 << 1, // 0x00000002,
        Leak =               1 << 2, // 0x00000004,
        Shutdown =           1 << 3, // 0x00000008,
        Cover_Open =         1 << 4, // 0x00000010,
        Wait_Remote =        1 << 5, // 0x00000020,
        Wait_Controller =    1 << 6, // 0x00000040,
        Aborting_Active =    1 << 7, // 0x00000080,

        // Pump-specific
        Init =               1 << 8, // 0x00000100,
        Sync_Piston =        1 << 9, // 0x00000200,
        Drive_Off =          1 << 10, // 0x00000400,
        Wait_Piston =        1 << 11, // 0x00000800,
        Wait_Volume =        1 << 12, // 0x00001000,
        Wait_Pressure =      1 << 13, // 0x00002000,
        Bottle_A_Not_Ready = 1 << 14, // 0x00004000,
        Bottle_B_Not_Ready = 1 << 15, // 0x00008000,
        Bottle_C_Not_Ready = 1 << 16, // 0x00010000,
        Bottle_D_Not_Ready = 1 << 17, // 0x00020000,
        Column_Flow =        1 << 18, // 0x00040000,
        Purge_A1 =           1 << 19, // 0x00080000,
        Purge_A2 =           1 << 20, // 0x00100000,
        Purge_B1 =           1 << 21, // 0x00200000,
        Purge_B2 =           1 << 22, // 0x00400000,
        Purge =              1 << 23, // 0x00800000,
        Comp_Change =        1 << 24, // 0x01000000,
        Mode =               1 << 25, // 0x02000000,
        Flow_Init =          1 << 26, // 0x04000000,
        Elasticity_Tuning =  1 << 27, // 0x08000000,
        Compression_Tuning = 1 << 28, // 0x10000000,
        Flow_Reduction =     1 << 29, // 0x20000000,
    }

    [Flags]
    public enum AgilentPumpStartNotReadyStates
    {
        Ready = 0,
        // Generic
        Startup = 1 << 0,                // 0x00000001,
        Reserved1 = 1 << 1,              // 0x00000002,
        Leak = 1 << 2,                   // 0x00000004,
        Shutdown = 1 << 3,               // 0x00000008,
        Abort = 1 << 4,                  // 0x00000010,
        Reserved2 = 1 << 5,              // 0x00000020,
        Reserved3 = 1 << 6,              // 0x00000040,
        No_Fan_Rotation = 1 << 7,        // 0x00000080,
        Incompatible_Firmware = 1 << 8,  // 0x00000100,
        Error_In_Loading_FPGA = 1 << 9,  // 0x00000200,

        // Pump-specific
        Drive_Off = 1 << 10,             // 0x00000400,
        Drive_Init = 1 << 11,            // 0x00000800,
    }
}
