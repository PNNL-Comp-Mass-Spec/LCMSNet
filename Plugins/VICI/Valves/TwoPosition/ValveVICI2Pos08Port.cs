using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
{
    [Serializable]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
        "Eight-Port",
        "Valves Two-Position")
    ]
    public class ValveVICI2Pos08Port : ValveVICI2Pos, IEightPortValve
    {
        public ValveVICI2Pos08Port()
            : base()
        {
        }

        public ValveVICI2Pos08Port(SerialPort port)
            : base(port)
        {
        }
    }
}