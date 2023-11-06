using System;
using System.Collections.Generic;
using System.IO.Ports;
using FluidicsSDK.Base;

namespace LcmsNetPlugins.VICI.Valves
{
    internal interface IValveConnection
    {
        string PortName { get; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }

        /// <summary>
        /// Open the serial port connection
        /// </summary>
        /// <param name="errorMessage">Connection error message</param>
        /// <returns></returns>
        ValveErrors Open(out string errorMessage);

        /// <summary>
        /// Open the serial port connection, do not internally handle exceptions
        /// </summary>
        void OpenTest();

        /// <summary>
        /// Close the serial port connection, do not internally handle exceptions
        /// </summary>
        void CloseTest();

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <returns></returns>
        ValveErrors SendCommand(string command, char id);

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        ValveErrors ReadCommand(string command, char id, out string returnData, int readDelayMs = 0);
    }

    internal class ValveConnection : IValveConnection, IDisposable
    {
        public string PortName { get; }
        public SerialPort Port { get; }
        public object Locker { get; }
        public List<char> ActiveIDs { get; }

        public int ReadTimeout
        {
            get => Port.ReadTimeout;
            set => Port.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => Port.WriteTimeout;
            set => Port.WriteTimeout = value;
        }

        /// <summary>
        /// Create a new tracked serial connection
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="readTimeout">serial port read timeout</param>
        /// <param name="writeTimeout">serial port write timeout</param>
        public ValveConnection(string portName, int readTimeout, int writeTimeout)
        {
            PortName = portName;
            Locker = new object();
            ActiveIDs = new List<char>();

            //     Baud Rate   9600
            //     Parity      None
            //     Stop Bits   One
            //     Data Bits   8
            //     Handshake   None
            Port = new SerialPort()
            {
                PortName = PortName,
                BaudRate = 9600,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                Parity = Parity.None,
                ReadTimeout = readTimeout,
                WriteTimeout = writeTimeout,
                NewLine = "\r"
            };
        }

        /// <summary>
        /// Create a new tracked serial connection
        /// </summary>
        /// <param name="port"></param>
        public ValveConnection(SerialPort port)
        {
            Locker = new object();
            ActiveIDs = new List<char>();
            PortName = port.PortName;
            Port = port;
        }

        public void Dispose()
        {
            Port?.Dispose();
        }

        /// <summary>
        /// Open the serial port connection
        /// </summary>
        /// <param name="errorMessage">Connection error message</param>
        /// <returns></returns>
        public ValveErrors Open(out string errorMessage)
        {
            lock (Locker)
            {
                return OpenPort(out errorMessage);
            }
        }

        public void OpenTest()
        {
            lock (Locker)
            {
                if (!Port.IsOpen)
                {
                    Port.Open();
                }
            }
        }

        public void CloseTest()
        {
            lock (Locker)
            {
                if (Port.IsOpen)
                {
                    Port.Close();
                }
            }
        }

        public ValveErrors OpenPort(out string errorMessage)
        {
            errorMessage = null;

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    errorMessage = "Could not access the COM Port.  " + ex.Message;
                    return ValveErrors.UnauthorizedAccess;
                }
            }

            return ValveErrors.Success;
        }

        public void Close()
        {
            //If the serial port is open, close it
            if (Port.IsOpen)
            {
                Port.Close();
            }
        }

        // TODO: All commands need to be prefixed with '/' when connected with RS-485!!

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <returns></returns>
        public ValveErrors SendCommand(string command, char id)
        {
            if (ActiveIDs.Count > 1)
            {
                return SendCommandLocked(command, id);
            }

            return SendCommandLocked(command, id);
        }

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        public ValveErrors ReadCommand(string command, char id, out string returnData, int readDelayMs = 0)
        {
            if (ActiveIDs.Count > 1)
            {
                return ReadCommandLocked(command, id, out returnData, readDelayMs);
            }

            return ReadCommandNoLock(command, id, out returnData, readDelayMs);
        }

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <returns></returns>
        private ValveErrors SendCommandLocked(string command, char id)
        {
            lock (Locker)
            {
                return SendCommandNoLock(command, id);
            }
        }

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        private ValveErrors ReadCommandLocked(string command, char id, out string returnData, int readDelayMs = 0)
        {
            returnData = "";

            lock (Locker)
            {
                return ReadCommandNoLock(command, id, out returnData, readDelayMs);
            }
        }

        private string ConvertID(char id)
        {
            // For valves with no set id, they expect a 'null character' at the start of the command
            // ID of ' ' (space) is considered 'no ID'
            // NOTE: Spaces are ignored by the controller in sent commands
            return id == ' ' ? "" : id.ToString();
        }

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <returns></returns>
        private ValveErrors SendCommandNoLock(string command, char id)
        {
            var id2 = ConvertID(id);

            //If the serial port is not open, open it
            var error = OpenPort(out _);
            if (error != ValveErrors.Success)
            {
                return error;
            }

            try
            {
                Port.WriteLine(id2 + command);
            }
            catch (TimeoutException)
            {
                //ApplicationLogger.LogError(0, "Could not send command.  Write timeout.");
                return ValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                //ApplicationLogger.LogError(0, "Could not send command.  Could not access serial port.");
                return ValveErrors.UnauthorizedAccess;
            }

            return ValveErrors.Success;
        }

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="id">Valve ID to pre-pend to the command</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        private ValveErrors ReadCommandNoLock(string command, char id, out string returnData, int readDelayMs = 0)
        {
            returnData = "";
            var id2 = ConvertID(id);

            //If the serial port is not open, open it
            var error = OpenPort(out _);
            if (error != ValveErrors.Success)
            {
                return error;
            }

            try
            {
                Port.DiscardInBuffer();
                Port.WriteLine(id2 + command);
                if (readDelayMs > 0)
                {
                    System.Threading.Thread.Sleep(readDelayMs);
                }
            }
            catch (TimeoutException)
            {
                return ValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return ValveErrors.UnauthorizedAccess;
            }

            try
            {
                //Read in whatever is waiting in the buffer
                returnData = Port.ReadExisting();
                if (!string.IsNullOrEmpty(returnData))
                {
                    // Valve may return string containing \r\n or \n\r; make all instances be \n
                    returnData = returnData.Replace("\r", "\n").Replace("\n\n", "\n").Trim('\n');
                }
            }
            catch (TimeoutException)
            {
                return ValveErrors.TimeoutDuringRead;
            }
            catch (UnauthorizedAccessException)
            {
                return ValveErrors.UnauthorizedAccess;
            }

            if (returnData.IndexOf("Bad command", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return ValveErrors.BadCommand;
            }

            return ValveErrors.Success;
        }
    }
}
