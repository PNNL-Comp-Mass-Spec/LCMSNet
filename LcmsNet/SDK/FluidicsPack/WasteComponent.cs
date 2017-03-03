using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
        [classDeviceControlAttribute(null,
                                 typeof(FluidicsWasteComponent),
                                 "Waste",
                                 "Fluidics Components")]
    public class WasteComponent:FluidicsComponentBase
    {

           public WasteComponent()
           {
               Name = "Waste";
               Version = "infinity.";
               Position = 1;
               AbortEvent = new System.Threading.ManualResetEvent(false);
           }          
            public int Position { get; set; } 
    }
}
