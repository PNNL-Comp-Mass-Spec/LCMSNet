using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    [DeviceControl(null,
        typeof(FluidicsWasteComponent),
        "Waste",
        "Fluidics Components")]
    public class WasteComponent : FluidicsComponentBase
    {

        public WasteComponent()
        {
            Name = "Waste";
            Position = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }

        public int Position { get; set; }
    }
}
