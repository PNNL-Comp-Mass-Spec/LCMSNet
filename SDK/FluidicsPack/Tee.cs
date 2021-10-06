using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    [DeviceControl(null,
        typeof(FluidicsTee),
        "Tee",
        "Fluidics Components")]
    public class Tee : FluidicsComponentBase
    {
        #region Methods
        public Tee()
        {
            Name = "Tee";
        }
        #endregion
    }
}
