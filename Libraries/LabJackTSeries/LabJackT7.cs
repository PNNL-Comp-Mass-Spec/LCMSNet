using LabJack;
using System;

namespace LabJackTSeries
{
    /// <summary>
    /// LabJack instance that will only connect to a LabJack T7
    /// </summary>
    public class LabJackT7 : LabJackT
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabJackT7() : base(LJM.CONSTANTS.dtT7)
        {
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="identifier">The LabJack's local ID</param>
        public LabJackT7(string identifier) : base(LJM.CONSTANTS.dtT7, identifier)
        {
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value, or -1 if a problem</returns>
        public double Read(LabJackT7Inputs channel)
        {
            var portName = Enum.GetName(typeof(LabJackT7Inputs), channel);
            if (portName == null)
            {
                return -1;
            }

            return ReadRegisterByName((LabJackT7IONames)(int)channel);
        }

        /// <summary>
        /// General method for reading from a port, using the extended feature inputs
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value, or -1 if a problem</returns>
        public double Read(LabJackT7ExtendFeatureInputs channel)
        {
            var portName = Enum.GetName(typeof(LabJackT7ExtendFeatureInputs), channel);
            if (portName == null)
            {
                return -1;
            }

            return ReadRegisterByName((LabJackT7IONames)(int)channel);
        }

        /// <summary>
        /// Reads the current state/voltage of one of the inputs
        /// </summary>
        /// <param name="register">The register to read from</param>
        /// <returns>The state (digital, 0/1) or voltage (analog) of the channel</returns>
        protected double ReadRegisterByName(LabJackT7IONames register)
        {
            var registerName = Enum.GetName(typeof(LabJackT7IONames), register);
            if (registerName == null)
            {
                return -1;
            }

            return ReadRegisterByName(registerName);
        }

        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="name">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(LabJackT7Outputs name, double value)
        {
            var portName = Enum.GetName(typeof(LabJackT7Outputs), name);
            if (portName == null)
                return;

            WriteRegisterByName((LabJackT7IONames)(int)name, value);
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="register">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        protected int WriteRegisterByName(LabJackT7IONames register, double value)
        {
            var registerName = Enum.GetName(typeof(LabJackT7IONames), register);
            if (registerName == null)
            {
                return -1;
            }

            return WriteRegisterByName(registerName, value);
        }
    }
}
