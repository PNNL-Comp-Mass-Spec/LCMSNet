using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    [classDeviceControlAttribute(null,
        typeof(FluidicsSprayNeedle),
        "Spray Needle",
        "Fluidics Components")]
    public class SprayNeedle : FluidicsComponentBase
    {
        public SprayNeedle()
        {
            Name = "Spray Needle";
            Version = "infinity.";
            Position = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }

        public int Position { get; set; }
    }
}