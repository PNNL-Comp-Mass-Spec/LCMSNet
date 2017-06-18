using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    [classDeviceControlAttribute(null,
        typeof(FluidicsSprayNeedleWpf),
        "Spray Needle",
        "Fluidics Components")]
    public class SprayNeedleWpf : FluidicsComponentBase
    {
        public SprayNeedleWpf()
        {
            Name = "Spray Needle";
            Version = "infinity.";
            Position = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }

        public int Position { get; set; }
    }
}