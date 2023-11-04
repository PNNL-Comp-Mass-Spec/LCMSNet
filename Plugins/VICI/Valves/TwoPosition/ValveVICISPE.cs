using System;
using System.IO.Ports;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves.TwoPosition
{
    [Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [DeviceControl(typeof(ValveVICI2PosViewModel),
                                 "Six-port SPE",
                                 "Valves Two-Position")
    ]
    public class ValveVICISPE : ValveVICI2Pos, ISolidPhaseExtractor
    {
        public ValveVICISPE()
            : base()
        {
        }

        public ValveVICISPE(SerialPort port)
            : base(port)
        {
        }
    }
}
