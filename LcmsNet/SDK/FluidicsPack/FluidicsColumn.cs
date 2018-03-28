﻿using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    [classDeviceControl(typeof(FluidicsColumnControlViewModel),
                                 typeof(FluidicsColumnGlyph),
                                 "Column",
                                 "Fluidics Components")
    ]
    public class FluidicsColumn : FluidicsComponentBase
    {
        public FluidicsColumn()
        {
            Name = "Column";
        }

        public double InnerDiameter     { get; set; }
        public double Length            { get; set; }
        public string PackingMaterial   { get; set; }
    }
}
