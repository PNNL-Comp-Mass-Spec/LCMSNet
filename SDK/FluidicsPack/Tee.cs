using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    [DeviceControl(null,
        typeof(FluidicsTee),
        "Tee",
        "Fluidics Components")]
    public class Tee : FluidicsComponentBase
    {
        public Tee()
        {
            Name = "Tee";
        }
    }
}
