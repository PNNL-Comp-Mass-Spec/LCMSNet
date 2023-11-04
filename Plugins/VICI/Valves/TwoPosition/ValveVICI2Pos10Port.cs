using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
{
    [Serializable]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Ten-Port",
                                 "Valves Two-Position")
    ]
    public class ValveVICI2Pos10Port : ValveVICI2Pos, ITenPortValve
    {
        public ValveVICI2Pos10Port()
            : base()
        {
        }

        public ValveVICI2Pos10Port(SerialPort port)
            : base(port)
        {
        }
    }
}
