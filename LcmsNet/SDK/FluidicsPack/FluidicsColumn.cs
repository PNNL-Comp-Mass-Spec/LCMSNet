using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]	
    [classDeviceControlAttribute(typeof(FluidicsColumnControl),
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