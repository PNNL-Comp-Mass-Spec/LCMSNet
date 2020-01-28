using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves
{
    [Serializable]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Four-Port",
                                 "Valves Two-Position")
    ]
    public class ValveVICI2Pos04port: ValveVICI2Pos, IFourPortValve
    {
        public ValveVICI2Pos04port()
            : base()
        {
        }

        public ValveVICI2Pos04port(SerialPort port)
            : base(port)
        {
        }
    }
}
