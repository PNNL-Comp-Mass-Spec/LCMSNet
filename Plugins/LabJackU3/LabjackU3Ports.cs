namespace LcmsNetPlugins.LabJackU3
{
    /// <summary>
    /// All available ports on the Labjack U3
    /// FIO and EIO ports can do Analog Input or Digital I/O.
    /// CIO ports can do only Digital I/O,
    /// /// </summary>
    public enum LabjackU3Ports
    {
        // FIO pins on the front of the U3, digital mode
        FIO0Digital = 0,
        FIO1Digital = 1,
        FIO2Digital = 2,
        FIO3Digital = 3,
        FIO4Digital = 4,
        FIO5Digital = 5,
        FIO6Digital = 6,
        FIO7Digital = 7,
        // EIO pins in the U3 DB-15 connector, digital mode
        EIO0Digital = 8,
        EIO1Digital = 9,
        EIO2Digital = 10,
        EIO3Digital = 11,
        EIO4Digital = 12,
        EIO5Digital = 13,
        EIO6Digital = 14,
        EIO7Digital = 15,
        // Digital I/O pins in the U3 DB-15 connector
        CIO0Digital = 16,
        CIO1Digital = 17,
        CIO2Digital = 18,
        CIO3Digital = 19,
        // FIO pins on the front of the U3, analog mode
        FIO0Analog = 20,
        FIO1Analog = 21,
        FIO2Analog = 22,
        FIO3Analog = 23,
        FIO4Analog = 24,
        FIO5Analog = 25,
        FIO6Analog = 26,
        FIO7Analog = 27,
        // EIO pins in the U3 DB-15 connector, analog mode
        EIO0Analog = 28,
        EIO1Analog = 29,
        EIO2Analog = 30,
        EIO3Analog = 31,
        EIO4Analog = 32,
        EIO5Analog = 33,
        EIO6Analog = 34,
        EIO7Analog = 35,
        // analog outputs 0 and 1(DAC0, DAC1)
        DAC0Analog = 36,
        DAC1Analog = 37,
    }

    /// <summary>
    /// All input ports on the Labjack U3
    /// </summary>
    public enum LabjackU3InputPorts
    {
        // FIO pins on the front of the U3, digital mode
        FIO0Digital = 0,
        FIO1Digital = 1,
        FIO2Digital = 2,
        FIO3Digital = 3,
        FIO4Digital = 4,
        FIO5Digital = 5,
        FIO6Digital = 6,
        FIO7Digital = 7,
        // EIO pins in the U3 DB-15 connector, digital mode
        EIO0Digital = 8,
        EIO1Digital = 9,
        EIO2Digital = 10,
        EIO3Digital = 11,
        EIO4Digital = 12,
        EIO5Digital = 13,
        EIO6Digital = 14,
        EIO7Digital = 15,
        // Digital I/O pins in the U3 DB-15 connector
        CIO0Digital = 16,
        CIO1Digital = 17,
        CIO2Digital = 18,
        CIO3Digital = 19,
        // FIO pins on the front of the U3, analog mode
        FIO0Analog = 20,
        FIO1Analog = 21,
        FIO2Analog = 22,
        FIO3Analog = 23,
        FIO4Analog = 24,
        FIO5Analog = 25,
        FIO6Analog = 26,
        FIO7Analog = 27,
        // EIO pins in the U3 DB-15 connector, analog mode
        EIO0Analog = 28,
        EIO1Analog = 29,
        EIO2Analog = 30,
        EIO3Analog = 31,
        EIO4Analog = 32,
        EIO5Analog = 33,
        EIO6Analog = 34,
        EIO7Analog = 35,
    }

    /// <summary>
    /// All output ports on the Labjack U3
    /// </summary>
    public enum LabjackU3OutputPorts
    {
        // FIO pins on the front of the U3, digital mode
        FIO0Digital = 0,
        FIO1Digital = 1,
        FIO2Digital = 2,
        FIO3Digital = 3,
        FIO4Digital = 4,
        FIO5Digital = 5,
        FIO6Digital = 6,
        FIO7Digital = 7,
        // EIO pins in the U3 DB-15 connector, digital mode
        EIO0Digital = 8,
        EIO1Digital = 9,
        EIO2Digital = 10,
        EIO3Digital = 11,
        EIO4Digital = 12,
        EIO5Digital = 13,
        EIO6Digital = 14,
        EIO7Digital = 15,
        // Digital I/O pins in the U3 DB-15 connector
        CIO0Digital = 16,
        CIO1Digital = 17,
        CIO2Digital = 18,
        CIO3Digital = 19,
        // analog outputs 0 and 1(DAC0, DAC1)
        DAC0Analog = 20,
        DAC1Analog = 21,
    }
}
