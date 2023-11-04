using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
{
    [Serializable]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Four-Port",
                                 "Valves Two-Position")
    ]
    public class ValveVICI2Pos04Port: ValveVICI2Pos, IFourPortValve
    {
        public ValveVICI2Pos04Port()
            : base()
        {
        }

        public ValveVICI2Pos04Port(SerialPort port)
            : base(port)
        {
        }
    }
}
