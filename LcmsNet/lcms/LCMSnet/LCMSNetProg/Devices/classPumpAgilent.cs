using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;

using Agilent.Licop;

namespace LcmsNet.Devices.Pumps
{
    //TODO: Can I pick up events from the pump? Do I need to?
    public class classPumpAgilent : IDevice
    {
        #region Members

        private Instrument mobj_pumps;
        private Module mobj_module;     //I dunno either.
        private Channel mobj_evChannel; //Yeah, what?
        private Channel mobj_inChannel; //...
        private string mstr_address;
        private string mstr_name;
        private string mstr_version;
        private System.IO.Ports.SerialPort mobj_port;
        private bool mbool_running;

        #endregion

        #region Events

        /*//Save required
        public event DelegateDeviceSaveRequired DeviceSaveRequired;
        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this);
            }
        }*/

        #endregion

        #region Properties

        public LcmsNetDataClasses.Devices.enumDeviceStatus Status
        {
            get
            {
                return LcmsNetDataClasses.Devices.enumDeviceStatus.Disabled;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the serial port to use for communicating with the pumps.
        /// </summary>
        public System.IO.Ports.SerialPort Port
        {
            get
            {
                return mobj_port;
            }
            set
            {
                Shutdown();
                mobj_port = value;
                Initialize();
            }
        }

        public bool Running
        {
            get
            {
                return mbool_running;
            }
            set
            {
                mbool_running = value;
            }
        }

        public string Name
        {
            get
            {
                return mstr_name;
            }
            set
            {
                mstr_name = value;
            }
        }

        public string Version
        {
            get
            {
                return mstr_version;
            }
            set
            {
                mstr_version = value;
            }
        }

        #endregion

        #region Methods

        public bool Initialize()
        {
            //TODO: Don't hardcode this
            mobj_port = new System.IO.Ports.SerialPort("COM1");
            mobj_pumps = new Instrument(6000, 6000);
                //TODO: Do I need these?
            //mobj_pumps.ConfigurationChanged += new EventHandler<ConfigurationEventArgs>(ConfigurationChanged);
            //mobj_pumps.ErrorOccurred += new EventHandler<ErrorEventArgs>(ErrorOccurred);
                
            mstr_address = mobj_port.PortName;
            
            //Try initial connection
            if (mobj_pumps.TryConnect(mstr_address, 5000) == false)
            {
                //Could not connect
                return false;
            }

            //What the heck is a module anyway?
            mobj_module = mobj_pumps.CreateModule(mobj_pumps.GetAccessPointIdentifier());

            //And an EV channel. What is that?
            mobj_evChannel = mobj_module.CreateChannel("EV");
            //TODO: Do I need this?
            //evChannel.DataReceived += new EventHandler<DataEventArgs>(DataReceived);
            if (mobj_evChannel.TryOpen(ReadMode.Events) == false)
            {
                //"Could not open EV channel."
                return false;
            }

            //I really don't know what any of this is.
            mobj_inChannel = mobj_module.CreateChannel("IN");
            if (mobj_inChannel.TryOpen(ReadMode.Polling) == false)
            {
                //"Could not open IN channel."
                return false;
            }

            //If we made it this far, great success!
            return true;
        }

        public bool Shutdown()
        {
            // close EV channel
            //TODO: Do I need this?
            //mobj_evChannel.DataReceived -= DataReceived;
            mobj_evChannel.Close();

            // close IN channel
            mobj_inChannel.Close();

            // disconnect
            //TODO: Do I need these?
            //mobj_pumps.ErrorOccurred -= ErrorOccurred;
            //mobj_pumps.ConfigurationChanged -= ConfigurationChanged;
            mobj_pumps.Disconnect();

            return true;
        }

        private bool SendCommand(string command, ref string reply, string errorstring)
        {
            //Send the command over our serial port
            //TODO: Wrap this in exception checking
            //      (if there is an error, send out errorstring)
            if (mobj_inChannel.TryWrite(command, 10000) == false)
            {
                //Couldn't send instruction
                return false;
            }            
            if (mobj_inChannel.TryRead(out reply, 10000) == false)
            {
                //Couldn't read reply
                return false;
            }

            return true;
        }

        public void SetFlowRate(double newFlowRate)
        {
            string reply = "";
            SendCommand("FLOW " + newFlowRate.ToString(),ref reply, "Attempting to set flow rate to " + newFlowRate.ToString());
        }

        public double GetFlowRate()
        {
            string reply = "";
            SendCommand("FLOW?", ref reply, "Attempting to query flow rate.");
            //We expect something like:
            //reply = "RA 0000 FLOW 2.000";
            int start = reply.IndexOf("FLOW");
            if (start == -1)
            {
                //TODO: Error!
                return -1;
            }
            reply = reply.Substring(start + 5, 5);
            return Convert.ToDouble(reply);
        }

        public double GetPressure()
        {
            string reply = "";
            SendCommand("ACT:PRES?", ref reply, "Attmempting to query pressure.");
            //expect: RA 0000 ACT:PRES 16.00
            int start = reply.IndexOf("ACT:PRES");
            if (start == -1)
            {
                //TODO: Error!
                return -1;
            }
            reply = reply.Substring(start + 9, 5);
            return Convert.ToDouble(reply);
        }

        public void SetMode(enumPumpAgilentModes newMode)
        {
            string reply = "";
            SendCommand("MODE " + (int)newMode, ref reply, "Attempting to set mode to " + newMode.ToString());
        }

        public enumPumpAgilentModes GetMode()
        {
            string reply = "";
            SendCommand("MODE?", ref reply, "Attempting to query mode.");
            //reply = "RA 000 MODE 1
            int start = reply.IndexOf("MODE");
            if (start == -1)
            {
                //TODO: Error!
                return enumPumpAgilentModes.Unknown;
            }
            reply = reply.Substring(start + 5, 1);
            return (enumPumpAgilentModes)(Convert.ToInt32(reply));
        }

        //TODO: Do I want htis to be a double or an int?
        public void SetMixerVolume(double newVolumeuL)
        {
            string reply = "";
            if (newVolumeuL >= 0 && newVolumeuL <= 2000)
                SendCommand("MVOL " + newVolumeuL.ToString(), ref reply, "Attempting to set mixer volume to " + newVolumeuL.ToString());
        }

        public double GetMixerVolume()
        {
            string reply = "";
            SendCommand("MVOL?", ref reply, "Attempting to query mixer volume.");
            int start = reply.IndexOf("MVOL");
            if (start == -1)
            {
                //TODO: Error!
                return -1;
            }
            reply = reply.Substring(start + 5, reply.Length - (start + 5));
            return Convert.ToDouble(reply);
        }

        //TODO: Do I need to be able to set/get purge options?

        public double GetActualFlow()
        {
            string reply = "";
            SendCommand("ACT:FLOW?", ref reply, "Attempting to query actual flow.");
            int start = reply.IndexOf("ACT:FLOW");
            if (start == -1)
            {
                //TODO: Error!
                return -1;
            }
            reply = reply.Substring(start + 9, 5);
            return Convert.ToDouble(reply);
        }


        #endregion
    }
}
