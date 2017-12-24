using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace LabJack
{
    public class LabJackU12Wrapper
    {
        static LabJackU12Wrapper()
        {
            // Pre-load the correct ljackuw.dll dll, according to current execution architecture...
            var myPath = new Uri(typeof(LabJackU12Wrapper).Assembly.CodeBase).LocalPath;
            var myFolder = Path.GetDirectoryName(myPath);
            if (string.IsNullOrWhiteSpace(myFolder))
            {
                myFolder = ".";
            }

            var subfolder = Environment.Is64BitProcess ? "x64" : "x86";

            LoadLibrary(Path.Combine(myFolder, subfolder, "ljackuw.dll"));
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        /// <summary>
        /// Converts a LabJack errorcode, returned by another function, into a string describing the error. No hardware communication is involved.
        /// </summary>
        /// <param name="errorCode">LabJack errorcode</param>
        /// <param name="errorString">Sequence of characters describing the error</param>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern void GetErrorString(int errorCode, StringBuilder errorString);

        /// <summary>
        /// Searches the USB for all LabJacks, and returns the serial number and local ID for each.
        /// </summary>
        /// <param name="productIDList">Pointer to a 127 element array. Send filled with zeros.</param>
        /// <param name="serialNumList">Pointer to a 127 element array. Send filled with zeros. Returned filled with serial numbers, and unused locations are filled with 9999.0.</param>
        /// <param name="localIDList">Pointer to a 127 element array. Send filled with zeros. Returned filled with local ID numbers, and unused locations are filled with 9999.0.</param>
        /// <param name="powerList">Pointer to a 127 element array. Send filled with zeros.</param>
        /// <param name="calMatrix">Pointer to a 127 by 20 element array. Send filled with zeros.</param>
        /// <param name="numFound">Number of LabJacks found on the USB.</param>
        /// <param name="reserved1">reserved</param>
        /// <param name="reserved2">reserved</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int ListAll([In, Out] int[] productIDList, [In, Out] int[] serialNumList, [In, Out] int[] localIDList, [In, Out] int[] powerList, [In, Out] int[,] calMatrix, ref int numFound, ref int reserved1, ref int reserved2);

        /// <summary>
        /// Easy function. This is a simplified version of AISample. Reads the voltage from 1 analog input. Calling this function turns/leaves the status LED on.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="channel">Channel command is 0-7 for single-ended, or 8-11 for differential.</param>
        /// <param name="gain">Gain command is 0=1, 1=2, …, 7=20. This amplification is only available for differential channels.</param>
        /// <param name="overVoltage">If &gt;0, an overvoltage has been detected on one of the selected analog inputs.</param>
        /// <param name="voltage">Returns the voltage reading.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less(typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int EAnalogIn(ref int idnum, int demo, int channel, int gain, ref int overVoltage, ref float voltage);

        /// <summary>
        /// Easy function. This is a simplified version of AOUpdate. Sets the voltage of both analog outputs.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="analogOut0">Voltage from 0.0 to 5.0 for AO0.</param>
        /// <param name="analogOut1">Voltage from 0.0 to 5.0 for AO1.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        /// <remarks>
        /// If either passed voltage is less than zero, the DLL uses the last set voltage.
        /// This provides a way to update 1 output without changing the other. Note that when the DLL is first loaded,
        /// it does not know if the analog outputs have been set, and assumes they are both the default of 0.0 volts.
        /// Similarly, there are situations where the LabJack could reset without the knowledge of the DLL, and thus
        /// the DLL could think the analog outputs are set to a non-zero voltage when in fact they have been reinitialized to 0.0 volts.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int EAnalogOut(ref int idnum, int demo, float analogOut0, float analogOut1);

        /// <summary>
        /// Easy function. This is a simplified version of Counter. Reads &amp; resets the counter (CNT). Calling this function disables STB (which is the default anyway).
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="resetCounter">If &gt;0, the counter is reset to zero after being read.</param>
        /// <param name="count">Current count, before reset.</param>
        /// <param name="ms">Value of Window’s millisecond timer at the time of the counter read (within a few ms). Note that the millisecond timer rolls over about every 50 days. In general, the millisecond timer starts counting from zero whenever the computer reboots.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int ECount(ref int idnum, int demo, int resetCounter, ref double count, ref double ms);

        /// <summary>
        /// Easy function. This is a simplified version of DigitalIO that reads the state of one digital input. Also configures the requested pin to input and leaves it that way.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="channel">Line to read. 0-3 for IO or 0-15 for D.</param>
        /// <param name="readD">If &gt;0, a D line is read as opposed to an IO line.</param>
        /// <param name="state">The selected line is TRUE/Set if &gt;0. FALSE/Clear if 0.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        /// <remarks>
        /// Note that this is a simplified version of the lower level function DigitalIO, which operates on all 20 digital lines.
        /// The DLL (ljackuw) attempts to keep track of the current direction and output state of all lines, so that this easy function
        /// can operate on a single line without changing the others. When the DLL is first loaded, though, it does not know the
        /// direction and state of the lines and assumes all directions are input and output states are low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int EDigitalIn(ref int idnum, int demo, int channel, int readD, ref int state);

        /// <summary>
        /// Easy function. This is a simplified version of DigitalIO that sets/clears the state of one digital output. Also configures the requested pin to output and leaves it that way.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="channel">Line to read. 0-3 for IO or 0-15 for D.</param>
        /// <param name="writeD">If &gt;0, a D line is written as opposed to an IO line.</param>
        /// <param name="state">If &gt;0, the line is set, otherwise the line is cleared.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        /// <remarks>
        /// Note that this is a simplified version of the lower level function DigitalIO, which operates on all 20 digital lines.
        /// The DLL (ljackuw) attempts to keep track of the current direction and output state of all lines, so that this easy function
        /// can operate on a single line without changing the others. When the DLL is first loaded, though, it does not know the
        /// direction and state of the lines and assumes all directions are input and output states are low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int EDigitalOut(ref int idnum, int demo, int channel, int writeD, int state);

        /// <summary>
        /// Requires firmware V1.1 or higher. This function writes to the asynch registers and sets the direction of the D lines (input/output) as needed.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="timeoutMult">If enabled, read timeout is about 100 milliseconds times this value (0-255).</param>
        /// <param name="configA">If &gt;0, D8 is set to output-high and D9 is set to input.</param>
        /// <param name="configB">If &gt;0, D10 is set to output-high and D11 is set to input.</param>
        /// <param name="configTE">If &gt;0, D12 is set to output-low.</param>
        /// <param name="fullA">Time constants for a “full” delay (1-255).</param>
        /// <param name="fullB">Time constants for a “full” delay (1-255).</param>
        /// <param name="fullC">Time constants for a “full” delay (1-255).</param>
        /// <param name="halfA">Time constants for a “half” delay (1-255).</param>
        /// <param name="halfB">Time constants for a “half” delay (1-255).</param>
        /// <param name="halfC">Time constants for a “half” delay (1-255).</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 60 milliseconds or less (typically 48 milliseconds in Windows).</remarks>
        /// <remarks>
        /// The actual 1-bit time is about 1.833 plus a "full" delay (us).
        /// The actual 1/2-bit time is about 1.0 plus a "half" delay (us).
        /// full/half delay = 0.833 + 0.833C + 0.667BC + 0.5ABC (us)
        /// Common baud rates (full A,B,C; half A,B,C):
        ///     1        55,153,232 ; 114,255,34
        ///     10       63,111,28 ; 34,123,23
        ///     100      51,191,2 ; 33,97,3
        ///     300      71,23,4 ; 84,39,1
        ///     600      183,3,6 ; 236,7,1
        ///     1000     33,29,2 ; 123,8,1
        ///     1200     23,17,4 ; 14,54,1
        ///     2400     21,37,1 ; 44,3,3
        ///     4800     10,18,2 ; 1,87,1
        ///     7200     134,2,1 ; 6,9,2
        ///     9600     200,1,1 ; 48,2,1
        ///     10000    63,3,1 ; 46,2,1
        ///     19200    96,1,1 ; 22,2,1
        ///     38400    3,5,2 ; 9,2,1
        ///     57600    3,3,2 ; 11,1,1
        ///     100000   3,3,1 ; 1,2,1
        ///     115200   9,1,1 ; 2,1,1 or 1,1,1
        /// When using data rates over 38.4 kbps, the following conditions need to be considered:• When reading the first byte, the start bit is first tested about 11.5 us after the start of the tx stop bit.
        /// • When reading bytes after the first, the start bit is first tested about "full" + 11 us after the previous bit 8 read, which occurs near the middle of bit 8.
        /// When enabled, STB does the following to aid in debugging asynchronous reads:
        /// • STB is set about 6 us after the start of the last tx stop bit, or about "full" + 6 us after the previous bit 8 read.
        /// • STB is cleared about 0-2 us after the rx start bit is detected.
        /// • STB is set after about "half".
        /// • STB is cleared after about "full".
        /// • Bit 0 is read about 1 us later.
        /// • STB is set about 1 us after the bit 0 read.
        /// • STB is cleared after about "full".
        /// • Bit 1 is read about 1 us later.
        /// • STB is set about 1 us after the bit 1 read.
        /// • STB is cleared after about "full".
        /// • This continues for all 8 data bits and the stop bit, after which STB remains low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AsynchConfig(ref int idnum, int demo, int timeoutMult, int configA, int configB, int configTE, int fullA, int fullB, int fullC, int halfA, int halfB, int halfC);

        /// <summary>
        /// Requires firmware V1.1 or higher. This function writes and then reads half-duplex asynchronous data on 1 of two pairs of D lines (8,n,1). Call AsynchConfig to set the baud rate. Similar to RS232, except that logic is normal CMOS/TTL (0=low=GND, 1=high=+5V, idle state of transmit line is high). Connection to a normal RS232 device will probably require a converter chip such as the MAX233.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="portB">If &gt;0, asynch PortB is used instead of PortA.</param>
        /// <param name="enableTE">If &gt;0, D12 (Transmit Enable) is set high during transmit and low during receive.</param>
        /// <param name="enableTO">If &gt;0, timeout is enabled for the receive phase (per byte).</param>
        /// <param name="enableDel">If &gt;0, a 1 bit delay is inserted between each transmit byte.</param>
        /// <param name="baudrate">This is the bps as set by AsynchConfig. Asynch needs this so it has an idea how long the transfer should take.</param>
        /// <param name="numWrite">Number of bytes to write (0-18).</param>
        /// <param name="numRead">Number of bytes to read (0-18).</param>
        /// <param name="data">Serial data buffer. Send an 18 element array. Fill unused locations with zeros. Returns any serial read data, with unused locations filled with 9999s.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is about 20 milliseconds to write and/or read up to 4 bytes, plus about 20 milliseconds for each additional 4 bytes written or read. Slow baud rates can result in longer execution time.</remarks>
        /// <remarks>
        /// PortA =&gt; TX is D8 and RX is D9
        /// PortB =&gt; TX is D10 and RX is D11
        /// Transmit Enable is D12
        /// Up to 18 bytes can be written and read. If more than 4 bytes are written or read, this function uses calls to WriteMem/ReadMem to load/read the LabJack's data buffer.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int Asynch(ref int idnum, int demo, int portB, int enableTE, int enableTO, int enableDel, int baudrate, int numWrite, int numRead, [In, Out] int[] data);

        /// <summary>
        /// Reads the voltages from 1,2, or 4 analog inputs. Also controls/reads the 4 IO ports.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="stateIO">Output states for IO0-IO3. Has no effect if IO are configured as inputs, so a different function must be used to configure as output. Returns input states of IO0-IO3.</param>
        /// <param name="updateIO">If &gt;0, state values will be written. Otherwise, just a read is performed.</param>
        /// <param name="ledOn">If &gt;0, the LabJack LED is turned on.</param>
        /// <param name="numChannels">Number of analog input channels to read (1,2, or 4).</param>
        /// <param name="channels">Pointer to an array of channel commands with at least numChannels elements. Each channel command is 0-7 for single-ended, or 8-11 for differential.</param>
        /// <param name="gains">Pointer to an array of gain commands with at least numChannels elements. Gain commands are 0=1, 1=2, …, 7=20. This amplification is only available for differential channels.</param>
        /// <param name="disableCal">If &gt;0, voltages returned will be raw readings that are not corrected using calibration constants.</param>
        /// <param name="overVoltage">If &gt;0, an overvoltage has been detected on one of the selected analog inputs.</param>
        /// <param name="voltages">Pointer to an array where voltage readings are returned. Send a 4-element array of zeros. Returns a pointer to an array with numChannels voltage readings.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AISample(ref int idnum, int demo, ref int stateIO, int updateIO, int ledOn, int numChannels, [In, Out] int[] channels, [In, Out] int[] gains, int disableCal, ref int overVoltage, [In, Out] float[] voltages);

        /// <summary>
        /// Reads a specified number of scans (up to 4096) at a specified scan rate (up to 8192 Hz) from 1,2, or 4 analog inputs. First, data is acquired and stored in the LabJack’s 4096 sample RAM buffer. Then, the data is transferred to the PC.
        /// If the LED is enabled (ledOn&gt;0), it will blink at about 4 Hz while waiting for a trigger, turn off during acquisition, blink rapidly while transferring data to the PC, and turn on when done.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="stateIOin">Output states for IO0-IO3. Has no effect if IO are configured as inputs, so a different function must be used to configure as output.</param>
        /// <param name="updateIO">If &gt;0, state values will be written. Otherwise, just a read is performed.</param>
        /// <param name="ledOn">If &gt;0, the LabJack LED is turned on.</param>
        /// <param name="numChannels">Number of analog input channels to read (1,2, or 4).</param>
        /// <param name="channels">Pointer to an array of channel commands with at least numChannels elements. Each channel command is 0-7 for single-ended, or 8-11 for differential.</param>
        /// <param name="gains">Pointer to an array of gain commands with at least numChannels elements. Gain commands are 0=1, 1=2, …, 7=20. This amplification is only available for differential channels.</param>
        /// <param name="scanRate">Scans acquired per second. A scan is a reading from every channel (1,2, or 4). The sample rate (scanRate * numChannels) must be between 400 and 8192. Returns the actual scan rate, which due to clock resolution is not always exactly the same as the desired scan rate.</param>
        /// <param name="disableCal">If &gt;0, voltages returned will be raw readings that are not corrected using calibration constants.</param>
        /// <param name="triggerIO">The IO port to trigger on (0=none, 1=IO0, or 2=IO1).</param>
        /// <param name="triggerState">If &gt;0, the acquisition will be triggered when the selected IO port reads high.</param>
        /// <param name="numScans">Number of scans which will be returned. Minimum is 1. Maximum numSamples is 4096, where numSamples is numScans * numChannels.</param>
        /// <param name="timeout">This function will return immediately with a timeout error if it does not receive a scan within this number of seconds.</param>
        /// <param name="voltages">Pointer to a 4096 by 4 array where voltage readings are returned. Send filled with zeros. Returns voltage readings, with unused locations filled with 9999.0.</param>
        /// <param name="stateIOout">Pointer to a 4096 element array where IO states are returned. Send filled with zeros. Returns IO states, with unused locations filled with 9999.0.</param>
        /// <param name="overVoltage">If &gt;0, an overvoltage has been detected on at least one sample of one of the selected analog inputs.</param>
        /// <param name="transferMode">0=auto,1=normal, 2=turbo. If auto, turbo mode is used unless timeout is &gt;= 4, or numScans/scanRate &gt;=4.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>The execution time of this function, in milliseconds, depends on transfermode and can be estimated with the below formulas. The actual number of samples collected and transferred by the LabJack is the smallest power of 2 from 64 to 4096 which is at least as big as numScans*numChannels. This is represented below as numSamplesActual.</remarks>
        /// <remarks>
        /// Normal => 30+(1000*numSamplesActual/sampleRate)+(2.5*numSamplesActual)
        /// Turbo => 30+(1000*numSamplesActual/sampleRate)+(0.4*numSamplesActual)
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AIBurst(ref int idnum, int demo, int stateIOin, int updateIO, int ledOn, int numChannels, [In, Out] int[] channels, [In, Out] int[] gains, ref float scanRate, int disableCal, int triggerIO, int triggerState, int numScans, int timeout, [In, Out] float[,] voltages, [In, Out] int[] stateIOout, ref int overVoltage, int transferMode);

        /// <summary>
        /// Starts a hardware timed continuous acquisition where data is sampled and stored in the LabJack RAM buffer, and can be simultaneously transferred out of the RAM buffer to the PC application. A call to this function should be followed by periodic calls to AIStreamRead, and eventually a call to AIStreamClear. Note that while streaming the LabJack U12 is too busy to do anything else. If any function besides AIStreamRead is called while a stream is in progress, the stream will be stopped.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="stateIOin">Output states for IO0-IO3.</param>
        /// <param name="updateIO">If &gt;0, state values will be written. Otherwise, just a read is performed.</param>
        /// <param name="ledOn">If &gt;0, the LabJack LED is turned on.</param>
        /// <param name="numChannels">Number of analog input channels to read (1,2, or 4). If readCount is &gt;0, numChannels should be 4.</param>
        /// <param name="channels">Pointer to an array of channel commands with at least numChannels elements. Each channel command is 0-7 for single-ended, or 8-11 for differential.</param>
        /// <param name="gains">Pointer to an array of gain commands with at least numChannels elements. Gain commands are 0=1, 1=2, …, 7=20. This amplification is only available for differential channels.</param>
        /// <param name="scanRate">Scans acquired per second. A scan is a reading from every channel (1,2, or 4). The sample rate (scanRate * numChannels) must be between 200 and 1200. Returns the actual scan rate, which due to clock resolution is not always exactly the same as the desired scan rate.</param>
        /// <param name="disableCal">If &gt;0, voltages returned will be raw readings that are not corrected using calibration constants.</param>
        /// <param name="reserved1">Reserved for future use. Send 0.</param>
        /// <param name="readCount">If &gt;0, the current count (CNT) is returned instead of the 2nd, 3rd, and 4th analog input channels. 2nd channel is bits 0-11. 3rd channel is bits 12-23. 4th channel is bits 24-31. This feature was added to the LabJack U12 starting with firmware version 1.03, and this input has no effect with earlier firmware versions.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 30 milliseconds or less (typically 24 milliseconds in Windows).</remarks>
        /// <remarks>If the LED is enabled (ledOn&gt;0), it will toggle every 40 samples during acquisition and turn on when the stream operation stops.</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AIStreamStart(ref int idnum, int demo, int stateIOin, int updateIO, int ledOn, int numChannels, [In, Out] int[] channels, [In, Out] int[] gains, ref float scanRate, int disableCal, int reserved1, int readCount);

        /// <summary>
        /// Waits for a specified number of scans to be available and reads them. AIStreamStart should be called before this function and AIStreamClear should be called when finished with the stream.
        /// Note that while streaming the LabJack U12 is too busy to do anything else. If any function besides AIStreamRead is called while a stream is in progress, the stream will be stopped.
        /// </summary>
        /// <param name="localID">Send the local ID from AIStreamStart.</param>
        /// <param name="numScans">Function will wait until this number of scans is available. Minimum is 1. Maximum numSamples is 4096, where numSamples is numScans * numChannels. Internally this function gets data from the LabJack in blocks of 64 samples, so it is recommended that numSamples be at least 64.</param>
        /// <param name="timeout">Function timeout value in seconds.</param>
        /// <param name="voltages">Pointer to a 4096 by 4 array where voltage readings are returned. Send filled with zeros. Returns voltage readings, with unused locations filled with 9999.0.</param>
        /// <param name="stateIOout">Pointer to a 4096 element array where IO states are returned. Send filled with zeros. Returns IO states, with unused locations filled with 9999.0.</param>
        /// <param name="reserved">Reserved for future use. Send 0.</param>
        /// <param name="ljScanBacklog">Returns the scan backlog of the LabJack RAM buffer. The size of the buffer in terms of scans is 4096/numChannels.</param>
        /// <param name="overVoltage">If &gt;0, an overvoltage has been detected on at least one sample of one of the selected analog inputs.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AIStreamRead(int localID, int numScans, int timeout, [In, Out] float[,] voltages, [In, Out] int[] stateIOout, ref int reserved, ref int ljScanBacklog, ref int overVoltage);

        /// <summary>
        /// This function stops the continuous acquisition. It should be called once when finished with the stream. The sequence of calls for a typical stream operation is: AIStreamStart, AIStreamRead, AIStreamRead, AIStreamRead, …, AIStreamClear.
        /// </summary>
        /// <param name="localID">Send the local ID from AIStreamStart/Read.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AIStreamClear(int localID);

        /// <summary>
        /// Sets the voltages of the analog outputs. Also controls/reads all 20 digital I/O and the counter.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="trisD">Directions for D0-D15. 0=Input, 1=Output.</param>
        /// <param name="trisIO">Directions for IO0-IO3. 0=Input, 1=Output.</param>
        /// <param name="stateD">Output states for D0-D15. Returns states of D0-D15.</param>
        /// <param name="stateIO">Output states for IO0-IO3. Returns states of IO0-IO3.</param>
        /// <param name="updateDigital">If &gt;0, tris and state values will be written. Otherwise, just a read is performed.</param>
        /// <param name="resetCounter">If &gt;0, the counter is reset to zero after being read.</param>
        /// <param name="count">Current value of the 32-bit counter (CNT). This value is read before the counter is reset.</param>
        /// <param name="analogOut0">Voltage from 0.0 to 5.0 for AO0.</param>
        /// <param name="analogOut1">Voltage from 0.0 to 5.0 for AO1.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        /// <remarks>
        /// If either passed voltage is less than zero, the DLL uses the last set voltage. This provides a way to update 1 output without changing the other.
        /// Note that when the DLL is first loaded, it does not know if the analog outputs have been set, and assumes they are both the default of 0.0 volts.
        /// Similarly, there are situations where the LabJack could reset without the knowledge of the DLL, and thus the DLL could think the analog outputs
        /// are set to a non-zero voltage when in fact they have been reinitialized to 0.0 volts.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int AOUpdate(ref int idnum, int demo, int trisD, int trisIO, ref int stateD, ref int stateIO, int updateDigital, int resetCounter, ref uint count, float analogOut0, float analogOut1);

        /// <summary>
        /// Converts a 12-bit (0-4095) binary value into a LabJack voltage. No hardware communication is involved.
        /// </summary>
        /// <param name="chnum">Channel index. 0-7=SE, 8-11=Diff.</param>
        /// <param name="chgain">Gain index. 0=1, 1=2, …, 7=20.</param>
        /// <param name="bits">Binary value from 0-4095.</param>
        /// <param name="volts">Returns voltage.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Volts=((2*Bits*Vmax/4096)-Vmax)/Gain where Vmax=10 for SE, 20 for Diff.</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int BitsToVolts(int chnum, int chgain, int bits, ref float volts);

        /// <summary>
        /// Converts a voltage to it's 12-bit (0-4095) binary representation. No hardware communication is involved.
        /// </summary>
        /// <param name="chnum">Channel index. 0-7=SE, 8-11=Diff.</param>
        /// <param name="chgain">Gain index. 0=1, 1=2, …, 7=20.</param>
        /// <param name="volts">Voltage.</param>
        /// <param name="bits">Returns binary value from 0-4095.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Bits=(4096*((Volts*Gain)+Vmax))/(2*Vmax) where Vmax=10 for SE, 20 for Diff.</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int VoltsToBits(int chnum, int chgain, float volts, ref int bits);

        /// <summary>
        /// Controls and reads the counter. The counter is disabled if the watchdog timer is enabled.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="stateD">Returns states of D0-D15.</param>
        /// <param name="stateIO">Returns states of IO0-IO3.</param>
        /// <param name="resetCounter">If &gt;0, the counter is reset to zero after being read.</param>
        /// <param name="enableSTB">If &gt;0, STB is enabled. Used for testing and calibration. (This input has no effect with firmware V1.02 or earlier, in which case STB is always enabled)</param>
        /// <param name="count">Returns current value of the 32-bit counter (CNT). This value is read before the counter is reset.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int Counter(ref int idnum, int demo, ref int stateD, ref int stateIO, int resetCounter, int enableSTB, ref uint count);

        /// <summary>
        /// Reads and writes to all 20 digital I/O.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="trisD">Directions for D0-D15. 0=Input, 1=Output. Returns a read of the direction registers for D0-D15.</param>
        /// <param name="trisIO">Directions for IO0-IO3. 0=Input, 1=Output.</param>
        /// <param name="stateD">Output states for D0-D15. Returns states for D0-D15.</param>
        /// <param name="stateIO">Output states for IO0-IO3. Returns states for IO0-IO3.</param>
        /// <param name="updateDigital">If &gt;0, tris and state values will be written. Otherwise, just a read is performed.</param>
        /// <param name="outputD">Returns a read of the output registers for D0-D15.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int DigitalIO(ref int idnum, int demo, ref int trisD, int trisIO, ref int stateD, ref int stateIO, int updateDigital, ref int outputD);

        /// <summary>
        /// Returns the version number of ljackuw.dll. No hardware communication is involved.
        /// </summary>
        /// <returns>Version number of ljackuw.dll.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern float GetDriverVersion();

        /// <summary>
        /// Converts a LabJack errorcode, returned by another function, into a string describing the error. No hardware communication is involved.
        /// </summary>
        /// <param name="errorcode">LabJack errorcode.</param>
        /// <param name="errorString">Pointer to a 50 element array of characters. Returns a sequence of characters describing the error. Unused locations are filled with 0x00.</param>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern void GetErrorString(int errorcode, [In, Out] char[] errorString);

        /// <summary>
        /// Retrieves the firmware version from the LabJack’s processor.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found. If error, returns 512 plus a normal LabJack errorcode.</param>
        /// <returns>Version number of the LabJack firmware or 0 for error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern float GetFirmwareVersion(ref int idnum);

        /// <summary>
        /// Uses a Windows API function to get the OS version.
        /// </summary>
        /// <param name="majorVersion">Returns Windows Major Version</param>
        /// <param name="minorVersion">Returns Windows Minor Version</param>
        /// <param name="buildNumber">Returns Windows Build Number</param>
        /// <param name="platformID">Returns Windows Platform ID</param>
        /// <param name="servicePackMajor">Returns Windows Service Pack Major Version</param>
        /// <param name="servicePackMinor">Returns Windows Service Pack Minor Version</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>
        ///                     Platform  Major  Minor  Build
        /// Windows 3.1         0         -      -      -
        /// Windows 95          1         4      0      950
        /// Windows 95 OSR2     1         4      0      1111
        /// Windows 98          1         4      10     1998
        /// Windows 98SE        1         4      10     2222
        /// Windows Me          1         4      90     3000
        /// Windows NT 3.51     2         3      51     -
        /// Windows NT 4.0      2         4      0      1381
        /// Windows 2000        2         5      0      2195
        /// Windows XP          2         5      1      -
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int GetWinVersion(ref uint majorVersion, ref uint minorVersion, ref uint buildNumber, ref uint platformID, ref uint servicePackMajor, ref uint servicePackMinor);

        /// <summary>
        /// Changes the local ID of a specified LabJack. Changes will not take effect until the LabJack is re-enumerated or reset, either manually by disconnecting and reconnecting the USB cable or by calling ReEnum or Reset.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="localID">New local ID.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int LocalID(ref int idnum, int localID);

        /// <summary>
        /// This function is needed when interfacing TestPoint to the LabJack DLL on Windows 98/ME (see ljackuw.h for more information). Call this function to disable/enable thread creation for other functions. Normally, thread creation should be enabled, but it must be disabled for LabJack functions to work when called from TestPoint. One other situation where disabling thread creation might be useful, is when running a time-critical application in the Visual C debugger. Slow thread creation is a known problem with the Visual C debugger.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="noThread">If &gt;0, the thread will not be used.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is about 80 milliseconds.</remarks>
        /// <remarks>If the read thread is disabled, the "timeout" specified in AIBurst and AIStreamRead is also disabled.</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int NoThread(ref int idnum, int noThread);

        /// <summary>
        /// Requires firmware V1.1 or higher. This command creates pulses on any/all of D0-D7. The desired D lines must be set to output using another function (DigitalIO or AOUpdate). All selected lines are pulsed at the same time, at the same rate, for the same number of pulses.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="lowFirst">If &gt;0, each line is set low then high, otherwise the lines are set high then low.</param>
        /// <param name="bitSelect">Set bits 0 to 7 to enable pulsing on each of D0-D7 (0-255).</param>
        /// <param name="numPulses">Number of pulses for all lines (1-32767).</param>
        /// <param name="timeB1">B value for first half cycle (1-255).</param>
        /// <param name="timeC1">C value for first half cycle (1-255).</param>
        /// <param name="timeB2">B value for second half cycle (1-255).</param>
        /// <param name="timeC2">C value for second half cycle (1-255).</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is about 20 milliseconds plus pulse output time.</remarks>
        /// <remarks>
        /// This function commands the time for the first half cycle of each pulse, and the second half cycle of each pulse. Each time is commanded by sending a value B &amp; C, where the time is,
        ///      1st half-cycle microseconds = ~17 + 0.83*C + 20.17*B*C,
        ///      2nd half-cycle microseconds = ~12 + 0.83*C + 20.17*B*C,
        /// which can be approximated as,
        ///      microseconds = 20*B*C.
        /// For best accuracy when using the approximation, minimize C. B and C must be between 1 and 255, so each half cycle can vary from about 38/33 microseconds to just over 1.3 seconds.
        /// If you have enabled the LabJack Watchdog function, make sure it's timeout is longer than the time it takes to output all pulses.
        /// The timeout of this function, in milliseconds, is set to:
        ///      5000+numPulses*((B1*C1*0.02)+(B2*C2*0.02))
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int PulseOut(ref int idnum, int demo, int lowFirst, int bitSelect, int numPulses, int timeB1, int timeC1, int timeB2, int timeC2);

        /// <summary>
        /// Requires firmware V1.1 or higher. PulseOutStart and PulseOutFinish are used as an alternative to PulseOut (See PulseOut for more information). PulseOutStart starts the pulse output and returns without waiting for the finish. PulseOutFinish waits for the LabJack's response which signifies the end of the pulse output.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="lowFirst">If &gt;0, each line is set low then high, otherwise the lines are set high then low.</param>
        /// <param name="bitSelect">Set bits 0 to 7 to enable pulsing on each of D0-D7 (0-255).</param>
        /// <param name="numPulses">Number of pulses for all lines (1-32767).</param>
        /// <param name="timeB1">B value for first half cycle (1-255).</param>
        /// <param name="timeC1">C value for first half cycle (1-255).</param>
        /// <param name="timeB2">B value for second half cycle (1-255).</param>
        /// <param name="timeC2">C value for second half cycle (1-255).</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function about 10 milliseconds.</remarks>
        /// <remarks>
        /// If anything besides PulseOutFinish is called after PulseOutStart, the pulse output will be terminated and the LabJack will execute the new command. Calling PulseOutStart repeatedly, before the previous pulse output has finished, provides a pretty good approximation of continuous pulse output.
        /// Note that due to boot-up tests on the LabJack U12, if PulseOutStart is the first command sent to the LabJack after reset or power-up, there will be no response for PulseOutFinish. In practice, even if no precautions were taken, this would probably never happen, since before calling PulseOutStart, a call is needed to set the desired D lines to output.
        /// Also note that PulseOutFinish must be called before the LabJack completes the pulse output to read the response. If PulseOutFinish is not called until after the LabJack sends it's response, the function will never receive the response and will timeout.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int PulseOutStart(ref int idnum, int demo, int lowFirst, int bitSelect, int numPulses, int timeB1, int timeC1, int timeB2, int timeC2);

        /// <summary>
        /// Requires firmware V1.1 or higher. See PulseOutStart for more information.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="timeoutMS">Amount of time, in milliseconds, that this function will wait for the PulseOutStart response.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int PulseOutFinish(ref int idnum, int demo, int timeoutMS);

        /// <summary>
        /// Requires firmware V1.1 or higher. This function can be used to calculate the cycle times for PulseOut or PulseOutStart.
        /// </summary>
        /// <param name="frequency">Desired frequency in Hz. Returns actual best frequency found in Hz.</param>
        /// <param name="timeB">B value for first and second half cycle.</param>
        /// <param name="timeC">C value for first and second half cycle.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int PulseOutCalc(ref float frequency, ref int timeB, ref int timeC);

        /// <summary>
        /// Causes the LabJack to electrically detach from and re-attach to the USB so it will re-enumerate. The local ID and calibration constants are updated at this time.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int ReEnum(ref int idnum);

        /// <summary>
        /// Causes the LabJack to reset after about 2 seconds. After resetting the LabJack will re-enumerate. Reset and ResetLJ are identical.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int Reset(ref int idnum);

        /// <summary>
        /// Causes the LabJack to reset after about 2 seconds. After resetting the LabJack will re-enumerate. Reset and ResetLJ are identical.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int ResetLJ(ref int idnum);

        /// <summary>
        /// This function retrieves temperature and/or humidity readings from an SHT1X sensor. Data rate is about 2 kbps with firmware V1.1 or higher (hardware communication). If firmware is less than V1.1, or TRUE is passed for softComm, data rate is about 20 bps.
        /// DATA = IO0, SCK = IO1
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="softComm">If &gt;0, forces software based communication. Otherwise software communication is only used if the LabJack U12 firmware version is less than V1.1.</param>
        /// <param name="mode">0=temp and RH,1=temp only,2=RH only. If mode is 2, the current temperature must be passed in for the RH corrections using *tempC.</param>
        /// <param name="statusReg">Current value of the SHT1X status register. The value of the status register is 0 unless you have used advanced functions to write to the status register (enabled heater, low resolution, or no reload from OTP).</param>
        /// <param name="tempC">Returns temperature in degrees C. If mode is 2, the current temperature must be passed in for the RH corrections.</param>
        /// <param name="tempF">Returns temperature in degrees F.</param>
        /// <param name="rh">Returns RH in percent.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>
        /// DATA = IO0
        /// SCK = IO1
        /// The EI-1050 has an extra enable line that allows multiple probes to be connected at the same time using only the one line for DATA and one line for SCK. This function does not control the enable line.
        /// This function automatically configures IO0 has an input and IO1 as an output.
        /// Note that internally this function operates on the state and direction of IO0 and IO1, and to operate on any of the IO lines the LabJack must operate on all 4. The DLL keeps track of the current direction and output state of all lines, so that this function can operate on IO0 and IO1 without changing IO2 and IO3. When the DLL is first loaded, though, it does not know the direction and state of the lines and assumes all directions are input and output states are low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int SHT1X(ref int idnum, int demo, int softComm, int mode, int statusReg, ref float tempC, ref float tempF, ref float rh);

        /// <summary>
        /// Low-level public function to send and receive up to 4 bytes to from an SHT1X sensor. Data rate is about 2 kbps with firmware V1.1 or higher (hardware communication). If firmware is less than V1.1, or TRUE is passed for softComm, data rate is about 20 bps.
        /// DATA = IO0, SCK = IO1
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="softComm">If &gt;0, forces software based communication. Otherwise software communication is only used if the LabJack U12 firmware version is less than V1.1.</param>
        /// <param name="waitMeas">If &gt;0, this is a T or RH measurement request.</param>
        /// <param name="serialReset">If &gt;0, a serial reset is issued before sending and receiving bytes.</param>
        /// <param name="dataRate">0=no extra delay (default), 1=medium delay, 2=max delay.</param>
        /// <param name="numWrite">Number of bytes to write (0-4).</param>
        /// <param name="numRead">Number of bytes to read (0-4).</param>
        /// <param name="datatx">Array of 0-4 bytes to send. Make sure you pass at least numWrite number of bytes.</param>
        /// <param name="datarx">Returns 0-4 read bytes as determined by numRead.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>
        /// DATA = IO0
        /// SCK = IO1
        /// The EI-1050 has an extra enable line that allows multiple probes to be connected at the same time using only the one line for DATA and one line for SCK. This function does not control the enable line.
        /// This function automatically configures IO0 has an input and IO1 as an output.
        /// Note that internally this function operates on the state and direction of IO0 and IO1, and to operate on any of the IO lines the LabJack must operate on all 4. The DLL keeps track of the current direction and output state of all lines, so that this function can operate on IO0 and IO1 without changing IO2 and IO3. When the DLL is first loaded, though, it does not know the direction and state of the lines and assumes all directions are input and output states are low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int SHTComm(ref int idnum, int softComm, int waitMeas, int serialReset, int dataRate, int numWrite, int numRead, [In, Out] byte[] datatx, [In, Out] byte[] datarx);

        /// <summary>
        /// Checks the CRC on an SHT1X communication. Last byte of datarx is the CRC. Returns 0 if CRC is good, or SHT1X_CRC_ERROR_LJ if CRC is bad.
        /// </summary>
        /// <param name="statusReg">Current value of the SHT1X status register.</param>
        /// <param name="numWrite">Number of bytes that were written (0-4).</param>
        /// <param name="numRead">Number of bytes that were read (1-4).</param>
        /// <param name="datatx">Array of 0-4 bytes that were sent.</param>
        /// <param name="datarx">Array of 1-4 bytes that were read.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int SHTCRC(int statusReg, int numWrite, int numRead, [In, Out] byte[] datatx, [In, Out] byte[] datarx);

        /// <summary>
        /// Requires firmware V1.1 or higher. This function performs SPI communication. Data rate is about 160 kbps with no extra delay, although delays of 100 us or 1 ms per bit can be enabled.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="mode">Specify SPI mode as: 0=A,1=B,2=C,3=D (0-3).</param>
        /// <param name="msDelay">If &gt;0, a 1 ms delay is added between each bit.</param>
        /// <param name="husDelay">If &gt;0, a hundred us delay is added between each bit.</param>
        /// <param name="controlCS">If &gt;0, D0-D7 is automatically controlled as CS. The state and direction of CS is only tested if control is enabled.</param>
        /// <param name="csLine">D line to use as CS if enabled (0-7).</param>
        /// <param name="csState">Active state for CS line. This would be 0 for the normal !CS, or &gt;0 for the less common CS.</param>
        /// <param name="configD">If &gt;0, state and tris are configured for D13, D14, D15, and !CS.</param>
        /// <param name="numWriteRead">Number of bytes to write and read (1-18).</param>
        /// <param name="data">Serial data buffer. Send an 18 element array of bytes. Fill unused locations with zeros. Returns any serial read data, with unused locations filled with 9999s.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is about 20 milliseconds to write and/or read up to 4 bytes, plus about 20 milliseconds for each additional 4 bytes written or read. Extra 20 milliseconds if configD is true, and extra time if delays are enabled.</remarks>
        /// <remarks>
        /// Control of CS (chip select) can be enabled in this function for D0-D7 or handled externally via any digital output.
        ///     MOSI is D13
        ///     MISO is D14
        ///     SCK is D15
        /// If using the CB25, the protection resistors might need to be shorted on all SPI connections (MOSI, MISO, SCK, CS).
        /// The initial state of SCK is set properly (CPOL), by this function, before !CS is brought low (final state is also set properly before !CS is brought high again). If chip-select is being handled manually, outside of this function, care must be taken to make sure SCK is initially set to CPOL.
        /// All modes supported (A, B, C, and D).
        ///     Mode A: CPHA=1, CPOL=1
        ///     Mode B: CPHA=1, CPOL=0
        ///     Mode C: CPHA=0, CPOL=1
        ///     Mode D: CPHA=0, CPOL=0
        /// If Clock Phase (CPHA) is 1, data is valid on the edge going to CPOL. If CPHA is 0, data is valid on the edge going away from CPOL. Clock Polarity (CPOL) determines the idle state of SCK.
        /// Up to 18 bytes can be written/read. Communication is full duplex so 1 byte is read at the same time each byte is written. If more than 4 bytes are written or read, this function uses calls to WriteMem/ReadMem to load/read the LabJack's data buffer.
        /// This function has the option (configD) to automatically configure default state and direction for MOSI (D13 Output), MISO (D14 Input), SCK (D15 Output CPOL), and CS (D0-D7 Output High for !CS). This function uses a call to DigitalIO to do this. Similar to EDigitalIn and EDigitalOut, the DLL keeps track of the current direction and output state of all lines, so that these 4 D lines can be configured without affecting other digital lines. When the DLL is first loaded, though, it does not know the direction and state of the lines and assumes all directions are input and output states are low.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int Synch(ref int idnum, int demo, int mode, int msDelay, int husDelay, int controlCS, int csLine, int csState, int configD, int numWriteRead, [In, Out] int[] data);

        /// <summary>
        /// Controls the LabJack watchdog function. When activated, the watchdog can change the states of digital I/O if the LabJack does not successfully communicate with the PC within a specified timeout period. This function could be used to reboot the PC allowing for reliable unattended operation. The 32-bit counter (CNT) is disabled when the watchdog is enabled.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="demo">Send 0 for normal operation, &gt;0 for demo mode. Demo mode allows this function to be called without a LabJack.</param>
        /// <param name="active">Enables the LabJack watchdog function. If enabled, the 32-bit counter is disabled.</param>
        /// <param name="timeout">Timer reset value in seconds (1-715).</param>
        /// <param name="reset">If &gt;0, the LabJack will reset on timeout.</param>
        /// <param name="activeD0">If &gt;0, D0 will be set to stateD0 upon timeout.</param>
        /// <param name="activeD1">If &gt;0, D1 will be set to stateD1 upon timeout.</param>
        /// <param name="activeD8">If &gt;0, D8 will be set to stateD8 upon timeout.</param>
        /// <param name="stateD0">Timeout state of D0, 0=low, &gt;0=high.</param>
        /// <param name="stateD1">Timeout state of D1, 0=low, &gt;0=high.</param>
        /// <param name="stateD8">Timeout state of D8, 0=low, &gt;0=high.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        /// <remarks>
        /// If you set the watchdog to reset the LabJack, and choose too small of a timeout period, it might be difficult to make the device stop resetting. To disable the watchdog, reset the LabJack with IO0 shorted to STB, and then reset again without the short.
        /// </remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int Watchdog(ref int idnum, int demo, int active, int timeout, int reset, int activeD0, int activeD1, int activeD8, int stateD0, int stateD1, int stateD8);

        /// <summary>
        /// Reads 4 bytes from a specified address in the LabJack's nonvolatile memory.
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="address">Starting address of data to read (0-8188).</param>
        /// <param name="data3">Byte at address.</param>
        /// <param name="data2">Byte at address+1.</param>
        /// <param name="data1">Byte at address+2.</param>
        /// <param name="data0">Byte at address+3.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int ReadMem(ref int idnum, int address, ref int data3, ref int data2, ref int data1, ref int data0);

        /// <summary>
        /// Writes 4 bytes to the LabJack's 8,192 byte nonvolatile memory at a specified address. The data is read back and verified after the write. Memory 0-511 is reserved for configuration and calibration data. Memory from 512-1023 is unused by the LabJack and available for the user (this corresponds to starting addresses from 512-1020). Memory 1024-8191 is used as a data buffer in hardware timed AI modes (burst and stream).
        /// </summary>
        /// <param name="idnum">Local ID, serial number, or -1 for first found. Returns the local ID or –1 if no LabJack is found.</param>
        /// <param name="unlocked">If &gt;0, addresses 0-511 are unlocked for writing.</param>
        /// <param name="address">Starting address for writing (0-8188).</param>
        /// <param name="data3">Byte at address.</param>
        /// <param name="data2">Byte at address+1.</param>
        /// <param name="data1">Byte at address+2.</param>
        /// <param name="data0">Byte at address+3.</param>
        /// <returns>LabJack errorcodes or 0 for no error.</returns>
        /// <remarks>Execution time for this function is 20 milliseconds or less (typically 16 milliseconds in Windows).</remarks>
        [DllImport("ljackuw.dll", CharSet = CharSet.Ansi)]
        public static extern int WriteMem(ref int idnum, int unlocked, int address, int data3, int data2, int data1, int data0);
    }
}
