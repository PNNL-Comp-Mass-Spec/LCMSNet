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
        // ReSharper restore InconsistentNaming
    }

    // TEMPERATURE_AIR_K 60050 FLOAT32
    // TEMPERATURE_DEVICE_K 60052 FLOAT32

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
