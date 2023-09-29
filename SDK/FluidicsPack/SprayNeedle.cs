using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    [DeviceControl(null,
        typeof(FluidicsSprayNeedle),
        "Spray Needle",
        "Fluidics Components")]
    public class SprayNeedle : FluidicsComponentBase
    {
        public SprayNeedle()
        {
            Name = "Spray Needle";
            Position = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }

        public int Position { get; set; }
    }
}
