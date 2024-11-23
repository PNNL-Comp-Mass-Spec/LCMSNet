namespace LabJackTSeries
{
    // Address information pulled from:
    // https://support.labjack.com/docs/3-1-2-printable-modbus-map
    // https://support.labjack.com/docs/3-1-modbus-map-t-series-datasheet
    // https://support.labjack.com/docs/13-0-digital-i-o-t-series-datasheet
    // https://support.labjack.com/docs/14-0-analog-inputs-t-series-datasheet
    // https://support.labjack.com/docs/15-0-dac-t-series-datasheet
    // https://support.labjack.com/docs/4-0-hardware-overview-t-series-datasheet

    /// <summary>
    /// Available I/O names and addresses on the LabJack T7. Not a comprehensive list.
    /// </summary>
    public enum LabJackT7IONames
    {
        // IMPORTANT: This enum needs to duplicate the values of other enums for this device!
        // ReSharper disable InconsistentNaming
        AIN0 = 0,
        AIN1 = 2,
        AIN2 = 4,
        AIN3 = 6,
        AIN4 = 8,
        AIN5 = 10,
        AIN6 = 12,
        AIN7 = 14,
        AIN8 = 16,
        AIN9 = 18,
        AIN10 = 20,
        AIN11 = 22,
        AIN12 = 24,
        AIN13 = 26,
        /// <summary>
        /// Internal temperature sensor, analog value
        /// </summary>
        AIN14 = 28,
        /// <summary>
        /// Internal reference ground
        /// </summary>
        AIN15 = 30,
        DAC0 = 1000,
        DAC1 = 1002,
        FIO0 = 2000,
        FIO1 = 2001,
        FIO2 = 2002,
        FIO3 = 2003,
        FIO4 = 2004,
        FIO5 = 2005,
        FIO6 = 2006,
        FIO7 = 2007,
        EIO0 = 2008,
        EIO1 = 2009,
        EIO2 = 2010,
        EIO3 = 2011,
        EIO4 = 2012,
        EIO5 = 2013,
        EIO6 = 2014,
        EIO7 = 2015,
        CIO0 = 2016,
        CIO1 = 2017,
        CIO2 = 2018,
        CIO3 = 2019,
        MIO0 = 2020,
        MIO1 = 2021,
        MIO2 = 2022,
        PRODUCT_ID = 60000,         // FLOAT32, R
        HARDWARE_VERSION = 60002,   // FLOAT32, R
        FIRMWARE_VERSION = 60004,   // FLOAT32, R
        BOOTLOADER_VERSION = 60006, // FLOAT32, R
        WIFI_VERSION = 60008,       // FLOAT32, R
        HARDWARE_INSTALLED = 60010, // UINT32, R
        ETHERNET_MAC = 60020,       // UINT64, R
        WIFI_MAC = 60024,           // UINT64, R
        SERIAL_NUMBER = 60028,      // UINT32, R
        DEVICE_NAME_DEFAULT = 60500, // STRING, R/W, up to 49 characters, no periods, requires reboot to update reported name

        AIN0_EF_READ_A = 7000,
        AIN1_EF_READ_A = 7002,
        AIN2_EF_READ_A = 7004,
        AIN3_EF_READ_A = 7006,
        AIN4_EF_READ_A = 7008,
        AIN5_EF_READ_A = 7010,
        AIN6_EF_READ_A = 7012,
        AIN7_EF_READ_A = 7014,
        AIN8_EF_READ_A = 7016,
        AIN9_EF_READ_A = 7018,
        AIN10_EF_READ_A = 7020,
        AIN11_EF_READ_A = 7022,
        AIN12_EF_READ_A = 7024,
        AIN13_EF_READ_A = 7026,
        AIN0_EF_READ_B = 7300,
        AIN1_EF_READ_B = 7302,
        AIN2_EF_READ_B = 7304,
        AIN3_EF_READ_B = 7306,
        AIN4_EF_READ_B = 7308,
        AIN5_EF_READ_B = 7310,
        AIN6_EF_READ_B = 7312,
        AIN7_EF_READ_B = 7314,
        AIN8_EF_READ_B = 7316,
        AIN9_EF_READ_B = 7318,
        AIN10_EF_READ_B = 7320,
        AIN11_EF_READ_B = 7322,
        AIN12_EF_READ_B = 7324,
        AIN13_EF_READ_B = 7326,
        AIN0_EF_READ_C = 7600,
        AIN1_EF_READ_C = 7602,
        AIN2_EF_READ_C = 7604,
        AIN3_EF_READ_C = 7606,
        AIN4_EF_READ_C = 7608,
        AIN5_EF_READ_C = 7610,
        AIN6_EF_READ_C = 7612,
        AIN7_EF_READ_C = 7614,
        AIN8_EF_READ_C = 7616,
        AIN9_EF_READ_C = 7618,
        AIN10_EF_READ_C = 7620,
        AIN11_EF_READ_C = 7622,
        AIN12_EF_READ_C = 7624,
        AIN13_EF_READ_C = 7626,
        AIN0_EF_READ_D = 7900,
        AIN1_EF_READ_D = 7902,
        AIN2_EF_READ_D = 7904,
        AIN3_EF_READ_D = 7906,
        AIN4_EF_READ_D = 7908,
        AIN5_EF_READ_D = 7910,
        AIN6_EF_READ_D = 7912,
        AIN7_EF_READ_D = 7914,
        AIN8_EF_READ_D = 7916,
        AIN9_EF_READ_D = 7918,
        AIN10_EF_READ_D = 7920,
        AIN11_EF_READ_D = 7922,
        AIN12_EF_READ_D = 7924,
        AIN13_EF_READ_D = 7926,
        TEMPERATURE_AIR_K = 60050,    // FLOAT32, R
        TEMPERATURE_DEVICE_K = 60052, // FLOAT32, R
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Input names and addresses on the LabJack T7. Not a comprehensive list.
    /// </summary>
    public enum LabJackT7Inputs
    {
        // ReSharper disable InconsistentNaming
        AIN0 = 0,
        AIN1 = 2,
        AIN2 = 4,
        AIN3 = 6,
        AIN4 = 8,
        AIN5 = 10,
        AIN6 = 12,
        AIN7 = 14,
        AIN8 = 16,
        AIN9 = 18,
        AIN10 = 20,
        AIN11 = 22,
        AIN12 = 24,
        AIN13 = 26,
        /// <summary>
        /// Internal temperature sensor, analog value
        /// </summary>
        AIN14 = 28,
        /// <summary>
        /// Internal reference ground
        /// </summary>
        AIN15 = 30,
        FIO0 = 2000,
        FIO1 = 2001,
        FIO2 = 2002,
        FIO3 = 2003,
        FIO4 = 2004,
        FIO5 = 2005,
        FIO6 = 2006,
        FIO7 = 2007,
        EIO0 = 2008,
        EIO1 = 2009,
        EIO2 = 2010,
        EIO3 = 2011,
        EIO4 = 2012,
        EIO5 = 2013,
        EIO6 = 2014,
        EIO7 = 2015,
        CIO0 = 2016,
        CIO1 = 2017,
        CIO2 = 2018,
        CIO3 = 2019,
        MIO0 = 2020,
        MIO1 = 2021,
        MIO2 = 2022
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Analog Input extended feature names and addresses on the LabJack T7. Not a comprehensive list.
    /// </summary>
    public enum LabJackT7ExtendFeatureAnalogInputs
    {
        // ReSharper disable InconsistentNaming

        AIN0_EF_READ_A = 7000,
        AIN1_EF_READ_A = 7002,
        AIN2_EF_READ_A = 7004,
        AIN3_EF_READ_A = 7006,
        AIN4_EF_READ_A = 7008,
        AIN5_EF_READ_A = 7010,
        AIN6_EF_READ_A = 7012,
        AIN7_EF_READ_A = 7014,
        AIN8_EF_READ_A = 7016,
        AIN9_EF_READ_A = 7018,
        AIN10_EF_READ_A = 7020,
        AIN11_EF_READ_A = 7022,
        AIN12_EF_READ_A = 7024,
        AIN13_EF_READ_A = 7026,
        AIN0_EF_READ_B = 7300,
        AIN1_EF_READ_B = 7302,
        AIN2_EF_READ_B = 7304,
        AIN3_EF_READ_B = 7306,
        AIN4_EF_READ_B = 7308,
        AIN5_EF_READ_B = 7310,
        AIN6_EF_READ_B = 7312,
        AIN7_EF_READ_B = 7314,
        AIN8_EF_READ_B = 7316,
        AIN9_EF_READ_B = 7318,
        AIN10_EF_READ_B = 7320,
        AIN11_EF_READ_B = 7322,
        AIN12_EF_READ_B = 7324,
        AIN13_EF_READ_B = 7326,
        AIN0_EF_READ_C = 7600,
        AIN1_EF_READ_C = 7602,
        AIN2_EF_READ_C = 7604,
        AIN3_EF_READ_C = 7606,
        AIN4_EF_READ_C = 7608,
        AIN5_EF_READ_C = 7610,
        AIN6_EF_READ_C = 7612,
        AIN7_EF_READ_C = 7614,
        AIN8_EF_READ_C = 7616,
        AIN9_EF_READ_C = 7618,
        AIN10_EF_READ_C = 7620,
        AIN11_EF_READ_C = 7622,
        AIN12_EF_READ_C = 7624,
        AIN13_EF_READ_C = 7626,
        AIN0_EF_READ_D = 7900,
        AIN1_EF_READ_D = 7902,
        AIN2_EF_READ_D = 7904,
        AIN3_EF_READ_D = 7906,
        AIN4_EF_READ_D = 7908,
        AIN5_EF_READ_D = 7910,
        AIN6_EF_READ_D = 7912,
        AIN7_EF_READ_D = 7914,
        AIN8_EF_READ_D = 7916,
        AIN9_EF_READ_D = 7918,
        AIN10_EF_READ_D = 7920,
        AIN11_EF_READ_D = 7922,
        AIN12_EF_READ_D = 7924,
        AIN13_EF_READ_D = 7926,
        TEMPERATURE_AIR_K = 60050,    // FLOAT32, R
        TEMPERATURE_DEVICE_K = 60052, // FLOAT32, R
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Output names and addresses on the LabJack T7. Not a comprehensive list.
    /// </summary>
    public enum LabJackT7Outputs
    {
        // ReSharper disable InconsistentNaming
        DAC0 = 1000,
        DAC1 = 1002,
        FIO0 = 2000,
        FIO1 = 2001,
        FIO2 = 2002,
        FIO3 = 2003,
        FIO4 = 2004,
        FIO5 = 2005,
        FIO6 = 2006,
        FIO7 = 2007,
        EIO0 = 2008,
        EIO1 = 2009,
        EIO2 = 2010,
        EIO3 = 2011,
        EIO4 = 2012,
        EIO5 = 2013,
        EIO6 = 2014,
        EIO7 = 2015,
        CIO0 = 2016,
        CIO1 = 2017,
        CIO2 = 2018,
        CIO3 = 2019,
        MIO0 = 2020,
        MIO1 = 2021,
        MIO2 = 2022
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Digital IO extended feature names and addresses on the LabJack T7. Not a comprehensive list.
    /// </summary>
    public enum LabJackTClockDivisor
    {
        // ReSharper disable InconsistentNaming
        TwoTo0 = 1,
        TwoTo1 = 2,
        TwoTo2 = 4,
        TwoTo3 = 8,
        TwoTo4 = 16,
        TwoTo5 = 32,
        TwoTo6 = 64,
        TwoTo8 = 256,
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Auxiliary registers. Not a comprehensive list.
    /// </summary>
    public enum LabJackTGeneralRegisters
    {
        // ReSharper disable InconsistentNaming
        PRODUCT_ID = 60000,         // FLOAT32, R
        HARDWARE_VERSION = 60002,   // FLOAT32, R
        FIRMWARE_VERSION = 60004,   // FLOAT32, R
        BOOTLOADER_VERSION = 60006, // FLOAT32, R
        WIFI_VERSION = 60008,       // FLOAT32, R
        HARDWARE_INSTALLED = 60010, // UINT32, R
        ETHERNET_MAC = 60020,       // UINT64, R
        WIFI_MAC = 60024,           // UINT64, R
        SERIAL_NUMBER = 60028,      // UINT32, R
        DEVICE_NAME_DEFAULT = 60500, // STRING, R/W, up to 49 characters, no periods, requires reboot to update reported name
        // ReSharper restore InconsistentNaming
    }
}
