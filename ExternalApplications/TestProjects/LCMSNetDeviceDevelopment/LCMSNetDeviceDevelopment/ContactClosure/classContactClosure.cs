using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNet.Devices.ContactClosure
{
    public class classContactClosure
    {
        #region Members

        /// <summary>
        /// The labjack used for signalling the pulse
        /// </summary>
        private classLabjackU12 mobj_labjack;
        private enumLabjackU12OutputPorts mobj_port;
        

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor--no labjack assigned!
        /// </summary>
        public classContactClosure()
        {
            mobj_labjack = new classLabjackU12();
            mobj_port = enumLabjackU12OutputPorts.AO0;
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public classContactClosure(classLabjackU12 lj)
        {
            mobj_labjack = lj;
            mobj_port = enumLabjackU12OutputPorts.AO0;
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosure(enumLabjackU12OutputPorts newPort)
        {
            mobj_labjack = new classLabjackU12();
            mobj_port = newPort;
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosure(classLabjackU12 lj, enumLabjackU12OutputPorts newPort)
        {
            mobj_labjack = lj;
            mobj_port = newPort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the labjack associated with the contact closure signal
        /// </summary>
        public classLabjackU12 labjack
        {
            get
            {
                return mobj_labjack;
            }
            set
            {
                mobj_labjack = value;
            }
        }

        /// <summary>
        /// The port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        public enumLabjackU12OutputPorts port
        {
            get
            {
                return mobj_port;
            }
            set
            {
                mobj_port = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        public int Trigger(int pulseLengthMS)
        {
            string tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), mobj_port).ToString();

            int error = 0;

            try
            {
                if (tempPortName[0] == 'A')
                {
                    mobj_labjack.Write(mobj_port, 5);
                }
                else
                {
                    mobj_labjack.Write(mobj_port, 1);
                }
            }
            catch (classLabjackU12Exception)
            {
                error = 1;
            }

            System.Threading.Thread.Sleep(pulseLengthMS);

            try
            {
                mobj_labjack.Write(mobj_port, 0);
            }
            catch (classLabjackU12Exception)
            {
                error = 1;
            }

            return error;
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        /// <param name="voltage">The voltage to set</param>
        public int Trigger(int pulseLengthMS, double voltage)
        {
            string tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), mobj_port).ToString();
            int error = 0;
            try
            {
                if (tempPortName[0] == 'A')
                {
                    mobj_labjack.Write(mobj_port, voltage);
                }
                else
                {
                    mobj_labjack.Write(mobj_port, 1);
                }
            }
            catch (classLabjackU12Exception)
            {
                error = 1;
            }

            System.Threading.Thread.Sleep(pulseLengthMS);

            try
            {
                mobj_labjack.Write(mobj_port, 0);
            }
            catch (classLabjackU12Exception)
            {
                error = 1;
            }

            return error;
        }

        #endregion
    }
}
