using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    [classDeviceControlAttribute(null,
        typeof(FluidicsWasteComponentWpf),
        "Waste",
        "Fluidics Components")]
    public class WasteComponentWpf : FluidicsComponentBase
    {

        public WasteComponentWpf()
        {
            Name = "Waste";
            Version = "infinity.";
            Position = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }
        public int Position { get; set; }
    }
}
